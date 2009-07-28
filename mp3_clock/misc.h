volatile unsigned long long ovf_cnt_2;
volatile unsigned long menu_timer;
volatile unsigned long song_title_timer;
volatile unsigned long clk_timer;
volatile unsigned long LCD_rst_timer;
volatile unsigned long fade_timer;
volatile unsigned long vol_timer;
volatile unsigned long BL_timer;
volatile unsigned char BL_mode;
volatile unsigned char rtc_cnt;
volatile unsigned char sec_cnt;
volatile unsigned char min_cnt;
volatile unsigned char hour_cnt;
volatile unsigned char day_cnt;
volatile unsigned char new_day_flag;

ISR(BADISR_vect)
{
}

ISR(TIMER2_OVF_vect)
{
	ovf_cnt_2++;
	song_title_timer++;
	clk_timer++;
	BL_timer++;
	menu_timer++;
	vol_timer++;
	LCD_rst_timer++;

	if(BL_timer >= BL_timeout)
	{
		BL_mode = 0;
		BL_timer = BL_timeout;
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

	if(rtc_cnt == F_OSC / 256)
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
		new_day_flag = 1;
	}

	if(day_cnt == 7)
	{
		day_cnt = 0;
	}

	check_btns();

	disk_timerproc();
}

void timer_init()
{
	new_day_flag = 1;
	rtc_cnt = 0;
	sec_cnt = 0;
	min_cnt = 0;
	hour_cnt = 0;
	day_cnt = 0;
	LCD_rst_timer = 0;
	BL_timer = 0;
	BL_mode = 1;
	fade_timer = 0;
	ovf_cnt_2 = 0;
	song_title_timer = 0;
	clk_timer = 0;
	menu_timer = 0;
	vol_timer = 0;
	sbi(ASSR, AS2);
	TCCR2A = 0b10000011;
	OCR2A = 0;
	TCCR2B = 0b00000001;
	sbi(TIMSK2, TOIE2);
}

void BL_on()
{
	BL_mode = 1;
	BL_timer = 0;
}

volatile unsigned char btn_flags[16];
volatile unsigned char btn_timer[16];

void btn_port_init()
{
	btn_A_output_reg |= 00111111;
	btn_B_output_reg |= 01111111;
}

void check_btns()
{
	for(unsigned char i = 0; i < 8; i++)
	{
		if(bit_is_set(btn_A_input_reg, i) && bit_is_clear(btn_flags[i], last_state_flag))
		{
			btn_timer[i] = 0;
		}
		else if(bit_is_clear(btn_A_input_reg, i) && bit_is_set(btn_flags[i], last_state_flag))
		{
			btn_timer[i] = 0;
		}
		else
		{
			if(btn_timer[i] < 255)
			{
				btn_timer[i]++;
			}
			if(btn_timer[i] > btn_debounce_time)
			{
				if(bit_is_clear(btn_A_input_reg, i))
				{
					if(bit_is_set(btn_flags[i], fixed_state_flag))
					{
						sbi(btn_flags[i], click_flag);
					}
					cbi(btn_flags[i], fixed_state_flag);
				}
				else
				{
					sbi(btn_flags[i], fixed_state_flag);
				}
			}
		}
		if(bit_is_clear(btn_A_input_reg, i))
		{
			cbi(btn_flags[i], last_state_flag);
		}
		else
		{
			sbi(btn_flags[i], last_state_flag);
		}

		if(bit_is_set(btn_B_input_reg, i) && bit_is_clear(btn_flags[8 + i], last_state_flag))
		{
			btn_timer[8 + i] = 0;
		}
		else if(bit_is_clear(btn_B_input_reg, i) && bit_is_set(btn_flags[8 + i], last_state_flag))
		{
			btn_timer[8 + i] = 0;
		}
		else
		{
			if(btn_timer[8 + i] < 255)
			{
				btn_timer[8 + i]++;
			}
			if(btn_timer[8 + i] > btn_debounce_time)
			{
				if(bit_is_clear(btn_B_input_reg, i))
				{
					if(bit_is_set(btn_flags[8 + i], fixed_state_flag))
					{
						sbi(btn_flags[8 + i], click_flag);
					}
					cbi(btn_flags[8 + i], fixed_state_flag);
				}
				else
				{
					sbi(btn_flags[8 + i], fixed_state_flag);
				}
			}
		}
		if(bit_is_clear(btn_B_input_reg, i))
		{
			cbi(btn_flags[8 + i], last_state_flag);
		}
		else
		{
			sbi(btn_flags[8 + i], last_state_flag);
		}
	}
}

