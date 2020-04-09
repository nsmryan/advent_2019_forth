include utils.fs

struct
  cell% field >stack-depth 
  cell% field >stack-max-depth 
  cell% field >stack-data
end-struct stack%

variable stack
: stack-depth     stack @ >stack-depth ;
: stack-max-depth stack @ >stack-max-depth ;
: stack-data      stack @ >stack-data ;

: deep            cells + ;
: stack-init      ( stack depth -- )
                  2dup  cells stack% nip cells + 0 fill
                  swap  >stack-max-depth ! ;


: pop-stack?      stack-depth @ 0 > ;
: push-stack?     stack-depth @ 1 + stack-max-depth @ <= ;

: check-push      assert( push-stack? ) ;
: check-pop       assert( pop-stack? ) ;

: stack-incr      stack-depth 1+! ;
: stack-decr      stack-depth 1-! ;

: stack-cell      ( stack -- *cell )
                  stack-depth @ cells stack-data + ;

: push-stack      ( n stack -- ) check-push
                  stack-cell !  stack-incr ;

: pop-stack       ( stack -- n ) check-pop
                  stack-decr stack-cell @ ;

: clear-stack     0 stack-depth ! ;
: index-stack     cells stack-data + ;
: revdex-stack    cells stack-depth @ 1- swap - cells stack-data + ;
: pair@           dup index-stack @ swap revdex-stack @ ;
: pair!           tuck index-stack ! revdex-stack ! ;
: rev-stack       stack-depth @ 2 / 0 ?do i pair@ i pair! loop ;

: .stack-info     ." depth: " stack-depth @ . ." , max: " stack-max-depth @ . ;
: .stack          cr .stack-info cr stack-depth @ 0 ?do i index-stack @ . space loop ;

: stack-test      stack% 3 deep %allot stack ! stack @ 3 stack-init
                  assert( 3 stack-max-depth @ = )
                  assert( 0 stack-depth @ = )
                  1 push-stack
                  2 push-stack
                  3 push-stack
                  rev-stack
                  assert( pop-stack 1 = ) 
                  assert( pop-stack 2 = ) 
                  assert( pop-stack 3 = ) ;

