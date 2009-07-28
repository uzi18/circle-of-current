#ifndef main_inc

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <avr/io.h>
#include <avr/eeprom.h>
#include <avr/pgmspace.h>
#include <avr/interrupt.h>
#include <util/delay.h>
#include <util/atomic.h>

#include "lcd.h"
#include "ser.h"
#include "ff.h"
#include "diskio.h"
#include "mp3.h"
#include "spi.h"
#include "pindef.h"
#include "config.h"

typedef struct {
	unsigned char flags;
	unsigned char mode;
	unsigned char alarm_on[7];
	unsigned char alarm_h[7];
	unsigned char alarm_m[7];
	unsigned char cur_day;
	unsigned char cur_h;
	unsigned char cur_m;
	unsigned char invert;
	unsigned char alarm_mode;
	unsigned char alarm_fade;
	unsigned char ampm;
} OP_STRUCT;

#define main_inc
#endif
