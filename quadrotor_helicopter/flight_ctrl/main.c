#include "main.h"

static ppm_data vex_data;
static sens_history sens_data[8];
static mot_speed motor_speed;
static mot_cali motor_cali;
static heli_action copter_action;
static volatile servo_ctrl servo_data;
static PID_data yaw_pid;
static PID_data pitch_pid_a;
static PID_data pitch_pid_b;
static PID_data roll_pid_a;
static PID_data roll_pid_b;
static volatile unsigned char tx_good;
static volatile unsigned char op_mode;
static calibration main_cali;
static volatile unsigned long timer1_ovf_cnt;

#include "main_headers.h"

void low_priority_interrupts()
{
	// read interrupt flags, execute, then clear the flags
	if(bit_is_set(TIFR1, ICF1))
	{
		timer_1_input_capture(&vex_data);
		TIFR1 |= _BV(ICF1);
	}
	if(bit_is_set(TIFR1, TOV1))
	{
		timer_1_ovf(&vex_data);
		TIFR1 |= _BV(TOV1);
	}
	if(bit_is_clear(ADCSRA, ADSC))
	{
		sens_read_adc();
	}
}

void apply_calibration(calibration c)
{
	
}

void start_next_servo_pwm_period()
{
	unsigned long sum_ticks = servo_data.servo_ticks[0] + servo_data.servo_ticks[1] + servo_data.servo_ticks[2] + servo_data.servo_ticks[3] + servo_data.servo_ticks[4];

	while(servo_data.ready_to_restart == 0)
	{
		low_priority_interrupts();
	}

	unsigned int OCR1A_t = 0;

	if(sum_ticks < main_cali.servo_period_delay)
	{
		if(main_cali.servo_period_delay - sum_ticks <= 0xFFFF)
		{
			OCR1A_t = main_cali.servo_period_delay - sum_ticks;
		}
		else
		{
			OCR1A_t = 0xFFFF;
		}
	}

	if(OCR1A_t < 64)
	{
		OCR1A_t = 64;
	}

	if(bit_is_set(TIFR1, OCF1A))
	{
		TIFR1 |= _BV(OCF1A);
	}

	servo_data.period_finished = 0;
	servo_data.ready_to_restart = 0;

	OCR1A = TCNT1 + OCR1A_t;
	TIMSK1 |= _BV(OCIE1A);
}

int main()
{
	hardware_init();
	software_init();

	load_calibration(&main_cali, 0);

	op_mode = FLY_MODE;

	while(1)
	{
		low_priority_interrupts();
		if(op_mode == FLY_MODE)
		{
			if(servo_data.period_finished != 0)
			{			
				sens_data_proc();

				ppm_data ctrl_data = vex_data;

				if(vex_data.tx_good == 0)
				{
					for(unsigned char i = 0; i < 8; i++)
					{
						ctrl_data.chan_width[i] = 0;
					}
				}

				signed long target;
				signed long current;

				target = scale(ctrl_data.chan_width[yaw_ppm_chan], main_cali.spin_scale, spin_scale_multiplier);
				current = scale(sens_data[yaw_sens_chan].centered_avg, main_cali.yaw_scale, yaw_scale_multiplier);

				copter_action.yaw = PID_mv(&yaw_pid, current, target);
				
				mot_set(&motor_speed, &motor_cali, &copter_action);
				mot_apply(&motor_speed, &motor_cali);

				start_next_servo_pwm_period();
			}
		}
		else if(op_mode == POWER_OFF_MODE)
		{
		}
		else if(op_mode == OTHER_MODE)
		{
		}
	}
	return 0;
}

