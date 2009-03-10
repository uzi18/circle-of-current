#ifndef mp3_inc

#include "print.h"
#include "ser.h"
#include "tff.h"
#include "macros.h"
#include "spi.h"
#include "stringword.h"
#include <string.h>
#include <util/delay.h>
#include <avr/wdt.h>
#include <avr/interrupt.h>

#define MP3_Port PORTB
#define MP3_PinIn PINB
#define MP3_DDR DDRB
#define MP3_xCDS_Pin 1
#define MP3_xCS_Pin 2
#define MP3_DREQ_Pin 0

#define MP3PacketSize 32

#define MP3_Reg_MODE 0x00
#define MP3_Reg_STATUS 0x01
#define MP3_Reg_BASS 0x02
#define MP3_Reg_CLOCKF 0x03
#define MP3_Reg_DECODE_TIME 0x04
#define MP3_Reg_AUDATA 0x05
#define MP3_Reg_WRAM 0x06
#define MP3_Reg_WRAMADDR 0x07
#define MP3_Reg_HDAT0 0x08
#define MP3_Reg_HDAT1 0x09
#define MP3_Reg_AIADDR 0x0A
#define MP3_Reg_VOL 0x0B
#define MP3_Reg_AICTRL0 0x0C
#define MP3_Reg_AICTRL1 0x0D
#define MP3_Reg_AICTRL2 0x0E
#define MP3_Reg_AICTRL3 0x0F

typedef struct _MP3File
{
	FileName83 fn;

	unsigned char title[31];
	unsigned char titleLen;

	unsigned long duration;

	unsigned long headerLoc;

	FIL fil;
	DIR dir;

	unsigned char success;
	unsigned char opened;
} MP3File;

#define mp3_inc
#endif
