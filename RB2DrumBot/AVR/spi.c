#include "spi.h"

void SPITx(unsigned char c)
{
	SPDR = c;
	loop_until_bit_is_set(SPSR, SPIF);
}

unsigned char SPIRx(unsigned char c)
{
	SPDR = c;
	loop_until_bit_is_set(SPSR, SPIF);
	return SPDR;
}

void SPIInitMasterMode0()
{
	// SS pin must be output and high
	sbi(PORTB, 4);
	sbi(DDRB, 4);

	// output
	sbi(DDRB, 5);
	sbi(DDRB, 7);

	// input
	cbi(DDRB, 6);
	
	SPCR = 0b01010000;
	SPSR = 0b00000000;
	SPSR |= 0b00000001;
}
