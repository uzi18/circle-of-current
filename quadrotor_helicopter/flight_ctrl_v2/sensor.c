#include "sensor.h"

static volatile sens_hist sens_res[8];
static volatile unsigned char sens_hist_len;
static volatile unsigned char adc_chan;

ISR(ADC_vect)
{
	adc_chan = ADMUX & 0b00000111;
	sens_res[adc_chan].res[sens_res[adc_chan].cnt % sens_hist_len] = ADC;
	sens_res[adc_chan].cnt++;
	adc_chan++;
	adc_chan %= 8;
	ADMUX = (ADMUX & 0b11100000) | adc_chan;
	ADCSRA = _BV(ADEN) | _BV(ADSC) | _BV(ADIE) | _BV(ADPS2) | _BV(ADPS1) | _BV(ADPS0);
}

void adc_start(unsigned char c, unsigned char f)
{
	ADMUX = (ADMUX & 0b11100000) | c;
	ADCSRA = _BV(ADEN) | _BV(ADSC) | _BV(ADPS2) | _BV(ADPS1) | _BV(ADPS0) | f;
}

void adc_wait_stop()
{
	cbi(ADCSRA, ADIE);
	while(bit_is_set(ADCSRA, ADSC));
}

void sens_init()
{
	for(unsigned int i = 0; i < 8; i++)
	{
		sens_res[i].offset = 0;
		sens_res[i].cnt = 0;
	}
	sens_hist_len = sens_hist_len_default;
	adc_chan = 0;

	adc_start(0, _BV(ADIE));
}

signed int sens_read(unsigned char index, unsigned char calc_noise)
{
	unsigned long sum = 0;
	unsigned char cnt = 0;
	for(unsigned char i = 0; i < sens_res[index].cnt && i < sens_hist_len; i++)
	{
		sum += sens_res[index].res[i];
		cnt++;
	}
	for(unsigned char i = sens_res[index].cnt + sens_hist_len, j = 0; j < sens_hist_len / 2 && j < sens_res[index].cnt; j++, i--)
	{
		sum += sens_res[index].res[i % sens_hist_len];
		cnt++;
	}
	sens_res[index].avg = calc_multi(sum, 1, cnt);

	if(calc_noise != 0)
	{
		sum = 0;
		cnt = 0;
		for(unsigned char i = 0; i < sens_res[index].cnt && i < sens_hist_len; i++)
		{
			sum += calc_abs((signed long)sens_res[index].res[i] - (signed long)sens_res[index].avg);
			cnt++;
		}
		sens_res[index].noise = calc_multi(sum, noise_multiplier, cnt);
	}

	if(sens_res[index].cnt > sens_hist_len / 2)
	{
		sens_res[index].cnt = 0;
	}

	signed int r = sens_res[index].avg;
	r -= sens_res[index].offset;

	return r;
}

unsigned int sens_noise(unsigned char i)
{
	return sens_res[i].noise;
}

void sens_calibrate(unsigned char t)
{
	unsigned long sum[8] = {0,0,0,0,0,0,0,0};

	adc_wait_stop();
	for(unsigned char i = 0; i < t; i++)
	{
		for(unsigned char j = 0; j < 8; j++)
		{
			adc_start(j, 0);
			while(bit_is_set(ADCSRA, ADSC));
			sum[j] += ADC;
		}
	}
	for(unsigned char j = 0; j < 8; j++)
	{
		sens_res[j].offset = calc_multi(sum[j], 1, t);
	}
	adc_start(0, _BV(ADIE));
}
