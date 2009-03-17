#include "esc.h"

static volatile unsigned int esc_chan_width[8];
static volatile unsigned char esc_chan;
static volatile unsigned char esc_done;
static volatile unsigned char esc_extra_chan_num;
static volatile unsigned char esc_safety;

ISR(TIMER1_COMPA_vect)
{
	sbi(TCCR1C, FOC1A);

	OCR1A += esc_chan_width[esc_chan]; // calculate next alarm considering overflow

	esc_chan++;

	if(esc_chan > 4 + esc_extra_chan_num)
	{
		esc_done = 1;
		if(esc_chan == 8)
		{
			esc_chan = 7;
		}
	}
}

void esc_init()
{
	for(unsigned char i = 0; i < 8; i++)
	{
		esc_chan_width[i] = ticks_500us;
	}

	esc_safety = 0;

	cbi(esc_port, esc_rst_pin);
	cbi(esc_port, esc_clk_pin);
	cbi(esc_port, esc_dat_pin);

	sbi(esc_ddr, esc_rst_pin);
	sbi(esc_ddr, esc_clk_pin);
	sbi(esc_ddr, esc_dat_pin);

	timer1_init();

	sbi(TCCR1A, COM1A0);
	sbi(TIMSK1, OCIE1A);
}

void esc_shift_rst()
{
	cbi(esc_port, esc_rst_pin);

	if(esc_safety != 0)
	{
		sbi(esc_port, esc_rst_pin);
	}

	cbi(esc_port, esc_clk_pin);

	sbi(esc_port, esc_dat_pin);

	sbi(esc_port, esc_clk_pin);
	cbi(esc_port, esc_clk_pin);

	cbi(esc_port, esc_dat_pin);
}

void esc_start_next(unsigned int * w)
{
	for(unsigned char i = 0; i < 8; i++)
	{
		esc_chan_width[i] = w[i];
	}

	OCR1A = TCNT1 + 128;

	esc_chan = 0;
	esc_done = 0;

	esc_shift_rst();
}

unsigned char esc_is_done()
{
	return esc_done;
}

void esc_safe(unsigned char c)
{
	esc_safety = c;
}

void esc_set_extra_chan(unsigned char c)
{
	esc_extra_chan_num = c;
}
