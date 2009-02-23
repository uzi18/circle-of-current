ISR (TWI_vect){
	uint8_t data;

	switch (TWSR&0xF8){		//Mask the prescaler bits to 0
	
	//SLAVE RECEIVER MODES

	case 0x60:  			//Received SLA+W packet, ACK returned
		break;
	case 0x68:
		break;
	case 0x70:				//General Call received, ACK returned
		break;
	case 0x78:
		break;
	case 0x80:				//Byte received, ACK returned
		if (newData){
			memory[address] = TWDR;
			address += 1;
		}
		else {
			address = TWDR;
			newData = 1;
		}
		break;
	case 0x88:				//Byte received, NACK returned
		break;
	case 0x90:
		break;
	case 0x98:
		break;
	case 0xA0:								//Stop or repeated start condition received
		newData = 0;
		if (address == 0x50){
			//wiimote_gen_key();
			generate = 1;
		}
		break;

	//SLAVE TRANSMITTER MODES

	case 0xA8:	//SLA+R has been received, send a data byte
		TWDR = encryptByte(memory[address]);
		address += 1;		
		break;
	case 0xB0:	//SLA+R has previously been received, send a data byte
		TWDR = encryptByte(memory[address]);
		address += 1;	
		break;
	case 0xB8:	//SLA+R has previously been received, send a data byte
		TWDR = encryptByte(memory[address]);
		address += 1;		
		break;
	case 0xC0:	//Done sending data
		newData = 0;
		break;
	case 0xC8:	//Done sending data
		newData = 0;
		break;
	}
	TWCR = _BV(TWEN) | _BV(TWIE) | _BV(TWINT) | _BV(TWEA);
	//TWCR |= (1 << TWEA) | (1 << TWINT);
}
