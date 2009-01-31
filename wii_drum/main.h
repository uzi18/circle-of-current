#ifndef main_h

#if F_CPU == NULL
#error "define your clock speed"
#endif

#include <avr/io.h>

#include "pindef.h"
#include "macros.h"

#include "ser.h"
#include "wiimote.h"

#define main_h
#endif
