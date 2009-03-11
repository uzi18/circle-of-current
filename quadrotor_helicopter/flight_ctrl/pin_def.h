#ifndef pin_def_h
#define pin_def_h

#define servo_port PORTD
#define servo_ddr DDRD
#define servo_shift_pin 5
#define servo_input_pin 2
#define servo_reset_pin 3
#define f_motor_pin 0
#define b_motor_pin 1
#define l_motor_pin 2
#define r_motor_pin 3
#define aux_servo_pin 4

#define roll_sens_chan 0
#define pitch_sens_chan 1
#define yaw_sens_chan 2
#define fb_accel_chan 3
#define lr_accel_chan 4
#define ud_accel_chan 5
#define ranger_chan 6

#define input_capt_port PORTD
#define input_capt_ddr DDRD
#define input_capt_pin 6

#define LED_1_pin 2
#define LED_1_port PORTD
#define LED_1_ddr DDRD

#define LED_2_pin 3
#define LED_2_port PORTD
#define LED_2_ddr DDRD

#endif
