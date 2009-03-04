#include "main.h"

static volatile ppm_data vex_data;
static volatile sens_history sens_data[6];
static volatile mot_speed motor_speed;
static volatile mot_cali motor_cali;
static volatile heli_action copter_action;
static volatile servo_ctrl servo_data;

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
}

int main()
{
	software_init();
	hardware_init();

	while(1)
	{
		low_priority_interrupts();
		if(servo_data.servo_new_period_started != 0)
		{			
			sens_data_proc();

			while(servo_data.safe_to_update_servo_array == 0)
			{
				low_priority_interrupts();
			}

			mot_apply(&motor_speed, &motor_cali);

			servo_data.servo_new_period_started = 0;
		}
	}
	return 0;
}

