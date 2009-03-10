#ifndef stringword_inc

/* string structure, makes it easier to pass strings around */
typedef struct _StringWord
{
	unsigned char c[25];
	unsigned char length;
	unsigned char endOfFile;
} StringWord;

typedef struct _FileName83
{
	unsigned char n[8+1];
	unsigned char e[1+3+1];
	unsigned char a[8+1+3+1];
	unsigned char p[25];
	unsigned char nLen;
	unsigned char eLen;
	unsigned char aLen;
	unsigned char pLen;
} FileName83;

#define stringword_inc
#endif
