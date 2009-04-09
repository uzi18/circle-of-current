#ifndef lcd_inc
#define lcd_inc

#include <stdlib.h>
#include <stdio.h>
#include <avr/io.h>
#include <avr/pgmspace.h>
#include <util/delay.h>
#include "macros.h"
#include "pindef.h"
#include "config.h"

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

void LCDSoftReset();

#define upperlineaddr 0
#define ulc upperlineaddr
#define lowerlineaddr 1
#define llc lowerlineaddr
#define bothlineaddr 2
#define blc bothlineaddr
#define largecolonleftaddr 3
#define largecolonrightaddr 4
#define xxc 0xFF
#define ssc 0x20
#define upcharaddr 5
#define downcharaddr 6

#endif
