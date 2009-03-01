#include <avr/io.h>
#include <avr/interrupt.h>

//#define SIMULATE

#ifndef SIMULATE
#define AUTOBAUDRATEDETECT // switch to enable or disable automatic baud rate detection upon compile
#endif

// below are default settings, edit them if needed

#if (F_CPU == 8000000)

#define BAUD_RATE_REG 12 // default for 38400 baud at 8 MHz
#define default_ticks 12000 // 1500 microseconds for 8 MHz clock
#define default_period (20000 * 8) // 2500 microseconds * 8 channels for 8 MHz clock

#elif (F_CPU == 20000000)

#define BAUD_RATE_REG 64 // default for 19200 baud at 20 MHz
#define default_ticks 30000 // 1500 microseconds for 20 MHz clock
#define default_period (50000 * 8) // 2500 microseconds * 8 channels for 20 MHz clock

#elif (F_CPU != NULL)

#define BAUD_RATE_REG 12 // default for 38400 baud at 8 MHz
#define default_ticks 12000 // 1500 microseconds for 8 MHz clock
#define default_period (20000 * 8) // 2500 microseconds * 8 channels for 8 MHz clock

#else

#error "Please set a clock frequency"

#endif

// port names
#define out_port PORTB
#define out_ddr DDRB

// global variables
static volatile unsigned int ticks[10]; // widths for each channel
static volatile unsigned char chan_en; // channel enabled bit mask
static volatile unsigned long period_ticks; // period length
static volatile unsigned char next_mask; // pin mask
static volatile unsigned char chan; // current channel
static volatile unsigned long sum; // sum of time since start of period
static volatile unsigned long ovf_needed; // overflows needed, used for long periods

ISR(TIMER1_COMPA_vect) // timer 1 output compare A interrupt
{
	out_port = next_mask; // new channel output
	unsigned long t = (OCR1A + ticks[chan]) & 0xFFFF; // calculate next alarm considering overflow
	OCR1A = t; // set next alarm
	sum += ticks[chan]; // sum is elapsed time for current period
	do
	{
		chan++; // next channel
	}
	while(bit_is_clear(chan_en, chan) && chan < 8); // if disabled, increment again, exit if final channel
	if(chan == 8) // final channel
	{
		next_mask = 0; // pins off on next interrupt
		if(period_ticks > sum) // if time left over
		{
			unsigned long diff = period_ticks - sum; // calculate remainder
			t = diff & 0xFFFF; // calculate remainder
			ticks[8] = 0; // use 0 to cause overflow
			ticks[9] = t; // store remainder
			ovf_needed = (diff & 0xFFFF0000) >> 16; // calculate overflow
			return;
		}
		else
		{
			chan = 9; // no delay needed
			ovf_needed = 0;
		}
	}
	if(chan == 9) // if all channels done
	{
		if(ovf_needed == 0) // no overflows needed
		{
			chan = 0; // start from first channel
			sum = 0; // reset period counter
			while(bit_is_clear(chan_en, chan) && chan < 8)
			{
				chan++; // next channel until enabled or end of all channels
			}
		}
		else // overflows needed
		{
			chan = 8; // make next interrupt on overflow
			ovf_needed--; // one less needed
			if(ovf_needed == 0) // won't be needed on next
			{
				ticks[8] = ticks[9]; // load remainder
			}
		}
	}
	next_mask = _BV(chan); // prepare next pin mask
	return;
}

// waits for then returns a serial byte
unsigned char rx()
{
	loop_until_bit_is_set(UCSRA, RXC);
	unsigned char d = UDR;
	return d;
}

#ifdef AUTOBAUDRATEDETECT
// round to the nearest hundreds
unsigned long round100(unsigned long tr)
{
	unsigned long tens = tr % 100;
	if(tens >= 50)
	{
		tr += 100 - tens;
	}
	else
	{
		tr -= tens;
	}
	return tr;
}
#endif

