using System.Runtime.InteropServices;
using System.Text;

namespace mmr_gui;

internal static class Hr
{
    public const int S_OK = 0;
    public const int S_FALSE = 1;
    public const int E_NOTIMPL = unchecked((int)0x80004001);
    public const int E_INVALIDARG = unchecked((int)0x80070057);
    public const int E_ACCESSDENIED = unchecked((int)0x80070005);
    public const int CLASS_E_CLASSNOTAVAILABLE = unchecked((int)0x80040111);

    public static bool Is(int actual, int expected) =>
        unchecked((uint)actual) == unchecked((uint)expected);

    public static string Hex(int hr) => $"0x{unchecked((uint)hr) & 0xFFFFFFFF:X8}";

    public static string Name(int hr) => hr switch
    {
        S_OK => "S_OK",
        S_FALSE => "S_FALSE",
        E_NOTIMPL => "E_NOTIMPL",
        E_INVALIDARG => "E_INVALIDARG",
        E_ACCESSDENIED => "E_ACCESSDENIED",
        CLASS_E_CLASSNOTAVAILABLE => "CLASS_E_CLASSNOTAVAILABLE",
        _ => Hex(hr),
    };
}

[StructLayout(LayoutKind.Sequential)]
internal struct Rect { public int Left; public int Top; public int Right; public int Bottom; }

[StructLayout(LayoutKind.Sequential)]
internal struct Size2 { public int cx; public int cy; }

[StructLayout(LayoutKind.Sequential)]
internal struct WinMsg
{
    public IntPtr hwnd;
    public uint message;
    public IntPtr wParam;
    public IntPtr lParam;
    public uint time;
    public int ptX;
    public int ptY;
}

[StructLayout(LayoutKind.Sequential)]
internal struct PublishConfig
{
    public int Target;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)] public string Title;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)] public string Description;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)] public string Tags;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string Category;
    public int Private;
    public int AllowEmbed;
    public uint PrivacyLevel;
}

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal delegate int Fn0();

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal delegate int FnClassObject(ref Guid rclsid, ref Guid riid, out IntPtr ppv);

