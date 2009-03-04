signed long PID_mv(PID_data * pid, signed long current, signed long target)
{
	signed long err = target - current;
	pid->err_sum += err;
	signed long delta_err = err - pid->err_last;
	if(pid->err_sum > 1073741824 / PID_const_multiplier)
	{
		pid->err_sum = 1073741824 / PID_const_multiplier;
	}
	else if(pid->err_sum < -1073741824 / PID_const_multiplier)
	{
		pid->err_sum = -1073741824 / PID_const_multiplier;
	}
	else if(pid->err_sum * pid->err_sum < pid->constants.err_low_thresh * pid->constants.err_low_thresh && delta_err * delta_err < pid->constants.delta_err_low_thresh * pid->constants.delta_err_low_thresh)
	{
		pid->err_sum = 0;
	}
	signed long mv_ = (pid->constants.kp * err) + (pid->constants.ki * pid->err_sum) + (pid->constants.kd * delta_err);
	pid->err_last = err;
	signed long mv = mv_ / PID_const_multiplier;
	signed long _mv = mv * PID_const_multiplier;
	signed long diff = mv_ - _mv;
	if(diff >= PID_const_multiplier / 2)
	{
		mv++;
	}
	else if(-diff >= PID_const_multiplier / 2)
	{
		mv--;
	}

	return mv;
}
