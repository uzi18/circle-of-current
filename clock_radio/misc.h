volatile unsigned long ovf_cnt;
volatile unsigned long song_timer;
volatile unsigned long clk_timer;
volatile unsigned long fade_timer;
volatile unsigned long BL_timer;
volatile unsigned char BL_mode;
#define BL_on_speed 0
#define BL_off_speed 2

ISR(BADISR_vect)
{
}

ISR(TIMER2_OVF_vect)
{
	ovf_cnt++;
	song_timer++;
	clk_timer++;
	BL_timer++;

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

	disk_timerproc();
}

void timer_init()
{
	BL_timer = 0;
	BL_mode = 1;
	fade_timer = 0;
	ovf_cnt = 0;
	song_timer = 0;
	clk_timer = 0;
	TCCR2A = 0b10000011;
	OCR2A = 0;
	TCCR2B = 0b00000101;
	sbi(TIMSK2, TOIE2);
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
