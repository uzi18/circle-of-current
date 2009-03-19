#ifndef pindef_h_inc
#define pindef_h_inc

#define roll_accel_chan 0
#define pitch_accel_chan 1
#define vert_accel_chan 2
#define roll_gyro_chan 3
#define pitch_gyro_chan 4
#define yaw_gyro_chan 5

#define esc_port PORTD
#define esc_ddr DDRD
#define esc_rst_pin 4
#define esc_clk_pin 5
#define esc_dat_pin 3

#define ppm_port PORTD
#define ppm_ddr DDRD
#define ppm_pin 6

#define LED_port PORTC
#define LED_ddr DDRC
#define LED1_pin 2
#define LED2_pin 3

#endif