const unsigned char waveheader[44] PROGMEM = {
	0x52, 0x49, 0x46, 0x46, 0xFF, 0xFF, 0xFF, 0xFF, 0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20,
	0x10, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00,	0x44, 0xAC, 0x00, 0x00, 0x88, 0x58, 0x01, 0x00,
	0x02, 0x00, 0x10, 0x00, 0x64, 0x61, 0x74, 0x61, 0xFF, 0xFF, 0xFF, 0xFF,
};

const unsigned char waveform[200] PROGMEM = {
	0x00, 0x00, 0x07, 0x18, 0xC5, 0x2F, 0xF3, 0x46, 0x4C, 0x5D, 0x8E, 0x72, 0xFF, 0x7F, 0xFF, 0x7F,
	0xFF, 0x7F, 0xFF, 0x7F, 0xFF, 0x7F, 0xFF, 0x7F,	0xFF, 0x7F, 0xFF, 0x7F, 0xFF, 0x7F, 0xFF, 0x7F,
	0xFF, 0x7F, 0xFF, 0x7F, 0xFF, 0x7F, 0xFF, 0x7F, 0xFF, 0x7F, 0xFF, 0x7F, 0xFF, 0x7F, 0xFF, 0x7F,
	0xFF, 0x7F, 0xFF, 0x7F, 0xAB, 0x70, 0x2D, 0x60, 0xA9, 0x4F, 0x60, 0x3F, 0x8D, 0x2F, 0x6C, 0x20,
	0x30, 0x12, 0x06, 0x05, 0x21, 0xF9, 0x98, 0xEE,	0x8E, 0xE5, 0x1F, 0xDE, 0x4B, 0xD8, 0x28, 0xD4,
	0xA4, 0xD1, 0xC5, 0xD0, 0x73, 0xD1, 0x97, 0xD3, 0x13, 0xD7, 0xC7, 0xDB, 0x85, 0xE1, 0x1E, 0xE8,
	0x64, 0xEF, 0x1B, 0xF7, 0x16, 0xFF, 0x17, 0x07, 0xE5, 0x0E, 0x48, 0x16, 0x0E, 0x1D, 0x00, 0x23,
	0xF3, 0x27, 0xBA, 0x2B, 0x30, 0x2E, 0x36, 0x2F,	0xB1, 0x2E, 0x92, 0x2C, 0xCA, 0x28, 0x58, 0x23,
	0x46, 0x1C, 0x99, 0x13, 0x66, 0x09, 0xCA, 0xFD, 0xE9, 0xF0, 0xE6, 0xE2, 0xF3, 0xD3, 0x45, 0xC4,
	0x13, 0xB4, 0x97, 0xA3, 0x0F, 0x93, 0xC1, 0x82, 0x00, 0x80, 0x00, 0x80, 0x00, 0x80, 0x00, 0x80,
	0x00, 0x80, 0x00, 0x80, 0x00, 0x80, 0x00, 0x80,	0x00, 0x80, 0x00, 0x80, 0x00, 0x80, 0x00, 0x80,
	0x00, 0x80, 0x00, 0x80, 0x00, 0x80, 0x00, 0x80, 0x00, 0x80, 0x00, 0x80, 0x00, 0x80, 0xC9, 0x88,
	0xC5, 0x9D, 0xE5, 0xB3, 0xE5, 0xCA, 0x8D, 0xE2,
};

static unsigned char wave_last_index;

void start_alarm()
{
	MP3Reset();

	while(bit_is_clear(MP3_PinIn, MP3_DREQ_Pin));

	cbi(MP3_Port, MP3_xDCS_Pin);

	for(unsigned char i = 0; i < 22; i++)
	{
		SPITx(pgm_read_byte(&waveheader[i]));
	}

	sbi(MP3_Port, MP3_xDCS_Pin);

	while(bit_is_clear(MP3_PinIn, MP3_DREQ_Pin));

	cbi(MP3_Port, MP3_xDCS_Pin);

	for(unsigned char i = 0, j = 22; i < 22; i++, j++)
	{
		SPITx(pgm_read_byte(&waveheader[j]));
	}

	sbi(MP3_Port, MP3_xDCS_Pin);
}

void alarm_play()
{
	if(bit_is_set(MP3_PinIn, MP3_DREQ_Pin))
	{
		wave_last_index %= 10;

		cbi(MP3_Port, MP3_xDCS_Pin);

		for(unsigned char i = 0, j = 20 * wave_last_index; i < 20; i++, j++)
		{
			SPITx(pgm_read_byte(&waveform[j]));
		}

		sbi(MP3_Port, MP3_xDCS_Pin);

		wave_last_index++;
	}
}
