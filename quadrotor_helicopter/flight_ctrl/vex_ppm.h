void timer_1_input_capture(ppm_data * pd)
{
	unsigned long t_icr = ICR1; // convert to unsigned long

	// calculate total time using overflows and time difference
	signed long t = ((t_icr | 0x10000) - pd->last_capt) & 0xFFFF;
	pd->last_capt = t_icr;

	// check sync pulse
	if(pd->ovf_cnt >= 2)
	{
		pd->chan_cnt = 0;
	}
	else // if pulse is shorter than 3ms, then it's a servo pulse
	{
		pd->chan_width[pd->chan_cnt] = t - (width_500 * 3) - pd->chan_offset[pd->chan_cnt]; // store time
		pd->chan_cnt++; // next channel
		if(pd->chan_cnt == 6) // last channel, data is now good, reset to first pin
		{
			pd->tx_good = 1;
		}
	}
}

void timer_1_ovf(ppm_data * pd)
{
	pd->ovf_cnt++;
	if(pd->ovf_cnt > 7) // if too many, then transmitter is missing
	{
		pd->tx_good = 0;
	}
	timer1_ovf_cnt++;
}
