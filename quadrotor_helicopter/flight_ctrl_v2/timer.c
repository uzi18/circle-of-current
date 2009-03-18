#include "timer.h"

void timer0_init()
{
	TCCR0B |= _BV(CS00) | _BV(CS02);
}


void timer1_init()
{
	sbi(TCCR1B, CS10);
}

unsigned long timer1_period_wait(signed long passed, signed long total)
{
	unsigned long ttt = 0;
	unsigned long lt = TCNT1;
	while((passed + ttt) < total)
	{
		unsigned long tcntt = TCNT1;
		ttt += ((tcntt | 0x10000) - lt) & 0xFFFF;
		lt = tcntt;
	}
	return ttt;
}
