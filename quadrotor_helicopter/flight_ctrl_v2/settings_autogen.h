#ifndef settings_autogen_h_inc
#define settings_autogen_h_inc

// define default values

#define f_mot_bot_def_val (F_CPU / 1000)
#define b_mot_bot_def_val (F_CPU / 1000)
#define l_mot_bot_def_val (F_CPU / 1000)
#define r_mot_bot_def_val (F_CPU / 1000)
#define f_mot_scale_def_val 100
#define b_mot_scale_def_val 100
#define l_mot_scale_def_val 100
#define r_mot_scale_def_val 100
#define roll_gyro_center_def_val (1024 / 2)
#define pitch_gyro_center_def_val (1024 / 2)
#define yaw_gyro_center_def_val (1024 / 2)
#define yaw_scale_def_val 1
#define roll_accel_bot_def_val 0
#define pitch_accel_bot_def_val 0
#define vert_accel_bot_def_val 0
#define roll_accel_top_def_val 1023
#define pitch_accel_top_def_val 1023
#define vert_accel_top_def_val 1023
#define roll_offset_def_val 0
#define pitch_offset_def_val 0
#define roll_ppm_scale_def_val 1
#define pitch_ppm_scale_def_val 1
#define roll_gyro_to_rate_def_val 1
#define pitch_gyro_to_rate_def_val 1
#define roll_level_kp_def_val 1
#define roll_level_ki_def_val 1
#define roll_level_kd_def_val 1
#define pitch_level_kp_def_val 1
#define pitch_level_ki_def_val 1
#define pitch_level_kd_def_val 1
#define roll_rate_kp_def_val 1
#define roll_rate_ki_def_val 1
#define roll_rate_kd_def_val 1
#define pitch_rate_kp_def_val 1
#define pitch_rate_ki_def_val 1
#define pitch_rate_kd_def_val 1
#define yaw_kp_def_val 1
#define yaw_ki_def_val 1
#define yaw_kd_def_val 1
#define comp_filter_w_def_val 0.5
#define kalman_q_ang_def_val 0.01
#define kalman_q_gyro_def_val 0.01
#define kalman_r_ang_def_val 0.01
#define throttle_hover_def_val ((F_CPU * 3) / 4000)
#define throttle_scale_def_val 100
#define period_ticks_def_val (F_CPU / 102400)
#define when_to_update_esc_def_val ((period_ticks_def_val * 3) / 5)
#define roll_ppm_chan_def_val 0
#define pitch_ppm_chan_def_val 1
#define yaw_ppm_chan_def_val 3
#define throttle_ppm_chan_def_val 2
#define params_updated_def_val 1

// define addresses

#define f_mot_bot_addr 0
#define b_mot_bot_addr 1
#define l_mot_bot_addr 2
#define r_mot_bot_addr 3
#define f_mot_scale_addr 4
#define b_mot_scale_addr 5
#define l_mot_scale_addr 6
#define r_mot_scale_addr 7
#define roll_gyro_center_addr 8
#define pitch_gyro_center_addr 9
#define yaw_gyro_center_addr 10
#define yaw_scale_addr 11
#define roll_accel_bot_addr 12
#define pitch_accel_bot_addr 13
#define vert_accel_bot_addr 14
#define roll_accel_top_addr 15
#define pitch_accel_top_addr 16
#define vert_accel_top_addr 17
#define roll_offset_addr 18
#define pitch_offset_addr 19
#define roll_ppm_scale_addr 20
#define pitch_ppm_scale_addr 21
#define roll_gyro_to_rate_addr 22
#define pitch_gyro_to_rate_addr 23
#define roll_level_kp_addr 24
#define roll_level_ki_addr 25
#define roll_level_kd_addr 26
#define pitch_level_kp_addr 27
#define pitch_level_ki_addr 28
#define pitch_level_kd_addr 29
#define roll_rate_kp_addr 30
#define roll_rate_ki_addr 31
#define roll_rate_kd_addr 32
#define pitch_rate_kp_addr 33
#define pitch_rate_ki_addr 34
#define pitch_rate_kd_addr 35
#define yaw_kp_addr 36
#define yaw_ki_addr 37
#define yaw_kd_addr 38
#define comp_filter_w_addr 39
#define kalman_q_ang_addr 40
#define kalman_q_gyro_addr 41
#define kalman_r_ang_addr 42
#define throttle_hover_addr 43
#define throttle_scale_addr 44
#define period_ticks_addr 45
#define when_to_update_esc_addr 46
#define roll_ppm_chan_addr 47
#define pitch_ppm_chan_addr 48
#define yaw_ppm_chan_addr 49
#define throttle_ppm_chan_addr 50
#define params_updated_addr 51


