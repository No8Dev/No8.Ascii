# TermInfo

```csharp
// Load the active terminfo description
var info = TermInfoDesc.Load();

// Load via terminfo name
var info = TermInfoDesc.Load("xterm-256color");

// Default capabilities are strongly typed
Debug.Assert(info.MaxColors == 256)

// But extended capabilities are also supported
bool? ax = info.Extended.GetBoolean("AX");
int? u8 = info.Extended.GetNum("U8");
string? kup = info.Extended.GetNum("kUP");
```

## Acknowledgements

Ported from https://github.com/spectreconsole/terminfo.git

That code was partly a port of https://github.com/xo/terminfo,
licensed under [MIT](https://github.com/xo/terminfo/blob/ca9a967f877831dd8742c136f5c19f82d03673f4/LICENSE).


## terminfo.src

### SCREEN Extensions:

The screen program uses the termcap interface.  It recognizes a few useful nonstandard capabilities. Those are used in this file.

       AX   (bool)  Does  understand  ANSI  set  default fg/bg color (\E[39m / \E[49m).
       G0   (bool)  Terminal can deal with ISO 2022  font  selection sequences.
       E0   (str)   Switch charset 'G0' back to standard charset.
       S0   (str)   Switch charset 'G0' to the specified charset.
       XT   (bool)  Terminal understands special xterm sequences  (OSC,  mouse
                    tracking).

```csharp
TermInfo.Extended.Exist("AX");
```

