#define BAUD 19200 // set baud rate here

#include <avr/io.h> // IO definitions
#include <avr/boot.h> // bootloader macros and functions
#include <avr/pgmspace.h> // for reading from flash program space
#include <util/setbaud.h> // utility to calculate UBRR value

// serial receive time out constant
#define TIMEOUT 0x7FFF // ((SPM_PAGESIZE * 2 * 10 * F_CPU) / (BAUD * 1024))

// bootloader invoking jumper
#define JUMPER_INPUT PINB
#define JUMPER_PIN 0

// address to start of bootloader
#define BOOTLOADER_START 0x7000

// union for char access of short integers
typedef union {
	unsigned char c[2];
	unsigned short d;
} u16;

// location of application flash space
void (*app_start)(void) = 0x0000;

// send char out serial port and wait for finish
void ser_tx(unsigned char c)
{
	UDR0 = c;
	loop_until_bit_is_set(UCSR0A, TXC0);
	UCSR0A = _BV(TXC0);
}

// send short integer out serial port
void ser_tx_short(unsigned short d)
{
	u16 data;
	data.d = d;
	ser_tx(data.c[0]);
	ser_tx(data.c[1]);
}

int main()
{
	// jump to application if bootloader not active
	if(bit_is_clear(JUMPER_INPUT, JUMPER_PIN))
	{
		app_start();
	}

	// start serial port
	UBRR0 = UBRR_VALUE;
	UCSR0B = _BV(RXEN0) | _BV(TXEN0);

	unsigned short cnts;
	unsigned char cntc;

	// send sync signal
	ser_tx_short(0xFFFF);

	// start timeout timer
	TCCR1B = _BV(CS10) | _BV(CS12);

	// get number of pages from serial port
	u16 page_num;
	page_num.d = 0;
	cntc = 0;

	// exit loop when received 2 bytes or timed out after 0.5 seconds
	while(cntc < 2 || TCNT1 < 9765)
	{
		if(bit_is_set(UCSR0A, RXC0))
		{
			unsigned char c = UDR0;
			page_num.c[cntc] = c;
			cntc++;
		}
	}

	// reset if timeout
	if(cntc < 2)
	{
		TCCR1B = 0;
		TCNT1 = 0;
		UCSR0B = 0;
		app_start();
	}

	for(unsigned short i = 0; i < page_num.d; i++)
	{
		while(1)
		{
			// request page
			ser_tx_short(i);

			// reset timeout timer
			TCNT1 = 0;

			// read into buffer from computer
			unsigned short checksum_a = 0;
			unsigned char buff[SPM_PAGESIZE];
			cnts = 0;
			while(cnts < SPM_PAGESIZE && TCNT1 < TIMEOUT)
			{
				if(bit_is_set(UCSR0A, RXC0))
				{
					unsigned char c = UDR0;
					buff[cnts] = c;
					checksum_a += c;
					cnts++;
				}
			}

			// get checksum from computer
			u16 checksum_b;
			cntc = 0;
			while(cntc < 2 && TCNT1 < TIMEOUT)
			{
				if(bit_is_set(UCSR0A, RXC0))
				{
					unsigned char c = UDR0;
					checksum_b.c[cntc] = c;
					cntc++;
				}
			}

			// if checksum match then serial port bytes = buffered bytes
			if(checksum_a == checksum_b.d)
			{
				u16 data;

				// erase the flash page
				boot_page_erase(i * SPM_PAGESIZE);
				boot_spm_busy_wait();

				// fill temporary buffer
				for(unsigned short j = 0; j < SPM_PAGESIZE; j += 2)
				{
					data.c[0] = buff[j];
					data.c[1] = buff[j + 1];
					boot_page_fill(j, data.d);
				}

				// write the entire page from buffer
				boot_page_write(i * SPM_PAGESIZE);
				boot_spm_busy_wait(); // wait until done

				// allow flash to be verified
				boot_rww_enable();

				// verify by reading flash and calculating checksum
				unsigned int checksum_c = 0;
				for(unsigned short j = 0; j < SPM_PAGESIZE; j++)
				{
					unsigned char k = pgm_read_byte((i * SPM_PAGESIZE) + j);
					checksum_c += k;
				}

				// if checksum match, then next page, or else try again
				if(checksum_c == checksum_b.d)
				{
					break;
				}

				#ifdef TX_BAD_CHECKSUM
				else
				{
					ser_tx_short(checksum_c);
					while(1);
				}
			}
			else
			{
				ser_tx_short(checksum_a);
				while(1);
			}
			#else
			}
			#endif
		}
	}

	// indicate finished
	ser_tx_short(page_num.d);

	while(1); // freeze program

	return 0;
}
