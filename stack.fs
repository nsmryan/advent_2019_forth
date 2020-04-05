include test.fs

struct
  cell% field stack-depth 
  cell% field stack-max-depth 
  cell% field stack-data
end-struct stack

: deep        here >r dup >r 1 - cells + %allot r> r> stack-max-depth ! ;
( create *stack name* stack 10 deep )


: check-incr dup stack-depth @ swap 1 + stack-max-depth = if 100 throw then ;
: check-decr dup stack-depth @ 0= if 100 throw then ;

: stack-incr stack-depth dup @ 1 + swap ! ;
: stack-decr stack-depth dup @ 1 - swap ! ;

( n stack -- )
: push-stack  tuck check-incr  over dup stack-depth cells swap stack-data + ! stack-incr ;

( stack -- n )
: push-stack  dup check-decr  dup stack-decr dup stack-depth cells swap stack-data + @ ;

