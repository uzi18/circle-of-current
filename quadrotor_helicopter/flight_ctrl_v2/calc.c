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
	volatile signed long t_ = in * numer;
	volatile signed long t = t_ / denom;
	volatile signed long _t = t * denom;
	volatile signed long diff = t_ - _t;
	if(denom * denom != 1)
	{
		volatile signed long one = 1;
		if(denom < 0 && numer >= 0)
		{
			one = -1;
		}

		if(diff >= denom / 2)
		{
			t += one;
		}
		else if(-diff >= denom / 2)
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

signed long calc_map(signed long in, signed long old_min, signed long old_max, signed long new_min, signed long new_max)
{
	return new_min + calc_multi(in - old_min, new_max - new_min, old_max - old_min);
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

signed long calc_constrain(signed long in, signed long min, signed long max)
{
	return calc_min(calc_max(in, min), max);
}

signed long PID_mv(PID_data * pid, signed long current, signed long target)
{
	signed long err = target - current;
	pid->err_sum = calc_constrain(pid->err_sum + err, -1073741824, 1073741824);
	signed long delta_err = err - pid->err_last;
	if(calc_abs(pid->err_sum) < pid->constants.err_low_thresh && calc_abs(delta_err) < pid->constants.delta_err_low_thresh)
	{
		pid->err_sum = 0;
	}
	signed long mv = err * pid->constants.kp + pid->err_sum * pid->constants.ki + delta_err * pid->constants.kd;
	mv = calc_multi(mv, 1, PID_const_multiplier);
	pid->err_sum = calc_multi(pid->err_sum, pid->constants.err_dec, 100);
	pid->err_last = err;

	return mv;
}
