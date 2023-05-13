using System.Runtime.InteropServices;

namespace Asciis.UI.Terminals;

// ReSharper disable InconsistentNaming
[Flags]
public enum DlOpenFlags
{
    RTLD_LOCAL = 0x0_0000,
    RTLD_LAZY = 0x0_0001, /* Lazy function call binding.  */
    RTLD_NOW = 0x0_0002, /* Immediate function call binding.  */
    RTLD_BINDING_MASK = 0x0_0003, /* Mask of binding time value.  */
    RTLD_NOLOAD = 0x0_0004, /* Do not load the object.  */
    RTLD_DEEPBIND = 0x0_0008, /* Use deep binding.  */
    RTLD_GLOBAL = 0x0_0100, /* Load symbols globally */
    RTLD_NODELETE = 0x0_1000  /* Do not delete object when closed.  */
}
// ReSharper restore InconsistentNaming

// ReSharper disable once InconsistentNaming
public static class CoreCLR
{
    [DllImport("libcoreclr.so")]
    public static extern IntPtr dlopen(string filename, DlOpenFlags flags);

    [DllImport("libcoreclr.so")]
    public static extern IntPtr dlsym(IntPtr handle, string symbol);
}
