#include "ser.h"

#ifdef USE_SERPORT

volatile unsigned char serRxBuffer[serRxBufferSize];
volatile unsigned char serRxBufferHead;
volatile unsigned char serRxBufferTail;

/* selects UBBR value based on frequency and baud rate */
unsigned int getBaudRate(unsigned long baudRate)
{
	unsigned int res = 0;

	#if F_CPU == 16000000
	switch(baudRate)
	{
		case 76800:
			res = 12;
			break;
		case 38400:
			res = 25;
			break;
		case 19200:
			res = 51;
			break;
		case 9600:
			res = 103;
			break;
		case 4800:
			res = 207;
			break;
		case 2400:
			res = 416;
			break;
		default:
			abort();
	}
	#elif F_CPU == 8000000
	switch(baudRate)
	{
		case 38400:
			res = 12;
			break;
		case 19200:
			res = 25;
			break;
		case 9600:
			res = 51;
			break;
		case 4800:
			res = 103;
			break;
		case 2400:
			res = 207;
			break;
		default:
			abort();
	}
	#elif F_CPU == 20000000
	switch(baudRate)
	{
		case 250000:
			res = 4;
			break;
		case 19200:
			res = 64;
			break;
		case 14400:
			res = 86;
			break;
		case 9600:
			res = 129;
			break;
		case 4800:
			res = 259;
			break;
		case 2400:
			res = 520;
			break;
		default:
			abort();
	}
	#elif F_CPU == 12000000
	switch(baudRate)
	{
		case 57600:
			res = 12;
			break;
		case 28800:
			res = 25;
			break;
		case 19200:
			res = 38;
			break;
		case 14400:
			res = 51;
			break;
		case 9600:
			res = 77;
			break;
		case 4800:
			res = 155;
			break;
		case 2400:
			res = 312;
			break;
		default:
			abort();
	}
	#elif F_CPU == 1000000
	switch(baudRate)
	{
		case 4800:
			res = 12;
			break;
		case 2400:
			res = 25;
			break;
		case 1200:
			res = 51;
			break;
		default:
			abort();
	}
	#elif F_CPU == 18432000
	res = (((F_CPU / (baudRate * 16))) - 1)
	#elif F_CPU == 14745600
	res = (((F_CPU / (baudRate * 16))) - 1)
	#elif F_CPU == 11059200
	res = (((F_CPU / (baudRate * 16))) - 1)
	#elif F_CPU == 9216000
	res = (((F_CPU / (baudRate * 16))) - 1)
	#elif F_CPU == 7372800
	res = (((F_CPU / (baudRate * 16))) - 1)
	#elif F_CPU == 3686400
	res = (((F_CPU / (baudRate * 16))) - 1)
	#else
	#error "Non Standard Frequency"
	#endif

	return res;
}

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

#endif
