// snippet.using
using PubnubApi;
using PubnubApi.Unity;

// snippet.end
using UnityEngine;
using System;

public class PushSample
{
    private static Pubnub pubnub;

    static void Init()
    {
        // snippet.pubnub_init
        // Configuration
        PNConfiguration pnConfiguration = new PNConfiguration(new UserId("myUniqueUserId"))
        {
            SubscribeKey = "demo",
            PublishKey = "demo",
            Secure = true
        };

        // Initialize PubNub
        Pubnub pubnub = PubnubUnityUtils.NewUnityPubnub(pnConfiguration);

        // If you're using Unity Editor setup you can get the Pubnub instance from PNManagerBehaviour
        // For more details, see https://www.pubnub.com/docs/sdks/unity#configure-pubnub
        /*
        [SerializeField] private PNManagerBehaviour pubnubManager;
        Pubnub pubnub = pubnubManager.pubnub;
        */

        // snippet.end
    }

    public static void ListChannelsForDeviceBasicUsage()
    {
        // snippet.list_channels_for_device_basic_usage
        // for FCM/GCM
        pubnub.AuditPushChannelProvisions()
            .DeviceId("googleDevice")
            .PushType(PNPushType.FCM)
            .Execute(new PNPushListProvisionsResultExt((r, s) =>
            {
                Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(r));
            }));

        // for APNS2
        pubnub.AuditPushChannelProvisions()
            .DeviceId("appleDevice")
            .PushType(PNPushType.APNS2)
            .Topic("myapptopic")
            .Environment(PushEnvironment.Development)
            .Execute(new PNPushListProvisionsResultExt((r, s) =>
            {
                Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(r));
            }));
        // snippet.end
    }

    public static void RemoveDeviceFromChannelBasicUsage()
    {
        // snippet.remove_device_from_channel_basic_usage
        // for FCM/GCM
        pubnub.RemovePushNotificationsFromChannels()
            .DeviceId("googleDevice")
            .Channels(new string[] {
                "ch1",
                "ch2",
                "ch3"
            })
            .PushType(PNPushType.FCM)
            .Execute(new PNPushRemoveChannelResultExt((r, s) =>
            {
                Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(r));
            }));

        // for APNS2
        pubnub.RemovePushNotificationsFromChannels()
            .DeviceId("appleDevice")
            .Channels(new string[] {
                "ch1",
                "ch2",
                "ch3"
            })
            .PushType(PNPushType.APNS2)
            .Topic("myapptopic")
            .Environment(PushEnvironment.Development)
            .Execute(new PNPushRemoveChannelResultExt((r, s) =>
            {
                Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(r));
            }));
        // snippet.end
    }

    public static void RemoveAllPushNotificationsBasicUsage()
    {
        // snippet.remove_all_push_notifications_basic_usage
        // for FCM/GCM
        pubnub.RemoveAllPushNotificationsFromDeviceWithPushToken()
            .DeviceId("googleDevice")
            .PushType(PNPushType.FCM)
            .Execute(new PNPushRemoveAllChannelsResultExt((r, s) => {
                Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(r));
            }));

        // for APNS2
        pubnub.RemoveAllPushNotificationsFromDeviceWithPushToken()
            .DeviceId("appleDevice")
            .PushType(PNPushType.APNS2)
            .Topic("myapptopic")
            .Environment(PushEnvironment.Development)
            .Execute(new PNPushRemoveAllChannelsResultExt((r, s) => {
                Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(r));
            }));
        // snippet.end
    }
}