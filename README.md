# Advent of Code 2019 Intcode Computer in Forth
This repository contains an implementation of the Advent of Code 2019 intcode computer,
written in Forth. I had previously completed Advent of Code 2019 in Rust, and I wanted
to see what it would look like in Forth instead.


Having a completed intcode computer means that it passes the tests on day 9, which includes
large numbers and relative instructions.


I expect that there are not particularly many solutions to Advent of Code in Forth, and
while I only did the intcode parts, and did not go past day 9, I'm still a bit proud
of my solution.


## Style
One note about style- the Forth programming style I use is inspired by Samuel Falvo II's,
especially his video [Over the Shoulder Forth](https://www.youtube.com/watch?v=mvrE2ZGe-rs).
This means that most definitions are one line, there are relatively few stack comments,
and definitions that go together are aligned. I find this a very pleasing style, although
I could have benefitted some a couple more comments.


I point this style out because I have often see very long Forth words, often with stack comments
on every line, and I find this very difficult to understand. Small factoring is part of what I
like about Forth, and I try to factor into tiny pieces, with the line length limit as a good
indicator of when a word is getting too long.

## Design Notes
I have generally found with Forth that I have trouble when I start to reach for more complex and
nested definitions. In some sense, perhaps this is because Forth is not meant for this style of
programming, but I still try. In this project, I came up with several strategies for dealing with
increasing the size and complexity of my Forth code, which I felt very good about.


### Data Structure Pointers
There is one strategy for writing larger Forth programs that I came up with during this project that
I found very helpful. I started to need some structures like stacks and ring buffers, and while
these could be done in a custom fashion (special words for each one) I decided to implement them
as a data structure with words to operate on them.


What I found is that definitions that rely on a 'self' or 'this' pointer
(in other words, the current data structure being operated one) are much simplier then trying
to pass pointers to structures on the stack. I need all the stack space available, as Forth is
very pleasant with 1-3 items on the stack, its okay when using double width items (2swap, 2drop, etc),
but more than that creates constant difficulties. Adding a pointer to a data structure means that many
normal situations where there is a reasonable stack depth end up with one additional item on the stack
(the data structure pointer) which causes the code to fall off the cliff of comfortable stack use.


By storing the data structure pointer in a variable (in this code, the name of the variable is the
name of the data structure), keeps extra data off the stack. This means that any data structure words,
like pushing to a ring buffer, requires setting the current ring buffer, but if several functions for
that structure are used between switching, this is still much nicer than other strategies I've tried.


### Field Accessors
The other thing to note about data structures in this code base is that 1) they are defined with gforth
structs, and their fields are are not use directly. Instead, a word is defined for each field which gets
the current structure (the 'this', or currently active structure), and offsets from that. This means
that these extra words get direct access to fields of the structure, which is very convenient.


I also frequently defined special words for setting and getting the values of fields if there were
special rules- see ip@, ip@++, ring-read-cell, or ring-write-cell as examples.


### Tests
When implementing a series of data structure words, one thing that I found useful was to write a word
that performs a simple test as a series of asserts. There is nothing novel about this, but I haven't done
this in Forth before. See ring-test in ring.fs as an example.

