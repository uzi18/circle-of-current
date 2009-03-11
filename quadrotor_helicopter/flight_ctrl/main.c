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
}

void start_next_servo_pwm_period()
{
	unsigned long sum_ticks = servo_data.servo_ticks[0] + servo_data.servo_ticks[1] + servo_data.servo_ticks[2] + servo_data.servo_ticks[3] + servo_data.servo_ticks[4];

	while(servo_data.ready_to_restart == 0)
	{
		low_priority_interrupts();
		if(bit_is_clear(ADCSRA, ADSC))
		{
			sens_read_adc();
		}
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

	unsigned int tt = TCNT1;

	OCR1A = tt + OCR1A_t;
	TIMSK1 |= _BV(OCIE1A);
}

int main()
{
	hardware_init();
	software_init();

	default_calibration(&main_cali);
	//calibrate_sensors(&main_cali);
	//calibrate_controller(&main_cali);
	//save_calibration(main_cali, 0);
	//load_calibration(&main_cali, 0);
	apply_calibration(main_cali);

	LED_1_off();
	LED_2_off();

	_delay_ms(1);

	LED_1_on();
	LED_2_on();

	while(1)
	{
		low_priority_interrupts();
		if(bit_is_clear(ADCSRA, ADSC))
		{
			sens_read_adc();
		}

		if(op_mode == FLY_MODE)
		{
			if(servo_data.period_finished != 0)
			{			
				sens_data_proc();

				ppm_data ctrl_data = vex_data;

				if(vex_data.tx_good != 2)
				{
					for(unsigned char i = 0; i < 8; i++)
					{
						ctrl_data.chan_width[i] = 0;
					}
				}

				low_priority_interrupts();

				signed long target;
				signed long current;

				target = scale(ctrl_data.chan_width[yaw_ppm_chan], main_cali.spin_scale, spin_scale_multiplier);
				current = scale(sens_data[yaw_sens_chan].centered_avg, main_cali.yaw_scale, yaw_scale_multiplier);

				copter_action.yaw = PID_mv(&yaw_pid, current, target);

				target = scale(ctrl_data.chan_width[roll_ppm_chan], main_cali.move_scale, move_scale_multiplier);
				current = scale(sens_data[lr_accel_chan].centered_avg, main_cali.fb_lr_accel_scale, fb_lr_accel_scale_multiplier);

				target = PID_mv(&roll_pid_a, current, target);
				current = scale(sens_data[roll_sens_chan].centered_avg, main_cali.roll_pitch_scale, roll_pitch_scale_multiplier);

				copter_action.roll = PID_mv(&roll_pid_b, current, target);				
					
				low_priority_interrupts();

				target = scale(ctrl_data.chan_width[pitch_ppm_chan], main_cali.move_scale, move_scale_multiplier);
				current = scale(sens_data[fb_accel_chan].centered_avg, main_cali.fb_lr_accel_scale, fb_lr_accel_scale_multiplier);

				target = PID_mv(&pitch_pid_a, current, target);
				current = scale(sens_data[pitch_sens_chan].centered_avg, main_cali.roll_pitch_scale, roll_pitch_scale_multiplier);

				copter_action.pitch = PID_mv(&pitch_pid_b, current, target);				

				copter_action.col = main_cali.hover_throttle + scale(ctrl_data.chan_width[throttle_ppm_chan], main_cali.throttle_scale, throttle_scale_multiplier);
				
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
		else if(op_mode == TEST_MODE_A)
		{
			if(servo_data.period_finished != 0)
			{
				sens_data_proc();

				if(vex_data.tx_good == 2)
				{
					LED_1_on();
				}
				else
				{
					LED_1_off();
				}

				copter_action.yaw = width_500 / 4;
				copter_action.roll = width_500 / 4;
				copter_action.pitch = width_500 / 4;
				copter_action.col = width_500 *  + vex_data.chan_width[0];

				mot_set(&motor_speed, &motor_cali, &copter_action);
				mot_apply(&motor_speed, &motor_cali);

				start_next_servo_pwm_period();
			}
		}
		else if(op_mode == TEST_MODE_B)
		{
			for(unsigned char i = 0; i < 8; i++)
			{
				low_priority_interrupts();
				if(bit_is_clear(ADCSRA, ADSC))
				{
					sens_read_adc();
				}
				sens_data_calc_avg(&sens_data[i]);
				unsigned int a = sens_data[i].avg;
				unsigned char l = a & 0x7F;
				unsigned int h_ = ((a - l) >> 7) | (i << 4) | 0x80;
				unsigned char h = (unsigned char)h_;
				ser_tx(h); ser_tx(l);
			}
			ser_rx_wait();
		}
	}
	return 0;
}

