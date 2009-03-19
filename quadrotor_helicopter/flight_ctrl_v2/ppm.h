#ifndef ppm_h_inc
#define ppm_h_inc

#include <math.h>
#include <avr/io.h>
#include <avr/interrupt.h>

#include "config.h"
#include "pindef.h"
#include "macros.h"
#include "calc.h"
#include "timer.h"
#include "ser.h"

void ppm_init();
unsigned char ppm_is_new_data(unsigned char);
signed int ppm_chan_width(unsigned char);
void ppm_calibrate(unsigned char);
unsigned char ppm_highest_chan_read();
unsigned char ppm_tx_is_good(unsigned char);

#endif
