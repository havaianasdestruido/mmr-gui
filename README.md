# mmr-gui — WMMR 32-bit DLL test-suite browser

WinForms + CLI harness that exercises the 32-bit Windows Movie Maker (WMMR) native
DLLs built from the parent repo's `src` tree. Ships **128 checks across 21 DLLs**:
COM boilerplate, deep workflows (publish/subscribe lifecycle, MP4 pipeline stubs,
UXCore clean-C surface, CLI matrix), export-alias parity, and presence-only scans of
mangled C++ export sets.

## Requirements

- .NET 10 SDK (project targets `net10.0-windows`, `PlatformTarget=x86`).
- The 32-bit WMMR DLLs to test (e.g. `WMMR\build_clean\bin\Debug`). These are **not**
  vendored here; point the harness at a build output of the parent repo.
- Running the x86 exe needs an x86 .NET 10 runtime: `$env:DOTNET_ROOT_X86 = <x86 runtime dir>`.

## Usage

```powershell
# GUI browser
mmr-gui.exe

# Headless run (exit 0 = all pass, 1 = any failure, 2 = usage error)
mmr-gui.exe --selftest "C:\WMMR\build_clean\bin\Debug"
mmr-gui.exe --selftest "C:\WMMR\build_clean\bin\Debug" --junit out\suite.xml --json out\suite.json

# Managed self-check (suite construction + export plumbing, no native DLLs; CI gate)
mmr-gui.exe --selfcheck
```

## GUI features

- DLL list with per-DLL check count and pass count.
- **Run All** / **Run Selected** (per-DLL checkboxes).
- Live progress bar + **Cancel**.
- Color-coded results grid: check name, DLL, group, PASS/FAIL, duration, detail/error.
- **Failures only** filter.
- **Export JUnit XML** and **Export JSON** (SaveFileDialog).
- Status bar with coverage summary (`X/Y passed`) and run duration.

## Workflows

| Group | DLLs | Checks |
|---|---|---|
| wlidcli (identity, tickets, env) | 1 | 7 |
| uxctl (UX controls) | 1 | 3 |
| WLXVideoTrim (transcode factories + alias parity) | 1 | 6 |
| WLXPipetran | 1 | 1 |
| MovieMakerCore CLI matrix | 1 | 6 |
| MetadataSys (property handler + COM) | 1 | 5 |
| WLMFReadWrite (MF reader/writer arg guards + COM) | 1 | 9 |
| Publish-subscribe (create/enumerate/name/auth/publish/stub statuses/alias parity) | 1 | 26 |
| WLXMP4Parser (graph builders + alias parity) | 1 | 9 |
| WLXPipeline | 1 | 2 |
| UXCore (init/threading/layer manager/RM resources/StrToID/message hooks) | 1 | 25 |
| presence-WLXPhotoBase / DmxBici / WLXPhotoSqm / GPURenderer | 4 | 8 |
| COM boilerplate (can-unload/class-object/register/unregister) | 5 | 17 |
| WLMFDS load characterization | 1 | 1 |
| **Total** | **21** | **128** |

## Presence-only probes (why not functional)

Some export sets are heavily mangled C++ (e.g. `?AddToDataPoint@BiciWrapper@@YG_NKKPB_W@Z`,
`?DrawVideoFrame@GPURenderer@DirectUI@@...@Z`) and safe invocation would require exact
vtable/layout reconstruction. For those, the suite verifies the DLL loads and that the
**full mangled export set is present** via `GetProcAddress`, without calling in. Ditto
for `UXCore.GetMessageEx` (blocks until a message arrives) and `MovieMakerMain --selftest`
(never invoked; known to hang). Where a call is exercised, expectations are pinned to
empirically probed behavior of the **built binaries**, not stale on-disk sources.

Notable findings encoded as expectations:

- `MovieMakerCore` unknown switches block in the single-instance guard on first call
  (exit 2 on subsequent calls in a clean process); the suite documents the hazard.
- `WLMFDS.dll` exports the COM quartet (dumpbin) but its `DllMain` fails to initialize
  at load (`ERROR_DLL_INIT_FAILED`, 1114).
- `WLXMediaPublishSubscribe` / `WLXMP4Parser` export **decorated** names
  (`_PublishManager_RefreshToken@8` aliases `_PublishManager_GetResult@8`,
  `_BuildMP4PlayBack@8` aliases `_BuildMP4FilterGraph@8`); parity checks use the
  decorated spellings.
