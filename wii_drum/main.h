#ifndef main_h

// enables serial port functions
//#define USE_SERPORT

#if F_CPU == NULL
#error "define your clock speed"
#endif

#include <avr/io.h>

#include "pindef.h"
#include "macros.h"

#ifdef USE_SERPORT
#include "ser.h"
#endif

#include "wiimote.h"

#define main_h
#endif
