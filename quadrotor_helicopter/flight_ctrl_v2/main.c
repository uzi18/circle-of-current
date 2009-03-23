#include "main.h"

static volatile saved_params_s saved_params;

#include "ser_rx.h"

void main_init()
{
	sbi(LED_port, LED1_pin);
	sbi(LED_port, LED2_pin);
	sbi(LED_ddr, LED1_pin);
	sbi(LED_ddr, LED2_pin);

	sei();

	ser_init();
	param_init(&saved_params);

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

	PID_data roll_level_pid = PID_init();
	PID_data pitch_level_pid = PID_init();
	PID_data roll_rate_pid = PID_init();
	PID_data pitch_rate_pid = PID_init();
	PID_data yaw_pid = PID_init();

	#ifdef use_kalman_filter
	kalman_data roll_kalman;
	kalman_data pitch_kalman;
	#endif

	signed long zero_G_val;
	signed long one_G_val;

	signed long roll_ang = 0; 
	signed long pitch_ang = 0;

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

		zero_G_val = (saved_params.sl_s.vert_accel_top + saved_params.sl_s.vert_accel_bot + 1) / 2;
		one_G_val = sens_offset(vert_accel_chan) - zero_G_val;

		signed long roll_accel_val = ((signed long)sens_read(roll_accel_chan) - ((saved_params.sl_s.roll_accel_bot + saved_params.sl_s.roll_accel_top + 1) / 2));
		signed long pitch_accel_val = ((signed long)sens_read(pitch_accel_chan) - ((saved_params.sl_s.pitch_accel_bot + saved_params.sl_s.pitch_accel_top + 1) / 2));
		signed long vert_accel_val = ((signed long)sens_read(vert_accel_chan) - ((saved_params.sl_s.vert_accel_bot + saved_params.sl_s.vert_accel_top + 1) / 2));

		signed long roll_gyro_val = (signed long)sens_read(roll_gyro_chan) - saved_params.sl_s.roll_gyro_center;
		signed long pitch_gyro_val = (signed long)sens_read(pitch_gyro_chan) - saved_params.sl_s.pitch_gyro_center;

		signed long roll_trig;
		signed long pitch_trig;

		signed long delta_time = timer0_elapsed();

		#ifdef use_atan
		roll_trig = calc_atan2((double)roll_accel_val, (double)vert_accel_val);
		pitch_trig = calc_atan2((double)pitch_accel_val, (double)vert_accel_val);
		#endif

		#ifdef use_asin
		roll_trig = calc_asin((double)roll_accel_val, (double)one_G_val);
		pitch_trig = calc_asin((double)pitch_accel_val, (double)one_G_val);
		#endif

		roll_trig += saved_params.sl_s.roll_offset;
		pitch_trig += saved_params.sl_s.pitch_offset;

		roll_trig = calc_ang_range(roll_trig);
		pitch_trig = calc_ang_range(pitch_trig);

		#ifdef use_comp_filter
		roll_ang = complementary_filter(&roll_ang, roll_trig, calc_multi(roll_gyro_val, saved_params.sl_s.roll_gyro_to_rate, MATH_MULTIPLIER), saved_params.sl_s.comp_filter_w, delta_time);
		pitch_ang = complementary_filter(&pitch_ang, pitch_trig, calc_multi(pitch_gyro_val, saved_params.sl_s.pitch_gyro_to_rate, MATH_MULTIPLIER), saved_params.sl_s.comp_filter_w, delta_time);
		#endif

		#ifdef use_kalman_filter
		roll_ang = kalman_filter(&roll_kalman, calc_multi(roll_gyro_val, saved_params.sl_s.roll_gyro_to_rate, MATH_MULTIPLIER), roll_trig, delta_time);
		pitch_ang = kalman_filter(&pitch_kalman, calc_multi(pitch_gyro_val, saved_params.sl_s.pitch_gyro_to_rate, MATH_MULTIPLIER), pitch_trig, delta_time);
		#endif

		roll_ang = calc_ang_range(roll_ang);
		pitch_ang = calc_ang_range(pitch_ang);

		if(esc_is_done())
		{
			PID_const pid_k;

			pid_k.p = saved_params.sl_s.roll_level_kp;
			pid_k.i = saved_params.sl_s.roll_level_ki;
			pid_k.d = saved_params.sl_s.roll_level_kd;

			signed long roll_tgt_rate = PID_mv(&roll_level_pid, pid_k, calc_multi(roll_ang, saved_params.sl_s.roll_ppm_scale, MATH_MULTIPLIER), (signed long)ppm_chan_width((unsigned char)saved_params.sl_s.roll_ppm_chan));

			pid_k.p = saved_params.sl_s.pitch_level_kp;
			pid_k.i = saved_params.sl_s.pitch_level_ki;
			pid_k.d = saved_params.sl_s.pitch_level_kd;

			signed long pitch_tgt_rate = PID_mv(&pitch_level_pid, pid_k, calc_multi(pitch_ang, saved_params.sl_s.pitch_ppm_scale, MATH_MULTIPLIER), (signed long)ppm_chan_width((unsigned char)saved_params.sl_s.pitch_ppm_chan));

			pid_k.p = saved_params.sl_s.roll_rate_kp;
			pid_k.i = saved_params.sl_s.roll_rate_ki;
			pid_k.d = saved_params.sl_s.roll_rate_kd;

			signed long roll_mot = PID_mv(&roll_rate_pid, pid_k, roll_tgt_rate, roll_gyro_val);

			pid_k.p = saved_params.sl_s.pitch_rate_kp;
			pid_k.i = saved_params.sl_s.pitch_rate_ki;
			pid_k.d = saved_params.sl_s.pitch_rate_kd;

			signed long pitch_mot = PID_mv(&pitch_rate_pid, pid_k, pitch_tgt_rate, pitch_gyro_val);

			pid_k.p = saved_params.sl_s.yaw_kp;
			pid_k.i = saved_params.sl_s.yaw_ki;
			pid_k.d = saved_params.sl_s.yaw_kd;

			signed long yaw_gyro_val = sens_read(yaw_gyro_chan) - saved_params.sl_s.yaw_gyro_center;
			signed long yaw_mot = PID_mv(&yaw_pid, pid_k, calc_multi(yaw_gyro_val, saved_params.sl_s.yaw_scale, MATH_MULTIPLIER), ppm_chan_width((unsigned char)saved_params.sl_s.yaw_ppm_chan));

			signed long throttle_cmd = calc_multi((signed long)ppm_chan_width((unsigned char)saved_params.sl_s.throttle_ppm_chan), saved_params.sl_s.throttle_scale, MATH_MULTIPLIER);

			throttle_cmd += saved_params.sl_s.throttle_hover;

			signed long f_mot = saved_params.sl_s.f_mot_bot + calc_multi((signed long)(throttle_cmd + yaw_mot + pitch_mot), saved_params.sl_s.f_mot_scale, MATH_MULTIPLIER);
			signed long b_mot = saved_params.sl_s.b_mot_bot + calc_multi((signed long)(throttle_cmd + yaw_mot - pitch_mot), saved_params.sl_s.b_mot_scale, MATH_MULTIPLIER);
			signed long l_mot = saved_params.sl_s.l_mot_bot + calc_multi((signed long)(throttle_cmd - yaw_mot + roll_mot), saved_params.sl_s.l_mot_scale, MATH_MULTIPLIER);
			signed long r_mot = saved_params.sl_s.r_mot_bot + calc_multi((signed long)(throttle_cmd - yaw_mot - roll_mot), saved_params.sl_s.r_mot_scale, MATH_MULTIPLIER);

			esc_set_width(f_mot_chan, f_mot);
			esc_set_width(b_mot_chan, b_mot);
			esc_set_width(l_mot_chan, l_mot);
			esc_set_width(r_mot_chan, r_mot);

			//esc_set_width(4, ppm_chan_width(extra_ppm_chan_1) + (ticks_500us * 3));
			//esc_set_width(5, ppm_chan_width(extra_ppm_chan_2) + (ticks_500us * 3));
			//esc_set_width(6, ppm_chan_width(extra_ppm_chan_3) + (ticks_500us * 3));

			esc_start_next();
		}
			
	}

	while(1)
	{
		main_loop();
	}

	return 0;
}
