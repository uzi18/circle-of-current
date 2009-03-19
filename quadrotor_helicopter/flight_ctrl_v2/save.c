#define COMING_FROM_SAVE_C
#include "save.h"
#undef COMING_FROM_SAVE_C

double param_get_d(unsigned char addr)
{
	return saved_params.d_val[addr];
}

unsigned long param_get_ul(unsigned char addr)
{
	return saved_params.ul_val[addr];
}

signed long param_get_sl(unsigned char addr)
{
	return saved_params.sl_val[addr];
}

void param_set_d(unsigned char addr, double d)
{
	saved_params.d_val[addr] = d;
}

void param_set_ul(unsigned char addr, unsigned long d)
{
	saved_params.ul_val[addr] = d;
}

void param_set_sl(unsigned char addr, signed long d)
{
	saved_params.sl_val[addr] = d;
}

void param_save(unsigned char addr)
{
	for(unsigned int i = addr * 4; i < (addr * 4) + 4; i++)
	{
		unsigned char c = eeprom_read_byte(i);
		unsigned char d = saved_params.c[i];
		if(c != d)
		{
			eeprom_write_byte(i, d);
		}
	}
}

void param_load(unsigned char addr)
{
	for(unsigned int i = addr * 4; i < (addr * 4) + 4; i++)
	{
		unsigned char c = eeprom_read_byte(i);
		saved_params.c[i] = c;
	}
}

void param_save_all()
{
	for(unsigned int i = 0; i < sizeof(saved_params_s); i++)
	{
		unsigned char c = eeprom_read_byte(i);
		unsigned char d = saved_params.c[i];
		if(c != d)
		{
			eeprom_write_byte(i, d);
		}
	}
}

void param_load_all()
{
	for(unsigned int i = 0; i < sizeof(saved_params_s); i++)
	{
		unsigned char c = eeprom_read_byte(i);
		saved_params.c[i] = c;
	}
}

void param_init()
{
	params_set_default();
}
