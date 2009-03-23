static volatile unsigned char ser_rx_addr;
static volatile double ser_rx_data;
static volatile signed char ser_rx_data_sign;
static volatile signed char ser_rx_data_dot;
static volatile unsigned char ser_rx_state;

ISR(USART0_RX_vect)
{
	unsigned long c = UDR0;
	if(c == '@')
	{
		ser_rx_addr = 0;
		ser_rx_state = 0;
	}
	else if(c == '=')
	{
		ser_rx_data = 0;
		ser_rx_data_sign = 1;
		ser_rx_data_dot = 0;
		ser_rx_state = 1;
	}
	else if((c == 'D' || c == 'L') && ser_rx_state == 1)
	{
		for(signed char i = 0; i < ser_rx_data_dot - 1; i++)
		{
			ser_rx_data /= 10;
		}
		
		if(c == 'D')
		{
			param_set_d(&saved_params, ser_rx_addr % 128, ser_rx_data * (double)ser_rx_data_sign);
		}
		else
		{
			param_set_sl(&saved_params, ser_rx_addr % 128, lround(ser_rx_data) * ser_rx_data_sign);
		}
	}
	else if(c == 'R')
	{
		param_save(&saved_params, ser_rx_addr);
	}
	else if(c == 'W')
	{
		param_load(&saved_params, ser_rx_addr);
	}
	else if(c >= '0' && c <= '9')
	{
		if(ser_rx_state == 0)
		{
			ser_rx_addr *= 10;
			ser_rx_addr += c - '0';
		}
		else
		{
			ser_rx_data *= 10;
			ser_rx_data += (double)(c - '0');
			if(ser_rx_data_dot > 0)
			{
				ser_rx_data_dot++;
			}
		}
	}
	else if(c == '.')
	{
		ser_rx_data_dot = 1;
	}
	else if(c == '-')
	{
		ser_rx_data_sign = -1;
	}
}
