# AsciiApp

## CanvasLoop

The same concept as any game loop. The canvas loop continuously runs, monitoring input from stdIn, and updating the window whenever the canvas changes.

Each loop iteration, the following happens:

- Execute active Timers
- Check if window has changed dimensions
- Process and input (keyboard, mouse)
- Process all visible windows
- Execute and Idle timers

## Modes

Canvas or Graphics. This is where the app simply writes to the canvas each loop.  All control is up to the caller

Control Mode. Add controls to the screen and let the engine control mouse, focus, input and updating the screen.

Combined. Why not both. 

## Timers

There are different types of timers to execute code at different times.

- One-off Timeouts. Run some code after an amount of time. 
- Regular Timeouts. Run some code regularly each amount of time.
- Idle. Run some code on the next canvas loop

## Windows

A window is the container and processor of a canvas. 
Each canvas loop, any updates are written out to the console.
