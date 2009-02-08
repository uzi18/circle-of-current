#include "wiimote.h"
#include "wm_crypto.h"

static volatile unsigned char wm_id[6];
static volatile unsigned char wm_cal_data[16];

static volatile unsigned char wm_rand[10];
static volatile unsigned char wm_key[6];

static volatile unsigned char wm_ft[8];
static volatile unsigned char wm_sb[8];

static volatile unsigned char wm_packet[16];
static volatile unsigned char wm_packet_len;

// read_cnt is a psuedo timer
static volatile unsigned int wm_read_cnt;

static volatile wm_cd_s wm_action;

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

void wm_slaveTx()
{
	// data requested, so load into buffer
	twi_transmit(wm_packet, wm_packet_len);
}

void wm_transmit(unsigned char * d, unsigned char l, unsigned char enc)
{
	if(enc != 0)
	{
		for(unsigned char i = 0; i < l; i++)
		{
			// encrypt
			wm_packet[i] = (d[i] - wm_ft[i % 8]) ^ wm_sb[i % 8];
		}
	}
	else
	{
		memcpy(wm_packet, d, 8); // not encrypted
	}

	// set length
	wm_packet_len = l;
}

void wm_slaveRx(unsigned char * b, int l)
{
	switch(b[0])
	{
		case 0xFA: // wants to read ID
			goto load_id;
		case 0x00: // taking a sample
			goto load_data;
		case 0x40: // receiving key
		case 0x46:
		case 0x4C:
			goto load_key_data;
		case 0x20: // wants calibration
		case 0x30:
			goto load_cali_data;
		default:
			return;
	}

	load_cali_data:
	{
		// wiimote expects calibration data
		wm_transmit(wm_cal_data, 16, 1); // send them encrypted
		return;
	}

	load_key_data:
	{
		// wiimote is sending rand and key
		if(b[0] == 0x40)
		{
			wm_rand[9] = b[1];
			wm_rand[8] = b[2];
			wm_rand[7] = b[3];
			wm_rand[6] = b[4];
			wm_rand[5] = b[5];
			wm_rand[4] = b[6];
		}
		else if(b[0] == 0x46)
		{
			wm_rand[3] = b[1];
			wm_rand[2] = b[2];
			wm_rand[1] = b[3];
			wm_rand[0] = b[4];
			wm_key[5] = b[5];
			wm_key[4] = b[6];
		}
		else if(b[0] == 0x4C)
		{
			wm_key[3] = b[1];
			wm_key[2] = b[2];
			wm_key[1] = b[3];
			wm_key[0] = b[4];

			// wiimote is finished sending rand and key
			// generate decryption method now
			wm_gentabs();
		}
		return;
	}

	load_data:
	{
		// load button data into buffer encrypted
		wm_transmit(wm_action.d, 6, 1);
		wm_read_cnt++; // increment counter
		return;
	}
	
	load_id:
	{
		// send unencrypted identifier
		wm_transmit(wm_id, 6, 0);
		return;
	}
}

// read_cnt is a psuedo timer
void wm_read_cnt_set(unsigned long d)
{
	wm_read_cnt = d;
}

unsigned long wm_read_cnt_read()
{
	return wm_read_cnt;
}

void wm_newaction(wm_cd_s t)
{
	// load button data from user application
	wm_action = t;
}

void wm_init(unsigned char * id, wm_cd_s t, unsigned char * cal_data)
{
	// start state
	wm_action = t;

	// set id
	memcpy(wm_id, id, 6);

	// set calibration data
	memcpy(wm_cal_data, cal_data, 16);

	// initialize device detect pin
	cbi(dev_detect_port, dev_detect_pin);
	sbi(dev_detect_ddr, dev_detect_pin);
	_delay_ms(500); // delay to simulate disconnect

	// ready twi bus, no pull-ups
	cbi(twi_port, twi_scl_pin);
	cbi(twi_port, twi_sda_pin);

	// link twi slave events
	twi_attachRxEvent(wm_slaveRx);
	twi_attachTxEvent(wm_slaveTx);

	// initialize counter
	wm_read_cnt = 0;

	// start twi slave
	twi_slave_init(0x52);

	// make the wiimote think something is connected
	sbi(dev_detect_port, dev_detect_pin);
}
