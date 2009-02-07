#ifndef ser_h

#include "main.h"

#include <avr/io.h>
#include <avr/interrupt.h>
#include <stdlib.h>
#include <stdio.h>
#include "macros.h"

// Name Replacements

#define UDR UDR0
#define UCSRA UCSR0A
#define UCSRB UCSR0B
#define UCSRC UCSR0C
#define UBRRH UBRR0H
#define UBRRL UBRR0L
#define RXCIE RXCIE0
#define RXEN RXEN0
#define TXEN TXEN0
#define UDRE UDRE0
#define USART_RX_vect USART0_RX_vect

#define serRxBufferSize 8

#ifdef USE_SERPORT
// functions

unsigned int getBaudRate(unsigned long);

void serInit(unsigned long);

void serFlush(void);

unsigned char serRx(unsigned char *);

void serSkipTo(unsigned char);

unsigned char serAvail(unsigned char *, unsigned char *);

void serTx(unsigned char);

static int ser_putchar(unsigned char, FILE *);

static FILE serStdout = FDEV_SETUP_STREAM(ser_putchar, NULL, _FDEV_SETUP_WRITE);

#endif

#define ser_h
#endif
