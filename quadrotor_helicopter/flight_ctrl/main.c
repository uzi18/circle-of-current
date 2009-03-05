#include "main.h"

static ppm_data vex_data;
static sens_history sens_data[6];
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
static unsigned char op_mode;

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

void hardware_init()
{
	// initialize port

	servo_port &= 0xFF ^ (_BV(f_motor_pin) | _BV(b_motor_pin) | _BV(l_motor_pin) | _BV(r_motor_pin) | _BV(aux_servo_pin));
	servo_ddr |= _BV(f_motor_pin) | _BV(b_motor_pin) | _BV(l_motor_pin) | _BV(r_motor_pin) | _BV(aux_servo_pin);

	input_capt_port &= 0xFF ^ _BV(input_capt_pin);
	input_capt_ddr &= 0xFF ^ _BV(input_capt_pin);

	// initalize ADC

	ADMUX &= 0xFF ^ (_BV(REFS1) | _BV(REFS0) | _BV(ADLAR));
	ADCSRA |= _BV(ADEN) | _BV(ADSC) | _BV(ADPS2) | _BV(ADPS1) | _BV(ADPS0);

	// initialize timer

	TIMSK1 = _BV(OCIE1A);
	TCCR1B = 0b00000001 | _BV(ICES1);

	sei();
}

void software_init()
{
	for(unsigned char i = 0; i < 6; i++)
	{
		sens_data[i].cnt = 0;
		servo_data.servo_ticks[i] = width_500;
	}
	tx_good = 0;
	servo_data.servo_new_period_started = 0;
	servo_data.safe_to_update_servo_array = 0;
}

int main()
{
	hardware_init();
	software_init();
	load_calibration(0);

	while(1)
	{
		low_priority_interrupts();
		if(op_mode == 1)
		{
			if(servo_data.servo_new_period_started != 0)
			{			
				sens_data_proc();

				signed long target;
				signed long current;

				target = scale(vex_data.chan_width[yaw_ppm_chan], 100, 100);
				current = scale(sens_data[yaw_sens_chan].centered_avg, 100. 100);

				copter_action.yaw = PID_mv(&yaw_pid, current, target);

				while(servo_data.safe_to_update_servo_array == 0)
				{
					low_priority_interrupts();
				}

				mot_apply(&motor_speed, &motor_cali);

				servo_data.servo_new_period_started = 0;
			}
		}
		else if(op_mode == 2)
		{
		}
	}
	return 0;
}

