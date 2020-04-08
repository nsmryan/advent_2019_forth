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

: .stack          stack-depth @ 0 ?do memory i cells + @ . space loop ;

: stack-test      stack% 3 deep %allot stack ! stack @ 3 stack-init
                  assert( 3 stack-max-depth @ = )
                  assert( 0 stack-depth @ = )
                  1 push-stack
                  2 push-stack
                  3 push-stack
                  assert( pop-stack 3 = ) 
                  assert( pop-stack 2 = ) 
                  assert( pop-stack 1 = ) ;

