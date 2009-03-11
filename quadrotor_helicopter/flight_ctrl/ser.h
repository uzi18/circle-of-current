#define BAUD 19200
#include <util/setbaud.h>

void ser_init()
{
	UBRR0 = UBRR_VALUE;
	UCSR0B = _BV(RXEN0) | _BV(TXEN0);
}

void ser_tx(unsigned char d)
{
	UDR0 = d;
	while(bit_is_clear(UCSR0A, TXC0))
	{
		low_priority_interrupts();
		if(bit_is_clear(ADCSRA, ADSC))
		{
			sens_read_adc();
		}
	}
	UCSR0A |= _BV(TXC0);
}

signed int ser_rx()
{
	if(bit_is_set(UCSR0A, RXC0))
	{
		return UDR0;
	}
	else
	{
		return -1;
	}
}

unsigned char ser_rx_wait()
{
	signed int a;
	do
	{
		low_priority_interrupts();
		if(bit_is_clear(ADCSRA, ADSC))
		{
			sens_read_adc();
		}
		a = ser_rx();
	}
	while(a == -1);
	return (unsigned char)a;
}
