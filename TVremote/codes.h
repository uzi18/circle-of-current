 #define POWER 0
 #define MUTE 1
 #define VOLUP 2
 #define VOLDOWN 3
 #define CHUP 4
 #define CHDOWN 5
 #define SOURCE 6

#define PRESCALER 1

#if PRESCALER == 1
#define PRESCALER_BITS 1
#else 
	#if PRESCALER = 8
	#define PRESCALER_BITS 2
	#else 
		#if PRESCALER = 64
		#define PRESCALER_BITS 3 
		#else 
			#if PRESCALER = 256
			#define PRESCALER_BITS 4
			#else 
				#if PRESCALER = 1024
					#define PRESCALER_BITS 5

				#else
					#error "Invalid prescaler"
				#endif
			#endif
		#endif
	#endif
#endif

#define frequencyToTimerValue(x) (((F_CPU)/(2*PRESCALER*x))-1)

volatile const struct codeSet apex  = {  //This will be stored in progmem/external flash later, just using normal SRAM for now to simplify code
	frequencyToTimerValue(37470),
	{{0x16,0x15},
	 {0x16,0x41}},
	{{32,{0x02,0xFD,0x48,0xB7,0x00,0x00}},//Power
	 {32,{0x02,0xFD,0x48,0xB7,0x00,0x00}},//Mute These codes are just placeholders for now
	 {32,{0x02,0xFD,0x48,0xB7,0x00,0x00}},//VolUp
	 {32,{0x02,0xFD,0x48,0xB7,0x00,0x00}},//volDown
	 {32,{0x02,0xFD,0x48,0xB7,0x00,0x00}},//chUp
	 {32,{0x02,0xFD,0x48,0xB7,0x00,0x00}},//chDown
	 {32,{0x02,0xFD,0x48,0xB7,0x00,0x00}}},//source
	1,
	6,
	{{346,174},{22,1519},{347,86},{22,3702},{347,86},{22,0}}
};

volatile const struct codeSet *codeSetList[] = {&apex};
