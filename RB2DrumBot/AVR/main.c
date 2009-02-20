#include "main.h"

// TFF needs this
unsigned long get_fattime()
{
	return 0;
}

// waits for transmitter to not be busy then tx a byte
void ser_tx(unsigned char data)
{
	UDR0 = data; // tx
	while(bit_is_clear(UCSR0A, TXC0));
	sbi(UCSR0A, TXC0);
}

// waits for byte to be received
unsigned char ser_rx_wait()
{
	while(bit_is_clear(UCSR0A, RXC0));
	unsigned char d = UDR0;
	return d;
}

// return a byte received if new, or else return -1
signed int ser_rx_nowait()
{
	if(bit_is_set(UCSR0A, RXC0))
	{
		return (signed int)UDR0;
	}
	else
	{
		return -1;
	}
}

// start UART hardware
void ser_init(unsigned long baudRate)
{
	unsigned int temp = getBaudRate(baudRate);
	
	UBRR0H = (temp & 0xFF00) >> 8;
	UBRR0L = temp & 0xFF;

	UCSR0B = 0; // reset just to be sure

	sbi(uart_port, uart_rx_pin); // pull up
	
	sbi(UCSR0B, RXEN0); // enable rx and tx now
	sbi(UCSR0B, TXEN0);
}

// instruction structure
typedef struct _instruct
{
	unsigned int delay;
	unsigned char bitmask;
	unsigned char special;
} instruct;

// amount of instructions to buffer
#define instruct_buff_size 10

// circular FIFO buffer to store instructions
typedef struct _instruct_buff
{
	instruct i[instruct_buff_size];
	unsigned char head; // where to read from
	unsigned char tail; // where to write to
} instruct_buff;

static volatile instruct cur_instruct;
static volatile instruct_buff instruct_buffer;
static volatile unsigned char status;
FIL inst_fil;

void read_instruct()
{
	unsigned char data[5];
	
	f_read(&inst_fil, &data, 4, &data[4]); // read from file
	
	if(data[4] != 0) // if not end of file
	{
		// place into buffer
		instruct_buffer.i[instruct_buffer.tail].special = data[0];
		instruct_buffer.i[instruct_buffer.tail].delay = ((unsigned int)data[2] << 8);
		instruct_buffer.i[instruct_buffer.tail].delay += data[1];
		instruct_buffer.i[instruct_buffer.tail].bitmask = data[3];
		
		// increment index for next read_instruct
		instruct_buffer.tail++;
		instruct_buffer.tail %= instruct_buff_size;
	}
}

void get_instruct()
{
	// read next instruction
	cur_instruct = instruct_buffer.i[instruct_buffer.head];
	
	// increment index for next get_instruct
	instruct_buffer.head++;
	instruct_buffer.head %= instruct_buff_size;
}

unsigned char instruct_buff_length()
{
	// returns the number of instructions in the buffer
	return (instruct_buff_size + instruct_buffer.tail - instruct_buffer.head) % instruct_buff_size;
}

void exe_instruct()
{
	if(cur_instruct.special == 2) // 2 indicates "wait for button to start"
	{ 
		// stop everything
		drum_out_port = 0xFF;
		TCCR1B = 0b00000000;
		TCNT1 = 0;
		cbi(TIMSK1, OCIE1A);
		while(bit_is_set(button_input_reg, button_pin)) // wait for button
		{
			cbi(LED_port, LED_pin);
		}
		sbi(LED_port, LED_pin);
	}
	drum_out_port = cur_instruct.bitmask; // drum action
	if(cur_instruct.special == 3) // 3 means last instruction
	{
		ser_tx(8); // stop timer
		// stop everything
		TCCR1B = 0b00000000;
		cbi(TIMSK1, OCIE1A);
		status = 1; // go to idle state
	}
	else
	{
		if(cur_instruct.special == 2)
		{
			ser_tx(7); // start timer
		}
		// set delay for next instruction
		OCR1A = cur_instruct.delay;
		OCR1B = cur_instruct.delay / 2;
		// run timer, interrupt on
		TCCR1B = 0b00000101;
		sbi(TIMSK1, OCIE1A);
		get_instruct(); // read the next instruction
		status = 0; // working state
	}
}

