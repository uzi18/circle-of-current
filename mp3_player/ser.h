#ifndef ser_inc

#include <avr/io.h>
#include <avr/wdt.h>
#include <avr/interrupt.h>
#include <stdlib.h>
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

// variables

#define serRxBufferSize 200
volatile unsigned char serRxBuffer[serRxBufferSize];
volatile unsigned char serRxBufferHead;
volatile unsigned char serRxBufferTail;

// functions

unsigned int getBaudRate(unsigned long baudRate);

void serInit(unsigned long baudRate);

void serFlush(void);

unsigned char serRx(unsigned char *bufSize);

void serSkipTo(unsigned char c);

unsigned char serAvail(unsigned char *nextData, unsigned char *latestData);

void serTx(unsigned char data);

#define ser_inc
#endif
