using System.Runtime.InteropServices;

namespace Asciis.UI.Terminals;

public static partial class Windows
{
    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern int AddFontResourceEx(string lpszFilename, uint fl, IntPtr pdv);
}
