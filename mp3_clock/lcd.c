#include "lcd.h"

// below are custom LCD characters, stored in flash

const unsigned char upperlinechar[8] PROGMEM = {
	0b00011111,
	0b00011111,
	0b00011111,
	0b00000000,
	0b00000000,
	0b00000000,
	0b00000000,
	0b00000000
};

const unsigned char lowerlinechar[8] PROGMEM = {
	0b00000000,
	0b00000000,
	0b00000000,
	0b00000000,
	0b00000000,
	0b00011111,
	0b00011111,
	0b00011111
};

const unsigned char bothlinechar[8] PROGMEM = {
	0b00011111,
	0b00011111,
	0b00000000,
	0b00000000,
	0b00000000,
	0b00000000,
	0b00011111,
	0b00011111
};

const unsigned char largecolonleftchar[8] PROGMEM = {
	0b00000000,
	0b00000000,
	0b00000001,
	0b00000011,
	0b00000011,
	0b00000001,
	0b00000000,
	0b00000000
};

const unsigned char largecolonrightchar[8] PROGMEM = {
	0b00000000,
	0b00000000,
	0b00010000,
	0b00011000,
	0b00011000,
	0b00010000,
	0b00000000,
	0b00000000
};

const unsigned char upchar[8] PROGMEM = {
	0b00000000,
	0b00000100,
	0b00001110,
	0b00010101,
	0b00000100,
	0b00000100,
	0b00000100,
	0b00000000
};

const unsigned char downchar[8] PROGMEM = {
	0b00000000,
	0b00000100,
	0b00000100,
	0b00000100,
	0b00010101,
	0b00001110,
	0b00000100,
	0b00000000
};

const unsigned char numchar[10][6] PROGMEM = {
	{
		xxc, ulc, xxc,
		xxc, llc, xxc,
	},
	{
		ulc, xxc, ssc,
		llc, xxc, llc,
	},
	{
		ulc, blc, xxc,
		xxc, llc, llc,
	},
	{
		ulc, blc, xxc,
		llc, llc, xxc,
	},
	{
		xxc, llc, xxc,
		ssc, ssc, xxc,
	},
	{
		xxc, blc, ulc,
		llc, llc, xxc,
	},
	{
		xxc, blc, ulc,
		xxc, llc, xxc,
	},
	{
		ulc, ulc, xxc,
		ssc, ssc, xxc,
	},
	{
		xxc, blc, xxc,
		xxc, llc, xxc,
	},
	{
		xxc, blc, xxc,
		llc, llc, xxc,
	}
};

void LCDSend(unsigned char data, unsigned char registerSelect)
{
	if(registerSelect != 0) // decide RS pin state
	{
		sbi(LCDCtrlPort, LCDRSPin); // send text
	}
	else
	{
		cbi(LCDCtrlPort, LCDRSPin); // send command
	}

	unsigned char pin_dst[4] = {LCDDataBit0, LCDDataBit1, LCDDataBit2, LCDDataBit3};

	// send upper 4 bits
	for(unsigned char i = 0; i < 4; i++)
	{
		if(bit_is_set(data, i + 4))
		{
			sbi(LCDDataPort, pin_dst[i]);
		}
		else
		{
			cbi(LCDDataPort, pin_dst[i]);
		}
	}
	
	sbi(LCDCtrlPort, LCDEPin);		
	cbi(LCDCtrlPort, LCDEPin);

	// send lower 4 bits
	for(unsigned char i = 0; i < 4; i++)
	{
		if(bit_is_set(data, i))
		{
			sbi(LCDDataPort, pin_dst[i]);
		}
		else
		{
			cbi(LCDDataPort, pin_dst[i]);
		}
	}

	sbi(LCDCtrlPort, LCDEPin);		
	cbi(LCDCtrlPort, LCDEPin);

	_delay_us(40); // wait for command to execute
}

void LCDPrintTime(unsigned char h, unsigned char m, unsigned char ampm)
{
	unsigned char a[4];

	// handle 24/12 hour modes
	if(h == 0)
	{
		h = 24;
	}

	if(ampm != 0)
	{
		h %= 12;
		if(h == 0)
		{
			h = 12;
		}
	}

	// get individual digits
	a[0] = h / 10;
	a[1] = h % 10;
	a[2] = m / 10;
	a[3] = m % 10;

	// print characters
	for(unsigned char i = 0; i < 4; i++)
	{
		// handle spacing
		unsigned char j;
		if(i > 1)
		{
			j = 1;
		}
		else
		{
			j = 0;
		}

		unsigned char d = a[i];

		unsigned char e = (i * 4) + j;

		LCDSend(0b10000000 + e, 0);
		LCDSend(pgm_read_byte(&numchar[d][0]), 1);
		LCDSend(pgm_read_byte(&numchar[d][1]), 1);
		LCDSend(pgm_read_byte(&numchar[d][2]), 1);
		LCDSend(0b11000000 + e, 0);
		LCDSend(pgm_read_byte(&numchar[d][3]), 1);
		LCDSend(pgm_read_byte(&numchar[d][4]), 1);
		LCDSend(pgm_read_byte(&numchar[d][5]), 1);
	}
	
	// print colon and spacing
	LCDSend(0b10000000 + 4 + 3, 0);
	LCDSend(largecolonleftaddr, 1);
	LCDSend(0b10000000 + 8, 0);
	LCDSend(largecolonrightaddr, 1);
	LCDSend(0b11000000 + 4 + 3, 0);
	LCDSend(largecolonleftaddr, 1);
	LCDSend(0b11000000 + 8, 0);
	LCDSend(largecolonrightaddr, 1);
	LCDSend(0b10000000 + 3, 0);
	LCDSend(' ', 1);
	LCDSend(0b11000000 + 3, 0);
	LCDSend(' ', 1);
	LCDSend(0b10000000 + 12, 0);
	LCDSend(' ', 1);
	LCDSend(0b11000000 + 12, 0);
	LCDSend(' ', 1);
}

