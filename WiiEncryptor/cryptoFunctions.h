static inline uint8_t ror8(uint8_t a, uint8_t b) {
	return (a>>b) | ((a<<(8-b))&0xff);
}

void genkey(uint8_t *rand, uint8_t idx, uint8_t *key)
{
	uint8_t ans[6];
	for (int i=0;i<6;i++){
		ans[i] = pgm_read_byte(&ans_tbl[idx][i]);
	}
	uint8_t t0[10];
	int i;
	
	for(i=0;i<10;i++)
		t0[i] = pgm_read_byte(tsbox[rand[i]]);

	key[5] = ((ror8((ans[5]^t0[4]),(t0[7]%8)) - t0[0]) ^ t0[5]);
	key[4] = ((ror8((ans[4]^t0[8]),(t0[9]%8)) - t0[4]) ^ t0[2]);
	key[3] = ((ror8((ans[3]^t0[3]),(t0[1]%8)) - t0[7]) ^ t0[9]);
	key[2] = ((ror8((ans[2]^t0[5]),(t0[2]%8)) - t0[6]) ^ t0[7]);
	key[1] = ((ror8((ans[1]^t0[8]),(t0[3]%8)) - t0[6]) ^ t0[5]);
	key[0] = ((ror8((ans[0]^t0[2]),(t0[1]%8)) - t0[4]) ^ t0[0]);
}

uint8_t gen_ft_sb (uint8_t idx,uint8_t a,uint8_t b){
	return (pgm_read_byte(&sboxes[idx][a])^pgm_read_byte(&sboxes[(idx+1)%8][b]));
}

void gentabs(uint8_t *key, uint8_t idx, uint8_t *ft, uint8_t *sb)
{
	//Instead of using key and rand, I combined them into one for this fucntion. 
	//This reduces code by about 300 bytes since I can now use 1 for loop with 4 8 byte arrays.

	//Potential bug: I may not have the correct order, I think I do, but I may not. Will find out when ready for testing.


	uint8_t a[8] = {11,13,10,15,14,12,9,8};
	uint8_t b[8] = {6,4,2,7,5,0,3,1};
	uint8_t c[8] = {15,10,12,13,11,14,6,7};
	uint8_t d[8] = {8,5,9,0,2,1,4,3};
	for (int i = 0;i<8;i++){
		ft[i] = gen_ft_sb(idx,a[i],b[i]);
		sb[i] = gen_ft_sb(idx,c[i],d[i]);
	}    

//*/

}

/* generate a ft and sb, given the deviceside keybuffer (rand+key) */
void wiimote_gen_key(wiimote_key *key, uint8_t *keydata)
{
	uint8_t rand[10];
	uint8_t skey[6];
	uint8_t testkey[6];
	int idx = 0;
	int i;

	for(i=0;i<10;i++)
		rand[i] = keydata[i];
	for(i=0;i<6;i++)
		skey[i] = keydata[i+10];
	
	//*

	for(idx=0;idx<7;idx++) {
		genkey(rand, idx, testkey);
		if(!memcmp(testkey,skey,6))
			break;
	}
	//*/

	// default case is idx = 7 which is valid (homebrew uses it for the 0x17 case)
	//printf("idx:  %d\n", idx);
	
	gentabs(keydata, idx, key->ft, key->sb);
	
	// for homebrew, ft and sb are all 0x97 which is equivalent to 0x17
}

void wiimote_encrypt(wiimote_key *key, int len, uint8_t *data, uint32_t addr)
{
	int i;
	for(i=0; i<len; i++, addr++) {
		data[i] = (data[i] - key->ft[addr%8]) ^ key->sb[addr%8];
	}
}