// variable getting

/*
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

signed long f_mot_bot_;
signed long b_mot_bot_;
signed long l_mot_bot_;
signed long r_mot_bot_;
signed long f_mot_scale_;
signed long b_mot_scale_;
signed long l_mot_scale_;
signed long r_mot_scale_;
signed long roll_gyro_center_;
signed long pitch_gyro_center_;
signed long yaw_gyro_center_;
signed long yaw_scale_;
signed long roll_accel_bot_;
signed long pitch_accel_bot_;
signed long vert_accel_bot_;
signed long roll_accel_top_;
signed long pitch_accel_top_;
signed long vert_accel_top_;
signed long roll_offset_;
signed long pitch_offset_;
signed long roll_ppm_scale_;
signed long pitch_ppm_scale_;
signed long roll_gyro_to_rate_;
signed long pitch_gyro_to_rate_;
signed long roll_level_kp_;
signed long roll_level_ki_;
signed long roll_level_kd_;
signed long pitch_level_kp_;
signed long pitch_level_ki_;
signed long pitch_level_kd_;
signed long roll_rate_kp_;
signed long roll_rate_ki_;
signed long roll_rate_kd_;
signed long pitch_rate_kp_;
signed long pitch_rate_ki_;
signed long pitch_rate_kd_;
signed long yaw_kp_;
signed long yaw_ki_;
signed long yaw_kd_;
signed long comp_filter_w_;
signed long kalman_q_ang_;
signed long kalman_q_gyro_;
signed long kalman_r_ang_;
signed long throttle_hover_;
signed long throttle_scale_;
unsigned char period_ticks_;
unsigned char when_to_update_esc_;
unsigned char roll_ppm_chan_;
unsigned char pitch_ppm_chan_;
unsigned char yaw_ppm_chan_;
unsigned char throttle_ppm_chan_;
unsigned char params_updated_;

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

// */

// make union

