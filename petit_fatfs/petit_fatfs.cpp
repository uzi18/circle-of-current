/*

Arduino Wrapper Function Library for Petit FatFs

Petit FatFs - http://elm-chan.org/fsw/ff/00index_p.html
by ChanN

Wrapper Functions by Frank Zhao

mmc.c origially written by ChanN, modified by Frank Zhao

*/

extern "C" {

#include "diskio.h"

}

#include "petit_fatfs.h"

FATFS PFF::fatfs_obj;
int PFF::MMC_CS;

PFF::PFF()
{
}

void PFF::MMC_SELECT()
{
  digitalWrite(MMC_CS, LOW);
}

void PFF::MMC_DESELECT()
{
  digitalWrite(MMC_CS, HIGH);
}

int PFF::begin(int cs_pin, unsigned char (* rx)(void), void (* tx)(unsigned char))
{
  MMC_CS = cs_pin;
  pinMode(MMC_CS, OUTPUT);
  mmc_attach_functs(tx, rx, MMC_SELECT, MMC_DESELECT);
  disk_initialize();
  return pf_mount(&fatfs_obj);
}

void PFF::buffer_mode()
{
	fatfs_obj.flag &= ~FA_STREAM;
}

void PFF::stream_mode()
{
	fatfs_obj.flag |= FA_STREAM;
}

int PFF::open_file(char * fn)
{
	return pf_open(fn);
}

int PFF::read_file(void * dest, int to_read, int * read_from)
{
	return pf_read(dest, to_read, (WORD *)read_from);
}

int PFF::lseek_file(int p)
{
	return pf_lseek(p);
}

int PFF::open_dir(DIR * dnfo, char * dn)
{
	return pf_opendir(dnfo, dn);
}

int PFF::read_dir(DIR * dnfo, FILINFO * fnfo)
{
	return pf_readdir(dnfo, fnfo);
}

PFF PFFS = PFF();