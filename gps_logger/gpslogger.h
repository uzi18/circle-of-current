#ifndef main_inc

// Compiler Configuration

#define F_CPU_ 8000000L
#if (F_CPU != F_CPU_)
#define F_CPU F_CPU_
#endif

#ifndef __AVR_ATmega32__
#ifndef __AVR_ATmega644__

//#define __AVR_ATmega32__
#define __AVR_ATmega644__

#endif
#endif

// Standard Headers
#include <avr/io.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <util/delay.h>
#include <avr/interrupt.h>
#include <avr/wdt.h>

// Other Headers
#include "print.h"
#include "gps.h"
#include "stringword.h"
#include "config.h"

// Tiny-FATfs
#include "integer.h"
#include "tff.h"
#include "diskio.h"

#define main_inc
#endif
