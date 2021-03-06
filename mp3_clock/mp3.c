#include "mp3.h"

#ifdef MP3_Custom_Code
// provided by VLSI, not used

#define CODE_SIZE 89
const unsigned char MP3Code_atab[CODE_SIZE] PROGMEM = { /* Register addresses */
    0x7, 0x6, 0x6, 0x7, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6,
    0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6,
    0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6,
    0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6,
    0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6,
    0x7, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6, 0x6
};
const unsigned short MP3Code_dtab[CODE_SIZE] PROGMEM = { /* Data to write */
    0x8030, 0x2a00, 0x0c40, 0x8031, 0x3613, 0x0024, 0x3e12, 0xb817,
    0x3e12, 0x3815, 0x3e05, 0xb814, 0x3615, 0x0024, 0x0000, 0x800a,
    0x3e10, 0xb803, 0x3e14, 0x7812, 0x3e13, 0xf80e, 0x4194, 0x380d,
    0x0000, 0x0011, 0x2800, 0x1395, 0xf400, 0x4442, 0x6206, 0x0012,
    0x0000, 0x0024, 0x2800, 0x1398, 0x4094, 0x0024, 0x2400, 0x1302,
    0x3613, 0x0024, 0x3210, 0x3840, 0x2900, 0x15c0, 0x32f0, 0x7801,
    0x4084, 0x4483, 0x3113, 0x1bc1, 0x6396, 0x9800, 0xf400, 0x40d5,
    0x3d00, 0x8024, 0x3a20, 0x8024, 0x36f3, 0x0024, 0x36f3, 0x4024,
    0x36f3, 0xd80e, 0x36f4, 0x5812, 0x36f0, 0x9803, 0x3405, 0x9014,
    0x36f3, 0x0024, 0x36f2, 0x1815, 0x2000, 0x0000, 0x36f2, 0x9817,
    0x8057, 0x4c02, 0x0024, 0xf1c2, 0x0024, 0x2000, 0x0000, 0xf7c0,
    0x0024
};
#endif

// transfer data via SPI to SDI
void MP3DataTx(unsigned char * d, unsigned char len)
{
	cbi(MP3_Port, MP3_xDCS_Pin);

	for(unsigned char i = 0; i < len; i++)
	{
		SPITx(d[i]);
	}

	sbi(MP3_Port, MP3_xDCS_Pin);
}

