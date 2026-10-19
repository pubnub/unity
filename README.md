<img width="1920" height="600" alt="unity-header" src="https://github.com/user-attachments/assets/d802aad2-11f0-44a0-81a8-516ec87ab440" />

# PubNub Unity SDK

[![PubNub Unity SDK](https://img.shields.io/badge/dynamic/yaml?url=https%3A%2F%2Fraw.githubusercontent.com%2Fpubnub%2Funity%2Fmaster%2F.pubnub.yml&query=%24.version&label=PubNub%20Unity&color=blue)](https://github.com/pubnub/unity/releases) [![Build status](https://ci.appveyor.com/api/projects/status/1p3494pnt6rgqdsm/branch/master?svg=true)](https://ci.appveyor.com/project/PubNub/unity)

PubNub provides global infrastructure for real-time, interactive applications.

Publish and receive messages in Unity. Use this SDK for Unity applications targeting mobile, desktop, WebGL, VR, and AR.

[Documentation](https://www.pubnub.com/docs/sdks/unity) · [API reference](https://www.pubnub.com/docs/sdks/unity/api-reference/publish-and-subscribe) · [Changelog](https://www.pubnub.com/docs/sdks/unity/changelog) · [Support](https://support.pubnub.com/)

## Requirements

| Requirement | Supported version or setup |
| --- | --- |
| Unity Editor | [Unity 2018.4.26f1 or later](https://www.pubnub.com/docs/sdks/unity) |
| Build targets | [Mobile, Windows, macOS, Linux, WebGL, VR, and AR](https://www.pubnub.com/docs/sdks/unity) |
| Unity Package Manager | After installing the package if you want to use the optional pre-made Unity-specific assets, run **PubNub > Set up templates** as documented by the SDK, restart Unity if prompted, and configure the generated PubNub assets/components. |

## Installation

In Unity Package Manager, choose **Add package from git URL** and enter:

```text
https://github.com/pubnub/unity.git?path=/PubNubUnity/Assets/PubNub
```

For other installation methods, see [environment setup](https://www.pubnub.com/docs/sdks/unity).

## Quickstart

This example runs in a Unity scene. It subscribes to `hello_world`, publishes one message, and prints the received text.

### Get your keys

1. Open the [PubNub Admin Portal](https://admin.pubnub.com/).
2. Create an app and a keyset for development, or select an existing development keyset.
3. Copy its publish key and subscribe key.

If Access Manager is enabled, obtain a token from your trusted backend and call `pubnub.SetAuthToken(token)`. Keep the secret key out of the distributed Unity application.

### Send and receive a message

Create `PubNubHelloWorld.cs`, attach it to a GameObject in the active scene, and add:

Replace the key placeholders. Use a User ID that identifies the user or device in your app.

```csharp
using UnityEngine;
using PubnubApi;
using PubnubApi.Unity;

public class PubNubHelloWorld : MonoBehaviour
{
    private Pubnub pubnub;
    private SubscribeCallbackListener listener;
    private bool published;

    void Start()
    {
        PNConfiguration config = new PNConfiguration(new UserId("hello-world-user"))
        {
            SubscribeKey = "YOUR_SUBSCRIBE_KEY",
            PublishKey = "YOUR_PUBLISH_KEY"
        };

        pubnub = PubnubUnityUtils.NewUnityPubnub(config);

        listener = new SubscribeCallbackListener();
        listener.onMessage += (pn, message) =>
        {
            Debug.Log(message.Message);
            pubnub.UnsubscribeAll();
        };
        listener.onStatus += (pn, status) =>
        {
            if (
                status.Category == PNStatusCategory.PNConnectedCategory &&
                !published
            )
            {
                published = true;
                _ = pubnub.Publish()
                    .Channel("hello_world")
                    .Message("Hello world")
                    .ExecuteAsync();
            }
        };

        pubnub.AddListener(listener);
        pubnub.Subscribe<string>()
            .Channels(new[] { "hello_world" })
            .Execute();
    }

    void OnDestroy()
    {
        if (pubnub == null)
        {
            return;
        }

        pubnub.UnsubscribeAll();

        if (listener != null)
        {
            pubnub.RemoveListener(listener);
        }
    }
}
```

### Run the example

```text
Enter Play Mode in Unity Editor.
```

The Unity Console should show:

```text
Hello world
```

The example calls `pubnub.UnsubscribeAll()` after receiving the message and during `OnDestroy()`, and removes the registered listener during teardown.

For a complete application, see the [getting started guide](https://www.pubnub.com/docs/sdks/unity).

## Next steps

| Task | Documentation |
| --- | --- |
| Configure the client | [Configuration](https://www.pubnub.com/docs/sdks/unity/api-reference/configuration) |
| Work with subscriptions and messages | [Publish & Subscribe](https://www.pubnub.com/docs/sdks/unity/api-reference/publish-and-subscribe) |
| Check channel occupancy | [Presence](https://www.pubnub.com/docs/sdks/unity/api-reference/presence) |
| Read message history | [Message Persistence](https://www.pubnub.com/docs/sdks/unity/api-reference/storage-and-playback) |

## Build with an AI coding assistant

The [PubNub MCP server](https://www.pubnub.com/docs/ai/pubnub-mcp-server) gives an AI coding assistant access to PubNub SDK documentation and PubNub APIs. Connect the assistant to the hosted server at [`https://mcp.pubnub.com`](https://mcp.pubnub.com), or run `npx @pubnub/mcp@latest` locally.

The [server repository](https://github.com/pubnub/pubnub-mcp-server) has setup steps for VS Code, Cursor, Claude Code, Claude Desktop, Codex, Gemini CLI, and OpenCode.

## Before production

Use [Access Manager](https://www.pubnub.com/docs/sdks/unity/api-reference/access-manager) to grant each client the permissions it needs. Keep the secret key on your backend. Never include it in a distributed client.

Keep one PubNub client for the intended game/session lifetime rather than constructing clients as GameObjects are recreated. Call `pubnub.UnsubscribeAll()` to stop subscriptions and `pubnub.RemoveListener(listener)` when the listener's owner is destroyed.

Live delivery through PubNub SDKs is at-most-once. A subscriber can miss messages while disconnected or if its buffer overflows. For longer-gap recovery, see [Message Persistence](https://www.pubnub.com/docs/sdks/unity/api-reference/storage-and-playback).

## Troubleshooting

| Symptom | Check |
| --- | --- |
| The project cannot resolve PubNub types after package installation | Install the package using the documented Git URL, run the PubNub template setup, and allow Unity to finish package import and script compilation. See the [Unity SDK guide](https://www.pubnub.com/docs/sdks/unity). |
| A publish succeeds but no message appears | Check the message handler, subscription readiness, keyset, and channel name. |
| A mobile build loses PubNub types or callbacks after managed-code stripping | Apply the stripping/linker configuration documented in [Unity troubleshooting](https://www.pubnub.com/docs/sdks/unity/troubleshoot). |
| Extra clients or duplicate messages during development | Keep one client at game/session scope and remove subscriptions/listeners during teardown. Do not accumulate listeners every time a scene or GameObject is recreated. |

For setup help, see [troubleshooting](https://www.pubnub.com/docs/sdks/unity/troubleshoot). Check [network status](https://status.pubnub.com/) for service incidents.

## Releases

Read the [changelog](https://www.pubnub.com/docs/sdks/unity/changelog) before upgrading.

For a major version change, review the Unity migration guidance linked from the [Unity SDK documentation](https://www.pubnub.com/docs/sdks/unity) and the [changelog](https://www.pubnub.com/docs/sdks/unity/changelog).

## Support and contributions

For setup or account help, contact [PubNub Support](https://support.pubnub.com/).

For a reproducible SDK bug, use [GitHub Issues](https://github.com/pubnub/unity/issues). Include the SDK version, runtime, and a small reproduction with credentials removed.

Test the change in a supported Unity Editor version and include a reproducible test or scene for behavioral changes before opening a pull request.

## License

See the [PubNub Software Development Kit License](https://github.com/pubnub/unity/blob/master/LICENSE).
