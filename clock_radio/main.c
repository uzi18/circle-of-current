#include "main.h"
#include "misc.h"
#include "print.h"

#define playflag 0
#define continueflag 1

#define PLAYCMD 0
#define NEXTCMD 1
#define PREVCMD 2
#define UPCMD 3
#define DOWNCMD 4
#define MENUCMD 5

#define normalmode 0
#define loopmode 1
#define shufflemode 2

const char day_monday[] = "Mon";
const char day_tuesday[] = "Tues";
const char day_wednesday[] = "Wed";
const char day_thursday[] = "Thurs";
const char day_friday[] = "Fri";
const char day_saturday[] = "Sat";
const char day_sunday[] = "Sun";

const char * day_array[7] = {
    day_monday,
    day_tuesday,
	day_wednesday,
	day_thursday,
	day_friday,
	day_saturday,
	day_sunday
};


typedef struct {
	unsigned char flags;
	unsigned char mode;
	unsigned char alarm_on[7];
	unsigned char alarm_h[7];
	unsigned char alarm_m[7];
	unsigned char cur_day;
	unsigned char cur_h;
	unsigned char cur_m;
	unsigned char invert;
} OP_STRUCT;

unsigned char open_next(DIR * dh, MP3File * mf, char * p)
{
	unsigned char looped = 0;
	unsigned char err;
	if(dh->index == 0)
	{
		err = f_opendir(dh, p);
		if(err != 0) return err;
		looped = 1;
	}

	LCDSetPos(1, 1);
	fprintf_P(&LCDstdout, PSTR("Next Song:\n"));
	LCDClear(2);

	while(1)
	{
		FILINFO fno;
		f_readdir(dh, &fno);
		if(fno.fname[0] != 0)
		{
			if(MP3Open(&fno, mf, p) == 0)
			{
				fprintf_P(&LCDstdout, PSTR("%s\n"), mf->title);
				song_timer = 0;
				
				return 0;
			}
		}
		else
		{
			err = f_opendir(dh, p);
			if(err != 0) return err;
			if(looped != 0)
			{
				return 255;
			}
			looped = 1;
		}
	}
}

unsigned char shuffle(DIR * dh, MP3File * mf, char * p, unsigned char j)
{
	LCDSetPos(1, 1);
	fprintf_P(&LCDstdout, PSTR("Shuffling\n"));
	LCDClear(2);

	srand(TCNT2);
	unsigned long t = rand() % j;
	for(unsigned char i = 0; i < t; i++)
	{
		FILINFO fno;
		f_readdir(dh, &fno);
		if(fno.fname[0] == 0)
		{
			f_opendir(dh, p);
		}
	}
	return open_next(dh, mf, p);
}

unsigned char open_prev(DIR * dh, MP3File * mf, char * p)
{
	DIR tdir;
	unsigned char err = f_opendir(&tdir, p);
	if(err != 0) return err;

	LCDSetPos(1, 1);
	fprintf_P(&LCDstdout, PSTR("Previous Song:\n"));
	LCDClear(2);

	DIR ldir;
	MP3File tmf;
	MP3File lmf;

	unsigned char firstflag = 0;

	while(1)
	{
		FILINFO fno;
		f_readdir(&tdir, &fno);
		if(fno.fname[0] == 0)
		{
			break;
		}
		else
		{
			if(MP3Open(&fno, &tmf, p) == 0)
			{
				if(strcmp(mf->fn.n, tmf.fn.n) == 0)
				{
					if(firstflag != 0)
					{
						memcpy(dh, &ldir, sizeof(DIR));
						memcpy(mf, &lmf, sizeof(MP3File));
						fprintf_P(&LCDstdout, PSTR("%s\n"), mf->title);
						song_timer = 0;

						return 0;
					}
					else
					{
						firstflag = 2;
					}
				}
				else
				{
					memcpy(&ldir, &tdir, sizeof(DIR));
					memcpy(&lmf, &tmf, sizeof(MP3File));
					if(firstflag == 0)
					{
						firstflag = 1;
					}
				}
			}
		}
	}

	if(firstflag != 0)
	{
		memcpy(dh, &ldir, sizeof(DIR));
		memcpy(mf, &lmf, sizeof(MP3File));
		fprintf_P(&LCDstdout, PSTR("%s\n"), mf->title);
		song_timer = 0;

		return 0;
	}
	else
	{
		return 255;
	}
}

