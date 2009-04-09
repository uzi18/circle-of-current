#include "main.h"

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

FILE LCDstdout = FDEV_SETUP_STREAM(LCD_putc, NULL, _FDEV_SETUP_WRITE);
FILE serstdout = FDEV_SETUP_STREAM(ser_putc, NULL, _FDEV_SETUP_WRITE);

#include "misc.h"

void get_time(OP_STRUCT * o)
{
	cli();
	o->cur_day = day_cnt;
	o->cur_h = hour_cnt;
	o->cur_m = min_cnt;
	sei();
}

void set_time(OP_STRUCT * o)
{
	cli();
	day_cnt = o->cur_day;
	min_cnt = o->cur_m;
	hour_cnt = o->cur_h;
	sei();
}

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
				song_title_timer = 0;
				
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
						song_title_timer = 0;

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
		song_title_timer = 0;

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
	unsigned char a;
	unsigned char c = BADCMD;
	unsigned char firstloopflag = 1;

	unsigned char mode = 0;
	unsigned char submode = 0;

	menu_timer = 0;

	while(menu_timer < menu_timeout)
	{
		if(firstloopflag != 0)
		{
			firstloopflag = 0;
			a = 1;
		}
		else
		{
			c = serRx(&a);
		}

		BL_on();
		
		if(a > 0)
		{
			menu_timer = 0;

			if(c == PREVCMD)
			{
				mode--;
			}
			else if(c == NEXTCMD)
			{
				if(mode == 0)
				{
					mode = o->cur_day + 2;
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

			showmenu:

			if(mode == play_mode_menu)
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
			else if(mode == cur_time_menu)
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

				set_time(o);
			}
			else if(mode == alarm_mode_menu)
			{
				//               23456789012345
				LCDPrint_P(PSTR("  Alarm Mode   "), 2, 1);

				if(c == UPCMD || c == PLAYCMD)
				{
					if(o->alarm_mode == 3)
					{
						o->alarm_mode = 0;
					}
					else
					{
						o->alarm_mode++;
					}
				}
				else if(c == DOWNCMD)
				{
					if(o->alarm_mode == 0)
					{
						o->alarm_mode = 3;
					}
					else
					{
						o->alarm_mode--;
					}
				}

				if(o->alarm_mode == 0)
				{
					//               23456789012345
					LCDPrint_P(PSTR(" All Off       "), 2, 2);
				}
				else if(o->alarm_mode == alarm_mode_random)
				{
					//               23456789012345
					LCDPrint_P(PSTR(" Random        "), 2, 2);
				}
				else if(o->alarm_mode == alarm_mode_daily)
				{
					//               23456789012345
					LCDPrint_P(PSTR(" Daily         "), 2, 2);
				}
				else if(o->alarm_mode == alarm_mode_default)
				{
					//               23456789012345
					LCDPrint_P(PSTR(" Predefined    "), 2, 2);
				}
			}
			else if(mode == display_mode_menu)
			{
				//               23456789012345
				LCDPrint_P(PSTR(" Time Display  "), 2, 1);

				if(c == UPCMD || c == PLAYCMD || c == DOWNCMD)
				{
					if(o->ampm == 0)
					{
						o->ampm = 1;
					}
					else
					{
						o->ampm = 0;
					}
				}

				if(o->ampm == 0)
				{
					//               23456789012345
					LCDPrint_P(PSTR(" > 24 Hour     "), 2, 2);
				}
				else
				{
					//               23456789012345
					LCDPrint_P(PSTR(" > AM / PM     "), 2, 2);
				}
			}
			else if(mode == alarm_fade_menu)
			{
				//               23456789012345
				LCDPrint_P(PSTR("  Alarm Fade  "), 2, 1);

				if(c == UPCMD || c == PLAYCMD || c == DOWNCMD)
				{
					if(o->alarm_fade == 0)
					{
						o->alarm_fade = 1;
					}
					else
					{
						o->alarm_fade = 0;
					}
				}

				if(o->alarm_fade == 0)
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
			else if(mode == invert_output_menu)
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
			else
			{
				if(c == PREVCMD)
				{
					mode--;
				}
				else if(c == NEXTCMD)
				{
					mode++;
				}

				goto showmenu;
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
	}

	LCDPrintTime(o->cur_h, o->cur_m, o->ampm);

	union {
		unsigned char d[sizeof(OP_STRUCT)];
		OP_STRUCT o;
	} savedata;

	memcpy(&savedata.o, o, sizeof(OP_STRUCT));

	eeprom_write_byte(0, eeprom_magic);

	for(unsigned int i = 0; i < sizeof(OP_STRUCT); i++)
	{
		if(eeprom_read_byte(i + 1) != savedata.d[i])
		{
			eeprom_write_byte(i + 1, savedata.d[i]);
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
	btn_port_init();

	signed char err = 1;

	static FATFS fs_handle;
	static DIR dir_handle;
	static MP3File mp3_handle;
	static OP_STRUCT ops;

	union {
		unsigned char d[sizeof(OP_STRUCT)];
		OP_STRUCT o;
	} savedata;

	if(eeprom_read_byte(0) == eeprom_magic)
	{
		for(unsigned int i = 0; i < sizeof(OP_STRUCT); i++)
		{
			savedata.d[i] = eeprom_read_byte(i + 1);
		}
	}
	else
	{
		for(unsigned int i = 0; i < sizeof(OP_STRUCT); i++)
		{
			savedata.d[i] = 0;
			eeprom_write_byte(i + 1, 0);
		}
	}

	memcpy(&ops, &savedata.o, sizeof(OP_STRUCT));

	MP3Init(255, ops.invert);

	ops.flags = 0;

	sbi(ops.flags, continueflag);
	sbi(ops.flags, showerrflag);

	unsigned short btncnt = 0;

	while(1)
	{
		static unsigned char a = 0;
		unsigned char c = 255;
		
		c = serRx(&a);

		if(bit_is_set(btn_flags[0], click_flag))
		{
			btncnt++;
			fprintf_P(&serstdout, PSTR("btn %d\n"), btncnt);
			cbi(btn_flags[0], click_flag);
		}

		if(a != 0)
		{
			BL_on();

			if(c == PLAYCMD)
			{
				tog(ops.flags, playflag);

				LCDSetPos(1, 1);
				fprintf_P(&LCDstdout, PSTR("Current Song:\n"));
				LCDSetPos(1, 2);
				fprintf_P(&LCDstdout, PSTR("%s\n"), mp3_handle.title);
				song_title_timer = 0;
			}
			else if(c == NEXTCMD)
			{
				if(ops.mode != shufflemode)
				{
					err = open_next(&dir_handle, &mp3_handle, "/mp3");
				}
				else
				{
					err = shuffle(&dir_handle, &mp3_handle, "/mp3", shuffle_factor);
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
						err = shuffle(&dir_handle, &mp3_handle, "/mp3", shuffle_factor);
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

			sbi(ops.flags, showerrflag);
		}

		if(err == 0)
		{
			sbi(ops.flags, showerrflag);

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
								err = shuffle(&dir_handle, &mp3_handle, "/mp3", shuffle_factor);
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

			if(err != 0 && bit_is_set(ops.flags, showerrflag))
			{
				cbi(ops.flags, showerrflag);
				LCDSetPos(1, 1);
				fprintf_P(&LCDstdout, PSTR("Disk Error\n"));
				LCDSetPos(1, 2);
				fprintf_P(&LCDstdout, PSTR("Error = %d\n"), err);
				song_title_timer = 0;
			}
		}

		if(song_title_timer >= display_title_time && song_title_timer < display_title_time + 100)
		{
			song_title_timer = display_title_time + 100;
			LCDPrintTime(ops.cur_h, ops.cur_m, ops.ampm);
		}
		else if(song_title_timer >= display_title_time + 100)
		{
			song_title_timer = display_title_time + 100;
		}

		if(clk_timer >= refresh_time && song_title_timer >= display_title_time + 100)
		{
			get_time(&ops);
			if(LCD_rst_timer >= song_title_timer * 2)
			{
				LCD_rst_timer = 0;
				LCDSoftReset();
			}
			LCDPrintTime(ops.cur_h, ops.cur_m, ops.ampm);

			clk_timer = 0;

			unsigned int cur_t = ops.cur_h * 60 + ops.cur_m;
			unsigned int alarm_t = ops.alarm_h[ops.cur_day] * 60 + ops.alarm_m[ops.cur_day];

			if((ops.alarm_mode != 0 && ops.alarm_on[ops.cur_day] != 0) && cur_t >= alarm_t && bit_is_clear(ops.flags, alarmflag))
			{
				sbi(ops.flags, alarmflag);

				unsigned char alarm_err;

				if(ops.alarm_fade == 0)
				{
					MP3SetVol(255, 255);
				}
				else if(bit_is_clear(ops.flags, playflag))
				{
					MP3SetVol(0, 0);
				}

				static MP3File alarm_handle;

				if(ops.alarm_mode == alarm_mode_random)
				{
					alarm_err = shuffle(&dir_handle, &alarm_handle, "/mp3", shuffle_factor);
				}
				else if(ops.alarm_mode == alarm_mode_default)
				{
					alarm_err = MP3Open(0, &alarm_handle, "/alarm/default.mp3");
				}
				else if(ops.alarm_mode == alarm_mode_daily)
				{
					char * alarm_path = calloc(20, sizeof(char));
					alarm_path = "/alarm/";
					strcat(alarm_path, day_array[ops.cur_day]);
					strcat(alarm_path, ".mp3");

					alarm_err = MP3Open(0, &alarm_handle, alarm_path);

					free(alarm_path);

					if(alarm_err != 0)
					{
						alarm_err = MP3Open(0, &alarm_handle, "/alarm/default.mp3");
					}
				}

				vol_timer = 0;

				while(1)
				{
					c = serRx(&a);

					BL_on();

					if(clk_timer >= refresh_time)
					{
						clk_timer = 0;
						get_time(&ops);
						if(LCD_rst_timer >= song_title_timer * 2)
						{
							LCD_rst_timer = 0;
							LCDSoftReset();
						}
						LCDPrintTime(ops.cur_h, ops.cur_m, ops.ampm);
					}

					if(ops.alarm_fade != 0 && vol_timer >= vol_fade_speed)
					{
						vol_timer = 0;
						
						MP3ChangeVol(8);
					}

					if(alarm_err == 0)
					{
						if(MP3Play(&alarm_handle) == 2)
						{
							rewind(&alarm_handle);
						}
					}

					if(a != 0)
					{
						break;
					}
				}
			}

			if(cur_t < alarm_t || new_day_flag != 0)
			{
				cbi(ops.flags, alarmflag);
				new_day_flag = 0;
			}
		}
	}

	while(1);

	return 0;
}
