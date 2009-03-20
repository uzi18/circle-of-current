#ifndef settings_autogen_h_inc
#define settings_autogen_h_inc

// define default values

#define test1_def_val 1
#define wetwet_def_val 2
#define wertwet_def_val 3
#define wetwertw_def_val 4
#define terwtwertwe_def_val 5
#define rtwertwe_def_val 6
#define twetwertw_def_val 7
#define wertewrt_def_val 8

// define addresses

#define test1_addr 0
#define wetwet_addr 1
#define wertwet_addr 2
#define wetwertw_addr 3
#define terwtwertwe_addr 4
#define rtwertwe_addr 5
#define twetwertw_addr 6
#define wertewrt_addr 7

// make union

typedef union saved_params_s_ {
	struct {
		double test1;
		double wetwet;
		double wertwet;
		double wetwertw;
		double terwtwertwe;
		double rtwertwe;
		double twetwertw;
		double wertewrt;
	} d_s;

	struct {
		signed long test1;
		signed long wetwet;
		signed long wertwet;
		signed long wetwertw;
		signed long terwtwertwe;
		signed long rtwertwe;
		signed long twetwertw;
		signed long wertewrt;
	} sl_s;

	struct {
		unsigned long test1;
		unsigned long wetwet;
		unsigned long wertwet;
		unsigned long wetwertw;
		unsigned long terwtwertwe;
		unsigned long rtwertwe;
		unsigned long twetwertw;
		unsigned long wertewrt;
	} ul_s;

	double d_val[8];
	signed long sl_val[8];
	unsigned long ul_val[8];
	unsigned char c[32];
} saved_params_s;

void params_set_default();

#ifdef COMING_FROM_SAVE_C

static saved_params_s saved_params;

// set default

void params_set_default()
{
	saved_params.d_s.test1 = test1_def_val;
	saved_params.d_s.wetwet = wetwet_def_val;
	saved_params.d_s.wertwet = wertwet_def_val;
	saved_params.d_s.wetwertw = wetwertw_def_val;
	saved_params.d_s.terwtwertwe = terwtwertwe_def_val;
	saved_params.d_s.rtwertwe = rtwertwe_def_val;
	saved_params.d_s.twetwertw = twetwertw_def_val;
	saved_params.d_s.wertewrt = wertewrt_def_val;
}

#endif



#endif