int main()
{
	unsigned char stop_flag;

	out_ddr = 0xFF; // channels as output
	out_port = 0; // all off

	#ifdef AUTOBAUDRATEDETECT

	TCCR1B = 0b00000001; // start timer, clock div 1

	// capture time of first falling edge
	loop_until_bit_is_set(TIFR, ICF1);
	unsigned long tc1 = ICR1;
	TIFR |= _BV(ICF1);

	// capture time of second falling edge
	loop_until_bit_is_set(TIFR, ICF1);
	unsigned long tc2 = ICR1;

	// calculate difference in time
	unsigned long auto_baud = ((tc2 | 0x10000) - tc1) & 0xFFFF;

	auto_baud *= 100; // multiply by 100 so that the number can be divided then rounded
	auto_baud = round100(auto_baud / 10); // 10 is because there's 10 bit widths between falling edges
	auto_baud = round100(auto_baud / 16);
	auto_baud = (auto_baud / 100) - 1; // un-multiply, then subtract 1

	// apply to baud rate register
	UBRRH = (auto_baud & 0xFF00) >> 8; 
	UBRRL = auto_baud;

	#else

	// apply default to baud rate register
	UBRRH = (BAUD_RATE_REG & 0xFF00) >> 8;
	UBRRL = BAUD_RATE_REG;

	#endif

	UCSRB = _BV(RXEN); // enable serial port

	// reset variables and set defaults
	chan = 0;
	next_mask = 0;
	sum = 0;
	stop_flag = 1;
	period_ticks = default_period;
	chan_en = 0;
	ticks[0] = default_ticks; ticks[1] = default_ticks; ticks[2] = default_ticks; ticks[3] = default_ticks; ticks[4] = default_ticks; ticks[5] = default_ticks; ticks[6] = default_ticks; ticks[7] = default_ticks;

	#ifdef SIMULATE

	stop_flag = 0;
	chan_en = 0xFF;

	// get ready to start pulsing
	out_port = 0;
	OCR1A = 0x8000;
	TIMSK = _BV(OCIE1A);
	TCCR1B = 0b00000001;
	TCNT1 = 0;
	sei(); // enable interrupt

	#endif

	while(1)
	{
		unsigned char d = rx(); // command byte

		if(d == 9) // enable/disable channels
		{
			chan_en = rx();
			if(chan_en == 0) // all off
			{
				stop_flag = 1; // has been stopped
				cli(); // no more timer interrupts
				out_port = 0; // all channels off
			}
			else // at least one channel
			{
				if(stop_flag != 0) // previously stopped
				{
					// reset channel counter
					for(unsigned char i = chan; i < 16; i++)
					{
						if(bit_is_set(chan_en, i % 8))
						{
							chan = i % 8;
						}
					}
					// get ready to start pulsing
					out_port = 0;
					OCR1A = 0x8000;
					TIMSK = _BV(OCIE1A);
					TCCR1B = 0b00000001;
					TCNT1 = 0;
					sei(); // enable interrupt
				}
				stop_flag = 0; // not stopped anymore
			}
		}
		else if(d == 10) // set period length
		{
			// read in 4 bytes
			unsigned char _24_31 = rx();
			unsigned char _16_23 = rx();
			unsigned char _8_15 = rx();
			unsigned char _0_7 = rx();

			// calculate into 32 bit integer
			unsigned long res = _24_31;
			res <<= 8;
			res += _16_23;
			res <<= 8;
			res += _8_15;
			res <<= 8;
			res += _0_7;

			// set period length
			period_ticks = res;
		}
		else if(d <= 8 &&  d != 0) // set channel pulse width
		{
			unsigned char h = rx(); // read high byte
			unsigned char l = rx(); // read low byte

			// combine bytes into 16 bit integer
			unsigned int t = h;
			t <<= 8;
			t += l;

			// set width
			ticks[d - 1] = t;
		}
	}

	return 0;
}
