#ifndef save_h_inc
#define save_h_inc

#include <avr/io.h>
#include <avr/eeprom.h>

#include "config.h"
#include "macros.h"
#include "ser.h"

#include "settings_autogen.h"

double param_get_d(unsigned char);
unsigned long param_get_ul(unsigned char);
signed long param_get_sl(unsigned char);
void param_set_d(unsigned char, double);
void param_set_ul(unsigned char, unsigned long);
void param_set_sl(unsigned char, signed long);
void param_save(unsigned char);
void param_load(unsigned char);
void param_save_all();
void param_load_all();
void param_init();

#endif