unsigned char rewind(MP3File * mf)
{
	unsigned char * p = calloc(MP3PacketSize, sizeof(unsigned char));
	for(unsigned char i = 0; i < 256 / MP3PacketSize; i++)
	{
		while(bit_is_clear(MP3_PinIn, MP3_DREQ_Pin));
		MP3DataTx(p, MP3PacketSize);
	}
	free(p);
	return f_lseek(&(mf->fh), 0);
}

unsigned char menu(OP_STRUCT * o, MP3File * mf)
{
	unsigned char a = 0;
	unsigned char c = 255;

	unsigned char mode = 0;
	unsigned char submode = 0;

	LCDSetPos(1, 1);
	LCDSend(127, 1);
	LCDSetPos(1, 2);
	LCDSend(upcharaddr, 1);

	//               23456789012345
	LCDPrint_P(PSTR(" Mode         "), 2, 1);
	if(o->mode == 0)
	{
		//               23456789012345
		LCDPrint_P(PSTR(" Normal       "), 2, 2);
	}
	else if(o->mode == 1)
	{
		//               23456789012345
		LCDPrint_P(PSTR(" Loop         "), 2, 2);
	}
	else if(o->mode == 2)
	{
		//               23456789012345
		LCDPrint_P(PSTR(" Shuffle      "), 2, 2);
	}

	LCDSetPos(16, 2);
	LCDSend(downcharaddr, 1);
	LCDSetPos(16, 1);
	LCDSend(126, 1);

	while(1)
	{
		c = serRx(&a);
		BL_on();
		
		if(a > 0)
		{
			fprintf_P(&serstdout, PSTR("a%d c%d m%d s%d\n"), a, c, mode, submode);

			if(c == PREVCMD)
			{
				if(mode == 0)
				{
					mode = 9;
				}
				else
				{
					mode--;
				}
			}
			else if(c == NEXTCMD)
			{
				if(mode == 9)
				{
					mode = 0;
				}
				else
				{
					mode++;
				}
			}
			else if(c == MENUCMD)
			{
				break;
			}

			LCDSetPos(1, 1);
			LCDSend(127, 1);
			LCDSetPos(1, 2);
			LCDSend(upcharaddr, 1);

			if(mode == 0)
			{
				//               23456789012345
				LCDPrint_P(PSTR(" Mode         "), 2, 1);

				if(c == UPCMD || c == PLAYCMD)
				{
					if(o->mode == 2)
					{
						o->mode = 0;
					}
					else
					{
						o->mode++;
					}
				}
				else if(c == DOWNCMD)
				{
					if(o->mode == 0)
					{
						o->mode = 2;
					}
					else
					{
						o->mode--;
					}
				}

				if(o->mode == normalmode)
				{
					//               23456789012345
					LCDPrint_P(PSTR(" Normal       "), 2, 2);
				}
				else if(o->mode == loopmode)
				{
					//               23456789012345
					LCDPrint_P(PSTR(" Loop         "), 2, 2);
				}
				else if(o->mode == shufflemode)
				{
					//               23456789012345
					LCDPrint_P(PSTR(" Shuffle      "), 2, 2);
				}
			}
			else if(mode >= 1 && mode < 1 + 7)
			{
				unsigned char daycnt = mode - 1;
				char * day_name = day_array[daycnt];

				LCDSetPos(2, 1);
				//                          23456789012345
				fprintf_P(&LCDstdout, PSTR(" %s Alarm      "), day_name);

				if(c == PLAYCMD)
				{
					submode++;
					submode %= 3;
				}

				if(submode == 0)
				{
					if(c == UPCMD || c == DOWNCMD)
					{
						if(o->alarm_on[daycnt] == 0)
						{
							o->alarm_on[daycnt] = 1;
						}
						else
						{
							o->alarm_on[daycnt] = 0;
						}
					}
			
					if(o->alarm_on[daycnt] == 0)
					{
						//               23456789012345
						LCDPrint_P(PSTR(" > Off        "), 2, 2);
					}
					else
					{
						//               23456789012345
						LCDPrint_P(PSTR(" > On         "), 2, 2);
					}
				}
				else if(submode == 1)
				{
					if(c == UPCMD)
					{
						if(o->alarm_h[daycnt] == 23)
						{
							o->alarm_h[daycnt] = 0;
						}
						else
						{
							o->alarm_h[daycnt]++;
						}
					}
					else if(c == DOWNCMD)
					{
						if(o->alarm_h[daycnt] == 0)
						{
							o->alarm_h[daycnt] = 23;
						}
						else
						{
							o->alarm_h[daycnt]--;
						}
					}
			
					LCDSetPos(2, 2);
					//                          23456789012345
					fprintf_P(&LCDstdout, PSTR(" > Hour: %d    "), o->alarm_h[daycnt]);
				}
				else if(submode == 2)
				{
					if(c == UPCMD)
					{
						if(o->alarm_m[daycnt] == 59)
						{
							o->alarm_m[daycnt] = 0;
						}
						else
						{
							o->alarm_m[daycnt]++;
						}
					}
					else if(c == DOWNCMD)
					{
						if(o->alarm_m[daycnt] == 0)
						{
							o->alarm_m[daycnt] = 59;
						}
						else
						{
							o->alarm_m[daycnt]--;
						}
					}
			
					LCDSetPos(2, 2);
					//                          23456789012345
					fprintf_P(&LCDstdout, PSTR(" > Minute: %d  "), o->alarm_m[daycnt]);
				}
			}
			else if(mode == 8)
			{
				//               23456789012345
				LCDPrint_P(PSTR(" Current Time "), 2, 1);

				if(c == PLAYCMD)
				{
					submode++;
					submode %= 3;
				}

				if(submode == 0)
				{
					if(c == UPCMD)
					{
						if(o->cur_day == 6)
						{
							o->cur_day = 0;
						}
						else
						{
							o->cur_day++;
						}
					}
					else if(c == DOWNCMD)
					{
						if(o->cur_day == 0)
						{
							o->cur_day = 6;
						}
						else
						{
							o->cur_day--;
						}
					}
			
					char * day_name = day_array[o->cur_day];

					LCDSetPos(2, 2);
					//                          23456789012345
					fprintf_P(&LCDstdout, PSTR(" > Day: %s     "), day_name);
				}
				else if(submode == 1)
				{
					if(c == UPCMD)
					{
						if(o->cur_h == 23)
						{
							o->cur_h = 0;
						}
						else
						{
							o->cur_h++;
						}
					}
					else if(c == DOWNCMD)
					{
						if(o->cur_h == 0)
						{
							o->cur_h = 23;
						}
						else
						{
							o->cur_h--;
						}
					}
			
					LCDSetPos(2, 2);
					//                          23456789012345
					fprintf_P(&LCDstdout, PSTR(" > Hour: %d    "), o->cur_h);
				}
				else if(submode == 2)
				{
					if(c == UPCMD)
					{
						if(o->cur_m == 59)
						{
							o->cur_m = 0;
						}
						else
						{
							o->cur_m++;
						}
					}
					else if(c == DOWNCMD)
					{
						if(o->cur_m == 0)
						{
							o->cur_m = 59;
						}
						else
						{
							o->cur_m--;
						}
					}
			
					LCDSetPos(2, 2);
					//                          23456789012345
					fprintf_P(&LCDstdout, PSTR(" > Minute: %d  "), o->cur_m);
				}
			}
			else if(mode == 9)
			{
				//               23456789012345
				LCDPrint_P(PSTR("Invert  Output "), 2, 1);

				if(c == UPCMD || c == PLAYCMD || c == DOWNCMD)
				{
					if(o->invert == 0)
					{
						o->invert = 1;
					}
					else
					{
						o->invert = 0;
					}
				}

				if(o->invert == 0)
				{
					//               23456789012345
					LCDPrint_P(PSTR(" > Off         "), 2, 2);
				}
				else
				{
					//               23456789012345
					LCDPrint_P(PSTR(" > On         "), 2, 2);
				}
			}

			LCDSetPos(16, 2);
			LCDSend(downcharaddr, 1);
			LCDSetPos(16, 1);
			LCDSend(126, 1);
		}

		if(o->invert != 0)
		{
			MP3WriteReg(MP3_Reg_MODE, 0x08, 0x01);
		}
		else
		{
			MP3WriteReg(MP3_Reg_MODE, 0x08, 0x00);
		}

		/*
		if(bit_is_set(o->flags, playflag))
		{
			if(mf->open != 0)
			{
				if(MP3Play(mf) == 2)
				{
					if(bit_is_set(o->flags, continueflag))
					{
						rewind(mf);
					}
					else
					{
						cbi(o->flags, playflag);
					}
				}
			}
		}
		*/
	}

	LCDPrintTime(o->cur_h, o->cur_m);

	union {
		unsigned char d[sizeof(OP_STRUCT)];
		OP_STRUCT o;
	} savedata;

	memcpy(&savedata.o, o, sizeof(OP_STRUCT));

	for(unsigned int i = 0; i < sizeof(OP_STRUCT); i++)
	{
		if(eeprom_read_byte(i) != savedata.d[i])
		{
			eeprom_write_byte(i, savedata.d[i]);
		}
	}
}

