#include "ser.h"

volatile unsigned char serRxBuffer[serRxBufferSize];
volatile unsigned char serRxBufferHead;
volatile unsigned char serRxBufferTail;

volatile unsigned char serTxBuffer[serTxBufferSize];
volatile unsigned char serTxBufferHead;
volatile unsigned char serTxBufferTail;
volatile unsigned char serTxBusy;

/* initialize serial port */
void serInit()
{	
	UBRRH = UBRRH_VALUE;
	UBRRL = UBRRL_VALUE;

	UCSRB = 0; // reset just to be sure
	
	serRxBufferHead = 0; // reset
	serRxBufferTail = 0;
	serTxBufferHead = 0;
	serTxBufferTail = 0;
	serTxBusy = 0;
	
	sbi(UCSRB, TXCIE); // Rx int enable
	sbi(UCSRB, RXCIE); // Rx int enable
	sbi(UCSRB, RXEN); // enable rx and tx now
	sbi(UCSRB, TXEN);
}

ISR(USART_TX_vect)
{
	if(serTxBufferHead != serTxBufferTail)
	{
		UDR0 = serTxBuffer[serTxBufferHead];
		serTxBufferHead = (serTxBufferHead + 1) % serTxBufferSize;
	}
	else
	{
		serTxBusy = 0;
	}
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
	while((serTxBufferTail + 1) % serTxBufferSize == serTxBufferHead);
	serTxBuffer[serTxBufferTail] = data;
	serTxBufferTail = (serTxBufferTail + 1) % serTxBufferSize;

	if(serTxBusy == 0)
	{
		UDR0 = data;
		serTxBufferHead = (serTxBufferHead + 1) % serTxBufferSize;
		serTxBusy = 1;
	}
}

unsigned char serTxIsBusy()
{
	return serTxBusy;
}

int ser_putc(unsigned char c, FILE *stream)
{
	if(c == '\n') serTx('\r');
	serTx(c);
	return 0;
}
