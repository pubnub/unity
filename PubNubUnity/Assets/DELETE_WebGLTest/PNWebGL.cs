using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PubnubApi;
using PubnubApi.Unity;
using UnityEngine;

public class PNWebGL : PNManagerBehaviour
{
	public string userId;

	private async void Awake()
	{
		Debug.LogError("Awoke!");

		await Task.Delay(3000);

		Init();
	}

	private async void Init() {
		if (string.IsNullOrEmpty(userId)) {
			// It is recommended to change the UserId to a meaningful value to be able to identify this client.
			userId = System.Guid.NewGuid().ToString();
		}

		// Listener example.
		listener.onStatus += OnPnStatus;
		listener.onMessage += OnPnMessage;

		// Initialize will create a PubNub instance, pass the configuration object, and prepare the listener.
		Initialize(userId);
		Debug.LogWarning("Inited!");

		await Task.Delay(3000);

		// Subscribe example
		pubnub.Subscribe<string>().Channels(new[] { "TestChannel" }).Execute();
		Debug.LogWarning("Subbed!");

		await Task.Delay(5000);

		// Publish example
		await pubnub.Publish().Channel("TestChannel").Message("Hello World from WEEEEEEB GLLLLL!").ExecuteAsync().ConfigureAwait(false);
		Debug.LogWarning("Pubbed!");
	}

	void OnPnStatus(Pubnub pn, PNStatus status) {
		Debug.LogWarning(status.Category == PNStatusCategory.PNConnectedCategory ? "Connected" : "Not connected");
	}

	void OnPnMessage(Pubnub pn, PNMessageResult<object> result) {
		Debug.LogWarning($"Message received: {result.Message}");
	}

	protected override void OnDestroy() {
		// Use OnDestroy to clean up, for example, to unsubscribe from listeners.
		listener.onStatus -= OnPnStatus;
		listener.onMessage -= OnPnMessage;

		base.OnDestroy();
	}
}
