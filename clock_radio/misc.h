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

volatile unsigned char btn_A_flags[8];
volatile unsigned char btn_A_timer[8];
volatile unsigned char btn_B_flags[8];
volatile unsigned char btn_B_timer[8];

void btn_port_init()
{
	btn_A_output_reg |= 00111111;
	btn_B_output_reg |= 01111111;
}

void check_btns()
{
	for(unsigned char i = 0; i < 8; i++)
	{
		if(bit_is_set(btn_A_input_reg, i) && bit_is_clear(btn_A_flags[i], last_state_flag))
		{
			btn_A_timer[i] = 0;
		}
		else if(bit_is_clear(btn_A_input_reg, i) && bit_is_set(btn_A_flags[i], last_state_flag))
		{
			btn_A_timer[i] = 0;
		}
		else
		{
			if(btn_A_timer[i] < 255)
			{
				btn_A_timer[i]++;
			}
			if(btn_A_timer[i] > btn_debounce_time)
			{
				if(bit_is_clear(btn_A_input_reg, i))
				{
					if(bit_is_set(btn_A_flags[i], fixed_state_flag))
					{
						sbi(btn_A_flags[i], click_flag);
					}
					cbi(btn_A_flags[i], fixed_state_flag);
				}
				else
				{
					sbi(btn_A_flags[i], fixed_state_flag);
				}
			}
		}
		if(bit_is_clear(btn_A_input_reg, i))
		{
			cbi(btn_A_flags[i], last_state_flag);
		}
		else
		{
			sbi(btn_A_flags[i], last_state_flag);
		}

		if(bit_is_set(btn_B_input_reg, i) && bit_is_clear(btn_B_flags[i], last_state_flag))
		{
			btn_B_timer[i] = 0;
		}
		else if(bit_is_clear(btn_B_input_reg, i) && bit_is_set(btn_B_flags[i], last_state_flag))
		{
			btn_B_timer[i] = 0;
		}
		else
		{
			if(btn_B_timer[i] < 255)
			{
				btn_B_timer[i]++;
			}
			if(btn_B_timer[i] > btn_debounce_time)
			{
				if(bit_is_clear(btn_B_input_reg, i))
				{
					if(bit_is_set(btn_B_flags[i], fixed_state_flag))
					{
						sbi(btn_B_flags[i], click_flag);
					}
					cbi(btn_B_flags[i], fixed_state_flag);
				}
				else
				{
					sbi(btn_B_flags[i], fixed_state_flag);
				}
			}
		}
		if(bit_is_clear(btn_B_input_reg, i))
		{
			cbi(btn_B_flags[i], last_state_flag);
		}
		else
		{
			sbi(btn_B_flags[i], last_state_flag);
		}
	}
}
