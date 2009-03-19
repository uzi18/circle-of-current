#ifndef ser_h_inc
#define ser_h_inc

#include <stdlib.h>
#include <math.h>
#include <avr/io.h>
#include <avr/interrupt.h>
#include <avr/pgmspace.h>

#include "config.h"
#include "pindef.h"
#include "macros.h"

#include <util/setbaud.h>

typedef struct _ser_buff
{
	unsigned char d[255];
	unsigned char h;
	unsigned char t;
	unsigned char s;
	unsigned char f;
}
ser_buff_s;

typedef struct _cmd
{
	signed long data;
	unsigned char addr;
} cmd;

typedef struct _cmd_buff
{
	cmd com[8];
	unsigned char h;
	unsigned char t;
	unsigned char s;
	unsigned char addr;
	unsigned char sign_f;
	unsigned char cnt;
	signed long data;
}
cmd_buff_s;

void ser_init();
cmd com_rx();
unsigned char com_rx_size();
void ser_tx(unsigned char);
void debug_tx(unsigned char, const signed char *, double);
unsigned char ser_tx_is_busy();

#endif
