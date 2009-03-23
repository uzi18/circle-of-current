#ifndef esc_h_inc
#define esc_h_inc

#include <avr/io.h>
#include <avr/interrupt.h>
#include <util/atomic.h>

#include "config.h"
#include "pindef.h"
#include "macros.h"
#include "calc.h"
#include "timer.h"
#include "ser.h"

void esc_init();
void esc_start_next();
unsigned char esc_is_done();
void esc_is_done_clear();
void esc_safe(unsigned char);
void esc_set_width(unsigned char, unsigned int);
unsigned long esc_get_total();

#endif
