# AGENTS.md

Guidance for AI coding agents working on the PubNub Unity SDK, and the single source of truth for guidance shared by all agents. Cursor and other agents that read `AGENTS.md` (such as Codex) load it directly, and `CLAUDE.md` imports it for Claude Code.

## What this repository is

- **The package:** the PubNub Unity SDK is the UPM package `com.pubnub.sdk` (`PubNubUnity/Assets/PubNub`). It installs from a git URL or as a `.unitypackage`. The repository also contains the Unity project used to develop and test it (Unity 2021.3.8f1).
- **Almost all SDK logic lives in the PubNub C# SDK** (`pubnub/c-sharp`), imported here as one precompiled DLL, `Runtime/Plugins/PubnubApiUnity.dll`.
  - The DLL is the `netstandard2.0` Release build of the C# repo's `PubnubApiUnity` project, with BouncyCastle merged in by ILRepack.
  - Newtonsoft.Json comes from Unity's `com.unity.nuget.newtonsoft-json` package.
  - API behavior, REST calls, crypto and the event engine are changed in the C# repository. Never edit, patch or decompile the DLL.
- **This repository adds a thin Unity layer** (namespace `PubnubApi.Unity`): main-thread event dispatching, the `PNConfigAsset` ScriptableObject, the `PNManagerBehaviour` component, a WebGL transport, a Unity logger, a Unity copy of the JSON adapter, and convenience extensions.
- **More context:** for PubNub concepts and the C# API, see the C# SDK's `AGENTS.md` and <https://www.pubnub.com/docs/sdks/unity>.

```text
PubNubUnity/                    Unity project; open this folder in the Unity Editor
  Assets/PubNub/                The UPM package (package.json)
    Runtime/Plugins/            PubnubApiUnity.dll + .xml from the C# SDK, licenses, third-party notices
    Runtime/Adapters/           SubscribeCallbackListener; Execute(Action) and List<string> extension methods; file/texture helpers
    Runtime/Util/               Dispatcher, PNConfigAsset, PNManagerBehaviour, PubnubUnityUtils, NewtonsoftJsonUnity,
                                UnityWebGLHttpClientService, UnityPubNubLogger, UnityPNSDKSource
    Editor/                     Inspectors (PNConfigAsset, Dispatcher, PNManager) and the "PubNub > Set up templates" menu
    Tests/                      Unity Test Framework play-mode tests (live keys)
    Samples/, Samples~/         Demo config asset; the UPM sample (PubNubManagerSample)
  Assets/Example/, Scenes/      Demo scene and scripts
  Assets/Snippets/              Documentation snippets (`// snippet.<name>` ... `// snippet.end`)
  Assets/Tooling/Editor/        "Assets > Export PubNub Package" (.unitypackage export)
