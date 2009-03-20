#include "ser.h"

static volatile ser_buff_s ser_tx_buff;

void ser_init()
{
	UBRR0 = UBRR_VALUE;

	ser_tx_buff.h = 0;
	ser_tx_buff.t = 0;
	ser_tx_buff.f = 0;
	ser_tx_buff.s = 256;

	UCSR0B = _BV(RXEN0) | _BV(TXEN0) | _BV(RXCIE0) | _BV(TXCIE0);
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

static volatile unsigned char ser_rx_addr;
static volatile double ser_rx_data;
static volatile signed char ser_rx_data_sign;
static volatile signed char ser_rx_data_dot;
static volatile unsigned char ser_rx_state;


ISR(USART0_RX_vect)
{
	unsigned long c = UDR0;
	if(c == '@')
	{
		ser_rx_addr = 0;
		ser_rx_state = 0;
	}
	else if(c == '=')
	{
		ser_rx_data = 0;
		ser_rx_data_sign = 1;
		ser_rx_data_dot = 0;
		ser_rx_state = 1;
	}
	else if((c == 'D' || c == 'L') && ser_rx_state == 1)
	{
		for(signed char i = 0; i < ser_rx_data_dot - 1; i++)
		{
			ser_rx_data /= 10;
		}
		
		if(c == 'D')
		{
			param_set_d(ser_rx_addr % 128, ser_rx_data * (double)ser_rx_data_sign);
		}
		else
		{
			param_set_sl(ser_rx_addr % 128, lround(ser_rx_data) * ser_rx_data_sign);
		}
	}
	else if(c == 'R')
	{
		param_save(ser_rx_addr);
	}
	else if(c == 'W')
	{
		param_load(ser_rx_addr);
	}
	else if(c >= '0' && c <= '9')
	{
		if(ser_rx_state == 0)
		{
			ser_rx_addr *= 10;
			ser_rx_addr += c - '0';
		}
		else
		{
			ser_rx_data *= 10;
			ser_rx_data += (double)(c - '0');
			if(ser_rx_data_dot > 0)
			{
				ser_rx_data_dot++;
			}
		}
	}
	else if(c == '.')
	{
		ser_rx_data_dot = 1;
	}
	else if(c == '-')
	{
		ser_rx_data_sign = -1;
	}
}

void debug_tx(unsigned char addr, const signed char * str, double data)
{
	ser_tx(addr | 0x80);
	unsigned char c;
	while ((c = pgm_read_byte(str++)))
	{
    	ser_tx(c);
	}
	ser_tx('=');
	signed char * num_str;
	num_str = calloc(16, sizeof(num_str));
	num_str = dtostrf(data, -5, 3, num_str);
	for(unsigned char i = 0; num_str[i] != 0; i++)
	{
		ser_tx(num_str[i]);
	}
	free(num_str);
	ser_tx('\r'); ser_tx('\n');
}

unsigned char ser_tx_is_busy()
{
	return ser_tx_buff.f;
}
