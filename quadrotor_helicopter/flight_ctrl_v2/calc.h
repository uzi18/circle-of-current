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
	signed long p;
	signed long i;
	signed long d;
} PID_const;

typedef struct PID_data_
{
	signed long err_sum;
	signed long err_last;
} PID_data;

typedef struct kalman_data_
{
	signed long x[2];
	signed long P[4];
	signed long Q[2];
	signed long R;
} kalman_data;

signed long calc_multi(signed long, signed long, signed long);
signed long calc_abs(signed long);
signed long calc_constrain_long(signed long, signed long, signed long);
signed long calc_ang_range(signed long);
double calc_constrain_double(double, double, double);
signed long calc_max(signed long, signed long);
signed long calc_min(signed long, signed long);
signed long calc_map_long(signed long, signed long, signed long, signed long, signed long);
double calc_map_double(double, double, double, double, double);
signed long PID_mv(PID_data *, PID_const, signed long, signed long);
PID_data PID_init();
signed long complementary_filter(signed long *, signed long, signed long, signed long, signed long);
double kalman_filter(kalman_data *, double, double, double);
signed long calc_atan2(signed long, signed long);
signed long calc_asin(signed long, signed long);
double calc_pow(double, signed char);
void kalman_init(kalman_data *, double, double, double);

#endif
