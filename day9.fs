[IFDEF] __marker__
  __marker__
[ENDIF]
marker __marker__

include common.fs
include intcode.fs

( input )
s" day9.fs" solution-file-name 2!
s" input_day9.txt" 2constant input-file-name
: read-input        input-file-name ingest text @ #text @ ;

( utilities )

( solution )
: .result           .intcode ;

: setup-1           start-intcode ;
: setup-2           start-intcode ;

( part 1 )
s" 109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99" 2constant input-test1
: t1                input-test1 setup-1 ;
: test-1            input-test1 setup-1 run .result ;

s" 1102,34915192,34915192,7,4,7,99,0" 2constant input-test2
: t2                input-test2 setup-1 ;
: test-2            input-test2 setup-1 run .result ;
: part-1            read-input setup-1 1 0 intcode-in run .result ;

( part 2)
: part-2            read-input setup-2 2 0 intcode-in run .result ;


