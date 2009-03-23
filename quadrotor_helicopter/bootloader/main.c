#include <avr/io.h>
#include <avr/boot.h>
#include <avr/pgmspace.h>
#include <avr/eeprom.h>

#define BAUD 19200
#include <util/setbaud.h>

#define TIMEOUT (SPM_PAGESIZE * 2 * 10 * F_CPU) / (BAUD * 1024)

#define LSB_FIRST

#define SWITCH_INPUT PINC
#define SWITCH_PIN PINC

#define BOOTLOADER_START 0x7C00

void (*app_start)(void) = 0x0000;

void ser_init()
{
	UBRR0 = UBRR_VALUE;
	UCSR0B = _BV(RXEN0) | _BV(TXEN0);
}

void ser_tx(unsigned char c)
{
	loop_until_bit_is_set(UCSR0A, TXC0);
	UCSR0A = _BV(TXC0);
	UDR0 = c;
}

void timer_rst()
{
	TCCR1B = 0;
	TCNT1 = 0;
	TCCR1B = _BV(CS10) | _BV(CS12);
}

void ser_tx_short(unsigned int d)
{
	union {
		struct {
		unsigned char c0;
		unsigned char c1;

		} s;
		unsigned int d;
	} data;
	data.d = d;
	ser_tx(data.s.c0);
	ser_tx(data.s.c1);
}

unsigned char ser_rx_short(unsigned int * cnt, unsigned int * d, unsigned int * cs)
{
	if(bit_is_set(UCSR0A, RXC0))
	{
		unsigned char c = UDR0;
		*cs += c;
		
		if((*cnt & 0x01) == 0x00)
		{
			#ifdef LSB_FIRST
			*d += (unsigned int)c << 8;
			#else
			*d <<= 8;
			*d += c;
			#endif
		}
		else
		{
			*d = c;
		}

		*cnt++;

		return *cnt;
	}

	return 0;
}

int main()
{
	if(bit_is_set(SWITCH_INPUT, SWITCH_PIN))
	{
		app_start();
	}

	ser_init();

	unsigned int holder;

	unsigned int page_num;
	unsigned int cnt = 0;

	do
	{
		ser_tx('B'); ser_tx('O'); ser_tx('O'); ser_tx('T');

		page_num = 0;
		timer_rst();
		cnt = 0;
		while(cnt < 2 && TCNT1 < TIMEOUT)
		{
			ser_rx_short(&cnt, &page_num, &holder);
		}
	}
	while(cnt < 2);

	for(unsigned int i = 0; i < page_num; i++)
	{
		while(1)
		{
			ser_tx_short(i);

			timer_rst();

			unsigned int checksum_a = 0;
			unsigned char data = 0;

			cnt = 0;
			while(cnt < SPM_PAGESIZE && TCNT1 < TIMEOUT)
			{
				unsigned char t = ser_rx_short(&cnt, &data, &checksum_a);
				if(t != 0 && (t % 2) == 0)
				{
					boot_page_fill_safe(cnt - 2, data);
				}
			}

			unsigned int checksum_b = 0;

			cnt = 0;
			while(cnt < 2 && TCNT1 < TIMEOUT)
			{
				ser_rx_short(&cnt, &checksum_b, &holder);
			}

			if(checksum_a == checksum_b)
			{
				boot_page_erase_safe(i * SPM_PAGESIZE);
				boot_page_write_safe(i * SPM_PAGESIZE);

				unsigned int checksum_c = 0;

				for(unsigned int j = 0; j < SPM_PAGESIZE; j++)
				{
					checksum_c += pgm_read_byte((i * SPM_PAGESIZE) + j);
				}

				if(checksum_c == checksum_a)
				{
					break;
				}
			}
		}
	}

	ser_tx_short(page_num);

	return 0;
}
