#include "timer.h"

static volatile unsigned char start_proc_flag;

void timer0_init()
{
	start_proc_flag = 0;
	OCR0A = process_time;
	OCR0B = loop_frame;
	TIMSK0 |= _BV(OCIE0A) | _BV(OCIE0B);
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

ISR(TIMER0_COMPA_vect)
{
	start_proc_flag = 1;
	OCR0A += loop_frame;
}

ISR(TIMER0_COMPB_vect)
{
	start_proc_flag = 0;
	esc_start_next();
	OCR0B += loop_frame;
}

unsigned char start_proc()
{
	return start_proc_flag;
}
