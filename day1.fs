10 emit

variable input-fd

: open-input r/o open-file throw input-fd ! ;


256 constant MAX_LINE
create line MAX_LINE allot

: fill-line line  line MAX_LINE input-fd @ read-line throw ;


: start s" input_day1.txt" open-input ;

: mass>fuel 3 / 2 - ;

variable total-fuel
0 total-fuel !

: +fuel total-fuel @ + total-fuel ! ;

: get-fuel s>number? -1 <> if s" Could not convert number!" type then drop mass>fuel ;

: fuel-fuel dup begin mass>fuel dup 0 > while tuck + swap repeat drop ;

: .result total-fuel @ . ;

: part-1 start begin fill-line while get-fuel +fuel repeat .result ;
: part-2 start begin fill-line while get-fuel fuel-fuel +fuel repeat .result ;

: reload s" day1.fs" included ;
