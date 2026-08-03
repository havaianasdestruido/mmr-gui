using System.Runtime.InteropServices;

namespace mmr_gui;

internal static class NativeProbe
{
    private static readonly Dictionary<string, IntPtr> _handles = new(StringComparer.OrdinalIgnoreCase);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr LoadLibraryW([MarshalAs(UnmanagedType.LPWStr)] string fileName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    public static IntPtr Load(string dllFileName)
    {
        if (_handles.TryGetValue(dllFileName, out var h) && h != IntPtr.Zero)
            return h;
        var loaded = LoadLibraryW(dllFileName);
        _handles[dllFileName] = loaded;
        return loaded;
    }

    public static bool DllLoads(string dllFileName) => Load(dllFileName) != IntPtr.Zero;

    public static int LoadError(string dllFileName)
    {
        if (_handles.TryGetValue(dllFileName, out var h) && h != IntPtr.Zero)
            return 0;
        var loaded = LoadLibraryW(dllFileName);
        _handles[dllFileName] = loaded;
        return loaded == IntPtr.Zero ? Marshal.GetLastWin32Error() : 0;
    }

    public static IntPtr GetExport(string dllFileName, string procName)
    {
        var h = Load(dllFileName);
        return h == IntPtr.Zero ? IntPtr.Zero : GetProcAddress(h, procName);
    }

    public static bool ExportExists(string dllFileName, string procName) =>
        GetExport(dllFileName, procName) != IntPtr.Zero;

    public static T GetFn<T>(string dllFileName, string procName) where T : Delegate
    {
        IntPtr p = GetExport(dllFileName, procName);
        if (p == IntPtr.Zero)
            throw new EntryPointNotFoundException($"export '{procName}' not found in {dllFileName}");
        return Marshal.GetDelegateForFunctionPointer<T>(p);
    }
}
