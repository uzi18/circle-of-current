#ifndef sensor_h_inc
#define sensor_h_inc

#include <avr/io.h>
#include <avr/interrupt.h>

#include "config.h"
#include "pindef.h"
#include "macros.h"
#include "calc.h"
#include "ser.h"

typedef struct sens_hist_
{
	unsigned int res[sens_hist_len_max];
	unsigned char cnt;
	unsigned char last_cnt;
	double avg;
	double offset;
	double noise;
} sens_hist;

void sens_init();
sens_hist sens_read(unsigned char, unsigned char);
void sens_calibrate(unsigned char);
void adc_start(unsigned char, unsigned char);
void adc_wait_stop();
double sens_avg(unsigned char);
double sens_noise(unsigned char);
double sens_offset(unsigned char);
unsigned char sens_cnt(unsigned char);

#endif
