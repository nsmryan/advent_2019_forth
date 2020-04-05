( utilities )
: .. dup . ;
: 1+! dup @ 1 + swap ! ;

( intcode machine )
2048 constant #memory
0 value memory

128 constant #input
0 value input-stack

128 constant #output
0 value output-stack

variable ip
variable cycles
variable cells-used

0 value original

0 constant INT_RUNNING
1 constant INT_DONE
2 CONSTANT INT_NEED_INPUT

: save              memory original #memory move ;
: restore           original memory #memory move  0 ip !  0 cycles ! ;

: store             cells memory + ! ;
: load              cells memory + @ ;

: ip@               ip @ load ;
: ip++              ip 1+! ;

: .ip               ." ip: " ip @ . ." = " ip@ . cr ;
: .memory           0 begin dup cells-used @ < while dup load . 1+ repeat drop ;
: .cells-used       ." cells: " cells-used @ . ;
: .intcode          cr .ip cr .cells-used cr .memory cr  ;

: arg               ip@ ip++ ;
: op-add            arg load arg load + arg store ;
: op-mult           arg load arg load * arg store ;
: op-input          ;
: op-output         ;

( parsing )
variable prog-start
variable #prog
variable cursor

: store-code        cells-used @ cells memory + !  cells-used 1+! ;
: read-code         cursor @ over s>number? drop drop swap cursor @ + 1 + cursor ! ;
: -end              cursor @  prog-start @ #prog @ +  >= if 0 rdrop then ;
: code-length       -end 0 begin dup cursor @ + c@ [char] , <> while 1 + repeat ;
: parse-program     begin code-length dup while read-code store-code repeat drop ;

: init              0 ip !
                    here #memory cells allot to original
                    here #memory cells allot to memory
                    here #input  cells allot to input-stack 
                    here #output cells allot to output-stack
                    prog-start @ cursor ! ;

: start-intcode     #prog ! prog-start !  init  parse-program  save ;

( running )
: not-done          ip@ 99 <> ;

: exec              case
                      1 of op-add endof
                      2 of op-mult endof
                      3 of op-input endof
                      4 of op-output endof
                      dup ." Unexpected opcode! " . cr
                    endcase ;

: step              ip@  ip++  exec  cycles 1+! ;
: run               begin not-done while step repeat ;

: poke              swap  1 store  2 store ;

