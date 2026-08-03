using System.Runtime.InteropServices;

namespace mmr_gui;

/// <summary>
/// P/Invoke harness exercising the real 32-bit WMMR DLLs.
/// Reused by both the GUI buttons and the --selftest command-line path.
/// </summary>
public static class TestRunner
{
    const uint S_OK = 0x00000000;
    const uint CLASS_E_CLASSNOTAVAILABLE = 0x80040111;
    const uint E_NOTIMPL = 0x80004001;

    static uint U32(int hr) => (uint)hr & 0xFFFFFFFFU;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetDllDirectoryW([MarshalAs(UnmanagedType.LPWStr)] string lpPathName);

    [DllImport("wlidcli.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    static extern int WLCheckCredentials([MarshalAs(UnmanagedType.LPWStr)] string? account);

    [DllImport("wlidcli.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    static extern int WLClogin(uint identity, [MarshalAs(UnmanagedType.LPWStr)] string? password, uint flags, out IntPtr outValue);

    [DllImport("wlidcli.dll", CallingConvention = CallingConvention.StdCall)]
    static extern uint WLCreateIdentityHandle();

    [DllImport("wlidcli.dll", CallingConvention = CallingConvention.StdCall)]
    static extern int WLGetEnvironment(out IntPtr env);

    [DllImport("wlidcli.dll", CallingConvention = CallingConvention.StdCall)]
    static extern int WLGetTicket(uint identity, out IntPtr ticket);

    [DllImport("wlidcli.dll", CallingConvention = CallingConvention.StdCall)]
    static extern void WLFreeMemory(IntPtr p);

    [DllImport("wlidcli.dll", CallingConvention = CallingConvention.StdCall)]
    static extern int WLIsSignedIn(uint identity);

    [DllImport("uxctl.dll", CallingConvention = CallingConvention.StdCall)]
    static extern int UxControlsInitProcess();

    [DllImport("uxctl.dll", CallingConvention = CallingConvention.StdCall)]
    static extern int UxControlsCreateObject(out IntPtr pObject);

    [DllImport("uxctl.dll", CallingConvention = CallingConvention.StdCall)]
    static extern int UxControlsUninitProcess();

    [DllImport("WLXVideoTrim.dll", CallingConvention = CallingConvention.StdCall)]
    static extern int CreateVideoPlayer(out IntPtr pPlayer);

    [DllImport("WLXVideoTrim.dll", CallingConvention = CallingConvention.StdCall)]
    static extern int CreateVideoFormatContextTranscoder(out IntPtr pTranscoder);

    [DllImport("WLXVideoTrim.dll", CallingConvention = CallingConvention.StdCall)]
    static extern int CreateVideoWMVTranscoder(out IntPtr pTranscoder);

    [DllImport("WLXVideoTrim.dll", CallingConvention = CallingConvention.StdCall)]
    static extern int CreateAVICopierDirect(out IntPtr pCopier);

    [DllImport("WLXPipetran.dll", CallingConvention = CallingConvention.StdCall)]
    static extern int GetTFXCreateFunctions(out IntPtr pCreateFuncs, out uint count);

    [DllImport("MovieMakerCore.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    static extern int MovieMakerMain(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] argv);

    public static bool RunAllTests(string binDir) => RunAllTests(binDir, null);

    public static bool RunAllTests(string binDir, TextWriter? log)
    {
        log ??= Console.Out;
        var ok = true;

        void Report(string name, bool pass, string? detail = null)
        {
            if (pass)
                log!.WriteLine($"PASS {name}");
            else
                log!.WriteLine($"FAIL {name} ({detail})");
            ok &= pass;
            log!.Flush();
        }

        void ReportHr(string name, Func<(int hr, IntPtr p)> call, uint expected)
        {
            var (hr, p) = call();
            if (p != IntPtr.Zero)
                Marshal.Release(p);
            Report(name, U32(hr) == expected, $"0x{U32(hr):X8}");
        }

        if (string.IsNullOrWhiteSpace(binDir))
        {
            log.WriteLine("FAIL binDir (empty or null)");
            return false;
        }
        if (!Directory.Exists(binDir))
        {
            log.WriteLine($"FAIL binDir ({binDir} does not exist)");
            return false;
        }

        if (!SetDllDirectoryW(binDir))
            log.WriteLine("WARN SetDllDirectoryW (failed, DLLs may not resolve)");

        try
        {
            // --- wlidcli.dll ---
            {
                int hr = WLCheckCredentials(null);
                Report("WLCheckCredentials", U32(hr) == S_OK, $"0x{U32(hr):X8}");
            }

            {
                uint h1 = WLCreateIdentityHandle();
                uint h2 = WLCreateIdentityHandle();
                Report("WLCreateIdentityHandle", h1 >= 0x1000 && h2 > h1, $"h1=0x{h1:X} h2=0x{h2:X}");

                int signedIn = WLIsSignedIn(h1);
                Report("WLIsSignedIn", signedIn == 0 || signedIn == 1, $"returned {signedIn}");
            }
            {
                uint identity = WLCreateIdentityHandle();
                IntPtr ticket = IntPtr.Zero;
                int hr = WLGetTicket(identity, out ticket);
                bool pass = U32(hr) == S_OK;
                if (pass && ticket != IntPtr.Zero)
                {
                    string? s = Marshal.PtrToStringUni(ticket);
                    pass = s != null && s.Contains("ticket=");
                }
                Report("WLGetTicket", pass, $"0x{U32(hr):X8}");
                if (ticket != IntPtr.Zero)
                    WLFreeMemory(ticket);
            }
            {
                IntPtr env = IntPtr.Zero;
                int hr = WLGetEnvironment(out env);
                bool pass = U32(hr) == S_OK;
                if (pass && env != IntPtr.Zero)
                {
                    string? s = Marshal.PtrToStringUni(env);
                    pass = s != null && s == "production";
                }
                Report("WLGetEnvironment", pass, $"0x{U32(hr):X8}");
                if (env != IntPtr.Zero)
                    WLFreeMemory(env);
            }
            {
                uint identity = WLCreateIdentityHandle();
                IntPtr outValue = IntPtr.Zero;
                int hr = WLClogin(identity, null, 0, out outValue);
                Report("WLClogin", U32(hr) == S_OK, $"0x{U32(hr):X8}");
            }

            // --- uxctl.dll ---
            {
                int hr = UxControlsInitProcess();
                Report("UxControlsInitProcess", U32(hr) == S_OK, $"0x{U32(hr):X8}");
            }
            ReportHr("UxControlsCreateObject", () => { IntPtr p = IntPtr.Zero; return (UxControlsCreateObject(out p), p); }, CLASS_E_CLASSNOTAVAILABLE);
            {
                int hr = UxControlsUninitProcess();
                Report("UxControlsUninitProcess", true, $"0x{U32(hr):X8} (no expected value specified)");
            }

            // --- WLXVideoTrim.dll ---
            ReportHr("CreateVideoPlayer", () => { IntPtr p = IntPtr.Zero; return (CreateVideoPlayer(out p), p); }, E_NOTIMPL);
            ReportHr("CreateVideoFormatContextTranscoder", () => { IntPtr p = IntPtr.Zero; return (CreateVideoFormatContextTranscoder(out p), p); }, E_NOTIMPL);
            ReportHr("CreateVideoWMVTranscoder", () => { IntPtr p = IntPtr.Zero; return (CreateVideoWMVTranscoder(out p), p); }, E_NOTIMPL);
            ReportHr("CreateAVICopierDirect", () => { IntPtr p = IntPtr.Zero; return (CreateAVICopierDirect(out p), p); }, E_NOTIMPL);

            // --- WLXPipetran.dll ---
            {
                IntPtr pCreateFuncs = IntPtr.Zero;
                uint count = 0;
                int hr = GetTFXCreateFunctions(out pCreateFuncs, out count);
                Report("GetTFXCreateFunctions", U32(hr) == E_NOTIMPL && count == 0, $"0x{U32(hr):X8}, count={count}");
                if (pCreateFuncs != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(pCreateFuncs);
            }

            // --- MovieMakerCore.dll ---
            {
                int hr = MovieMakerMain(2, new[] { "MovieMaker.exe", "--help" });
                Report("MovieMakerMain", U32(hr) == S_OK, $"0x{U32(hr):X8}");
            }
        }
        catch (DllNotFoundException ex)
        {
            Report($"DllLoad {ex.Message}", false, "32-bit DLL missing from bin dir");
        }
        catch (EntryPointNotFoundException ex)
        {
            Report($"Export {ex.Message}", false, "export name not found in DLL");
        }
        catch (Exception ex)
        {
            Report("RunAllTests", false, ex.Message);
        }

        return ok;
    }
}
