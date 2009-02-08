#include "twi.h"

static void (*twi_onTx)(void);
static void (*twi_onRx)(unsigned char *, unsigned char);

static volatile unsigned char twi_txMem[16];
static volatile unsigned char twi_txMemIndex;
static volatile unsigned char twi_txMemLength;

static volatile unsigned char twi_rxMem[16];
static volatile unsigned char twi_rxMemIndex;

static volatile unsigned char twi_was_receiver_flag;

void twi_slave_init(unsigned char addr)
{
	// ready bus
	cbi(twi_ddr, twi_scl_pin);
	cbi(twi_ddr, twi_sda_pin);
	
	// set address
	TWAR = addr << 1;

	// enable twi module, acks, and twi interrupt
	TWCR = _BV(TWEN) | _BV(TWIE) | _BV(TWEA);
	sei();

	// initialize flag
	twi_was_receiver_flag = 0;
}

void twi_transmit(unsigned char * data, unsigned char length)
{
	// set length and copy data into tx buffer
	twi_txMemLength = length;
	memcpy(twi_txMem, data, 16);
	twi_txMemIndex = 0;
}

// link events to functions
void twi_attachRxEvent(void (* function)(unsigned char *, unsigned char))
{
	twi_onRx = function;
}

void twi_attachTxEvent(void (* function)(void))
{
	twi_onTx = function;
}


void twi_sendAck(unsigned char ack)
{
	// transmit master read ready signal, with or without ack
	if(ack != 0)
	{
		TWCR = _BV(TWEN) | _BV(TWIE) | _BV(TWINT) | _BV(TWEA);
	}
	else
	{
		TWCR = _BV(TWEN) | _BV(TWIE) | _BV(TWINT);
	}
}

#ifdef USE_FAKEMASTER
// these functions keep the bus busy so the AVR has more time to process
void twi_fakeMaster()
{
	// stop twi module and issue fake start condition
	TWCR = 0;
	sbi(twi_ddr, twi_sda_pin);
	// delay
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();
	sbi(twi_ddr, twi_scl_pin);
}

void twi_fakeMasterOff()
{
	// fake stop condition
	cbi(twi_ddr, twi_scl_pin);
	// delay
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();
	nop(); nop(); nop(); nop();
	cbi(twi_ddr, twi_sda_pin);
	// re-enable twi module
	TWCR = _BV(TWEN) | _BV(TWIE) | _BV(TWEA);
}
#endif

ISR(TWI_vect)
{
	switch(TW_STATUS)
	{
		// Slave Receiver
		case TW_SR_SLA_ACK: // addressed, returned ack
		case TW_SR_GCALL_ACK: // addressed generally, returned ack
		case TW_SR_ARB_LOST_SLA_ACK: // lost arbitration, returned ack
		case TW_SR_ARB_LOST_GCALL_ACK: // lost arbitration, returned ack
			// ready rx buffer and ack
			twi_rxMemIndex = 0;
			twi_sendAck(1);
			break;
		case TW_SR_DATA_ACK: // data received, returned ack
		case TW_SR_GCALL_DATA_ACK: // data received generally, returned ack
			// put in buffer and ack
			twi_rxMem[twi_rxMemIndex] = TWDR;
			twi_rxMemIndex++;
			twi_sendAck(1);
			break;
		case TW_SR_STOP: // stop or repeated start condition
			#ifdef USE_FAKEMASTER
			// ack future responses
			twi_sendAck(1);
			// keep bus busy until user application finishes
			twi_fakeMaster();
			// user application
			twi_onRx(twi_rxMem, twi_rxMemIndex);
			// free the bus
			twi_fakeMasterOff();
			#else
			// user application
			twi_onRx(twi_rxMem, twi_rxMemIndex);
			// ack future responses
			twi_sendAck(1);
			#endif
			// set flag
			twi_was_receiver_flag = 1;
			break;
		case TW_SR_DATA_NACK: // data received, returned nack
		case TW_SR_GCALL_DATA_NACK: // data received generally, returned nack
			// nack
			twi_sendAck(0);
			break;
	
		// Slave Transmitter
		case TW_ST_SLA_ACK: // addressed, returned ack
		case TW_ST_ARB_LOST_SLA_ACK: // arbitration lost, returned ack
			if(twi_was_receiver_flag != 0)
			{				
				// load into buffer
				twi_onTx();
				// reset flag since now transmitter
				twi_was_receiver_flag = 0;
			}
		case TW_ST_DATA_ACK: // byte sent, ack returned
			// copy data to output
			TWDR = twi_txMem[twi_txMemIndex];
			twi_txMemIndex++; 
			// if there is more to send, ack, otherwise nack
			if(twi_txMemIndex < twi_txMemLength)
			{
				twi_sendAck(1);
			}
			else
			{
				twi_sendAck(0);
			}
			break;
		case TW_ST_DATA_NACK: // received nack
		case TW_ST_LAST_DATA: // received ack
			// ack future responses
			twi_sendAck(1);
			break;
	}
}
