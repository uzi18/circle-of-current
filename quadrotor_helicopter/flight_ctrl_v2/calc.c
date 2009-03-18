#include "calc.h"

signed long calc_multi(signed long in, signed long numer, signed long denom)
{
	if(denom == 0)
	{
		if(in * numer > 0)
		{
			return 0x7FFFFFFF;
		}
		else if(in * numer < 0)
		{
			return 0x80000000;
		}
		else
		{
			return 0;
		}
	}
	else if(denom == 1 || denom == -1)
	{
		return in * numer * denom;
	}

	signed long t_ = in * numer;

	if(t_ == 0)
	{
		return 0;
	}

	signed long t = t_ / denom;
	signed long _t = t * denom;
	signed long diff = t_ - _t;

	signed long one;
	if(denom < 0 && numer >= 0)
	{
		one = -1;
	}
	else
	{
		one = 1;
	}

	if(bit_is_set(denom, 0))
	{
		if(diff >= denom / 2)
		{
			t += one;
		}
		else if(-diff >= denom / 2)
		{
			t -= one;
		}
	}
	else
	{
		if(diff > denom / 2)
		{
			t += one;
		}
		else if(-diff > denom / 2)
		{
			t -= one;
		}
	}
	return t;
}

signed long calc_abs(signed long in)
{
	if(in < 0)
	{
		return in * -1;
	}
	else
	{
		return in;
	}
}

signed long calc_map_long(signed long in, signed long old_min, signed long old_max, signed long new_min, signed long new_max)
{
	return new_min + calc_multi(in - old_min, new_max - new_min, old_max - old_min);
}

double calc_map_double(double in, double old_min, double old_max, double new_min, double new_max)
{
	return new_min + (((new_max - new_min) * (in - old_min)) / (old_max - old_min));
}

signed long calc_max(signed long a, signed long b)
{
	if(a > b)
	{
		return a;
	}
	else
	{
		return b;
	}
}

signed long calc_min(signed long a, signed long b)
{
	if(a < b)
	{
		return a;
	}
	else
	{
		return b;
	}
}

signed long calc_constrain_long(signed long in, signed long min_, signed long max_)
{
	return calc_min(calc_max(in, min_), max_);
}

double calc_constrain_double(double in, double min_, double max_)
{
	return fmin(fmax(in, min_), max_);
}

double complementary_filter(double * ang, double accel_h, double accel_v, double gyro_r, double w)
{
	*ang = (1 - w) * (*ang + gyro_r) + (w * atan2(accel_h, accel_v));
	return *ang;
}

double PID_mv(PID_data * pid, double current, double target)
{
	double err = target - current;

	pid->err_sum = calc_constrain_double(pid->err_sum + err, -1000000, 1000000);

	double delta_err = err - pid->err_last;

	if(fabs(pid->err_sum) < pid->constants.err_low_thresh && fabs(delta_err) < pid->constants.delta_err_low_thresh)
	{
		pid->err_sum = 0;
	}
	double mv = err * pid->constants.kp + pid->err_sum * pid->constants.ki + delta_err * pid->constants.kd;

	pid->err_sum *= pid->constants.err_dec;

	pid->err_last = err;

	return mv;
}

double kalman_filter(kalman_data * kd, double gyro_r, double accel_h, double accel_v)
{
	kd->x[0] += gyro_r - kd->x[1];
	kd->P[0] += (-1 * (kd->P[2] + kd->P[1])) + kd->Q[0];
	kd->P[1] -= kd->P[3];
	kd->P[2] -= kd->P[3];
	kd->P[3] += kd->Q[1];

	double ang = atan2(accel_h, accel_v);

	double y = ang - kd->x[0];
	double S = kd->P[0] + kd->R;
	double K[2] = {kd->P[0] / S, kd->P[2] / S};

	kd->x[0] += K[0] * y;
	kd->x[1] += K[1] * y;

	kd->P[0] -= K[0] * kd->P[0];
	kd->P[1] -= K[0] * kd->P[1];
	kd->P[2] -= K[1] * kd->P[0];
	kd->P[3] -= K[1] * kd->P[1];

	return kd->x[0];
}
