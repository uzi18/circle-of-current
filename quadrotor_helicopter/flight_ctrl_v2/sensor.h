#ifndef sensor_h_inc
#define sensor_h_inc

#include <math.h>
#include <avr/io.h>
#include <avr/interrupt.h>

#include "config.h"
#include "pindef.h"
#include "macros.h"
#include "calc.h"
#include "ser.h"

/*
typedef struct sens_hist_
{
	unsigned int res[sens_hist_len_max];
	unsigned char cnt;
	unsigned char last_cnt;
	unsigned int latest;
	double avg;
	double offset;
	double noise;
} sens_hist;
//*/

void sens_init();
void sens_calibrate(unsigned char);
void adc_start(unsigned char, unsigned char);
void adc_wait_stop();
unsigned char adc_new(unsigned char);
unsigned int sens_read(unsigned char);
unsigned int sens_offset(unsigned char);

#endif
