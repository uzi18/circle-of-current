#ifndef settings_autogen_h_inc
#define settings_autogen_h_inc

// define default values

#define dhgsdfgsdfg_def_val 2345
#define sdfgsdgsdfgsdfgs_def_val 2345
#define sdfgsdfg_def_val 2345
#define sdfgsdfgsdgds_def_val 2345
#define _blank_name_4_def_val 0

// define addresses

#define dhgsdfgsdfg_addr 0
#define sdfgsdgsdfgsdfgs_addr 1
#define sdfgsdfg_addr 2
#define sdfgsdfgsdgds_addr 3
#define _blank_name_4_addr 4

// make union

typedef union saved_params_s_ {
	struct {
		double dhgsdfgsdfg;
		double sdfgsdgsdfgsdfgs;
		double sdfgsdfg;
		double sdfgsdfgsdgds;
		double _blank_name_4;
	} d_s;

	struct {
		signed long dhgsdfgsdfg;
		signed long sdfgsdgsdfgsdfgs;
		signed long sdfgsdfg;
		signed long sdfgsdfgsdgds;
		signed long _blank_name_4;
	} sl_s;

	struct {
		unsigned long dhgsdfgsdfg;
		unsigned long sdfgsdgsdfgsdfgs;
		unsigned long sdfgsdfg;
		unsigned long sdfgsdfgsdgds;
		unsigned long _blank_name_4;
	} ul_s;

	double d_val[5];
	signed long sl_val[5];
	unsigned long ul_val[5];
	unsigned char c[20];
} saved_params_s;

void params_set_default();

#ifdef COMING_FROM_SAVE_C

static saved_params_s saved_params;

// set default

void params_set_default()
{
	saved_params.d_s.dhgsdfgsdfg = dhgsdfgsdfg_def_val;
	saved_params.d_s.sdfgsdgsdfgsdfgs = sdfgsdgsdfgsdfgs_def_val;
	saved_params.d_s.sdfgsdfg = sdfgsdfg_def_val;
	saved_params.d_s.sdfgsdfgsdgds = sdfgsdfgsdgds_def_val;
	saved_params.d_s._blank_name_4 = _blank_name_4_def_val;
}

#endif



#endif

