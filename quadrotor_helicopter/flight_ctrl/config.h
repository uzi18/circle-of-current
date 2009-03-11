#ifndef config_h
#define config_h

#define width_500 (F_CPU * 5) / 10000
#if (width_500 != 10000)
#error "weird, width_500 is not = 10000"
#endif
#define sens_history_max_length 15
#define capt_history_length 16
#define PID_const_multiplier 100
#define mot_scale_multiplier 100
#define yaw_scale_multiplier 100
#define roll_pitch_scale_multiplier 100
#define fb_lr_accel_scale_multiplier 100
#define throttle_scale_multiplier 100
#define spin_scale_multiplier 100
#define motor_scale_multiplier 100
#define move_scale_multiplier 100

#define f_mot_chan 1
#define b_mot_chan 2
#define l_mot_chan 3
#define r_mot_chan 4
#define aux_servo_chan 5

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

#define yaw_center_offset_default 0
#define pitch_center_offset_default 0
#define roll_center_offset_default 0

#define yaw_scale_default 100
#define roll_pitch_scale_default 100

#define fb_accel_center_offset_default 100
#define lr_accel_center_offset_default 100
#define ud_accel_center_offset_default 100

#define fb_lr_accel_scale_default 100
#define ud_accel_scale_default 100

#define ppm_chan_offset_default 0

#define yaw_pid_kp_default 100
#define yaw_pid_ki_default 100
#define yaw_pid_kd_default 100
#define yaw_pid_err_low_thresh_default 0
#define yaw_pid_delta_err_low_thresh_default 0

#define roll_pitch_a_pid_kp_default 100
#define roll_pitch_a_pid_ki_default 100
#define roll_pitch_a_pid_kd_default 100
#define roll_pitch_a_pid_err_low_thresh_default 0
#define roll_pitch_a_pid_delta_err_low_thresh_default 0

#define roll_pitch_b_pid_kp_default 100
#define roll_pitch_b_pid_ki_default 100
#define roll_pitch_b_pid_kd_default 100
#define roll_pitch_b_pid_err_low_thresh_default 0
#define roll_pitch_b_pid_delta_err_low_thresh_default 0

#define servo_pulse_scale_default 100
#define servo_pulse_stop_default width_500
#define servo_period_delay_default (width_500 * 20)

#define throttle_scale_default 100
#define spin_scale_default 100
#define move_scale_default 100

#define yaw_sens_hist_len_default 15
#define roll_pitch_sens_hist_len_default 15
#define vert_accel_hist_len_default 15
#define hori_accel_hist_len_default 15

#define f_mot_adj_addr (0 + 4 * 0)
#define f_mot_scale_addr (f_mot_adj_addr + 4 * 4)
#define yaw_center_offset_addr (f_mot_scale_addr + 4 * 4)
#define fb_accel_center_offset_addr (yaw_center_offset_addr + 4 * 3)
#define fb_lr_accel_scale_addr (fb_accel_center_offset_addr + 4 * 2)
#define yaw_scale_addr (fb_lr_accel_scale_addr + 4 * 3)
#define ppm_chan_offset_addr (yaw_scale_addr + 4 * 2)
#define yaw_pid_kp_addr (ppm_chan_offset_addr + 4 * 8)
#define roll_pitch_a_pid_kp_addr (yaw_pid_kp_addr + 4 * 5)
#define roll_pitch_b_pid_kp_addr (roll_pitch_a_pid_kp_addr + 4 * 5)
#define servo_pulse_scale_addr (roll_pitch_b_pid_kp_addr + 4 * 5)
#define throttle_scale_addr (servo_pulse_scale_addr + 4 * 2)
#define yaw_sens_hist_len_addr (throttle_scale_addr + 4 * 3)
#define hover_throttle_addr (yaw_sens_hist_len_addr + 1 * 4)
#define yaw_ppm_chan_addr (hover_throttle_addr + 4 * 1)

#endif 
