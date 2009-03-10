#ifndef main_inc

// Configuration

#define F_CPU_ 8000000L
#if (F_CPU != F_CPU_)
#define F_CPU F_CPU_
#endif

#ifndef __AVR_ATmega644__
#define __AVR_ATmega644__
#endif

// Standard Headers
#include <avr/io.h>
#include <stdio.h>
#include <stdlib.h>
#include <util/delay.h>
#include <avr/interrupt.h>
#include <avr/wdt.h>

// Other Headers
#include "print.h"
#include "timer.h"
#include "stringword.h"
#include "config.h"
#include "mp3.h"

// Tiny-FATfs
#include "integer.h"
#include "tff.h"
#include "diskio.h"

// Function Prototypes
void _10ms();
unsigned long get_fattime();
StringWord f_readWord(FIL * fil__, const char *delimS);
void f_checkErrorFreeze(unsigned int f_tffError, const char * s);
void main_init();
void main_loop();
int main();

#define main_inc
#endif
