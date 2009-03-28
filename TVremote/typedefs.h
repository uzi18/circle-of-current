#define POWER	0
#define MUTE	1
#define VOLUP	2
#define VOLDOWN	3
#define CHUP	4
#define CHDOWN	5
#define SOURCE	6

struct code{
	uint8_t size;    //in bits NOT bytes, some codes do not send a multiple of 8 bits.
	uint8_t data[6];
};


//A burst pair is two numbers, the first represents on time, the second off time.
//Each time lasts n cycles of the frequency
struct burstPair{
	uint16_t on;
	uint16_t off;
};


struct codeSet{
	uint16_t frequency; //This is not the actual frequency! It is a timer value which is more appropriate.
	struct burstPair bits [2];
	struct code functions[7]; //store the codes in an array instead of many code fields. See above definitions for the index of each code.
	/*
	struct code power;    
	struct code mute;
	struct code volUp;
	struct code volDown;
	struct code chUp;
	struct code chDown;
	struct code source;
	//*/
	uint8_t leadInSize;
	uint8_t leadSize;    //Store total lead size instead of lead out size to avoid calculations. (For loop i=0,i<in and i=in,i<total)
	struct burstPair leadData[]; 
};
/*
size of a codeSet struct:
frequency:     2*1=2
bits           2*2=4
codes          9*n=63 (assuming n=7, the number of functions I intend to use)
leadInSize     1*1=1
leadSise       1*1=1
burstPairLarge 4*x=(8-64)
total          (16-72) + 9*n (Depends on the lead data, unique for each tv, and how many functions I want to use)
*/
