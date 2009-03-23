#ifndef save_h_inc
#define save_h_inc

#include <avr/io.h>
#include <avr/eeprom.h>

#include "config.h"
#include "macros.h"
#include "ser.h"

#include "settings_autogen.h"

double param_get_d(saved_params_s *, unsigned char);
unsigned long param_get_ul(saved_params_s *, unsigned char);
signed long param_get_sl(saved_params_s *, unsigned char);
void param_set_d(saved_params_s *, unsigned char, double);
void param_set_ul(saved_params_s *, unsigned char, unsigned long);
void param_set_sl(saved_params_s *, unsigned char, signed long);
void param_save(saved_params_s *, unsigned char);
void param_load(saved_params_s *, unsigned char);
void param_save_all(saved_params_s *);
void param_load_all(saved_params_s *);
void param_init(saved_params_s *);

#endif
