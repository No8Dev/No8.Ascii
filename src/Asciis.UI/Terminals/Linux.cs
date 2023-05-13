using System.Runtime.InteropServices;

namespace Asciis.UI.Terminals;

public static class Linux
{
    [DllImport("libdl.so")]
    public static extern IntPtr dlopen(string filename, DlOpenFlags flags);

    [DllImport("libdl.so")]
    public static extern IntPtr dlsym(IntPtr handle, string symbol);
}
