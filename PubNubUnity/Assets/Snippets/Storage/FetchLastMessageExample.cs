// snippet.fetch_history_basic_usage
using PubnubApi.Unity;
using UnityEngine;

public class FetchLastMessageExample : MonoBehaviour {
	// Reference to a pubnub manager previously setup in Unity Editor
	// For more details, see https://www.pubnub.com/docs/sdks/unity#configure-pubnub
	[SerializeField] private PNManagerBehaviour pubnubManager;

	// An editor-serialized string for the channel ID
	[SerializeField] private string channelId = "my_channel";

	private async void Start() {
		// Getting a reference to the Pubnub instance
		var pubnub = pubnubManager.pubnub;

		// Note that you can also initialize Pubnub instance for Unity directly from code:
		/*
		PNConfiguration pnConfiguration = new PNConfiguration(new UserId("myUniqueUserId"))
		{
			SubscribeKey = "demo",
			PublishKey = "demo",
		};
		Pubnub pubnub = PubnubUnityUtils.NewUnityPubnub(pnConfiguration);
		*/

		// Fetching the last message from the specified channel
		var fetchHistoryResponse = await pubnub.FetchHistory()
			.Channels(new string[] { channelId })
			.IncludeMeta(true)
			.MaximumPerChannel(1)  // Changed to 1 to specifically get the last message
			.IncludeCustomMessageType(true)
			.ExecuteAsync();

		// Checking the result and status of the fetch operation
		var fetchResult = fetchHistoryResponse.Result;
		var status = fetchHistoryResponse.Status;

		if (status.Error) {
			Debug.LogError($"Error fetching history: {status.ErrorData.Information}");
		} else if (fetchResult != null && fetchResult.Messages.ContainsKey(channelId)) {
			var messages = fetchResult.Messages[channelId];
			if (messages.Count > 0) {
				var lastMessage = messages[0];
				Debug.Log($"Last message: {lastMessage.Entry}, Timetoken: {lastMessage.Timetoken}");
			} else {
				Debug.Log("No messages found.");
			}
		}
	}
}
// snippet.end