#ifndef config_h_inc
#define config_h_inc

#define DEBUG_MSG
#define BAUD 19200

#define ticks_500us ((F_CPU * 5) / 10000)
#define ticks_10ms (ticks_500us * 20)
#define loop_frame ((F_CPU / 1024) / 100)
#define process_time ((loop_frame * 3) / 4)

#define gyro_to_rad_per_sec 1
#define accel_gyro_w_ratio 0.2
#define frame_delta_time 0.01

#define ppm_highest_chan_default 6
#define esc_extra_chan_num_default 3

#define sens_hist_len_max 16
#define sens_hist_len_default 16

#endif
