#include "main.h"

static volatile ppm_data vex_data;
static ppm_data ctrl_data;
static volatile sens_history sens_data[8];
static mot_speed motor_speed;
static mot_cali motor_cali;
static heli_action copter_action;
static volatile servo_ctrl servo_data;
static PID_data yaw_pid;
static PID_data pitch_pid_level;
static PID_data pitch_pid_rate;
static PID_data roll_pid_level;
static PID_data roll_pid_rate;
static volatile unsigned char op_mode;
static volatile unsigned char fly_by_wire;
static volatile unsigned char safety;
static calibration main_cali;
static unsigned long process_time;
static unsigned int process_time_stamp;

#include "main_headers.h"

ISR(BADISR_vect)
{
}

void cmd_proc()
{
}

void start_next_servo_pwm_period()
{
	servo_data.ready_to_restart = 0;
	servo_data.chan = 0;

	servo_shift_reset();

	unsigned int tt = TCNT1;

	OCR1A = tt + 128;

	servo_port &= 0xFF ^ _BV(servo_shift_pin);

	if(bit_is_set(TIFR1, OCF1A))
	{
		TIFR1 |= _BV(OCF1A);
	}

	TCCR1A |= _BV(COM1A0);
	TIMSK1 |= _BV(OCIE1A);
}

signed long timer_1_period_wait(signed long passed, signed long total)
{
	volatile unsigned long ttt = 0;
	volatile unsigned long lt = TCNT1;
	while((passed + ttt) < total)
	{
		volatile unsigned long tcntt = TCNT1;
		ttt += ((tcntt | 0x10000) - lt) & 0xFFFF;
		lt = tcntt;
	}
	return ttt;
}

