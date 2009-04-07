#ifndef lcd_inc
#define lcd_inc

#include <stdlib.h>
#include <stdio.h>
#include <avr/io.h>
#include <avr/pgmspace.h>
#include <util/delay.h>
#include "macros.h"

/*
Port and Pin Definitions (below, x indicates what may be changed)
LCD_Port is the PORTx Register, changes pin states (0 = low, 1 = high)
LCD_DDR is the DDRx Register, selects pin as input(0)/output(1)
LCD_In is the PINx Register, unused in this case
LCDData_ connects the 8 bit data bus of the LCD
LCDCtrl_ is the port connected to the E, RS, and RW pins of the LCD
LCDBL_ is connected to the backlight of the LCD
LCD_Pin defines which pin is connected to what
*/

// EDIT ME
#define LCDDataPort PORTA
#define LCDDataDDR DDRA
#define LCDDataIn PINA
#define LCDCtrlPort PORTA
#define LCDCtrlDDR DDRA
#define LCDRSPin 2
#define LCDRWPin 1
#define LCDEPin 3
#define LCDBLPort PORTD
#define LCDBLDDR DDRD
#define LCDBLPin 7

#define LCDDataBit0 4
#define LCDDataBit1 5
#define LCDDataBit2 6
#define LCDDataBit3 7

void LCDSend(unsigned char, unsigned char);

void LCDSetPos(unsigned char, unsigned char);

void LCDPrintTime(unsigned char, unsigned char, unsigned char);

void LCDPrint_P(const char *, unsigned char, unsigned char);

void LCDPrint(char *);

void LCDPrintXY(const char *, unsigned char, unsigned char);

void LCDClear(unsigned char);

void LCDBL(unsigned char);

void LCDCustomChar(unsigned char, unsigned char *);

void LCDInit(void);

int LCD_putc(char, FILE *);

#define upcharaddr 5
#define downcharaddr 6

#endif