// opens a MP3 file, using FILINFO and a folder path, or use a null for FILINFO and use a file path
unsigned char MP3Open(FILINFO * fno, MP3File * mf, char * p)
{
	mf->open = 0;

	{ // open file depeding on how the file name is given

		FileName83 r;
		r.nLen = 0;
		r.eLen = 0;
		r.aLen = 0;
		r.pLen = 0;
		for(unsigned char i = 0; i < 128 && *p != 0; i++)
		{
			r.p[r.pLen] = *p;
			r.pLen++;
			p++;
		}
		r.p[r.pLen] = 0;
		if(fno != 0) // FILINFO provided, using folder path
		{
			if(r.p[r.pLen - 1] != '/')
			{
				r.p[r.pLen] = '/';
				r.pLen++;
			}
			for(unsigned char i = 0; i < 8 + 1 + 3 + 1; i++)
			{
				if(fno->fname[i] != '.' && fno->fname[i] != 0)
				{
					r.n[r.nLen] = fno->fname[i];
					r.nLen++;

					r.a[r.aLen] = fno->fname[i];
					r.aLen++;

					r.p[r.pLen] = fno->fname[i];
					r.pLen++;
				}
				else if(fno->fname[i] == '.')
				{
					r.a[r.aLen] = fno->fname[i];
					r.aLen++;

					r.p[r.pLen] = fno->fname[i];
					r.pLen++;
					break;
				}
				else break;
			}
			r.n[r.nLen] = 0;
			r.a[r.aLen] = 0;
			r.p[r.pLen] = 0;
			for(unsigned char i = r.aLen; i < 8 + 1 + 3 + 1; i++)
			{
				if(fno->fname[i] != 0)
				{
					signed char c = fno->fname[i];
					if(c >= 'A' && c <= 'Z')
					{
						c -= 'A';
						c += 'a';
					}
					r.e[r.eLen] = c;
					r.eLen++;
				
					r.a[r.aLen] = fno->fname[i];
					r.aLen++;

					r.p[r.pLen] = fno->fname[i];
					r.pLen++;
				}
				else break;
			}
		}
		else // using file path when fileinfo not given
		{
			unsigned char slash = 0;
			unsigned char slashpos = 0;
			for(unsigned char i = r.pLen; i != 0; i--)
			{
				if(r.p[i - 1] == '/')
				{
					slash = i;
					slashpos = i;
					break;
				}
			}

			for(unsigned char i = 0; i < 32; i++, slash++)
			{
				if(r.p[slash] != '.' && r.p[slash] != 0)
				{
					r.n[r.nLen] = r.p[slash];
					r.nLen++;
				}
				else break;
			}
			slash++;
			for(unsigned char i = 0; i < 3; i++, slash++)
			{
				if(r.p[slash] != 0)
				{
					r.e[r.eLen] = r.p[slash];
					r.eLen++;
				}
				else break;
			}

			// ensure file name is formated 8.3
			if(r.nLen > 8)
			{
				for(unsigned char i = 0; i < 6; i++, slashpos++)
				{
					r.p[slashpos] = r.n[i];
				}
				r.p[slashpos] = '~';
				slashpos++;
				r.p[slashpos] = '1';
				slashpos++;
				r.p[slashpos] = '.';
				slashpos++;
				for(unsigned char i = 0; i < 3; i++, slashpos++)
				{
					r.p[slashpos] = r.e[i];
				}
				r.pLen = slashpos;
			}

		}
		r.e[r.eLen] = 0;
		r.a[r.aLen] = 0;
		r.p[r.pLen] = 0;

		// must be mp3 file
		if(r.e[0] != 'm' || r.e[1] != 'p' || r.e[2] != '3') return 255;

		memmove(&(mf->fn), &r, sizeof(FileName83));
	}

	{
		// open file
		FIL temp_fh;
		unsigned char err = f_open(&temp_fh, mf->fn.p, FA_READ);
		if(err != 0) return err;
		memcpy(&(mf->fh), &temp_fh, sizeof(FIL));
	}

	{
		// get song title from ID3 tags

		unsigned char id3v2[5];

		f_lseek(&(mf->fh), 0);
		f_read(&(mf->fh), id3v2, 3, &id3v2[4]);

		if(id3v2[0] == 'I' && id3v2[1] == 'D' && id3v2[2] == '3')
		{
			f_lseek(&(mf->fh), 0);

			unsigned char pb[5];
			pb[4] = 0;
			unsigned char c[2];

			while(1)
			{
				// loop for TIT2 title tag

				f_read(&(mf->fh), c, 1, &c[1]);
				
				pb[3] = pb[2];
				pb[2] = pb[1];
				pb[1] = pb[0];
				pb[0] = c[0];

				if(pb[0] == '2' && pb[1] == 'T' && pb[2] == 'I' && pb[3] == 'T')
				{
					// TIT2 tag found, read length of title

					f_read(&(mf->fh), pb, 4, &c[1]);
					unsigned long tl = ((unsigned long)pb[0] << (8 * 3)) + ((unsigned long)pb[1] << (8 * 2)) + ((unsigned long)pb[2] << (8 * 1)) + pb[3];

					mf->titleLen = tl - 1;
					if(mf->titleLen > 30) mf->titleLen = 30; // too long

					// skip 3 null, then read in title
					f_read(&(mf->fh), pb, 3, &c[1]);
					f_read(&(mf->fh), mf->title, mf->titleLen, &c[1]);

					mf->title[mf->titleLen] = 0;

					break;
				}
				else if(c[1] == 0)
				{
					// end of file, use file name
					mf->titleLen = mf->fn.nLen;
					strcpy(mf->title, mf->fn.n);
		
					break;
				}
			}
		}
		else
		{
			unsigned char id3v1[5];

			f_lseek(&(mf->fh), mf->fh.fsize - 128);
			f_read(&(mf->fh), id3v1, 3, &id3v1[4]);		

			if(id3v1[0] == 'T' && id3v1[1] == 'A' && id3v1[2] == 'G')
			{
				// TAG tag found for id3v1
				// read in title
				f_read(&(mf->fh), mf->title, 30, &id3v1[3]);			
				mf->titleLen = 30;
				// strip spaces and non printables from end
				for(signed char i = 29; i != -1; i--)
				{
					if(mf->title[i] <= ' ' || mf->title[i] > 126)
					{
						mf->title[i] = 0;
						mf->titleLen--;
					}
					else break;
				}
			}
			else
			{
				// end of file, use file name
				mf->titleLen = mf->fn.nLen;
				strcpy(mf->title, mf->fn.n);
			}
		}
	}

	#ifdef calc_song_length
	{
		// finished reading title, now calculate song length

		f_lseek(&mf->fh, 0);

		unsigned char pb[4];
		unsigned char c[2];

		while(1)
		{
			// loop for mp3 header

			f_read(&mf->fh, c, 1, &c[1]);
	
			pb[0] = pb[1];
			pb[1] = pb[2];
			pb[2] = pb[3];
			pb[3] = c[0];

			if((pb[1] & 0b11111110) == 0b11111010 && pb[0] == 0xFF)
			{
				// sync found

				mf->headerLoc = mf->fh.fptr - 4;

				// get the bit rate according to which MPEG version
				unsigned long i = ((pb[2] & 0xF0) >> 4);

				if(bit_is_set(pb[1], 3))
				{
					// MPEG version 1

					switch(i)
					{
						case 1:
							i = 32;
							break;
						case 2:
							i = 40;
							break;
						case 3:
							i = 48;
							break;
						case 4:
							i = 56;
							break;
						case 5:
							i = 64;
							break;
						case 6:
							i = 80;
							break;
						case 7:
							i = 96;
							break;
						case 8:
							i = 112;
							break;
						case 9:
							i = 128;
							break;
						case 10:
							i = 160;
							break;
						case 11:
							i = 192;
							break;
						case 12:
							i = 224;
							break;
						case 13:
							i = 256;
							break;
						case 14:
							i = 320;
							break;
					}
				}
				else
				{
					// MPEG version 2

					switch(i)
					{
						case 1:
							i = 8;
							break;
						case 2:
							i = 16;
							break;
						case 3:
							i = 24;
							break;
						case 4:
							i = 32;
							break;
						case 5:
							i = 40;
							break;
						case 6:
							i = 48;
							break;
						case 7:
							i = 56;
							break;
						case 8:
							i = 64;
							break;
						case 9:
							i = 80;
							break;
						case 10:
							i = 96;
							break;
						case 11:
							i = 112;
							break;
						case 12:
							i = 128;
							break;
						case 13:
							i = 144;
							break;
						case 14:
							i = 160;
							break;
					}
				}

				// calculate duration from bit rate and file size
				i *= (1024 / 8);
				mf->bps = i;
				mf->duration = (mf->fh.fsize - mf->headerLoc) / mf->bps;
				mf->duration++;

				break;
			}
			else if(c[1] == 0) return 254; // end of file
		}
	}
	#endif

	f_lseek(&(mf->fh), 0); // rewind to start

	mf->open = 1;
	return 0;
}

