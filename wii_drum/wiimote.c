#include "wiimote.h"
#include "wm_crypto.h"

// pointer to user function
static void (*wm_sample_event)();

// id and calibration data
static volatile unsigned char wm_id[6];
static volatile unsigned char wm_cal_data[32];

// crypto data
static volatile unsigned char wm_rand[10];
static volatile unsigned char wm_key[6];
static volatile unsigned char wm_ft[8];
static volatile unsigned char wm_sb[8];

// button data
static volatile wm_cd_s wm_action;
static volatile wm_cd_s wm_action_old;

/*

I'd like to thank Hector Martin for posting his encryption method!
His website is http://www.marcansoft.com/
Decryption method found at http://www.derkeiler.com/pdf/Newsgroups/sci.crypt/2008-11/msg00110.pdf

*/

unsigned char wm_ror8(unsigned char a, unsigned char b)
{
	// bit shift with roll-over
	return (a >> b) | ((a << (8 - b)) & 0xFF);
}

void wm_gentabs()
{
	unsigned char idx;

	// check all idx
	for(idx = 0; idx < 7; idx++)
	{
		// generate test key
		unsigned char ans[6];
		unsigned char tkey[6];
		unsigned char t0[10];
		
		for(unsigned char i = 0; i < 6; i++)
		{
			ans[i] = pgm_read_byte(&(ans_tbl[idx][i]));
		}	
		for(unsigned char i = 0; i < 10; i++)
		{
			t0[i] = pgm_read_byte(&(sboxes[0][wm_rand[i]]));
		}
	
		tkey[0] = ((wm_ror8((ans[0] ^ t0[5]), (t0[2] % 8)) - t0[9]) ^ t0[4]);
		tkey[1] = ((wm_ror8((ans[1] ^ t0[1]), (t0[0] % 8)) - t0[5]) ^ t0[7]);
		tkey[2] = ((wm_ror8((ans[2] ^ t0[6]), (t0[8] % 8)) - t0[2]) ^ t0[0]);
		tkey[3] = ((wm_ror8((ans[3] ^ t0[4]), (t0[7] % 8)) - t0[3]) ^ t0[2]);
		tkey[4] = ((wm_ror8((ans[4] ^ t0[1]), (t0[6] % 8)) - t0[3]) ^ t0[4]);
		tkey[5] = ((wm_ror8((ans[5] ^ t0[7]), (t0[8] % 8)) - t0[5]) ^ t0[9]);

		// compare with actual key
		if(memcmp(tkey, wm_key, 6) == 0) break; // if match, then use this idx
	}

	// generate encryption from idx key and rand
	wm_ft[0] = pgm_read_byte(&(sboxes[idx + 1][wm_key[4]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[3]]));
	wm_ft[1] = pgm_read_byte(&(sboxes[idx + 1][wm_key[2]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[5]]));
	wm_ft[2] = pgm_read_byte(&(sboxes[idx + 1][wm_key[5]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[7]]));
	wm_ft[3] = pgm_read_byte(&(sboxes[idx + 1][wm_key[0]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[2]]));
	wm_ft[4] = pgm_read_byte(&(sboxes[idx + 1][wm_key[1]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[4]]));
	wm_ft[5] = pgm_read_byte(&(sboxes[idx + 1][wm_key[3]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[9]]));
	wm_ft[6] = pgm_read_byte(&(sboxes[idx + 1][wm_rand[0]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[6]]));
	wm_ft[7] = pgm_read_byte(&(sboxes[idx + 1][wm_rand[1]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[8]]));
	
	wm_sb[0] = pgm_read_byte(&(sboxes[idx + 1][wm_key[0]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[1]]));
	wm_sb[1] = pgm_read_byte(&(sboxes[idx + 1][wm_key[5]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[4]]));
	wm_sb[2] = pgm_read_byte(&(sboxes[idx + 1][wm_key[3]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[0]]));
	wm_sb[3] = pgm_read_byte(&(sboxes[idx + 1][wm_key[2]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[9]]));
	wm_sb[4] = pgm_read_byte(&(sboxes[idx + 1][wm_key[4]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[7]]));
	wm_sb[5] = pgm_read_byte(&(sboxes[idx + 1][wm_key[1]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[8]]));
	wm_sb[6] = pgm_read_byte(&(sboxes[idx + 1][wm_rand[3]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[5]]));
	wm_sb[7] = pgm_read_byte(&(sboxes[idx + 1][wm_rand[2]])) ^ pgm_read_byte(&(sboxes[idx + 2][wm_rand[6]]));
}

