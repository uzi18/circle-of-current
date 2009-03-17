void timer_1_reset()
{
	TCCR1B = 0;
	TCNT1 = 0;
	OCR1A = 0;
	OCR1B = 0;
	vex_data.tx_good = 0;
	vex_data.new_flag = 0;
	ppm_ovf_cnt = 0;
	//timer1_ovf_cnt = 0;
	volatile unsigned char f = TIFR1;
	TIFR1 |= f;
	TCCR1B |= _BV(ICNC1) | _BV(ICES1) | _BV(CS10);
	TIMSK1 |= _BV(TOIE1) | _BV(ICIE1);
}

void hardware_init()
{
	// initialize port

	servo_port &= 0xFF ^ (_BV(servo_shift_pin) | _BV(servo_input_pin) | _BV(servo_reset_pin));
	servo_ddr |= _BV(servo_shift_pin) | _BV(servo_input_pin) | _BV(servo_reset_pin);

	input_capt_port &= 0xFF ^ _BV(input_capt_pin);
	input_capt_ddr &= 0xFF ^ _BV(input_capt_pin);

	LED_init();

	ser_init();

	// initalize ADC

	adc_init();

	// initialize timer

	timer_1_reset();

	TCCR0B |= _BV(CS00) | _BV(CS02); // start timer 0

	// enable interrupts

	sei();
}

void software_init()
{
	for(unsigned char i = 0; i < 8; i++)
	{
		sens_data[i].cnt = 0;
		servo_data.servo_ticks[i] = width_500 * 2;
	}
	vex_data.tx_good = 0;
	ppm_ovf_cnt = 0;
	//timer1_ovf_cnt = 0;
	process_time = 0;
	safety = 0;

	op_mode = TEST_MODE_A;
}

unsigned char to_load_from_eeprom()
{
	return 0;
}

unsigned char to_calibrate_sens()
{
	return 0;
}

unsigned char to_calibrate_ppm()
{
	return 1;
}

unsigned char to_save_to_eeprom()
{
	return 0;
}

unsigned char to_load_from_serial()
{
	return 0;
}

unsigned char to_set_limit()
{
	return 0;
}
