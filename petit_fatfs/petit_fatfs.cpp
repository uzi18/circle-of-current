/*

Arduino Wrapper Function Library for Petit FatFs

Petit FatFs - http://elm-chan.org/fsw/ff/00index_p.html
by ChanN

Wrapper Functions by Frank Zhao

mmc.c origially written by ChanN, modified by Frank Zhao

*/

extern "C" {

#include <stdlib.h>
#include <string.h>
#include "diskio.h"

}

#include "petit_fatfs.h"

FATFS PFF::fatfs_obj; // stores working copy of FS
DIR PFF::dir_obj; // stores working copy of DIR
char * PFF::dir_path; // stores last accessed directory path
int PFF::MMC_CS; // stores pin number for MMC card's CS pin

PFF::PFF()
{
}

/*
Chip Select / Deselect Functions
*/

void PFF::MMC_SELECT()
{
	digitalWrite(MMC_CS, LOW);
}

void PFF::MMC_DESELECT()
{
	digitalWrite(MMC_CS, HIGH);
}

/*
Initialization Function, re-call if error detected (returns non-zero)
cs_pin is the Arduino pin connected to the MMC card's CS pin
rx and tx are pointers to SPI functions
*/

int PFF::begin(int cs_pin, unsigned char (* rx)(void), void (* tx)(unsigned char))
{
	MMC_CS = cs_pin; // set CS pin number
	pinMode(MMC_CS, OUTPUT); // set CS pin to output
	mmc_attach_functs(tx, rx, MMC_SELECT, MMC_DESELECT);
	
	disk_initialize();
	return pf_mount(&fatfs_obj);
}

/*
Open file by file path
*/

int PFF::open_file(char * fn)
{
	return pf_open(fn);
}

/*
Read last file opened into a buffer
dest must be a pointer to a buffer
to_read is how many bytes to read
read_from returns the actual number of bytes that has been read, use it to determine end-of-file
*/

int PFF::read_file(void * dest, int to_read, int * read_from)
{
	fatfs_obj.flag &= ~FA_STREAM; // disable streaming
	return pf_read(dest, to_read, (WORD *)read_from);
}

/*
Stream last file opened to a function
dest must be a pointer to a function
to_read is how many bytes to read
read_from returns the actual number of bytes that has been read, use it to determine end-of-file
*/

int PFF::stream_file(void * dest, int to_read, int * read_from)
{
	fatfs_obj.flag |= FA_STREAM; // enable streaming
	int res = pf_read(dest, to_read, (WORD *)read_from); // perform read
	return res; // return error
}

/*
Move file read pointer
*/

int PFF::lseek_file(int p)
{
	return pf_lseek(p);
}

/*
Opens a directory, will rewind pointer to first file in directory
dn is the directory path, must end in a slash character "/"
*/

int PFF::open_dir(char * dn)
{
	free(dir_path); // free allocated memory if any
	dir_path = (char *)calloc(strlen(dn) + 1, sizeof(dir_path)); // allocate memory for path
	strcpy(dir_path, dn); // store path
	return pf_opendir(&dir_obj, dn);
}

/*
Saves the FILINFO of the next file in currently open directory
fnfo is the pointer to the user's FILINFO struct
*/

int PFF::read_dir(FILINFO * fnfo)
{
	return pf_readdir(&dir_obj, fnfo);
}

/*
Opens a file using a FILINFO struct instead of a file path
the actual file path opened is the last path used in open_dir joined with the file name provided by fnfo
*/

int PFF::open_file(FILINFO * fnfo)
{
	char * fpath; // stores string for file path
	fpath = (char *)calloc(strlen(fnfo->fname) + strlen(dir_path) + 1, sizeof(fpath)); // create memory space for file path
	strcpy(fpath, dir_path); // copy dir_path into fpath so the strcat doesn't destroy dir_path
	fpath = strcat(fpath, fnfo->fname);	 // join path with file name
	int res = pf_open(fpath); // open the file, store error
	free(fpath); // free memory for path since it's no longer needed
	return res; // return error code
}

PFF PFFS = PFF(); // create usuable instance