---
name: update-csharp-sdk
description: Imports a new build of the PubNub C# SDK DLL (PubnubApiUnity.dll) into the Unity SDK and ports the Unity-layer changes it requires (PNConfigAsset, SubscribeCallbackListener, NewtonsoftJsonUnity, WebGL transport, extensions, snippets, tests). Use when updating the base C# SDK, importing a C# feature or fix into Unity, or when the C# PNConfiguration, listener events or IHttpClientService change.
---

# Update the C# SDK

```text
- [ ] 1. Build the Unity DLL in the C# repository
- [ ] 2. Compare PNConfiguration with PNConfigAsset
- [ ] 3. Copy the DLL, XML docs and notices
- [ ] 4. Port the Unity-layer changes
- [ ] 5. Verify in Unity
- [ ] 6. Propose the changelog line
```

## 1. Build the DLL

In the C# SDK repository (`pubnub/c-sharp`; ask the user for the local path):

```bash
dotnet build src/Api/PubnubApiUnity/PubnubApiUnity.csproj -c Release
```

The output is in `src/Api/PubnubApiUnity/bin/Release/netstandard2.0/`. **Use the `netstandard2.0` build.** Unity can't load the `net6.0` one. A correctly merged DLL is about 6 MB, because BouncyCastle is merged in by ILRepack.

## 2. Compare configurations

```powershell
powershell -ExecutionPolicy Bypass -File .claude/skills/update-csharp-sdk/scripts/Compare-PNConfigAsset.ps1 -DllPath <c-sharp>/src/Api/PubnubApiUnity/bin/Release/netstandard2.0/PubnubApiUnity.dll
```

New, removed, obsoleted or re-defaulted `PNConfiguration` settings show up as FAIL, WARN or INFO lines. Update `PNConfigAsset` as described in `.cursor/rules/pnconfig-asset.mdc`. After step 3, run the script without `-DllPath`; it must print no FAIL lines.

## 3. Copy files

Overwrite these in `PubNubUnity/Assets/PubNub/Runtime/Plugins/`, keeping every existing `.meta`:
- `PubnubApiUnity.dll`
- `PubnubApiUnity.xml`
- `THIRD-PARTY-NOTICES.txt` and `LICENSE-APACHE-2.0.txt`, when the C# SDK's dependencies changed

Never rename the DLL; `Tests.asmdef` references it by file name. Check the version with `[Reflection.AssemblyName]::GetAssemblyName((Resolve-Path PubNubUnity/Assets/PubNub/Runtime/Plugins/PubnubApiUnity.dll).Path).Version`.

## 4. Port Unity-layer changes

Read the C# changes since the last import (the C# repo's `CHANGELOG` and `git log`), then check each item:

| C# change | Unity file to update |
| --- | --- |
| `PNConfiguration` setting | `Runtime/Util/PNConfigAsset.cs`, plus `propNames` in `Editor/PNConfigAssetEditor.cs` |
| New `SubscribeCallback` / `SubscribeCallbackExt` event | `Runtime/Adapters/SubscribeCallbackListener.cs` (event, constructor parameter, dispatched handler) |
| `NewtonsoftJsonDotNet.cs` or `IJsonPluggableLibrary` | `Runtime/Util/NewtonsoftJsonUnity.cs` |
| `IHttpClientService`, `TransportRequest` or `TransportResponse` | `Runtime/Util/UnityWebGLHttpClientService.cs` |
| New operation or builder method | `Runtime/Adapters/ExecAsyncOperations.cs` and `SubscribeOperation.cs` extensions |
| `Pubnub` constructor or `SetLogger`/`SetJsonPluggableLibrary` | `Runtime/Util/PubnubUnityUtils.cs` |
| New or changed docs snippet | `Assets/Snippets/` (mirrors `src/PubnubApi.Snippets/`) |

Add play-mode tests in `Assets/PubNub/Tests/` for new behavior; `AesGcmCryptoModule.cs` is an example from a previous import. New files need `.meta` files, which Unity generates when the project is opened.

## 5. Verify in Unity

You can't compile here without the Unity Editor. Ask the user to:
1. open `PubNubUnity/` in Unity 2021.3.8f1 and check the Console for compile errors
2. run **Test Runner > PlayMode**, or `<Unity> -batchmode -projectPath PubNubUnity -runTests -testPlatform PlayMode -testResults results.xml`
3. make a WebGL build if the transport or JSON adapter changed

Report which of these were done.

## 6. Changelog

Don't edit `package.json`, `VERSION`, `.pubnub.yml`, `README.md` or `UnityPNSDKSource.cs`; the release bot does. Propose the PR changelog line, for example "Updated the base C# SDK to latest version - 9.2.0.", plus one line per user-visible Unity-layer change.
