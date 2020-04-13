[IFDEF] __marker__
  __marker__
[ENDIF]
marker __marker__

include common.fs
include intcode.fs

( input )
s" day7.fs" solution-file-name 2!
s" input_day7.txt" 2constant input-file-name
: read-input        input-file-name ingest text @ #text @ ;

( utilities )
variable most-signal

here 0 , 1 , 2 , 3 , 4 , constant sequence
here 0 , 0 , 0 , 0 , 0 , constant perm
here 5 cells allot constant best-perm
here 0 , 1 , 2 , 3 , 4 ,  constant ids
: id@               cells ids + @ ;

variable num-permutes

0 value perm-offset

: .seq              5 0 ?do i cells over + @ . loop drop ;

( solution )
: .result           cr ." result = " most-signal @ . cr
                    ." best permutation: " best-perm .seq ;

here 5 cells allot constant thrusters
: thrusters!        cells thrusters + ! ;
: thrusters@        cells thrusters + @ ;
: use-thruster      thrusters@ intcode ! ;

: restore-thrusters 5 0 ?do i thrusters@ restore loop ;

: thrusters-allot   5 0 ?do intcode-new  intcode @ i thrusters! loop ;

: reset-perms       0 num-permutes ! 0 most-signal ! perm 5 cells 0 fill  best-perm 5 cells 0 fill ;
: setup             start-intcode  reset-perms  thrusters-allot ;

: ignite            5 0 ?do i use-thruster i id@ perm-offset + intcode-in loop ;
: >signal           intcode-in ;
: signal>           intcode-out ;
: run-thrusters     5 0 ?do i use-thruster >signal run signal> loop ;
: loop-thrusters    begin 4 use-thruster state @ INT_DONE <> while run-thrusters repeat ;

( part 1 )
: read-perm         cells perm + @ ;
: write-perm        cells perm + ! ;
: update-perm       1+ swap 5 swap - dup 0 > if /mod swap else drop 0 swap then ;
: carry             ( index -- carry )
                    dup dup read-perm  update-perm rot write-perm ;
: mark              num-permutes 1+! ;
: increment         1 5 0 ?do dup 0= if drop unloop exit else i carry then loop ;
: swap-cells        2dup @ swap @ rot ! swap ! ;
: apply             5 0 ?do i read-perm i + cells ids + i cells ids + swap-cells loop ;
: reset-ids         sequence ids 5 cells move ;
: permute           mark increment reset-ids apply ;
: permute?          num-permutes @ 120 < ;

: solution          dup .. most-signal @ > if most-signal ! ids best-perm 5 cells move else drop then ;
: solve             begin permute? while
                          restore-thrusters ignite 0 run-thrusters solution permute
                    repeat ;
s" 3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0" 2constant test-input1
: test-1            test-input1 setup solve dup 43210 = . .result ;

s" 3,23,3,24,1002,24,10,24,1002,23,-1,23,101,5,23,23,1,24,23,23,4,23,99,0,0" 2constant test-input2
: test-2            test-input2 setup solve dup 54321 = . .result ;

s" 3,31,3,32,1002,32,10,32,1001,31,-2,31,1007,31,0,33,1002,33,7,33,1,33,31,31,1,32,31,31,4,31,99,0,0,0" 2constant test-input3
: test-3            test-input3 setup solve dup 65210 = . .result ;

: part-1            0 is perm-offset read-input setup solve .result ;

( part 2)
: solve-2           begin permute? while cr ." iteration: " num-permutes @ . cr ids .seq ." , " most-signal @ .
                          restore-thrusters ignite 0 loop-thrusters solution permute
                    repeat ;
s" 3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5" 2constant test-input4
: test-4            5 is perm-offset test-input4 setup solve-2 .result ;

s" 3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,-5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10" 2constant test-input5
: test-5            5 is perm-offset test-input5 setup solve-2 .result ;

: part-2            5 is perm-offset read-input setup solve-2 .result ;


