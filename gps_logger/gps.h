#ifndef gps_inc

/* GPS data structure, string structure, and function prototypes */

#include "ser.h"
#include <stdlib.h>
#include <stdio.h>
#include <avr/wdt.h>
#include <util/delay.h>

#define GPSBaud 4800

typedef struct _GPSData
{
	unsigned char valid;
	unsigned char updatedFlag;

	unsigned char hour;
	unsigned char hour_;
	unsigned char minute;
	unsigned char second;
	unsigned char day;
	unsigned char month;
	unsigned int year;
	unsigned char day_;
	unsigned char month_;
	unsigned int year_;

	double latitude;
	double longitude;

	double altitude;
	double speed;
	double heading;
} GPSData;

typedef struct _NMEAWord
{
	unsigned char c [ 15 ];
	unsigned char length;
	unsigned char isLast;
} NMEAWord;

signed char GTimeZone;

unsigned char GPSRead(GPSData *);

static int gps_putchar ( unsigned char c, FILE *stream );
static FILE gpsStdout = FDEV_SETUP_STREAM ( gps_putchar, NULL, _FDEV_SETUP_WRITE );

void GPSInit(unsigned char, signed char);

#define gps_inc
#endif
