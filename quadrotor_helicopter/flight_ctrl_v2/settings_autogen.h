#ifndef settings_autogen_h_inc
#define settings_autogen_h_inc

// define default values

#define calculation_mode_def_val 0

// define addresses

#define calculation_mode_addr 0

// make union

typedef union saved_params_s_ {
	struct {
		double calculation_mode;
	} d_s;

	struct {
		signed long calculation_mode;
	} sl_s;

	struct {
		unsigned long calculation_mode;
	} ul_s;

	double d_val[1];
	signed long sl_val[1];
	unsigned long ul_val[1];
	unsigned char c[4];
} saved_params_s;

void params_set_default();

#ifdef COMING_FROM_SAVE_C

static saved_params_s saved_params;

// set default

void params_set_default()
{
	saved_params.d_s.calculation_mode = calculation_mode_def_val;
}

#endif



#endif

