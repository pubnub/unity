// snippet.pubnub_basic_usage
using PubnubApi;
using PubnubApi.Unity;
using UnityEngine;

public class PubnubBasicUsageExample : MonoBehaviour {

	//Reference to a pubnub manager previously setup in Unity Editor
	//For more details see https://www.pubnub.com/docs/sdks/unity#configure-pubnub
	[SerializeField] private PNManagerBehaviour pubnubManager;

	//An editor-serialized string with the test channel ID
	[SerializeField] private string testChannelId = "test_channel_id";

	private async void Start() {
		//Getting a reference to the Pubnub instance
		var pubnub = pubnubManager.pubnub;

		//Subscribing to string messages on an example channel
		pubnub.Subscribe<string>().Channels(new []{testChannelId}).Execute();

		//Setting up a simple callback on receiving a message
		pubnubManager.listener.onMessage += (pn, result) => {
			Debug.Log($"Received a message on {result.Channel}: {result.Message}");
		};

		//Sending a message that will be received by our callback
		var publishResult = await pubnub.Publish().Message("Hello from PubNub!").Channel(testChannelId).ExecuteAsync();

		//Doing simple error handling in case something went wrong during the publish
		if (publishResult.Status.Error) {
			Debug.LogError($"Error in publish operation: {publishResult.Status.ErrorData.Information}");
		}
	}

	// You can also use Pubnub inside Unity without any editor-side setup:
	private async void NoEditorSetupExample() {
		PNConfiguration pnConfiguration = new PNConfiguration(new UserId("myUniqueUserId"))
		{
			SubscribeKey = "demo",
			PublishKey = "demo",
		};
		Pubnub pubnub = PubnubUnityUtils.NewUnityPubnub(pnConfiguration);

		//Setting up a simple callback on receiving a message
		SubscribeCallbackListener listener = new SubscribeCallbackListener();
		listener.onMessage += (pn, result) => {
			Debug.Log($"Received a message on {result.Channel}: {result.Message}");
		};
		pubnub.AddListener(listener);

		//Subscribing to string messages on an example channel
		pubnub.Subscribe<string>().Channels(new []{testChannelId}).Execute();

		//Sending a message that will be received by our callback
		var publishResult = await pubnub.Publish().Message("Hello from PubNub!").Channel(testChannelId).ExecuteAsync();

		//Doing simple error handling in case something went wrong during the publish
		if (publishResult.Status.Error) {
			Debug.LogError($"Error in publish operation: {publishResult.Status.ErrorData.Information}");
		}
	}
}
// snippet.end