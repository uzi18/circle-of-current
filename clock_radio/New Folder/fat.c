//*****************************************************************************
//
// Title        : MP3stick - MP3 player
// Authors      : Michael Wolf
// File Name    : 'fat.c'
// Date         : January 6, 2006
// Version      : 1.12
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
// 1.00     07/01/2006  Michael Wolf  Initial Release
// 1.10     22/10/2006  Michael Wolf  +Added direct M3U playlist support
// 1.11     05/11/2006  Michael Wolf  Complete code rework
// 1.12     26/11/2006  Michael Wolf  + Added some card present checks to prevent
//                                      unnecessary card access
//
//*****************************************************************************

#include "fat.h"

char sector_buffer[SECTOR_SIZE];
char fat_cache[SECTOR_SIZE];	//this keeps us from reading the fat everytime we read a new sector
uint32_t current_fat_sector;
uint32_t current_data_sector;
uint16_t cur_dir_entry = 0;	// number of files entrys in directory for specific extention

static int LCD_putc(char c, FILE *stream)
{
	LCDSend(c, 1);
	return 0;
}

static FILE LCDstdout = FDEV_SETUP_STREAM(LCD_putc, NULL, _FDEV_SETUP_WRITE);

//*****************************************************************************
// Function: read_sector
// Parameters: 
//              Sector to read
//              Data buffer
// Returns: Number of bytes read
//
// Description: Read one data or FAT sector
//*****************************************************************************
uint16_t
read_sector (const uint8_t fat,
	     const uint32_t sector,
	     const uint16_t num_of_bytes,
	     char * data_buffer)
{
    if (fat) {
        // read only sector from card if last sector in buffer
        // is not the same as requested
        if (current_fat_sector == sector)
            return num_of_bytes;
        else {
            current_fat_sector = sector;
            return mmc_read_sector (sector, num_of_bytes, data_buffer);
        }
    }
    else {
        // read only sector from card if last sector in buffer
        // is not the same as requested
        if (current_data_sector == sector)
            return num_of_bytes;
        else {
            current_data_sector = sector;
            return mmc_read_sector (sector, num_of_bytes, data_buffer);
        }
    }
}

//*****************************************************************************
// Function: init_fat
// Parameters: none.
// Returns: Error code
//
// Description: Init MMC card and read 1st partition info from MBR.
//              Return initilization status from MMC card.
//*****************************************************************************
int8_t
init_fat (void)
{
    if (mmc_card_present () != 0) return E_ERROR;
	if (mmc_init () != 0) return E_ERROR; // init MMC card

    current_fat_sector = -1;
    current_data_sector = -1;

	// read Master Boot Record
	read_sector (DATA, 0, SECTOR_SIZE, sector_buffer);

	char *MBR = &sector_buffer[446];	// set pointer to FAT type code in MBR

    uint8_t part_type = MBR[4];	// get partition type

    // get partition start sector
    uint32_t part_start = *((uint32_t *) & MBR[8]);	// get partition start sector

    // Read Partition Boot Record (Volume ID)
    uint32_t sectors_per_fat;
    uint8_t num_of_fats;

    // read partition sector 0 - Volume ID
    read_sector (DATA, part_start, SECTOR_SIZE, sector_buffer);

    partition.bytes_per_sector = *((uint16_t *) & sector_buffer[0x0b]);	// bytes per sector
    partition.sectors_per_cluster = *((uint8_t *) & sector_buffer[0x0d]);	// sectors per cluster
    partition.reserved_sectors = *((uint16_t *) & sector_buffer[0x0e]);	// number of reserved sectors
    partition.reserved_sectors += part_start;
    num_of_fats = *((uint8_t *) & sector_buffer[0x10]);	// number of FATs

    // check for FAT32
    if (!(part_type == 0x0b || part_type == 0x0c))
	{
        return E_INVALID;	// return on error
	}

    // store number of sectors per FAT, 32bit
    sectors_per_fat = *((uint32_t *) & sector_buffer[0x24]);
    // store Root Directorys first cluster, 32bit
    partition.root_directory = *((uint32_t *) & sector_buffer[0x2c]);	//first cluster in directory chain
    partition.root_directory &= 0xfffffff;
    partition.current_directory = partition.root_directory;
    // Calculate the first LBA cluster
    partition.data_starts =
        partition.reserved_sectors + sectors_per_fat * num_of_fats;

    return S_OK;
}


//*****************************************************************************
// Function: cluster2sector
// Parameters: Cluster number
// Returns: Sector number
//
// Description:  Convert cluster number to sector number
//*****************************************************************************
uint32_t
cluster2sector (uint32_t cluster)
{

    // Convert cluster number to sector number
    return partition.data_starts +
        ((cluster - 2) * partition.sectors_per_cluster);
}

