#include "main.h"

static volatile ppm_data vex_data;
static volatile sens_history sens_data[8];
static volatile mot_speed motor_speed;
static volatile mot_cali motor_cali;
static volatile heli_action copter_action;
static volatile servo_ctrl servo_data;
static volatile PID_data yaw_pid;
static volatile PID_data pitch_pid_a;
static volatile PID_data pitch_pid_b;
static volatile PID_data roll_pid_a;
static volatile PID_data roll_pid_b;
static volatile unsigned char op_mode;
static volatile calibration main_cali;

#include "main_headers.h"

void debug_tx(unsigned char addr, signed long data)
{
	addr++;
	unsigned char c = 0;
	if(data < 0)
	{
		c = 1;
		data *= -1;
	}
	c |= (addr << 4);
	ser_tx(c);

	for(unsigned char i = 0; i < 8; i++)
	{
		ser_tx(data & 0x0F);
		data = (data & 0xFFFFFFF0) >> 4;
	}
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

int main()
{
	hardware_init();
	software_init();

	LED_1_off();
	LED_2_off();

	_delay_ms(1);

	LED_1_on();
	LED_2_on();

	default_calibration(&main_cali);
	if(to_calibrate())
	{
		calibrate_sensors(&main_cali);
		calibrate_controller(&main_cali);
	}
	if(to_load_from_eeprom())
	{
		load_calibration(&main_cali, 0);
	}
	if(to_save_to_eeprom())
	{
		save_calibration(main_cali, 0);
	}
	calibrate_controller(&main_cali);

	hardware_init();
	software_init();

	apply_calibration(main_cali);

	start_next_servo_pwm_period();

	while(1)
	{
		if(servo_data.ready_to_restart != 0)
		{
			unsigned long tsum = servo_data.servo_ticks[0] + servo_data.servo_ticks[1] + servo_data.servo_ticks[2] + servo_data.servo_ticks[3] + servo_data.servo_ticks[4];
			unsigned long ttt = 0;
			unsigned long lt = TCNT1;
			do
			{
				unsigned long tcntt = TCNT1;
				ttt += ((tcntt | 0x10000) - lt) & 0xFFFF;
				lt = tcntt;
			}
			while((tsum + ttt) < servo_data.servo_period_length);

			sens_data_proc();

			if(op_mode == FLY_MODE || op_mode == SAFE_MODE)
			{
				ppm_data ctrl_data = vex_data;

				if(vex_data.tx_good != 2)
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

				signed long target;
				signed long current;

				target = scale(ctrl_data.chan_width[ctrl_data.yaw_ppm_chan], main_cali.yaw_cmd_scale, yaw_cmd_scale_multiplier);
				current = scale(sens_data[yaw_sens_chan].centered_avg, main_cali.yaw_sens_scale, yaw_sens_scale_multiplier);

				copter_action.yaw = PID_mv(&yaw_pid, current, target);

				target = scale(ctrl_data.chan_width[ctrl_data.roll_ppm_chan], main_cali.move_cmd_scale, move_cmd_scale_multiplier);
				current = scale(sens_data[lr_accel_chan].centered_avg, main_cali.fb_lr_accel_scale, fb_lr_accel_scale_multiplier);

				target = PID_mv(&roll_pid_a, current, target);
				current = sens_data[roll_sens_chan].centered_avg;

				copter_action.roll = PID_mv(&roll_pid_b, current, target);

				target = scale(ctrl_data.chan_width[ctrl_data.pitch_ppm_chan], main_cali.move_cmd_scale, move_cmd_scale_multiplier);
				current = scale(sens_data[fb_accel_chan].centered_avg, main_cali.fb_lr_accel_scale, fb_lr_accel_scale_multiplier);

				target = PID_mv(&pitch_pid_a, current, target);
				current = sens_data[pitch_sens_chan].centered_avg;

				copter_action.pitch = PID_mv(&pitch_pid_b, current, target);				

				copter_action.col = main_cali.hover_throttle + scale(ctrl_data.chan_width[ctrl_data.throttle_ppm_chan], main_cali.throttle_cmd_scale, throttle_cmd_scale_multiplier);
				
				mot_set(&motor_speed, &motor_cali, &copter_action);
				mot_apply(&motor_speed, &motor_cali);
			}
			else if(op_mode == POWER_OFF_MODE)
			{
			}
			else if(op_mode == OTHER_MODE)
			{
			}
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
					for(unsigned char i = 0; i < 4; i++)
					{
						debug_tx(i, sens_data[i].noise);
					}
					for(unsigned char i = 4; i < 8; i++)
					{
						debug_tx(i, sens_data[i - 4].avg);
					}
					for(unsigned char i = 8; i < 12; i++)
					{
						debug_tx(i, vex_data.chan_width[i - 8]);
					}
				}
			}
			else if(op_mode == TEST_MODE_B)
			{
				for(unsigned char i = 0; i < 8; i++)
				{
					unsigned int a = sens_data[i].avg;
					unsigned char l = a & 0x7F;
					unsigned int h_ = ((a - l) >> 7) | (i << 4) | 0x80;
					unsigned char h = (unsigned char)h_;
					ser_tx(h); ser_tx(l);
				}
				while(ser_rx() == -1);
			}

			start_next_servo_pwm_period();
		}
	}
	return 0;
}

