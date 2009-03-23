#ifndef trig_tbl_inc
#define trig_tbl_inc

#include <avr/pgmspace.h>
#include "config.h"

#ifdef use_atan

#define atan_multiplier 20
const signed long atan_tbl [atan_multiplier + 1] PROGMEM = {
	     0,  28624,  57106,  85308, 113099, 
	140362, 166992, 192900, 218014, 242277, 
	265651, 288108, 309638, 330239, 349920, 
	368699, 386598, 403645, 419872, 435312, 
	450000, 
};

#endif

#ifdef use_asin

#define asin_multiplier 40
const signed long asin_tbl [asin_multiplier + 1] PROGMEM = {
	     0,  14325,  28660,  43012,  57392, 
	 71808,  86269, 100787, 115370, 130029, 
	144775, 159620, 174576, 189656, 204873, 
	220243, 235782, 251507, 267437, 283594, 
	300000, 316682, 333670, 350996, 368699, 
	386822, 405416, 424542, 444270, 464688, 
	485904, 508050, 531301, 555885, 582117, 
	610450, 641581, 676684, 718051, 771614, 
	900000, 
};

#endif

#endif
