ISR (TWI_vect){

	/*              //Takes too long, screws up TWI
	serTx (TWSR);
	serTx (TWDR);
	//*/
	TWIBuffer [bufferMax] = TWSR;
	bufferMax += 1;
	TWIBuffer [bufferMax] = TWDR;
	bufferMax += 1;
	//cli();   	//Turn interrupts off	
	//Static cast the bit masks to save space
	switch (TWSR&0xF8){		//Mask the prescaler bits to 0
	
	//SLAVE RECEIVER MODES

	case 0x60:  			//Received SLA+W packet, ACK returned
		//TWCR &= 0xFF^(1<<TWSTO);
		//TWCR |= 1<<TWEA | 1 << TWINT;
		//if (mode != 0)cbi(TWCR,TWEA);
		break;
	case 0x68:
		break;
	case 0x70:				//General Call received, ACK returned
		break;
	case 0x78:
		break;
	case 0x80:				//Byte received, ACK returned
		inBuffer [newData++] = TWDR;
		//TWCR &= (0xFF^(1<<TWSTO));
		//TWCR |= 1<<TWEA | 1 << TWINT;
		break;
	case 0x88:				//Byte received, NACK returned
		inBuffer [newData - 1] = TWDR;
		//TWCR &= (0xFF^(1<<TWSTO|1<<TWSTA));
		//TWCR |= 1<<TWEA;
		break;
	case 0x90:
		break;
	case 0x98:
		break;
	case 0xA0:								//Stop or repeated start condition received
		TWCR &= (0xFF^(1<<TWSTO|1<<TWSTA));
		//TWCR |= 1<<TWEA | 1 << TWINT;
		break;

	//SLAVE TRANSMITTER MODES

	case 0xA8:
		nextOut = 1;
		TWDR = outBuffer[0];
		//TWCR &= (0xFF^(1<<TWSTO));
		//TWCR |= 1<<TWEA;
		break;
	case 0xB0:
		TWDR = outBuffer [nextOut++];
		//TWCR |= 1<<TWEA;
		break;
	case 0xB8:
		TWDR = outBuffer [nextOut++];	
		//TWCR |= 1<<TWEA;
		break;
	case 0xC0:
		//TWCR &= (0xFF^(1<<TWSTA|1<<TWSTO));
		//TWCR |= 1<<TWEA;
		break;
	case 0xC8:
		//TWCR &= (0xFF^(1<<TWSTA|1<<TWSTO));
		//TWCR |= 1<<TWEA;
		break;
	}
	TWCR = _BV(TWEN) | _BV(TWIE) | _BV(TWINT) | _BV(TWEA);
	//TWCR |= (1 << TWEA) | (1 << TWINT);
}
