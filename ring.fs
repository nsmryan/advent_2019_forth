include utils.fs


( gets number of storage cells. the 1+ is for the dummy cell )
: long             1+ 2cells + ;

struct
  cell% field >ring-write
  cell% field >ring-read
  cell% field >ring-depth
  cell% field >ring-data
end-struct ring%

variable ring
: >>ring           ring ! ;
: ring-write       ring @ >ring-write ;
: ring-read        ring @ >ring-read ;
: ring-depth       ring @ >ring-depth ;
: ring-data        ring @ >ring-data ;
: ring-write-cell  ring-data ring-write @ 2cells + ;
: ring-read-cell   ring-data ring-read @ 2cells + ;

: ring-incr        1 + ring-depth @ mod ;

: ring-write++     ring-write @ ring-incr ring-write ! ;
: ring-read++      ring-read @ ring-incr ring-read ! ;

: %ring-allot      ( depth -- a ) ring% rot 1+ 2cells + %allot ;
: ring-init        ( depth -- )
                   dup 1+ ring-depth !  ring-read ! 0 ring-write ! ;
\ : ring-init        ( depth -- )
\                    1+ dup 2cells ring% nip + ring @ swap 0 fill 
\                        ring-depth ! 
\                        ring-depth @ 1- ring-read ! ;

: ring-new         dup %ring-allot ring ! ring-init ;
: ring-push?       ring-write @ ring-read @ <> ;
: assert-ring-push assert( ring-push? ) ;
: ring-push        assert-ring-push ring-write-cell 2! ring-write++ ;

: ring-pop?        ring-read @ ring-incr ring-write @ <> ;
: assert-ring-pop  assert( ring-pop? ) ;
: ring-pop         assert-ring-pop ring-read++ ring-read-cell 2@ ;

: ring-clear       ring-depth @ 1- ring-init ;

: .ring-data       ring-read @ begin ring-incr dup ring-write @ <> while dup 2cells ring-data + 2@ d. repeat drop ;
: .ring            ." ring: " cr
                   ." read: " ring-read @ . cr
                   ." write " ring-write @ . cr 
                   .ring-data cr ;

: ring-test        3 ring-new
                   assert( ring-depth @ 4 = )
                   assert( ring-write @ 0 = )
                   assert( ring-read  @ 3 = )
                   1 s>d ring-push
                   2 s>d ring-push
                   3 s>d ring-push
                   assert( ring-pop 1 s>d d= )
                   assert( ring-pop 2 s>d d= )
                   assert( ring-pop 3 s>d d= )
                   88 s>d ring-push
                   assert( ring-pop d>s 88 = )
                   100 s>d ring-push
                   assert( ring-pop d>s 100 = )
                   12 s>d ring-push
                   assert( ring-pop 12 s>d d= ) 
                   1 s>d ring-push
                   2 s>d ring-push
                   ring-clear
                   assert( ring-depth @ 4 = )
                   assert( ring-write @ 0 = )
                   assert( ring-read  @ 3 = )
                   cr ." ring buffer tests passed!" cr ;

