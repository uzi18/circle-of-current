static inline uint8_t ror8(uint8_t a, uint8_t b) {
	return (a>>b) | ((a<<(8-b))&0xff);
}

/*
uint8_t genKeyElement(uint8_t *t0,uint8_t a,uint8_t b,uint8_t c,uint8_t d,uint8_t e){
	return ((ror8((a^t0[b]),(t0[c]%8)) - t0[d]) ^ t0[e]);	
}
//*/

void genKey(){	//Using arrays to save space here as with genTables does not work (Uses more space), will look into why later.
	uint8_t ans[6];
	uint8_t t0[10];
	//uint8_t a[6]={5,1,6,4,1,7};
	//uint8_t b[6]={2,0,8,7,6,8};
	//uint8_t c[6]={9,5,2,3,3,5};
	//uint8_t d[6]={4,7,0,2,4,9};

	for (uint8_t i=0;i<10;i++){
		t0[i] = pgm_read_byte(&sboxes[0][memory[0x49-i]]);
	}
	for (uint8_t i=0;i<6;i++){
		ans [i] = pgm_read_byte(&ans_tbl[idx][i]);
		//testKey [5-i] = genKeyElement(t0,ans[i],a[i],b[i],c[i],d[i]); /
	}
	//*
	testKey[5] = ((ror8((ans[0]^t0[5]),(t0[2]%8)) - t0[9]) ^ t0[4]);//order may be backwards
	testKey[4] = ((ror8((ans[1]^t0[1]),(t0[0]%8)) - t0[5]) ^ t0[7]);
	testKey[3] = ((ror8((ans[2]^t0[6]),(t0[8]%8)) - t0[2]) ^ t0[0]);
	testKey[2] = ((ror8((ans[3]^t0[4]),(t0[7]%8)) - t0[3]) ^ t0[2]);
	testKey[1] = ((ror8((ans[4]^t0[1]),(t0[6]%8)) - t0[3]) ^ t0[4]);
	testKey[0] = ((ror8((ans[5]^t0[7]),(t0[8]%8)) - t0[5]) ^ t0[9]);
	//*/
}

uint8_t genTableElement(uint8_t a, uint8_t b){
	return pgm_read_byte(&sboxes[idx+1][memory[a]]) ^ pgm_read_byte(&sboxes[idx+2][memory[b]]);
}

void genTables(){
	//
	uint8_t a[8]={0x4B,0x4D,0x4A,0x4F,0x4E,0x4C,0x49,0x48};
	uint8_t b[8]={0x46,0x44,0x42,0x47,0x45,0x40,0x43,0x41};
	uint8_t c[8]={0x4F,0x4A,0x4C,0x4D,0x4B,0x4E,0x46,0x47};
	uint8_t d[8]={0x48,0x45,0x49,0x40,0x42,0x41,0x44,0x43};
	//*/

	/*
	uint8_t a[8]={0x4F-4,0x4F-2,0x4F-5,0x4F-0,0x4F-1,0x4F-3,0x49-0,0x49-1};
	uint8_t b[8]={0x49-3,0x49-5,0x49-7,0x49-2,0x49-4,0x49-9,0x49-6,0x49-8};
	uint8_t c[8]={0x4F-0,0x4F-5,0x4F-3,0x4F-2,0x4F-4,0x4F-1,0x49-3,0x49-2};
	uint8_t d[8]={0x49-1,0x49-4,0x49-0,0x49-9,0x49-7,0x49-8,0x49-5,0x49-6};
	//*/
	for (uint8_t i=0;i<8;i++){
		ft[i]= genTableElement(a[i],b[i]);
		sb[i]= genTableElement(c[i],d[i]);
	}
}

uint8_t equal(){

	for (int i=0;i<6;i++){
		if(testKey[i] != memory[0x4A+i]){
			return 0;
		}
	}
	return 1;
}

void wiimote_gen_key(){
	for (idx=0;idx<7;idx++){
		genKey();
		if(equal()){
			break;
		}
	}	
	genTables();
	generate = 0;
}

uint8_t encryptByte(uint8_t a){
		if (memory[0xF0]== 0xAA){
			a = (a-ft[address%8])^sb[address%8];
		}
	return a;
}
