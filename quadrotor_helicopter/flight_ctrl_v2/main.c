#include "main.h"

void main_init()
{
	sbi(LED_port, LED1_pin);
	sbi(LED_port, LED2_pin);
	sbi(LED_ddr, LED1_pin);
	sbi(LED_ddr, LED2_pin);

	sei();

	ser_init();
	param_init();

	ppm_init();
	sens_init();
	esc_init();
	timer0_init();

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

	double roll_ang = 0; 
	double pitch_ang = 0;

	while(1)
	{
		tog(LED_port, LED2_pin);

		if(ppm_tx_is_good(3) == 2)
		{
			//sbi(LED_port, LED1_pin);
		}
		else
		{
			//cbi(LED_port, LED1_pin);
		}

		double roll_accel_val = (double)sens_read(roll_accel_chan) - sens_offset(roll_accel_chan);
		double pitch_accel_val = (double)sens_read(pitch_accel_chan) - sens_offset(pitch_accel_chan);
		double vert_accel_val = (double)sens_read(vert_accel_chan) - sens_offset(vert_accel_chan);

		double roll_gyro_val = sens_read(roll_gyro_chan) - sens_offset(roll_gyro_chan);
		double pitch_gyro_val = sens_read(pitch_gyro_chan) - sens_offset(pitch_gyro_chan);

		double roll_trig = 0;
		double pitch_trig = 0;

		double delta_time = timer1_elapsed();

		#ifdef use_atan
		roll_trig += calc_atan2(roll_accel_val, vert_accel_val);
		pitch_trig += calc_atan2(pitch_accel_val, vert_accel_val);
		#endif

		#ifdef use_asin
		roll_trig += calc_asin(roll_accel_val / one_G_val);
		pitch_trig += calc_asin(pitch_accel_val / one_G_val);
		#endif

		#ifdef use_comp_filter
		roll_ang = complementary_filter(&roll_ang, roll_trig, roll_gyro_val * param_get_d(0), param_get_d(0), delta_time);
		pitch_ang = complementary_filter(&pitch_ang, pitch_trig, pitch_gyro_val * param_get_d(0), param_get_d(0), delta_time);
		#endif

		#ifdef use_kalman_filter
		roll_kalman.Q[0] = param_get_d(0);
		pitch_kalman.Q[0] = roll_kalman.Q[0];
		roll_kalman.Q[1] = param_get_d(0);
		pitch_kalman.Q[1] = roll_kalman.Q[1];
		roll_kalman.R = param_get_d(0);
		pitch_kalman.R = roll_kalman.R;

		roll_ang = kalman_filter(&roll_kalman, roll_gyro_val * param_get_d(0), roll_trig, delta_time);
		pitch_ang = kalman_filter(&pitch_kalman, pitch_gyro_val * param_get_d(0), pitch_trig, delta_time);
		#endif

		if(esc_is_done())
		{
			PID_const pid_k;

			pid_k.p = param_get_d(0);
			pid_k.i = param_get_d(0);
			pid_k.d = param_get_d(0);

			double roll_cmd = (double)ppm_chan_width(roll_ppm_chan) * param_get_d(0);
			double roll_mot = PID_mv(&roll_pid, pid_k, roll_ang, roll_cmd);

			pid_k.p = param_get_d(0);
			pid_k.i = param_get_d(0);
			pid_k.d = param_get_d(0);

			double pitch_cmd = (double)ppm_chan_width(pitch_ppm_chan) * param_get_d(0);
			double pitch_mot = PID_mv(&pitch_pid, pid_k, pitch_ang, pitch_cmd);

			pid_k.p = param_get_d(0);
			pid_k.i = param_get_d(0);
			pid_k.d = param_get_d(0);

			double yaw_gyro_val = sens_read(yaw_gyro_chan) - sens_offset(yaw_gyro_chan);
			double yaw_cmd = (double)ppm_chan_width(yaw_ppm_chan) * param_get_d(0);
			double yaw_mot = PID_mv(&yaw_pid, pid_k, yaw_gyro_val, yaw_cmd);

			double throttle_cmd = (double)ppm_chan_width(throttle_ppm_chan) * param_get_d(0);

			double throttle_min = param_get_d(0);

			double f_mot = calc_map_double(throttle_cmd + yaw_cmd + pitch_cmd, -100, 100, param_get_d(0), param_get_d(0));
			double b_mot = calc_map_double(throttle_cmd + yaw_cmd - pitch_cmd, -100, 100, param_get_d(0), param_get_d(0));
			double l_mot = calc_map_double(throttle_cmd - yaw_cmd + roll_cmd, -100, 100, param_get_d(0), param_get_d(0));
			double r_mot = calc_map_double(throttle_cmd - yaw_cmd - roll_cmd, -100, 100, param_get_d(0), param_get_d(0));

			esc_set_width(f_mot_chan, lround(f_mot + throttle_min));
			esc_set_width(b_mot_chan, lround(b_mot + throttle_min));
			esc_set_width(l_mot_chan, lround(l_mot + throttle_min));
			esc_set_width(r_mot_chan, lround(r_mot + throttle_min));

			esc_is_done_clear();
		}
			
		}
	}

	while(1)
	{
		main_loop();
	}

	return 0;
}
