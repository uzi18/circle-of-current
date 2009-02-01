#include "main.h"

// enables the + - up down left right buttons if defined
//#define GHWT

// trigger on either rising or falling edge (never both please)
// these only apply to the 5 pads, buttons and bass pedals are always falling edge
// pull_up_res will enable internal pull-up resistors for the pad pins
// does not effect buttons and bass pedals
//#define trig_on_rise
#define trig_on_fall
//#define pull_up_res 

// minimum pulse time (x = actual pulse time / 0.003)
// keep at 5, raise if you see double hits
#define hit_min_time 5

// hit softness (0 = max, 7 = min, just make it 0)
#define default_hit_softness 0

// simulated degree of thumb stick movement (something under 32)
#define thumbstick_speed 16

// most of this data is found on
// http://wiibrew.org/wiki/Wiimote/Extension_Controllers

// drum id
const unsigned char drum_id[6] = {0x01, 0x00, 0xA4, 0x20, 0x01, 0x03};

// code for each pad
const unsigned char pad_tbl[8] = {
	0xFF,
	0xFF,
	0b10110110,
	0b10011110,
	0b10100100,
	0b10100010,
	0b10110010,
	0b10011100
};

// calibration data
const unsigned char cal_data[16] = {
	0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00
};

static volatile unsigned char hit_f[8]; // hit flag
static volatile unsigned long hit_t[8]; // time associated with flag
static volatile unsigned char hit_last; // last pad hit

// pin input comparison variables
#if defined(PA0)
static volatile unsigned char PINAP;
#endif
#if defined(PB0)
static volatile unsigned char PINBP;
#endif
#if defined(PC0)
static volatile unsigned char PINCP;
#endif
#if defined(PD0)
static volatile unsigned char PINDP;
#endif
#if defined(PE0)
static volatile unsigned char PINEP;
#endif
#if defined(PF0)
static volatile unsigned char PINFP;
#endif

void hit_pcint(unsigned char w)
{
	unsigned long t = wm_read_cnt_read(); // keep time

	if(hit_f[w] == 0) // fresh hit
	{
		hit_f[w] = 1;
		hit_t[w] = t;
		hit_last = w;
	}
}

void check_for_hits()
{
	unsigned char c;
	unsigned char s;

	// compare before and after states

	#ifdef trig_on_fall

	c = green_in_reg;
	s = green_in_preg;
	if(bit_is_clear(c, green_pin) && bit_is_set(s, green_pin)) hit_pcint(green_bit);

	c = red_in_reg;
	s = red_in_preg;
	if(bit_is_clear(c, red_pin) && bit_is_set(s, red_pin)) hit_pcint(red_bit);

	c = yellow_in_reg;
	s = yellow_in_preg;
	if(bit_is_clear(c, yellow_pin) && bit_is_set(s, yellow_pin)) hit_pcint(yellow_bit);

	c = blue_in_reg;
	s = blue_in_preg;
	if(bit_is_clear(c, blue_pin) && bit_is_set(s, blue_pin)) hit_pcint(blue_bit);

	c = orange_in_reg;
	s = orange_in_preg;
	if(bit_is_clear(c, orange_pin) && bit_is_set(s, orange_pin)) hit_pcint(orange_bit);

	#endif

	#ifdef trig_on_rise

	s = green_in_reg;
	c = green_in_preg;
	if(bit_is_clear(c, green_pin) && bit_is_set(s, green_pin)) hit_pcint(green_bit);

	s = red_in_reg;
	c = red_in_preg;
	if(bit_is_clear(c, red_pin) && bit_is_set(s, red_pin)) hit_pcint(red_bit);

	s = yellow_in_reg;
	c = yellow_in_preg;
	if(bit_is_clear(c, yellow_pin) && bit_is_set(s, yellow_pin)) hit_pcint(yellow_bit);

	s = blue_in_reg;
	c = blue_in_preg;
	if(bit_is_clear(c, blue_pin) && bit_is_set(s, blue_pin)) hit_pcint(blue_bit);

	s = orange_in_reg;
	c = orange_in_preg;
	if(bit_is_clear(c, orange_pin) && bit_is_set(s, orange_pin)) hit_pcint(orange_bit);

	#endif
	
	c = bass1_in_reg;
	s = bass1_in_preg;
	if(bit_is_clear(c, bass1_pin) && bit_is_set(s, bass1_pin)) hit_pcint(bass_bit);

	c = bass2_in_reg;
	s = bass2_in_preg;
	if(bit_is_clear(c, bass2_pin) && bit_is_set(s, bass2_pin)) hit_pcint(bass_bit);

	// previous = current
	green_in_preg = green_in_reg;
	red_in_preg = red_in_reg;
	yellow_in_preg = yellow_in_reg;
	blue_in_preg = blue_in_reg;
	orange_in_preg = orange_in_reg;
	bass1_in_preg = bass1_in_reg;
	bass2_in_preg = bass2_in_reg;
}