.github/workflows/              CI: play-mode tests on a macOS runner, release automation
```

## How the Unity layer differs from the C# SDK

### Listener events reach the main thread through the Dispatcher

- **The threading problem:** the C# SDK raises listener events and callbacks on thread-pool threads, but Unity APIs (GameObjects, components, UI, textures, most of `UnityEngine`) only work on the main thread.
- **`SubscribeCallbackListener`** (`Runtime/Adapters`) wraps a C# `SubscribeCallbackExt` and re-raises every event through the `Dispatcher`, so handlers can use Unity objects directly.
  - Its events are `onMessage`, `onPresence`, `onSignal`, `onObject`, `onMessageAction`, `onFile`, `onStatus` and `onDataSync`.
  - Register it with `pubnub.AddListener(listener)`; it converts implicitly to `SubscribeCallback`.
- **`Dispatcher`** (`Runtime/Util`) is a hidden `[PubNub Dispatcher]` MonoBehaviour created before the first scene loads.
  - `Dispatcher.Dispatch(action)` queues work, and `FixedUpdate` runs the queue on the main thread.
  - `action.Dispatch(arg1, arg2)` and `Dispatcher.DispatchTask(task, callback)` are helpers.
  - Its `OnDestroy`/`OnDisable` call the C# `Pubnub.CleanUp()`.
- **Keep the listener in step with the C# SDK.** When the C# SDK adds a listener event, add it to `SubscribeCallbackListener` too: the event, the constructor parameter, and the dispatched handler. Otherwise Unity users never receive it.
- **Other user callbacks:** any code that calls user code from a C# SDK callback must go through the `Dispatcher`. Plain C# `SubscribeCallbackExt` listeners still run off the main thread.

### `await` in the Unity layer

The C# library rule "every await uses `.ConfigureAwait(false)`" only partly applies here:
- **Code that resumes into user callbacks deliberately awaits without it.** `ExecAsyncOperations`' `Execute(Action<T, PNStatus>)` extensions do this, so a call started on the main thread resumes there through Unity's synchronization context. Don't add `ConfigureAwait(false)` to that code.
- **Internal code that never touches Unity APIs or user code** keeps `ConfigureAwait(false)`, as `UnityWebGLHttpClientService` does.

### `PNConfigAsset` must track the C# `PNConfiguration`

- **What it is:** `PNConfigAsset` (`Runtime/Util`) is a ScriptableObject (**Create > PubNub > PubNub Config Asset**) that lets users define a configuration in the editor.
  - It mirrors a subset of the C# `PNConfiguration` and converts to it with an implicit operator. `UserId` is set at runtime.
  - `EnableWebGLBuildMode` and `LogToUnityConsole` are Unity-only; `PubnubUnityUtils.NewUnityPubnub()` reads them.
- **Whenever the C# `PNConfiguration` changes** (a new, renamed, removed or obsoleted setting, or a changed default), update all three places in the same change:
  1. the public field in `Runtime/Util/PNConfigAsset.cs`, with the C# default unless there is a Unity-specific reason
  2. the mapping in its `implicit operator PNConfiguration`
  3. the `propNames` list in `Editor/PNConfigAssetEditor.cs`; fields not listed there never appear in the inspector
- **Check** with `powershell -ExecutionPolicy Bypass -File .claude/skills/update-csharp-sdk/scripts/Compare-PNConfigAsset.ps1`. It fails when the three places disagree and lists default mismatches plus the C# settings the asset doesn't expose.
- **Unity serialization rules:**
  - Renaming a field loses its value in existing assets, so add `[FormerlySerializedAs("OldName")]`.
  - Fields must be Unity-serializable (primitives, strings, enums). Build complex values such as `RetryConfiguration` or `CryptoModule` inside the operator from simple fields.
  - The committed assets (`Assets/PNConfigAsset.asset`, `Samples/DemoKeys.asset`, `Samples~/.../PNSampleConfigAsset.asset`) must only ever contain empty or `demo` keys.

### Other Unity files that track the C# SDK

- **`NewtonsoftJsonUnity.cs`** is a Unity copy of the C# `NewtonsoftJsonDotNet.cs`, installed on every instance by `NewUnityPubnub()`. Port C# changes to it, such as the `IsValidJson` operation list or `IJsonPluggableLibrary` changes.
- **`UnityWebGLHttpClientService.cs`** implements `IHttpClientService` over `UnityWebRequest` for WebGL, so it must implement any new interface members.
- **`ExecAsyncOperations.cs` and `SubscribeOperation.cs`** hold `Execute(Action<T, PNStatus>)` and `List<string>` overload extensions. Add matching ones for new C# operations where they fit.
- **`PubnubUnityUtils.NewUnityPubnub()`** creates instances with the Unity transport (the Unity 6.5+ HTTP/2 handler, or WebGL), the Unity JSON adapter and logger. Unity code should use it, or `PNManagerBehaviour`, instead of `new Pubnub(...)`.
- **`Assets/Snippets/`** mirrors the C# SDK's `src/PubnubApi.Snippets/` for the Unity docs.

## Working rules

- **Updating the C# SDK** (importing a new DLL): follow the `update-csharp-sdk` skill.
- **Building and testing needs the Unity Editor** (2021.3.8f1, licensed), and nothing in this repository compiles from the command line.
  - If the editor is installed, play-mode tests run in batch mode: `<Unity> -batchmode -projectPath PubNubUnity -runTests -testPlatform PlayMode -testResults results.xml`. Ask the user for the editor path.
  - Otherwise ask the user to open the project, check the Console for compile errors, and run **Test Runner > PlayMode**. Never claim code compiles without one of these.
  - Tests read `PUB_KEY`, `SUB_KEY` and `PAM_SECRET_KEY`, defaulting to `demo-36`, and hit the live network.
- **`.meta` files:** Unity tracks assets by the GUID in each `.meta`.
  - Commit a `.meta` with every new file or folder; let Unity generate it by opening the project.
  - Never delete or regenerate existing ones. In particular, keep `PubnubApiUnity.dll.meta` (plugin import settings) and the asmdef metas; the editor assembly references the runtime assembly by GUID.
- **Assemblies:**
  - runtime: `PubNubAPI.asmdef`
  - editor only: `PubNubAPIEditor.asmdef`
  - tests: `Tests.asmdef`, which references the DLL by file name; never rename `PubnubApiUnity.dll`
- **C# language and Unity APIs:**
  - Package code uses C# 9 syntax (target-typed `new()`, `is not null`); don't use anything newer.
  - Guard newer Unity APIs with version defines, as `PubnubUnityUtils` does with `#if UNITY_6000_5_OR_NEWER`.
  - Match the file you edit: Unity-layer files use tabs and `class X {` braces, while `NewtonsoftJsonUnity.cs` keeps the C# SDK's style.