// write two bytes to register
void MP3WriteReg(unsigned char addr, unsigned char hC, unsigned char lC)
{
	cbi(MP3_Port, MP3_xCS_Pin);
	
	SPITx(0b00000010);
	
	SPITx(addr);
	
	SPITx(hC);
	SPITx(lC);
	
	sbi(MP3_Port, MP3_xCS_Pin);
}

void MP3WriteRegS(unsigned char addr, unsigned short c)
{
	unsigned char lC = c & 0xFF;
	unsigned short hC = (c & 0xFF00) >> 8;
	MP3WriteReg(addr, hC, lC);
}

// read 2 bytes from register
unsigned int MP3ReadReg(unsigned char addr)
{
	unsigned char lC;
	unsigned char hC;
	unsigned int r;
	
	cbi(MP3_Port, MP3_xCS_Pin);
	
	SPITx(0b00000011);
	
	SPITx(addr);
	
	hC = SPIRx(0);
	lC = SPIRx(0);
	
	sbi(MP3_Port, MP3_xCS_Pin);
	
	r = ((unsigned int)hC << 8) + lC;
	return r;
}

// set volume
void MP3SetVol(unsigned char lC, unsigned char rC)
{
	lC = 255 - lC;
	rC = 255 - rC;
	MP3WriteReg(MP3_Reg_VOL, lC, rC);
}

void MP3ChangeVol(signed char v)
{
	// read in previous volume, and change it

	union {
		unsigned char c[2];
		unsigned short s;
	} cur;
	cur.s = MP3ReadReg(MP3_Reg_VOL);
	signed short vol = ((signed short)cur.c[0] + (signed short)cur.c[1] + 1) / 2;
	vol -= v;
	if(vol < 0)
	{
		vol = 0;
	}
	else if(vol > 255)
	{
		vol = 255;
	}
	MP3WriteReg(MP3_Reg_VOL, vol, vol);	
}

