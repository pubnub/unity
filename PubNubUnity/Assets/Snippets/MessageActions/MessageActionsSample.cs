// snippet.using
using PubnubApi;
using PubnubApi.Unity;

// snippet.end
using UnityEngine;
using System;

public class MessageActionsSample
{
    private static Pubnub pubnub;

    static void Init()
    {
        // snippet.init
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

    public static void RemoveMessageActionBasicUsage()
    {
        // snippet.remove_message_action_basic_usage
        pubnub.RemoveMessageAction()
            .Channel("my_channel")
            .MessageTimetoken(15701761818730000)
            .ActionTimetoken(15701775691010000)
            .Uuid("mytestuuid")
            .Execute(new PNRemoveMessageActionResultExt((result, status) =>
            {
                //empty result of type PNRemoveMessageActionResult.
            }));
        // snippet.end
    }

    public static void GetMessageActionsBasicUsage()
    {
        // snippet.get_message_actions_basic_usage
        pubnub.GetMessageActions()
            .Channel("my_channel")
            .Execute(new PNGetMessageActionsResultExt((result, status) =>
            {
                //result is of type PNGetMessageActionsResult.
            }));
        // snippet.end
    }
}