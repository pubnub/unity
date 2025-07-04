// snippet.add_message_action_basic_usage
using PubnubApi;
using PubnubApi.Unity;
using UnityEngine;

public class AddMessageActionExample : MonoBehaviour {
	// Reference to a pubnub manager previously setup in Unity Editor
	// For more details, see https://www.pubnub.com/docs/sdks/unity#configure-pubnub
	[SerializeField] private PNManagerBehaviour pubnubManager;

	// An editor-serialized string for the channel ID
	[SerializeField] private string channelId = "my_channel";

	// An editor-serialized timetoken for the message
	[SerializeField] private long messageTimetoken = 5610547826969050;

	private void Start() {
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

		// Adding a message action (reaction) to a specified message
		pubnub.AddMessageAction()
			.Channel(channelId)
			.MessageTimetoken(messageTimetoken)
			.Action(new PNMessageAction { Type = "reaction", Value = "smiley_face" })
			.Execute((result, status) => {
				// Handling the result of the operation
				if (status.Error) {
					Debug.LogError($"Error adding message action: {status.ErrorData.Information}");
				} else {
					Debug.Log($"Successfully added message action: {pubnub.JsonPluggableLibrary.SerializeToJsonString(result)}");
				}
			});
	}
}
// snippet.end