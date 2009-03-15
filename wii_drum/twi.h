#ifndef twi_h
#define twi_h

#include <avr/io.h>
#include <avr/interrupt.h>
#include <util/twi.h>

// function prototypes
void twi_slave_init(unsigned char, void (*)(unsigned char, unsigned char), void (*)(unsigned char), void (*)(unsigned char, unsigned char));
void twi_set_reg(unsigned char, unsigned char);
unsigned char twi_read_reg(unsigned char);

#endif
