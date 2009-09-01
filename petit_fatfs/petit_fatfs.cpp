/*
 * 
 * Arduino Wrapper Function Library for Petit FatFs
 * 
 * Petit FatFs - http://elm-chan.org/fsw/ff/00index_p.html
 * by ChanN
 * 
 * Wrapper Functions by Frank Zhao
 * 
 * mmc.c origially written by ChanN, modified by Frank Zhao
 * 
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
void * PFF::stream_dest; // function pointer for stream destination function

/*
Constructor, do not actually call, it's been done for you
*/

PFF::PFF()
{
	dir_path = (char *)calloc(max_path_len, sizeof(dir_path)); // allocate memory for path
}

/*
Chip Select / Deselect Functions for the MMC Card's CS pin
*/

void PFF::MMC_SELECT()
{
	digitalWrite(MMC_CS, LOW);
}

void PFF::MMC_DESELECT()
{
	digitalWrite(MMC_CS, HIGH);
}

///////////////////////////////////
/// begin list of public methods
///////////////////////////////////

/*

Initialization Function, re-call if error detected (returns non-zero)

parameters:
cs_pin is the Arduino pin connected to the MMC card's CS pin
rx and tx are pointers to SPI functions
rx must return a byte and accept no parameters
tx must accept a byte as a parameter and return nothing

returns:
error code, refer to comments in pff.h for the FRESULT enumeration

notes:
The reason why the SPI functions are not built in is because some people
may wish to use software SPI, or want to share SPI functions with other other code
so this reduces redundant code.

*/

int PFF::begin(int cs_pin, unsigned char (* rx)(void), void (* tx)(unsigned char))
{
	MMC_CS = cs_pin; // set CS pin number
	pinMode(MMC_CS, OUTPUT); // set CS pin to output
	MMC_DESELECT();
	disk_attach_spi_functs(tx, rx, MMC_SELECT, MMC_DESELECT); // attach SPI functions
	
	// start disk and mount FS, then open root directory
	disk_initialize();
	pf_mount(&fatfs_obj);
	return open_dir((char *)"/");
}

/*

Open file by file path

parameters:
fn is the file path, not relative to current directory

returns:
error code, refer to comments in pff.h for the FRESULT enumeration

*/

int PFF::open_file(char * fn)
{
	return pf_open(fn);
}

/*

Read last file opened into a buffer

parameters:
dest must be a pointer to a buffer
to_read is how many bytes to read
read_from returns the actual number of bytes that has been read, use it to determine end-of-file

returns:
error code, refer to comments in pff.h for the FRESULT enumeration

*/

int PFF::read_file(char * dest, int to_read, int * read_from)
{
	fatfs_obj.flag &= ~FA_STREAM; // disable streaming
	return pf_read((void *)dest, to_read, (WORD *)read_from);	
}

/*

Attach functions for streaming

parameters:
dest must be a pointer to a function, it must accept a byte as a parameter, it returns non-zero for streaming to continue, returning 0 will end the stream prematurely
pre_ and post_block are called before and after the first and last byte streamed (before and after the pre_ and post_byte functions)
pre_ and post_byte are called before and after each byte streamed.

notes:

refer to the code below to understand calling order

pre_block();
do
{
	pre_byte();
	res = dest(SPI_RX());
	post_byte();
}
while (--cnt && res);
post_block();

These functions are meant for users who would like to stream directly from the MISO line to another device
For example, when streaming mp3 data to a VS1002d decoder, pre_byte can be used to wait until there is room in the decoder's buffer
and pre_block and post_block can be used to select and deselect the decoder's SDI bus

If not needed, these functions can be empty functions.

*/

void PFF::setup_stream(void (* pre_block)(void), void (* pre_byte)(void), char (* dest)(char), void (* post_byte)(void), void (* post_block)(void))
{
	disk_attach_stream_functs(pre_byte, post_byte, pre_block, post_block); // attach functions
	stream_dest = (void *)dest;
}

/*

Stream last file opened to a function

parameters:
to_read is how many bytes to read
read_from returns the actual number of bytes that has been read, use it to determine end-of-file

returns:
error code, refer to comments in pff.h for the FRESULT enumeration

notes:
you must setup the stream first

*/

int PFF::stream_file(int to_read, int * read_from)
{
	fatfs_obj.flag |= FA_STREAM; // enable streaming
	int res = pf_read(stream_dest, to_read, (WORD *)read_from); // perform read
	return res; // return error
}

/*

Move file read pointer

parameters:
p is the desired pointer location

returns:
error code, refer to comments in pff.h for the FRESULT enumeration

*/

int PFF::lseek_file(long p)
{
	return pf_lseek(p);
}

/*

Opens a directory, will rewind pointer to first file in directory

parameters:
dn is the directory path, must start with a slash, must not end with a slash

returns:
error code, refer to comments in pff.h for the FRESULT enumeration

*/

int PFF::open_dir(char * dn)
{
	int res = pf_opendir(&dir_obj, dn);
	if (res == 0) // if successful
	{
		strcpy(dir_path, dn); // store path
	}
	return res; // return error if any
}

/*

Reopens current directory, which rewinds the file index

*/

int PFF::rewind_dir()
{
	return pf_opendir(&dir_obj, dir_path);
}

/*

Opens the parent directory, also rewinds the file index

*/

int PFF::up_dir()
{
	int res;
	int i;
	for (i = strlen(dir_path) - 1; i != -1 && dir_path[i] != '/'; i--); // finds last slash
	if (i >= 1) // if not already in root
	{
		char * path = (char *)calloc(i + 1, sizeof(path)); // make string
		path = (char *)memcpy((void *)path, dir_path, sizeof(path) * (i + 1)); // copy up to slash
		path[i] = 0; // null terminate
		res = open_dir(path); // attempt to open
		free(path); // free string
	}
	else
	{
		res = open_dir((char *)"/"); // reopen root
	}
	return res;
}

/*

Saves the FILINFO of the next file in currently open directory

parameters:
fnfo is the pointer to the user's FILINFO struct

returns:
error code, refer to comments in pff.h for the FRESULT enumeration

*/

int PFF::read_dir(FILINFO * fnfo)
{
	return pf_readdir(&dir_obj, fnfo);
}

/*

Opens a file using a FILINFO struct instead of a file path

parameters:
fnfo is the pointer to the user's FILINFO struct

returns:
error code, refer to comments in pff.h for the FRESULT enumeration

notes:
the actual file path opened is the last path used in open_dir joined with the file name provided by fnfo

*/

int PFF::open(FILINFO * fnfo)
{
	int res; // stores error code
	char * fpath; // stores string for file path
	fpath = (char *)calloc(strlen(fnfo->fname) + strlen(dir_path) + 1, sizeof(fpath)); // create memory space for file path
	strcpy(fpath, dir_path); // copy dir_path into fpath so the strcat doesn't destroy dir_path
	if (fpath[strlen(fpath) - 1] != '/')
	{
		fpath = strcat(fpath, "/");	 // join path with slash character
	}
	fpath = strcat(fpath, fnfo->fname);	 // join path with file name
	if (fnfo->fattrib & AM_DIR) // is a directory
	{
		res = open_dir(fpath); // open directory
	}
	else // is a file
	{
		res = pf_open(fpath); // open the file, store error
	}
	free(fpath); // free memory for path since it's no longer needed
	return res; // return error code
}

/*

Returns current directory path as a string

returns:
pointer to a string

*/

char * PFF::cur_dir()
{
	return dir_path;
}

//////////////////////////////////////
/// end list of public methods
//////////////////////////////////////

PFF PFFS = PFF(); // create usuable instance