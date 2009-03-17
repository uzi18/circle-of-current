#ifndef config_h
#define config_h

#define width_500 (F_CPU * 5) / 10000
#if (width_500 != 10000)
#error "weird, width_500 is not = 10000"
#endif
#define sens_history_max_length 15
#define PID_const_multiplier 100
#define mot_scale_multiplier 100
#define yaw_sens_scale_multiplier 100
#define roll_pitch_sens_scale_multiplier 100
#define fb_lr_accel_scale_multiplier 100
#define throttle_cmd_scale_multiplier 100
#define yaw_cmd_scale_multiplier 100
#define motor_scale_multiplier 100
#define move_cmd_scale_multiplier 100

#define sine_of_max_ang_default 38
#define sine_multiplyer 100

#define extra_servo_chan_default 1
#define servo_ppm_link_0_default 4
#define servo_ppm_link_1_default 5
#define servo_ppm_link_2_default 6

#define roll_ppm_chan_default 0
#define pitch_ppm_chan_default 1
#define yaw_ppm_chan_default 2
#define throttle_ppm_chan_default 3

#define f_mot_adj_default 0
#define b_mot_adj_default 0
#define l_mot_adj_default 0
#define r_mot_adj_default 0

#define f_mot_scale_default 100
#define b_mot_scale_default 100
#define l_mot_scale_default 100
#define r_mot_scale_default 100

#define hover_throttle_default 0

#define yaw_sens_center_offset_default 0
#define pitch_sens_center_offset_default 0
#define roll_sens_center_offset_default 0

#define yaw_sens_max_reading (1024 / 2)
#define yaw_sens_scale_default ((width_500 * yaw_sens_scale_multiplier) / yaw_sens_max_reading)

#define fb_accel_center_offset_default 100
#define lr_accel_center_offset_default 100
#define ud_accel_center_offset_default 100

#define fb_lr_accel_max_reading (1024 / 2)
#define fb_lr_accel_scale_default ((width_500 * fb_lr_accel_scale_multiplier) / fb_lr_accel_max_reading)
#define ud_accel_scale_default fb_lr_accel_scale_default

#define ppm_chan_offset_default 0

#define yaw_pid_kp_default 100
#define yaw_pid_ki_default 100
#define yaw_pid_kd_default 100
#define yaw_pid_err_low_thresh_default 0
#define yaw_pid_delta_err_low_thresh_default 0

#define roll_pitch_level_pid_kp_default 100
#define roll_pitch_level_pid_ki_default 100
#define roll_pitch_level_pid_kd_default 100
#define roll_pitch_level_pid_err_low_thresh_default 0
#define roll_pitch_level_pid_delta_err_low_thresh_default 0

#define roll_pitch_rate_pid_kp_default 100
#define roll_pitch_rate_pid_ki_default 100
#define roll_pitch_rate_pid_kd_default 100
#define roll_pitch_rate_pid_err_low_thresh_default 0
#define roll_pitch_rate_pid_delta_err_low_thresh_default 0

#define servo_pulse_scale_default 100
#define servo_pulse_stop_default width_500
#define servo_period_length_default (width_500 * 20)

#define throttle_cmd_scale_default 100
#define yaw_cmd_scale_default 100
#define move_cmd_scale_default 100

#define yaw_sens_hist_len_default 15
#define roll_pitch_sens_hist_len_default 15
#define vert_accel_hist_len_default 15
#define hori_accel_hist_len_default 15

#endif 
