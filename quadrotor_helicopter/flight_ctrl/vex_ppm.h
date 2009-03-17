static volatile unsigned char ppm_ovf_cnt;
//static volatile unsigned char timer1_ovf_cnt;

static volatile signed long chan_width_holder[8];
static volatile unsigned char highest_chan;


ISR(TIMER1_CAPT_vect)
{
	unsigned char ovf_cnt = ppm_ovf_cnt;
	ppm_ovf_cnt = 0;
	unsigned long t_icr = ICR1;

	// calculate total time using overflows and time difference
	signed long t = ((t_icr | 0x10000) - vex_data.last_capt) & 0xFFFF;
	if(t_icr < vex_data.last_capt)
	{
	        ovf_cnt--;
	}
	t += 0x10000 * ovf_cnt;

	vex_data.last_capt = t_icr;

	// check sync pulse
	if(t > width_500 * 6)
	{
		highest_chan = vex_data.chan_cnt;
		vex_data.chan_cnt = 0;
		if(vex_data.tx_good == 0)
		{
			vex_data.tx_good = 1;
		}
	}
	else // if pulse is shorter than 3ms, then it's a servo pulse
	{
		unsigned char index = vex_data.chan_cnt % 8;
		if(t >= width_500 && t <= width_500 * 5)
		{
			vex_data.chan_width[index] = t - (width_500 * 3) - vex_data.chan_offset[index]; // store time
			vex_data.chan_cnt++; // next channel
			if(vex_data.chan_cnt >= 4 && vex_data.tx_good != 0) // last channel, data is now good, reset to first pin
			{
				vex_data.tx_good = 2;
				if(vex_data.chan_cnt == highest_chan)
				{
					vex_data.new_flag = 1;
				}
			}
		}
		else
		{
			vex_data.tx_good = 0;
		}
	}
}

ISR(TIMER1_OVF_vect)
{
	ppm_ovf_cnt++;
	//timer1_ovf_cnt++;
	if(ppm_ovf_cnt > 8) // if too many, then transmitter is missing
	{
		vex_data.tx_good = 0;
		ppm_ovf_cnt = 8;
	}
}