- `PublishManager_GetAccountInfo` returns `E_INVALIDARG` for a null handle and
  `S_OK 'User'` for a live manager.
- `MovieMakerPreviewClient.DllRegisterServer` writes `HKCR\CLSID` and returns
  `E_ACCESSDENIED` un-elevated.
- `WLMFReadWrite.DllCanUnloadNow` returns `S_OK` in the shipped binary (source says
  `S_FALSE`; built binary wins).

## Baseline selftest (verification record)

Native DLL set: WMMR `build_clean\bin\Debug` (WMMR submodule baseline `2508d97`).
Harness commit: `3deeaa6`. Working tree: clean (`git status --porcelain` empty).

```text
WMMR 32-bit DLL suite -- 128 checks, 21 DLLs
binDir: C:\Users\mcmco\Desktop\WMMR\build_clean\bin\Debug

PASS wlidcli.WLCheckCredentials (S_OK)
PASS wlidcli.WLCreateIdentityHandle (h1=0x1001 h2=0x1002)
PASS wlidcli.WLIsSignedIn (returned 1)
PASS wlidcli.WLGetTicket (S_OK ticket=st%3d1%26token%3dWLID_SIMULATED_TOKEN)
PASS wlidcli.WLGetEnvironment (S_OK production)
PASS wlidcli.WLClogin (S_OK)
PASS wlidcli.WLFreeMemory (export present)
PASS uxctl.UxControlsInitProcess (S_OK)
PASS uxctl.UxControlsCreateObject (CLASS_E_CLASSNOTAVAILABLE)
PASS uxctl.UxControlsUninitProcess (callable (void))
PASS WLXVideoTrim.CreateVideoPlayer (E_NOTIMPL)
PASS WLXVideoTrim.CreateVideoFormatContextTranscoder (E_NOTIMPL)
PASS WLXVideoTrim.CreateVideoWMVTranscoder (E_NOTIMPL)
PASS WLXVideoTrim.CreateAVICopierDirect (E_NOTIMPL)
PASS WLXVideoTrim.CreateVideoCopierFromMediaType (E_NOTIMPL)
PASS WLXVideoTrim.factory-alias-parity (3 factories alias CreateAVICopierDirect; CreateVideoCopierFromMediaType distinct)
PASS WLXPipetran.GetTFXCreateFunctions (E_NOTIMPL, count=0)
PASS MovieMakerCore.matrix(argc=0) (argc=0, argv=null -> exit 0x00000001)
PASS MovieMakerCore.matrix(argc=1) (argc=1, argv=null -> exit 0x00000001)
PASS MovieMakerCore.matrix(--help) (--help -> exit 0x00000000)
PASS MovieMakerCore.matrix(/? ) (/? -> exit 0x00000000)
PASS MovieMakerCore.matrix(-h) (-h -> exit 0x00000000)
PASS MovieMakerCore.unknown-switch (unknown switch '--bogus' blocked >3s (single-instance guard) -- documented hazard)
PASS MetadataSys.WLXPSGetItemPropertyHandler (E_NOTIMPL)
PASS MetadataSys.DllCanUnloadNow (S_OK)
PASS MetadataSys.DllGetClassObject (CLASS_E_CLASSNOTAVAILABLE)
PASS MetadataSys.DllRegisterServer (S_OK)
PASS MetadataSys.DllUnregisterServer (S_OK)
PASS WLMFReadWrite.DllCanUnloadNow (S_OK)
PASS WLMFReadWrite.DllGetClassObject (CLASS_E_CLASSNOTAVAILABLE)
PASS WLMFReadWrite.MFReader_Open (NULL for missing file)
PASS WLMFReadWrite.MFReader_Close (callable (void))
PASS WLMFReadWrite.MFReader_GetProperties (E_INVALIDARG)
PASS WLMFReadWrite.MFReader_ReadFrame (E_INVALIDARG)
PASS WLMFReadWrite.MFWriter_Create (NULL for null path)
PASS WLMFReadWrite.MFWriter_WriteFrame (E_INVALIDARG)
PASS WLMFReadWrite.MFWriter_Finalize (E_INVALIDARG)
PASS Publish - subscribe.DllCanUnloadNow (S_OK)
PASS Publish - subscribe.DllGetClassObject (CLASS_E_CLASSNOTAVAILABLE)
PASS Publish - subscribe.DllRegisterServer (S_OK)
PASS Publish - subscribe.DllUnregisterServer (S_OK)
PASS Publish - subscribe.PublishManager_Create (handle 0x8AD5D68)
PASS Publish - subscribe.PublishManager_EnumerateTargets (S_OK, count=5 targets=0,1,2,3,4)
PASS Publish - subscribe.PublishManager_GetTargetName (S_OK 'Facebook')
PASS Publish - subscribe.PublishManager_Authenticate (S_OK)
PASS Publish - subscribe.PublishManager_IsAuthenticated (S_OK, auth=FALSE)
PASS Publish - subscribe.PublishManager_SignOut (S_OK)
PASS Publish - subscribe.PublishManager_StartPublish (NULL for null file path)
PASS Publish - subscribe.PublishManager_GetStatus (E_NOTIMPL)
PASS Publish - subscribe.PublishManager_Cancel (E_NOTIMPL)
PASS Publish - subscribe.PublishManager_GetResult (E_NOTIMPL)
PASS Publish - subscribe.PublishManager_SetProgressCallback (E_NOTIMPL)
PASS Publish - subscribe.PublishManager_SetCompleteCallback (E_NOTIMPL)
PASS Publish - subscribe.PublishManager_StartSubscribe (NULL (subscribe stub))
PASS Publish - subscribe.PublishManager_GetSubscribeStatus (E_NOTIMPL)
PASS Publish - subscribe.PublishManager_GetAccountInfo (S_OK 'User')
PASS Publish - subscribe.PublishManager_RefreshToken (E_NOTIMPL)
PASS Publish - subscribe.PublishManager_SetDefaultTarget (S_OK)
PASS Publish - subscribe.PublishManager_GetDefaultTarget (S_OK, target=Facebook(0))
PASS Publish - subscribe.PublishManager_GetServiceStatus (S_OK, available=TRUE)
PASS Publish - subscribe.PublishManager_Cleanup (S_OK)
PASS Publish - subscribe.PublishManager_Destroy (callable (void))
PASS Publish - subscribe.export-alias-parity (5 alias pairs verified)
PASS WLXMP4Parser.DllCanUnloadNow (S_OK)
PASS WLXMP4Parser.DllGetClassObject (CLASS_E_CLASSNOTAVAILABLE)
PASS WLXMP4Parser.DllRegisterServer (S_OK)
PASS WLXMP4Parser.DllUnregisterServer (S_OK)
PASS WLXMP4Parser.AddMP4SourceFilter (E_NOTIMPL)
PASS WLXMP4Parser.BuildMP4FilterGraph (E_NOTIMPL)
PASS WLXMP4Parser.BuildMP4PlayBack (E_NOTIMPL)
PASS WLXMP4Parser.IsMP4FilePlayable (FALSE for null path)
PASS WLXMP4Parser.alias-parity (_BuildMP4PlayBack@8 aliases _BuildMP4FilterGraph@8)
PASS WLXPipeline.DllRegisterServer (S_OK)
PASS WLXPipeline.GetPipelineCreateFunctions (E_NOTIMPL, count=0)
PASS UXCore.UXCoreInitProcess (S_OK)
PASS UXCore.UXCoreInitThread (S_OK)
PASS UXCore.UXCoreUnInitThread (callable (void))
PASS UXCore.UXCoreUnInitProcess (callable (void))
PASS UXCore.UxGetClassObject (CLASS_E_CLASSNOTAVAILABLE)
PASS UXCore.DuiCreateObject (E_NOTIMPL)
PASS UXCore.LayerManagerInitThread (S_OK)
PASS UXCore.LayerManagerUnInitThread (callable (void))
PASS UXCore.DuiGetLayerManager (nullptr)
PASS UXCore.ElementFromGadget (nullptr)
PASS UXCore.GetGadgetRect (FALSE)
PASS UXCore.GetGadgetSize (FALSE)
PASS UXCore.GetTopHWNDParent (0)
PASS UXCore.Internal_GetKeyFocusedElement_HWNDElement (nullptr)
PASS UXCore.RMFindModule (0)
PASS UXCore.RMFindModuleForResource (0)
PASS UXCore.RMUpdateResourceSet (callable (void))
PASS UXCore.RMLoadImage (nullptr)
PASS UXCore.RMLoadMenu (nullptr)
PASS UXCore.RMLoadString (0 chars)
PASS UXCore.RMLoadStringBSTR (nullptr)
PASS UXCore.StrToID(null) (0)
PASS UXCore.StrToID(123) (123)
PASS UXCore.PeekMessageEx (FALSE (no queued messages))
PASS UXCore.GetMessageEx-presence (export present (blocking, not invoked))
PASS presence.WLXPhotoBase_Init (callable (void))
PASS presence.WLXPhotoBase-mangled-exports (4 exports present)
PASS presence.DmxBici-exports (19 exports present)
PASS presence.DmxBici-clean-alias-set (5 exports present)
PASS presence.WLXPhotoSqm-exports (44 exports present)
PASS presence.WLXPhotoSqm-key-set (5 exports present)
PASS presence.GPURenderer-loads (dll loads)
PASS presence.GPURenderer-mangled-exports (4 exports present)
PASS WLMFDS.load (LoadLibrary fails: ERROR_DLL_INIT_FAILED (1114) -- DllMain init failure; COM quartet exported (dumpbin) but module cannot initialize)
PASS WLXFaceRecognition.dll.DllCanUnloadNow (S_OK)
PASS WLXFaceRecognition.dll.DllGetClassObject (CLASS_E_CLASSNOTAVAILABLE)
PASS WLXFaceRecognition.dll.DllRegisterServer (S_OK)
PASS WLXFaceRecognition.dll.DllUnregisterServer (S_OK)
PASS WLXMovieLibrary.dll.DllCanUnloadNow (S_OK)
PASS WLXMovieLibrary.dll.DllGetClassObject (CLASS_E_CLASSNOTAVAILABLE)
PASS WLXMovieLibrary.dll.DllRegisterServer (S_OK)
PASS WLXMovieLibrary.dll.DllUnregisterServer (S_OK)
PASS WLXPhotoCinematic.dll.DllCanUnloadNow (S_OK)
PASS WLXPhotoCinematic.dll.DllGetClassObject (CLASS_E_CLASSNOTAVAILABLE)
PASS WLXPhotoCinematic.dll.DllRegisterServer (S_OK)
PASS WLXPhotoCinematic.dll.DllUnregisterServer (S_OK)
PASS WLXSlideshow.dll.DllCanUnloadNow (S_OK)
PASS WLXSlideshow.dll.DllGetClassObject (CLASS_E_CLASSNOTAVAILABLE)
PASS WLXSlideshow.dll.DllRegisterServer (S_OK)
PASS WLXSlideshow.dll.DllUnregisterServer (S_OK)
PASS MovieMakerPreviewClient.dll.DllCanUnloadNow (S_OK)
PASS MovieMakerPreviewClient.dll.DllGetClassObject (CLASS_E_CLASSNOTAVAILABLE)
PASS MovieMakerPreviewClient.dll.DllRegisterServer (E_ACCESSDENIED (writes HKCR\CLSID -- needs elevation))
PASS MovieMakerPreviewClient.dll.DllUnregisterServer (S_OK)
RESULT: 128 passed, 0 failed, in 3075 ms
total time: 3118 ms
```

Exit code: **0** (all 128 checks passed).

## CI

`.github/workflows/ci.yml`:

- `build` job (x64 SDK): `dotnet build -warnaserror` in Debug and Release.
- `selfcheck` job (x86 SDK): builds and runs `mmr-gui.exe --selfcheck`, verifying the
  128-case / 21-DLL suite invariant and JUnit/JSON export plumbing without needing the
  native DLL set. The full native run is a local step (see Usage).

## File tree

```
mmr-gui/
  Program.cs            CLI: --selftest, --selfcheck entry points
  Form1.cs              GUI: DLL list, run/cancel, filter, exports
  Form1.Designer.cs     GUI layout
  TestModels.cs         Check/TestCase/TestResult/RunSummary/TestEngine
  TestSuite.cs          128 checks across 21 DLLs (the suite itself)
  Native.cs             P/Invoke surface (all 21 DLLs) + Hr helpers
  NativeProbe.cs        LoadLibrary/GetProcAddress export-presence helpers
  Exporters.cs          JUnit XML + JSON writers
  mmr-gui.csproj        net10.0-windows, x86
  .github/workflows/ci.yml
```
