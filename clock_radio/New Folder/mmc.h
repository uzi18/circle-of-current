//*****************************************************************************
//
// Title        : MP3stick - MP3 player
// Authors      : Michael Wolf
// File Name    : 'mmc.h'
// Date         : January 6, 2006
// Version      : 1.00
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
//
//*****************************************************************************
#ifndef MMC_H
#define MMC_H

#include <avr/io.h>
#include <stdint.h>

#define MMC_PORT      PORTB	// MMC Connection port
#define MMC_DIR       DDRB	// MMC Direction port
#define MMC_CS        PB3	// MMC card chip select signal

#define SPI_PORT      PORTB	// SPI Connection port
#define SPI_DIR       DDRB	// SPI Direction port
#define MISO          PB6	// SPI MISO signal
#define MOSI          PB5	// SPI MOSI signal
#define SCK           PB7	// SPI SCK signal
#define SS            PB4	// SPI SS signal

#define SECTOR_SIZE   512	// MMC card sector size

#define MMC_GO_IDLE_STATE			    0x40	// software reset
#define MMC_SEND_OP_COND			    0x41	// brings card out of idle state
#define MMC_SEND_CSD				      0x49	// ask card to send card speficic data (CSD)
#define MMC_SEND_CID				      0x4A	// ask card to send card identification (CID)
#define MMC_STOP_MULTI_TRANS      0x4C	// stop transmission on multiple block read
#define MMC_SEND_STATUS				    0x4D	// ask the card to send it's status register
#define MMC_SET_BLOCKLEN			    0x50	// sets the block length used by the memory card
#define MMC_READ_SINGLE_BLOCK			0x51	// read single block
#define MMC_READ_MULTI_BLOCK      0x52	// read multiple block
#define MMC_WRITE_BLOCK				    0x58	// writes a single block
#define MMC_WRITE_MULTI_BLOCK     0x59	// writes multiple blocks
#define MMC_PROGRAM_CSD				    0x5B	// change the bits in CSD
#define MMC_SET_WRITE_PROT			  0x5C	// sets the write protection bit
#define MMC_CLR_WRITE_PROT			  0x5D	// clears the write protection bit
#define MMC_SEND_WRITE_PROT			  0x5E	// checks the write protection bit
#define MMC_TAG_SECTOR_START			0x60	// Sets the address of the first sector of the erase group#define MMC_TAG_SECTOR_END                      33
#define MMC_SET_LAST_GROUP        0x61	// Sets the address of the last sector of the erase group
#define MMC_UNTAG_SECTOR			    0x62	// removes a sector from the selected group
#define MMC_TAG_ERASE_GROUP_START 0x63	// Sets the address of the first group
#define MMC_TAG_ERARE_GROUP_END		0x64	// Sets the address of the last erase group
#define MMC_UNTAG_ERASE_GROUP			0x65	// removes a group from the selected section
#define MMC_ERASE				          0x66	// erase all selected groups
#define MMC_LOCK_BLOCK            0x6A	// locks a block
#define MMC_READ_OCR              0x7A	// reads the OCR register
#define MMC_CRC_ON_OFF				    0x7B	// turns CRC off


// R1 Response bit-defines
#define MMC_R1_BUSY				    0x80
#define MMC_R1_PARAMETER			0x40
#define MMC_R1_ADDRESS				0x20
#define MMC_R1_ERASE_SEQ			0x10
#define MMC_R1_COM_CRC				0x08
#define MMC_R1_ILLEGAL_COM		0x04
#define MMC_R1_ERASE_RESET		0x02
#define MMC_R1_IDLE_STATE			0x01

// Data Start tokens
#define MMC_STARTBLOCK_READ			0xFE
#define MMC_STARTBLOCK_WRITE			0xFE
#define MMC_STARTBLOCK_MWRITE			0xFC

#define MMC_DESELECT  MMC_PORT |= (1<<MMC_CS);
#define MMC_SELECT    MMC_PORT &= ~(1<<MMC_CS);

//void hardware_init(void);
uint8_t mmc_sendbyte (uint8_t);
uint8_t mmc_init (void);
uint8_t mmc_command (uint8_t,
		     uint32_t);
uint8_t mmc_sendcommand (uint8_t,
			 uint32_t);
uint16_t mmc_read_sector (uint32_t,
          uint16_t,
				  char *);
uint8_t mmc_busy_wait (void);
uint8_t mmc_card_response (uint8_t);
uint8_t mmc_card_present (void);

#endif
