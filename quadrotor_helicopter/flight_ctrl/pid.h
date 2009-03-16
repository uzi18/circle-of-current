signed long PID_mv(PID_data * pid, signed long current, signed long target)
{
	signed long err = target - current;
	pid->err_sum = constrain(pid->err_sum + err, -1073741824, 1073741824);
	signed long delta_err = err - pid->err_last;
	if(pid->err_sum * pid->err_sum < pid->constants.err_low_thresh * pid->constants.err_low_thresh && delta_err * delta_err < pid->constants.delta_err_low_thresh * pid->constants.delta_err_low_thresh)
	{
		pid->err_sum = 0;
	}
	signed long mv = err * pid->constants.kp + pid->err_sum * pid->constants.ki + delta_err * pid->constants.kd;
	mv = scale(mv, 1, PID_const_multiplier);
	pid->err_sum = scale(pid->err_sum, 99, 100);
	pid->err_last = err;

	return mv;
}
