signed long scale(signed long in, signed long numer, signed long denom)
{	
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

signed long map(signed long in, signed long old_min, signed long old_max, signed long new_min, signed long new_max)
{
	signed long old_diff = old_max - old_min;
	signed long new_diff = new_max - new_min;

	return new_min + scale(in - old_min, new_diff, old_diff);
}

signed long constrain(signed long in, signed long min, signed long max)
{
	if(in < min)
	{
		return min;
	}
	else if(in > max)
	{
		return max;
	}
	else
	{
		return in;
	}
}

signed long abs(signed long in)
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
