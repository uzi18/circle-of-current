#include "esc.h"

static volatile unsigned int esc_chan_width[8];
static volatile unsigned char esc_chan;
static volatile signed long esc_elapsed;
static volatile unsigned char esc_done;
static volatile unsigned char esc_safety;
static volatile unsigned char esc_extra_servo;

ISR(TIMER1_COMPA_vect)
{
	sbi(TCCR1C, FOC1A);

	OCR1A += esc_chan_width[esc_chan]; // calculate next alarm considering overflow
	esc_elapsed += esc_chan_width[esc_chan];

	esc_chan++;

	if(esc_chan == 4 + esc_extra_servo)
	{
		if(esc_elapsed > ticks_10ms)
		{
			esc_done = 1;
		}
		else
		{
			esc_chan--;
		}
	}
}

void esc_init()
{
	for(unsigned char i = 0; i < 8; i++)
	{
		esc_chan_width[i] = ticks_500us;
	}

	esc_safety = 1;

	cbi(esc_port, esc_rst_pin);
	cbi(esc_port, esc_clk_pin);
	cbi(esc_port, esc_dat_pin);

	sbi(esc_ddr, esc_rst_pin);
	sbi(esc_ddr, esc_clk_pin);
	sbi(esc_ddr, esc_dat_pin);

	esc_start_next();

	timer1_init();

	sbi(TCCR1A, COM1A0);
	sbi(TIMSK1, OCIE1A);
}

void esc_shift_rst()
{
	cbi(esc_port, esc_rst_pin);
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();

	if(esc_safety == 0)
	{
		sbi(esc_port, esc_rst_pin);
	}

	cbi(esc_port, esc_clk_pin);

	sbi(esc_port, esc_dat_pin);

	sbi(TCCR1C, FOC1A);
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();
	sbi(TCCR1C, FOC1A);

	cbi(esc_port, esc_dat_pin);
}

void esc_start_next()
{
	OCR1A = TCNT1 + 128;

	esc_chan = 0;
	esc_done = 0;
	esc_elapsed = 0;

	esc_shift_rst();
}

unsigned char esc_is_done()
{
	return esc_done;
}

void esc_is_done_clear()
{
	esc_done = 0;
}

void esc_safe(unsigned char c)
{
	esc_safety = c;
}

void esc_set_width(unsigned char c, unsigned int w)
{
	if(w < ticks_500us * 2)
	{
		w = ticks_500us * 2;
	}
	else if(w > ticks_500us * 4)
	{
		w = ticks_500us * 4;
	}
	
	esc_chan_width[c] = w;
}

unsigned long esc_get_total()
{
	unsigned long sum = 0;
	for(unsigned char i = 0; i < 4 + esc_extra_servo; i++)
	{
		sum += esc_chan_width[i];
	}
	return sum;
}

void esc_set_extra_chan(unsigned char n)
{
	esc_extra_servo = n;
}
