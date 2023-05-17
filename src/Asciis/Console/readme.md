# Console


`ConsoleKey` : Standard Key on a console                 https://learn.microsoft.com/en-us/dotnet/api/system.consolekey

`ConsoleKeyInfo` : ConsoleKey with modifiers split out   https://learn.microsoft.com/en-us/dotnet/api/system.consolekeyinfo

`VirtualKeyCode` : Windows virtualised keys              https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes

`Key` : Windows input key                                https://learn.microsoft.com/en-us/dotnet/api/system.windows.input.key

Console.ReadKey will return ConsoleKeyInfo on all desktop Platforms. Unsupported for Browser + mobile platforms.



MapKeyToConsoleKey
    : Mapping a Windows input key to ConsoleKey

MapCharToUnicode
    : map a character (0..255) to a printable unicode character.

MapVirtualKeyToConsoleKey
    : map a Windows virtual key to ConsoleKey
