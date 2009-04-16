#ifndef calc_h_inc
#define calc_h_inc

#include <avr/io.h>
#include <avr/pgmspace.h>
#include <math.h>
#include "config.h"
#include "pindef.h"
#include "macros.h"

#define rad_to_deg_const 57.2957795130823

#define calc_abs(in) 				(in < 0 ? -1 * in : in)
#define calc_sign(in) 				(in < 0 ? -1 : 1)
#define calc_map_long(in,old_min,old_max,new_min,new_max) (new_min + calc_multi(in - old_min, new_max - new_min, old_max - old_min))
#define calc_max(a,b) 				(a > b ? a : b)
#define calc_min(a,b) 				(a < b ? a : b)
#define calc_ang_range(ang) 		(ang > (180 * MATH_MULTIPLIER) ? ((-180 * MATH_MULTIPLIER) + ang - (180 * MATH_MULTIPLIER)) : (ang < (-180 * MATH_MULTIPLIER) ? ((180 * MATH_MULTIPLIER) + ang - (-180 * MATH_MULTIPLIER)) : ang))
#define calc_constrain(in,min,max) 	(in < min ? min : (in > max ? max : in))
#define calc_multi(in,numer,denom) 	((in * numer) + (denom / 2)) / denom

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

//signed long calc_multi(signed long, signed long, signed long);
signed long PID_mv(PID_data *, PID_const, signed long, signed long);
PID_data PID_init();
signed long complementary_filter(signed long *, signed long, signed long, signed long, signed long);
double kalman_filter(kalman_data *, double, double, double);
signed long calc_atan2(signed long, signed long);
signed long calc_asin(signed long, signed long);
void kalman_init(kalman_data *, double, double, double);

#endif
