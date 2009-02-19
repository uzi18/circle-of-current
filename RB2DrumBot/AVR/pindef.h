#ifndef pindef_h

#define uart_port PORTD
#define uart_rx_pin 0

#define LED_pin 3
#define LED_port PORTC
#define LED_ddr DDRC

#define button_port PORTC
#define button_ddr DDRC
#define button_pin 2
#define button_input_reg PINC

// for the bit numbers, go to
// http://wiibrew.org/wiki/Wiimote/Extension_Controllers

#define drum_out_port PORTA
#define drum_out_ddr DDRA

#define green_bit 4
#define green_pin 0

#define red_bit 6
#define red_pin 3

#define yellow_bit 5
#define yellow_pin 2

#define blue_bit 3
#define blue_pin 1

#define bass_bit 2
#define bass_pin 4

#define pindef_h
#endif
