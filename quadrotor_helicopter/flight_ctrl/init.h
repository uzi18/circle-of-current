void hardware_init()
{
	// initialize port

	servo_port &= 0xFF ^ (_BV(servo_shift_pin) | _BV(servo_input_pin) | _BV(servo_reset_pin));
	servo_ddr |= _BV(servo_shift_pin) | _BV(servo_input_pin) | _BV(servo_reset_pin);

	input_capt_port &= 0xFF ^ _BV(input_capt_pin);
	input_capt_ddr &= 0xFF ^ _BV(input_capt_pin);

	LED_init();

	ser_init();

	servo_shift_reset();

	// initalize ADC

	ADMUX &= 0xFF ^ (_BV(REFS1) | _BV(REFS0) | _BV(ADLAR));
	ADCSRA |= _BV(ADEN) | _BV(ADSC) | _BV(ADIE) | _BV(ADPS2) | _BV(ADPS1) | _BV(ADPS0);

	// initialize timer

	TCCR1B |= _BV(ICNC1) | _BV(ICES1) | _BV(CS10);
	TIMSK1 |= _BV(TOIE1) | _BV(ICIE1);

	// enable interrupts

	sei();
}

void software_init()
{
	for(unsigned char i = 0; i < 8; i++)
	{
		sens_data[i].cnt = 0;
		servo_data.servo_ticks[i] = width_500;
	}
	servo_data.servo_ticks[5] = 0x10000;
	vex_data.tx_good = 0;
	servo_data.ready_to_restart = 1;
	servo_data.chan = 0;
	ppm_ovf_cnt = 0;
	timer1_ovf_cnt = 0;
	ADC_chan_cnt = 0;

	op_mode = TEST_MODE_A;
}

unsigned char to_load_from_eeprom()
{
	return 0;
}

unsigned char to_calibrate()
{
	return 0;
}

unsigned char to_save_to_eeprom()
{
	return 0;
}

unsigned char to_load_from_serial()
{
	return 0;
}