void LCDSetPos(unsigned char x, unsigned char y)
{
	// this function sets the next position to be writen on the LCD
	// LCDSetPos(1, 1); would be the first character of the first line
	if(y == 2)
	{
		LCDSend(0b11000000 + x - 1, 0);
	}
	else
	{
		LCDSend(0b10000000 + x - 1, 0);
	}
}

void LCDPrint_P(const char * data, unsigned char x, unsigned char y)
{
	LCDSetPos(x, y);
	// This function lets you send a string to the LCD
	char c;
	while((c = pgm_read_byte(data++)))
	{
		LCDSend(c, 1);
	}
}

void LCDPrint(char * data)
{
	// This function lets you send a string to the LCD
	while(*data)
	{
		LCDSend(*data++, 1);
	}
}

void LCDClear(unsigned char y)
{
	// clears a line by writing 16 spaces to it
	LCDPrint_P(PSTR("                "), 1, y);
	LCDSetPos(1, y);
}

void LCDBL(unsigned char data)
{
	// turn backlight on or off
	if(data != 0)
	{
		sbi(LCDBLPort, LCDBLPin);
	}
	else
	{
		cbi(LCDBLPort, LCDBLPin);
	}
}

void LCDCustomChar(unsigned char address, unsigned char * dataArray)
{
	// send custom character definition
	unsigned char addr = address * 8;
	LCDSend(0b01000000 + addr, 0);
	unsigned char i;
	for(i = 0; i < 8; i++)
	{
		LCDSend(dataArray[i], 1);
	}
}

void LCDLoadChars()
{
	// load all of the custom characters into LCD

	unsigned char temp[8];

	for(unsigned char i = 0; i < 8; i++)
	{
		temp[i] = pgm_read_byte(&upperlinechar[i]);
	}
	LCDCustomChar(upperlineaddr, temp);

	for(unsigned char i = 0; i < 8; i++)
	{
		temp[i] = pgm_read_byte(&lowerlinechar[i]);
	}
	LCDCustomChar(lowerlineaddr, temp);

	for(unsigned char i = 0; i < 8; i++)
	{
		temp[i] = pgm_read_byte(&bothlinechar[i]);
	}
	LCDCustomChar(bothlineaddr, temp);

	for(unsigned char i = 0; i < 8; i++)
	{
		temp[i] = pgm_read_byte(&largecolonleftchar[i]);
	}
	LCDCustomChar(largecolonleftaddr, temp);

	for(unsigned char i = 0; i < 8; i++)
	{
		temp[i] = pgm_read_byte(&largecolonrightchar[i]);
	}
	LCDCustomChar(largecolonrightaddr, temp);

	for(unsigned char i = 0; i < 8; i++)
	{
		temp[i] = pgm_read_byte(&upchar[i]);
	}
	LCDCustomChar(upcharaddr, temp);

	for(unsigned char i = 0; i < 8; i++)
	{
		temp[i] = pgm_read_byte(&downchar[i]);
	}
	LCDCustomChar(downcharaddr, temp);
}

void LCDSoftReset()
{
	// 4 bit code
	LCDSend(0b00000001, 0); // all clear command
	_delay_us(1200); // wait for LCD to execute
	LCDSend(0b00000010, 0); // return home command
	_delay_us(1200); // wait for LCD to execute
	LCDSend(0b00101000, 0); // function set 4 bit, 2 line, 5x8 character
	LCDSend(0b00001100, 0); // cursor off
	LCDSetPos(1, 1); // set position to (1, 1)

	//LCDLoadChars();
}

void LCDInit()
{
	// LCD Initialization Function

	// backlight as output, turn backlight on
	sbi(LCDBLDDR, LCDBLPin);
	LCDBL(1);

	// 8 bit code
	//LCDDataDDR  0xFF; // data port as output

	// 4 bit code
	unsigned char pin_dst[4] = {LCDDataBit0, LCDDataBit1, LCDDataBit2, LCDDataBit3};

	for(unsigned char i = 0; i < 4; i++)
	{
		sbi(LCDDataDDR, pin_dst[i]);
	}

	sbi(LCDCtrlDDR, LCDRSPin); // control pins as output
	sbi(LCDCtrlDDR, LCDRWPin);
	sbi(LCDCtrlDDR, LCDEPin);

	// clear the port and pins
	LCDDataPort = 0;
	cbi(LCDCtrlPort, LCDRSPin);
	cbi(LCDCtrlPort, LCDRWPin);
	cbi(LCDCtrlPort, LCDEPin);

	sbi(LCDBLDDR, LCDBLPin);
	cbi(LCDBLPort, LCDBLPin);

	_delay_ms(125); // wait for LCD driver to warm up

	LCDSoftReset();

	LCDLoadChars();
}

int LCD_putc(char c, FILE *stream)
{
	// low level STDIO stream function

	if(c == '\n') // clear rest of line on new line character
	{
		for(unsigned char i = 0; i < 16; i++)
		{
			LCDSend(' ', 1);
		}
	}
	else
	{
		LCDSend(c, 1);
	}
	return 0;
}
