#ifndef baud_rate_h

#include <stdlib.h>

unsigned int getBaudRate(unsigned long baudRate)
{
	unsigned int res = 0;

	#if F_CPU == 16000000
	switch(baudRate)
	{
		case 76800:
			res = 12;
			break;
		case 38400:
			res = 25;
			break;
		case 19200:
			res = 51;
			break;
		case 9600:
			res = 103;
			break;
		case 4800:
			res = 207;
			break;
		case 2400:
			res = 416;
			break;
		default:
			abort();
	}
	#elif F_CPU == 8000000
	switch(baudRate)
	{
		case 38400:
			res = 12;
			break;
		case 19200:
			res = 25;
			break;
		case 9600:
			res = 51;
			break;
		case 4800:
			res = 103;
			break;
		case 2400:
			res = 207;
			break;
		default:
			abort();
	}
	#elif F_CPU == 20000000
	switch(baudRate)
	{
		case 250000:
			res = 4;
			break;
		case 19200:
			res = 64;
			break;
		case 14400:
			res = 86;
			break;
		case 9600:
			res = 129;
			break;
		case 4800:
			res = 259;
			break;
		case 2400:
			res = 520;
			break;
		default:
			abort();
	}
	#elif F_CPU == 12000000
	switch(baudRate)
	{
		case 57600:
			res = 12;
			break;
		case 28800:
			res = 25;
			break;
		case 19200:
			res = 38;
			break;
		case 14400:
			res = 51;
			break;
		case 9600:
			res = 77;
			break;
		case 4800:
			res = 155;
			break;
		case 2400:
			res = 312;
			break;
		default:
			abort();
	}
	#elif F_CPU == 1000000
	switch(baudRate)
	{
		case 4800:
			res = 12;
			break;
		case 2400:
			res = 25;
			break;
		case 1200:
			res = 51;
			break;
		default:
			abort();
	}
	#elif F_CPU == 18432000
	res = (((F_CPU / (baudRate * 16))) - 1)
	#elif F_CPU == 14745600
	res = (((F_CPU / (baudRate * 16))) - 1)
	#elif F_CPU == 11059200
	res = (((F_CPU / (baudRate * 16))) - 1)
	#elif F_CPU == 9216000
	res = (((F_CPU / (baudRate * 16))) - 1)
	#elif F_CPU == 7372800
	res = (((F_CPU / (baudRate * 16))) - 1)
	#elif F_CPU == 3686400
	res = (((F_CPU / (baudRate * 16))) - 1)
	#else
	#error "Non Standard Frequency"
	#endif

	return res;
}

#define baud_rate_h
#endif
