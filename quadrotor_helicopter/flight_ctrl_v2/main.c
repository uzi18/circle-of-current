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

	signed long f_mot_bot_ = param_get_sl(f_mot_bot_addr);
	signed long b_mot_bot_ = param_get_sl(b_mot_bot_addr);
	signed long l_mot_bot_ = param_get_sl(l_mot_bot_addr);
	signed long r_mot_bot_ = param_get_sl(r_mot_bot_addr);
	signed long f_mot_scale_ = param_get_sl(f_mot_scale_addr);
	signed long b_mot_scale_ = param_get_sl(b_mot_scale_addr);
	signed long l_mot_scale_ = param_get_sl(l_mot_scale_addr);
	signed long r_mot_scale_ = param_get_sl(r_mot_scale_addr);
	signed long roll_gyro_center_ = param_get_sl(roll_gyro_center_addr);
	signed long pitch_gyro_center_ = param_get_sl(pitch_gyro_center_addr);
	signed long yaw_gyro_center_ = param_get_sl(yaw_gyro_center_addr);
	signed long yaw_scale_ = param_get_sl(yaw_scale_addr);
	signed long roll_accel_bot_ = param_get_sl(roll_accel_bot_addr);
	signed long pitch_accel_bot_ = param_get_sl(pitch_accel_bot_addr);
	signed long vert_accel_bot_ = param_get_sl(vert_accel_bot_addr);
	signed long roll_accel_top_ = param_get_sl(roll_accel_top_addr);
	signed long pitch_accel_top_ = param_get_sl(pitch_accel_top_addr);
	signed long vert_accel_top_ = param_get_sl(vert_accel_top_addr);
	signed long roll_offset_ = param_get_sl(roll_offset_addr);
	signed long pitch_offset_ = param_get_sl(pitch_offset_addr);
	signed long roll_ppm_scale_ = param_get_sl(roll_ppm_scale_addr);
	signed long pitch_ppm_scale_ = param_get_sl(pitch_ppm_scale_addr);
	signed long roll_gyro_to_rate_ = param_get_sl(roll_gyro_to_rate_addr);
	signed long pitch_gyro_to_rate_ = param_get_sl(pitch_gyro_to_rate_addr);
	signed long roll_level_kp_ = param_get_sl(roll_level_kp_addr);
	signed long roll_level_ki_ = param_get_sl(roll_level_ki_addr);
	signed long roll_level_kd_ = param_get_sl(roll_level_kd_addr);
	signed long pitch_level_kp_ = param_get_sl(pitch_level_kp_addr);
	signed long pitch_level_ki_ = param_get_sl(pitch_level_ki_addr);
	signed long pitch_level_kd_ = param_get_sl(pitch_level_kd_addr);
	signed long roll_rate_kp_ = param_get_sl(roll_rate_kp_addr);
	signed long roll_rate_ki_ = param_get_sl(roll_rate_ki_addr);
	signed long roll_rate_kd_ = param_get_sl(roll_rate_kd_addr);
	signed long pitch_rate_kp_ = param_get_sl(pitch_rate_kp_addr);
	signed long pitch_rate_ki_ = param_get_sl(pitch_rate_ki_addr);
	signed long pitch_rate_kd_ = param_get_sl(pitch_rate_kd_addr);
	signed long yaw_kp_ = param_get_sl(yaw_kp_addr);
	signed long yaw_ki_ = param_get_sl(yaw_ki_addr);
	signed long yaw_kd_ = param_get_sl(yaw_kd_addr);
	signed long comp_filter_w_ = param_get_sl(comp_filter_w_addr);
	signed long kalman_q_ang_ = param_get_sl(kalman_q_ang_addr);
	signed long kalman_q_gyro_ = param_get_sl(kalman_q_gyro_addr);
	signed long kalman_r_ang_ = param_get_sl(kalman_r_ang_addr);
	signed long throttle_hover_ = param_get_sl(throttle_hover_addr);
	signed long throttle_scale_ = param_get_sl(throttle_scale_addr);
	unsigned char period_ticks_ = param_get_ul(period_ticks_addr);
	unsigned char when_to_update_esc_ = param_get_ul(when_to_update_esc_addr);
	unsigned char roll_ppm_chan_ = param_get_ul(roll_ppm_chan_addr);
	unsigned char pitch_ppm_chan_ = param_get_ul(pitch_ppm_chan_addr);
	unsigned char yaw_ppm_chan_ = param_get_ul(yaw_ppm_chan_addr);
	unsigned char throttle_ppm_chan_ = param_get_ul(throttle_ppm_chan_addr);
	unsigned char params_updated_ = param_get_ul(params_updated_addr);

	PID_data roll_level_pid = PID_init();
	PID_data pitch_level_pid = PID_init();
	PID_data roll_rate_pid = PID_init();
	PID_data pitch_rate_pid = PID_init();
	PID_data yaw_pid = PID_init();

	#ifdef use_kalman_filter
	kalman_data roll_kalman;
	kalman_data pitch_kalman;
	kalman_init(&roll_kalman, kalman_q_ang_, kalman_q_gyro_, kalman_r_ang_);
	kalman_init(&pitch_kalman, kalman_q_ang_, kalman_q_gyro_, kalman_r_ang_);
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

		if(param_get_ul(params_updated_addr) != 0)
		{
			f_mot_bot_ = param_get_sl(f_mot_bot_addr);
			b_mot_bot_ = param_get_sl(b_mot_bot_addr);
			l_mot_bot_ = param_get_sl(l_mot_bot_addr);
			r_mot_bot_ = param_get_sl(r_mot_bot_addr);
			f_mot_scale_ = param_get_sl(f_mot_scale_addr);
			b_mot_scale_ = param_get_sl(b_mot_scale_addr);
			l_mot_scale_ = param_get_sl(l_mot_scale_addr);
			r_mot_scale_ = param_get_sl(r_mot_scale_addr);
			roll_gyro_center_ = param_get_sl(roll_gyro_center_addr);
			pitch_gyro_center_ = param_get_sl(pitch_gyro_center_addr);
			yaw_gyro_center_ = param_get_sl(yaw_gyro_center_addr);
			yaw_scale_ = param_get_sl(yaw_scale_addr);
			roll_accel_bot_ = param_get_sl(roll_accel_bot_addr);
			pitch_accel_bot_ = param_get_sl(pitch_accel_bot_addr);
			vert_accel_bot_ = param_get_sl(vert_accel_bot_addr);
			roll_accel_top_ = param_get_sl(roll_accel_top_addr);
			pitch_accel_top_ = param_get_sl(pitch_accel_top_addr);
			vert_accel_top_ = param_get_sl(vert_accel_top_addr);
			roll_offset_ = param_get_sl(roll_offset_addr);
			pitch_offset_ = param_get_sl(pitch_offset_addr);
			roll_ppm_scale_ = param_get_sl(roll_ppm_scale_addr);
			pitch_ppm_scale_ = param_get_sl(pitch_ppm_scale_addr);
			roll_gyro_to_rate_ = param_get_sl(roll_gyro_to_rate_addr);
			pitch_gyro_to_rate_ = param_get_sl(pitch_gyro_to_rate_addr);
			roll_level_kp_ = param_get_sl(roll_level_kp_addr);
			roll_level_ki_ = param_get_sl(roll_level_ki_addr);
			roll_level_kd_ = param_get_sl(roll_level_kd_addr);
			pitch_level_kp_ = param_get_sl(pitch_level_kp_addr);
			pitch_level_ki_ = param_get_sl(pitch_level_ki_addr);
			pitch_level_kd_ = param_get_sl(pitch_level_kd_addr);
			roll_rate_kp_ = param_get_sl(roll_rate_kp_addr);
			roll_rate_ki_ = param_get_sl(roll_rate_ki_addr);
			roll_rate_kd_ = param_get_sl(roll_rate_kd_addr);
			pitch_rate_kp_ = param_get_sl(pitch_rate_kp_addr);
			pitch_rate_ki_ = param_get_sl(pitch_rate_ki_addr);
			pitch_rate_kd_ = param_get_sl(pitch_rate_kd_addr);
			yaw_kp_ = param_get_sl(yaw_kp_addr);
			yaw_ki_ = param_get_sl(yaw_ki_addr);
			yaw_kd_ = param_get_sl(yaw_kd_addr);
			comp_filter_w_ = param_get_sl(comp_filter_w_addr);
			kalman_q_ang_ = param_get_sl(kalman_q_ang_addr);
			kalman_q_gyro_ = param_get_sl(kalman_q_gyro_addr);
			kalman_r_ang_ = param_get_sl(kalman_r_ang_addr);
			throttle_hover_ = param_get_sl(throttle_hover_addr);
			throttle_scale_ = param_get_sl(throttle_scale_addr);
			period_ticks_ = param_get_ul(period_ticks_addr);
			when_to_update_esc_ = param_get_ul(when_to_update_esc_addr);
			roll_ppm_chan_ = param_get_ul(roll_ppm_chan_addr);
			pitch_ppm_chan_ = param_get_ul(pitch_ppm_chan_addr);
			yaw_ppm_chan_ = param_get_ul(yaw_ppm_chan_addr);
			throttle_ppm_chan_ = param_get_ul(throttle_ppm_chan_addr);
			params_updated_ = param_get_ul(params_updated_addr);

			zero_G_val = (vert_accel_top_ + vert_accel_bot_) / 2;
			one_G_val = sens_offset(vert_accel_chan) - zero_G_val;

			timer0_set_loop(period_ticks_);
			timer0_set_start_time(when_to_update_esc_);

			#ifdef use_kalman_filter
			kalman_init(&roll_kalman, kalman_q_ang_, kalman_q_gyro_, kalman_r_ang_);
			kalman_init(&pitch_kalman, kalman_q_ang_, kalman_q_gyro_, kalman_r_ang_);
			#endif

			param_set_ul(params_updated_addr, 0);
		}

		signed long roll_accel_val = ((signed long)sens_read(roll_accel_chan) - ((roll_accel_bot_ + roll_accel_top_) / 2));
		signed long pitch_accel_val = ((signed long)sens_read(pitch_accel_chan) - ((pitch_accel_bot_ + pitch_accel_top_) / 2));
		signed long vert_accel_val = ((signed long)sens_read(vert_accel_chan) - ((vert_accel_bot_ + vert_accel_top_) / 2));

		signed long roll_gyro_val = (signed long)sens_read(roll_gyro_chan) - roll_gyro_center_;
		signed long pitch_gyro_val = (signed long)sens_read(pitch_gyro_chan) - pitch_gyro_center_;

		signed long roll_trig = 0;
		signed long pitch_trig = 0;

		signed long delta_time = timer0_elapsed();

		#ifdef use_atan
		roll_trig = calc_atan2((double)roll_accel_val, (double)vert_accel_val);
		pitch_trig = calc_atan2((double)pitch_accel_val, (double)vert_accel_val);
		#endif

		#ifdef use_asin
		roll_trig = calc_asin((double)roll_accel_val, (double)one_G_val);
		pitch_trig = calc_asin((double)pitch_accel_val, (double)one_G_val);
		#endif

		roll_trig += roll_offset_;
		pitch_trig += pitch_offset_;

		roll_trig = calc_ang_range(roll_trig);
		pitch_trig = calc_ang_range(pitch_trig);

		#ifdef use_comp_filter
		roll_ang = complementary_filter(&roll_ang, roll_trig, calc_multi(roll_gyro_val, roll_gyro_to_rate_, MATH_MULTIPLIER), comp_filter_w_, delta_time);
		pitch_ang = complementary_filter(&pitch_ang, pitch_trig, calc_multi(pitch_gyro_val, pitch_gyro_to_rate_, MATH_MULTIPLIER), comp_filter_w_, delta_time);
		#endif

		#ifdef use_kalman_filter
		roll_ang = kalman_filter(&roll_kalman, roll_gyro_val * roll_gyro_to_rate_, roll_trig, delta_time);
		pitch_ang = kalman_filter(&pitch_kalman, pitch_gyro_val * pitch_gyro_to_rate_, pitch_trig, delta_time);
		#endif

		roll_ang = calc_ang_range(roll_ang);
		pitch_ang = calc_ang_range(pitch_ang);

		if(start_proc(1))
		{
			PID_const pid_k;

			pid_k.p = roll_level_kp_;
			pid_k.i = roll_level_ki_;
			pid_k.d = roll_level_kd_;

			signed long roll_tgt_rate = PID_mv(&roll_level_pid, pid_k, calc_multi(roll_ang, roll_ppm_scale_, MATH_MULTIPLIER), (signed long)ppm_chan_width(roll_ppm_chan_));

			pid_k.p = pitch_level_kp_;
			pid_k.i = pitch_level_ki_;
			pid_k.d = pitch_level_kd_;

			signed long pitch_tgt_rate = PID_mv(&pitch_level_pid, pid_k, calc_multi(pitch_ang, pitch_ppm_scale_, MATH_MULTIPLIER), (signed long)ppm_chan_width(pitch_ppm_chan_));

			pid_k.p = roll_rate_kp_;
			pid_k.i = roll_rate_ki_;
			pid_k.d = roll_rate_kd_;

			signed long roll_mot = PID_mv(&roll_rate_pid, pid_k, roll_tgt_rate, roll_gyro_val);

			pid_k.p = pitch_rate_kp_;
			pid_k.i = pitch_rate_ki_;
			pid_k.d = pitch_rate_kd_;

			signed long pitch_mot = PID_mv(&pitch_rate_pid, pid_k, pitch_tgt_rate, pitch_gyro_val);

			pid_k.p = yaw_kp_;
			pid_k.i = yaw_ki_;
			pid_k.d = yaw_kd_;

			signed long yaw_gyro_val = sens_read(yaw_gyro_chan) - yaw_gyro_center_;
			signed long yaw_mot = PID_mv(&yaw_pid, pid_k, calc_multi(yaw_gyro_val, yaw_scale_, MATH_MULTIPLIER), ppm_chan_width(yaw_ppm_chan_));

			signed long throttle_cmd = calc_multi((signed long)ppm_chan_width(throttle_ppm_chan_), throttle_scale_, MATH_MULTIPLIER);

			throttle_cmd += throttle_hover_;

			signed int f_mot = f_mot_bot_ + calc_multi((signed long)(throttle_cmd + yaw_mot + pitch_mot), f_mot_scale_, MATH_MULTIPLIER);
			signed int b_mot = b_mot_bot_ + calc_multi((signed long)(throttle_cmd + yaw_mot - pitch_mot), b_mot_scale_, MATH_MULTIPLIER);
			signed int l_mot = l_mot_bot_ + calc_multi((signed long)(throttle_cmd - yaw_mot + roll_mot), l_mot_scale_, MATH_MULTIPLIER);
			signed int r_mot = r_mot_bot_ + calc_multi((signed long)(throttle_cmd - yaw_mot - roll_mot), r_mot_scale_, MATH_MULTIPLIER);

			esc_set_width(f_mot_chan, f_mot);
			esc_set_width(b_mot_chan, b_mot);
			esc_set_width(l_mot_chan, l_mot);
			esc_set_width(r_mot_chan, r_mot);

			start_proc(0);

			tog(LED_port, LED1_pin);
		}
			
	}

	while(1)
	{
		main_loop();
	}

	return 0;
}
