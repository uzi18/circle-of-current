ISR(TIMER1_COMPB_vect) // timer 1 output compare A interrupt
{
	TCCR1A &= 0xFF ^ (_BV(COM1A0) | _BV(COM1A1));
	servo_port &= 0xFF ^ _BV(servo_shift_pin);
	if(bit_is_set(TIFR1, OCF1A))
	{
		TIFR1 |= _BV(OCF1A);
	}
	TCCR1A |= _BV(COM1A0) | _BV(COM1A1);
	unsigned long t = (OCR1A + servo_data.servo_ticks[servo_data.chan]) & 0xFFFF; // calculate next alarm considering overflow
	OCR1A = t; // set next alarm
	OCR1B = t + 128;
	servo_data.chan++; // next channel	

	if(servo_data.chan == 6)
	{
		servo_data.ready_to_restart = 1;
		TIMSK1 &= 0xFF ^ _BV(OCIE1B);
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
	//servo_port &= 0xFF ^ (_BV(servo_shift_pin));

	servo_port &= 0xFF ^ (_BV(servo_input_pin));
}
