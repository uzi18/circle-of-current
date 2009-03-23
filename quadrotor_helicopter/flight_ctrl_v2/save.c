#include "save.h"

double param_get_d(saved_params_s * sps, unsigned char addr)
{
	return sps->d_val[addr];
}

unsigned long param_get_ul(saved_params_s * sps, unsigned char addr)
{
	return sps->ul_val[addr];
}

signed long param_get_sl(saved_params_s * sps, unsigned char addr)
{
	return sps->sl_val[addr];
}

void param_set_d(saved_params_s * sps, unsigned char addr, double d)
{
	sps->d_val[addr] = d;
}

void param_set_ul(saved_params_s * sps, unsigned char addr, unsigned long d)
{
	sps->ul_val[addr] = d;
}

void param_set_sl(saved_params_s * sps, unsigned char addr, signed long d)
{
	sps->sl_val[addr] = d;
}

void param_save(saved_params_s * sps, unsigned char addr)
{
	for(unsigned int i = addr * 4; i < (addr * 4) + 4; i++)
	{
		unsigned char c = eeprom_read_byte(i);
		unsigned char d = sps->c[i];
		if(c != d)
		{
			eeprom_write_byte(i, d);
		}
	}
}

void param_load(saved_params_s * sps, unsigned char addr)
{
	for(unsigned int i = addr * 4; i < (addr * 4) + 4; i++)
	{
		unsigned char c = eeprom_read_byte(i);
		sps->c[i] = c;
	}
}

void param_save_all(saved_params_s * sps)
{
	for(unsigned int i = 0; i < sizeof(saved_params_s); i++)
	{
		unsigned char c = eeprom_read_byte(i);
		unsigned char d = sps->c[i];
		if(c != d)
		{
			eeprom_write_byte(i, d);
		}
	}
}

void param_load_all(saved_params_s * sps)
{
	for(unsigned int i = 0; i < sizeof(saved_params_s); i++)
	{
		unsigned char c = eeprom_read_byte(i);
		sps->c[i] = c;
	}
}

void param_init(saved_params_s * sps)
{
	sps->sl_s.f_mot_bot = f_mot_bot_def_val;
	sps->sl_s.b_mot_bot = b_mot_bot_def_val;
	sps->sl_s.l_mot_bot = l_mot_bot_def_val;
	sps->sl_s.r_mot_bot = r_mot_bot_def_val;
	sps->sl_s.f_mot_scale = f_mot_scale_def_val;
	sps->sl_s.b_mot_scale = b_mot_scale_def_val;
	sps->sl_s.l_mot_scale = l_mot_scale_def_val;
	sps->sl_s.r_mot_scale = r_mot_scale_def_val;
	sps->sl_s.roll_gyro_center = roll_gyro_center_def_val;
	sps->sl_s.pitch_gyro_center = pitch_gyro_center_def_val;
	sps->sl_s.yaw_gyro_center = yaw_gyro_center_def_val;
	sps->sl_s.yaw_scale = yaw_scale_def_val;
	sps->sl_s.roll_accel_bot = roll_accel_bot_def_val;
	sps->sl_s.pitch_accel_bot = pitch_accel_bot_def_val;
	sps->sl_s.vert_accel_bot = vert_accel_bot_def_val;
	sps->sl_s.roll_accel_top = roll_accel_top_def_val;
	sps->sl_s.pitch_accel_top = pitch_accel_top_def_val;
	sps->sl_s.vert_accel_top = vert_accel_top_def_val;
	sps->sl_s.roll_offset = roll_offset_def_val;
	sps->sl_s.pitch_offset = pitch_offset_def_val;
	sps->sl_s.roll_ppm_scale = roll_ppm_scale_def_val;
	sps->sl_s.pitch_ppm_scale = pitch_ppm_scale_def_val;
	sps->sl_s.roll_gyro_to_rate = roll_gyro_to_rate_def_val;
	sps->sl_s.pitch_gyro_to_rate = pitch_gyro_to_rate_def_val;
	sps->sl_s.roll_level_kp = roll_level_kp_def_val;
	sps->sl_s.roll_level_ki = roll_level_ki_def_val;
	sps->sl_s.roll_level_kd = roll_level_kd_def_val;
	sps->sl_s.pitch_level_kp = pitch_level_kp_def_val;
	sps->sl_s.pitch_level_ki = pitch_level_ki_def_val;
	sps->sl_s.pitch_level_kd = pitch_level_kd_def_val;
	sps->sl_s.roll_rate_kp = roll_rate_kp_def_val;
	sps->sl_s.roll_rate_ki = roll_rate_ki_def_val;
	sps->sl_s.roll_rate_kd = roll_rate_kd_def_val;
	sps->sl_s.pitch_rate_kp = pitch_rate_kp_def_val;
	sps->sl_s.pitch_rate_ki = pitch_rate_ki_def_val;
	sps->sl_s.pitch_rate_kd = pitch_rate_kd_def_val;
	sps->sl_s.yaw_kp = yaw_kp_def_val;
	sps->sl_s.yaw_ki = yaw_ki_def_val;
	sps->sl_s.yaw_kd = yaw_kd_def_val;
	sps->sl_s.comp_filter_w = comp_filter_w_def_val;
	sps->sl_s.kalman_q_ang = kalman_q_ang_def_val;
	sps->sl_s.kalman_q_gyro = kalman_q_gyro_def_val;
	sps->sl_s.kalman_r_ang = kalman_r_ang_def_val;
	sps->sl_s.throttle_hover = throttle_hover_def_val;
	sps->sl_s.throttle_scale = throttle_scale_def_val;
	sps->ul_s.period_ticks = period_ticks_def_val;
	sps->ul_s.when_to_update_esc = when_to_update_esc_def_val;
	sps->ul_s.roll_ppm_chan = roll_ppm_chan_def_val;
	sps->ul_s.pitch_ppm_chan = pitch_ppm_chan_def_val;
	sps->ul_s.yaw_ppm_chan = yaw_ppm_chan_def_val;
	sps->ul_s.throttle_ppm_chan = throttle_ppm_chan_def_val;
	sps->ul_s.params_updated = params_updated_def_val;
}
