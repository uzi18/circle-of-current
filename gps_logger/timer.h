#ifndef timer_inc

#include <avr/io.h>
#include <stdlib.h>
#include "macros.h"

#ifdef __AVR_ATmega32__
#define TCCR0B TCCR0
#define TCCR2B TCCR2
#define OCR2A OCR2
#define TIMSK2 TIMSK
#define OCIE2A OCIE2
#endif

/* function will start specified timer with a clock divider specified, set divider to 0 to stop */
void timerSetDivider(unsigned char timerNum, unsigned int divider)
{
	unsigned char temp = 0;
	switch(divider)
	{
		case 0:
			break;
		case 1:
			temp |= 1;
			break;
		case 8:
			temp |= 2;
			break;
		case 64:
			temp |= 3;
			break;
		case 256:
			temp |= 4;
			break;
		case 1024:
			temp |= 5;
			break;
		default:
			abort();
	}

	switch(timerNum)
	{
		case 0:
			TCCR0B &= 0b11111000;
			TCCR0B |= temp;
			break;
		case 1:
			TCCR1B &= 0b11111000;
			TCCR1B |= temp;
			break;
		case 2:
			TCCR2B &= 0b11111000;
			TCCR2B |= temp;
			break;
		default:
			abort();
	}
}

/* basic timer functions */

void timer2Restart(unsigned int divider, unsigned char start)
{
	timerSetDivider(2, divider);
	TCNT2 = start;
}

void timer2MatchAIntOn(unsigned char c)
{
	OCR2A = c;
	sbi(TIMSK2, OCIE2A);
}

/*
void timer2MatchAIntOff()
{
	cbi(TIMSK2, OCIE2A);
}

#ifndef __AVR_ATmega32__
void timer2MatchBIntOn(unsigned char c)
{
	OCR2B = c;
	sbi(TIMSK2, OCIE2B);
}

void timer2MatchBIntOff()
{
	cbi(TIMSK2, OCIE2B);
}
#endif

void timer2OverflowIntOn()
{
	sbi(TIMSK2, TOIE2);
}

void timer2OverflowIntOff()
{
	cbi(TIMSK2, TOIE2);
}

void timer1Restart(unsigned int divider, unsigned int start)
{
	timerSetDivider(1, divider);
	TCNT1 = start;
}

void timer1MatchAIntOn(unsigned int c)
{
	OCR1A = c;
	sbi(TIMSK1, OCIE1A);
}

void timer1MatchAIntOff()
{
	cbi(TIMSK1, OCIE1A);
}

#ifndef __AVR_ATmega32__
void timer1MatchBIntOn(unsigned int c)
{
	OCR1B = c;
	sbi(TIMSK1, OCIE1B);
}

void timer1MatchBIntOff()
{
	cbi(TIMSK1, OCIE1B);
}
#endif

void timer1OverflowIntOn()
{
	sbi(TIMSK1, TOIE1);
}

void timer1OverflowIntOff()
{
	cbi(TIMSK1, TOIE1);
}

void timer0Restart(unsigned int divider, unsigned int start)
{
	timerSetDivider(0, divider);
	TCNT0 = start;
}

void timer0MatchAIntOn(unsigned int c)
{
	OCR1A = c;
	sbi(TIMSK0, OCIE0A);
}

void timer0MatchAIntOff()
{
	cbi(TIMSK0, OCIE0A);
}

#ifndef __AVR_ATmega32__
void timer0MatchBIntOn(unsigned int c)
{
	OCR0B = c;
	sbi(TIMSK0, OCIE0B);
}

void timer0MatchBIntOff()
{
	cbi(TIMSK0, OCIE0B);
}
#endif

void timer0OverflowIntOn()
{
	sbi(TIMSK0, TOIE0);
}

void timer0OverflowIntOff()
{
	cbi(TIMSK0, TOIE0);
}
*/

#define timer_inc
#endif
