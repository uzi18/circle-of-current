#ifndef spi_inc

#include <avr/io.h>
#include "macros.h"

void SPITx(unsigned char c);
unsigned char SPIRx(unsigned char c);
void SPIInitMasterMode0();

#define spi_inc
#endif
