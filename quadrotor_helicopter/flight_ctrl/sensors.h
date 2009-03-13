static volatile unsigned char ADC_chan_cnt;

void sens_data_add(sens_history * h, unsigned int adc_res)
{
	h->res[h->cnt % h->sens_history_length] = adc_res;
	h->cnt++;
}

ISR(ADC_vect)
{
	sens_data_add(&sens_data[ADC_chan_cnt], ADC);
	ADC_chan_cnt++;
	ADC_chan_cnt %= 8;
	ADMUX = (ADMUX & 0b11100000) | ADC_chan_cnt;
	ADCSRA |= _BV(ADEN) | _BV(ADSC) | _BV(ADIE) | _BV(ADPS2) | _BV(ADPS1) | _BV(ADPS0);
}

void sens_data_calc_avg(sens_history * h)
{
	unsigned long sum = 0;
	unsigned char cnt = 0;
	for(unsigned char i = 0; i < h->cnt && i < h->sens_history_length; i++)
	{
		sum += h->res[i];
		cnt++;
	}
	if(h->cnt > h->sens_history_length)
	{
		for(unsigned char i = h->cnt - (h->sens_history_length / 2); i < h->cnt; i++)
		{
			sum += h->res[i % h->sens_history_length];
			cnt++;
		}
	}
	else
	{
		for(unsigned char i = h->cnt / 2; i < h->cnt; i++)
		{
			sum += h->res[i];
			cnt++;
		}
	}
	h->avg = scale(sum, 1, cnt);

	#ifdef DEBUG
	sum = 0;
	cnt = 0;
	for(unsigned char i = 0; i < h->cnt && i < h->sens_history_length; i++)
	{
		sum += abs(h->res[i] - h->avg) * 10;
		cnt++;
	}
	h->noise = scale(sum, 1, cnt);
	#endif

	h->centered_avg = h->avg - h->centering_offset;
	h->cnt = 0;
}

void sens_data_proc()
{
	ADCSRA &= 0xFF ^ _BV(ADIE);
	for(unsigned char i = 0; i < 8; i++)
	{
		sens_data_calc_avg(&sens_data[i]);
	}
	ADCSRA |= _BV(ADEN) | _BV(ADIE) | _BV(ADPS2) | _BV(ADPS1) | _BV(ADPS0);
}

void adc_init()
{
	ADC_chan_cnt = 0;
	ADMUX &= 0xFF ^ (_BV(REFS1) | _BV(REFS0) | _BV(ADLAR));
	ADCSRA |= _BV(ADEN) | _BV(ADSC) | _BV(ADIE) | _BV(ADPS2) | _BV(ADPS1) | _BV(ADPS0);
}
