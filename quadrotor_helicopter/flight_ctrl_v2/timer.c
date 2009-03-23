#include "timer.h"

static volatile unsigned char timer_0_loop_width;
static volatile unsigned char timer_0_proc_start;
static volatile unsigned char start_proc_flag;
static volatile unsigned long last_tcnt1;
static volatile unsigned long last_tcnt0;

void timer0_init()
{
	start_proc_flag = 0;
	timer_0_loop_width = param_get_ul(period_ticks_addr);
	timer_0_proc_start = param_get_ul(when_to_update_esc_addr);
	OCR0B += timer_0_loop_width;
	OCR0A = OCR0B - (0xFF - timer_0_proc_start);
	TIMSK0 |= _BV(OCIE0B) | _BV(OCIE0A);
	TCCR0B |= _BV(CS00) | _BV(CS02);
}

void timer0_set_loop(unsigned char d)
{
	timer_0_loop_width = d;
}

void timer0_set_start_time(unsigned char d)
{
	timer_0_proc_start = d;
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
	esc_start_next();
	OCR0B += timer_0_loop_width;
}

ISR(TIMER0_COMPA_vect)
{
	start_proc_flag = 1;
	OCR0A = OCR0B - (0xFF - timer_0_proc_start);
}

unsigned char start_proc(unsigned char c)
{
	start_proc_flag &= c;
	return start_proc_flag;
}
