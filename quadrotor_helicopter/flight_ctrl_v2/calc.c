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

double calc_pow(double x, signed char n)
{
	if(n == 0)
	{
		return 1;
	}
	else if(bit_is_set(n, 0))
	{
		return x * calc_pow(x, n - 1);
	}
	else
	{
		double z = calc_pow(x, n / 2);
		return z * z;
	}
}

#include "trig_tbl.h"

double calc_asin(double x)
{
	unsigned long addr = lround(fabs(calc_constrain_double(x, -1, 1) * asin_multiplier));
	union float__ {
		unsigned long l;
		double d;
	} _float_;
	_float_.l = pgm_read_dword(&(asin_tbl[addr])); 
	double r = _float_.d;
	if(x < 0)
	{
		return -r;
	}
	else
	{
		return r;
	}
}

double calc_atan2(double y, double x)
{
	double z = 0;
	if(fabs(x) > fabs(y))
	{
		z = y / x;
	}
	else if(fabs(x) < fabs(y))
	{
		z = x / y;
	}
	else if(x == 0 && y == 0)
	{
		return 0;
	}
	else if(x == y)
	{
		z = 1;
	}

	z *= atan_multiplier;

	unsigned long addr = lround(fabs(z));

	union float__ {
		unsigned long l;
		double d;
	} _float_;

	_float_.l = pgm_read_dword(&(atan_tbl[addr])); 
	double r = _float_.d;

	if(fabs(x) < fabs(y))
	{
		r = (M_PI / 2) - r;
	}

	if(x > 0)
	{
		if(y > 0)
		{
			return r;
		}
		else
		{
			return -r;
		}
	}
	else
	{
		if(y > 0)
		{
			return M_PI - r;
		}
		else
		{
			return 0 - M_PI + r;
		}
	}
}

double calc_rad_to_deg(double x)
{
	return x * rad_to_deg_const;
}

double calc_deg_to_rad(double x)
{
	return x / rad_to_deg_const;
}

signed long calc_constrain_long(signed long in, signed long min_, signed long max_)
{
	return calc_min(calc_max(in, min_), max_);
}

double calc_constrain_double(double in, double min_, double max_)
{
	return fmin(fmax(in, min_), max_);
}

double complementary_filter(double * ang, double accel_ang, double gyro_r, double w, double dt)
{
	*ang = (1 - w) * (*ang + (gyro_r * dt)) + (w * accel_ang);
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

double kalman_filter(kalman_data * kd, double gyro_r, double ang, double dt)
{
	kd->x[0] += dt * (gyro_r - kd->x[1]);
	kd->P[0] += (-dt * (kd->P[2] + kd->P[1])) + (dt * kd->Q[0]);
	kd->P[1] -= dt * kd->P[3];
	kd->P[2] -= dt * kd->P[3];
	kd->P[3] += dt * kd->Q[1];

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
