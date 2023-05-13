using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Asciis.Terminal.ConsoleDrivers.CursesDriver;

/// <summary>
/// Represents a dynamically loaded unmanaged library in a (partially) platform independent manner.
/// First, the native library is loaded using dlopen (on Unix systems) or using LoadLibrary (on Windows).
/// dlsym or GetProcAddress are then used to obtain symbol addresses. <c>Marshal.GetDelegateForFunctionPointer</c>
/// transforms the addresses into delegates to native methods.
/// See http://stackoverflow.com/questions/13461989/p-invoke-to-dynamically-loaded-library-on-mono.
/// </summary>
internal class UnmanagedLibrary
{
    public static bool IsWindows;
    public static bool IsLinux;
    public static bool IsMacOS;
    public static bool Is64Bit;
    public static bool IsMono;

    static UnmanagedLibrary()
    {
        IsMacOS = OperatingSystem.IsMacOS();
        IsLinux = OperatingSystem.IsLinux();
        IsWindows = OperatingSystem.IsWindows();
        Is64Bit = Marshal.SizeOf(typeof(IntPtr)) == 8;
        IsMono = Type.GetType("Mono.Runtime") != null;
    }

    // flags for dlopen
    private const int RTLD_LAZY = 1;
    private const int RTLD_GLOBAL = 8;

    private readonly string? libraryPath;
    private readonly IntPtr handle;

    public IntPtr NativeLibraryHandle => handle;

    //
    // if isFullPath is set to true, the provided array of libraries are full paths
    // and are tested for the file existing, otherwise the file is merely the name
    // of the shared library that we pass to dlopen
    //
    public UnmanagedLibrary(string[] libraryPathAlternatives, bool isFullPath)
    {
        if (isFullPath)
        {
            libraryPath = FirstValidLibraryPath(libraryPathAlternatives);
            handle = PlatformSpecificLoadLibrary(libraryPath);
        }
        else
        {
            foreach (var lib in libraryPathAlternatives)
            {
                handle = PlatformSpecificLoadLibrary(lib);
                if (handle != IntPtr.Zero)
                    break;
            }
        }

        if (handle == IntPtr.Zero)
            throw new IOException(string.Format("Error loading native library \"{0}\"", libraryPath));
    }

    /// <summary>
    /// Loads symbol in a platform specific way.
    /// </summary>
    public IntPtr LoadSymbol(string symbolName)
    {
        if (IsWindows)
        {
            // See http://stackoverflow.com/questions/10473310 for background on this.
            if (Is64Bit)
                return Windows.GetProcAddress(handle, symbolName);

            // Yes, we could potentially predict the size... but it's a lot simpler to just try
            // all the candidates. Most functions have a suffix of @0, @4 or @8 so we won't be trying
            // many options - and if it takes a little bit longer to fail if we've really got the wrong
            // library, that's not a big problem. This is only called once per function in the native library.
            symbolName = "_" + symbolName + "@";
            for (var stackSize = 0; stackSize < 128; stackSize += 4)
            {
                var candidate = Windows.GetProcAddress(handle, symbolName + stackSize);
                if (candidate != IntPtr.Zero) return candidate;
            }

            // Fail.
            return IntPtr.Zero;
        }

        if (IsLinux)
        {
            if (IsMono) 
                return Mono.dlsym(handle, symbolName);

            return CoreCLR.dlsym(handle, symbolName);

            // .net Framework
            //return Linux.dlsym(handle, symbolName);
        }

        if (IsMacOS) 
            return MacOSX.dlsym(handle, symbolName);

        throw new InvalidOperationException("Unsupported platform.");
    }

    public T GetNativeMethodDelegate<T>(string methodName)
        where T : class
    {
        var ptr = LoadSymbol(methodName);
        if (ptr == IntPtr.Zero)
            throw new MissingMethodException(string.Format("The native method \"{0}\" does not exist", methodName));
        return Marshal.GetDelegateForFunctionPointer<T>(ptr); // non-generic version is obsolete
    }

