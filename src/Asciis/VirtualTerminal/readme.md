# Virtual Terminal


## ExCon Extended Console

ExCon is the main interface into the Ascii library.

## ConSeq (consequences)

Low level console sequence parsing of bytes coming in via StdIn.

Console Sequences have a standard format

## TerminalSeq

TerminalSeq is a static utility class producing console sequences for for StdOut.

# ExConsole

**ExConsole** is an extension on top of Console with a heap of additional support for Terminal sequences, Runes with a mix of awesome.

- Traps posix events like Ctrl-C
- Keyboard events
- Mouse input
- Window resize
- Alternative screen

Most extensions are generic across all OS and terminals, but different terminal emulators may have different support for different features. Your mileage will vary. 

## ExConsole.Character 

Character attributes like underline, reverse..

## ExConsole.Cursor

Cursor location and attributes

## ExConsole.Editing

Insert + Delete characters

Insert + Delete lines

Apply character attributes

## ExConsole.Mouse

Mouse control

## ExConsole.Screen

Screen control.  

## ExConsole.Scroll 

Control scrolling

## ExConsole.Special

Special terminal extensions