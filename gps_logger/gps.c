#include "gps.h"
#include "print.h"

/* function stores a string until a comma or * is found */
NMEAWord NMEAReadWord()
{
	NMEAWord result;

	wdt_reset();

	result.length = 0;
	
	unsigned char bytesAvail;
	unsigned char data = 0;
	
	while ( data != ',' && data != '*' )
	{
		// wait for byte from UART then store
		do { data = serRx ( &bytesAvail ); } while ( bytesAvail == 0 );
		
		if ( data != ','  && data != '*' )
		{
			// add to string if not delimiter
			result.c [ result.length ] = data;
			result.length += 1;
		}
	}
	result.c [ result.length ] = 0; // null terminate
	
	// if * found, that means it's the last string in sentence
	if ( data == '*' ) result.isLast = 1; else result.isLast = 0;
	
	return result;
}

/* function converts string to a double */
void NMEAParseLatitude ( NMEAWord d, GPSData * g )
{
	unsigned char degree = (d.c[0] - '0') * 10 + (d.c[1] - '0');
	d.c[0] = '0';
	d.c[1] = '0';
	unsigned char ts [ 10 ] = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
	double t = (strtod ( d.c, &ts ) / (double)60.0) + (double)degree;

	g->latitude = t;
}

/* function converts string to a double */
void NMEAParseLongitude ( NMEAWord d, GPSData * g )
{
	unsigned char degree = (d.c[0] - '0') * 100 + (d.c[1] - '0') * 10 + (d.c[2] - '0');
	d.c[0] = '0';
	d.c[1] = '0';
	d.c[2] = '0';
	unsigned char ts [ 10 ] = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
	double t = (strtod ( d.c, &ts ) / (double)60.0) + (double)degree;

	g->longitude = t;
}

/* function converts string to store time data */
void NMEAParseTime ( NMEAWord d, GPSData * g )
{
	g->hour_ = (d.c[0] - '0') * 10 + (d.c[1] - '0');
	g->minute = (d.c[2] - '0') * 10 + (d.c[3] - '0');
	g->second = (d.c[4] - '0') * 10 + (d.c[5] - '0');
}

/* function converts string to store date data */
void NMEAParseDate ( NMEAWord d, GPSData * g )
{
	g->day_ = (d.c[0] - '0') * 10 + (d.c[1] - '0');
	g->month_ = (d.c[2] - '0') * 10 + (d.c[3] - '0');
	g->year_ = (d.c[4] - '0') * 10 + (d.c[5] - '0');
}

/* function processes timezone and adjusts for skipping days */
void GPSTimeProc ( GPSData * g )
{
	signed char daysInMonth [ 13 ] = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

	signed char gDay = g->day_;
	signed char gMonth = g->month_;
	signed int gYear = g->year_;
	signed char gHour = g->hour_;

	if ( g->year % 4 == 0 )
	{
		daysInMonth[2] = 29;
	}

	signed char hHere = gHour + GTimeZone;

	if ( hHere < 0 )
	{
		gDay -= 1;
	}
	else if ( hHere >= 24 )
	{
		gDay += 1;
	}

	hHere += 24;
	hHere %= 24;

	if ( gDay > daysInMonth [ gMonth ] )
	{
		gDay = 1;
		gMonth += 1;
	}
	else if ( gDay == 0 )
	{
		gMonth -= 1;
		gDay = daysInMonth [ gMonth ];
	}

	if ( gMonth == 0 )
	{
		gMonth = 12;
		gYear -= 1;
	}
	else if ( gMonth == 13 )
	{
		gMonth = 1;
		gYear += 1;
	}

	g->day = gDay;
	g->month = gMonth;
	g->year = gYear + 2000;
	g->hour = hHere;
}

