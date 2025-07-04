// snippet.getting_started_full
using UnityEngine;
using PubnubApi;
using PubnubApi.Unity;

public class PNManager : PNManagerBehaviour {
	// UserId identifies this client.
	public string userId;

	private async void Awake() {
		if (string.IsNullOrEmpty(userId)) {
			// It is recommended to change the UserId to a meaningful value to be able to identify this client.
			userId = System.Guid.NewGuid().ToString();
		}

		// Listener example.
		listener.onStatus += OnPnStatus;
		listener.onMessage += OnPnMessage;

		// Initialize will create a PubNub instance, pass the configuration object, and prepare the listener.
		Initialize(userId);

		// Modern API example
		Channel channel = pubnub.Channel("TestChannel");
		Subscription subscription = channel.Subscription();
		subscription.Subscribe<object>();

		// Or legacy subscription example
		// pubnub.Subscribe<string>().Channels(new[] { "TestChannel" }).Execute();

		// Publish example
		await pubnub.Publish().Channel("TestChannel").Message("Hello World from Unity!").ExecuteAsync();
	}

	void OnPnStatus(Pubnub pn, PNStatus status) {
		Debug.Log(status.Category == PNStatusCategory.PNConnectedCategory ? "Connected" : "Not connected");
	}

	void OnPnMessage(Pubnub pn, PNMessageResult<object> result) {
		Debug.Log($"Message received: {result.Message}");
	}

	protected override void OnDestroy() {
		// Use OnDestroy to clean up, e.g. unsubscribe from listeners.
		listener.onStatus -= OnPnStatus;
		listener.onMessage -= OnPnMessage;

		base.OnDestroy();
	}
}
// snippet.end