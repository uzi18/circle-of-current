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
	sbi(SPI_port, SPI_SS);
	sbi(SPI_ddr, SPI_SS);

	// output
	sbi(SPI_port, SPI_MOSI);
	sbi(SPI_port, SPI_SCK);
	sbi(SPI_ddr, SPI_MOSI);
	sbi(SPI_ddr, SPI_SCK);

	// input
	cbi(SPI_ddr, SPI_MISO);
	sbi(SPI_port, SPI_MISO);
	
	SPCR = 0b01010000;
	SPSR = 0b00000000;
	SPSR |= 0b00000001;
}
