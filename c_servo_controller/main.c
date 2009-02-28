#include <inttypes.h>
#include <avr/io.h>
#include <avr/interrupt.h>

#define AUTOBAUDRATEDETECT // switch to enable or disable automatic baud rate detection upon compile

#define offset 41 // found using simulator

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
static volatile uint16_t ticks[9]; // widths for each channel
static volatile uint8_t chan_en; // channel enabled bit mask
static volatile uint32_t period_ticks; // period length
static volatile uint8_t next_mask; // pin mask
static volatile uint8_t chan; // current channel
static volatile uint32_t sum; // sum of time since start of period

ISR(TIMER1_COMPA_vect) // timer 1 output compare A interrupt
{
	out_port = next_mask; // new channel output
	TCNT1 = 0; // reset timer
	OCR1A = ticks[chan] - offset; // set new alarm
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
			uint32_t diff = period_ticks - sum; // calculate remainder
			if(diff > 0xFFFF)
			{
				// if too much, then try as long as possible
				ticks[8] = 0xFFFF;
			}
			else
			{
				// set delay period
				ticks[8] = diff;
			}
			return;
		}
		else
		{
			chan = 9; // no delay needed
		}
	}
	if(chan == 9) // if all channels done
	{
		chan = 0; // start from first channel
		sum = 0; // reset period counter
		while(bit_is_clear(chan_en, chan) && chan < 8)
		{
			chan++; // next channel until enabled or end of all channels
		}
	}
	next_mask = _BV(chan); // prepare next pin mask
	return;
}

// waits for then returns a serial byte
uint8_t rx()
{
	loop_until_bit_is_set(UCSRA, RXC);
	uint8_t d = UDR;
	return d;
}

#ifdef AUTOBAUDRATEDETECT
// round to the nearest hundreds
uint32_t round100(uint32_t tr)
{
	uint32_t tens = tr % 100;
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
	uint8_t stop_flag;

	out_ddr = 0xFF; // channels as output
	out_port = 0; // all off

	#ifdef AUTOBAUDRATEDETECT

	TCCR1B = 0b00000001; // start timer, clock div 1

	// capture time of first falling edge
	loop_until_bit_is_set(TIFR, ICF1);
	uint32_t tc1 = ICR1;
	TIFR |= _BV(ICF1);

	// capture time of second falling edge
	loop_until_bit_is_set(TIFR, ICF1);
	uint32_t tc2 = ICR1;

	// calculate difference in time
	uint32_t auto_baud = ((tc2 | 0x10000) - tc1) & 0xFFFF;

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

	while(1)
	{
		uint8_t d = rx(); // command byte

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
					for(uint8_t i = chan; i < 16; i++)
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
			uint8_t _24_31 = rx();
			uint8_t _16_23 = rx();
			uint8_t _8_15 = rx();
			uint8_t _0_7 = rx();

			// calculate into 32 bit integer
			uint32_t res = _24_31;
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
			uint8_t h = rx(); // read high byte
			uint8_t l = rx(); // read low byte

			// combine bytes into 16 bit integer
			uint16_t t = h;
			t <<= 8;
			t += l;

			// set width
			ticks[d - 1] = t;
		}
	}

	return 0;
}