void check_hit_flags()
{
	if
	(
		hit_f[0] == 0 &&
		hit_f[1] == 0 &&
		hit_f[2] == 0 &&
		hit_f[3] == 0 &&
		hit_f[4] == 0 &&
		hit_f[5] == 0 &&
		hit_f[6] == 0 &&
		hit_f[7] == 0
	)
	{
		// if nothing is hit, then clear timer
		wm_read_cnt_set(0);
	}
	else
	{
		unsigned long t = wm_read_cnt_read();
		for(unsigned char i = 0; i < 8; i++)
		{
			if((t - hit_t[i]) > hit_min_time)
			{
				// clear flag if flag has been set for too long
				hit_f[i] = 0;
			}
		}
	}
}

int main()
{
	sei();

	// start serial port
	sbi(uart_port, uart_rx_pin); // pull up
	serInit(38400);

	// initialize ports

	#ifdef pull_up_res
	#ifdef trig_on_fall
	// setting port = pull ups on
	sbi(green_port, green_pin);
	sbi(red_port, red_pin);
	sbi(yellow_port, yellow_pin);
	sbi(blue_port, blue_pin);
	sbi(orange_port, orange_pin);
	#endif
	#else
	cbi(green_port, green_pin);
	cbi(red_port, red_pin);
	cbi(yellow_port, yellow_pin);
	cbi(blue_port, blue_pin);
	cbi(orange_port, orange_pin);
	#endif

	#ifdef trig_on_rise
	// clearing port = pull ups off
	cbi(green_port, green_pin);
	cbi(red_port, red_pin);
	cbi(yellow_port, yellow_pin);
	cbi(blue_port, blue_pin);
	cbi(orange_port, orange_pin);
	#endif

	sbi(bass1_port, bass1_pin);
	sbi(bass2_port, bass2_pin);

	#ifdef GHWT
	sbi(plus_port, plus_pin);
	sbi(minus_port, minus_pin);
	sbi(up_stick_port, up_stick_pin);
	sbi(down_stick_port, down_stick_pin);
	sbi(left_stick_port, left_stick_pin);
	sbi(right_stick_port, right_stick_pin);
	#endif

	// all input
	cbi(green_ddr, green_pin);
	cbi(red_ddr, red_pin);
	cbi(yellow_ddr, yellow_pin);
	cbi(blue_ddr, blue_pin);
	cbi(orange_ddr, orange_pin);
	cbi(bass1_ddr, bass1_pin);
	cbi(bass2_ddr, bass2_pin);

	#ifdef GHWT
	cbi(plus_ddr, plus_pin);
	cbi(minus_ddr, minus_pin);
	cbi(up_stick_ddr, up_stick_pin);
	cbi(down_stick_ddr, down_stick_pin);
	cbi(left_stick_ddr, left_stick_pin);
	cbi(right_stick_ddr, right_stick_pin);
	#endif

	// preinitialize comparison
	green_in_preg = green_in_reg;
	red_in_preg = red_in_reg;
	yellow_in_preg = yellow_in_reg;
	blue_in_preg = blue_in_reg;
	orange_in_preg = orange_in_reg;
	bass1_in_preg = bass1_in_reg;
	bass2_in_preg = bass2_in_reg;

	// initialize variables	
	unsigned char hit_softness = default_hit_softness;

	// initialize flags
	for(unsigned char i = 0; i < 8; i++)
	{
		hit_f[i] = 0;
		hit_t[i] = 0;
	}

	wm_cd_s but_dat; // struct containing button data
	but_dat.d[0] = 0b00011111;
	but_dat.d[1] = 0b00011111;
	but_dat.d[2] = 0b11111111;
	but_dat.d[3] = 0b11111111;
	but_dat.d[4] = 0b11111111;
	but_dat.d[5] = 0b11111111;

	// start making wiimote think this is a drum
	wm_init(drum_id, but_dat, cal_data);

	while(1)
	{
		// check hardware
		check_for_hits();
		check_hit_flags();

		unsigned char d; // serial port latest data
		unsigned char c; // number of char in serial port buffer
		d = serRx(&c); // check for serial command
		
		if
		(
			hit_f[0] != 0 ||
			hit_f[1] != 0 ||
			hit_f[2] != 0 ||
			hit_f[3] != 0 ||
			hit_f[4] != 0 ||
			hit_f[5] != 0 ||
			hit_f[6] != 0 ||
			hit_f[7] != 0
		) // if any flag is set
		{
			but_dat.d[4] = 0xFF;
			but_dat.d[5] = 0xFF;

			// clear bits according to which pin was pulsed
			for(unsigned char i = 0; i < 8; i++)
			{
				if(hit_f[i] != 0) cbi(but_dat.d[5], i);
			}
		}
		else if(c > 0) // new command over serial port
		{
			but_dat.d[4] = 0xFF;
			but_dat.d[5] = 0xFF;

			if(bit_is_set(d, 0)) cbi(but_dat.d[5], green_bit);
			if(bit_is_set(d, 1)) cbi(but_dat.d[5], red_bit);
			if(bit_is_set(d, 2)) cbi(but_dat.d[5], yellow_bit);
			if(bit_is_set(d, 3)) cbi(but_dat.d[5], blue_bit);
			if(bit_is_set(d, 4)) cbi(but_dat.d[5], orange_bit);
			if(bit_is_set(d, 5)) cbi(but_dat.d[5], bass_bit);

			if(bit_is_set(d, 6)) cbi(but_dat.d[4], minus_bit);
			if(bit_is_set(d, 7)) cbi(but_dat.d[4], plus_bit);
		}

		but_dat.d[2] = 0xFF;
		but_dat.d[3] = 0xFF;

		if(but_dat.d[5] != 0xFF)
		{
			// set softness
			but_dat.d[4] &= 0b01111110;
			but_dat.d[3] = 0b00001100 | (hit_softness << 5);

			unsigned long t = wm_read_cnt_read();
			
			// if any pads active, then send "softness"
			for(unsigned long i = hit_last + t; i < 16; i++)
			{
				unsigned char j = (unsigned char)(i % 8);
				if(bit_is_clear(but_dat.d[5], j))
				{
					but_dat.d[2] = pad_tbl[j];
					break;
				}
			}
		}

		#ifdef GHWT
		// plus and minus buttons
		if(bit_is_clear(plus_in_reg, plus_pin)) cbi(but_dat.d[4], plus_bit);
		if(bit_is_clear(minus_in_reg, minus_pin)) cbi(but_dat.d[4], minus_bit);

		// simulate thumbstick with switches
		but_dat.d[0] = 0b00011111;
		but_dat.d[1] = 0b00011111;
		if(bit_is_clear(up_stick_in_reg, up_stick_pin)) but_dat.d[1] += thumbstick_speed;
		if(bit_is_clear(down_stick_in_reg, down_stick_pin)) but_dat.d[1] -= thumbstick_speed;
		if(bit_is_clear(left_stick_in_reg, left_stick_pin)) but_dat.d[0] -= thumbstick_speed;
		if(bit_is_clear(right_stick_in_reg, right_stick_pin)) but_dat.d[0] += thumbstick_speed;
		#endif

		wm_newaction(but_dat);
	}

	return 0;
}

#ifdef trig_on_fall
#ifdef trig_on_rise
#error "cannot define both trig_on_fall and trig_on_rise"
#endif
#endif
