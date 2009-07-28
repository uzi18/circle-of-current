#ifndef spi_inc
#define spi_inc

#include <avr/io.h>

#include "macros.h"
#include "pindef.h"

void SPI_init();

void SPITx(unsigned char);

unsigned char SPIRx(unsigned char);

#endif
