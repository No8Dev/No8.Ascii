using System.Runtime.InteropServices;

namespace Asciis.UI.Terminals;

public static class Mono
{
    [DllImport("__Internal")]
    public static extern IntPtr dlopen(string filename, DlOpenFlags flags);

    [DllImport("__Internal")]
    public static extern IntPtr dlsym(IntPtr handle, string symbol);
}
