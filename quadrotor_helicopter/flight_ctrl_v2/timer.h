#ifndef timer_h_inc
#define timer_h_inc

#include <avr/io.h>
#include <avr/interrupt.h>

#include "config.h"
#include "pindef.h"
#include "macros.h"
#include "esc.h"
#include "save.h"
#include "calc.h"

void timer0_init();
void timer0_set_loop(unsigned char);
void timer0_set_start_time(unsigned char);
void timer1_init();
signed long timer0_elapsed();
signed long timer1_elapsed();
unsigned char start_proc(unsigned char);

#endif
