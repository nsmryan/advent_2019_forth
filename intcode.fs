include utils.fs
include ring.fs


( intcode definition )
2048 constant #memory
128 constant #input
128 constant #output
4 constant #modes
0 constant POS-MODE
1 constant IMM-MODE

0 constant INT_RUNNING
1 constant INT_DONE
2 constant INT_NEED_INPUT
3 constant INT_ERROR

1 constant ADD-CODE
2 constant MULT-CODE
3 constant IN-CODE
4 constant OUT-CODE
5 constant JEQ-CODE
6 constant JNEQ-CODE
7 constant LT-CODE
8 constant EQ-CODE
99 constant END-CODE

: memory%       1 #memory cells ;
: in-ring%      ring% #input long ;
: out-ring%     ring% #output long ;
: mode-ring%    ring% #modes long ;

struct
  cell% field >ip
  cell% field >cycles
  cell% field >cells-used
  cell% field >instr
  cell% field >state
  in-ring% field >in-ring
  out-ring% field >out-ring
  mode-ring% field >mode-ring
  memory% field >memory
end-struct intcode%

: #intcode          intcode% nip ;
: intcode0          #intcode 0 fill ;
: intcode-init      dup intcode0
                    dup >in-ring   >>ring #input  ring-init
                    dup >out-ring  >>ring #output ring-init
                        >mode-ring >>ring #modes  ring-init ;


variable intcode
: >>intcode         intcode ! ;
: ip                intcode @ >ip ;
: cycles            intcode @ >cycles ;
: cells-used        intcode @ >cells-used ;
: instr             intcode @ >instr ;
: state             intcode @ >state ;
: in-ring           intcode @ >in-ring ;
: out-ring          intcode @ >out-ring ;
: mode-ring         intcode @ >mode-ring ;
: memory            intcode @ >memory ;

: intcode-new       intcode% %allot dup intcode-init >>intcode ;

variable original
here original ! intcode% %allot intcode-init

: restore         original @ swap intcode% nip move ;

: store           cells memory + ! ;
: load            cells memory + @ ;

: ip@             ip @ load ;
: ip!             ip @ store ;
: ip++            ip 1+! ;
: ip!++           ip! ip++ ;
: ip@++           ip@ ip++ ;
: ip--            ip 1-! ;

: intcode-in      in-ring >>ring ring-push ;
: intcode-out     out-ring >>ring ring-pop ;

: .ip             ." ip: " ip @ . ." = " ip@ . cr ;
: .memory         0 begin dup cells-used @ 32 min < while dup load . 1+ repeat drop ;
: .cells-used     ." cells: " cells-used @ . ;
: .input          ." input:" cr in-ring >>ring .ring ;
: .output         ." output:" cr out-ring >>ring .ring ;
: .intcode        cr .ip cr .cells-used cr .memory cr .input cr .output ;

: .current-cell   ip @ . ;
: .output         out-ring >>ring .ring ;

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

: init-intcode    intcode% %allot  >>intcode  intcode @ restore ;
: init-parser     #program ! program !  program @ cursor ! ;

: start-intcode   init-parser  original @ >>intcode  parse-program init-intcode ;

( instruction decoding )
: push-mode       mode-ring >>ring ring-push ;
: pop-mode        mode-ring >>ring ring-pop? if ring-pop else 0 then ;
: split-mode      10 /mod swap ;
: split-op        100 /mod swap ;
: init-decode     mode-ring >>ring ring-clear ;
: decode          split-op instr ! begin dup 0 > while split-mode push-mode repeat drop ;
: decode-instr    init-decode decode ;

( running )
: running?        state @ INT_RUNNING = ;
: need-input      INT_NEED_INPUT state ! ;
: error-mode      INT_ERROR state ! ;

: arg             ip@++  pop-mode POS-MODE = if load then ;
: dst!            ip@++ store ;
: op-add          arg arg + dst! ;
: op-mult         arg arg * dst! ;
: op-input        in-ring >>ring ring-pop? if ring-pop dst! else ip-- need-input then ;
: op-output       arg out-ring >>ring ring-push ;
: op-jeq          arg if arg ip ! else ip++ then ;
: op-jneq         arg 0 = if arg ip ! else ip++ then ;
: op-lt           arg arg < if 1 else 0 then dst! ;
: op-eq           arg arg = if 1 else 0 then dst! ;
: op-end          INT_DONE state ! ;

: exec            case
                    ADD-CODE  of op-add    endof
                    MULT-CODE of op-mult   endof
                    IN-CODE   of op-input  endof
                    OUT-CODE  of op-output endof
                    JEQ-CODE  of op-jeq    endof
                    JNEQ-CODE of op-jneq   endof
                    LT-CODE   of op-lt     endof
                    EQ-CODE   of op-eq     endof
                    END-CODE  of op-end    endof
                    dup ." unexpected opcode! " . ." at: " .current-cell cr error-mode
                  endcase ;

: step            ip@++  decode-instr  instr @ exec  cycles 1+! ;
: run             begin running? while step repeat ;


( intcode assembler )
: asm               ;
: arg-order         swap rot ;
: ,add              ADD-CODE ip!++ arg-order ip!++ ip!++ ip!++
                    ip @ cells-used ! ;
: ,mult             MULT-CODE ip!++ arg-order ip!++ ip!++ ip!++ 
                    ip @ cells-used ! ;
: ,in               IN-CODE ip!++ ip!++
                    ip @ cells-used ! ;
: ,out              OUT-CODE ip!++ ip!++
                    ip @ cells-used ! ;
: ,end              END-CODE ip!++
                    ip @ cells-used ! ;
: ,jeq              JEQ-CODE ip!++ swap ip!++ ip!++ ;
: ,jneq             JNEQ-CODE ip!++ swap ip!++ ip!++ ;
: ,lt               LT-CODE ip!++ arg-order ip!++ ip!++ ip!++ ;
: ,eq               EQ-CODE ip!++ arg-order ip!++ ip!++ ip!++ ;
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
: dis-jeq           ." jeq " offset++ .arg .arg cr ;
: dis-jneq          ." jneq " offset++ .arg .arg cr ;
: dis-lt            ." lt " offset++ .arg .arg .arg cr ;
: dis-eq            ." eq " offset++ .arg .arg .arg cr ;
: dis-exit          ." .exit " cr offset++ ;

: disop             offset@ case
                      ADD-CODE  of dis-add    endof
                      MULT-CODE of dis-mult   endof
                      IN-CODE   of dis-input  endof
                      OUT-CODE  of dis-output endof
                      JEQ-CODE  of dis-jeq    endof
                      JNEQ-CODE of dis-jneq   endof
                      LT-CODE   of dis-lt     endof
                      EQ-CODE   of dis-eq     endof
                      END-CODE  of dis-exit   endof
                      dup cr ." unexpected opcode! " . ." at: " .current-cell cr
                    endcase ;

: disassemble       begin offset @ cells-used @ < while disop repeat ;
: dis-header        cr ." intcode disassembly" cr ." cells used: " cells-used @ . cr ;
: disasm            0 offset ! dis-header disassemble ;

( example intcode program )
: ex intcode-new asm 1 1 1 3 ,add 2 4 4 0 ,mult ,end end-asm ;
