// snippet.add_to_group
using PubnubApi.Unity;
using UnityEngine;

public class AddChannelsToGroupExample : MonoBehaviour {
	// Reference to a pubnub manager previously setup in Unity Editor
	// For more details, see https://www.pubnub.com/docs/sdks/unity#configure-pubnub
	[SerializeField] private PNManagerBehaviour pubnubManager;

	// An editor-serialized string for the channel group ID
	[SerializeField] private string channelGroupId = "cg1";

	// An editor-serialized array for the channels to add
	[SerializeField] private string[] channelsToAdd = { "ch1", "ch2", "ch3" };

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

		// Adding channels to the specified channel group
		var cgAddChResponse = await pubnub.AddChannelsToChannelGroup()
			.ChannelGroup(channelGroupId)
			.Channels(channelsToAdd)
			.ExecuteAsync();

		// Checking the status of the operation
		var status = cgAddChResponse.Status;
		if (status.Error) {
			Debug.LogError($"Error adding channels to group: {status.ErrorData.Information}");
		} else {
			Debug.Log($"Successfully added channels to group {channelGroupId}");
		}
	}
}
// snippet.end