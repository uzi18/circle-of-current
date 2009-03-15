#ifndef wiimote_h

#include <string.h>
#include <avr/io.h>
#include <util/delay.h>

#include "config.h"
#include "pindef.h"
#include "macros.h"
#include "twi.h"

// initialize wiimote interface with id, starting data, and calibration data
void wm_init(unsigned char *, unsigned char *, unsigned char *, void (*)(void));

// set button data
void wm_newaction(unsigned char *);

#define wiimote_h
#endif
