#ifndef stringword_inc

/* string structure, makes it easier to pass strings around */
typedef struct _StringWord
{
	unsigned char c[15];
	unsigned char length;
	unsigned char endOfFile;
} StringWord;

#define stringword_inc
#endif
