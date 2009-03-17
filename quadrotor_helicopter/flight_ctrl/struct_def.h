#ifndef struct_def_h
#define struct_def_h

typedef struct _ppm_data
{
	signed int chan_width[8];
	signed long chan_offset[8];
	unsigned char chan_cnt;
	unsigned long last_capt;
	unsigned char tx_good;
	unsigned char new_flag;
	unsigned long ovf_cnt;
	unsigned char yaw_ppm_chan;
	unsigned char roll_ppm_chan;
	unsigned char pitch_ppm_chan;
	unsigned char throttle_ppm_chan;
} ppm_data;

typedef struct sens_history_
{
	unsigned int sens_history_length;
	unsigned long res[sens_history_max_length];
	unsigned int cnt;
	unsigned long avg;
	signed long centered_avg;
	signed long centering_offset;
	#ifdef DEBUG
	signed long noise;
	#endif
} sens_history;

typedef struct PID_const_
{
	signed long kp;
	signed long ki;
	signed long kd;
	signed long err_low_thresh;
	signed long delta_err_low_thresh;
} PID_const;

typedef struct PID_data_
{
	signed long err_sum;
	signed long err_last;
	PID_const constants;
} PID_data;

typedef struct mot_cali_
{
	signed long f_mot_tweak;
	signed long b_mot_tweak;
	signed long l_mot_tweak;
	signed long r_mot_tweak;
	signed long f_mot_scale;
	signed long b_mot_scale;
	signed long l_mot_scale;
	signed long r_mot_scale;
} mot_cali;

typedef struct heli_action_
{
	signed long col;
	signed long yaw;
	signed long pitch;
	signed long roll;
} heli_action;

typedef struct mot_speed_
{
	signed long f;
	signed long b;
	signed long l;
	signed long r;
} mot_speed;

typedef struct servo_ctrl_
{
	unsigned long servo_ticks[8];
	unsigned char ready_to_restart;
	unsigned char chan;
	unsigned long servo_period_length;
} servo_ctrl;

typedef struct calibration_
{
	signed long f_mot_adj;
	signed long b_mot_adj;
	signed long l_mot_adj;
	signed long r_mot_adj;
	signed long f_mot_scale;
	signed long b_mot_scale;
	signed long l_mot_scale;
	signed long r_mot_scale;

	signed long yaw_sens_center_offset;
	signed long pitch_sens_center_offset;
	signed long roll_sens_center_offset;

	signed long fb_accel_center_offset;
	signed long lr_accel_center_offset;
	signed long ud_accel_center_offset;

	signed long yaw_sens_scale;

	signed long fb_lr_accel_scale;
	signed long ud_accel_scale;

	unsigned char sine_of_max_ang;

	signed long yaw_pid_kp;
	signed long yaw_pid_ki;
	signed long yaw_pid_kd;
	signed long yaw_pid_err_low_thresh;
	signed long yaw_pid_delta_err_low_thresh;

	signed long roll_pitch_level_pid_kp;
	signed long roll_pitch_level_pid_ki;
	signed long roll_pitch_level_pid_kd;
	signed long roll_pitch_level_pid_err_low_thresh;
	signed long roll_pitch_level_pid_delta_err_low_thresh;

	signed long roll_pitch_rate_pid_kp;
	signed long roll_pitch_rate_pid_ki;
	signed long roll_pitch_rate_pid_kd;
	signed long roll_pitch_rate_pid_err_low_thresh;
	signed long roll_pitch_rate_pid_delta_err_low_thresh;

	signed long servo_period_length;
	
	signed long throttle_cmd_scale;
	signed long yaw_cmd_scale;
	signed long move_cmd_scale;
	signed long hover_throttle;

	signed long ppm_chan_offset[8];

	unsigned char yaw_sens_hist_len;
	unsigned char roll_pitch_sens_hist_len;
	unsigned char vert_accel_hist_len;
	unsigned char hori_accel_hist_len;

	unsigned char yaw_ppm_chan;
	unsigned char roll_ppm_chan;
	unsigned char pitch_ppm_chan;
	unsigned char throttle_ppm_chan;

	unsigned char extra_servo_chan;
	unsigned char servo_ppm_link[3];
} calibration;
#endif
