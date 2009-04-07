#ifndef ser_inc
#define ser_inc

#define BAUD 38400

#include <stdio.h>
#include <avr/io.h>
#include <avr/interrupt.h>
#include <util/setbaud.h>

#include "macros.h"

#define UDR UDR0
#define UCSRA UCSR0A
#define UCSRB UCSR0B
#define UCSRC UCSR0C
#define UBRRH UBRR0H
#define UBRRL UBRR0L
#define RXCIE RXCIE0
#define TXCIE TXCIE0
#define RXEN RXEN0
#define TXEN TXEN0
#define UDRE UDRE0
#define USART_RX_vect USART0_RX_vect
#define USART_TX_vect USART0_TX_vect

#define serRxBufferSize 128
#define serTxBufferSize 128

void serInit();

unsigned char serRx(unsigned char *);

unsigned char serAvail(unsigned char *, unsigned char *);

void serTx(unsigned char);

unsigned char serTxIsBusy();

int ser_putc(unsigned char, FILE *);

#endif
