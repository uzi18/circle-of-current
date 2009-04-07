//*****************************************************************************
//
// Title        : MP3stick - MP3 player
// Authors      : Michael Wolf
// File Name    : 'mmc.c'
// Date         : January 6, 2006
// Version      : 1.01
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
// 1.00     04/01/2006  Michael Wolf  Initial Release
// 1.01     05/11/2006  Michael Wolf  *changed sector read function
//
//*****************************************************************************

#include "mmc.h"

//*****************************************************************************
// Function: mmc_sendbyte
// Parameters: Data byte to send
// Returns: Received data byte
//
// Description: Send one byte to SPI and receive one byte
//*****************************************************************************
uint8_t
mmc_sendbyte (uint8_t data)
{

    uint16_t i;

    SPDR = data;

    i = 0x1000;
    while (!(SPSR & (1 << SPIF))) {
        if (i-- == 0)
            break;
    }

    return SPDR;
}


//*****************************************************************************
// Function: mmc_init
// Parameters: none.
// Returns: 0 - Sucessfull
//          !0 - Error
//
// Description: Init MMC card and return status
//*****************************************************************************
uint8_t
mmc_init (void)
{
    MMC_PORT |= _BV (MMC_CS);	// deselect MMC card
    MMC_DIR |= _BV (MMC_CS);	// MMC Chip Select is outputs
    uint16_t retry;		// timeout counter
    uint8_t result;		// card response

    // send dummy bytes
    for (retry = 0; retry < 11; retry++) {
        mmc_sendbyte (0xFF);
    }

    retry = 0;
    do {
        result = mmc_sendcommand (MMC_GO_IDLE_STATE, 0);	// put card into idle state
        if (retry++ == 100)
            break;

    } while (result != MMC_R1_IDLE_STATE);

    if (result == 0) {
      return -1;		// return if card was not detected
    }

    retry = 0;
    do {
        result = mmc_sendcommand (MMC_SEND_OP_COND, 0);	// wait for card init process
        if (retry++ == 1000)
            break;
    } while (result);

    retry = 0;
    do {
        result = mmc_sendcommand (MMC_CRC_ON_OFF, 0);	// turn CRC off
        if (retry++ == 100)
            break;
    } while (result);

    retry = 0;
    do {
        result = mmc_sendcommand (MMC_SET_BLOCKLEN, 512);	// set 512 byte block lenght
        if (retry++ == 100)
            break;
    } while (result);

    return 0;
}


//*****************************************************************************
// Function: mmc_sendcommand
// Parameters: Command byte
//             Argument
// Returns: Received response byte
//
// Description: Send command to MMC card with CS signal handle
//*****************************************************************************
uint8_t
mmc_sendcommand (uint8_t cmd,
		 uint32_t arg)
{

    uint8_t resp;

    MMC_PORT &= ~_BV (MMC_CS);	// select MMC card

    resp = mmc_command (cmd, arg);	// issue the command

    MMC_PORT |= _BV (MMC_CS);	// deselect MMC card
    mmc_sendbyte (0xFF);	// send dummy byte

    return resp;
}


//*****************************************************************************
// Function: mmc_command
// Parameters: Command byte
//             Argument
// Returns: Received response byte
//
// Description: Send command to MMC card NO CS signal handle
//*****************************************************************************
uint8_t
mmc_command (uint8_t cmd,
	     uint32_t arg)
{

    uint8_t result;		// card response
    uint8_t retry = 0;		// retry counter

    mmc_sendbyte (cmd);		// send command
    mmc_sendbyte (arg >> 24);	// send argument
    mmc_sendbyte (arg >> 16);
    mmc_sendbyte (arg >> 8);
    mmc_sendbyte (arg);
    mmc_sendbyte (0x95);	// crc valid only for MMC_GO_IDLE_STATE

    // wait for card
    while ((result = mmc_sendbyte (0xff)) == 0xff) {
        if (retry++ > 8)
            break;
    }

    return result;		// return command response
}


//*****************************************************************************
// Function: mmc_read_sector
// Parameters: Sector number in actual partition
//             Data buffer
// Returns: Number of bytes read
//
// Description: Read data from one sectors
//              The sector number starts always from begin of partition not from
//              MBR!!! Reading sector 0 doesn't read the MBR but the Volume ID!!!
//*****************************************************************************
uint16_t
mmc_read_sector (uint32_t sec,
			 uint16_t num_of_bytes,
			 char * buffer)
{
    int16_t i = 0, count = 0;

    mmc_busy_wait ();		// wait until card is avail.

    MMC_SELECT

    if (0 == mmc_command (MMC_READ_SINGLE_BLOCK, sec << 8)) {
        if (mmc_card_response (0xFE)) {
            for (i = 0; i < num_of_bytes; i++) {
                *(buffer + i) = mmc_sendbyte (0xFF);
                count++;
            }
            for (i = 0; i < SECTOR_SIZE - num_of_bytes; i++) {
                mmc_sendbyte (0xFF);
            }
        }
    }

    MMC_DESELECT

    mmc_sendbyte (0xff);

    return count;
}


//*****************************************************************************
// Function: mmc_busy_wait
// Parameters: none.
// Returns: 0 - Not Busy
//          1 - Busy
//
// Description: Wait until card is not busy
//*****************************************************************************
uint8_t
mmc_busy_wait (void)
{

    uint16_t i;

    MMC_DESELECT mmc_sendbyte (0xff);	//wait 8 clock cycles
    MMC_SELECT mmc_sendbyte (0xff);	//wait 8 clock cycles
    mmc_sendbyte (0xff);	//wait 8 clock cycles
    mmc_sendbyte (0xff);	//wait 8 clock cycles
    mmc_sendbyte (0xff);	//wait 8 clock cycles

    i = 25000;

    while (i--) {
        if (mmc_sendbyte (0xff) == 0xff)
            break;		//wait for data line to become low
    }
    if (i == 0)
        return 1;		// Card still busy

    return 0;			// Card no longer busy
}

//*****************************************************************************
// Function: card_response
// Parameters: Expected response
// Returns: 1 - Response valid
//          0 - Response invalid
//
// Description: Wait for expected card response, return response status
//*****************************************************************************
uint8_t
mmc_card_response (uint8_t res)
{

    uint16_t count = 50000;

    while (count > 0) {
        if (mmc_sendbyte (0xFF) == res)
            return 1;		//wait for result to be the expected one...
        count--;
    }
    return 0;
}

//*****************************************************************************
// Function: card_present
// Parameters: None.
// Returns: 0 - card detected
//          1 - no card present
//
// Description: Check if a MMC/SD card is present
//*****************************************************************************
uint8_t
mmc_card_present (void)
{
	/*
    CARD_PORT |= _BV (CARD_DET); // enable card detection, pull-up
    CARD_DIR &= ~_BV (CARD_DET);
    
    if (bit_is_clear(CARD_INP, CARD_DET))
        return 0; // card detected
        
    return 1;
	*/

	return 0;
}
