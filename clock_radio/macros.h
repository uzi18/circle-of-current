#ifndef macros_inc

/* old macros */

// Macros
#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit)) // clear bit
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit)) // set bit
#define tog(sfr, bit) (_SFR_BYTE(sfr) ^= _BV(bit)) // toggle bit
#define wdr() __asm__ __volatile__ ("wdr") // watchdog reset

#define macros_inc
#endif
