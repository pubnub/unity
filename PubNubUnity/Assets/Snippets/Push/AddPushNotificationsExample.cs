// snippet.add_device_to_channel_basic_usage
using PubnubApi;
using PubnubApi.Unity;
using UnityEngine;

public class AddPushNotificationsExample : MonoBehaviour {
    // Reference to a pubnub manager previously setup in Unity Editor
    // For more details, see https://www.pubnub.com/docs/sdks/unity#configure-pubnub
    [SerializeField] private PNManagerBehaviour pubnubManager;

    // An editor-serialized array for the channels to add notifications
    [SerializeField] private string[] channels = { "ch1", "ch2", "ch3" };

    // An editor-serialized string for the device IDs
    [SerializeField] private string fcmDeviceId = "googleDevice";
    [SerializeField] private string apnsDeviceId = "appleDevice";

    // An editor-serialized string for the APNS2 topic
    [SerializeField] private string apnsTopic = "myapptopic";

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

        // Adding FCM/GCM push notifications
        pubnub.AddPushNotificationsOnChannels()
            .PushType(PNPushType.FCM)
            .Channels(channels)
            .DeviceId(fcmDeviceId)
            .Execute((result, status) => {
                if (status.Error) {
                    Debug.LogError($"Error adding FCM notifications: {status.ErrorData.Information}");
                } else {
                    Debug.Log("Successfully added FCM notifications.");
                }
            });

        // Adding APNS2 push notifications
        pubnub.AddPushNotificationsOnChannels()
            .PushType(PNPushType.APNS2)
            .Channels(channels)
            .DeviceId(apnsDeviceId)
            .Topic(apnsTopic)
            .Environment(PushEnvironment.Development)
            .Execute((result, status) => {
                if (status.Error) {
                    Debug.LogError($"Error adding APNS2 notifications: {status.ErrorData.Information}");
                } else {
                    Debug.Log("Successfully added APNS2 notifications.");
                }
            });
    }
}
// snippet.end