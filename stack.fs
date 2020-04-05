include test.fs

struct
  cell% field >stack-depth 
  cell% field >stack-max-depth 
  cell% field >stack-data
end-struct stack%

: deep        cells + ;
: stack-init  ( stack depth -- )
              2dup  cells stack% nip cells + 0 fill
              swap  >stack-max-depth ! ;


: check-push dup assert( dup >stack-depth @ 1 + swap >stack-max-depth <= ) ;
: check-pop  dup assert( >stack-depth @ 0 > ) ;

: stack-incr >stack-depth dup @ 1 + swap ! ;
: stack-decr >stack-depth dup @ 1 - swap ! ;

: stack-cell ( stack -- *cell )
             dup >stack-depth cells swap >stack-data + ;

: push-stack  ( n stack -- ) check-push
              tuck stack-cell !  stack-incr ;

: pop-stack  ( stack -- n ) check-pop
             dup stack-decr stack-cell @ ;

