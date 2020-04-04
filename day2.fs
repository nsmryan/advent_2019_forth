10 emit
: .. dup . ;

( handle input )
s" day2.fs" 2constant solution-file-name
s" input_day2.txt" 2constant input-file-name

: reload solution-file-name included ;

variable input-fd
variable input
variable #input

: read-input-file  input @ #input @ input-fd @ read-file throw ;
: allocate-file    here input !  input-fd @ file-size throw drop dup #input !  allot ;
: open-input       input-file-name r/o open-file throw input-fd ! ;
: ingest           open-input allocate-file read-input-file ;

( solution )
( intcode machine )
2048 constant #memory
0 variable memory
0 variable ip

variable cursor  
variable cell-ix
: store-code        cell-ix @ memory @ + !  cell-ix @  1 +  cell-ix ! ;
: read-code         cursor @ over s>number? drop drop swap cursor @ + 1 + cursor ! ;
: -end              cursor @  input @ #input @ +  >= if 0 rdrop then ;
: code-length       -end 0 begin dup cursor @ + c@ char , <> while 1 + repeat ;
: parse-program     begin code-length dup while read-code store-code repeat drop ;
: initialize        here memory !  #memory cells allot  input @ cursor ! ;
: start-intcode     initialize parse-program ;

: check-done 0 ;

: step ;

( run )
: .result          input @ #input @ dump ;

: part-1           ingest start-intcode begin check-done while step repeat .result ;

