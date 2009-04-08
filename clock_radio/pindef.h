#define MMC_PORT      PORTB	// MMC Connection port
#define MMC_DIR       DDRB	// MMC Direction port
#define MMC_CS        PB3	// MMC card chip select signal

#define SPI_PORT      PORTB	// SPI Connection port
#define SPI_DIR       DDRB	// SPI Direction port
#define MISO          PB6	// SPI MISO signal
#define MOSI          PB5	// SPI MOSI signal
#define SCK           PB7	// SPI SCK signal
#define SS            PB4	// SPI SS signal

#define MP3_Port 		PORTB
#define MP3_PinIn 		PINB
#define MP3_DDR 		DDRB
#define MP3_xCDS_Pin 	PB1
#define MP3_xCS_Pin 	PB2
#define MP3_DREQ_Pin 	PB0
#define MP3_RST_Pin 	PB4

#define LCDDataPort 	PORTA
#define LCDDataDDR 		DDRA
#define LCDDataIn 		PINA
#define LCDCtrlPort 	PORTA
#define LCDCtrlDDR 		DDRA
#define LCDRSPin 		2
#define LCDRWPin 		1
#define LCDEPin 		3
#define LCDBLPort 		PORTD
#define LCDBLDDR 		DDRD
#define LCDBLPin 		7

#define LCDDataBit0 4
#define LCDDataBit1 5
#define LCDDataBit2 6
#define LCDDataBit3 7

#define btn_A_dir_reg 		DDRC
#define btn_B_dir_reg 		DDRD
#define btn_A_output_reg 	PORTC
#define btn_B_output_reg 	PORTD
#define btn_A_input_reg 	PINC
#define btn_B_input_reg 	PIND