unsigned char MP3Play(MP3File * f)
{
	unsigned char d[MP3PacketSize + 1];

	if(bit_is_set(MP3_PinIn, MP3_DREQ_Pin)) // data request active
	{
		// read then forward to decoder
		unsigned char err = f_read(&(f->fh), d, MP3PacketSize, &d[MP3PacketSize]);
		MP3DataTx(d, d[MP3PacketSize]);

		if(d[MP3PacketSize] != MP3PacketSize) return 2; else return 0;
	}
	else return 1;
}

void MP3PlayToEnd(MP3File * f)
{
	while(MP3Play(f) != 2);
}

void MP3Init(unsigned char vol, unsigned char invert)
{
	// DREQ as input with pullup resistor
	cbi(MP3_Port, MP3_DREQ_Pin);
	cbi(MP3_DDR, MP3_DREQ_Pin);

	// chip select both output and high
	sbi(MP3_Port, MP3_xCS_Pin);
	sbi(MP3_Port, MP3_xDCS_Pin);
	sbi(MP3_DDR, MP3_xCS_Pin);
	sbi(MP3_DDR, MP3_xDCS_Pin);

	sbi(MP3_DDR, MP3_RST_Pin); // reset pin as output

	// reset the decoder
	sbi(MP3_Port, MP3_RST_Pin);
	_delay_us(10);
	cbi(MP3_Port, MP3_RST_Pin);
	_delay_us(10);
	sbi(MP3_Port, MP3_RST_Pin);

	// wait for reset
	for(unsigned char i = 0; i < 100; i++)
	{
		_delay_us(10);
		if(bit_is_clear(MP3_PinIn, MP3_DREQ_Pin)) break;
	}
	loop_until_bit_is_set(MP3_PinIn, MP3_DREQ_Pin);

	// initial setup
	if(invert == 1)
	{
		MP3WriteReg(MP3_Reg_MODE, 0x08, 0x01);
	}
	else
	{
		MP3WriteReg(MP3_Reg_MODE, 0x08, 0x00);
	}

	MP3WriteReg(MP3_Reg_CLOCKF, 0x98, 0x00);

	#ifdef MP3_Custom_Code
	for(unsigned int i = 0; i < CODE_SIZE; i++)
	{
		while(bit_is_clear(MP3_PinIn, MP3_DREQ_Pin));
		MP3WriteRegS(pgm_read_byte(&MP3Code_atab[i]), pgm_read_word(&MP3Code_dtab[i]));
	}

	MP3WriteRegS(MP3_Reg_AIADDR, 0x30);
	#endif

	MP3SetVol(vol, vol);

	// wait for reset
	for(unsigned char i = 0; i < 100; i++)
	{
		_delay_us(10);
		if(bit_is_clear(MP3_PinIn, MP3_DREQ_Pin)) break;
	}
	loop_until_bit_is_set(MP3_PinIn, MP3_DREQ_Pin);
}

void MP3Reset()
{
	unsigned short old_mode = MP3ReadReg(MP3_Reg_MODE);
	unsigned short old_vol = MP3ReadReg(MP3_Reg_VOL);
	unsigned short old_clk = MP3ReadReg(MP3_Reg_VOL);

	MP3SetVol(0, 0);

	// reset the decoder
	sbi(MP3_Port, MP3_RST_Pin);
	_delay_us(10);
	cbi(MP3_Port, MP3_RST_Pin);
	_delay_us(10);
	sbi(MP3_Port, MP3_RST_Pin);
	
	// wait for reset
	for(unsigned char i = 0; i < 100; i++)
	{
		_delay_us(10);
		if(bit_is_clear(MP3_PinIn, MP3_DREQ_Pin)) break;
	}
	loop_until_bit_is_set(MP3_PinIn, MP3_DREQ_Pin);

	MP3WriteRegS(MP3_Reg_MODE, old_mode);
	MP3WriteRegS(MP3_Reg_CLOCKF, old_clk);
	MP3WriteRegS(MP3_Reg_VOL, old_vol);

	#ifdef MP3_Custom_Code
	for(unsigned int i = 0; i < CODE_SIZE; i++)
	{
		while(bit_is_clear(MP3_PinIn, MP3_DREQ_Pin));
		MP3WriteRegS(pgm_read_byte(&MP3Code_atab[i]), pgm_read_word(&MP3Code_dtab[i]));
	}

	MP3WriteRegS(MP3_Reg_AIADDR, 0x30);
	#endif

	// wait for reset
	for(unsigned char i = 0; i < 100; i++)
	{
		_delay_us(10);
		if(bit_is_clear(MP3_PinIn, MP3_DREQ_Pin)) break;
	}
	loop_until_bit_is_set(MP3_PinIn, MP3_DREQ_Pin);
}
