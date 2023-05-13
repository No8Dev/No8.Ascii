using System.Runtime.InteropServices;

namespace Asciis.UI.Terminals;

public static class MacOS
{
    [DllImport("libSystem.dylib")]
    public static extern IntPtr dlopen(string filename, DlOpenFlags flags);

    [DllImport("libSystem.dylib")]
    public static extern IntPtr dlsym(IntPtr handle, string symbol);

}
