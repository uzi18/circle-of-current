#include "ser.h"

volatile unsigned char serRxBuffer[serRxBufferSize];
volatile unsigned char serRxBufferHead;
volatile unsigned char serRxBufferTail;

/* selects UBBR value based on frequency and baud rate */

/* initialize serial port */
void serInit(unsigned long baudRate)
{
	unsigned int temp = getBaudRate(baudRate);
	
	UBRRH = (temp & 0xFF00) >> 8;
	UBRRL = temp & 0xFF;

	UCSRB = 0; // reset just to be sure
	
	serRxBufferHead = 0; // reset
	serRxBufferTail = 0;
	
	sbi(UCSRB, RXCIE); // Rx int enable
	sbi(UCSRB, RXEN); // enable rx and tx now
	sbi(UCSRB, TXEN);

	sei();
}

/* on rx event, shift and elongate buffer, then store byte at head of buffer*/
ISR(USART_RX_vect)
{
	unsigned char data = UDR; // read
	unsigned char temp = (serRxBufferHead + 1) % serRxBufferSize; // check buffer location first
	if(temp != serRxBufferTail) // if not overflow
	{
		serRxBuffer[serRxBufferHead] = data; // store in buffer
		serRxBufferHead = temp; // increase array pointer
	}
}

/* empties buffer */
void serFlush()
{
	serRxBufferHead = 0;
	serRxBufferTail = 0;
}

/* read FIFO from buffer, return buffer size before reading */
unsigned char serRx(unsigned char *bufSize)
{
	if(serRxBufferHead == serRxBufferTail) // if buffer empty, return error
	{
		*bufSize = 0;
		return 0;
	}
	else
	{
		*bufSize = (serRxBufferSize + serRxBufferHead - serRxBufferTail) % serRxBufferSize;
		unsigned char data = serRxBuffer[serRxBufferTail]; // read from buffer
		serRxBufferTail = (serRxBufferTail + 1) % serRxBufferSize; // advance buffer read pointer
		return data; // return
	}
}

/* clear buffer until c (c will be cleared from buffer too) */
void serSkipTo(unsigned char c)
{
	unsigned char d;
	unsigned char b;
	do { d = serRx ( &b );} while ( b == 0 || d != c);
}

/* get buffer size, also lets you get the latest data and the next data */
unsigned char serAvail(unsigned char *nextData, unsigned char *latestData)
{
	*nextData = serRxBuffer[serRxBufferTail];
	*latestData = serRxBuffer[serRxBufferHead];
	// size of buffer
	return (serRxBufferSize + serRxBufferHead - serRxBufferTail) % serRxBufferSize;
}

/* waits for transmitter to not be busy then tx a byte */
void serTx(unsigned char data)
{
	loop_until_bit_is_set(UCSRA, UDRE); // wait while previous tx is finished
	UDR = data; // tx
}

static int ser_putchar(unsigned char c, FILE *stream)
{
	if(c == '\n') serTx('\r');
	serTx(c);
	return 0;
}
