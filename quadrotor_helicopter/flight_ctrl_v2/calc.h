#ifndef calc_h_inc
#define calc_h_inc

#include "config.h"

typedef struct PID_const_
{
	signed long kp;
	signed long ki;
	signed long kd;
	signed long err_low_thresh;
	signed long delta_err_low_thresh;
	signed long err_dec;
} PID_const;

typedef struct PID_data_
{
	signed long err_sum;
	signed long err_last;
	PID_const constants;
} PID_data;

typedef struct kalman_data_
{
	signed long x[2];
	signed long P[4];
	signed long Q[2];
	signed long R;
	signed long dt;
} kalman_data;

signed long calc_multi(signed long, signed long, signed long);
signed long calc_abs(signed long);
signed long calc_constrain(signed long, signed long, signed long);
signed long calc_max(signed long, signed long);
signed long calc_min(signed long, signed long);
signed long calc_map(signed long, signed long, signed long, signed long, signed long);

#endif
