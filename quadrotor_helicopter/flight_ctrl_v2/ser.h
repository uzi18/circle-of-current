#ifndef ser_h_inc
#define ser_h_inc

#include <avr/io.h>
#include <math.h>
#include <avr/interrupt.h>
#include <avr/pgmspace.h>

#include "config.h"
#include "pindef.h"
#include "macros.h"
#include "save.h"

#include <util/setbaud.h>

typedef struct _ser_buff
{
	unsigned char d[256];
	unsigned int h;
	unsigned int t;
	unsigned int s;
	unsigned char f;
}
ser_buff_s;

void ser_init();
void ser_tx(unsigned char);
void debug_tx(unsigned char, const signed char *, double);
unsigned char ser_tx_is_busy();

#endif
