#ifndef config_inc

// program configuration

//#define SerDebug
#define ReadConfig

#ifdef __AVR_ATmega32__
#else
#define SerDebug
#define ReadConfig
#define USE_KML
#endif

// default logging delay and timezone
#define defaultTimeZone -4
#define defaultDelay 2

#define config_inc
#endif
