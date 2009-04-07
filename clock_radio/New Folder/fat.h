//*****************************************************************************
//
// Title        : MP3stick - MP3 player
// Authors      : Michael Wolf
// File Name    : 'fat.h'
// Date         : January 6, 2006
// Version      : 1.11
// Target MCU   : Atmel AVR ATmega128/1281
// Editor Tabs  : 2
//
// NOTE: The authors in no way will be responsible for damages that you
//       coul'd be using this code.
//       Use this code at your own risk.
//
//       This code is distributed under the GNU Public License
//       which can be found at http://www.gnu.org/licenses/gpl.txt
//
// Change Log
//
// Version  When        Who           What
// -------  ----        ---           ----
// 1.00     06/01/2006  Michael Wolf  Initial Release
// 1.10     22/10/2006  Michael Wolf  +Added direct M3U playlist support
// 1.11     05/11/2006  Michael Wolf  Complete code rework
//
//*****************************************************************************
#ifndef FAT_H
#define FAT_H

#include <avr/io.h>
#include <avr/pgmspace.h>
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <ctype.h>
#include <stdint.h>
#include "mmc.h"

#define MAX_PATH        260

#define ATTR_NORMAL     0x00	// normal file
#define ATTR_READONLY   0x01	// file is readonly
#define ATTR_HIDDEN     0x02	// file is hidden
#define ATTR_SYSTEM     0x04	// file is a system file
#define ATTR_VOLUME     0x08	// entry is a volume label
#define ATTR_LFN      	0x0F	// this is a long filename entry
#define ATTR_DIRECTORY  0x10	// entry is a directory name
#define ATTR_ARCHIVE    0x20	// file is new or modified

#define S_OK            (0)
#define E_ERROR         (1)
#define E_INVALID       (2)
#define E_ERROR_MMC     (3)

#define DATA 0
#define FAT  1

#define SEEK_START  0
#define SEEK_CUR    1
#define SEEK_END    2

extern char sector_buffer[512];

struct {
    uint16_t bytes_per_sector;
    uint32_t data_starts; // 1st root dir sector
    uint32_t root_directory; // 1st root dir cluster
    uint32_t current_directory; // actual dir cluster
    uint8_t sectors_per_cluster;
    uint16_t reserved_sectors; // start of 1st FAT
} partition;

typedef struct {
    uint32_t current_cluster;
    uint32_t first_cluster;
    uint32_t file_size;
    uint32_t pointer;
} FAT_FILE;

int8_t init_fat (void);
uint8_t fat_open (uint16_t,
      char *,
      char *,
			FAT_FILE *);
uint16_t fat_read (char *,
		  uint16_t,
		  FAT_FILE *);
void fat_seek (FAT_FILE *,
	    int32_t,
	    uint8_t);
uint16_t dir_list (char *,
		  FAT_FILE *);

#endif
