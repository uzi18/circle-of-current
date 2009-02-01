#ifndef pindef_h

#define twi_port PORTC
#define twi_ddr DDRC
#define twi_scl_pin 5
#define twi_sda_pin 4

#define uart_port PORTD
#define uart_rx_pin 2

#define dev_detect_pin 4
#define dev_detect_port PORTD
#define dev_detect_ddr DDRD

#define LED_pin 3
#define LED_port PORTC
#define LED_ddr DDRC

#define power_detect_pin 3
#define power_detect_port PORTC
#define power_detect_ddr DDRC
#define power_detect_input PINC

#define adc_port PORTC
#define adc_ddr DDRC

// for the bit numbers, go to
// http://wiibrew.org/wiki/Wiimote/Extension_Controllers

#define green_bit 4
#define green_pin 3
#define green_in_reg PINC
#define green_in_preg PINCP
#define green_port PORTC
#define green_ddr DDRC

#define red_bit 6
#define red_pin 2
#define red_in_reg PINC
#define red_in_preg PINCP
#define red_port PORTC
#define red_ddr DDRC

#define yellow_bit 5
#define yellow_pin 1
#define yellow_in_reg PINC
#define yellow_in_preg PINCP
#define yellow_port PORTC
#define yellow_ddr DDRC

#define blue_bit 3
#define blue_pin 0
#define blue_in_reg PINC
#define blue_in_preg PINCP
#define blue_port PORTC
#define blue_ddr DDRC

#define orange_bit 7
#define orange_pin 4
#define orange_in_reg PINC
#define orange_in_preg PINCP
#define orange_port PORTC
#define orange_ddr DDRC

#define bass_bit 2

#define bass1_pin 1
#define bass1_in_reg PINB
#define bass1_in_preg PINBP
#define bass1_port PORTB
#define bass1_ddr DDRB

#define bass2_pin 2
#define bass2_in_reg PINB
#define bass2_in_preg PINBP
#define bass2_port PORTB
#define bass2_ddr DDRB

#define plus_bit 2
#define plus_pin 6
#define plus_in_reg PINA
#define plus_port PORTA
#define plus_ddr DDRA

#define minus_bit 4
#define minus_pin 7
#define minus_in_reg PINA
#define minus_port PORTA
#define minus_ddr DDRA

#define up_stick_pin 6
#define up_stick_in_reg PINA
#define up_stick_port PORTA
#define up_stick_ddr DDRA

#define down_stick_pin 6
#define down_stick_in_reg PINA
#define down_stick_port PORTA
#define down_stick_ddr DDRA

#define left_stick_pin 6
#define left_stick_in_reg PINA
#define left_stick_port PORTA
#define left_stick_ddr DDRA

#define right_stick_pin 6
#define right_stick_in_reg PINA
#define right_stick_port PORTA
#define right_stick_ddr DDRA

#define pindef_h
#endif
