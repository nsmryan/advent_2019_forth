include utils.fs


( gets number of storage cells. the 1+ is for the dummy cell )
: long             1+ cells + ;

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
: ring-write-cell  ring-data ring-write @ cells + ;
: ring-read-cell   ring-data ring-read @ cells + ;

: ring-incr        1 + ring-depth @ mod ;

: ring-write++     ring-write @ ring-incr ring-write ! ;
: ring-read++      ring-read @ ring-incr ring-read ! ;

: %ring-allot      ( depth -- a ) ring% rot cells + %allot ;
: ring-init        ( depth -- )
                   1+ dup cells ring% nip + ring @ swap 0 fill 
                       ring-depth ! 
                       ring-depth @ 1- ring-read ! ;

: ring-new         dup %ring-allot ring ! ring-init ;
: ring-push?       ring-write @ ring-read @ <> ;
: assert-ring-push assert( ring-push? ) ;
: ring-push        assert-ring-push ring-write-cell ! ring-write++ ;

: ring-pop?        ring-read @ ring-incr ring-write @ <> ;
: assert-ring-pop  assert( ring-pop? ) ;
: ring-pop         assert-ring-pop ring-read++ ring-read-cell @ ;

: ring-clear       ring-depth @ 1- ring-init ;

: .ring-data       ring-read @ begin ring-incr dup ring-write @ <> while dup cells ring-data + @ . repeat drop ;
: .ring            ." ring: " cr
                   ." read: " ring-read @ . cr
                   ." write " ring-write @ . cr 
                   .ring-data cr ;

: ring-test        3 ring-new
                   assert( ring-depth @ 4 = )
                   assert( ring-write @ 0 = )
                   assert( ring-read  @ 3 = )
                   1 ring-push
                   2 ring-push
                   3 ring-push
                   assert( ring-pop 1 = )
                   assert( ring-pop 2 = )
                   assert( ring-pop 3 = )
                   88 ring-push
                   assert( ring-pop 88 = )
                   100 ring-push
                   assert( ring-pop 100 = )
                   12 ring-push
                   assert( ring-pop 12 = ) 
                   1 ring-push
                   2 ring-push
                   ring-clear
                   assert( ring-depth @ 4 = )
                   assert( ring-write @ 0 = )
                   assert( ring-read  @ 3 = )
                   cr ." ring buffer tests passed!" cr ;

