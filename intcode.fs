include utils.fs
include stack.fs

( utilities )
: .. dup . ;

( intcode definition )
2048 constant #memory
128 constant #input
128 constant #output

0 constant INT_RUNNING
1 constant INT_DONE
2 constant INT_NEED_INPUT

1 constant ADD-CODE
2 constant MULT-CODE
3 constant IN-CODE
4 constant OUT-CODE
99 constant END-CODE

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
                        >out-stack #output stack-init ;


variable intcode
: ip                intcode @ >ip ;
: cycles            intcode @ >cycles ;
: cells-used        intcode @ >cells-used ;
: in-stack          intcode @ >in-stack ;
: out-stack         intcode @ >out-stack ;
: memory            intcode @ >memory ;

: intcode-new       intcode% %allot dup intcode-init intcode ! ;

variable original
here original ! intcode% %allot intcode-init

: restore         original @ swap intcode% nip move ;

: store           cells memory + ! ;
: load            cells memory + @ ;

: ip@             ip @ load ;
: ip!             ip @ store ;
: ip++            ip 1+! ;
: ip!++           ip! ip++ ;

: .ip             ." ip: " ip @ . ." = " ip@ . cr ;
: .memory         0 begin dup cells-used @ < while dup load . 1+ repeat drop ;
: .cells-used     ." cells: " cells-used @ . ;
: .input          ." in stack:" cr in-stack stack ! .stack ;
: .output         ." out stack:" cr out-stack stack ! .stack ;
: .intcode        cr .ip cr .cells-used cr .memory cr .input cr .output ;

( parsing )
variable program
variable #program
variable cursor

: store-code      cells-used @ cells memory + !  cells-used 1+! ;
: next-code       cursor @ + 1 + cursor ! ;         
: parse-cell      s>number? drop drop ;
: read-code       cursor @ over parse-cell swap next-code ;
: -end            cursor @  program @ #program @ +  >= if 0 rdrop then ;
: not-comma?      cursor @ + c@ [char] , <> ;
: code-length     -end 0 begin dup not-comma? while 1 + repeat ;
: parse-program   begin code-length dup while read-code store-code repeat drop ;

: init-intcode    intcode% %allot  intcode !  intcode @ restore ;
: init-parser     #program ! program !  program @ cursor ! ;

: start-intcode   init-parser  original @ intcode !  parse-program init-intcode ;

( running )
: not-done        ip@ END-CODE <> ;

: arg             ip@ ip++ ;
: op-add          arg load arg load + arg store ;
: op-mult         arg load arg load * arg store ;
: op-input        in-stack stack ! pop-stack arg store ;
: op-output       arg load out-stack stack ! push-stack ;

: exec            case
                    ADD-CODE  of op-add    endof
                    MULT-CODE of op-mult   endof
                    IN-CODE   of op-input  endof
                    OUT-CODE  of op-output endof
                    dup ." Unexpected opcode! " . cr
                  endcase ;

: step            ip@  ip++  exec  cycles 1+! ;
: run             begin not-done while step repeat ;

: poke            2 store  1 store ;

( intcode assembler )
: asm               ;
: arg-order         swap rot ;
: ,add              ADD-CODE ip!++ arg-order ip!++ ip!++ ip!++
                    ip @ cells-used ! ;
: ,mult             MULT-CODE ip!++ arg-order ip!++ ip!++ ip!++ 
                    ip @ cells-used ! ;
: ,end              END-CODE ip!++
                    ip @ cells-used ! ;
: end-asm           0 ip ! ;

( intcode disassembler )
variable offset
: offset@           offset @ load ;
: offset++          offset 1+! ;

: .arg              offset@ . space offset++ ;
: dis-add           ." add " offset++ .arg .arg .arg cr ;
: dis-mult          ." mult " offset++ .arg .arg .arg cr ;
: dis-input         ." in " offset++ .arg cr ;
: dis-output        ." out " offset++ .arg cr ;
: dis-exit          ." .exit " cr offset++ ;

: disop             offset@ case
                      ADD-CODE  of dis-add    endof
                      MULT-CODE of dis-mult   endof
                      IN-CODE   of dis-input  endof
                      OUT-CODE  of dis-output endof
                      END-CODE  of dis-exit   endof
                      dup ." Unexpected opcode! " . cr
                    endcase ;
: disassemble       begin offset @ cells-used @ < while disop repeat ;
: dis-header        cr ." intcode disassembly" cr ." cells used: " cells-used @ . cr ;
: disasm            0 offset ! dis-header disassemble ;

( example intcode program )
: ex intcode-new asm 1 1 1 3 ,add 2 4 4 0 ,mult ,end end-asm ;
