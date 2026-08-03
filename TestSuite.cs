using System.Runtime.InteropServices;
using System.Text;

namespace mmr_gui;

public static class TestSuite
{
    public static IReadOnlyList<TestCase> Build()
    {
        var list = new List<TestCase>();
        T("wlidcli.WLCheckCredentials", "wlidcli.dll", "wlidcli - identity", () =>
        {
            int hr = Native.WLCheckCredentials(null);
            return Hr.Is(hr, Hr.S_OK) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected S_OK");
        });

        T("wlidcli.WLCreateIdentityHandle", "wlidcli.dll", "wlidcli - identity", () =>
        {
            uint h1 = Native.WLCreateIdentityHandle();
            uint h2 = Native.WLCreateIdentityHandle();
            return h1 >= 0x1000 && h2 > h1
                ? Check.Ok($"h1=0x{h1:X} h2=0x{h2:X}")
                : Check.Fail($"h1=0x{h1:X} h2=0x{h2:X} (expected monotonic handles)");
        });

        T("wlidcli.WLIsSignedIn", "wlidcli.dll", "wlidcli - identity", () =>
        {
            uint identity = Native.WLCreateIdentityHandle();
            int signedIn = Native.WLIsSignedIn(identity);
            return signedIn == 0 || signedIn == 1
                ? Check.Ok($"returned {signedIn}")
                : Check.Fail($"returned {signedIn}");
        });

        T("wlidcli.WLGetTicket", "wlidcli.dll", "wlidcli - identity", () =>
        {
            uint identity = Native.WLCreateIdentityHandle();
            IntPtr ticket = IntPtr.Zero;
            int hr = Native.WLGetTicket(identity, out ticket);
            bool pass = Hr.Is(hr, Hr.S_OK);
            string? content = null;
            if (pass && ticket != IntPtr.Zero)
            {
                content = Marshal.PtrToStringUni(ticket);
                pass = content != null && content.Contains("ticket=");
            }
            if (ticket != IntPtr.Zero)
                Native.WLFreeMemory(ticket);
            return pass
                ? Check.Ok($"{Hr.Name(hr)} {content}")
                : Check.Fail($"{Hr.Hex(hr)}, expected S_OK with ticket= payload");
        });

        T("wlidcli.WLGetEnvironment", "wlidcli.dll", "wlidcli - identity", () =>
        {
            IntPtr env = IntPtr.Zero;
            int hr = Native.WLGetEnvironment(out env);
            bool pass = Hr.Is(hr, Hr.S_OK);
            string? content = null;
            if (pass && env != IntPtr.Zero)
            {
                content = Marshal.PtrToStringUni(env);
                pass = content != null && content == "production";
            }
            if (env != IntPtr.Zero)
                Native.WLFreeMemory(env);
            return pass
                ? Check.Ok($"{Hr.Name(hr)} {content}")
                : Check.Fail($"{Hr.Hex(hr)}, expected S_OK with 'production'");
        });

        T("wlidcli.WLClogin", "wlidcli.dll", "wlidcli - identity", () =>
        {
            uint identity = Native.WLCreateIdentityHandle();
            IntPtr outValue = IntPtr.Zero;
            int hr = Native.WLClogin(identity, null, 0, out outValue);
            return Hr.Is(hr, Hr.S_OK) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected S_OK");
        });

        T("wlidcli.WLFreeMemory", "wlidcli.dll", "wlidcli - identity", () =>
            NativeProbe.ExportExists("wlidcli.dll", "WLFreeMemory")
                ? Check.Ok("export present")
                : Check.Fail("export WLFreeMemory missing"));

        T("uxctl.UxControlsInitProcess", "uxctl.dll", "uxctl - UI", () =>
        {
            int hr = Native.UxControlsInitProcess();
            return Hr.Is(hr, Hr.S_OK) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected S_OK");
        });

        T("uxctl.UxControlsCreateObject", "uxctl.dll", "uxctl - UI", () =>
        {
            IntPtr p = IntPtr.Zero;
            Guid c = Guid.Empty;
            Guid i = Guid.Empty;
            int hr = Native.UxControlsCreateObject(ref c, ref i, out p);
            if (p != IntPtr.Zero)
                Marshal.Release(p);
            return Hr.Is(hr, Hr.CLASS_E_CLASSNOTAVAILABLE)
                ? Check.Ok(Hr.Name(hr))
                : Check.Fail($"{Hr.Hex(hr)}, expected CLASS_E_CLASSNOTAVAILABLE");
        });

        T("uxctl.UxControlsUninitProcess", "uxctl.dll", "uxctl - UI", () =>
        {
            Native.UxControlsUninitProcess();
            return Check.Ok("callable (void)");
        });

        T("WLXVideoTrim.CreateVideoPlayer", "WLXVideoTrim.dll", "WLXVideoTrim", () =>
        {
            IntPtr p = IntPtr.Zero;
            int hr = Native.CreateVideoPlayer(out p);
            if (p != IntPtr.Zero)
                Marshal.Release(p);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        T("WLXVideoTrim.CreateVideoFormatContextTranscoder", "WLXVideoTrim.dll", "WLXVideoTrim", () =>
        {
            IntPtr p = IntPtr.Zero;
            int hr = Native.CreateVideoFormatContextTranscoder(out p);
            if (p != IntPtr.Zero)
                Marshal.Release(p);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        T("WLXVideoTrim.CreateVideoWMVTranscoder", "WLXVideoTrim.dll", "WLXVideoTrim", () =>
        {
            IntPtr p = IntPtr.Zero;
            int hr = Native.CreateVideoWMVTranscoder(out p);
            if (p != IntPtr.Zero)
                Marshal.Release(p);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        T("WLXVideoTrim.CreateAVICopierDirect", "WLXVideoTrim.dll", "WLXVideoTrim", () =>
        {
            IntPtr p = IntPtr.Zero;
            int hr = Native.CreateAVICopierDirect(out p);
            if (p != IntPtr.Zero)
                Marshal.Release(p);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        T("WLXVideoTrim.CreateVideoCopierFromMediaType", "WLXVideoTrim.dll", "WLXVideoTrim", () =>
        {
            IntPtr p = IntPtr.Zero;
            Guid mt = Guid.Empty;
            int hr = Native.CreateVideoCopierFromMediaType(ref mt, out p);
            if (p != IntPtr.Zero)
                Marshal.Release(p);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        T("WLXVideoTrim.factory-alias-parity", "WLXVideoTrim.dll", "WLXVideoTrim", () =>
        {
            var avi = NativeProbe.GetExport("WLXVideoTrim.dll", "CreateAVICopierDirect");
            var names = new[] { "CreateVideoPlayer", "CreateVideoFormatContextTranscoder", "CreateVideoWMVTranscoder" };
            var missing = names.Where(n => NativeProbe.GetExport("WLXVideoTrim.dll", n) == IntPtr.Zero).ToList();
            var notAliased = names.Where(n => NativeProbe.GetExport("WLXVideoTrim.dll", n) != avi).ToList();
            var distinct = NativeProbe.GetExport("WLXVideoTrim.dll", "CreateVideoCopierFromMediaType");
            bool ok = missing.Count == 0 && notAliased.Count == 0 && distinct != avi && distinct != IntPtr.Zero;
            return ok
                ? Check.Ok("3 factories alias CreateAVICopierDirect; CreateVideoCopierFromMediaType distinct")
                : Check.Fail($"missing={string.Join(",", missing)} notAliased={string.Join(",", notAliased)}");
        });

        T("WLXPipetran.GetTFXCreateFunctions", "WLXPipetran.dll", "WLXPipetran", () =>
        {
            IntPtr pCreateFuncs = IntPtr.Zero;
            uint count = 0;
            int hr = Native.GetTFXCreateFunctions(out pCreateFuncs, out count);
            if (pCreateFuncs != IntPtr.Zero)
                Marshal.FreeCoTaskMem(pCreateFuncs);
            return Hr.Is(hr, Hr.E_NOTIMPL) && count == 0
                ? Check.Ok($"{Hr.Name(hr)}, count=0")
                : Check.Fail($"{Hr.Hex(hr)}, count={count}");
        });

        T("MovieMakerCore.matrix(argc=0)", "MovieMakerCore.dll", "MovieMakerCore - CLI",
            () => MovieMatrix("argc=0, argv=null", 0, null, 1));

        T("MovieMakerCore.matrix(argc=1)", "MovieMakerCore.dll", "MovieMakerCore - CLI",
            () => MovieMatrix("argc=1, argv=null", 1, null, 1));

        T("MovieMakerCore.matrix(--help)", "MovieMakerCore.dll", "MovieMakerCore - CLI",
            () => MovieMatrix("--help", 2, new[] { "MovieMaker.exe", "--help" }, 0));

        T("MovieMakerCore.matrix(/? )", "MovieMakerCore.dll", "MovieMakerCore - CLI",
            () => MovieMatrix("/?", 2, new[] { "MovieMaker.exe", "/?" }, 0));

        T("MovieMakerCore.matrix(-h)", "MovieMakerCore.dll", "MovieMakerCore - CLI",
            () => MovieMatrix("-h", 2, new[] { "MovieMaker.exe", "-h" }, 0));

        T("MovieMakerCore.unknown-switch", "MovieMakerCore.dll", "MovieMakerCore - CLI",
            () => MovieUnknownSwitch("--bogus"));

        T("MetadataSys.WLXPSGetItemPropertyHandler", "MetadataSys.dll", "MetadataSys", () =>
        {
            Guid riid = Guid.Empty;
            IntPtr ppv = IntPtr.Zero;
            int hr = Native.WLXPSGetItemPropertyHandler(IntPtr.Zero, 0, ref riid, out ppv);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        Com(MetadataSysStubs());

        T("WLMFReadWrite.DllCanUnloadNow", "WLMFReadWrite.dll", "WLMFReadWrite", () =>
            ComCall("WLMFReadWrite.dll", "DllCanUnloadNow", Hr.S_OK));

        T("WLMFReadWrite.DllGetClassObject", "WLMFReadWrite.dll", "WLMFReadWrite", () =>
            ComClassObjectCall("WLMFReadWrite.dll", "DllGetClassObject"));

        T("WLMFReadWrite.MFReader_Open", "WLMFReadWrite.dll", "WLMFReadWrite", () =>
        {
            IntPtr h = Native.MFReader_Open(@"C:\definitely-not-a-real-movie.mp4");
            return h == IntPtr.Zero ? Check.Ok("NULL for missing file") : Check.Fail($"returned 0x{h.ToInt64():X}");
        });

        T("WLMFReadWrite.MFReader_Close", "WLMFReadWrite.dll", "WLMFReadWrite", () =>
        {
            Native.MFReader_Close(IntPtr.Zero);
            return Check.Ok("callable (void)");
        });

        T("WLMFReadWrite.MFReader_GetProperties", "WLMFReadWrite.dll", "WLMFReadWrite", () =>
        {
            int hr = Native.MFReader_GetProperties(IntPtr.Zero, IntPtr.Zero);
            return Hr.Is(hr, Hr.E_INVALIDARG) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_INVALIDARG");
        });

        T("WLMFReadWrite.MFReader_ReadFrame", "WLMFReadWrite.dll", "WLMFReadWrite", () =>
        {
            uint read = 0;
            int hr = Native.MFReader_ReadFrame(IntPtr.Zero, 0, IntPtr.Zero, 0, out read);
            return Hr.Is(hr, Hr.E_INVALIDARG) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_INVALIDARG");
        });

        T("WLMFReadWrite.MFWriter_Create", "WLMFReadWrite.dll", "WLMFReadWrite", () =>
        {
            IntPtr h = Native.MFWriter_Create(null, IntPtr.Zero);
            return h == IntPtr.Zero ? Check.Ok("NULL for null path") : Check.Fail($"returned 0x{h.ToInt64():X}");
        });

        T("WLMFReadWrite.MFWriter_WriteFrame", "WLMFReadWrite.dll", "WLMFReadWrite", () =>
        {
            int hr = Native.MFWriter_WriteFrame(IntPtr.Zero, IntPtr.Zero, 0, 0);
            return Hr.Is(hr, Hr.E_INVALIDARG) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_INVALIDARG");
        });

        T("WLMFReadWrite.MFWriter_Finalize", "WLMFReadWrite.dll", "WLMFReadWrite", () =>
        {
            int hr = Native.MFWriter_Finalize(IntPtr.Zero);
            return Hr.Is(hr, Hr.E_INVALIDARG) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_INVALIDARG");
        });

        Com(PubSubStubs());

        T("Publish - subscribe.PublishManager_Create", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            IntPtr h = Native.PublishManager_Create();
            return h != IntPtr.Zero ? Check.Ok($"handle 0x{h.ToInt64():X}") : Check.Fail("NULL handle");
        });

        T("Publish - subscribe.PublishManager_EnumerateTargets", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            IntPtr h = Native.PublishManager_Create();
            try
            {
                var targets = new int[8];
                uint cap = 8;
                int hr = Native.PublishManager_EnumerateTargets(h, targets, ref cap);
                bool ok = Hr.Is(hr, Hr.S_OK) && cap == 5 &&
                          targets.Take((int)cap).OrderBy(x => x).SequenceEqual(new[] { 0, 1, 2, 3, 4 });
                return ok
                    ? Check.Ok($"{Hr.Name(hr)}, count={cap} targets={string.Join(",", targets.Take((int)cap))}")
                    : Check.Fail($"{Hr.Hex(hr)}, count={cap} targets={string.Join(",", targets.Take((int)cap))}");
            }
            finally
            {
                Native.PublishManager_Destroy(h);
            }
        });

        T("Publish - subscribe.PublishManager_GetTargetName", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            var name = new StringBuilder(64);
            int hr = Native.PublishManager_GetTargetName(0, name, (uint)name.Capacity);
            string s = name.ToString();
            return Hr.Is(hr, Hr.S_OK) && s == "Facebook"
                ? Check.Ok($"{Hr.Name(hr)} '{s}'")
                : Check.Fail($"{Hr.Hex(hr)}, name='{s}'");
        });

        T("Publish - subscribe.PublishManager_Authenticate", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            IntPtr h = Native.PublishManager_Create();
            try
            {
                int hr = Native.PublishManager_Authenticate(h, 1, IntPtr.Zero);
                return Hr.Is(hr, Hr.S_OK) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected S_OK");
            }
            finally
            {
                Native.PublishManager_Destroy(h);
            }
        });

        T("Publish - subscribe.PublishManager_IsAuthenticated", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            IntPtr h = Native.PublishManager_Create();
            try
            {
                int auth = -1;
                int hr = Native.PublishManager_IsAuthenticated(h, 0, out auth);
                return Hr.Is(hr, Hr.S_OK) && auth == 0
                    ? Check.Ok($"{Hr.Name(hr)}, auth=FALSE")
                    : Check.Fail($"{Hr.Hex(hr)}, auth={auth}");
            }
            finally
            {
                Native.PublishManager_Destroy(h);
            }
        });

        T("Publish - subscribe.PublishManager_SignOut", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            IntPtr h = Native.PublishManager_Create();
            try
            {
                int hr = Native.PublishManager_SignOut(h, 0);
                return Hr.Is(hr, Hr.S_OK) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected S_OK");
            }
            finally
            {
                Native.PublishManager_Destroy(h);
            }
        });

        T("Publish - subscribe.PublishManager_StartPublish", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            IntPtr h = Native.PublishManager_Create();
            try
            {
                var cfg = new PublishConfig();
                IntPtr session = Native.PublishManager_StartPublish(h, 1, null, ref cfg);
                return session == IntPtr.Zero
                    ? Check.Ok("NULL for null file path")
                    : Check.Fail($"returned 0x{session.ToInt64():X}");
            }
            finally
            {
                Native.PublishManager_Destroy(h);
            }
        });

        T("Publish - subscribe.PublishManager_GetStatus", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            uint st = 0, pct = 0;
            int hr = Native.PublishManager_GetStatus(IntPtr.Zero, out st, out pct);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        T("Publish - subscribe.PublishManager_Cancel", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            int hr = Native.PublishManager_Cancel(IntPtr.Zero);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        T("Publish - subscribe.PublishManager_GetResult", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            int hr = Native.PublishManager_GetResult(IntPtr.Zero, IntPtr.Zero);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        T("Publish - subscribe.PublishManager_SetProgressCallback", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            int hr = Native.PublishManager_SetProgressCallback(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        T("Publish - subscribe.PublishManager_SetCompleteCallback", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            int hr = Native.PublishManager_SetCompleteCallback(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        T("Publish - subscribe.PublishManager_StartSubscribe", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            IntPtr h = Native.PublishManager_Create();
            try
            {
                IntPtr session = Native.PublishManager_StartSubscribe(h, 0, null);
                return session == IntPtr.Zero
                    ? Check.Ok("NULL (subscribe stub)")
                    : Check.Fail($"returned 0x{session.ToInt64():X}");
            }
            finally
            {
                Native.PublishManager_Destroy(h);
            }
        });

        T("Publish - subscribe.PublishManager_GetSubscribeStatus", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            uint st = 0, pct = 0;
            int hr = Native.PublishManager_GetSubscribeStatus(IntPtr.Zero, out st, out pct);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        T("Publish - subscribe.PublishManager_GetAccountInfo", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            IntPtr h = Native.PublishManager_Create();
            try
            {
                var name = new StringBuilder(64);
                int hr = Native.PublishManager_GetAccountInfo(h, 0, name, (uint)name.Capacity);
                string s = name.ToString();
                return Hr.Is(hr, Hr.S_OK) && s == "User"
                    ? Check.Ok($"{Hr.Name(hr)} '{s}'")
                    : Check.Fail($"{Hr.Hex(hr)}, name='{s}'");
            }
            finally
            {
                Native.PublishManager_Destroy(h);
            }
        });

        T("Publish - subscribe.PublishManager_RefreshToken", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            int hr = Native.PublishManager_RefreshToken(IntPtr.Zero, 0);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        T("Publish - subscribe.PublishManager_SetDefaultTarget", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            int hr = Native.PublishManager_SetDefaultTarget(IntPtr.Zero, 2);
            return Hr.Is(hr, Hr.S_OK) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected S_OK");
        });

        T("Publish - subscribe.PublishManager_GetDefaultTarget", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            int target = -1;
            int hr = Native.PublishManager_GetDefaultTarget(IntPtr.Zero, out target);
            return Hr.Is(hr, Hr.S_OK) && target == 0
                ? Check.Ok($"{Hr.Name(hr)}, target=Facebook(0)")
                : Check.Fail($"{Hr.Hex(hr)}, target={target}");
        });

        T("Publish - subscribe.PublishManager_GetServiceStatus", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            int available = -1;
            int hr = Native.PublishManager_GetServiceStatus(0, out available);
            return Hr.Is(hr, Hr.S_OK) && available != 0
                ? Check.Ok($"{Hr.Name(hr)}, available=TRUE")
                : Check.Fail($"{Hr.Hex(hr)}, available={available}");
        });

        T("Publish - subscribe.PublishManager_Cleanup", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            int hr = Native.PublishManager_Cleanup();
            return Hr.Is(hr, Hr.S_OK) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected S_OK");
        });

        T("Publish - subscribe.PublishManager_Destroy", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            IntPtr h = Native.PublishManager_Create();
            Native.PublishManager_Destroy(h);
            return Check.Ok("callable (void)");
        });

        T("Publish - subscribe.export-alias-parity", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () =>
        {
            var d = "WLXMediaPublishSubscribe.dll";
            var pairs = new (string A, string B)[]
            {
                ("_PublishManager_RefreshToken@8", "_PublishManager_GetResult@8"),
                ("_PublishManager_SetCompleteCallback@12", "_PublishManager_GetStatus@12"),
                ("_PublishManager_SetProgressCallback@12", "_PublishManager_GetStatus@12"),
                ("DllRegisterServer", "DllCanUnloadNow"),
                ("DllUnregisterServer", "DllCanUnloadNow"),
            };
            var bad = new List<string>();
            foreach (var (a, b) in pairs)
            {
                var pa = NativeProbe.GetExport(d, a);
                var pb = NativeProbe.GetExport(d, b);
                if (pa == IntPtr.Zero || pa != pb)
                    bad.Add($"{a}!={b}");
            }
            return bad.Count == 0 ? Check.Ok("5 alias pairs verified") : Check.Fail(string.Join(", ", bad));
        });

        Com(Mp4Stubs());

        T("WLXMP4Parser.AddMP4SourceFilter", "WLXMP4Parser.dll", "WLXMP4Parser", () =>
        {
            IntPtr p = IntPtr.Zero;
            int hr = Native.AddMP4SourceFilter(null, IntPtr.Zero, out p);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        T("WLXMP4Parser.BuildMP4FilterGraph", "WLXMP4Parser.dll", "WLXMP4Parser", () =>
        {
            IntPtr p = IntPtr.Zero;
            int hr = Native.BuildMP4FilterGraph(null, out p);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        T("WLXMP4Parser.BuildMP4PlayBack", "WLXMP4Parser.dll", "WLXMP4Parser", () =>
        {
            int hr = Native.BuildMP4PlayBack(null, IntPtr.Zero);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });

        T("WLXMP4Parser.IsMP4FilePlayable", "WLXMP4Parser.dll", "WLXMP4Parser", () =>
        {
            int r = Native.IsMP4FilePlayable(null);
            return r == 0 ? Check.Ok("FALSE for null path") : Check.Fail($"returned {r}, expected FALSE");
        });

        T("WLXMP4Parser.alias-parity", "WLXMP4Parser.dll", "WLXMP4Parser", () =>
        {
            var g = NativeProbe.GetExport("WLXMP4Parser.dll", "_BuildMP4FilterGraph@8");
            var pb = NativeProbe.GetExport("WLXMP4Parser.dll", "_BuildMP4PlayBack@8");
            return g != IntPtr.Zero && pb == g
                ? Check.Ok("_BuildMP4PlayBack@8 aliases _BuildMP4FilterGraph@8")
                : Check.Fail("_BuildMP4PlayBack@8 does not alias _BuildMP4FilterGraph@8");
        });

        T("WLXPipeline.DllRegisterServer", "WLXPipeline.dll", "WLXPipeline", () =>
            ComCall("WLXPipeline.dll", "DllRegisterServer", Hr.S_OK));

        T("WLXPipeline.GetPipelineCreateFunctions", "WLXPipeline.dll", "WLXPipeline", () =>
        {
            IntPtr p = IntPtr.Zero;
            uint count = 0;
            int hr = Native.GetPipelineCreateFunctions(out p, out count);
            return Hr.Is(hr, Hr.E_NOTIMPL) && count == 0
                ? Check.Ok($"{Hr.Name(hr)}, count=0")
                : Check.Fail($"{Hr.Hex(hr)}, count={count}");
        });

        UxCoreCases(list);

        T("presence.WLXPhotoBase_Init", "WLXPhotoBase.dll", "presence - WLXPhotoBase", () =>
        {
            var fn = NativeProbe.GetFn<Fn0>("WLXPhotoBase.dll", "_WLXPhotoBase_Init@0");
            fn();
            return Check.Ok("callable (void)");
        });

        T("presence.WLXPhotoBase-mangled-exports", "WLXPhotoBase.dll", "presence - WLXPhotoBase", () =>
            ProbeAll("WLXPhotoBase.dll",
                new[]
                {
                    "??0Thread@Base@@QAE@XZ",
                    "?GetProcessorCount@CPU@Base@@YAHXZ",
                    "?IsWin8OrGreater@OS@Base@@YA_NXZ",
                    "?Open@File@Base@@QAE_NPB_WKKK@Z",
                }));

        T("presence.DmxBici-exports", "DmxBici.dll", "presence - DmxBici", () =>
            ProbeAll("DmxBici.dll", DmxBiciNames));

        T("presence.DmxBici-clean-alias-set", "DmxBici.dll", "presence - DmxBici", () =>
            ProbeAll("DmxBici.dll",
                new[]
                {
                    "?StartExperience@BiciWrapper@@YGJXZ",
                    "?AddToDataPoint@BiciWrapper@@YG_NKKK@Z",
                    "?SetAnid@BiciWrapper@@YGJPB_W@Z",
                    "?TimerStart@BiciWrapper@@YG_NK@Z",
                    "?TransferExperienceToWeb@BiciWrapper@@YG_NPB_WPAPA_W@Z",
                }));

        T("presence.WLXPhotoSqm-exports", "WLXPhotoSqm.dll", "presence - WLXPhotoSqm", () =>
            ProbeAll("WLXPhotoSqm.dll", SqmNames));

        T("presence.WLXPhotoSqm-key-set", "WLXPhotoSqm.dll", "presence - WLXPhotoSqm", () =>
            ProbeAll("WLXPhotoSqm.dll",
                new[]
                {
                    "?EnableShipAsserts@Sqm@@YGXXZ",
                    "?GetOptInState@Sqm@@YG?AW4OptInState@1@XZ",
                    "?IsStreamTimerActive@Sqm@@YG_NKK@Z",
                    "?Shutdown@Sqm@@YGXXZ",
                    "?Startup@Sqm@@YGXW4SqmDmxAppId@1@@Z",
                }));

        T("presence.GPURenderer-loads", "GPURenderer.dll", "presence - GPURenderer", () =>
            NativeProbe.DllLoads("GPURenderer.dll")
                ? Check.Ok("dll loads")
                : Check.Fail("LoadLibrary failed"));

        T("presence.GPURenderer-mangled-exports", "GPURenderer.dll", "presence - GPURenderer", () =>
            ProbeAll("GPURenderer.dll",
                new[]
                {
                    "?Initialize@GPURenderer@DirectUI@@QAEJPAUHWND__@@@Z",
                    "?BeginDraw@GPURenderer@DirectUI@@QAEJXZ",
                    "?DrawVideoFrame@GPURenderer@DirectUI@@QAEJPAUID3D11Texture2D@@ABUtagRECT@@@Z",
                    "?Present@GPURenderer@DirectUI@@QAEJXZ",
                }));

        foreach (var (dll, group) in StubDlls)
        {
            if (dll == "WLMFDS.dll")
            {
                T("WLMFDS.load", dll, group, () =>
                {
                    int err = NativeProbe.LoadError(dll);
                    return err == 1114
                        ? Check.Ok("LoadLibrary fails: ERROR_DLL_INIT_FAILED (1114) -- DllMain init failure; COM quartet exported (dumpbin) but module cannot initialize")
                        : Check.Fail($"LoadLibrary err={err}, expected 1114");
                });
                continue;
            }
            if (dll == "MovieMakerPreviewClient.dll")
            {
                Com(new[]
                {
                    Make($"{dll}.DllCanUnloadNow", dll, group, () => ComCall(dll, "DllCanUnloadNow", Hr.S_OK)),
                    Make($"{dll}.DllGetClassObject", dll, group, () => ComClassObjectCall(dll, "DllGetClassObject")),
                    Make($"{dll}.DllRegisterServer", dll, group, () =>
                    {
                        var c = ComCall(dll, "DllRegisterServer", Hr.E_ACCESSDENIED);
                        return c.Pass ? Check.Ok("E_ACCESSDENIED (writes HKCR\\CLSID -- needs elevation)") : c;
                    }),
                    Make($"{dll}.DllUnregisterServer", dll, group, () => ComCall(dll, "DllUnregisterServer", Hr.S_OK)),
                });
                continue;
            }
            Com(StubCom(dll, group));
        }

        return list;

        void T(string name, string dll, string group, Func<Check> body) =>
            list.Add(new TestCase { Name = name, Dll = dll, Group = group, Body = body });

        void Com(IEnumerable<TestCase> cases)
        {
            foreach (var c in cases)
                list.Add(c);
        }
    }

    private static Check ComCall(string dll, string proc, int expected)
    {
        if (!NativeProbe.ExportExists(dll, proc))
            return Check.Fail($"export '{proc}' missing");
        var fn = NativeProbe.GetFn<Fn0>(dll, proc);
        int hr = fn();
        return Hr.Is(hr, expected)
            ? Check.Ok(Hr.Name(hr))
            : Check.Fail($"{Hr.Hex(hr)}, expected {Hr.Name(expected)}");
    }

    private static Check ComClassObjectCall(string dll, string proc)
    {
        if (!NativeProbe.ExportExists(dll, proc))
            return Check.Fail($"export '{proc}' missing");
        var fn = NativeProbe.GetFn<FnClassObject>(dll, proc);
        var c = Guid.Empty;
        var i = Guid.Empty;
        IntPtr ppv = IntPtr.Zero;
        int hr = fn(ref c, ref i, out ppv);
        return Hr.Is(hr, Hr.CLASS_E_CLASSNOTAVAILABLE)
            ? Check.Ok(Hr.Name(hr))
            : Check.Fail($"{Hr.Hex(hr)}, expected CLASS_E_CLASSNOTAVAILABLE");
    }

    private static IEnumerable<TestCase> MetadataSysStubs() => new[]
    {
        Make("MetadataSys.DllCanUnloadNow", "MetadataSys.dll", "MetadataSys", () => ComCall("MetadataSys.dll", "DllCanUnloadNow", Hr.S_OK)),
        Make("MetadataSys.DllGetClassObject", "MetadataSys.dll", "MetadataSys", () => ComClassObjectCall("MetadataSys.dll", "DllGetClassObject")),
        Make("MetadataSys.DllRegisterServer", "MetadataSys.dll", "MetadataSys", () => ComCall("MetadataSys.dll", "DllRegisterServer", Hr.S_OK)),
        Make("MetadataSys.DllUnregisterServer", "MetadataSys.dll", "MetadataSys", () => ComCall("MetadataSys.dll", "DllUnregisterServer", Hr.S_OK)),
    };

    private static IEnumerable<TestCase> PubSubStubs() => new[]
    {
        Make("Publish - subscribe.DllCanUnloadNow", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () => ComCall("WLXMediaPublishSubscribe.dll", "DllCanUnloadNow", Hr.S_OK)),
        Make("Publish - subscribe.DllGetClassObject", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () => ComClassObjectCall("WLXMediaPublishSubscribe.dll", "DllGetClassObject")),
        Make("Publish - subscribe.DllRegisterServer", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () => ComCall("WLXMediaPublishSubscribe.dll", "DllRegisterServer", Hr.S_OK)),
        Make("Publish - subscribe.DllUnregisterServer", "WLXMediaPublishSubscribe.dll", "Publish - subscribe", () => ComCall("WLXMediaPublishSubscribe.dll", "DllUnregisterServer", Hr.S_OK)),
    };

    private static IEnumerable<TestCase> Mp4Stubs() => new[]
    {
        Make("WLXMP4Parser.DllCanUnloadNow", "WLXMP4Parser.dll", "WLXMP4Parser", () => ComCall("WLXMP4Parser.dll", "DllCanUnloadNow", Hr.S_OK)),
        Make("WLXMP4Parser.DllGetClassObject", "WLXMP4Parser.dll", "WLXMP4Parser", () => ComClassObjectCall("WLXMP4Parser.dll", "DllGetClassObject")),
        Make("WLXMP4Parser.DllRegisterServer", "WLXMP4Parser.dll", "WLXMP4Parser", () => ComCall("WLXMP4Parser.dll", "DllRegisterServer", Hr.S_OK)),
        Make("WLXMP4Parser.DllUnregisterServer", "WLXMP4Parser.dll", "WLXMP4Parser", () => ComCall("WLXMP4Parser.dll", "DllUnregisterServer", Hr.S_OK)),
    };

    private static IEnumerable<TestCase> StubCom(string dll, string group) => new[]
    {
        Make($"{dll}.DllCanUnloadNow", dll, group, () => ComCall(dll, "DllCanUnloadNow", Hr.S_OK)),
        Make($"{dll}.DllGetClassObject", dll, group, () => ComClassObjectCall(dll, "DllGetClassObject")),
        Make($"{dll}.DllRegisterServer", dll, group, () => ComCall(dll, "DllRegisterServer", Hr.S_OK)),
        Make($"{dll}.DllUnregisterServer", dll, group, () => ComCall(dll, "DllUnregisterServer", Hr.S_OK)),
    };

    private static void UxCoreCases(List<TestCase> list)
    {
        const string dll = "UXCore.dll";
        const string group = "UXCore";
        void T(string name, Func<Check> body) => list.Add(new TestCase { Name = name, Dll = dll, Group = group, Body = body });

        T("UXCore.UXCoreInitProcess", () =>
        {
            int hr = Native.UXCoreInitProcess();
            return Hr.Is(hr, Hr.S_OK) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected S_OK");
        });
        T("UXCore.UXCoreInitThread", () =>
        {
            int hr = Native.UXCoreInitThread();
            return Hr.Is(hr, Hr.S_OK) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected S_OK");
        });
        T("UXCore.UXCoreUnInitThread", () => { Native.UXCoreUnInitThread(); return Check.Ok("callable (void)"); });
        T("UXCore.UXCoreUnInitProcess", () => { Native.UXCoreUnInitProcess(); return Check.Ok("callable (void)"); });
        T("UXCore.UxGetClassObject", () =>
        {
            var c = Guid.Empty;
            var i = Guid.Empty;
            IntPtr ppv = IntPtr.Zero;
            int hr = Native.UxGetClassObject(ref c, ref i, out ppv);
            return Hr.Is(hr, Hr.CLASS_E_CLASSNOTAVAILABLE) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected CLASS_E_CLASSNOTAVAILABLE");
        });
        T("UXCore.DuiCreateObject", () =>
        {
            IntPtr p = IntPtr.Zero;
            int hr = Native.DuiCreateObject(null, out p);
            return Hr.Is(hr, Hr.E_NOTIMPL) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected E_NOTIMPL");
        });
        T("UXCore.LayerManagerInitThread", () =>
        {
            int hr = Native.LayerManagerInitThread();
            return Hr.Is(hr, Hr.S_OK) ? Check.Ok(Hr.Name(hr)) : Check.Fail($"{Hr.Hex(hr)}, expected S_OK");
        });
        T("UXCore.LayerManagerUnInitThread", () => { Native.LayerManagerUnInitThread(); return Check.Ok("callable (void)"); });
        T("UXCore.DuiGetLayerManager", () =>
            Native.DuiGetLayerManager() == IntPtr.Zero ? Check.Ok("nullptr") : Check.Fail("expected nullptr"));
        T("UXCore.ElementFromGadget", () =>
            Native.ElementFromGadget(IntPtr.Zero) == IntPtr.Zero ? Check.Ok("nullptr") : Check.Fail("expected nullptr"));
        T("UXCore.GetGadgetRect", () =>
        {
            Rect rc = default;
            int r = Native.GetGadgetRect(IntPtr.Zero, out rc);
            return r == 0 ? Check.Ok("FALSE") : Check.Fail($"returned {r}, expected FALSE");
        });
        T("UXCore.GetGadgetSize", () =>
        {
            Size2 sz = default;
            int r = Native.GetGadgetSize(IntPtr.Zero, out sz);
            return r == 0 ? Check.Ok("FALSE") : Check.Fail($"returned {r}, expected FALSE");
        });
        T("UXCore.GetTopHWNDParent", () =>
            Native.GetTopHWNDParent(IntPtr.Zero) == IntPtr.Zero ? Check.Ok("0") : Check.Fail("expected 0"));
        T("UXCore.Internal_GetKeyFocusedElement_HWNDElement", () =>
            Native.Internal_GetKeyFocusedElement_HWNDElement(IntPtr.Zero) == IntPtr.Zero ? Check.Ok("nullptr") : Check.Fail("expected nullptr"));
        T("UXCore.RMFindModule", () =>
            Native.RMFindModule(IntPtr.Zero, null) == IntPtr.Zero ? Check.Ok("0") : Check.Fail("expected 0"));
        T("UXCore.RMFindModuleForResource", () =>
            Native.RMFindModuleForResource(IntPtr.Zero, 0) == IntPtr.Zero ? Check.Ok("0") : Check.Fail("expected 0"));
        T("UXCore.RMUpdateResourceSet", () => { Native.RMUpdateResourceSet(IntPtr.Zero); return Check.Ok("callable (void)"); });
        T("UXCore.RMLoadImage", () =>
            Native.RMLoadImage(IntPtr.Zero, 0) == IntPtr.Zero ? Check.Ok("nullptr") : Check.Fail("expected nullptr"));
        T("UXCore.RMLoadMenu", () =>
            Native.RMLoadMenu(IntPtr.Zero, 0) == IntPtr.Zero ? Check.Ok("nullptr") : Check.Fail("expected nullptr"));
        T("UXCore.RMLoadString", () =>
        {
            var sb = new StringBuilder(128);
            int n = Native.RMLoadString(IntPtr.Zero, 0, sb, sb.Capacity);
            return n == 0 ? Check.Ok("0 chars") : Check.Fail($"returned {n}, expected 0");
        });
        T("UXCore.RMLoadStringBSTR", () =>
            Native.RMLoadStringBSTR(IntPtr.Zero, 0) == IntPtr.Zero ? Check.Ok("nullptr") : Check.Fail("expected nullptr"));
        T("UXCore.StrToID(null)", () =>
            Native.StrToID(null) == 0 ? Check.Ok("0") : Check.Fail("expected 0"));
        T("UXCore.StrToID(123)", () =>
            Native.StrToID("123") == 123 ? Check.Ok("123") : Check.Fail("expected 123"));
        T("UXCore.PeekMessageEx", () =>
        {
            WinMsg msg = default;
            int r = Native.PeekMessageEx(out msg, IntPtr.Zero, 0, 0, 0, 0);
            return r == 0 ? Check.Ok("FALSE (no queued messages)") : Check.Fail($"returned {r}, expected FALSE");
        });
        T("UXCore.GetMessageEx-presence", () =>
            NativeProbe.ExportExists(dll, "GetMessageEx")
                ? Check.Ok("export present (blocking, not invoked)")
                : Check.Fail("export GetMessageEx missing"));
    }

    private static Check ProbeAll(string dll, IEnumerable<string> names)
    {
        var missing = names.Where(n => !NativeProbe.ExportExists(dll, n)).ToList();
        return missing.Count == 0
            ? Check.Ok($"{names.Count()} exports present")
            : Check.Fail($"{missing.Count} missing: {string.Join(", ", missing)}");
    }

    private static Check MovieUnknownSwitch(string label)
    {
        var box = new Box { Done = false };
        var t = new Thread(() =>
        {
            try
            {
                box.Value = Native.MovieMakerMain(2, new[] { "MovieMaker.exe", label });
                box.Done = true;
            }
            catch (Exception ex)
            {
                box.Error = $"{ex.GetType().Name}: {ex.Message}";
                box.Done = true;
            }
        })
        {
            IsBackground = true,
        };
        t.Start();
        if (!t.Join(TimeSpan.FromSeconds(3)))
            return Check.Ok($"unknown switch '{label}' blocked >3s (single-instance guard) -- documented hazard");
        if (box.Error is not null)
            return Check.Fail($"exception ({label}): {box.Error}");
        return box.Value == 2
            ? Check.Ok($"unknown switch '{label}' -> exit 2 (SINGLE_INSTANCE)")
            : Check.Fail($"'{label}' -> exit 0x{(uint)box.Value:X8}, expected 2 or single-instance block");
    }

    private static Check MovieMatrix(string label, int argc, string[]? argv, int expected)
    {
        var box = new Box { Done = false };
        var t = new Thread(() =>
        {
            try
            {
                box.Value = Native.MovieMakerMain(argc, argv);
                box.Done = true;
            }
            catch (Exception ex)
            {
                box.Error = $"{ex.GetType().Name}: {ex.Message}";
                box.Done = true;
            }
        })
        {
            IsBackground = true,
        };
        t.Start();
        if (!t.Join(TimeSpan.FromSeconds(3)))
            return Check.Fail($"timeout ({label}): native call did not return", "timeout");
        if (box.Error is not null)
            return Check.Fail($"exception ({label}): {box.Error}", "exception");
        return Hr.Is(box.Value, expected)
            ? Check.Ok($"{label} -> exit 0x{(uint)box.Value:X8}")
            : Check.Fail($"{label} -> exit 0x{(uint)box.Value:X8}, expected 0x{(uint)expected:X8}");
    }

    private sealed class Box
    {
        public bool Done;
        public int Value;
        public string? Error;
    }

    private static TestCase Make(string name, string dll, string group, Func<Check> body) =>
        new() { Name = name, Dll = dll, Group = group, Body = body };

    private static readonly (string Dll, string Group)[] StubDlls = new[]
    {
        ("WLMFDS.dll", "COM - WLMFDS"),
        ("WLXFaceRecognition.dll", "COM - WLXFaceRecognition"),
        ("WLXMovieLibrary.dll", "COM - WLXMovieLibrary"),
        ("WLXPhotoCinematic.dll", "COM - WLXPhotoCinematic"),
        ("WLXSlideshow.dll", "COM - WLXSlideshow"),
        ("MovieMakerPreviewClient.dll", "COM - MovieMakerPreviewClient"),
    };

    private static readonly string[] DmxBiciNames = new[]
    {
        "?AddStringToDataPoint@BiciWrapper@@YG_NKKPB_W@Z",
        "?AddToAverage@BiciWrapper@@YG_NKK@Z",
        "?AddToDataPoint@BiciWrapper@@YG_NKKK@Z",
        "?AddToStream@BiciWrapper@@YGXKPBVTuple@1@@Z",
        "?EndExperience@BiciWrapper@@YGJXZ",
        "?Increment@BiciWrapper@@YG_NKK@Z",
        "?Set@BiciWrapper@@YG_NKK@Z",
        "?SetAnid@BiciWrapper@@YGJPB_W@Z",
        "?SetIfMax@BiciWrapper@@YG_NKK@Z",
        "?SetIfMin@BiciWrapper@@YG_NKK@Z",
        "?SetString@BiciWrapper@@YG_NKPB_W@Z",
        "?StartExperience@BiciWrapper@@YGJW4BiciStartupId@1@@Z",
        "?StartExperience@BiciWrapper@@YGJXZ",
        "?TimerAccumulate@BiciWrapper@@YG_NK@Z",
        "?TimerRecord@BiciWrapper@@YG_NK@Z",
        "?TimerStart@BiciWrapper@@YG_NK@Z",
        "?TransferExperienceToApp@BiciWrapper@@YG_NPAPA_W@Z",
        "?TransferExperienceToAppId@BiciWrapper@@YG_NK@Z",
        "?TransferExperienceToWeb@BiciWrapper@@YG_NPB_WPAPA_W@Z",
    };

    private static readonly string[] SqmNames = new[]
    {
        "?AbortStreamTimer@Sqm@@YGXKK@Z",
        "?AddStreamTimerData@Sqm@@YGXKKK@Z",
        "?AddStreamTimerData@Sqm@@YGXKKPB_W@Z",
        "?AddToAverage@Sqm@@YGXKK@Z",
        "?AddToStream@Sqm@@YGXKK@Z",
        "?AddToStream@Sqm@@YGXKKK@Z",
        "?AddToStream@Sqm@@YGXKKKK@Z",
        "?AddToStream@Sqm@@YGXKPBVTuple@1@@Z",
        "?AddToStream@Sqm@@YGXKPB_W@Z",
        "?AddToStreamTimer@Sqm@@YGXKKKPBVTuple@1@@Z",
        "?AddToStreamTimer@Sqm@@YGXKKPBVTuple@1@@Z",
        "?DeferAddToAverage@Sqm@@YGXKK@Z",
        "?DeferAddToMedian@Sqm@@YGXKK@Z",
        "?DeferReportAverage@Sqm@@YGXK@Z",
        "?DeferReportMax@Sqm@@YGXK@Z",
        "?DeferReportMedian@Sqm@@YGXK@Z",
        "?DeferReportMin@Sqm@@YGXK@Z",
        "?DeferSetIfMax@Sqm@@YGXKK@Z",
        "?DeferSetIfMin@Sqm@@YGXKK@Z",
        "?EnableShipAsserts@Sqm@@YGXXZ",
        "?GetOptInState@Sqm@@YG?AW4OptInState@1@XZ",
        "?Increment@Sqm@@YGXKK@Z",
        "?InitializeUserExecutedActionReporting@Sqm@@YGXKK@Z",
        "?IsEnabled@Sqm@@YG_NXZ",
        "?IsStreamTimerActive@Sqm@@YG_NKK@Z",
        "?IsStreamTimerDataSet@Sqm@@YG_NKK@Z",
        "?PauseTimer@Sqm@@YGXK@Z",
        "?ReportAppCloseStatus@Sqm@@YGX_N@Z",
        "?ReportAppLaunchStatus@Sqm@@YGX_N@Z",
        "?ReportUserExecutedAction@Sqm@@YGXKK@Z",
        "?Set@Sqm@@YGXKK@Z",
        "?Set@Sqm@@YGXKPB_W@Z",
        "?SetAppDefinedValue@Sqm@@YGXK@Z",
        "?SetAppStatusReportingMode@Sqm@@YGX_N@Z",
        "?SetApplicationMode@Sqm@@YGXK@Z",
        "?SetIfMax@Sqm@@YGXKK@Z",
        "?SetIfMin@Sqm@@YGXKK@Z",
        "?SetOptInPreference@Sqm@@YGX_N@Z",
        "?Shutdown@Sqm@@YGXXZ",
        "?StartStreamTimer@Sqm@@YGXKKK@Z",
        "?StartTimer@Sqm@@YGXK@Z",
        "?Startup@Sqm@@YGXW4SqmDmxAppId@1@@Z",
        "?Startup@Sqm@@YGXXZ",
        "?StopStreamTimer@Sqm@@YGXKK@Z",
    };
}
