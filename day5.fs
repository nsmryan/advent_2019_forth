include common.fs
include intcode.fs

( utilities )
: .. dup . ;
: 1+! dup @ 1 + swap ! ;

( handle input )
s" day5.fs" 2constant solution-file-name
s" input_day5.txt" 2constant input-file-name

: re                solution-file-name included ;

( part 1 )
: .result           ;
: setup             input-file-name ingest text @ #text @ start-intcode ;
: push-id           in-stack stack ! 1 push-stack ;
: pop-out           out-stack stack ! pop-stack ;

: part-1            setup push-id run .result ;

( part 2)
: .result           ;
: part-2            setup run .result ;

