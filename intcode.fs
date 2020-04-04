( utilities )
: .. dup . ;
: 1+! dup @ 1 + swap ! ;

( intcode machine )
2048 constant #memory
variable original
variable memory
variable ip
variable cycles
variable cells-used

: save              memory @ original @ #memory move ;
: restore           original @ memory @ #memory move  0 ip !  0 cycles ! ;

: store             cells memory @ + ! ;
: load              cells memory @ + @ ;

: ip@               ip @ load ;
: ip++              ip 1+! ;

: .ip               ." ip: " ip @ . ." = " ip@ . cr ;
: .memory           0 begin dup cells-used @ < while dup load . 1+ repeat drop ;
: .intcode          cr .ip .memory cr ;

: arg               ip@ ip++ ;
: add               arg load arg load + arg store ;
: mult              arg load arg load * arg store ;
: input             ;
: output            ;

( parsing )
variable text-start
variable #text
variable cursor

: store-code        cells-used @ cells memory @ + !  cells-used 1+! ;
: read-code         cursor @ over s>number? drop drop swap cursor @ + 1 + cursor ! ;
: -end              cursor @  text-start @ #text @ +  >= if 0 rdrop then ;
: code-length       -end 0 begin dup cursor @ + c@ [char] , <> while 1 + repeat ;
: parse-program     begin code-length dup while read-code store-code repeat drop ;
: memory-allot      here #memory cells allot ;
: init              0 ip !  memory-allot original !  memory-allot memory !  text-start @ cursor ! ;
: start-intcode     #text ! text-start !  init  parse-program  save ;

( running )
: not-done          ip@ 99 <> ;

: exec              case
                      1 of add endof
                      2 of mult endof
                      3 of input endof
                      4 of output endof
                      dup ." Unexpected opcode! " . cr
                    endcase ;

: step              ip@  ip++  exec  cycles 1+! ;
: run               begin not-done while step repeat ;

: poke              swap  1 store  2 store ;

