# PubNub Unity SDK (V4)

[![Build Status](https://travis-ci.com/pubnub/unity.svg?branch=master)](https://travis-ci.com/pubnub/unity) [![Build status](https://ci.appveyor.com/api/projects/status/1p3494pnt6rgqdsm/branch/master?svg=true)](https://ci.appveyor.com/project/PubNub/unity)

This is the official PubNub Unity SDK repository.

PubNub takes care of the infrastructure and APIs needed for the realtime communication layer of your application. Work on your app's logic and let PubNub handle sending and receiving data across the world in less than 100ms.

## Get keys

You will need the publish and subscribe keys to authenticate your app. Get your keys from the [Admin Portal](https://dashboard.pubnub.com/login).

## Configure PubNub

1. Download the PubNub Unity package from [this repository](https://github.com/pubnub/unity/releases/download/v6.0.1/PubNub.unitypackage).

2. Import it to your Unity project by going to Assets -> Import Package -> Custom Package.

3. Configure your keys:

    ```csharp
    PNConfiguration pnConfiguration = new PNConfiguration();
    pnConfiguration.SubscribeKey = "my_subkey";
    pnConfiguration.PublishKey = "my_pubkey";
    pnConfiguration.SecretKey = "my_secretkey";
    pnConfiguration.LogVerbosity = PNLogVerbosity.BODY;
    pnConfiguration.UUID = "myUniqueUUID"; 
    ```

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
	if (mea.UserEventResult != null) {					
		Debug.Log(mea.UserEventResult.Name);
		Debug.Log(mea.UserEventResult.Email);
		Debug.Log(mea.UserEventResult.ExternalID);
		Debug.Log(mea.UserEventResult.ProfileURL);
		Debug.Log(mea.UserEventResult.UserID);
		Debug.Log(mea.UserEventResult.ETag);
		Debug.Log(mea.UserEventResult.ObjectsEvent);
	}
	if (mea.SpaceEventResult != null) {					
		Debug.Log(mea.SpaceEventResult.Name);
		Debug.Log(mea.SpaceEventResult.Description);
		Debug.Log(mea.SpaceEventResult.SpaceID);
		Debug.Log(mea.SpaceEventResult.ETag);
		Debug.Log(mea.SpaceEventResult.ObjectsEvent);
	} 
	if (mea.MembershipEventResult != null) {					
		Debug.Log(mea.MembershipEventResult.UserID);
		Debug.Log(mea.MembershipEventResult.Description);
		Debug.Log(mea.MembershipEventResult.SpaceID);
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

* [Build your first realtime Unity app with PubNub](https://www.pubnub.com/docs/platform/quickstarts/unity)
* [API reference for Unity](https://www.pubnub.com/docs/unity3d-c-sharp/pubnub-c-sharp-sdk)

## Support

If you **need help** or have a **general question**, contact support@pubnub.com.
