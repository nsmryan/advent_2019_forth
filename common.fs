( handle input )
variable text-fd
variable text
variable #text

: read-input-file  text @ #text @ text-fd @ read-file throw drop ;
: allocate-file    here text !  text-fd @ file-size throw drop dup #text !  allot ;
: open-input       r/o open-file throw text-fd ! ;
: ingest           open-input allocate-file read-input-file ;

