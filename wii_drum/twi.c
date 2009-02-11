#include "twi.h"

// pointers to user functions
static void (*twi_tx_start_event)(unsigned char);
static void (*twi_tx_end_event)(unsigned char, unsigned char);
static void (*twi_rx_event)(unsigned char, unsigned char);

// virtual register
static volatile unsigned char twi_reg[256];
static volatile unsigned char twi_reg_addr;

static volatile unsigned char twi_first_addr_flag; // set address flag
static volatile unsigned char twi_rw_len; // length of most recent operation

void twi_slave_init(unsigned char addr)
{
	// initialize stuff
	twi_reg_addr = 0;
	for(unsigned int i = 0; i < 256; i++) twi_reg[i] = 0;	

	// set slave address
	TWAR = addr << 1;
	
	// enable twi module, acks, and twi interrupt
	TWCR = _BV(TWEN) | _BV(TWIE) | _BV(TWEA);
}

// access functions for virtual register
void twi_set_reg(unsigned char addr, unsigned char d)
{
	twi_reg[addr] = d;
}

unsigned char twi_read_reg(unsigned char addr)
{
	return twi_reg[addr];
}

// event linking functions
void twi_attach_rx_event( void (*function)(unsigned char, unsigned char) )
{
	twi_rx_event = function;
}

void twi_attach_tx_start( void (*function)(unsigned char) )
{
	twi_tx_start_event = function;
}

void twi_attach_tx_end( void (*function)(unsigned char, unsigned char) )
{
	twi_tx_end_event = function;
}

void twi_clear_int(unsigned char ack)
{
	// get ready by clearing interrupt, with or without ack
	if(ack != 0)
	{
		TWCR = _BV(TWEN) | _BV(TWIE) | _BV(TWINT) | _BV(TWEA);
	}
	else
	{
		TWCR = _BV(TWEN) | _BV(TWIE) | _BV(TWINT);
	}
}

ISR(TWI_vect)
{
	switch(TW_STATUS)
	{
		// Slave Rx
		case TW_SR_SLA_ACK: // addressed, returned ack
		case TW_SR_GCALL_ACK: // addressed generally, returned ack
		case TW_SR_ARB_LOST_SLA_ACK: // lost arbitration, returned ack
		case TW_SR_ARB_LOST_GCALL_ACK: // lost arbitration generally, returned ack
			// get ready to receive pointer
			twi_first_addr_flag = 0;
			// ack
			twi_clear_int(1);
			break;
		case TW_SR_DATA_ACK: // data received, returned ack
		case TW_SR_GCALL_DATA_ACK: // data received generally, returned ack
		if(twi_first_addr_flag != 0)
		{
			// put byte in register
			twi_reg[twi_reg_addr++] = TWDR;
			twi_rw_len++;
		}
		else
		{
			// set address
			twi_reg_addr = TWDR;
			twi_first_addr_flag = 1;
			twi_rw_len = 0;
		}
		twi_clear_int(1); // ack
			break;
		case TW_SR_STOP: // stop or repeated start condition received
			// run user defined function
			twi_rx_event(twi_reg_addr - twi_rw_len, twi_rw_len);
			twi_clear_int(1); // ack future responses
			break;
		case TW_SR_DATA_NACK: // data received, returned nack
		case TW_SR_GCALL_DATA_NACK: // data received generally, returned nack
			twi_clear_int(0); // nack back at master
			break;
		
		// Slave Tx
		case TW_ST_SLA_ACK:	// addressed, returned ack
		case TW_ST_ARB_LOST_SLA_ACK: // arbitration lost, returned ack
			// run user defined function
			twi_tx_start_event(twi_reg_addr);
			twi_rw_len = 0;
		case TW_ST_DATA_ACK: // byte sent, ack returned
			// ready output byte
			TWDR = twi_reg[twi_reg_addr++];
			twi_rw_len++;
			twi_clear_int(1); // ack
			break;
		case TW_ST_DATA_NACK: // received nack, we are done 
		case TW_ST_LAST_DATA: // received ack, but we are done already!
			// ack future responses
			twi_clear_int(1);
			// run user defined function
			twi_tx_end_event(twi_reg_addr - twi_rw_len, twi_rw_len);
			break;
	}
}

