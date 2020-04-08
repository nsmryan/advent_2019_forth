include common.fs
include intcode.fs

( utilities )
: .. dup . ;
: 1+! dup @ 1 + swap ! ;

( handle input )
s" day2.fs" 2constant solution-file-name
s" input_day2.txt" 2constant input-file-name

: reload solution-file-name included ;

( part 1 )
: .result           0 load . ;
: setup             input-file-name ingest text @ #text @ start-intcode ;

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
: part-2            setup begin not-done while intcode @ restore  pair poke run next-pair repeat .result ;

