include common.fs
include intcode.fs

( input )
s" day5.fs" solution-file-name 2!
s" input_day5.txt" 2constant input-file-name

( utilities )
: .result           begin intcode-out dup 0= while repeat . ;
: setup             input-file-name ingest text @ #text @ start-intcode ;

( part 1 )
: part-1            setup 1 intcode-in run .result ;

( part 2)
: part-2            setup 5 intcode-in run .result ;

