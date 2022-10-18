# PubNub Unity SDK (V6)

[![Build Status](https://travis-ci.com/pubnub/unity.svg?branch=master)](https://travis-ci.com/pubnub/unity) [![Build status](https://ci.appveyor.com/api/projects/status/1p3494pnt6rgqdsm/branch/master?svg=true)](https://ci.appveyor.com/project/PubNub/unity)

This is the official PubNub Unity SDK repository.

PubNub takes care of the infrastructure and APIs needed for the realtime communication layer of your application. Work on your app's logic and let PubNub handle sending and receiving data across the world in less than 100ms.

## Get keys

You will need the publish and subscribe keys to authenticate your app. Get your keys from the [Admin Portal](https://dashboard.pubnub.com/login).

## Configure PubNub

1. Download the PubNub Unity package from [this repository](https://github.com/pubnub/unity/releases/download/v6.0.5/PubNub.unitypackage).

2. Import the package to your Unity project by going to Assets > Import Package > Custom Package.

3. Configure your keys:

    ```csharp
    using PubNubAPI;
    PNConfiguration pnConfiguration = new PNConfiguration();
    pnConfiguration.SubscribeKey = "my_subkey";
    pnConfiguration.PublishKey = "my_pubkey";
    pnConfiguration.SecretKey = "my_secretkey";
    pnConfiguration.LogVerbosity = PNLogVerbosity.BODY;
    pnConfiguration.UserId = "myUniqueUserId";
    ```

4. If your Unity application contains existing [Assembly Definitions](https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html), you might receive [CS0246 compiler errors](https://support.unity.com/hc/en-us/articles/206116726-What-is-CS0246-), as the `PubNubAPI` namespace may not be recognized. You will need to create an assembly definition to use the `PubNubAPI` in your application.
    1. Navigate to the Assets > PubNub folder in the Project window.
    2. Right-click the PubNub folder and create an [Assembly Definition](https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html) by selecting Create > Assembly Definition. Assign a name to the asset (such as PubNub) and leave the default settings.
    3. Navigate to the folder that you wish to use the `PubNubAPI` namespace. Click on the Assembly Definition file.
    4. In the Inspector window, add the **PubNub** Assembly Definition you created earlier to the list of Assembly Definition References section by clicking on the **+** icon.
    5. Scroll down in the Inspector window, and click Apply. The `PubNubAPI` namespace should now be visible in your application.
    6. You may receive more compiler errors when trying to use the `PubNubAPI` namespace. These come from the PubNub > Editor folder, and contain test files. Delete the entire PubNub > Editor folder and allow the Unity editor to recompile changes.

## Add event listeners

```csharp
pubnub.SubscribeCallback += SubscribeCallbackHandler;

//Handler
void SubscribeCallbackHandler(object sender, EventArgs e) {
    SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;

    if (mea.Status != null) {
        switch (mea.Status.Category) {
            case PNStatusCategory.PNUnexpectedDisconnectCategory:
            case PNStatusCategory.PNTimeoutCategory:
                // handle publish
            break;
        }
    }
    if (mea.MessageResult != null) {
        Debug.Log("Channel" + mea.MessageResult.Channel);
        Debug.Log("Payload" + mea.MessageResult.Payload);
        Debug.Log("Publisher Id: " + mea.MessageResult.IssuingClientId);
    }
    if (mea.PresenceEventResult != null) {
        Debug.Log("SubscribeCallback in presence" + mea.PresenceEventResult.Channel + mea.PresenceEventResult.Occupancy + mea.PresenceEventResult.Event);
    }
    if (mea.SignalEventResult != null) {
        Debug.Log ("SubscribeCallback in SignalEventResult" + mea.SignalEventResult.Channel + mea.SignalEventResult.Payload);
    }
    if (mea.UUIDEventResult != null) {
        Debug.Log(mea.UUIDEventResult.Name);
        Debug.Log(mea.UUIDEventResult.Email);
        Debug.Log(mea.UUIDEventResult.ExternalID);
        Debug.Log(mea.UUIDEventResult.ProfileURL);
        Debug.Log(mea.UUIDEventResult.UUID);
        Debug.Log(mea.UUIDEventResult.ETag);
        Debug.Log(mea.UUIDEventResult.ObjectsEvent);
    }
    if (mea.ChannelEventResult != null) {
        Debug.Log(mea.ChannelEventResult.Name);
        Debug.Log(mea.ChannelEventResult.Description);
        Debug.Log(mea.ChannelEventResult.ChannelID);
        Debug.Log(mea.ChannelEventResult.ETag);
        Debug.Log(mea.ChannelEventResult.ObjectsEvent);
    }
    if (mea.MembershipEventResult != null) {
        Debug.Log(mea.MembershipEventResult.UUID);
        Debug.Log(mea.MembershipEventResult.Description);
        Debug.Log(mea.MembershipEventResult.ChannelID);
        Debug.Log(mea.MembershipEventResult.ObjectsEvent);
    }
    if (mea.MessageActionsEventResult != null) {
        Debug.Log(mea.MessageActionsEventResult.Channel);
        if(mea.MessageActionsEventResult.Data!=null){
            Debug.Log(mea.MessageActionsEventResult.Data.ActionTimetoken);
            Debug.Log(mea.MessageActionsEventResult.Data.ActionType);
            Debug.Log(mea.MessageActionsEventResult.Data.ActionValue);
            Debug.Log(mea.MessageActionsEventResult.Data.MessageTimetoken);
            Debug.Log(mea.MessageActionsEventResult.Data.UUID);
        }
        Debug.Log(mea.MessageActionsEventResult.MessageActionsEvent);
        Debug.Log(mea.MessageActionsEventResult.Subscription);
    }
}
```

## Publish/subscribe

```csharp
pubnub.Publish()
	.Channel("channel1")
	.Message("test message")
	.Async((result, status) => {
		if (!status.Error) {
			Debug.Log(string.Format("Publish Timetoken: {0}", result.Timetoken));
		} else {
			Debug.Log(status.Error);
			Debug.Log(status.ErrorData.Info);
		}
	});

pubnub.Subscribe()
    .Channels(new List<string>() {
        "my_channel"
    })
    .Execute();
```

## Documentation

* [API reference for Unity](https://www.pubnub.com/docs/sdks/unity)

## Support

If you **need help** or have a **general question**, contact support@pubnub.com.