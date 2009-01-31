#ifndef twi_h

#include <string.h>
#include <avr/io.h>
#include <avr/interrupt.h>
#include <util/twi.h>

#include "macros.h"
#include "pindef.h"

void twi_slave_init(unsigned char);
void twi_attachRxEvent(void (*)(unsigned char *, unsigned char));
void twi_attachTxEvent(void (*)(void));
void twi_transmit(unsigned char *, unsigned char);

#define twi_h
#endif
