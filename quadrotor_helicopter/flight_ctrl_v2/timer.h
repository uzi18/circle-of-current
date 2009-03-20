#ifndef timer_h_inc
#define timer_h_inc

#include <avr/io.h>
#include <avr/interrupt.h>

#include "config.h"
#include "pindef.h"
#include "macros.h"
#include "esc.h"
#include "save.h"

void timer0_init();
void timer0_set_loop(unsigned char);
void timer1_init();
double timer1_elapsed();

#endif
