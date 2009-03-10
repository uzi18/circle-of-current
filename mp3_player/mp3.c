#include "mp3.h"

volatile MP3File current_song;

volatile unsigned char lVol;
volatile unsigned char rVol;
volatile unsigned char MP3PauseFlag = 0;
volatile unsigned char MP3WaveStopFlag = 0;

// transfer data via SPI to SDI
void MP3DataTx(unsigned char d[], unsigned char len)
{
	while(bit_is_clear(MP3_PinIn, MP3_DREQ_Pin));

	cbi(MP3_Port, MP3_xCDS_Pin);

	unsigned char i;
	for(i = 0; i < len; i++)
	{
		SPITx(d[i]);
	}

	sbi(MP3_Port, MP3_xCDS_Pin);
}

void MP3KeepPlaying()
{
	if(current_song.opened == 0) return; // error

	unsigned char d[MP3PacketSize + 1];

	while(bit_is_set(MP3_PinIn, MP3_DREQ_Pin))
	{
		// read then forward to decoder
		f_read(&(current_song.fil), d, MP3PacketSize, &d[MP3PacketSize]);
		MP3DataTx(d, d[MP3PacketSize]);

		if(d[MP3PacketSize] != MP3PacketSize) return;
	}
}

void MP3Open(FileName83 fn, MP3File * mf, DIR dir_)
{
	mf->fn = fn;
	mf->titleLen = 0;
	mf->duration = 0;
	mf->headerLoc = 0;
	mf->opened = 0;
	mf->success = 0; // successful by default, any error sets flag
	mf->dir = dir_;

	{ // lower case file name
		unsigned char i;
		for(i = 1; i < 5; i++)
		{
			if(fn.e[i] >= 'A' && fn.e[i] <= 'Z')
			{
				fn.e[i] -= 'A';
				fn.e[i] += 'a';
			}
		}
	}

	mf->success = f_open(&mf->fil, mf->fn.p, FA_READ);

	// check file name, success if mp3
	if(fn.e[0] == '.' && fn.e[1] == 'm' && fn.e[2] == 'p' && fn.e[3] == '3' && fn.e[4] == 0)
	{
		static unsigned char id3v1[5]; // these needs to be static for some odd reason
		static unsigned char id3v2[5];

		mf->success |= f_read(&mf->fil, id3v2, 3, &id3v2[4]);
		id3v2[3] = 0;

		if(mf->fil.fsize >= 128)
		{
			mf->success |= f_lseek(&mf->fil, mf->fil.fsize - 128);
			mf->success |= f_read(&mf->fil, id3v1, 3, &id3v1[4]);
		}

		// check if using id3v2 header
		if(id3v2[4] == 3 && id3v2[0] == 'I' && id3v2[1] == 'D' && id3v2[2] == '3')
		{
			mf->success |= f_lseek(&mf->fil, 0);

			unsigned char pb[5];
			pb[4] = 0;
			unsigned char c[2];

			while(1)
			{
				// loop for TIT2 title tag

				mf->success |= f_read(&mf->fil, c, 1, &c[1]);
				if(c[1] == 0) break;
				
				pb[3] = pb[2];
				pb[2] = pb[1];
				pb[1] = pb[0];
				pb[0] = c[0];

				if(pb[0] == '2' && pb[1] == 'T' && pb[2] == 'I' && pb[3] == 'T')
				{
					// TIT2 tag found, read length of title

					mf->success |= f_read(&mf->fil, pb, 4, &c[1]);
					unsigned long tl = ((unsigned long)pb[0] << (8 * 3)) + ((unsigned long)pb[1] << (8 * 2)) + ((unsigned long)pb[2] << (8 * 1)) + pb[3];

					mf->titleLen = tl - 1;
					if(mf->titleLen > 30) mf->titleLen = 30; // too long

					// skip 3 null, then read in title
					mf->success |= f_read(&mf->fil, pb, 3, &c[1]);
					mf->success |= f_read(&mf->fil, mf->title, mf->titleLen, &c[1]);
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
		else if(id3v1[4] == 3 && id3v1[0] == 'T' && id3v1[1] == 'A' && id3v1[2] == 'G')
		{
			// TAG tag found for id3v1
			// read in title
			mf->success |= f_read(&mf->fil, mf->title, 30, &id3v1[3]);			
			mf->titleLen = 30;
			// strip spaces and non printables from end
			signed char i;
			for(i = 29; i != -1; i--)
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
			// no id3 tag at all, use file name
			mf->titleLen = mf->fn.nLen;
			strcpy(mf->title, mf->fn.n);
		}

		// finished reading title, now calculate song length

		mf->success |= f_lseek(&mf->fil, 0);

		unsigned char pb[4];
		unsigned char c[2];

		while(1)
		{
			// loop for mp3 header

			mf->success |= f_read(&mf->fil, c, 1, &c[1]);
			
			pb[0] = pb[1];
			pb[1] = pb[2];
			pb[2] = pb[3];
			pb[3] = c[0];

			if((pb[1] & 0b11100000) == 0b11100000 && pb[0] == 0xFF && c[1] != 0)
			{
				// sync found

				mf->headerLoc = mf->fil.fptr - 4;

				// get the bit rate according to which MPEG version
				unsigned long i = ((pb[2] & 0xF0) >> 4);

				if(bit_is_set(pb[1], 3))
				{
					// MPEG version 1

					switch(i)
					{;
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
				mf->duration = (mf->fil.fsize - mf->headerLoc) / i;
				mf->duration++;

				break;
			}
			else if(c[1] == 0){mf->success = 255; break;} // end of file
		}
	}
	else
	{
		// Not a MP3
		mf->success = 255;
	}

	mf->success |= f_lseek(&mf->fil, 0);

	if(mf->success == 0)
	{
		mf->opened = 1;
	}
	else
	{
		mf->success |= f_close(&mf->fil);
	}

	current_song = mf;
}

// close a file using this
unsigned char MP3Close(MP3File * f)
{
	if(f->opened == 0) return 0;
	f->opened = 0;
	FIL t = f->fil;
	unsigned char i = f_close(&t);
	f->fil = t;
	return i;
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

// read in volume
void MP3ReadVol()
{
	unsigned int r = MP3ReadReg(MP3_Reg_VOL);
	lVol = (r & 0xFF00) >> 8;
	rVol = r & 0xFF;
	lVol = 255 - lVol;
	rVol = 255 - rVol;
}

// set volume
void MP3SetVol(unsigned char lC, unsigned char rC)
{
	lVol = lC;
	rVol = rC;
	lC = 255 - lC;
	rC = 255 - rC;
	MP3WriteReg(MP3_Reg_VOL, lC, rC);
}

unsigned char MP3PlayToEnd(MP3File * f)
{
	if(f->opened == 0) return 255; // error

	unsigned char d[MP3PacketSize + 1];

	while(1)
	{
		if(MP3PauseFlag == 1) return 1; // stop playing if flag is set

		// read then forward to decoder
		f_read(&(f->fil), d, MP3PacketSize, &d[MP3PacketSize]);
		MP3DataTx(d, d[MP3PacketSize]);

		if(d[MP3PacketSize] != MP3PacketSize) return 0; // end of file
	}
}

void MP3Init(unsigned char vol)
{
	lVol = vol;
	rVol = vol;

	// DREQ as input with pullup resistor
	sbi(MP3_Port, MP3_DREQ_Pin);
	cbi(MP3_DDR, MP3_DREQ_Pin);

	// chip select both output and high
	sbi(MP3_Port, MP3_xCS_Pin);
	sbi(MP3_Port, MP3_xCDS_Pin);
	sbi(MP3_DDR, MP3_xCS_Pin);
	sbi(MP3_DDR, MP3_xCDS_Pin);

	sbi(DDRD, 4); // reset pin as output

	// start SPI
	SPIInitMasterMode0();

	// reset the decoder
	cbi(PORTD, 4);
	sbi(PORTD, 4);

	// wait for reset
	while(bit_is_clear(MP3_PinIn, MP3_DREQ_Pin));

	_delay_ms(1);

	// initial setup
	MP3WriteReg(MP3_Reg_MODE, 0x08, 0x00);
	MP3WriteReg(MP3_Reg_CLOCKF, 0x98, 0x00);
	MP3SetVol(lVol, rVol);
}
