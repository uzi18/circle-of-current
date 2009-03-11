#include <string.h>
#include <avr/io.h>
#include <avr/interrupt.h>
#include <util/delay.h>
#include <avr/eeprom.h>

#include "config.h"
#include "struct_def.h"
#include "pin_def.h"

#include "math.h"

#define FLY_MODE 1
#define POWER_OFF_MODE 2
#define OTHER_MODE 3
#define TEST_MODE_A 4
#define TEST_MODE_B 5
