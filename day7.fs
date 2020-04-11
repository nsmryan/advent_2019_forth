include common.fs
include intcode.fs

( input )
s" day7.fs" solution-file-name 2!
s" input_day7.txt" 2constant input-file-name

( utilities )
: .result           .output ;
: read-input        input-file-name ingest text ;

0 constant initial-input
here 0 , 1 , 2 , 3 , 4 , constant ids
: id@               cells ids + @ ;


here 5 cells allot constant thrusters
: thrusters!        cells thrusters + ! ;
: thrusters@        cells thrusters + @ ;
: use-thruster      thrusters@ intcode ! ;

: restore-thrusters 5 0 ?do i thrusters@ restore loop ;

: push-in           in-stack stack ! push-stack ;
: thrusters-allot   5 0 ?do intcode-new  intcode @ i thrusters! loop ;
: setup-thrusters   thrusters-allot restore-thrusters ;
: setup             read-input  @ #text @ start-intcode  setup-thrusters ;

: input-ids         5 0 ?do i use-thruster i id@ push-in loop ;
: ignite            input-ids ;
: >signal           ;       
: signal>           ;       
: run-thrusters     ignite 5 0 ?do i use-thruster >signal run signal> loop ;

( part 1 )
: part-1            setup run-thrusters .result ;

( part 2)
: part-2            setup run-thrusters .result ;