typedef union saved_params_s_ {
	struct {
		double f_mot_bot;
		double b_mot_bot;
		double l_mot_bot;
		double r_mot_bot;
		double f_mot_scale;
		double b_mot_scale;
		double l_mot_scale;
		double r_mot_scale;
		double roll_gyro_center;
		double pitch_gyro_center;
		double yaw_gyro_center;
		double yaw_scale;
		double roll_accel_bot;
		double pitch_accel_bot;
		double vert_accel_bot;
		double roll_accel_top;
		double pitch_accel_top;
		double vert_accel_top;
		double roll_offset;
		double pitch_offset;
		double roll_ppm_scale;
		double pitch_ppm_scale;
		double roll_gyro_to_rate;
		double pitch_gyro_to_rate;
		double roll_level_kp;
		double roll_level_ki;
		double roll_level_kd;
		double pitch_level_kp;
		double pitch_level_ki;
		double pitch_level_kd;
		double roll_rate_kp;
		double roll_rate_ki;
		double roll_rate_kd;
		double pitch_rate_kp;
		double pitch_rate_ki;
		double pitch_rate_kd;
		double yaw_kp;
		double yaw_ki;
		double yaw_kd;
		double comp_filter_w;
		double kalman_q_ang;
		double kalman_q_gyro;
		double kalman_r_ang;
		double throttle_hover;
		double throttle_scale;
		double period_ticks;
		double when_to_update_esc;
		double roll_ppm_chan;
		double pitch_ppm_chan;
		double yaw_ppm_chan;
		double throttle_ppm_chan;
		double params_updated;
	} d_s;

	struct {
		signed long f_mot_bot;
		signed long b_mot_bot;
		signed long l_mot_bot;
		signed long r_mot_bot;
		signed long f_mot_scale;
		signed long b_mot_scale;
		signed long l_mot_scale;
		signed long r_mot_scale;
		signed long roll_gyro_center;
		signed long pitch_gyro_center;
		signed long yaw_gyro_center;
		signed long yaw_scale;
		signed long roll_accel_bot;
		signed long pitch_accel_bot;
		signed long vert_accel_bot;
		signed long roll_accel_top;
		signed long pitch_accel_top;
		signed long vert_accel_top;
		signed long roll_offset;
		signed long pitch_offset;
		signed long roll_ppm_scale;
		signed long pitch_ppm_scale;
		signed long roll_gyro_to_rate;
		signed long pitch_gyro_to_rate;
		signed long roll_level_kp;
		signed long roll_level_ki;
		signed long roll_level_kd;
		signed long pitch_level_kp;
		signed long pitch_level_ki;
		signed long pitch_level_kd;
		signed long roll_rate_kp;
		signed long roll_rate_ki;
		signed long roll_rate_kd;
		signed long pitch_rate_kp;
		signed long pitch_rate_ki;
		signed long pitch_rate_kd;
		signed long yaw_kp;
		signed long yaw_ki;
		signed long yaw_kd;
		signed long comp_filter_w;
		signed long kalman_q_ang;
		signed long kalman_q_gyro;
		signed long kalman_r_ang;
		signed long throttle_hover;
		signed long throttle_scale;
		signed long period_ticks;
		signed long when_to_update_esc;
		signed long roll_ppm_chan;
		signed long pitch_ppm_chan;
		signed long yaw_ppm_chan;
		signed long throttle_ppm_chan;
		signed long params_updated;
	} sl_s;

	struct {
		unsigned long f_mot_bot;
		unsigned long b_mot_bot;
		unsigned long l_mot_bot;
		unsigned long r_mot_bot;
		unsigned long f_mot_scale;
		unsigned long b_mot_scale;
		unsigned long l_mot_scale;
		unsigned long r_mot_scale;
		unsigned long roll_gyro_center;
		unsigned long pitch_gyro_center;
		unsigned long yaw_gyro_center;
		unsigned long yaw_scale;
		unsigned long roll_accel_bot;
		unsigned long pitch_accel_bot;
		unsigned long vert_accel_bot;
		unsigned long roll_accel_top;
		unsigned long pitch_accel_top;
		unsigned long vert_accel_top;
		unsigned long roll_offset;
		unsigned long pitch_offset;
		unsigned long roll_ppm_scale;
		unsigned long pitch_ppm_scale;
		unsigned long roll_gyro_to_rate;
		unsigned long pitch_gyro_to_rate;
		unsigned long roll_level_kp;
		unsigned long roll_level_ki;
		unsigned long roll_level_kd;
		unsigned long pitch_level_kp;
		unsigned long pitch_level_ki;
		unsigned long pitch_level_kd;
		unsigned long roll_rate_kp;
		unsigned long roll_rate_ki;
		unsigned long roll_rate_kd;
		unsigned long pitch_rate_kp;
		unsigned long pitch_rate_ki;
		unsigned long pitch_rate_kd;
		unsigned long yaw_kp;
		unsigned long yaw_ki;
		unsigned long yaw_kd;
		unsigned long comp_filter_w;
		unsigned long kalman_q_ang;
		unsigned long kalman_q_gyro;
		unsigned long kalman_r_ang;
		unsigned long throttle_hover;
		unsigned long throttle_scale;
		unsigned long period_ticks;
		unsigned long when_to_update_esc;
		unsigned long roll_ppm_chan;
		unsigned long pitch_ppm_chan;
		unsigned long yaw_ppm_chan;
		unsigned long throttle_ppm_chan;
		unsigned long params_updated;
	} ul_s;

	double d_val[52];
	signed long sl_val[52];
	unsigned long ul_val[52];
	unsigned char c[208];
} saved_params_s;

void params_set_default();

#ifdef COMING_FROM_SAVE_C
#undef COMING_FROM_SAVE_C

static volatile saved_params_s saved_params;

// set default