int main()
{
	sei();

	SPI_init();
	serInit();
	timer_init();
	LCDInit();

	signed char err = 1;

	static FATFS fs_handle;
	static DIR dir_handle;
	static MP3File mp3_handle;
	static OP_STRUCT ops;

	union {
		unsigned char d[sizeof(OP_STRUCT)];
		OP_STRUCT o;
	} savedata;

	for(unsigned int i = 0; i < sizeof(OP_STRUCT); i++)
	{
		savedata.d[i] = eeprom_read_byte(i);
	}

	memcpy(&ops, &savedata.o, sizeof(OP_STRUCT));

	MP3Init(255, ops.invert);

	ops.flags = 0;

	sbi(ops.flags, continueflag);

	while(1)
	{
		unsigned char a = 0;
		unsigned char c = 255;
		
		c = serRx(&a);

		if(a != 0)
		{
			BL_on();
			fprintf_P(&serstdout, PSTR("a%d c%d\n"), a, c);
			if(c == PLAYCMD)
			{
				tog(ops.flags, playflag);

				LCDSetPos(1, 1);
				fprintf_P(&LCDstdout, PSTR("Current Song:\n"));
				LCDSetPos(1, 2);
				fprintf_P(&LCDstdout, PSTR("%s\n"), mp3_handle.title);
				song_timer = 0;
			}
			else if(c == NEXTCMD)
			{
				if(ops.mode != shufflemode)
				{
					err = open_next(&dir_handle, &mp3_handle, "/mp3");
				}
				else
				{
					err = shuffle(&dir_handle, &mp3_handle, "/mp3", 16);
				}
			}
			else if(c == PREVCMD)
			{
				unsigned long percent = mp3_handle.fh.fptr * 100;
				if(bit_is_set(ops.flags, playflag) && (percent / mp3_handle.fh.fsize) > 5)
				{
					rewind(&mp3_handle);
				}
				else
				{
					if(ops.mode != shufflemode)
					{
						err = open_prev(&dir_handle, &mp3_handle, "/mp3");
					}
					else
					{
						err = shuffle(&dir_handle, &mp3_handle, "/mp3", 16);
					}
				}
			}
			else if(c == UPCMD)
			{
				MP3ChangeVol(8);
			}
			else if(c == DOWNCMD)
			{
				MP3ChangeVol(-8);
			}
			else if(c == MENUCMD)
			{
				menu(&ops, &mp3_handle);
			}
		}

		if(err == 0)
		{
			if(bit_is_set(ops.flags, playflag))
			{
				if(mp3_handle.open != 0)
				{
					if(MP3Play(&mp3_handle) == 2)
					{
						if(bit_is_set(ops.flags, continueflag))
						{
							if(ops.mode == shufflemode)
							{
								err = shuffle(&dir_handle, &mp3_handle, "/mp3", 16);
							}
							else if(ops.mode == normalmode)
							{
								err = open_next(&dir_handle, &mp3_handle, "/mp3");
							}
							else if(ops.mode == loopmode)
							{
								rewind(&mp3_handle);
							}
						}
						else
						{
							cbi(ops.flags, playflag);
						}
					}
				}
				else
				{
					err = open_next(&dir_handle, &mp3_handle, "/mp3");
				}
			}
		}
		else
		{
			disk_initialize(0);
			f_mount(0, &fs_handle);
			
			err = open_next(&dir_handle, &mp3_handle, "/mp3");

			if(err != 0)
			{
				LCDSetPos(1, 1);
				fprintf_P(&LCDstdout, PSTR("Disk Error\n"));
				LCDSetPos(1, 2);
				fprintf_P(&LCDstdout, PSTR("Error = %d\n"), err);
				song_timer = 0;
			}
		}

		if(song_timer >= 1220 && song_timer < 1220 + 100)
		{
			song_timer = 1220 + 100;
			LCDPrintTime(ops.cur_h, ops.cur_m);
		}
		else if(song_timer >= 1220 + 100)
		{
			song_timer = 1220 + 100;
		}

		if(clk_timer >= 14892 / 2 && song_timer >= 1220 + 100)
		{
			clk_timer = 0;			
			LCDPrintTime(ops.cur_h, ops.cur_m);
		}
	}

	while(1);

	return 0;
}