internal static class Native
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool SetDllDirectoryW([MarshalAs(UnmanagedType.LPWStr)] string lpPathName);

    [DllImport("wlidcli.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    internal static extern int WLCheckCredentials([MarshalAs(UnmanagedType.LPWStr)] string? account);

    [DllImport("wlidcli.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    internal static extern int WLClogin(uint identity, [MarshalAs(UnmanagedType.LPWStr)] string? password, uint flags, out IntPtr outValue);

    [DllImport("wlidcli.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern uint WLCreateIdentityHandle();

    [DllImport("wlidcli.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int WLGetEnvironment(out IntPtr env);

    [DllImport("wlidcli.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int WLGetTicket(uint identity, out IntPtr ticket);

    [DllImport("wlidcli.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern void WLFreeMemory(IntPtr p);

    [DllImport("wlidcli.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int WLIsSignedIn(uint identity);

    [DllImport("uxctl.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int UxControlsInitProcess();

    [DllImport("uxctl.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int UxControlsCreateObject(ref Guid rclsid, ref Guid riid, out IntPtr pObject);

    [DllImport("uxctl.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern void UxControlsUninitProcess();

    [DllImport("WLXVideoTrim.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int CreateVideoPlayer(out IntPtr pPlayer);

    [DllImport("WLXVideoTrim.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int CreateVideoFormatContextTranscoder(out IntPtr pTranscoder);

    [DllImport("WLXVideoTrim.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int CreateVideoWMVTranscoder(out IntPtr pTranscoder);

    [DllImport("WLXVideoTrim.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int CreateAVICopierDirect(out IntPtr pCopier);

    [DllImport("WLXVideoTrim.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int CreateVideoCopierFromMediaType(ref Guid pMediaType, out IntPtr pCopier);

    [DllImport("WLXPipetran.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int GetTFXCreateFunctions(out IntPtr pCreateFuncs, out uint count);

    [DllImport("WLXPipeline.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int GetPipelineCreateFunctions(out IntPtr pCreateFuncs, out uint count);

    [DllImport("MovieMakerCore.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    internal static extern int MovieMakerMain(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[]? argv);

    [DllImport("MetadataSys.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int WLXPSGetItemPropertyHandler(IntPtr pItem, uint dwAccessMode, ref Guid riid, out IntPtr ppv);

    [DllImport("WLMFReadWrite.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    internal static extern IntPtr MFReader_Open([MarshalAs(UnmanagedType.LPWStr)] string? pszFilePath);

    [DllImport("WLMFReadWrite.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern void MFReader_Close(IntPtr hReader);

    [DllImport("WLMFReadWrite.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int MFReader_GetProperties(IntPtr hReader, IntPtr pProps);

    [DllImport("WLMFReadWrite.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int MFReader_ReadFrame(IntPtr hReader, long llTimeMs, IntPtr pBuffer, uint cbBuffer, out uint pcbRead);

    [DllImport("WLMFReadWrite.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    internal static extern IntPtr MFWriter_Create([MarshalAs(UnmanagedType.LPWStr)] string? pszOutputPath, IntPtr pProps);

    [DllImport("WLMFReadWrite.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int MFWriter_WriteFrame(IntPtr hWriter, IntPtr pData, uint cbData, long llTimeMs);

    [DllImport("WLMFReadWrite.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int MFWriter_Finalize(IntPtr hWriter);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern IntPtr PublishManager_Create();

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern void PublishManager_Destroy(IntPtr hManager);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int PublishManager_EnumerateTargets(IntPtr hManager, [Out] int[] targets, ref uint count);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    internal static extern int PublishManager_GetTargetName(int target, StringBuilder name, uint cchName);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int PublishManager_Authenticate(IntPtr hManager, int target, IntPtr hParentWnd);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int PublishManager_IsAuthenticated(IntPtr hManager, int target, out int pAuth);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int PublishManager_SignOut(IntPtr hManager, int target);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    internal static extern IntPtr PublishManager_StartPublish(IntPtr hManager, int target, [MarshalAs(UnmanagedType.LPWStr)] string? pszFilePath, ref PublishConfig config);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int PublishManager_GetStatus(IntPtr hPublish, out uint pStatus, out uint pPercent);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int PublishManager_Cancel(IntPtr hPublish);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int PublishManager_GetResult(IntPtr hPublish, IntPtr pResult);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int PublishManager_SetProgressCallback(IntPtr hPublish, IntPtr pfnProgress, IntPtr pUserData);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int PublishManager_SetCompleteCallback(IntPtr hPublish, IntPtr pfnComplete, IntPtr pUserData);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    internal static extern IntPtr PublishManager_StartSubscribe(IntPtr hManager, int target, [MarshalAs(UnmanagedType.LPWStr)] string? pszItemId);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int PublishManager_GetSubscribeStatus(IntPtr hSubscribe, out uint pStatus, out uint pPercent);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    internal static extern int PublishManager_GetAccountInfo(IntPtr hManager, int target, StringBuilder displayName, uint cchDisplayName);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int PublishManager_RefreshToken(IntPtr hManager, int target);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int PublishManager_SetDefaultTarget(IntPtr hManager, int target);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int PublishManager_GetDefaultTarget(IntPtr hManager, out int pTarget);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int PublishManager_GetServiceStatus(int target, out int pAvailable);

    [DllImport("WLXMediaPublishSubscribe.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int PublishManager_Cleanup();

    [DllImport("WLXMP4Parser.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    internal static extern int AddMP4SourceFilter([MarshalAs(UnmanagedType.LPWStr)] string? pszFilePath, IntPtr pFilterGraph, out IntPtr ppSource);

    [DllImport("WLXMP4Parser.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    internal static extern int BuildMP4FilterGraph([MarshalAs(UnmanagedType.LPWStr)] string? pszFilePath, out IntPtr ppGraph);

    [DllImport("WLXMP4Parser.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    internal static extern int BuildMP4PlayBack([MarshalAs(UnmanagedType.LPWStr)] string? pszFilePath, IntPtr hWnd);

    [DllImport("WLXMP4Parser.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    internal static extern int IsMP4FilePlayable([MarshalAs(UnmanagedType.LPWStr)] string? pszFilePath);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int UXCoreInitProcess();

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int UXCoreInitThread();

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern void UXCoreUnInitProcess();

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern void UXCoreUnInitThread();

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int UxGetClassObject(ref Guid rclsid, ref Guid riid, out IntPtr ppv);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    internal static extern int DuiCreateObject([MarshalAs(UnmanagedType.LPWStr)] string? className, out IntPtr ppElement);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr DuiGetLayerManager();

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr ElementFromGadget(IntPtr hGadget);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int GetGadgetRect(IntPtr hGadget, out Rect rc);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int GetGadgetSize(IntPtr hGadget, out Size2 sz);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr GetTopHWNDParent(IntPtr hWnd);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr Internal_GetKeyFocusedElement_HWNDElement(IntPtr hwndElement);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int LayerManagerInitThread();

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void LayerManagerUnInitThread();

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int PeekMessageEx(out WinMsg msg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg, uint flags);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RMFindModule(IntPtr hMod, [MarshalAs(UnmanagedType.LPWStr)] string? name);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RMFindModuleForResource(IntPtr hMod, uint id);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RMLoadImage(IntPtr hinst, uint id);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RMLoadMenu(IntPtr hinst, uint id);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    internal static extern int RMLoadString(IntPtr hinst, uint id, StringBuilder buf, int cch);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RMLoadStringBSTR(IntPtr hinst, uint id);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RMUpdateResourceSet(IntPtr hMod);

    [DllImport("UXCore.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    internal static extern int StrToID([MarshalAs(UnmanagedType.LPWStr)] string? str);
}