//*****************************************************************************
// Function: get_next_cluster
// Parameters: Cluster number
// Returns: Next cluster number
//
// Description:  Return next cluster in chain
//*****************************************************************************
uint32_t
get_next_cluster (uint32_t cluster)
{

    uint32_t sector_offset;
    uint32_t byte_offset;

    cluster &= 0xfffffff;	//FAT32 is actually 28 bits.  Need to mask upper 4 bits
    sector_offset = cluster >> 7;
    sector_offset += partition.reserved_sectors;
    byte_offset = (cluster & 0x7f) << 2;

    if (sector_offset != current_fat_sector) {
        read_sector (FAT, sector_offset, SECTOR_SIZE, fat_cache);
        current_fat_sector = sector_offset;
    }

    byte_offset &= 0x1ff;

    cluster = *((uint32_t *) & fat_cache[byte_offset]);

    if ((cluster & 0xffffff8) == 0xffffff8)	//anything greater is end-of-chain
        return 0;

    return cluster & 0xfffffff;
}


//*****************************************************************************
// Function: fat_open
// Parameters:  File name
//              File info structure
// Returns: Error code
//
// Description:  open specific file
//*****************************************************************************
uint8_t
fat_open (uint16_t entry,
	       char * fname,
	       char * ext,
	       FAT_FILE * file)
{
    if (mmc_card_present () != 0) return E_ERROR;

    cur_dir_entry = 0;
    char *nameptr;
    uint32_t sector = 0;
    uint32_t dir_cluster = 0;
    uint8_t sector_entry = 16, sector_count = 0;;
    uint8_t seqnum = 0;		// long name seqence number
    char localname[MAX_PATH] = { 0 };	// clear long name
    nameptr = localname;

    do {

        if (sector_entry >= 16) {
            if (dir_cluster == 0) {	// read first Dir sector
                dir_cluster = partition.current_directory;	// get first cluster of RootDir
                sector = cluster2sector (dir_cluster);	// convert 1st RootDir cluster to sector
                sector_count = 0;
                read_sector (DATA, sector++, SECTOR_SIZE, sector_buffer);	// read 1st RootDir sector in buffer
            }
            else {		// if we cross the sector bountry then get next Dir sector
      
                if (++sector_count >= partition.sectors_per_cluster) {
                    sector_count = 0;
                    dir_cluster = get_next_cluster (dir_cluster);	// get next RootDir cluster
            
                    if (dir_cluster == 0)
                  break;	// we reached the last cluster in folder
            
                    sector = cluster2sector (dir_cluster);	// convert cluster to sector 
                }
                read_sector (DATA, sector++, SECTOR_SIZE, sector_buffer);	// read next RootDir sector
            }
      
            sector_entry = 0;
            nameptr = sector_buffer;	// point to file name in Dir record entry
        }

        if (*nameptr != 0xE5)	// if not a deleted entry
        {
            if (*(nameptr + 11) == ATTR_LFN)	// check for long name
            {			//Entry for Long Filename
                seqnum = (*nameptr & 0x3F) - 1;	// get sequence number
            
                // make complete filename from entrys
                localname[seqnum * 13 + 0] = *(nameptr + 1);
                localname[seqnum * 13 + 1] = *(nameptr + 3);
                localname[seqnum * 13 + 2] = *(nameptr + 5);
                localname[seqnum * 13 + 3] = *(nameptr + 7);
                localname[seqnum * 13 + 4] = *(nameptr + 9);
                localname[seqnum * 13 + 5] = *(nameptr + 14);
                localname[seqnum * 13 + 6] = *(nameptr + 16);
                localname[seqnum * 13 + 7] = *(nameptr + 18);
                localname[seqnum * 13 + 8] = *(nameptr + 20);
                localname[seqnum * 13 + 9] = *(nameptr + 22);
                localname[seqnum * 13 + 10] = *(nameptr + 24);
                localname[seqnum * 13 + 11] = *(nameptr + 28);
                localname[seqnum * 13 + 12] = *(nameptr + 30);
                // terminate file name string
                if (*nameptr & 0x40)
                    localname[seqnum * 13 + 13] = 0;
            }
            else if ((*nameptr != '.') &&
               ((*(nameptr + 11) &
                 (ATTR_HIDDEN | ATTR_SYSTEM | ATTR_VOLUME))
                == 0)
          ) {		// Short name entry
            
                if (localname[0] == 0) {
                    // copy short name
                    localname[0] = *(nameptr);
                    localname[1] = *(nameptr + 1);
                    localname[2] = *(nameptr + 2);
                    localname[3] = *(nameptr + 3);
                    localname[4] = *(nameptr + 4);
                    localname[5] = *(nameptr + 5);
                    localname[6] = *(nameptr + 6);
                    localname[7] = *(nameptr + 7);
                    localname[8] = '.';
                    localname[9] = *(nameptr + 8);
                    localname[10] = *(nameptr + 9);
                    localname[11] = *(nameptr + 10);
                    localname[12] = 0;	// terminate string
            
                    // remove trailing spaces from SFN
                    uint8_t fname_len = strlen (fname) - 4;
                    for (uint8_t m = 0; m < 5; m++)
                        localname[fname_len + m] = localname[8 + m];
                }
            
                // get file data on filename match OR
                // on extention and entry number match
                if ((strncasecmp (localname, fname, MAX_PATH) == 0)
                    ||
                    ((strncasecmp (ext, nameptr + 8, 3) == 0) &&
                     cur_dir_entry == entry)
                    ) {
            
                    // get first cluster of file
                    file->first_cluster = *((uint16_t *) & nameptr[0x14]);
                    file->first_cluster <<= 16;
                    file->first_cluster += *((uint16_t *) & nameptr[0x1a]);
                    file->first_cluster &= 0xfffffff;
                    // get file size in bytes
                    file->file_size = *((uint32_t *) & nameptr[0x1c]);
                    file->current_cluster = file->first_cluster;
                    file->pointer = 0;
            
                    return S_OK;
                }		// if filename check
            
                // Increment file entry counter
                if ((*(nameptr + 11) != ATTR_DIRECTORY) &&
                    (strncasecmp (ext, nameptr + 8, 3) == 0)
                    )
                    cur_dir_entry++;
            
                localname[0] = 0;	// clear long name
            }			// if short name entry   
        }			// if valid entry          

        nameptr += 32;
        sector_entry++;
    } while (1);

    return E_INVALID;		// return if file was not found
}


