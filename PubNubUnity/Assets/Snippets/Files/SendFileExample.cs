// snippet.send_file_basic_usage
using PubnubApi;
using PubnubApi.Unity;
using UnityEngine;

public class SendFileExample : MonoBehaviour {
	// Reference to a pubnub manager previously setup in Unity Editor
	// For more details, see https://www.pubnub.com/docs/sdks/unity#configure-pubnub
	[SerializeField] private PNManagerBehaviour pubnubManager;

	// An editor-serialized string for the channel ID
	[SerializeField] private string channelId = "my_channel";

	// An editor-serialized string for the file path
	[SerializeField] private string filePath = "cat_picture.jpg";

	// An editor-serialized string for the cipher key
	[SerializeField] private string cipherKey = "my_cipher_key";

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

		// Sending a file to the specified channel
		var fileUploadResponse = await pubnub.SendFile()
			.Channel(channelId)
			.File(filePath) // checks the bin folder if no path is provided
			.CipherKey(cipherKey) // Deprecated: Prefer setting up in PubNub config
			.Message("Look at this photo!")
			.CustomMessageType("file-message")
			.ExecuteAsync();

		// Extracting the result and status
		var fileUploadResult = fileUploadResponse.Result;
		var fileUploadStatus = fileUploadResponse.Status;

		// Handling errors and logging results
		if (!fileUploadStatus.Error && fileUploadResult != null) {
			Debug.Log($"File uploaded successfully: {pubnub.JsonPluggableLibrary.SerializeToJsonString(fileUploadResult)}");
		} else {
			Debug.LogError($"Error uploading file: {pubnub.JsonPluggableLibrary.SerializeToJsonString(fileUploadStatus)}");
		}
	}
}
// snippet.end