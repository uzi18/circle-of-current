#include "ser.h"

static volatile ser_buff_s ser_tx_buff;
static volatile cmd_buff_s cmd_buff;

void ser_init()
{
	UBRR0 = UBRR_VALUE;

	ser_tx_buff.h = 0;
	ser_tx_buff.t = 0;
	ser_tx_buff.f = 0;
	ser_tx_buff.s = 255;

	cmd_buff.h = 0;
	cmd_buff.t = 0;
	cmd_buff.s = 8;

	UCSR0B = _BV(RXEN0) | _BV(TXEN0) | _BV(RXCIE0) | _BV(TXCIE0);
}

cmd com_rx()
{
	cmd c = cmd_buff.com[cmd_buff.h];
	cmd_buff.h = (cmd_buff.h + 1) % cmd_buff.s;
	return c;
}

unsigned char com_rx_size()
{
	return ((cmd_buff.t + cmd_buff.s) - cmd_buff.h) % cmd_buff.s;
}

void ser_tx(unsigned char c)
{
	while((ser_tx_buff.t + 1) % ser_tx_buff.s == ser_tx_buff.h);
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
	unsigned long c = UDR0;
	if(bit_is_set(c, 7))
	{
		c &= 0xFF ^ _BV(7);
		cmd_buff.sign_f = c & (0xFF ^ _BV(0));
		cmd_buff.addr = c >> 1;
		cmd_buff.cnt = 0;
		cmd_buff.data = 0;
	}
	else
	{
		cmd_buff.data += c << (7 * cmd_buff.cnt);
		cmd_buff.cnt++;
		if(cmd_buff.cnt == 5)
		{
			if(cmd_buff.sign_f != 0)
			{
				cmd_buff.data *= -1;
			}
			cmd_buff.com[cmd_buff.t].data = cmd_buff.data;
			cmd_buff.com[cmd_buff.t].addr = cmd_buff.addr;
			cmd_buff.t = (cmd_buff.t + 1) % cmd_buff.s;
		}
	}
}

void debug_tx(unsigned char addr, signed long data)
{
	addr *= 2;
	addr |= _BV(7);
	if(data < 0)
	{
		addr |= 1;
		data *= -1;
	}
	ser_tx(addr);

	for(unsigned char i = 0; i < 5; i++)
	{
		ser_tx(data & 0x7F);
		data = (data & 0xFFFFFF80) >> 7;
	}
}

unsigned char ser_tx_is_busy()
{
	return ser_tx_buff.f;
}
