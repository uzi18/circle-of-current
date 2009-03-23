#include "timer.h"

static volatile unsigned long last_tcnt1;
static volatile unsigned long last_tcnt0;

void timer0_init()
{
	//TIMSK0 |= _BV(OCIE0B) | _BV(OCIE0A);
	TCCR0B |= _BV(CS00) | _BV(CS02);
}

void timer1_init()
{
	last_tcnt1 = TCNT1;
	sbi(TCCR1B, CS10);
}

signed long timer1_elapsed()
{
	signed long temp_tcnt1 = TCNT1;
	signed long diff = ((temp_tcnt1 | 0x10000) - last_tcnt1) & 0xFFFF;
	last_tcnt1 = temp_tcnt1;
	signed long r = calc_multi(diff, MATH_MULTIPLIER, F_CPU);
	return r;
}

signed long timer0_elapsed()
{
	signed long temp_tcnt0 = TCNT0;
	signed long diff = ((temp_tcnt0 | 0x100) - last_tcnt0) & 0xFF;
	last_tcnt0 = temp_tcnt0;
	signed long r = calc_multi(diff, MATH_MULTIPLIER * 1024, F_CPU);
	return r;
}


ISR(TIMER0_COMPB_vect)
{
}

ISR(TIMER0_COMPA_vect)
{
}
