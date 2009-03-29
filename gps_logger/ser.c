#include "ser.h"

/* initialize serial port */
void serInit()
{	
	UBRRH = UBRRH_VALUE;
	UBRRL = UBRRL_VALUE;

	UCSRB = 0; // reset UART
	
	serRxBufferHead = 0; // reset buffer
	serRxBufferTail = 0;
	
	sbi(UCSRB, RXCIE); // rx interrupt enable
	sbi(UCSRB, RXEN); // enable rx and tx
	sbi(UCSRB, TXEN);
}

/* on rx event, shift and elongate buffer, then store byte at head of buffer*/
ISR(USART_RXC_vect)
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
/*
void serFlush()
{
	serRxBufferHead = 0;
	serRxBufferTail = 0;
}
*/

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
/*
unsigned char serAvail(unsigned char *nextData, unsigned char *latestData)
{
	*nextData = serRxBuffer[serRxBufferTail];
	*latestData = serRxBuffer[serRxBufferHead];
	// size of buffer
	return (serRxBufferSize + serRxBufferHead - serRxBufferTail) % serRxBufferSize;
}
*/

/* waits for transmitter to not be busy then tx a byte */
void serTx(unsigned char data)
{
	loop_until_bit_is_set(UCSRA, UDRE); // wait while previous tx is finished
	UDR = data; // tx
}
