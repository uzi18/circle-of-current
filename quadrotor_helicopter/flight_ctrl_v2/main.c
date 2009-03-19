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
}

void main_loop()
{
}

int main()
{
	main_init();

	esc_safe(0);

	PID_data roll_pid;
	PID_data pitch_pid;
	PID_data yaw_pid;

	kalman_data roll_kalman;
	kalman_data pitch_kalman;

	double zero_G_val = (sens_offset(roll_accel_chan) + sens_offset(pitch_accel_chan)) / 2;
	double one_G_val = sens_offset(vert_accel_chan) - zero_G_val;

	double roll_ang_kf = 0; 
	double pitch_ang_kf = 0;

	double roll_ang_cf = 0;
	double pitch_ang_cf = 0;

	while(1)
	{
		if(start_proc())
		{
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
				sens_read(i, 1);
			}

			double roll_accel_val = sens_avg(roll_accel_chan) - sens_offset(roll_accel_chan);
			double pitch_accel_val = sens_avg(pitch_accel_chan) - sens_offset(pitch_accel_chan);
			double vert_accel_val = sens_avg(vert_accel_chan) - sens_offset(vert_accel_chan);

			double roll_atan = calc_atan2(roll_accel_val, vert_accel_val);
			double pitch_atan = calc_atan2(pitch_accel_val, vert_accel_val);

			double roll_asin = calc_asin(roll_accel_val / one_G_val);
			double pitch_asin = calc_asin(pitch_accel_val / one_G_val);

			double roll_gyro_val = sens_avg(roll_gyro_chan) - sens_offset(roll_gyro_chan);
			double pitch_gyro_val = sens_avg(pitch_gyro_chan) - sens_offset(pitch_gyro_chan);
			double yaw_gyro_val = sens_avg(yaw_gyro_chan) - sens_offset(yaw_gyro_chan);

			roll_ang_cf = complementary_filter(&roll_ang_cf, roll_atan, roll_gyro_val * gyro_to_rad_per_sec, accel_gyro_w_ratio, frame_delta_time);
			pitch_ang_cf = complementary_filter(&pitch_ang_cf, pitch_atan, pitch_gyro_val * gyro_to_rad_per_sec, accel_gyro_w_ratio, frame_delta_time);

			roll_ang_kf = kalman_filter(&roll_kalman, roll_gyro_val * gyro_to_rad_per_sec, roll_atan, frame_delta_time);
			pitch_ang_kf = kalman_filter(&pitch_kalman, pitch_gyro_val * gyro_to_rad_per_sec, pitch_atan, frame_delta_time);

			double roll_cmd = ppm_chan_width(0);

			for(unsigned char i = 0; i < 8; i++)
			{
				esc_set_width(i, ppm_chan_width(i) + ticks_500us * 3);
			}

			if(ser_tx_is_busy() == 0)
			{
				for(unsigned char i = 8, k = 16, h = 24, j = 0; j < 8; i++, j++, k++, h++)
				{
					sens_hist t_sh = sens_read(j, 1);
					debug_tx(i, PSTR("sensor avg    "), t_sh.avg);
					debug_tx(h, PSTR("sensor offset "), t_sh.offset);
					debug_tx(k, PSTR("sensor noise  "), t_sh.noise);
				}

				for(unsigned char i = 0, j = 0; j < 8; i++, j++)
				{
					debug_tx(i, PSTR("ppm pulse     "), ppm_chan_width(j));
				}
			}			
		}
	}

	while(1)
	{
		main_loop();
	}

	return 0;
}
