// snippet.here_now_basic_usage
using System.Collections.Generic;
using PubnubApi;
using PubnubApi.Unity;
using UnityEngine;

public class HereNowExample : MonoBehaviour {
    // Reference to a pubnub manager previously setup in Unity Editor
    // For more details, see https://www.pubnub.com/docs/sdks/unity#configure-pubnub
    [SerializeField] private PNManagerBehaviour pubnubManager;

    // An editor-serialized array with test channel IDs
    [SerializeField] private string[] testChannelIds = { "coolChannel", "coolChannel2" };

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

        // Executing the HereNow operation to get a list of UUIDs subscribed to channels
        var herenowResponse = await pubnub.HereNow()
            .Channels(testChannelIds)
            .IncludeUUIDs(true)
            .ExecuteAsync();

        // Result and status of the HereNow operation
        var herenowResult = herenowResponse.Result;
        var status = herenowResponse.Status;

        // Handling potential errors
        if (status.Error) {
            Debug.LogError($"Error in HereNow operation: {status.ErrorData.Information}");
        } else {
            if (herenowResult?.Channels != null && herenowResult.Channels.Count > 0) {
                foreach (KeyValuePair<string, PNHereNowChannelData> kvp in herenowResult.Channels) {
                    PNHereNowChannelData channelData = kvp.Value;

                    Debug.Log("---");
                    Debug.Log($"channel: {channelData.ChannelName}");
                    Debug.Log($"occupancy: {channelData.Occupancy}");
                    Debug.Log("Occupants:");

                    if (channelData.Occupants != null && channelData.Occupants.Count > 0) {
                        foreach (var occupant in channelData.Occupants) {
                            Debug.Log($"uuid: {occupant.Uuid}");
                            Debug.Log($"state: {(occupant.State != null ? pubnub.JsonPluggableLibrary.SerializeToJsonString(occupant.State) : "No state")}");
                        }
                    }
                }
            } else {
                Debug.Log("No occupants found.");
            }
        }
    }
}
// snippet.end