#ifndef mp3_inc

#include "ser.h"
#include "ff.h"
#include "pindef.h"
#include "macros.h"
#include "spi.h"
#include "lcd.h"
#include "config.h"
#include <string.h>
#include <stdio.h>
#include <util/delay.h>
#include <avr/wdt.h>
#include <avr/pgmspace.h>
#include <avr/interrupt.h>

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

typedef struct {
	char n[32];
	char e[1+3+1];
	char a[32];
	char p[128];
	unsigned char nLen;
	unsigned char eLen;
	unsigned char aLen;
	unsigned char pLen;
} FileName83;

typedef struct {
	FileName83 fn;

	unsigned char title[31];
	unsigned char titleLen;

	#ifdef calc_song_length
	unsigned long bps;
	unsigned long duration;
	unsigned long headerLoc;
	#endif

	FIL fh;

	unsigned char open;
} MP3File;

void MP3DataTx(unsigned char *, unsigned char);

unsigned char MP3Open(FILINFO *, MP3File *, char *);

void MP3WriteReg(unsigned char, unsigned char, unsigned char);

void MP3WriteRegS(unsigned char, unsigned short);

unsigned int MP3ReadReg(unsigned char);

void MP3SetVol(unsigned char, unsigned char);

unsigned char MP3Play(MP3File *);

void MP3PlayToEnd(MP3File *);

void MP3Init(unsigned char, unsigned char);

#define mp3_inc
#endif
