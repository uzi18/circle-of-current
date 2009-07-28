#ifndef config_inc

#define F_OSC 32768

#define eeprom_magic 45

#define display_title_time ((5 * F_OSC)/(1*256))
#define BL_timeout ((5 * F_OSC)/(1*256))
#define refresh_time ((15 * F_OSC)/(1*256))
#define menu_timeout ((10 * F_OSC)/(1*256))
#define vol_fade_speed ((1 * F_OSC)/(1*256))

#define shuffle_factor 16

#define playflag 0
#define continueflag 1
#define alarmflag 2
#define showerrflag 3

#define PLAYCMD 0
#define NEXTCMD 1
#define PREVCMD 2
#define UPCMD 3
#define DOWNCMD 4
#define MENUCMD 5
#define BADCMD 255

#define alarm_mode_default 3
#define alarm_mode_daily 2
#define alarm_mode_random 1

#define normalmode 0
#define loopmode 1
#define shufflemode 2

#define cur_time_menu 0
#define alarm_mode_menu 9
#define alarm_fade_menu 10
#define play_mode_menu 13
#define display_mode_menu 12
#define invert_output_menu 11

#define last_state_flag 0
#define fixed_state_flag 1
#define click_flag 2

#define btn_debounce_time 4

#define BL_on_speed 0
#define BL_off_speed 2

#define MP3PacketSize 32

//#define calc_song_length

#define config_inc
#endif
