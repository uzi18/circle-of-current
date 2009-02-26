#define F_CPU 10000000UL

#define nop asm volatile ("nop"); 
#define ID1 0x01
#define ID2 0x03

//#define serial //comment to not use serial

#define remoteControlled //comment to read data from pins instead of over serial port

#ifdef remoteControlled
	#ifndef serial
		#define serial
	#endif
#endif


#define bufferSize 16
#include <avr/io.h>

#define USART_BAUDRATE 38400
#define BAUD_PRESCALE (((F_CPU / (USART_BAUDRATE * 16UL))) - 1) 

#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit)) // clear bit
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit)) // set bit

//#include <avr/eeprom.h> 
#include <stdio.h>
#include <avr/interrupt.h>

#include <util/delay.h>
#include <avr/iom168.h>
#include <avr/pgmspace.h> 
#include <string.h>

volatile uint8_t memory [0x100]; //This will store all the readable and writable addresses.
volatile uint8_t address; //This is the next memory address for to be accessed by I2C, for both read and write.
volatile uint8_t newData = 0; //This will be one if the data I am sent should be written/read, 0 when I should take it to be the address.

volatile uint8_t idx=0;
volatile uint8_t ft[8];
volatile uint8_t sb[8];
volatile uint8_t testKey[6];
volatile uint8_t generate = 0;

#include "constants.h"
#include "cryptoFunctions.h"
#include "TWI.h"

#ifdef serial

void serTx(unsigned char data)
	{
		loop_until_bit_is_set(UCSR0A, UDRE0); // wait while previous tx is finished
		UDR0 = data; // tx
	}
uint8_t serRx (){
	while(!(UCSR0A & (1<<RXC0)));
	return UDR0;
}

#endif

void setupTwi(){	//Setup TWI
	//PORTC = 0xFF;	//Enable all pull ups
	PRR &= 0xFF^1<<PRTWI;//Set PRTWI bit in power reduction register to 0
	TWAR = (0x52<<1) | (1<<TWGCE); //Set slave address to 0x52 (And skip the TWGCE bit)
	//TWAMR = 0xFF;
    TWCR = (1<<TWEA) | (1<<TWIE) | (1<<TWEN);	//Setup control register (page 238 reference)
}

int main(){
	sei();
	#ifdef serial
		UCSR0B |= (1 << RXEN0) | (1 << TXEN0);
		UCSR0C |= (1 << UCSZ00) | (1 << UCSZ01);
		UBRR0L = BAUD_PRESCALE; 
		UBRR0H = (BAUD_PRESCALE >> 8);
	#endif
	setupTwi();
	MCUCR &= 0xFF^(1<<PUD);
	DDRD = 0x00;
	PORTD = 0xFF;


	memory[0xFA]= 0x00;//Setup ID information.
	memory[0xFB]= 0x00;
	memory[0xFC]= 0xA4;
	memory[0xFD]= 0x20;
	memory[0xFE]= 0x01;
	memory[0xFF]= 0x03;
		
	while(generate == 0){

	}

	wiimote_gen_key();
	

	while (1){
		memory[0x00]=0xC0 | 32;
		memory[0x01]=0xC0 | 32;
		memory[0x02]=0x00;
		memory[0x03]=0x00;
		memory[0x04]=0xFF;
		#ifdef remoteControlled
			memory[0x05] = serRx();
			if (bit_is_set(memory[0x05],1)){
				memory[0x04] |= 1<<6;
			}
			else{
				memory[0x04] &= 0xFF ^ (1<<6);
				
			}
			memory[0x05] |= (1<<1);
			serTx (memory [0x04]);
		#else
			memory[0x05]=PIND;
		#endif
	}
}
