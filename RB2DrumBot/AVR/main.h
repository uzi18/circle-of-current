#ifndef main_h

#if F_CPU == NULL
#error "define your clock speed"
#endif

#include <avr/io.h>
#include <avr/interrupt.h>
#include <util/delay.h>

#include "config.h"
#include "pindef.h"
#include "macros.h"
#include "baud_rate.h"
#include "tff.h"

#define main_h
#endif
