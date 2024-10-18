using System.Collections;
using System.Collections.Generic;
using PubnubApi;
using PubnubApi.Unity;
using UnityEngine;

public class PNWebGL : PNManagerBehaviour
{
    // UserId identifies this client.
	public string userId;

	private async void Awake() {

		Debug.LogError("HELLO");

		if (string.IsNullOrEmpty(userId)) {
			// It is recommended to change the UserId to a meaningful value, to be able to identify this client.
			userId = System.Guid.NewGuid().ToString();
		}

		// Listener example.
		listener.onStatus += OnPnStatus;
		listener.onMessage += OnPnMessage;
		listener.onPresence += OnPnPresence;
		listener.onFile += OnPnFile;
		listener.onObject += OnPnObject;
		listener.onSignal += OnPnSignal;
		listener.onMessageAction += OnPnMessageAction;

		// Initialize will create a PubNub instance, pass the configuration object, and prepare the listener.
		Debug.LogError("BEFORE INIT");
		Initialize(userId);

		// Subscribe example
		Debug.LogError("SUBBING:");
		pubnub.Subscribe<string>().Channels(new[] { "TestChannel" }).Execute();

		// Publish example
		Debug.LogError("PUBBING:");
		await pubnub.Publish().Channel("TestChannel").Message("Hello World from Unity!").ExecuteAsync();
		Debug.LogError("AFTER PUBBING:");
	}

	private void OnPnMessageAction(Pubnub pn, PNMessageActionEventResult result) {
		Debug.LogError(result.Channel);
	}

	private void OnPnSignal(Pubnub pn, PNSignalResult<object> result) {
		Debug.LogError(result.Channel);
	}

	private void OnPnObject(Pubnub pn, PNObjectEventResult result) {
		Debug.LogError(result.Channel);
	}

	private void OnPnFile(Pubnub pn, PNFileEventResult result) {
		Debug.LogError(result.Channel);
	}

	private void OnPnPresence(Pubnub pn, PNPresenceEventResult result) {
		Debug.LogError(result.Event);
	}

	private void OnPnStatus(Pubnub pn, PNStatus status) {
		Debug.LogError(status.Category == PNStatusCategory.PNConnectedCategory ? "Connected" : "Not connected");
	}

	private void OnPnMessage(Pubnub pn, PNMessageResult<object> result) {
		Debug.LogError($"Message received: {result.Message}");
	}

	protected override void OnDestroy() {
		// Use OnDestroy to clean up, e.g. unsubscribe from listeners.
		listener.onStatus -= OnPnStatus;
		listener.onMessage -= OnPnMessage;
		listener.onPresence -= OnPnPresence;
		listener.onFile -= OnPnFile;
		listener.onObject -= OnPnObject;
		listener.onSignal -= OnPnSignal;
		listener.onMessageAction -= OnPnMessageAction;

		base.OnDestroy();
	}
}
