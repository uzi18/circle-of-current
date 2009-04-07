#include "spi.h"

void SPI_init()
{
	SPI_DIR |= _BV(SS) | _BV(MOSI) | _BV(SCK);
	SPI_PORT |= _BV(SS) | _BV(MOSI) | _BV(SCK) | _BV(MISO);

	SPCR = _BV (MSTR) | _BV (SPE);	// Master mode, SPI enable, clock speed MCU_XTAL/4
}

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
