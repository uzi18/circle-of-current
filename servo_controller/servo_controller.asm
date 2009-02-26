; select a chip here
.include "tn2313def.inc"

; constants

; baud rate
.equ BAUD_H = high(12)
.equ BAUD_L = low(12)
; 12 : 38400 at 8 mhz
; 64 : 9600 at 10 mhz
; 25 : 38400 at 16 mhz
; 64 : 19200 at 20 mhz

; center value of servos
.equ CENTER_H = high(100)
.equ CENTER_L = low(100)

; define default number of channels
.equ starting_max_chan = 8

; define ports

; servo channel
.equ CH_PORT = PORTB
.equ CH_DDR = DDRB

; status indicator
.equ STATUS_PORT = PORTD
.equ STATUS_DDR = DDRD
.equ STATUS_PIN = 6

; define registers
.def next_mask = r16 ; next mask to apply to port
.def chan = r17 ; stores the channel number of the next channel
.def zero = r18 ; always stores 0 to quickly clear a IO
.def temp_1 = r19 ; temporary working register
.def temp_2 = r20
.def cnt = r21 ; temporary counter

; flash origin point
.cseg
.org 0

rjmp start_point ; reset vector

; interrupt vectors
nop ; INT0
nop ; INT1
nop ; TIMER1 CAPT
; timer 1 compare A interrupt is here

; next pin
out CH_PORT, next_mask ; apply mask

; clear timer
out TCNT1L, zero
out TCNT1H, zero

; set SRAM read location
ldi XH, 0
mov XL, chan
lsl XL
ldi temp_2, SRAM_START
adc XL, temp_2 ; offset start point

; read MSB then LSB
ld temp_2, X+
out OCR1AH, temp_2
ld temp_2, X
out OCR1AL, temp_2

; next channel, then mod with max channel in SRAM
inc chan
ldi XL, 16 + SRAM_START
ld temp_2, X
cp chan, temp_2
;sbic SREG, SREG_Z ; commented out because SREG can't be accessed with SBIC
brne chan_not_eq_temp_2
ldi chan, 0
chan_not_eq_temp_2:

; for loop to set bit
ldi next_mask, 1
mov cnt, chan
for_loop_1:
cpi cnt, 0
breq break_for_loop_1
lsl next_mask
dec cnt
rjmp for_loop_1
break_for_loop_1:

; return from interrupt
reti

; starting point
start_point:

; always zero
ldi zero, 0

; initialize stack pointer
ldi temp_1, RAMEND
out SPL, temp_1

; set up mask and counter
ldi next_mask, 0x01
ldi chan, 0

; center servos at start
ldi temp_1, CENTER_H
out OCR1AH, temp_1
ldi temp_1, CENTER_L
out OCR1AL, temp_1

; for loop to set SRAM
ldi XH, 0
ldi XL, 0x60
ldi cnt, 8
for_loop_2:
cpi cnt, 0
breq break_for_loop_2
ldi temp_1, CENTER_H
ST X+, temp_1
ldi temp_1, CENTER_L
ST X+, temp_1
dec cnt
rjmp for_loop_2
break_for_loop_2:

; set default maximum channel number
ldi temp_1, starting_max_chan
ST X, temp_1

; setup UART

; load baud rate
ldi temp_1, BAUD_H
out UBRRH, temp_1
ldi temp_1, BAUD_L
out UBRRL, temp_1

; enable serial receiver
sbi UCSRB, RXEN

; setup timer 1

; enable timer 1 compare A interrupt
;sbi TIMSK, OCIE1A ; commented out because TIMSK can't be accessed with SBI for some reason
ldi temp_1, (1 << OCIE1A)
out TIMSK, temp_1

; set clock prescaler to 1
ldi temp_1, 1
out TCCR1B, temp_1

; setup Port

; port as output
ldi temp_1, 0xFF
out CH_DDR, temp_1

sbi STATUS_DDR, STATUS_PIN

; enable interrupt globally
sei

; main loop
loop_begin:

; toggle status pin
sbis STATUS_PORT, STATUS_PIN
rjmp status_1
cbi STATUS_PORT, STATUS_PIN
rjmp status_0
status_1:
sbi STATUS_PORT, STATUS_PIN
status_0:

; wait for a byte to be received
wait_for_byte_cmd:
sbis UCSRA, RXC
rjmp wait_for_byte_cmd

; read in byte
in temp_1, UDR

; no channel selected, quit
cpi temp_1, 0
breq loop_begin

; make index start at 0
dec temp_1

; set SRAM location to index
mov XL, temp_1
lsl XL
ldi temp_1, SRAM_START
adc XL, temp_1 ; offset start point

; wait for a byte to be received
wait_for_byte_h:
sbis UCSRA, RXC
rjmp wait_for_byte_h

; read in byte
in temp_1, UDR

; store (MSB) value in SRAM then increment pointer
ST X+, temp_1

; wait for a byte to be received
wait_for_byte_l:
sbis UCSRA, RXC
rjmp wait_for_byte_l

; read in byte
in temp_1, UDR

; store value in SRAM (LSB)
ST X, temp_1

; loop
rjmp loop_begin
