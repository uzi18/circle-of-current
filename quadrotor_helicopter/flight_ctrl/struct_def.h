#ifndef struct_def_h
#define struct_def_h

typedef struct _ppm_data
{
	signed long chan_width[6];
	signed long chan_offset[6];
	unsigned char chan_cnt;
	unsigned long last_capt;
	unsigned char tx_good;
	unsigned long ovf_cnt;
} ppm_data;

typedef struct sens_history_
{
	unsigned int sens_history_length;
	unsigned long res[sens_history_max_length];
	unsigned int cnt;
	unsigned long avg;
	signed long centered_avg;
	signed long centering_offset;
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
	signed long servo_pulse_scale;
	signed long servo_stop;
	signed long scale_multiplier;
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
	unsigned int servo_ticks[6];
	unsigned char safe_to_update_servo_array;
	unsigned char servo_new_period_started;
} servo_ctrl;

#endif
