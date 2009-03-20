#include "timer.h"

static volatile unsigned char timer_0_loop_width;

void timer0_init()
{
	OCR0B += timer_0_loop_width;
	TIMSK0 |= _BV(OCIE0B);
	TCCR0B |= _BV(CS00) | _BV(CS02);
}

void timer0_set_loop(unsigned char d)
{
	timer_0_loop_width = d;
}

static volatile unsigned long last_tcnt1;

void timer1_init()
{
	last_tcnt1 = TCNT1;
	sbi(TCCR1B, CS10);
}

double timer1_elapsed()
{
	unsigned long temp_tcnt1 = TCNT1;
	unsigned long diff = ((temp_tcnt1 | 0x10000) - last_tcnt1) & 0xFFFF;
	last_tcnt1 = temp_tcnt1;
	double r = (double)diff / (double)(F_CPU);
	return r;
}

ISR(TIMER0_COMPB_vect)
{
	esc_start_next();
	OCR0B += timer_0_loop_width;
}
