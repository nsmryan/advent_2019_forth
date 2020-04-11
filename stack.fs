include utils.fs

struct
  cell% field >stack-depth 
  cell% field >stack-max-depth 
  cell% field >stack-data
end-struct stack%

variable stack
: >>stack         stack ! ;
: stack-depth     stack @ >stack-depth ;
: stack-max-depth stack @ >stack-max-depth ;
: stack-data      stack @ >stack-data ;

: deep            cells + ;
: stack-init      ( depth -- )
                  dup cells stack% nip + stack @ swap 0 fill
                  stack-max-depth ! ;

: stack-new       dup stack% rot deep %allot >>stack stack-init ;


: stack-pop?      stack-depth @ 0 > ;
: stack-push?     stack-depth @ 1 + stack-max-depth @ <= ;

: assert-push     assert( stack-push? ) ;
: assert-pop      assert( stack-pop? ) ;

: stack-incr      stack-depth 1+! ;
: stack-decr      stack-depth 1-! ;

: stack-cell      ( stack -- *cell )
                  stack-depth @ cells stack-data + ;

: stack-push      ( n stack -- ) assert-push
                  stack-cell !  stack-incr ;

: stack-pop       ( stack -- n ) assert-pop
                  stack-decr stack-cell @ ;

: clear-stack     0 stack-depth ! ;
: index-stack     cells stack-data + ;
: revdex-stack    cells stack-depth @ 1- swap - cells stack-data + ;
: pair@           dup index-stack @ swap revdex-stack @ ;
: pair!           tuck index-stack ! revdex-stack ! ;
: rev-stack       stack-depth @ 2 / 0 ?do i pair@ i pair! loop ;

: .stack-info     ." depth: " stack-depth @ . ." , max: " stack-max-depth @ . ;
: .stack          cr .stack-info cr stack-depth @ 0 ?do i index-stack @ . space loop ;

: stack-test      3 stack-new
                  assert( 3 stack-max-depth @ = )
                  assert( 0 stack-depth @ = )
                  1 stack-push
                  2 stack-push
                  3 stack-push
                  rev-stack
                  assert( stack-pop 1 = )
                  assert( stack-pop 2 = )
                  assert( stack-pop 3 = )
                  cr ." stack tests passed!" cr ;

