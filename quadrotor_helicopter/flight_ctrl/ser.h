#define BAUD 19200
#include <util/setbaud.h>

typedef struct _ser_buff
{
	unsigned char d[128];
	unsigned char h;
	unsigned char t;
	unsigned char s;
	unsigned char f;
}
ser_buff_s;

static volatile ser_buff_s ser_tx_buff;
static volatile ser_buff_s ser_rx_buff;

void ser_init()
{
	UBRR0 = UBRR_VALUE;

	ser_tx_buff.h = 0;
	ser_tx_buff.t = 0;
	ser_tx_buff.f = 0;
	ser_tx_buff.s = 128;

	ser_rx_buff.h = 0;
	ser_rx_buff.t = 0;
	ser_rx_buff.f = 0;
	ser_rx_buff.s = 128;

	UCSR0B = _BV(RXEN0) | _BV(TXEN0) | _BV(RXCIE0) | _BV(TXCIE0);
}

signed int ser_rx()
{
	if(ser_rx_buff.h != ser_rx_buff.t)
	{
		unsigned char c = ser_rx_buff.d[ser_rx_buff.h];
		ser_rx_buff.h = (ser_rx_buff.h + 1) % ser_rx_buff.s;
		return c;
	}
	else
	{
		return -1;
	}
}

unsigned char ser_rx_size()
{
	return ((ser_rx_buff.t + ser_rx_buff.s) - ser_rx_buff.h) % ser_rx_buff.s;
}

void ser_tx(unsigned char c)
{
	ser_tx_buff.d[ser_tx_buff.t] = c;
	ser_tx_buff.t = (ser_tx_buff.t + 1) % ser_tx_buff.s;

	if(ser_tx_buff.f == 0)
	{
		UDR0 = c;
		ser_tx_buff.h = (ser_tx_buff.h + 1) % ser_tx_buff.s;
		ser_tx_buff.f = 1;
	}
}

ISR(USART0_TX_vect)
{
	if(ser_tx_buff.h != ser_tx_buff.t)
	{
		UDR0 = ser_tx_buff.d[ser_tx_buff.h];
		ser_tx_buff.h = (ser_tx_buff.h + 1) % ser_tx_buff.s;
	}
	else
	{
		ser_tx_buff.f = 0;
	}
}

ISR(USART0_RX_vect)
{
	ser_rx_buff.d[ser_rx_buff.t] = UDR0;
	ser_rx_buff.t = (ser_rx_buff.t + 1) % ser_rx_buff.s;
}
