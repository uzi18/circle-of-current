#ifndef wiimote_h

#include <string.h>
#include <avr/io.h>
#include <util/delay.h>

#include "pindef.h"
#include "macros.h"
#include "twi.h"

typedef struct _wm_cd
{
	unsigned char d[6];
} wm_cd_s;

// initialize wiimote interface with id, starting data, and calibration data
void wm_init(unsigned char *, wm_cd_s, unsigned char *);

// set button data
void wm_newaction(wm_cd_s);

// read_cnt is a psuedo timer
void wm_read_cnt_set(unsigned long);
unsigned long wm_read_cnt_read();

#define wiimote_h
#endif
