void LED_init()
{
	LED_1_port &= 0xFF ^ _BV(LED_1_pin);
	LED_1_ddr |= _BV(LED_1_pin);

	LED_2_port &= 0xFF ^ _BV(LED_2_pin);
	LED_2_ddr |= _BV(LED_2_pin);
}

void LED_1_on()
{
	LED_1_port |= _BV(LED_1_pin);
}

void LED_1_tog()
{
	LED_1_port ^= _BV(LED_1_pin);
}

void LED_1_off()
{
	LED_1_port &= 0xFF ^ _BV(LED_1_pin);
}

void LED_2_on()
{
	LED_2_port |= _BV(LED_2_pin);
}

void LED_2_off()
{
	LED_2_port &= 0xFF ^ _BV(LED_2_pin);
}

void LED_2_tog()
{
	LED_2_port ^= _BV(LED_2_pin);
}
