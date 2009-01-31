#include "gpslogger.h"

#include "printDouble.h"
#include "timer.h"

volatile unsigned char newLogButtonFlag = 0;
volatile unsigned char cardSafeVar = 0;

ISR(BADISR_vect)
{
	// vector declared to catch unexpected interrupts
	// not supposed to happen
}

#ifndef __AVR_ATmega32__
ISR(WDT_vect)
{
	// this occures before a watch dog reset if it's supposed to
}
#endif

void _10ms()
{
	// function occurs every 10ms
	// disk_timerproc (mmc.c) needs to be called
	disk_timerproc();
}

#ifndef __AVR_ATmega32__
ISR(TIMER2_COMPA_vect)
#else
ISR(TIMER2_COMP_vect)
#endif
{
	_10ms();
	TCNT2 = 0;
}

unsigned long get_fattime()
{
	// TODO, timestamp for file attributes
	return 0;
}

void cardSafeLED(unsigned char d)
{
	// TODO, a LED that indicates whether it's safe to remove the SD card or not
	// d = 1 means unsafe, 0 = safe
	// this will also indicated whether the logger is actively logging
	// rapid flashing means disk error and disk can be removed
}

void GPSLED(unsigned char d)
{
	// TODO, a LED that indicates whether or not the GPS has a fix
	// d = 1 means yes
}

unsigned char cardSafe()
{
	// TODO, a button that while pressed, the SD card should become safe to remove
	// note that the user must still wait for the LED to be off
	// return 1 if button is pressed, 0 if not

	// TODO, battery status checker, if power level too low, return 1
	// This prevents damaging the FAT

	return cardSafeVar;
}

/* returns whether or not the "start new log" button has been pressed */
unsigned char newLog()
{
	unsigned char r = newLogButtonFlag;
	newLogButtonFlag = 0;
	return r;
}

/* TODO: find interrupt vector name after implementing */
/*
ISR(edge interrupt)
{
	newLogButtonFlag = 1;
}
*/

// function reads a string from a file, seperated with delimiters
StringWord f_readWord(FIL * fil__)
{
	StringWord result;
	result.length = 0;
	result.endOfFile = 0;
	
	unsigned char delimA[3] = {' ', '\n', '\r'};

	unsigned char rc;
	unsigned char d[1] = {0};
	unsigned char exitFlag = 0;
	
	while(1)
	{
		// read in one character
		exitFlag = f_read(fil__, &d, 1, &rc);
		if(exitFlag == 0 && rc == 1)
		{
			unsigned char i;
			for(i = 0; i < 3; i++)
			{
				// check if match any of the delimiters
				if(delimA[i] == d[0])
				{
					exitFlag = 1;
					break;
				}
			}
			if(exitFlag == 0)
			{
				// add to string
				result.c[result.length] = d[0];
				result.length += 1;
			}
			else break;
		}
		else
		{
			if(rc == 0)
			{
				result.endOfFile = 1;
			}
			break;
		}
	}
	result.c[result.length] = 0; // null termination
	
	return result;
}

/* function checks if there's a error, if there is, show a message, freeze the program and wait for reset while flashing the LED */
void f_checkErrorFreeze(unsigned int f_tffError, const char * s)
{
	if(f_tffError != 0)
	{
		#ifdef SerDebug
		fprintf(&serStdout, "err, %s\n\n", s);
		#endif

		unsigned char flasher = 0;
		while(1)
		{
			cardSafeLED(flasher);
			flasher ^= 1;
			_delay_ms(250);
		}
	}
}