ISR(TIMER1_COMPA_vect)
{
	TCNT1 = 0; // reset timer for next delay
	exe_instruct(); // execute next instruction
}

ISR(TIMER1_COMPB_vect)
{
	drum_out_port = 0xFF; // clear drum hit
}

int main()
{
	sei(); // interrupts on

	unsigned char f_err; // filesystem error flag

	// initialize ports
	ser_init(57600);
	
	sbi(LED_port, LED_pin);
	sbi(LED_ddr, LED_pin);

	cbi(button_ddr, button_pin);
	sbi(button_port, button_pin);

	drum_out_port = 0xFF;
	drum_out_ddr = 0xFF;

	sbi(TIMSK1, OCIE1B);

	// initialize flash memory
	FATFS fatfs_;
	disk_initialize(0);
	f_err = f_mount(0, &fatfs_);
	
	while(f_err == 0) // main loop only if disk is ready
	{
		cbi(LED_port, LED_pin);
		
		// stop timer
		TCCR1B = 0b00000000;
		TCNT1 = 0;
		
		ser_tx(1); // notify computer, ready
		
		// read file name		
		unsigned char fn[14] = {'/', 0, 0, 0, 0, 0, 0, 0, 0, '.', 'b', 'i', 'n', 0};		
		for(unsigned char i = 1; i <= 8; i++)
		{
			fn[i] = ser_rx_wait();
		}
		
		// get command
		unsigned char cmd = ser_rx_wait();
		
		if(cmd == 1) // if write to file
		{		
			f_err = f_open(&inst_fil, fn, FA_WRITE | FA_CREATE_ALWAYS); // create file if not existing
			
			if(f_err == 0)
			{
				sbi(LED_port, LED_pin);
			
				ser_tx(2); // notify computer,  file creation succeeded
				
				// retrieve length of file
				unsigned int length_of_file = ser_rx_wait();
				length_of_file += ((unsigned int)ser_rx_wait()) << 8;
				
				for(unsigned int i = 0; i < length_of_file; i++)
				{
					ser_tx(3); // request next bytes
				
					unsigned char data[5];
					
					data[0] = ser_rx_wait();
					data[1] = ser_rx_wait();
					data[2] = ser_rx_wait();
					data[3] = ser_rx_wait();
					
					// write to file
					f_write(&inst_fil, &data, 4, &data[4]);
				}
				
				// close file, notify computer of completion
				f_close(&inst_fil);				
				ser_tx(4);
			}
			else
			{
				ser_tx(0xFF); // notify computer of disk error
			}
		}
		else if(cmd == 2) // start playing command
		{
			f_err = f_open(&inst_fil, fn, FA_READ); // open file for reading

			if(f_err == 0)
			{			
				sbi(LED_port, LED_pin);
				
				ser_tx(5); // notify computer of success opening file
				
				// empty buffer
				instruct_buffer.head = 0;
				instruct_buffer.tail = 0;

				// fill buffer until full
				for(unsigned char i = 0; i < instruct_buff_size; i++)
				{
					read_instruct();
				}
				
				// ready first instruction and execute
				get_instruct();
				exe_instruct();
				
				while(status == 0 && ser_rx_nowait() != 3) // exit when not working or commanded to stop
				{
					// fill buffer only if needed and if there is room
					if(instruct_buff_length() < instruct_buff_size)
					{
						read_instruct();
					}
				}
				
				// end of song, close file and notify computer
				f_close($inst_fil);				
				ser_tx(6);
			}
			else
			{
				ser_tx(128); // notify computer of file missing
			}
		}
	}
	
	ser_tx(0xFF); // notify computer of disk error
	
	while(1); // infinite loop, freeze AVR

	return 0;
}
