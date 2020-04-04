( handle input )
variable input-fd
variable input
variable #input

: read-input-file  input @ #input @ input-fd @ read-file throw drop ;
: allocate-file    here input !  input-fd @ file-size throw drop dup #input !  allot ;
: open-input       r/o open-file throw input-fd ! ;
: ingest           open-input allocate-file read-input-file ;

