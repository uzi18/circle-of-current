<title>MP3 Alarm Clock</title>
<h2>MP3 Alarm Clock</h2>
<h2>This Page and Project are Still Under Construction</h2>

<p>This project is an alarm clock that is also a small MP3 player, and uses MP3s as the alarm. It runs on an ATmega644 microcontroller, uses a 16 x 2 LCD as a display, and a VS1002d is used to play MP3s from a microSD card. The casing is from another alarm clock that had a burnt out transformer, I removed most of the components inside so I can use the buttons that are already there.

<p align='center'>Insert Picture Here

<p><exlnk=http://code.google.com/p/circle-of-current/source/browse/#svn/mp3_clock>Source Code and Schematic Link</exlnk>

<h3>The Alarm Clock Half</h3>

<p>I am not using a RTC right now but I do plan on upgrading to a RTC, a 32.768 KHz watch crystal is connected directly to the asynchronous timer on the microcontroller. Time is kept using the timer2 overflow interrupt, which also handles counters for various timeouts, fading the LCD backlight, and debouncing buttons. Timer 2’s compare match output is used to control the LCD brightness.

<p>Alarm settings are stored in EEPROM but time settings are volatile unless I add a battery backed RTC chip.

<p>Time is displayed in a large format on the LCD by using custom characters, each digit takes an area that is 3 characters wide and 2 characters high.  (I got the characters from "dcb" from <exlnk=http://www.arduino.cc/cgi-bin/yabb2/YaBB.pl?num=1213319639>here</exlnk>)

<p>There can be a different alarm time and MP3 file set for each day of the week. If a file is missing, then the default file is used. If the default file is missing, then a song stored on the AVR’s flash memory is used.

<h3>The MP3 Player Half</h3>

<p>The MP3 player software has the most basic features: play, pause, rewind, next song, previous song, loop, shuffle, and change volume. Song titles are extracted from the MP3’s ID3 tag, and displayed for a few seconds before going back into time display mode.

<p>The songs are stored on a microSD card, and FatFs is used to read the file system and files. FatFs provides a function to iterate through each file in the directory. The open_next function uses f_readdir to go through each file, rewinding to the beginning of the folder when it reaches the end. The open_previous function does the same except it checks for a match against the current file, and then opens the one before the current file. The shuffle function basically scans through a random number of files and then opens the next. The rewind function stops the MP3 decoder chip, then uses f_lseek to change the file R/W pointer to 0.

<p>The MP3Open function opens MP3 files and extracts the song title and song duration from the file. The MP3Play function feeds the decoder if the decoder is not too busy. All functions return 0 if successful, and if no files are found, an error message is displayed for a brief period. The disk is reinitialized checked continuously if no files are found, which allows for hot-inserting the SD card.


<h3>The Settings Menu</h3>

<p>A button is used to enter or exit the menu, and while in the menu, the left and right buttons scroll through the different types of settings, while the plus and minus buttons changes the settings, the play button is used to navigate sub-menus if a setting has sub-settings. The backlight is always kept on while in the menu, but if the user doesn’t do anything, the menu is exited automatically.

<p>From the menu, the current time and day can be changed. The play modes can be changed between normal, looping, or shuffling. Each alarm can be set or turned on/off. The alarm can be set to play a random song, a default song, or a song for the specific day. The alarm volume can be set to instantly full or gradually rising. The time display mode can be changed between 12 or 24 hour mode.

<p>Settings are stored into EEPROM upon exit, and to extend the EEPROM life, it is only written if the previous data and new data do not match.

<h3>Hardware</h3>

<p align='center'>Insert Schematic Here

<p>The main power supply is a 9V wall adapter. The 9V is regulated down to 5V to power the LCD and the MP3 decoder breakout board. The ATmega644 and the microSD card are powered with a 3.3V from another regulator. The backlight on the LCD is switched with a BS270 MOSFET so the LCD backlight can use 5V. The LCD contrast is adjusted by a 10 kohm potentiometer. The decoder chip has a built in headphone driver, and the left channel can be inverted, so the speaker is connected directly to the left and right channel outputs. In the future, a battery backed RTC and an audio amplifier may be added, maybe even a FM radio transmitter and receiver. I should also swap out my 5V LCD for a 3.3V one that has a black background and white lettering instead, doing so would allow the entire circuit to run on 3.3V which eliminates the need for a 5V regulator. There are a bunch of tactile push-button switches built into the broken alarm clock which I am using for the enclosure, which made the casing perfect for this project.

<p>The fuse settings for the ATmega644 are:

<p aligh='center'>Insert Fuse Settings Here

<h3>Software</h3>

<p><exlnk=http://code.google.com/p/circle-of-current/source/browse/#svn/mp3_clock>Source Code and Schematic Link</exlnk>

<p>Some notes if you want to make your own:

<p>Pin definitions are in pindef.h and configuration options can be changed in config.h (things like menu item ordering, timeout times)

<p>Only port C and D can be used for buttons, but avoid using the crystal input for timer 2, the I2C lines, the timer 2 compare match outputs, and the serial ports. The global variable array btn_flags contains a button’s current state, its debounced (stable) state, and a click event flag. There is an index scheme for this array: index 0 to 7 corresponds to the pins on port C and index 8 to 15 corresponds to the pins on port D.

<p>There is one free ADC port on port A, and the SS pin on port B is also free, these should not be used for button inputs. If I add FM radio capabilities, these will be used as chip select pins.