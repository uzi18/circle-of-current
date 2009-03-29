#ifndef stringword_inc

/* string structure, makes it easier to pass strings around */
typedef struct _StringStruct
{
	unsigned char c[15];
	unsigned char length;
	unsigned char endOfFile;
} StringStruct;

#define stringword_inc
#endif
