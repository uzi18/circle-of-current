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
void wm_init(unsigned char *, wm_cd_s, unsigned char *, void (*)(void));

// set button data
void wm_newaction(wm_cd_s);

#define wiimote_h
#endif
