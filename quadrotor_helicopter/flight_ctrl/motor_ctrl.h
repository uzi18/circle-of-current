void servo_set(servo_ctrl * sc, signed long f, signed long b, signed long l, signed long r)
{
	sc->servo_ticks[f_mot_chan] = f;
	sc->servo_ticks[b_mot_chan] = b;
	sc->servo_ticks[l_mot_chan] = l;
	sc->servo_ticks[r_mot_chan] = r;
}

void mot_apply(mot_speed * ms, mot_cali * mc)
{
	servo_set
	(
		&servo_data,
		mc->servo_stop + scale(ms->f, mc->servo_pulse_scale, motor_scale_multiplier),
		mc->servo_stop + scale(ms->b, mc->servo_pulse_scale, motor_scale_multiplier),
		mc->servo_stop + scale(ms->l, mc->servo_pulse_scale, motor_scale_multiplier),
		mc->servo_stop + scale(ms->r, mc->servo_pulse_scale, motor_scale_multiplier)
	);
}

void mot_set(mot_speed * ms, mot_cali * mc, heli_action * ha)
{
	ms->f = ha->col + ha->yaw - ha->pitch + mc->f_mot_tweak;
	ms->b = ha->col + ha->yaw + ha->pitch + mc->b_mot_tweak;
	ms->l = ha->col - ha->yaw - ha->roll + mc->l_mot_tweak;
	ms->r = ha->col - ha->yaw + ha->roll + mc->r_mot_tweak;
	ms->f = scale(ms->f, mc->f_mot_scale, motor_scale_multiplier);
	ms->b = scale(ms->b, mc->b_mot_scale, motor_scale_multiplier);
	ms->l = scale(ms->l, mc->l_mot_scale, motor_scale_multiplier);
	ms->r = scale(ms->r, mc->r_mot_scale, motor_scale_multiplier);
}
