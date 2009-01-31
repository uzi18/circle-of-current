#ifndef printDouble_inc

/* this .h contains functions that convert double variables to formated strings */

#include "stringword.h"
#include <stdio.h>
#include <math.h>

StringWord _doubleSW;

static int _double_putchar(unsigned char c, FILE *stream);

static FILE _doubleStdout = FDEV_SETUP_STREAM(_double_putchar, NULL, _FDEV_SETUP_WRITE);

static int _double_putchar(unsigned char c, FILE *stream)
{
	_doubleSW.c[_doubleSW.length] = c;
	_doubleSW.length += 1;
	_doubleSW.c[_doubleSW.length] = 0;
	return 0;
}

/* converts number to string */
StringWord printDec_(unsigned long data, unsigned char place)
{
	StringWord sw_;
	sw_.length = 0;
	unsigned long temp;
	if(data == 0)
	{
		unsigned char i;
		for(i = 0; i < place; i++)
		{
			sw_.c[sw_.length] = '0';
			sw_.length += 1;
		}
	}
	else
	{
		unsigned long usData = data;
		unsigned long temp2;
		unsigned char i;
		for(i = place; i != 0; i--)
		{
			unsigned long temp3;
			unsigned char j;

			temp3 = 1; for(j = 0; j < (i - 1); j++) temp3 *= 10; // temp3 = pow(10, i - 1);

			temp2 = usData / temp3;

			if(temp2 % 10 > 0) // see if that digit is 0
			{				
				temp3 = 1; for(j = 0; j < (i - 1); j++) temp3 *= 10;

				temp2 = usData / temp3;

				temp = temp2 % 10; // isolate that digit
				sw_.c[sw_.length] = '0' + (unsigned char)temp;
				sw_.length += 1;
			}
			else
			{
				sw_.c[sw_.length] = '0';
				sw_.length += 1;
			}
		}
	}
	sw_.c[sw_.length] = 0;
	return sw_;
}

/* converts double variable to string with set number of decimal places */
StringWord printDouble(double dat, unsigned char pla)
{
	_doubleSW.length = 0;

	// handles negatives
	if(signbit(dat))
	{
		fprintf(&_doubleStdout, "-");
		dat *= (double)-1.0;
	}

	// calculate multiplier based on decimal places wanted
	unsigned long p = 1;
	unsigned char i;
	for(i = 0; i < pla; i++)
	{
		p *= 10;
	}

	// strip non-decimal places and multiply
	double temp = (dat - floor(dat)) * (double)p;

	// convert the decimal places to string
	StringWord sw_ = printDec_((unsigned long)temp, (unsigned char)pla);
	
	// put together in format
	fprintf(&_doubleStdout, "%d.%s", (unsigned int)floor(dat), sw_.c);

	return _doubleSW;	
}

#define printDouble_inc
#endif
