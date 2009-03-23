#include "sensor.h"

static volatile unsigned int adc_res[8];
static volatile unsigned int adc_offset[8];
static volatile unsigned char adc_chan;

ISR(ADC_vect)
{
	tog(LED_port, LED1_pin);
	adc_res[adc_chan] = ADC;
	adc_chan++;
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
		adc_offset[i] = 0;
	}

	adc_chan = 0;

	adc_start(0, _BV(ADIE));
}

unsigned int sens_read(unsigned char i)
{
	while(adc_chan == i);
	return adc_res[i];
}

unsigned int sens_offset(unsigned char i)
{
	return adc_offset[i];
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
		adc_offset[j] = lround((double)sum[j] / (double)t);
	}
	adc_start(0, _BV(ADIE));
}
