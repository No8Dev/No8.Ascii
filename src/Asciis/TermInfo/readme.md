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
