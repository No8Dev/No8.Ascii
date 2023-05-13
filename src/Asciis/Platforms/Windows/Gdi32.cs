using System.Runtime.InteropServices;

namespace No8.Ascii.Platforms;

public static partial class Windows
{
    public static class Gdi32
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern int AddFontResourceEx(string lpszFilename, uint fl, IntPtr pdv);
    }

}