//*****************************************************************************
// Function: fat_read
// Parameters: Buffer to store bytes read
//             Number of bytes to read
//             File info structure
// Returns: Number of bytes read
//
// Description: read specific number of bytes from file
//              Be sure that out buffer IS NOT the same as sector_buffer!
//*****************************************************************************
uint16_t
fat_read (char * out,
	  uint16_t size,
	  FAT_FILE * file)
{
    if (mmc_card_present () != 0) return 0;

    if (!size)
        return size;
    if (size > SECTOR_SIZE)
        size = SECTOR_SIZE;

    // correct number of bytes requested
    if (file->pointer + size > file->file_size)
        size = (uint16_t) (file->file_size - file->pointer);

    uint16_t read = 0;		// number of bytes read
    char *ptr;

    do {
        // calculate sector offset in cluster
        uint8_t sector_in_cluster =
            (uint8_t) ((file->pointer / partition.bytes_per_sector) %
                 partition.sectors_per_cluster);
      
        ptr = &sector_buffer[file->pointer % partition.bytes_per_sector];
      
        // read required sector
        read_sector (DATA,
               cluster2sector (file->current_cluster) +
               sector_in_cluster, SECTOR_SIZE, sector_buffer);
      
        for (; size > 0; size--) {
      
            if (ptr >= (sector_buffer + sizeof (sector_buffer)))
                break;
      
            *out++ = *ptr++;
            file->pointer++;
            read++;
        }
      
        // check if we reach the cluster boundry
        if ((sector_in_cluster + 1) % partition.sectors_per_cluster == 0)
            // load new cluster
            file->current_cluster = get_next_cluster (file->current_cluster);
      
    } while (size);

    return read;		// return bytes read
}



//*****************************************************************************
// Function: fat_seek
// Parameters: File to seek
//             Offset
//             Seek base
// Returns: None
//
// Description:  Set file pointer to specific location in file
//*****************************************************************************
void
fat_seek (FAT_FILE * file,
	  int32_t offset,
	  uint8_t base)
{

    // add offset to pointer
    if (base == SEEK_CUR)
        file->pointer += offset;
    else if (base == SEEK_START)
        file->pointer = offset;
    else if (base == SEEK_END)
        file->pointer = file->file_size - offset;
    // check for valid pointer
    if (file->pointer > file->file_size)
        file->pointer = file->file_size;

    uint32_t cluster_offset =
        file->pointer / (partition.bytes_per_sector *
        partition.sectors_per_cluster);

    file->current_cluster = file->first_cluster;

    for (; cluster_offset > 0; --cluster_offset)
        file->current_cluster = get_next_cluster (file->current_cluster);
}


//*****************************************************************************
// Function: dir_list
// Parameters: File extention to list
//             File info structure
// Returns: Number of files found
// 
// Description:  Search actual directory for files with specific extention
//               and return number of files found
//*****************************************************************************
uint16_t
dir_list (char * ext,
	  FAT_FILE * file)
{
    if (mmc_card_present () != 0) return 0;

    fat_open (-1, NULL, ext, file);

    return cur_dir_entry;
}
