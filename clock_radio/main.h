#ifndef main_inc

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <avr/io.h>
#include <avr/eeprom.h>
#include <avr/pgmspace.h>
#include <avr/interrupt.h>
#include <util/delay.h>
#include <util/atomic.h>

#include "lcd.h"
#include "ser.h"
#include "ff.h"
#include "diskio.h"
#include "mp3.h"
#include "pindef.h"

#define main_inc
#endif
