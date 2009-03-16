ISR(TIMER1_COMPA_vect) // timer 1 output compare A interrupt
{
	TCCR1C |= _BV(FOC1A);
	servo_port &= 0xFF ^ (_BV(servo_input_pin));
	signed long t = (OCR1A + servo_data.servo_ticks[servo_data.chan]) & 0xFFFF; // calculate next alarm considering overflow
	OCR1A = t;
	servo_data.chan++; // next channel
	if(servo_data.chan >= 6)
	{
		servo_data.ready_to_restart = 1;
		TIMSK1 &= 0xFF ^ _BV(OCIE1A);
		TCCR1A &= 0xFF ^ (_BV(COM1A0) | _BV(COM1A1));
	}
}

void servo_shift_reset()
{
	servo_port &= 0xFF ^ (_BV(servo_shift_pin));

	servo_port &= 0xFF ^ (_BV(servo_reset_pin));
	servo_port |= _BV(servo_reset_pin);

	servo_port |= _BV(servo_input_pin);

	servo_port |= _BV(servo_shift_pin);

	servo_port &= 0xFF ^ (_BV(servo_shift_pin));

	servo_port &= 0xFF ^ (_BV(servo_input_pin));
}
