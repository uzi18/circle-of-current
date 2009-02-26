; select a chip here
.include "tn2313def.inc"
;.include "tn24def.inc"
;.include "tn44def.inc"
;.include "tn84def.inc"
;.include "tn261def.inc"
;.include "tn461def.inc"
;.include "tn861def.inc"

; constants

; baud rate
.equ BAUD_H = high(12)
.equ BAUD_L = low(12)
; 12 : 38400 at 8 mhz
; 64 : 9600 at 10 mhz
; 25 : 38400 at 16 mhz
; 64 : 19200 at 20 mhz

; center value of servos
.equ CENTER_H = high(3000)
.equ CENTER_L = low(3000)

; define ports

; servo channel
.equ CH_PORT = PORTB
.equ CH_DDR = DDRB
.equ chan0_pin = 0
.equ chan1_pin = 1
.equ chan2_pin = 2
.equ chan3_pin = 3

; status indicator
.equ STATUS_PORT = PORTD
.equ STATUS_DDR = DDRD
.equ STATUS_PIN = 6

; define registers
.def next_mask = r16 ; next mask to apply to port
; these are like arrays
.def ch0_h = r17
.def ch0_l = r18
.def ch1_h = r19
.def ch1_l = r20
.def ch2_h = r21
.def ch2_l = r22
.def ch3_h = r23
.def ch3_l = r24
.def zero = r25 ; always stores 0 to quickly clear a IO
.def temp = r26 ; temporary working register, X pointer is not used so ignore the warning message

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

; check which channel
sbrc next_mask, 0
rjmp set_timer_ch0
sbrc next_mask, 1
rjmp set_timer_ch1
sbrc next_mask, 2
rjmp set_timer_ch2
sbrc next_mask, 3
rjmp set_timer_ch3

; starting point
start_point:

; always zero
ldi zero, 0

; initialize stack pointer
ldi temp, RAMEND
out SPL, temp

; set up mask
ldi next_mask, 0x11

; center servos at start
ldi ch0_l, CENTER_L
ldi ch1_l, CENTER_L
ldi ch2_l, CENTER_L
ldi ch3_l, CENTER_L
ldi ch0_h, CENTER_H
ldi ch1_h, CENTER_H
ldi ch2_h, CENTER_H
ldi ch3_h, CENTER_H
out OCR1AH, ch0_h
out OCR1AL, ch0_l

; setup UART

; load baud rate
ldi temp, BAUD_H
out UBRRH, temp
ldi temp, BAUD_L
out UBRRL, temp

; enable serial receiver
sbi UCSRB, RXEN

; setup timer 1

; enable timer 1 compare A interrupt
;sbi TIMSK, OCIE1A ; commented out because TIMSK can't be accessed with SBI for some reason
ldi temp, (1 << OCIE1A)
out TIMSK, temp

; set clock prescaler to 1
ldi temp, 1
out TCCR1B, temp

; setup Port

; port as output
ldi temp, (1 << chan0_pin) + (1 << chan1_pin) + (1 << chan2_pin) + (1 << chan3_pin)
out DDRB, temp

sbi DDRD, STATUS_PIN

; enable interrupt globally
sei

; main loop
loop_begin:

; toggle status pin
sbis PORTD, STATUS_PIN
rjmp status_1
cbi PORTD, STATUS_PIN
rjmp status_0
status_1:
sbi PORTD, STATUS_PIN
status_0:

; wait for a byte to be received
wait_for_byte:
sbis UCSRA, RXC
rjmp wait_for_byte

; read in byte
in temp, UDR

; check which channel
sbrc temp, 0
rcall set_ch0
sbrc temp, 1
rcall set_ch1
sbrc temp, 2
rcall set_ch2
sbrc temp, 3
rcall set_ch3

; loop
rjmp loop_begin

; wait for byte, load into "array", then once more for the lower byte
set_ch0:
sbis UCSRA, RXC
rjmp set_ch0
in ch0_h, UDR
set_ch0_l:
sbis UCSRA, RXC
rjmp set_ch0_l
in ch0_l, UDR
ret

; read from "array" and load into timer 1 compare A register
set_timer_ch0:
out OCR1AH, ch0_h
out OCR1AL, ch0_l
ldi next_mask, (1 << chan1_pin)
reti

set_ch1:
sbis UCSRA, RXC
rjmp set_ch1
in ch1_h, UDR
set_ch1_l:
sbis UCSRA, RXC
rjmp set_ch1_l
in ch1_l, UDR
ret

set_timer_ch1:
out OCR1AH, ch1_h
out OCR1AL, ch1_l
ldi next_mask, (1 << chan2_pin)
reti

set_ch2:
sbis UCSRA, RXC
rjmp set_ch2
in ch2_h, UDR
set_ch2_l:
sbis UCSRA, RXC
rjmp set_ch2_l
in ch2_l, UDR
ret

set_timer_ch2:
out OCR1AH, ch2_h
out OCR1AL, ch2_l
ldi next_mask, (1 << chan3_pin)
reti

set_ch3:
sbis UCSRA, RXC
rjmp set_ch3
in ch3_h, UDR
set_ch3_l:
sbis UCSRA, RXC
rjmp set_ch3_l
in ch3_l, UDR
ret

set_timer_ch3:
out OCR1AH, ch3_h
out OCR1AL, ch3_l
ldi next_mask, (1 << chan0_pin)
reti
