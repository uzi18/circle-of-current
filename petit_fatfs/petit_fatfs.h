/*

Arduino Wrapper Function Library for Petit FatFs

Petit FatFs - http://elm-chan.org/fsw/ff/00index_p.html
by ChanN

Wrapper Functions by Frank Zhao

mmc.c origially written by ChanN, modified by Frank Zhao

*/

#ifndef PFF_h
#define PFF_h

extern "C" {

#include "integer.h"
#include "pff.h"

}

#include "WProgram.h"

class PFF
{
  private:
    static FATFS fatfs_obj;
    static int MMC_CS;
    static void MMC_SELECT(void);
    static void MMC_DESELECT(void);
  public:
    PFF();
    int begin(int, unsigned char (*)(void), void (*)(unsigned char));
	void buffer_mode();
	void stream_mode();
	int open_file(char *);
	int read_file(void *, int, int *);
	int lseek_file(int);
	int open_dir(DIR *, char *);
	int read_dir(DIR *, FILINFO *);
};

extern PFF PFFS;

#endif

