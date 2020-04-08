include utils.fs

2variable test-name
variable failures

: report     ." failed in test '" test-name 2@ type ." ' expected: " . ." was: " . ;

: assert-eq  2dup <> if failures 1+! cr ." assert-eq "  report else ." ." then ;
: assert-neq 2dup =  if failures 1+! cr ." assert-neq " report else ." ." then ;

: suite      0 failures ! cr ;
: end-suite  cr failures @ 0= if ." All tests passed!" else failures @ . ." failures" then ;
: test       cr 2dup type test-name 2! ;

(
suite
  s" first" test
    0 1 assert-neq
    1 1 assert-eq
  s" second" test
    0 0 assert-eq
  s" third" test
    0 1 assert-eq
end-suite
)
