#ifndef ser_inc

#define BAUD 4800L

#include <avr/io.h>
#include <avr/wdt.h>
#include <avr/interrupt.h>
#include <stdlib.h>
#include <stdio.h>
#include <util/setbaud.h>
#include "macros.h"

// Name Replacements
#ifndef __AVR_ATmega32__
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
#define USART_RXC_vect USART0_RX_vect
#endif

// variables

#define serRxBufferSize 150
volatile unsigned char serRxBuffer[serRxBufferSize];
volatile unsigned char serRxBufferHead;
volatile unsigned char serRxBufferTail;

// functions

void serInit();

//void serFlush(void);

unsigned char serRx(unsigned char *);

void serSkipTo(unsigned char);

//unsigned char serAvail(unsigned char *, unsigned char *);

void serTx(unsigned char);

#define ser_inc
#endif
