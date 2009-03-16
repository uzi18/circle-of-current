void servo_set(servo_ctrl * sc, mot_speed * ms)
{
	sc->servo_ticks[f_mot_chan] = constrain(ms->f, width_500, width_500 * 5);
	sc->servo_ticks[b_mot_chan] = constrain(ms->b, width_500, width_500 * 5);
	sc->servo_ticks[l_mot_chan] = constrain(ms->l, width_500, width_500 * 5);
	sc->servo_ticks[r_mot_chan] = constrain(ms->r, width_500, width_500 * 5);
}

void mot_set(mot_speed * ms, mot_cali * mc, heli_action * ha)
{
	ms->f = ha->col + ha->yaw - ha->pitch;
	ms->b = ha->col + ha->yaw + ha->pitch;
	ms->l = ha->col - ha->yaw - ha->roll;
	ms->r = ha->col - ha->yaw + ha->roll;
	ms->f = scale(ms->f, mc->f_mot_scale, motor_scale_multiplier) + mc->f_mot_tweak;
	ms->b = scale(ms->b, mc->b_mot_scale, motor_scale_multiplier) + mc->b_mot_tweak;
	ms->l = scale(ms->l, mc->l_mot_scale, motor_scale_multiplier) + mc->l_mot_tweak;
	ms->r = scale(ms->r, mc->r_mot_scale, motor_scale_multiplier) + mc->r_mot_tweak;
}
