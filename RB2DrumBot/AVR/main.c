#include "main.h"

unsigned long get_fattime()
{
	return 0;
}

typedef struct _instruct
{
	unsigned int delay;
	unsigned char bitmask;
	unsigned char special;
} instruct;

typedef struct _instruct_buff
{
	instruct i[10];
	unsigned char head;
	unsigned char tail;
} instruct_buff;

static volatile instruct curInstruct;
static volatile unsigned char status;

// waits for transmitter to not be busy then tx a byte
void serTx(unsigned char data)
{
	UDR0 = data; // tx
	while(bit_is_clear(UCSR0A, TXC0));
	sbi(UCSR0A, TXC0);
}

unsigned char serRx()
{
	while(bit_is_clear(UCSR0A, RXC0));
	unsigned char d = UDR0;
	return d;
}

void get_instruct()
{
	unsigned char d;
	do
	{
		d = serRx();
	}
	while(d != 0);
	curInstruct.special = serRx();
	curInstruct.delay = ((unsigned int)serRx() << 8);
	curInstruct.delay += serRx();
	curInstruct.bitmask = serRx();
}

void exe_instruct()
{
	if(curInstruct.special == 2)
	{
		while(bit_is_set(button_input_reg, button_pin))
		{
			cbi(LED_port, LED_pin);
			drum_out_port = 0xFF;
			TCCR1B = 0b00000000;
			cbi(TIMSK1, OCIE1A);
		}
		serTx(3);
		sbi(LED_port, LED_pin);
	}
	drum_out_port = curInstruct.bitmask;
	if(curInstruct.special == 3)
	{		
		serTx(4);
		status = 1;
		TCCR1B = 0b00000000;
		cbi(TIMSK1, OCIE1A);
	}
	else
	{
		OCR1A = curInstruct.delay;
		OCR1B = curInstruct.delay / 2;
		TCCR1B = 0b00000101;
		sbi(TIMSK1, OCIE1A);
		status = 0;
		serTx(0);
		get_instruct();
	}
}

ISR(TIMER1_COMPA_vect)
{
	TCNT1 = 0;
	exe_instruct();
}

ISR(TIMER1_COMPB_vect)
{
	drum_out_port = 0xFF;
}

// start UART hardware
void serInit(unsigned long baudRate)
{
	unsigned int temp = getBaudRate(baudRate);
	
	UBRR0H = (temp & 0xFF00) >> 8;
	UBRR0L = temp & 0xFF;

	UCSR0B = 0; // reset just to be sure

	sbi(uart_port, uart_rx_pin); // pull up
	
	sbi(UCSR0B, RXEN0); // enable rx and tx now
	sbi(UCSR0B, TXEN0);
}

FIL inst_fil;

int main()
{
	sei();

	// start serial port
	serInit(57600);

	// initialize ports
	sbi(LED_port, LED_pin);
	sbi(LED_ddr, LED_pin);

	cbi(button_ddr, button_pin);
	sbi(button_port, button_pin);

	drum_out_port = 0xFF;
	drum_out_ddr = 0xFF;

	sbi(TIMSK1, OCIE1B);

	FATFS fatfs_;
	disk_initialize(0);
	f_mount(0, &fatfs_);
	f_open(&inst_fil, "/12345678.bin",FR_READ);

	while(1)
	{
		get_instruct();
	}

	// 1 = waiting for fillup before execution
	status = 1;	

	while(1)
	{
		if(status == 1)
		{
			sbi(LED_port, LED_pin);
			serTx(1);
			get_instruct();
			exe_instruct();
		}
	}

	return 0;
}
