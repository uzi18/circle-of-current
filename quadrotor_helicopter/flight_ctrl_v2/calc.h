#ifndef calc_h_inc
#define calc_h_inc

#include <avr/io.h>
#include <avr/pgmspace.h>
#include <math.h>
#include "config.h"
#include "pindef.h"
#include "macros.h"

#define rad_to_deg_const 57.2957795130823

typedef struct PID_const_
{
	double kp;
	double ki;
	double kd;
	double err_low_thresh;
	double delta_err_low_thresh;
	double err_dec;
} PID_const;

typedef struct PID_data_
{
	double err_sum;
	double err_last;
	PID_const constants;
} PID_data;

typedef struct kalman_data_
{
	double x[2];
	double P[4];
	double Q[2];
	double R;
} kalman_data;

signed long calc_multi(signed long, signed long, signed long);
signed long calc_abs(signed long);
signed long calc_constrain_long(signed long, signed long, signed long);
double calc_constrain_double(double, double, double);
signed long calc_max(signed long, signed long);
signed long calc_min(signed long, signed long);
signed long calc_map_long(signed long, signed long, signed long, signed long, signed long);
double calc_map_double(double, double, double, double, double);
double PID_mv(PID_data *, double, double);
double complementary_filter(double *, double, double, double, double);
double kalman_filter(kalman_data *, double, double, double);
double calc_atan2(double, double);
double calc_asin(double);
double calc_pow(double, signed char);
double calc_rad_to_deg(double);
double calc_deg_to_rad(double);
void kalman_init(kalman_data *, double, double, double);

#endif
