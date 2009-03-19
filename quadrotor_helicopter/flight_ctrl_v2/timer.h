#ifndef timer_h_inc
#define timer_h_inc

#include <avr/io.h>
#include <avr/interrupt.h>

#include "config.h"
#include "pindef.h"
#include "macros.h"
#include "esc.h"

void timer0_init();
void timer1_init();
unsigned long timer1_period_wait(signed long, signed long);
unsigned char start_proc();

#endif