// put data into TWI slave register, encrypted if encryption is enabled
void wm_transmit(unsigned char * d, unsigned char addr, unsigned char l)
{
	if(twi_read_reg(0xF0) == 0xAA && addr != 0xF0)
	{
		for(unsigned char i = 0; i < l; i++)
		{
			twi_set_reg(addr + i, (d[i] - wm_ft[(addr + i) % 8]) ^ wm_sb[(addr + i) % 8]);
		}
	}
	else
	{
		for(unsigned char i = 0; i < l; i++)
		{
			twi_set_reg(addr + i, d[i]);
		}
	}
}

void wm_slaveTxStart(unsigned char addr)
{
	unsigned char d[8];
	if(addr >= 0x00 && addr < 0x06)
	{
		// call user event
		wm_sample_event();
	}
	if(addr >= 0x20 && addr < 0x28)
	{
		// requested calibration data
		for(unsigned char i = addr - 0x20, j = 0; j < 8; i++, j++)
		{			
			d[j] = wm_cal_data[i];
		}
		wm_transmit(d, addr, 8);
	}
	if(addr >= 0x28 && addr < 0x30)
	{
		// requested calibration data
		for(unsigned char i = addr - 0x20, j = 0; j < 8; i++, j++)
		{			
			d[j] = wm_cal_data[i];
		}
		wm_transmit(d, addr, 8);
	}
	if(addr >= 0x30 && addr < 0x38)
	{
		// requested calibration data
		for(unsigned char i = addr - 0x20, j = 0; j < 8; i++, j++)
		{			
			d[j] = wm_cal_data[i];
		}
		wm_transmit(d, addr, 8);
	}
	if(addr >= 0x38 && addr <= 0x3F)
	{
		// requested calibration data
		for(unsigned char i = addr - 0x20, j = 0; j < 8; i++, j++)
		{			
			d[j] = wm_cal_data[i];
		}
		wm_transmit(d, addr, 8);
	}
	if(addr >= 0xFA && addr <= 0xFF)
	{
		// requested id
		for(unsigned char i = addr - 0xFA, j = 0; j < 6; i++, j++)
		{			
			d[j] = wm_id[i];
		}
		wm_transmit(d, addr, 6);
	}
}

void wm_slaveTxEnd(unsigned char addr, unsigned char l)
{
}

void wm_slaveRx(unsigned char addr, unsigned char l)
{
	{
		// if encryption data is sent, store them accordingly
		if(addr >= 0x40 && addr < 0x46)
		{
			for(unsigned char i = 0; i < 6; i++)
			{
				wm_rand[9 - i] = twi_read_reg(0x40 + i);
			}
		}
		else if(addr >= 0x46 && addr < 0x4C)
		{
	
			for(unsigned char i = 6; i < 10; i++)
			{
				wm_rand[9 - i] = twi_read_reg(0x40 + i);
			}
			for(unsigned char i = 0; i < 2; i++)
			{
				wm_key[5 - i] = twi_read_reg(0x40 + 10 + i);
			}
		}
		else if(addr >= 0x4C && addr < 0x50)
		{
			for(unsigned char i = 2; i < 6; i++)
			{
				wm_key[5 - i] = twi_read_reg(0x40 + 10 + i);
			}
			if(addr + l == 0x50)
			{
				// generate decryption once all data is loaded
				wm_gentabs();
		
				wm_transmit(wm_action.d, 0x00, 6);
			}
		}
	}
}

void wm_newaction(wm_cd_s t)
{
	// load button data from user application
	wm_action = t;

	#ifdef GHWT

	if(memcmp(wm_action_old.d, wm_action.d) != 0)
	{
		// encrypt button data
		wm_transmit(wm_action.d, 0x00, 6);
		wm_action_old = wm_action;
	}

	#else

	unsigned char c[1];
	c[0] = wm_action.d[5];
	wm_transmit(c, 0x00 + 5, 1);

	#endif
}

void wm_init(unsigned char * id, wm_cd_s t, unsigned char * cal_data, void (*function)(void))
{
	// link user function
	wm_sample_event = function;

	// start state
	wm_action = t;

	// set id
	memcpy(wm_id, id, 6);

	// set calibration data
	memcpy(wm_cal_data, cal_data, 32);

	// initialize device detect pin
	cbi(dev_detect_port, dev_detect_pin);
	sbi(dev_detect_ddr, dev_detect_pin);
	_delay_ms(500); // delay to simulate disconnect

	// ready twi bus, no pull-ups
	cbi(twi_port, twi_scl_pin);
	cbi(twi_port, twi_sda_pin);

	// link twi slave events
	twi_attach_rx_event(wm_slaveRx);
	twi_attach_tx_start(wm_slaveTxStart);
	twi_attach_tx_end(wm_slaveTxEnd);

	// start twi slave
	twi_slave_init(0x52);

	// make the wiimote think something is connected
	sbi(dev_detect_port, dev_detect_pin);
}
