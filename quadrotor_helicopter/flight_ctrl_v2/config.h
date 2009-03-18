#ifndef config_h_inc
#define config_h_inc

#define DEBUG_MSG
#define BAUD 19200

#define noise_multiplier 10
#define PID_const_multiplier 100

#define ticks_500us ((F_CPU * 5) / 10000)

#define ppm_highest_chan_default 6
#define esc_extra_chan_num_default 3

#define sens_hist_len_max 16
#define sens_hist_len_default 16

#endif
