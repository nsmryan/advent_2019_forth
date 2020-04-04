10 emit

( utilities )
: .. dup . ;
: 1+! dup @ 1 + swap ! ;

( handle input )
s" day2.fs" 2constant solution-file-name
s" input_day2.txt" 2constant input-file-name

: reload solution-file-name included ;

variable input-fd
variable input
variable #input

: read-input-file  input @ #input @ input-fd @ read-file throw drop ;
: allocate-file    here input !  input-fd @ file-size throw drop dup #input !  allot ;
: open-input       input-file-name r/o open-file throw input-fd ! ;
: ingest           open-input allocate-file read-input-file ;

( solution )
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

( parsing )
variable cursor  

: store-code        cells-used @ cells memory @ + !  cells-used 1+! ;
: read-code         cursor @ over s>number? drop drop swap cursor @ + 1 + cursor ! ;
: -end              cursor @  input @ #input @ +  >= if 0 rdrop then ;
: code-length       -end 0 begin dup cursor @ + c@ [char] , <> while 1 + repeat ;
: parse-program     begin code-length dup while read-code store-code repeat drop ;
: memory-allot      here #memory cells allot ;
: init              0 ip !  memory-allot original !  memory-allot memory !  input @ cursor ! ;
: start-intcode     init parse-program save ;

: not-done          ip@ 99 <> ;

: exec              case
                      1 of add endof
                      2 of mult endof
                      dup ." Unexpected opcode! " . cr
                    endcase ;

: step              ip@  ip++  exec  cycles 1+! ;
: run               begin not-done while step repeat ;

: poke              swap  1 store  2 store ;

( part 1 )
: .result           0 load . ;
: setup             ingest start-intcode ;

: part-1            setup 12 2 poke  run  .result ;

( part 2)
variable noun
variable verb

19690720 constant ANSWER

: current-answer    0 load ;
: pair              noun @ verb @ ;
: not-done          current-answer ANSWER <> ;
: next-pair         0  noun 1+! noun @ 99 > if 0 noun ! 1 + then verb @ + verb ! ;
: .pair             pair swap . . ;
: .result           pair swap 100 * + . ;
: part-2            setup begin not-done while restore  pair poke run next-pair repeat .result ;

