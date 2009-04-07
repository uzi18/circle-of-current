volatile unsigned long long ovf_cnt_1;
volatile unsigned long long ovf_cnt_2;
volatile unsigned long menu_timer;
volatile unsigned long song_timer;
volatile unsigned long clk_timer;
volatile unsigned long fade_timer;
volatile unsigned long vol_timer;
volatile unsigned long BL_timer;
volatile unsigned char BL_mode;
volatile unsigned char rtc_cnt;
volatile unsigned char sec_cnt;
volatile unsigned char min_cnt;
volatile unsigned char hour_cnt;
volatile unsigned char day_cnt;
#define BL_on_speed 0
#define BL_off_speed 2

ISR(BADISR_vect)
{
}

ISR(TIMER1_OVF_vect)
{
	ovf_cnt_1++;
}

ISR(TIMER2_OVF_vect)
{
	ovf_cnt_2++;
	song_timer++;
	clk_timer++;
	BL_timer++;
	menu_timer++;
	vol_timer++;

	if(BL_timer >= 2441)
	{
		BL_mode = 0;
		BL_timer = 2441;
	}

	if(BL_mode == 0 && fade_timer >= BL_off_speed)
	{
		if(OCR2A > 0)
		{
			OCR2A--;
		}
		fade_timer = 0;
	}
	else if(BL_mode == 1 && fade_timer >= BL_on_speed)
	{
		if(OCR2A < 255)
		{
			OCR2A++;
		}
		fade_timer = 0;
	}
	else
	{
		fade_timer++;
	}

	rtc_cnt++;

	if(rtc_cnt == 128)
	{
		rtc_cnt = 0;
		sec_cnt++;
	}

	if(sec_cnt == 60)
	{
		sec_cnt = 0;
		min_cnt++;
	}

	if(min_cnt == 60)
	{
		min_cnt = 0;
		hour_cnt++;
	}

	if(hour_cnt == 24)
	{
		hour_cnt = 0;
		day_cnt++;
	}

	if(day_cnt == 7)
	{
		day_cnt = 0;
	}


	disk_timerproc();
}

void timer_init()
{
	rtc_cnt = 0;
	sec_cnt = 0;
	min_cnt = 0;
	hour_cnt = 0;
	day_cnt = 0;
	BL_timer = 0;
	BL_mode = 1;
	fade_timer = 0;
	ovf_cnt_1 = 0;
	ovf_cnt_2 = 0;
	song_timer = 0;
	clk_timer = 0;
	menu_timer = 0;
	vol_timer = 0;
	sbi(ASSR, AS2);
	TCNT2 = 0;
	TCCR2A = 0b10000011;
	OCR2A = 0;
	TCCR2B = 0b00000001;
	sbi(TIMSK2, TOIE2);
	TCCR1B = 0b00000101;
	sbi(TIMSK1, TOIE1);
}

void f_exe(FRESULT r, const char * s)
{
	if(r != 0)
	{
		#ifdef ser_debug
		fprintf_P(&serstdout, PSTR("err "));
		char c;
		while ((c = pgm_read_byte(s++)))
		{
			fputc(c, &serstdout);
		}
		fputc('\n', &serstdout);
		fputc('\n', &serstdout);
		#endif

		abort();
	}
}

DWORD get_fattime()
{
	return 0;
}

void BL_on()
{
	BL_mode = 1;
	BL_timer = 0;
}
