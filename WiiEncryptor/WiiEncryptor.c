#define F_CPU 10000000UL

#define nop asm volatile ("nop"); 
#define ID1 0x01
#define ID2 0x03

#define bufferSize 16
#include <avr/io.h>

#define USART_BAUDRATE 38400
#define BAUD_PRESCALE (((F_CPU / (USART_BAUDRATE * 16UL))) - 1) 

#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit)) // clear bit
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit)) // set bit

#include <avr/eeprom.h> 
#include <stdio.h>
#include <avr/interrupt.h>

typedef struct {
	uint8_t ft[8];
	uint8_t sb[8];
} wiimote_key;


#include <util/delay.h>
#include <avr/iom168.h>
#include <avr/pgmspace.h> 
#include <string.h>

volatile uint16_t maxBuf = 0;
uint8_t d[16];
volatile uint8_t inBuffer [bufferSize];
uint8_t tempByte;
volatile uint16_t address=0;
volatile uint8_t outBuffer[16]={0,0,0xA4,0x20,ID1,ID2,0,0,0,0,0,0,0,0,0};  //Gotta format this right
volatile uint8_t nextOut=0;
volatile uint16_t mode = 0;
//volatile uint8_t handshake=0;
uint8_t count=0;

wiimote_key k;
// this is the bytes writte, starting at address 0x20
uint8_t d[16];// = {0x99, 0x7f, 0x0c, 0x95, 0x4b, 0xdd, 0xb4, 0x1c, 0xf3, 0xee, 0x44, 0x58, 0x19, 0x81, 0x50, 0x76};
// the case that old homebrew uses prior to the discovery of 0xf0
//uint8_t d[] = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};


volatile uint8_t newData = 0;

uint8_t TWIBuffer [500];
uint16_t bufferIndex,bufferMax = 0;

void serTx(unsigned char data)
{
	loop_until_bit_is_set(UCSR0A, UDRE0); // wait while previous tx is finished
	UDR0 = data; // tx
}

#include "constants.h"
#include "cryptoFunctions.h"
#include "handleTWI.h"

void setup ()
{	
	//*
	UCSR0B |= (1 << RXEN0) | (1 << TXEN0);
	UCSR0C |= (1 << UCSZ00) | (1 << UCSZ01);
	UBRR0L = BAUD_PRESCALE; 
	UBRR0H = (BAUD_PRESCALE >> 8);
	//*/

	//Setup TWI
	MCUCR &= 0xFF^1<<PUD;
	//PORTC = 0xFF;	//Enable all pull ups
	PRR &= 0xFF^1<<PRTWI;//Set PRTWI bit in power reduction register to 0
	TWAR = (0x52<<1) | (1<<TWGCE); //Set slave address to 0x52 (And skip the TWGCE bit)
	//TWAMR = 0xFF;
    TWCR = (1<<TWEA) | (1<<TWIE) | (1<<TWEN);	//Setup control register (page 238 reference)

	
	DDRD = 0x00;
	PORTD = 0x00;	

	DDRB = 0x00;
	PORTB = 0xFF; 
	SREG |= 1<<7;   //Enable interrupts
}

int main()
{
	setup();
	for (int i = 0; i < 10; i++){
		serTx(i);
	}
		
			
	//for (int8_t i = 15; i >= 0; i--){
//		d [i] = inBuffer[i];     //Is this the right order? Backwards?
	//}

	//wiimote_gen_key(&k, d);
	//wiimote_encrypt(&k, 16, d, 0xa40008);
	while (1){
		if (bufferIndex < bufferMax){
				serTx (TWIBuffer [bufferIndex]);
				bufferIndex += 1;
		}
		//Everything else should be taken care of, format and encrypt output data here
	}
}
