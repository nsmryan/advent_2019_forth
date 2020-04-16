
: 1+!        dup @ 1 + swap ! ;
: 1-!        dup @ 1 - swap ! ;

: 2+!        dup @ 2 + swap ! ;
: 2-!        dup @ 2 - swap ! ;

: .. dup . ;

: 2cell     2 cells ;
: 2cells    2cell * ;
: 2cell%    2cell 2cell ;

\ a * b =
\ (a0 + a1) * (b0 + b1) =
\ a0 * b0 + a0 * b1 + a1 * b0 + a1 * b1
\ a0 * (b0 + b1) + a1 * (b0 + b1)

variable small1
variable small2
variable big1
variable big2
: dmult    small1 @ small2 @ m*
           big1   @ big2   @ * 0 swap 
           small1 @ big2   @ * 0 swap 
           small2 @ big1   @ * 0 swap 
           d+ d+ d+ ;

: d*         big2 ! small2 ! big1 ! small1 ! dmult ;

: dfastmult  small1 big1 small2 m*
             0 big2 small1 * d+ ;
: dfast*     big2 ! small2 ! big1 ! small1 ! dmult ;