void params_set_default()
{
	saved_params.sl_s.f_mot_bot = f_mot_bot_def_val;
	saved_params.sl_s.b_mot_bot = b_mot_bot_def_val;
	saved_params.sl_s.l_mot_bot = l_mot_bot_def_val;
	saved_params.sl_s.r_mot_bot = r_mot_bot_def_val;
	saved_params.sl_s.f_mot_scale = f_mot_scale_def_val;
	saved_params.sl_s.b_mot_scale = b_mot_scale_def_val;
	saved_params.sl_s.l_mot_scale = l_mot_scale_def_val;
	saved_params.sl_s.r_mot_scale = r_mot_scale_def_val;
	saved_params.sl_s.roll_gyro_center = roll_gyro_center_def_val;
	saved_params.sl_s.pitch_gyro_center = pitch_gyro_center_def_val;
	saved_params.sl_s.yaw_gyro_center = yaw_gyro_center_def_val;
	saved_params.sl_s.yaw_scale = yaw_scale_def_val;
	saved_params.sl_s.roll_accel_bot = roll_accel_bot_def_val;
	saved_params.sl_s.pitch_accel_bot = pitch_accel_bot_def_val;
	saved_params.sl_s.vert_accel_bot = vert_accel_bot_def_val;
	saved_params.sl_s.roll_accel_top = roll_accel_top_def_val;
	saved_params.sl_s.pitch_accel_top = pitch_accel_top_def_val;
	saved_params.sl_s.vert_accel_top = vert_accel_top_def_val;
	saved_params.sl_s.roll_offset = roll_offset_def_val;
	saved_params.sl_s.pitch_offset = pitch_offset_def_val;
	saved_params.sl_s.roll_ppm_scale = roll_ppm_scale_def_val;
	saved_params.sl_s.pitch_ppm_scale = pitch_ppm_scale_def_val;
	saved_params.sl_s.roll_gyro_to_rate = roll_gyro_to_rate_def_val;
	saved_params.sl_s.pitch_gyro_to_rate = pitch_gyro_to_rate_def_val;
	saved_params.sl_s.roll_level_kp = roll_level_kp_def_val;
	saved_params.sl_s.roll_level_ki = roll_level_ki_def_val;
	saved_params.sl_s.roll_level_kd = roll_level_kd_def_val;
	saved_params.sl_s.pitch_level_kp = pitch_level_kp_def_val;
	saved_params.sl_s.pitch_level_ki = pitch_level_ki_def_val;
	saved_params.sl_s.pitch_level_kd = pitch_level_kd_def_val;
	saved_params.sl_s.roll_rate_kp = roll_rate_kp_def_val;
	saved_params.sl_s.roll_rate_ki = roll_rate_ki_def_val;
	saved_params.sl_s.roll_rate_kd = roll_rate_kd_def_val;
	saved_params.sl_s.pitch_rate_kp = pitch_rate_kp_def_val;
	saved_params.sl_s.pitch_rate_ki = pitch_rate_ki_def_val;
	saved_params.sl_s.pitch_rate_kd = pitch_rate_kd_def_val;
	saved_params.sl_s.yaw_kp = yaw_kp_def_val;
	saved_params.sl_s.yaw_ki = yaw_ki_def_val;
	saved_params.sl_s.yaw_kd = yaw_kd_def_val;
	saved_params.sl_s.comp_filter_w = comp_filter_w_def_val;
	saved_params.sl_s.kalman_q_ang = kalman_q_ang_def_val;
	saved_params.sl_s.kalman_q_gyro = kalman_q_gyro_def_val;
	saved_params.sl_s.kalman_r_ang = kalman_r_ang_def_val;
	saved_params.sl_s.throttle_hover = throttle_hover_def_val;
	saved_params.sl_s.throttle_scale = throttle_scale_def_val;
	saved_params.ul_s.period_ticks = period_ticks_def_val;
	saved_params.ul_s.when_to_update_esc = when_to_update_esc_def_val;
	saved_params.ul_s.roll_ppm_chan = roll_ppm_chan_def_val;
	saved_params.ul_s.pitch_ppm_chan = pitch_ppm_chan_def_val;
	saved_params.ul_s.yaw_ppm_chan = yaw_ppm_chan_def_val;
	saved_params.ul_s.throttle_ppm_chan = throttle_ppm_chan_def_val;
	saved_params.ul_s.params_updated = params_updated_def_val;
}

#endif



#endif

