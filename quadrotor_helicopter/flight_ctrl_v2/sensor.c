#include "sensor.h"

static volatile sens_hist sens_res[8];
static volatile unsigned char sens_hist_len;
static volatile unsigned char adc_chan;
static volatile unsigned char sens_proc_busy;

ISR(ADC_vect)
{
	tog(LED_port, LED1_pin);
	adc_chan = ADMUX & 0b00000111;
	if(sens_proc_busy - 1 != adc_chan)
	{
		sens_res[adc_chan].res[sens_res[adc_chan].cnt % sens_hist_len] = ADC;
		sens_res[adc_chan].latest = ADC;
		sens_res[adc_chan].cnt++;
		adc_chan++;
	}
	adc_chan %= 6;
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
		sens_res[i].last_cnt = 0;
	}
	sens_hist_len = sens_hist_len_default;
	adc_chan = 0;
	sens_proc_busy = 0;

	adc_start(0, _BV(ADIE));
}

sens_hist sens_proc(unsigned char index, unsigned char calc_noise)
{
	sens_proc_busy = index + 1;
	unsigned long sum = 0;
	unsigned char cnt = 0;
	unsigned char scnt = sens_res[index].cnt;
	if(scnt == 0) goto end_label;
	for(unsigned char i = 0; i < scnt && i < sens_hist_len; i++)
	{
		sum += sens_res[index].res[i];
		cnt++;
	}
	sens_res[index].avg = (double)sum / (double)cnt;

	#ifdef enable_calc_noise
	signed long avg = lround(sens_res[index].avg);
	if(calc_noise != 0)
	{
		sum = 0;
		cnt = 0;
		for(unsigned char i = 0; i < scnt && i < sens_hist_len; i++)
		{
			sum += calc_abs((signed long)sens_res[index].res[i] - avg);
			cnt++;
		}
		sens_res[index].noise = (double)sum / (double)cnt;
	}
	#endif

	sens_res[index].last_cnt = sens_res[index].cnt;
	sens_res[index].cnt = 0;

	end_label:

	sens_proc_busy = 0;

	return sens_res[index];
}

unsigned int sens_read(unsigned char i)
{
	return sens_res[i].latest;
}

double sens_avg(unsigned char i)
{
	return sens_res[i].avg;
}

double sens_noise(unsigned char i)
{
	return sens_res[i].noise;
}

double sens_offset(unsigned char i)
{
	return sens_res[i].offset;
}

unsigned char sens_cnt(unsigned char i)
{
	return sens_res[i].last_cnt;
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
		sens_res[j].offset = (double)sum[j] / (double)t;
	}
	adc_start(0, _BV(ADIE));
}
