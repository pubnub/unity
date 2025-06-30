// snippet.get_all_uuid_metadata_basic_usage
using PubnubApi;
using PubnubApi.Unity;
using UnityEngine;

public class GetAllUuidMetadataExample : MonoBehaviour {
	// Reference to a pubnub manager previously setup in Unity Editor
	// For more details, see https://www.pubnub.com/docs/sdks/unity#configure-pubnub
	[SerializeField] private PNManagerBehaviour pubnubManager;

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

		// Fetching all UUID metadata
		var getAllUuidMetadataResponse = await pubnub.GetAllUuidMetadata()
			.IncludeCustom(true)
			.IncludeCount(true)
			.ExecuteAsync();

		// Extracting the result and status
		var getAllUuidMetadataResult = getAllUuidMetadataResponse.Result;
		var status = getAllUuidMetadataResponse.Status;

		// Handling errors and logging results
		if (status.Error) {
			Debug.LogError($"Error fetching UUID metadata: {status.ErrorData.Information}");
		} else if (getAllUuidMetadataResult?.Uuids != null) {
			Debug.Log("Successfully fetched UUID metadata:");
			foreach (var uuidMetadata in getAllUuidMetadataResult.Uuids) {
				Debug.Log($"UUID: {uuidMetadata.Uuid}, Name: {uuidMetadata.Name}");
				if (uuidMetadata.Custom != null) {
					foreach (var kvp in uuidMetadata.Custom) {
						Debug.Log($"Custom Key: {kvp.Key}, Value: {kvp.Value}");
					}
				}
			}
		}
	}
}
// snippet.end