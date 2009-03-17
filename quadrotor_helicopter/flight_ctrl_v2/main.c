#include "main.h"

void main_init()
{
	ser_init();
	ppm_init();
	sens_init();
	esc_init();	

	sei();
}

void main_loop()
{
}

int main()
{
	main_init();

	while(1)
	{
		signed long esc_ticks[8] = {ticks_500us,ticks_500us,ticks_500us,ticks_500us,ticks_500us,ticks_500us,ticks_500us,ticks_500us};
		if(esc_is_done())
		{
			unsigned long tsum = esc_ticks[0] + esc_ticks[1] + esc_ticks[2] + esc_ticks[3];
			for(unsigned char i = 4; i < 4 + main_cali.extra_servo_chan; i++)
			{
				tsum += esc_ticks[i];
			}

			timer_1_period_wait(tsum + process_time, servo_data.servo_period_length);

			process_time_stamp = TCNT0;

			esc_start_next();

			unsigned int tcnt0_ = TCNT0;
			process_time = (((tcnt0_ | 0x100) - process_time_stamp) & 0xFF) * 1024;
		}
	}

	while(1)
	{
		main_loop();
	}

	return 0;
}
