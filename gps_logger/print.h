#ifndef print_inc

/* this .h handles printing formated strings to serial port, used mainly for debug */

#include <stdio.h>
#include <avr/pgmspace.h>
#include "ser.h"

static int ser_putchar(unsigned char c, FILE *stream);

static FILE serStdout = FDEV_SETUP_STREAM(ser_putchar, NULL, _FDEV_SETUP_WRITE);

static int ser_putchar(unsigned char c, FILE *stream)
{
	if(c == '\n') serTx('\r');
	serTx(c);
	return 0;
}

#define print_inc
#endif
