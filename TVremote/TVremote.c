/*
Joshua Shabtai
Jrblast@gmail.com
http://www.josh.circleofcurrent.com

Feel free to contact me with any questions and I will do my
best to help you.

This project is meant to be a remote control for many TVs.
It was inspired in part from Lady Ada's TV-B-gone kit, and 
in part by a similar remote sold on DealExtreme
(http://www.dealextreme.com/details.dx/sku.2724)

It is open source for anyone to use and modify as they wish,
as long as they do not take credit for anything they did not do..
*/

#if F_CPU == NULL
#error "define your clock speed"//Make sure clock speed is defined
#endif

#include <avr/io.h>             // this contains all the IO port definitions
#include <avr/interrupt.h>      // definitions for interrupts
#include <avr/sleep.h>          // definitions for power-down modes
#include <avr/pgmspace.h>       // definitions or keeping constants in program memory

#include "typedefs.h"
#include "codes.h"

#define NOP __asm__ __volatile__ ("nop")

void on(){
	TCNT0 = 0;//reset timer
	PORTB = 0xFF;//Start with LEDs on
	TCCR0A = _BV(COM0A0) | _BV(COM0B0) | _BV(WGM01);//0x52, use CTC mode and toggle OC0A and OC0B
}

void off(){
	TCCR0A = 0x02;//CTC mode, but timer module does not touch PORTB
	PORTB = 0x00;//Ensure PORTB is low
}

void count (uint16_t n){
	for (int k=0;k<1;k++){//Each time i is incremented is only half 
		for (uint16_t i=0;i<n;i++){//count n cycles of reaching TOP
			TIFR |= _BV(OCF0A);	//Clear interrupt flag
			while(bit_is_clear(TIFR,OCF0A)); //wait for intterupt flag
		}
	}
	//Interrupt flag doesn't need to be cleared since it is only used in this function and gets cleared before
}

void sendBurstPair (struct burstPair bp, uint16_t timerValue){
	OCR0A = OCR0B = timerValue;
	on();		//Flash LED at carrier and wait n cycles
	count(bp.on);

	off();//Turn off LED for n cycles
	count (bp.off);
}

int my_bit_is_set(uint8_t data,uint8_t bitNum){//Like bit_is_set but returns 1 or 0, not 0 or non-zero
	if (data & (1<<bitNum)){
		return 1;
	}
	else{
		return 0;
	}
}


void delay (uint16_t dt){
	uint8_t _TCCR0A = TCCR0A;
	uint8_t _TCCR0B = TCCR0B;
	uint16_t count = 0;
	TCCR0A = 0;
	TCCR0B = 3;
	while (count<dt){
		if (TIFR & _BV(TOV0)){
			TCNT0 = 255-125+15;
			count++;
			TIFR |= _BV(TOV0);
		}
	}

	TCCR0A = _TCCR0A;
	TCCR0B = _TCCR0B;
}

void sendCode (struct codeSet *tvType, uint8_t function){
	struct code codeToUse=tvType->functions[function];	
	for (int i=0;i<tvType->leadInSize;i++){
		sendBurstPair(tvType->leadData[i], tvType->frequency);//Send lead in data
	}
	for (int i=0;i<codeToUse.size;i++){
		sendBurstPair(tvType->bits[my_bit_is_set(codeToUse.data[i/8],i%8)], tvType->frequency); //send button code
	}
	for (int i=tvType->leadInSize;i<tvType->leadSize;i++){
		sendBurstPair(tvType->leadData[i], tvType->frequency);	//send leadout data
	}
}

int main (void){
	DDRB = 0xFF;
	PORTB = 0x00;
	on();			//Use CTC mode and toggle OCR0A/B on match
	TCCR0B = 1;
	TIMSK = _BV(OCIE0A) | _BV(OCIE0B);
	while (1){
		sendCode(codeSetList[0],POWER);
		delay(0);
	}
}


/*
Potential bugs:	TV may not read code properly if no delay between codes
				Not completely sure how the codes I got represent the value, I may need to count twice/half as long (Depending on what I'm doing now)
				OCR0A/B may be twice as fast as it should be. I think the macro should be
							
							#define frequencyToTimerValue(x) (((F_CPU)/(4*PRESCALER*x))-1) ) (A 4 instead of a 2)
				
				CKDIV8/CKSEL bits might be set wrong
*/