    /// <summary>
    /// Loads library in a platform specific way.
    /// </summary>
    private static IntPtr PlatformSpecificLoadLibrary(string libraryPath)
    {
        if (IsWindows) 
            return Windows.LoadLibrary(libraryPath);

        if (IsLinux)
        {
            if (IsMono) 
                return Mono.dlopen(libraryPath, RTLD_GLOBAL + RTLD_LAZY);

            return CoreCLR.dlopen(libraryPath, RTLD_GLOBAL + RTLD_LAZY);

            // .Net Framework
            // return Linux.dlopen(libraryPath, RTLD_GLOBAL + RTLD_LAZY);
        }

        if (IsMacOS) 
            return MacOSX.dlopen(libraryPath, RTLD_GLOBAL + RTLD_LAZY);

        throw new InvalidOperationException("Unsupported platform.");
    }

    private static string FirstValidLibraryPath(string[] libraryPathAlternatives)
    {
        foreach (var path in libraryPathAlternatives)
        {
            if (File.Exists(path))
                return path;
        }
        throw new FileNotFoundException(
            string.Format(
                "Error loading native library. Not found in any of the possible locations: {0}",
                string.Join(",", libraryPathAlternatives)));
    }

    private static class Windows
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadLibrary(string filename);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
    }

    private static class Linux
    {
        [DllImport("libdl.so", CharSet = CharSet.Unicode)]
        internal static extern IntPtr dlopen(string filename, int flags);

        [DllImport("libdl.so", CharSet = CharSet.Unicode)]
        internal static extern IntPtr dlsym(IntPtr handle, string symbol);
    }

    private static class MacOSX
    {
        [DllImport("libSystem.dylib", CharSet = CharSet.Unicode)]
        internal static extern IntPtr dlopen(string filename, int flags);

        [DllImport("libSystem.dylib", CharSet = CharSet.Unicode)]
        internal static extern IntPtr dlsym(IntPtr handle, string symbol);
    }

    /// <summary>
    /// On Linux systems, using using dlopen and dlsym results in
    /// DllNotFoundException("libdl.so not found") if libc6-dev
    /// is not installed. As a workaround, we load symbols for
    /// dlopen and dlsym from the current process as on Linux
    /// Mono sure is linked against these symbols.
    /// </summary>
    private static class Mono
    {
        [DllImport("__Internal", CharSet = CharSet.Unicode)]
        internal static extern IntPtr dlopen(string filename, int flags);

        [DllImport("__Internal", CharSet = CharSet.Unicode)]
        internal static extern IntPtr dlsym(IntPtr handle, string symbol);
    }

    /// <summary>
    /// Similarly as for Mono on Linux, we load symbols for
    /// dlopen and dlsym from the "libcoreclr.so",
    /// to avoid the dependency on libc-dev Linux.
    /// </summary>
    private static class CoreCLR
    {
#if NET5_0_OR_GREATER
        // Custom resolver to support true single-file apps
        // (those which run directly from bundle; in-memory).
        //     -1 on Unix means self-referencing binary (libcoreclr.so)
        //     0 means fallback to CoreCLR's internal resolution
        // Note: meaning of -1 stay the same even for non-single-file form factors.
        static CoreCLR()
        {
            NativeLibrary.SetDllImportResolver(
                typeof(CoreCLR).Assembly,
                (string libraryName, Assembly assembly, DllImportSearchPath? searchPath) =>
                    libraryName == "libcoreclr.so" ? (IntPtr)(-1) : IntPtr.Zero);
        }
#endif

        [DllImport("libcoreclr.so", CharSet = CharSet.Unicode)]
        internal static extern IntPtr dlopen(string filename, int flags);

        [DllImport("libcoreclr.so", CharSet = CharSet.Unicode)]
        internal static extern IntPtr dlsym(IntPtr handle, string symbol);
    }
}
