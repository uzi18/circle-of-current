signed long scale(signed long in, signed long per, signed long multiplier)
{	
	signed long t_ = in * per;
	signed long t = t_ / multiplier;
	signed long _t = t * multiplier;
	signed long diff = t_ - _t;
	if(diff >= multiplier / 2)
	{
		t++;
	}
	else if(-diff >= multiplier / 2)
	{
		t--;
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
