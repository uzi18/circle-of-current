#include "main.h"

void main_init()
{
	sbi(LED_port, LED1_pin);
	sbi(LED_port, LED2_pin);
	sbi(LED_ddr, LED1_pin);
	sbi(LED_ddr, LED2_pin);

	sei();

	timer0_init();
	ser_init();
	ppm_init();
	sens_init();
	esc_init();

	sens_calibrate(10);
	tog(LED_port, LED2_pin);

	ppm_calibrate(10);
	tog(LED_port, LED1_pin);

	esc_safe(0);
}

void main_loop()
{
}

int main()
{
	unsigned long process_time = 0;

	main_init();

	while(1)
	{
		if(esc_is_done())
		{
			timer1_period_wait(esc_get_total() + process_time, ticks_500us * 25);

			unsigned int process_time_stamp = TCNT0;

			if(ppm_tx_is_good(3) == 2)
			{
				sbi(LED_port, LED1_pin);
			}
			else
			{
				cbi(LED_port, LED1_pin);
			}

			for(unsigned char i = 0; i < 8; i++)
			{
				esc_set_width(i, ppm_chan_width(i) + ticks_500us * 3);
			}

			if(ser_tx_is_busy() == 0)
			{
				for(unsigned char i = 8, k = 16, j = 0; j < 8; i++, j++, k++)
				{
					debug_tx(i, sens_read(j, 1));
					debug_tx(k, sens_noise(j));
				}

				for(unsigned char i = 0, j = 0; j < 8; i++, j++)
				{
					debug_tx(i, ppm_chan_width(j));
				}

				debug_tx(24, esc_get_total());
				debug_tx(25, process_time);
			}
			
			esc_start_next();

			unsigned int tcnt0_ = TCNT0;
			process_time = (unsigned long)(((tcnt0_ | 0x100) - process_time_stamp) & 0xFF) * 1024;
		}
	}

	while(1)
	{
		main_loop();
	}

	return 0;
}