int main()
{
	#ifdef __AVR_ATmega32__
	// enable watchdog reset (2 seconds)
	wdt_enable(WDTO_2S);
	#else
	// enable watchdog reset (8 seconds)
	wdt_enable(WDTO_8S);
	#endif

	wdt_reset();

	static unsigned int f_tffError = 0;
	FATFS fatfs_;

	FIL configFile;
	#ifdef USE_KML
	FIL pathFile;
	#endif
	FIL pointFile;

	GPSData GData;

	unsigned long writeRate = defaultDelay;
	signed long timeZone = defaultTimeZone;

	StringWord tempS1;
	StringWord tempS2;

	// enable interrupt
	sei();

	// Initialize UART Pins
	/* 
	// commented out because it's unnecessary
	sbi(PORTD, 0);
	sbi(PORTD, 1);
	cbi(DDRD, 0);
	sbi(DDRD, 1);
	*/

	// TODO, initialize ports for LEDs and Buttons
	// TODO, edge interrupt for "new log" button

	// start serial port
	serInit(GPSBaud);

	// start timer2, interrupt every 10 ms
	// these functions are found in timer.h
	timer2Restart(0, 0);
	timer2MatchAIntOn(78);
	timer2Restart(1024, 0);
	
	while(cardSafe() != 0)wdt_reset(); // maybe the user doesn't want to start yet

	// open disk, flash LED if disk bad or missing
	while(1)
	{
		wdt_reset();
		unsigned char flasher = 0;

		#ifdef SerDebug
		fprintf(&serStdout, "\n\n-Start");
		#endif

		// start the disk
		f_tffError = disk_initialize(0);
		f_tffError |= f_mount(0, &fatfs_);

		#ifdef SerDebug
		fprintf(&serStdout, " %d-\n", f_tffError);
		#endif

		if(f_tffError == 0) break;
		
		cardSafeLED(flasher);
		flasher ^= 1;
		_delay_ms(250);
	}

	wdt_reset();

	#ifdef ReadConfig

	cardSafeLED(1);

	/* either find or create the configuration file, if not found, write default values to it */

	f_tffError = f_open(&configFile, "/config.txt", FA_WRITE);

	if(f_tffError != 0)
	{
		f_tffError = f_open(&configFile, "/config.txt", FA_WRITE | FA_OPEN_ALWAYS | FA_CREATE_ALWAYS);

		#ifdef SerDebug
		fprintf(&serStdout, "M c\n");
		#endif
		
		f_checkErrorFreeze(f_tffError, "cant M c");

		// write default values to config file
		f_printf(&configFile, "delay %d\ntimezone %d\nend", defaultDelay, defaultTimeZone);
		f_tffError = f_sync(&configFile);
		f_tffError |= f_truncate(&configFile);
	}

	#ifdef SerDebug
	else fprintf(&serStdout, "c F\n");
	#endif

	f_tffError |= f_close(&configFile);

	f_tffError = f_open(&configFile, "/config.txt", FA_READ);

	f_checkErrorFreeze(f_tffError, "cant O c");

	#ifdef SerDebug
	fprintf(&serStdout, "R c\n");
	#endif

	wdt_reset();

	/* read in configuration */

	unsigned char ts[5] = {0,0,0,0,0};
	StringWord d;
	do
	{
		// read next label, strip unwanted characters
		do
		{
			d = f_readWord(&configFile);
		}
		while((d.c[0] == 0 || ((d.c[0] == ' ' || d.c[0] == '\n' || d.c[0] == '\r') && d.c[1] == 0)) && d.endOfFile == 0);

		if(strcmp(d.c, "delay") == 0)
		{
			// found delay, read next word (convert to a number) to set
			d = f_readWord(&configFile);
			writeRate = (unsigned long)strtod(d.c, &ts);

			#ifdef SerDebug
			fprintf(&serStdout, "i = %d\n", writeRate);
			#endif
		}
		else if(strcmp(d.c, "timezone") == 0)
		{
			// similar to above
			d = f_readWord(&configFile);
			timeZone = (signed long)strtod(d.c, &ts);

			#ifdef SerDebug
			fprintf(&serStdout, "tz = %d\n", (signed int)timeZone);
			#endif
		}
	}
	while(strcmp(d.c, "end") != 0 && d.endOfFile == 0);
	// end of file or end of configuration found, exit loop

	f_tffError = f_close(&configFile);

	#endif // ifdef ReadConfig

	wdt_reset();

	cardSafeLED(0);

	#ifdef SerDebug
	fprintf(&serStdout, "I GPS\n");
	#endif

	GPSInit(1, timeZone);

	#ifdef SerDebug
	fprintf(&serStdout, "GPS F\n");
	#endif

	do
	{
		wdt_reset();
		GPSRead(&GData);
		#ifdef SerDebug
		fprintf(&serStdout, "GPS R1\n");
		#endif
		wdt_reset();
		GPSRead(&GData);
		#ifdef SerDebug
		fprintf(&serStdout, "GPS R2\n");
		#endif
	}
	while(GData.valid == 0);

	#ifdef SerDebug
	fprintf(&serStdout, "GPS V\n");
	#endif

	if(GData.minute < 10){tempS1.c[0] = '0'; tempS1.c[1] = 0;}else tempS1.c[0] = 0;
	if(GData.second < 10){tempS2.c[0] = '0'; tempS2.c[1] = 0;}else tempS2.c[0] = 0;

	if(cardSafe() != 0)
	{
		#ifdef SerDebug
		fprintf(&serStdout, "CS\n");
		#endif

		/* this means the card has been removed, since this also 
		/  means the user could have changed the configurations,
		/  when the user releases the button, the avr resets
		/  instantly to read the new configuration */

		while(cardSafe() != 0)wdt_reset();
		wdt_enable(WDTO_15MS);
		while(1);
	}

	cardSafeLED(1);

	/* open or create the log files, skip to end, write a date and time for the new log */

	#ifdef USE_KML // path file only needed if we are using KML

	f_tffError = f_open(&pathFile, "/path.txt", FA_READ | FA_WRITE);
	if(f_tffError != 0)
	{
		#ifdef SerDebug
		fprintf(&serStdout, "M pa\n");
		#endif

		f_tffError = f_open(&pathFile, "/path.txt", FA_READ | FA_WRITE | FA_OPEN_ALWAYS | FA_CREATE_ALWAYS);
	}

	#ifdef SerDebug
	else fprintf(&serStdout, "path O\n");
	#endif

	f_checkErrorFreeze(f_tffError, "cant O pa");

	f_tffError = f_lseek(&pathFile, pathFile.fsize); // skip to end

	f_printf(&pathFile, "\n\nRST DMY %d/%d/%d T %d:%s%d:%s%d Int %d\n\n", GData.day, GData.month, GData.year, GData.hour, tempS1.c, GData.minute, tempS2.c, GData.second, writeRate);

	f_tffError = f_sync(&pathFile);
	f_tffError |= f_truncate(&pathFile);
	f_checkErrorFreeze(f_tffError, "cant W pa");

	#endif // ifdef USE_KML

	f_tffError = f_open(&pointFile, "/point.txt", FA_READ | FA_WRITE);
	if(f_tffError != 0)
	{
		#ifdef SerDebug
		fprintf(&serStdout, "M pt\n");
		#endif

		f_tffError = f_open(&pointFile, "/point.txt", FA_READ | FA_WRITE | FA_OPEN_ALWAYS | FA_CREATE_ALWAYS);
	}

	#ifdef SerDebug
	else fprintf(&serStdout, "pt O\n");
	#endif

	f_checkErrorFreeze(f_tffError, "cant O pt");

	f_tffError = f_lseek(&pointFile, pointFile.fsize);

	#ifdef USE_KML
	f_printf(&pointFile, "\n\nRST DMY %d/%d/%d T %d:%s%d:%s%d\n\n", GData.day, GData.month, GData.year, GData.hour, tempS1.c, GData.minute, tempS2.c, GData.second);
	#else
	f_printf(&pointFile, "D,M,Y,H,M,S,Lo,La,S,H,A,\n");
	#endif

	f_tffError = f_sync(&pointFile);
	f_tffError |= f_truncate(&pointFile);
	f_checkErrorFreeze(f_tffError, "cant W pt");

	#ifdef SerDebug
	fprintf(&serStdout, "log\n");
	#endif

	while(1)
	{
		GPSData tempG;
		unsigned long i;
		// delay in seconds, 2 NMEA sentences are sent in 1 second
		for(i = 0; i < writeRate; i++)
		{
			wdt_reset();
			GPSRead(&GData);
			wdt_reset();
			GPSRead(&GData);
			if(GData.valid == 1)
			{
				tempG = GData;
				GPSLED(1);
			}
			else
			{
				GPSLED(0);
			}
		}
		GData = tempG;
		if(GData.valid == 1) // only record if valid
		{
			/* stdio.h can't print decimal places so these functions saves the double variables as strings */
			StringWord longSW = printDouble(GData.longitude, 8);
			StringWord latSW = printDouble(GData.latitude, 8);
			StringWord altSW = printDouble(GData.altitude, 1);
			StringWord speedSW = printDouble(GData.speed, 1);
			StringWord headingSW = printDouble(GData.heading, 1);

			// if button not pressed
			if(cardSafe() == 0)
			{
				cardSafeLED(1);

				// make sure the minute and second of time data have two digits
				if(GData.minute < 10){tempS1.c[0] = '0'; tempS1.c[1] = 0;}else tempS1.c[0] = 0;
				if(GData.second < 10){tempS2.c[0] = '0'; tempS2.c[1] = 0;}else tempS2.c[0] = 0;

				// if new log button pressed, insert header
				if(newLog())
				{
					#ifdef USE_KML
					f_printf(&pointFile, "\n\nNL DMY %d/%d/%d T %d:%s%d:%s%d\n\n", GData.day, GData.month, GData.year, GData.hour, tempS1.c, GData.minute, tempS2.c, GData.second);
					f_printf(&pathFile, "\n\nNL DMY %d/%d/%d T %d:%s%d:%s%d Int %d\n\n", GData.day, GData.month, GData.year, GData.hour, tempS1.c, GData.minute, tempS2.c, GData.second, writeRate);
					#else
					f_printf(&pointFile, "D,M,Y,H,M,S,Lo,La,S,H,A,\n");
					#endif

					#ifdef SerDebug
					fprintf(&serStdout, "NL\n");
					#endif
				}

				fprintf(&serStdout, "%d:%s%d:%s%d %s,%s,%s\n", GData.hour, tempS1.c, GData.minute, tempS2.c, GData.second, longSW.c, latSW.c, altSW.c);

				#ifdef USE_KML

				/* write to file, data will be in KML-ready format */				

				f_printf(&pathFile, "%s,%s,%s\n", longSW.c, latSW.c, altSW.c);

				f_tffError = f_sync(&pathFile);
				f_tffError |= f_truncate(&pathFile);

				f_printf(&pointFile, "<Placemark>\n");
				f_printf(&pointFile, "\t<name></name>\n");

				f_printf(&pointFile, "\t<description>\n");
				f_printf(&pointFile, "Day: %d  Month: %d  Year: %d\n", GData.day, GData.month, GData.year);
				f_printf(&pointFile, "Time: %d:%s%d:%s%d\n", GData.hour, tempS1.c, GData.minute, tempS2.c, GData.second);
				f_printf(&pointFile, "Speed: %sKMH\nHeading: %s\nAltitude: %sM\n", speedSW.c, headingSW.c, altSW.c);
				f_printf(&pointFile, "\t</description>\n");

				f_printf(&pointFile, "\t<LookAt>\n");

				f_printf(&pointFile, "\t\t<longitude>");
				f_printf(&pointFile, "%s", longSW.c);
				f_printf(&pointFile, "</longitude>\n");

				f_printf(&pointFile, "\t\t<latitude>");
				f_printf(&pointFile, "%s", latSW.c);
				f_printf(&pointFile, "</latitude>\n");

				f_printf(&pointFile, "\t\t<heading>");
				f_printf(&pointFile, "%d", (signed int)GData.heading);
				f_printf(&pointFile, "</heading>\n");

				f_printf(&pointFile, "\t</LookAt>\n");

				f_printf(&pointFile, "\t<styleUrl>#msn_ylw-pushpin</styleUrl>\n");

				f_printf(&pointFile, "\t<Point>\n");

				f_printf(&pointFile, "\t\t<extrude>1</extrude>\n");
				f_printf(&pointFile, "\t\t<altitudeMode>absolute</altitudeMode>\n");

				f_printf(&pointFile, "\t\t<coordinates>");

				f_printf(&pointFile, "%s,%s,%s", longSW.c, latSW.c, altSW.c);

				f_printf(&pointFile, "</coordinates>\n");

				f_printf(&pointFile, "\t</Point>\n");
				f_printf(&pointFile, "</Placemark>\n");

				#else // ifdef USE_KML

				/* CSV format */

				f_printf(&pointFile, "%d,%d,%d,", GData.day, GData.month, GData.year);
				f_printf(&pointFile, "%d,%d,%d,", GData.hour, GData.minute, GData.second);
				f_printf(&pointFile, "%s,%s,", longSW.c, latSW.c);
				f_printf(&pointFile, "%s,%s,%s,\n", speedSW.c, headingSW.c, altSW.c);
				
				#endif // ifdef USE_KML

				f_tffError |= f_sync(&pointFile);
				f_tffError |= f_truncate(&pointFile);

				_delay_ms(100); // delay to make LED visible

				cardSafeLED(0);
				
				/* file should be written, if error, close and reopen the file and skip to end */
				unsigned char flasher = 0;
				if(f_tffError != 0)
				{
					#ifdef SerDebug
					fprintf(&serStdout, "rw err\n");
					#endif

					while(f_tffError != 0)
					{
						f_tffError = f_close(&pointFile);

						#ifdef USE_KML
						f_tffError = f_close(&pathFile);
						#endif

						cardSafeLED(flasher);
						flasher ^= 1;
						_delay_ms(250);

						f_tffError = disk_initialize(0);
						f_tffError |= f_mount(0, &fatfs_);

						f_tffError = f_open(&pointFile, "/point.txt", FA_WRITE);

						#ifdef USE_KML
						f_tffError |= f_open(&pathFile, "/path.txt", FA_WRITE);
						f_tffError |= f_lseek(&pathFile, pathFile.fsize);
						#endif

						f_tffError |= f_lseek(&pointFile, pointFile.fsize);
					}
				}
			}
			else
			{
				#ifdef SerDebug
				fprintf(&serStdout, "CS\n");
				#endif

				/* this means the card has been removed, since this also 
				/  means the user could have changed the configurations,
				/  when the user releases the button, the avr resets
				/  instantly to read the new configuration */

				while(cardSafe() != 0)wdt_reset();
				wdt_enable(WDTO_15MS);
				while(1);
			}
		}
		else if(GData.valid == 0)
		{
			#ifdef SerDebug
			fprintf(&serStdout, "NV\n");
			#endif
		}
	}

	return 0;
}
