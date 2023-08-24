# PubNub Unity SDK (V6)

[![Build Status](https://travis-ci.com/pubnub/unity.svg?branch=master)](https://travis-ci.com/pubnub/unity) [![Build status](https://ci.appveyor.com/api/projects/status/1p3494pnt6rgqdsm/branch/master?svg=true)](https://ci.appveyor.com/project/PubNub/unity)

This is the official PubNub Unity SDK repository.

PubNub takes care of the infrastructure and APIs needed for the realtime communication layer of your application. Work on your app's logic and let PubNub handle sending and receiving data across the world in less than 100ms.

## Get keys

You will need the publish and subscribe keys to authenticate your app. Get your keys from the [Admin Portal](https://dashboard.pubnub.com/login).

## Configure PubNub

1. Open Unity Editor and navigate to **Window -> Package Manager**.

2. In the Package Manager window, click + and select **Add package from git URL**.

3. Paste the PubNub Unity package link and click **Add**.
   ```
   https://github.com/pubnub/unity.git?path=/PubNubUnity/Assets/PubNub
   ```

4. In the editor menu bar, navigate to PubNub and click **Set up templates**.
5. Restart Unity Editor.

## Documentation

See the complete getting started guide and SDK documentation:

* [API reference for Unity](https://www.pubnub.com/docs/sdks/unity)

## Add event listeners

```csharp
listener.onStatus += OnPnStatus;
listener.onMessage += OnPnMessage;

void OnPnStatus(Pubnub pn, PNStatus status) {
    Debug.Log(status.Category == PNStatusCategory.PNConnectedCategory ? "Connected" : "Not connected");
}

void OnPnMessage(Pubnub pn, PNMessageResult<object> result) {
    Debug.Log($"Message received: {result.Message}");
}
```

## Publish/subscribe

```csharp
pubnub.Subscribe<string>().Channels(new[] { "TestChannel" }).Execute();

await pubnub.Publish().Channel("TestChannel").Message("Hello World from Unity!").ExecuteAsync();
// OR
await pubnub.Publish().Channel("TestChannel").Message(transform.position.GetJsonSafe()).ExecuteAsync(); 
```


## Support

If you **need help** or have a **general question**, contact support@pubnub.com.