/* function waits for, reads, and processes one NMEA sentence */
unsigned char GPSRead ( GPSData * g )
{
	unsigned char r = 0;
	NMEAWord NMEAWord_;
	unsigned char ts [ 6 ] = { 0, 0, 0, 0, 0, 0 };
	
	wdt_reset();
	serSkipTo ( '$' ); // wait for new sentence
	serSkipTo ( 'G' );
	serSkipTo ( 'P' );
	
	NMEAWord_ = NMEAReadWord(); // read sentence header
	
	/* see which header it is and process according to format */
	if ( strcmp( NMEAWord_.c, "GGA" ) == 0 )
	{
		NMEAParseTime ( NMEAReadWord ( ), g );

		NMEAParseLatitude ( NMEAReadWord ( ), g );

		// south and west means negatives, remember that
		NMEAWord_ = NMEAReadWord();
		if ( NMEAWord_.c [ 0 ] == 'S' ) g->latitude *= (double)-1.0;

		NMEAParseLongitude ( NMEAReadWord ( ), g );

		NMEAWord_ = NMEAReadWord();
		if ( NMEAWord_.c [ 0 ] == 'W' ) g->longitude *= (double)-1.0;

		NMEAWord_ = NMEAReadWord();
		if ( NMEAWord_.c [ 0 ] != '0' ) g->valid = 1; else g->valid = 0;

		NMEAWord_ = NMEAReadWord();

		NMEAWord_ = NMEAReadWord();

		NMEAWord_ = NMEAReadWord();
		g->altitude = strtod ( NMEAWord_.c, &ts );

		g->updatedFlag = 1;

		serSkipTo ( '\n' ); // finish reading rest of buffer

		r = 1;
	}
	/*
	else if ( strcmp( NMEAWord_.c, "GLL" ) == 0 )
	{
		NMEAParseLatitude ( NMEAReadWord ( ), g );

		NMEAWord_ = NMEAReadWord();
		if ( NMEAWord_.c [ 0 ] == 'S' ) g->latitude *= (double)-1.0;

		NMEAParseLongitude ( NMEAReadWord ( ), g );

		NMEAWord_ = NMEAReadWord();
		if ( NMEAWord_.c [ 0 ] == 'W' ) g->longitude *= (double)-1.0;

		NMEAParseTime ( NMEAReadWord ( ), g );

		NMEAWord_ = NMEAReadWord();
		if ( NMEAWord_.c [ 0 ] == 'A' ) g->valid = 1; else g->valid = 0;

		g->updatedFlag = 1;

		serSkipTo ( '\n' );

		r = 2;
	}
	*/
	else if ( strcmp( NMEAWord_.c, "RMC" ) == 0 )
	{
		NMEAParseTime ( NMEAReadWord ( ), g );

		NMEAWord_ = NMEAReadWord();
		if ( NMEAWord_.c [ 0 ] == 'A' ) g->valid = 1; else g->valid = 0;

		NMEAParseLatitude ( NMEAReadWord ( ), g );

		NMEAWord_ = NMEAReadWord();
		if ( NMEAWord_.c [ 0 ] == 'S' ) g->latitude *= (double)-1.0;

		NMEAParseLongitude ( NMEAReadWord ( ), g );

		NMEAWord_ = NMEAReadWord();
		if ( NMEAWord_.c [ 0 ] == 'W' ) g->longitude *= (double)-1.0;

		NMEAWord_ = NMEAReadWord();
		g->speed = strtod ( NMEAWord_.c, &ts ) * (double)1.852;

		NMEAWord_ = NMEAReadWord();
		g->heading = strtod ( NMEAWord_.c, &ts );

		NMEAParseDate ( NMEAReadWord ( ), g );

		g->updatedFlag = 1;

		serSkipTo ( '\n' );

		r = 5;
	}
	/*
	else if ( strcmp( NMEAWord_.c, "VTG" ) == 0 )
	{
		NMEAWord_ = NMEAReadWord();
		g->heading = strtod ( NMEAWord_.c, &ts );

		NMEAWord_ = NMEAReadWord(); // ref

		NMEAWord_ = NMEAReadWord(); // course

		NMEAWord_ = NMEAReadWord(); // ref

		NMEAWord_ = NMEAReadWord(); // speed

		NMEAWord_ = NMEAReadWord(); // unit

		NMEAWord_ = NMEAReadWord();
		g->speed = strtod ( NMEAWord_.c, &ts );

		g->updatedFlag = 1;

		serSkipTo ( '\n' );

		r = 6;
	}
	*/
	else
	{
		serSkipTo ( '\n' );
	}

	GPSTimeProc ( g );

	return r;
}

static int gps_putchar ( unsigned char c, FILE *stream )
{
	static unsigned char checksum;
	if ( c == '$' ) checksum = '*'; else checksum ^= c; // clear checksum if new sentence
	serTx ( c ); // send char
	if( c == '*' )
	{
		/* end of sentence, print the checksum in hex, then new line sequence */

		c = (checksum & 0xF0) >> 4;
		c += '0';
		if ( c > '9' )
		{
			c = c - '0' - 10 + 'A';
		}
		serTx ( c );
		c = checksum & 0x0F;
		c += '0';
		if ( c > '9' )
		{
			c = c - '0' - 10 + 'A';
		}
		serTx ( c );

		serTx ( '\r' );
		serTx ( '\n' );
	}
	return 0;
}

/* functions below prints out NMEA input messages */

/*
void NMEAQuery ( unsigned char m )
{
	fprintf(&gpsStdout, "$PSRF103,0%d,01,00,01*", m);
}
*/

void NMEASetRate ( unsigned char m, unsigned int r )
{
	fprintf(&gpsStdout, "$PSRF103,%d,00,%d,01*", m, r);
}

/*
void NMEASetSerialPort ( unsigned int b )
{
	fprintf(&gpsStdout, "$PSRF100,1,%d,8,1,0*", b);
}
*/

/* function initliazes GPS with port settings and message rates, only GGA and RMC are needed for GPS data, change baud rate for different modules, timezone specified also */
void GPSInit ( unsigned char r, signed char tz )
{
	GTimeZone = tz;

	wdt_reset();

	// code below is commented out because they are redundent and unnecessary
	//loop_until_bit_is_set(UCSRA, UDRE); // wait while previous tx is finished
	//serInit ( GPSBaud );
	//_delay_ms ( 125 );
	//NMEASetSerialPort ( GPSBaud );
	//_delay_ms ( 250 ); // delay needed or next command won't work

	wdt_reset();
	NMEASetRate ( 0, r );
	NMEASetRate ( 1, 0 );
	NMEASetRate ( 2, 0 );
	NMEASetRate ( 3, 0 );
	wdt_reset();
	NMEASetRate ( 4, r );
	NMEASetRate ( 5, 0 );
	NMEASetRate ( 6, 0 );
	NMEASetRate ( 8, 0 );
	wdt_reset();
}
