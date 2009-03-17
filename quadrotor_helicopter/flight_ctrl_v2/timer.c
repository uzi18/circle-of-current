#include "timer.h"

void timer0_init()
{
	TCCR0B |= _BV(CS00) | _BV(CS02);
}


void timer1_init()
{
	sbi(TCCR1B, CS10);
}
