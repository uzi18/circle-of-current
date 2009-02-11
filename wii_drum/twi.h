#ifndef twi_h
#define twi_h

#include <avr/io.h>
#include <avr/interrupt.h>
#include <util/twi.h>

// function prototypes
void twi_slave_init(unsigned char);
void twi_set_reg(unsigned char, unsigned char);
unsigned char twi_read_reg(unsigned char);
void twi_attach_rx_event( void (*)(unsigned char, unsigned char) );
void twi_attach_tx_start( void (*)(unsigned char) );
void twi_attach_tx_end( void (*)(unsigned char, unsigned char) );
#endif