- **Keep backward compatibility.** Games use the public `PubnubApi.Unity` types, the extension methods and the `PNConfigAsset` fields. Keep the existing `[Obsolete]` overloads working.
- **Leave versions and changelogs to the release bot:** `package.json`, `VERSION`, `.pubnub.yml`, the `README.md` download link and `UnityPNSDKSource.cs`. Suggest the changelog line for the PR instead; for DLL updates it is usually "Updated the base C# SDK to latest version - X.Y.Z."
- **Protect secrets:** no real keys in assets, scenes, snippets or tests.
- **Don't modify** `Library/`, `Temp/`, `Logs/`, `UserSettings/` or `ProjectSettings/` unless asked. Unity rewrites them, and local changes there are usually noise.

## Known quirks

- **Events stall while the game is paused.** The Dispatcher drains its queue in `FixedUpdate`, which doesn't run while `Time.timeScale == 0`.
- **Logging is verbose by default.** A new `PNConfigAsset` defaults `LogLevel` to `All` (the enum's first value), while the C# default is `None`.
- **`PNConfigAsset` exposes only a subset of settings.** There is no `PresenceTimeout`, `Origin`, `FilterExpression`, `RetryConfiguration` and more; users set those in code on the converted `PNConfiguration`.
- **`PNManagerBehaviour` can throw on teardown.** Its `OnDestroy`/`OnDisable` call `pubnub.UnsubscribeAll` without a null check, so they throw if `Initialize()` was never called.
- **Old CI files.** `AppVeyorScripts/`, `TravisScripts/` and `_appveyor.yml` are older CI configurations; the GitHub Actions workflows run tests and releases.

## Where to find more

- **Area rules** (Cursor attaches them by glob; other agents should read them before editing the matching files):
  - `.cursor/rules/unity-threading.mdc`: listener, Dispatcher and async code in `Runtime/`, snippets and samples
  - `.cursor/rules/pnconfig-asset.mdc`: `PNConfigAsset`, its inspector, and `PubnubUnityUtils`
- **Skill:** `.claude/skills/update-csharp-sdk/` (loaded by both Claude Code and Cursor) imports a new C# SDK build and ports the matching Unity-layer changes.
- **PubNub MCP server** (`https://mcp.pubnub.com`): configured in `.cursor/mcp.json` and `.mcp.json`.
  - Each user enables it and signs in with their own PubNub account.
  - Use it for documentation lookups.
  - It acts with the user's account permissions, so don't use its account-changing tools unless explicitly asked.