int main()
{
	hardware_init();

	vex_data.new_flag = 0;
	while(vex_data.new_flag == 0);
	vex_data.new_flag = 0;

	LED_1_off();
	LED_1_tog();

	default_calibration(&main_cali);
	if(to_load_from_eeprom())
	{
		load_calibration(&main_cali, 0);
	}
	if(to_calibrate_sens())
	{
		calibrate_sensors(&main_cali);
	}
	if(to_calibrate_ppm())
	{
		calibrate_controller(&main_cali);
	}
	if(to_set_limit())
	{
		set_ctrl_limit(&main_cali);
	}
	if(to_save_to_eeprom())
	{
		default_calibration(&main_cali);
		save_calibration(main_cali, 0);
	}
	apply_calibration(main_cali);

	software_init();
	servo_data.ready_to_restart = 1;

	while(1)
	{
		keep_busy;
		if(servo_data.ready_to_restart == 1)
		{
			//LED_2_tog();
			unsigned long tsum = servo_data.servo_ticks[0] + servo_data.servo_ticks[1] + servo_data.servo_ticks[2] + servo_data.servo_ticks[3];
			for(unsigned char i = 4; i < 4 + main_cali.extra_servo_chan; i++)
			{
				tsum += servo_data.servo_ticks[i];
			}

			timer_1_period_wait(tsum + process_time, servo_data.servo_period_length);

			process_time_stamp = TCNT0;

			sens_data_proc();

			if(vex_data.tx_good != 2 && fly_by_wire == 0 && op_mode == FLY_MODE)
			{
				op_mode = LOST_CMD_MODE;
			}

			if(op_mode == FLY_MODE || op_mode == LOST_CMD_MODE)
			{
				if(fly_by_wire == 0)
				{
					ctrl_data = vex_data;
				}

				if(op_mode == LOST_CMD_MODE)
				{
					for(unsigned char i = 0; i < 8; i++)
					{
						ctrl_data.chan_width[i] = 0;
					}
				}

				for(unsigned char i = 4, j = 0; i < 4 + main_cali.extra_servo_chan; i++, j++)
				{
					servo_data.servo_ticks[i] = ctrl_data.chan_width[main_cali.servo_ppm_link[j]];
				}

				signed long target;
				signed long current;

				signed long allowed_roll;
				signed long allowed_pitch;

				target = scale(ctrl_data.chan_width[ctrl_data.yaw_ppm_chan], main_cali.yaw_cmd_scale, yaw_cmd_scale_multiplier);
				current = scale(sens_data[yaw_sens_chan].centered_avg, main_cali.yaw_sens_scale, yaw_sens_scale_multiplier);

				copter_action.yaw = PID_mv(&yaw_pid, current, target);

				target = scale(ctrl_data.chan_width[ctrl_data.roll_ppm_chan], main_cali.move_cmd_scale, move_cmd_scale_multiplier);
				current = scale(sens_data[lr_accel_chan].centered_avg, main_cali.fb_lr_accel_scale, fb_lr_accel_scale_multiplier);

				allowed_roll = PID_mv(&roll_pid_level, current, target);
				target = allowed_roll;
				current = sens_data[roll_sens_chan].centered_avg;

				copter_action.roll = PID_mv(&roll_pid_rate, current, target);

				target = scale(ctrl_data.chan_width[ctrl_data.pitch_ppm_chan], main_cali.move_cmd_scale, move_cmd_scale_multiplier);
				current = scale(sens_data[fb_accel_chan].centered_avg, main_cali.fb_lr_accel_scale, fb_lr_accel_scale_multiplier);

				allowed_pitch = PID_mv(&pitch_pid_level, current, target);
				target = allowed_pitch;
				current = sens_data[pitch_sens_chan].centered_avg;

				copter_action.pitch = PID_mv(&pitch_pid_rate, current, target);				

				copter_action.col = main_cali.hover_throttle + scale(ctrl_data.chan_width[ctrl_data.throttle_ppm_chan], main_cali.throttle_cmd_scale, throttle_cmd_scale_multiplier);
				
				mot_set(&motor_speed, &motor_cali, &copter_action);
				servo_set(&servo_data, &motor_speed);

				#ifdef DEBUG

				if(ser_tx_buff.f == 0)
				{
					for(unsigned char i = 0, j = 0; i < 8; i++, j++)
					{
						debug_tx(i, sens_data[i].centered_avg);
					}
					for(unsigned char i = 0, j = 8; i < 8; i++, j++)
					{
						debug_tx(j, sens_data[i].noise);
					}
					for(unsigned char i = 0, j = 16; i < 8; i++, j++)
					{
						debug_tx(j, vex_data.chan_width[i]);
					}
					debug_tx(24, allowed_roll);
					debug_tx(25, allowed_pitch);
					debug_tx(26, copter_action.col);
					debug_tx(27, copter_action.yaw);
					debug_tx(28, copter_action.roll);
					debug_tx(29, copter_action.pitch);
				}

				#endif
			}
			else if(op_mode == POWER_OFF_MODE)
			{
			}
			else if(op_mode == OTHER_MODE)
			{
			}

			#ifdef DEBUG

			else if(op_mode == TEST_MODE_A)
			{
				if(vex_data.tx_good == 2)
				{
					//LED_1_on();
				}
				else
				{
					//LED_1_off();
				}

				motor_speed.f = vex_data.chan_width[0] + width_500 * 3;
				motor_speed.b = vex_data.chan_width[1] + width_500 * 3;
				motor_speed.l = vex_data.chan_width[2] + width_500 * 3;
				motor_speed.r = vex_data.chan_width[3] + width_500 * 3;
				
				servo_set(&servo_data, &motor_speed);

				if(ser_tx_buff.f == 0)
				{
					for(unsigned char i = 0, j = 0; i < 4; i++, j++)
					{
						debug_tx(j, sens_data[i].avg);
					}
					for(unsigned char i = 0, j = 8; i < 4; i++, j++)
					{
						debug_tx(j, sens_data[i].centered_avg);
					}
					for(unsigned char i = 0, j = 16; i < 4; i++, j++)
					{
						debug_tx(j, vex_data.chan_width[i]);
					}
					for(unsigned char i = 0, j = 24; i < 4; i++, j++)
					{
						debug_tx(j, vex_data.chan_offset[i]);
					}
					debug_tx(32, tsum);
				}
			}
			else if(op_mode == TEST_MODE_B)
			{
			}

			#endif

			start_next_servo_pwm_period();

			unsigned int tcnt0_ = TCNT0;
			process_time = (((tcnt0_ | 0x100) - process_time_stamp) & 0xFF) * 1024;
		}

		cmd_proc();
	}

	return 0;
}

