#ifndef macros_h_inc
#define macros_h_inc

#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit)) // clear bit
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit)) // set bit
#define tog(sfr, bit) (_SFR_BYTE(sfr) ^= _BV(bit)) // toggle bit

#endif
