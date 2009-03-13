#include "main.h"

volatile ppm_data vex_data;
volatile sens_history sens_data[8];
mot_speed motor_speed;
mot_cali motor_cali;
heli_action copter_action;
volatile servo_ctrl servo_data;
PID_data yaw_pid;
PID_data pitch_pid_a;
PID_data pitch_pid_b;
PID_data roll_pid_a;
PID_data roll_pid_b;
volatile unsigned char op_mode;
volatile unsigned char fly_by_wire;
calibration main_cali;

#include "main_headers.h"

#ifdef DEBUG

void debug_tx(unsigned char addr, signed long data)
{
	addr *= 2;
	addr |= _BV(7);
	if(data < 0)
	{
		addr |= 1;
		data *= -1;
	}
	ser_tx(addr);

	for(unsigned char i = 0; i < 8; i++)
	{
		ser_tx(data & 0x0F);
		data = (data & 0xFFFFFFF0) >> 4;
	}
}

#endif

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

void timer_1_period_wait(signed long passed, signed long total)
{
	unsigned long ttt = 0;
	unsigned long lt = TCNT1;
	do
	{
		unsigned long tcntt = TCNT1;
		ttt += ((tcntt | 0x10000) - lt) & 0xFFFF;
		lt = tcntt;
	}
	while((passed + ttt) < total);
}

int main()
{
	hardware_init();
	software_init();

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
	if(to_save_to_eeprom())
	{
		save_calibration(main_cali, 0);
	}
	apply_calibration(main_cali);

	timer_1_reset();

	start_next_servo_pwm_period();

	while(1)
	{
		if(servo_data.ready_to_restart != 0)
		{
			unsigned long tsum = servo_data.servo_ticks[0] + servo_data.servo_ticks[1] + servo_data.servo_ticks[2] + servo_data.servo_ticks[3] + servo_data.servo_ticks[4];

			timer_1_period_wait(tsum, servo_data.servo_period_length);

			sens_data_proc();

			if(op_mode == FLY_MODE || op_mode == SAFE_MODE)
			{
				ppm_data ctrl_data = vex_data;

				if(vex_data.tx_good != 2 && fly_by_wire == 0)
				{
					for(unsigned char i = 0; i < 8; i++)
					{
						ctrl_data.chan_width[i] = 0;
					}

					op_mode = SAFE_MODE;
				}
				else
				{
					op_mode = FLY_MODE;
				}

				if(fly_by_wire != 0)
				{
					//
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

				allowed_roll = PID_mv(&roll_pid_a, current, target);
				target = allowed_roll;
				current = sens_data[roll_sens_chan].centered_avg;

				copter_action.roll = PID_mv(&roll_pid_b, current, target);

				target = scale(ctrl_data.chan_width[ctrl_data.pitch_ppm_chan], main_cali.move_cmd_scale, move_cmd_scale_multiplier);
				current = scale(sens_data[fb_accel_chan].centered_avg, main_cali.fb_lr_accel_scale, fb_lr_accel_scale_multiplier);

				allowed_pitch = PID_mv(&pitch_pid_a, current, target);
				target = allowed_pitch;
				current = sens_data[pitch_sens_chan].centered_avg;

				copter_action.pitch = PID_mv(&pitch_pid_b, current, target);				

				copter_action.col = main_cali.hover_throttle + scale(ctrl_data.chan_width[ctrl_data.throttle_ppm_chan], main_cali.throttle_cmd_scale, throttle_cmd_scale_multiplier);
				
				mot_set(&motor_speed, &motor_cali, &copter_action);
				mot_apply(&motor_speed, &motor_cali);

				#ifdef DEBUG

				if(ser_tx_buff.f == 0)
				{
					for(unsigned char i = 0, j = 0; i < 8; i++, j++)
					{
						debug_tx(i, sens_data[i].avg);
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
					LED_1_on();
				}
				else
				{
					LED_1_off();
				}

				servo_set
				(
					&servo_data,
					vex_data.chan_width[0] + width_500 * 3,
					vex_data.chan_width[1] + width_500 * 3,
					vex_data.chan_width[2] + width_500 * 3,
					vex_data.chan_width[3] + width_500 * 3
				);

				if(ser_tx_buff.f == 0)
				{
					for(unsigned char i = 0, j = 0; i < 8; i++, j++)
					{
						debug_tx(i, sens_data[i].avg);
					}
					for(unsigned char i = 0, j = 8; i < 8; i++, j++)
					{
						debug_tx(j, sens_data[i].noise);
					}
					for(unsigned char i = 0, j = 16; i < 8; i++, j++)
					{
						debug_tx(j, vex_data.chan_width[i]);
					}
				}
			}
			else if(op_mode == TEST_MODE_B)
			{
			}

			#endif

			start_next_servo_pwm_period();
		}

		cmd_proc();
	}

	return 0;
}

