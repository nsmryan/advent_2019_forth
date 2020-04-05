include stack.fs

( utilities )
: .. dup . ;
: 1+! dup @ 1 + swap ! ;

( intcode definition )
2048 constant #memory
128 constant #input
128 constant #output

0 constant INT_RUNNING
1 constant INT_DONE
2 CONSTANT INT_NEED_INPUT

: memory%       1 #memory cells ;
: in-stack%     stack% #input deep ;
: out-stack%    stack% #output deep ;

struct
  cell% field >ip
  cell% field >cycles
  cell% field >cells-used
  in-stack% field >in-stack
  out-stack% field >out-stack
  memory% field >memory
end-struct intcode%

: intcode0          intcode% nip cells 0 fill ;
: intcode-init      dup intcode0
                    dup >in-stack  #input  stack-init
                    dup >out-stack #output stack-init ;

variable intcode
: ip                intcode >ip ;
: cycles            intcode >cycles ;
: cells-used        intcode >cells-used ;
: in-stack          intcode >in-stack ;
: out-stack         intcode >out-stack ;
: memory            intcode >memory ;

variable original
here original ! intcode% %allot intcode-init

: restore           original @ swap intcode% nip move ;

: store             cells memory + ! ;
: load              cells memory + @ ;

: ip@               ip @ load ;
: ip++              ip 1+! ;

: .ip               ." ip: " ip @ . ." = " ip@ . cr ;
: .memory           0 begin dup cells-used @ < while dup load . 1+ repeat drop ;
: .cells-used       ." cells: " cells-used @ . ;
: .intcode          cr .ip cr .cells-used cr .memory cr  ;

( parsing )
variable program
variable #program
variable cursor

: store-code        cells-used @ cells memory + !  cells-used 1+! ;
: next-code         cursor @ + 1 + cursor ! ;         
: parse-cell        s>number? drop drop ;
: read-code         cursor @ over parse-cell swap next-code ;
: -end              cursor @  program @ #program @ +  >= if 0 rdrop then ;
: code-length       -end 0 begin dup cursor @ + c@ [char] , <> while 1 + repeat ;
: parse-program     begin code-length dup while read-code store-code repeat drop ;

: init              intcode% %allot  intcode !  restore ;

: start-intcode     #program ! program !  original @ intcode !  parse-program init ;

( running )
: not-done          ip@ 99 <> ;

: arg               ip@ ip++ ;
: op-add            arg load arg load + arg store ;
: op-mult           arg load arg load * arg store ;
: op-input          ;
: op-output         ;

: exec              case
                      1 of op-add    endof
                      2 of op-mult   endof
                      3 of op-input  endof
                      4 of op-output endof
                      dup ." Unexpected opcode! " . cr
                    endcase ;

: step              ip@  ip++  exec  cycles 1+! ;
: run               begin not-done while step repeat ;

: poke              2 store  1 store ;

