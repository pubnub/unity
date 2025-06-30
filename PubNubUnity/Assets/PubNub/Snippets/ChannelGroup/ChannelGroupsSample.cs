// snippet.using
using PubnubApi;
using PubnubApi.Unity;

// snippet.end
using System.Threading.Tasks;

public class ChannelGroupsSample
{
    private static Pubnub pubnub;

    static void PubnubInit()
    {
        // snippet.pubnub_init
        //Create configuration
        PNConfiguration pnConfiguration = new PNConfiguration(new UserId("myUniqueUserId"))
        {
            SubscribeKey = "demo",
            PublishKey = "demo"
        };
        //Create a new PubNub instance
        Pubnub pubnub = PubnubUnityUtils.NewUnityPubnub(pnConfiguration);

        // If you're using Unity Editor setup you can get the Pubnub instance from PNManagerBehaviour
        // For more details, see https://www.pubnub.com/docs/sdks/unity#configure-pubnub
        /*
        [SerializeField] private PNManagerBehaviour pubnubManager;
        Pubnub pubnub = pubnubManager.pubnub;
        */

        // snippet.end
    }

    static async Task ListFromGroup()
    {
        // snippet.list
        PNResult<PNChannelGroupsAllChannelsResult> cgListChResponse = await pubnub.ListChannelsForChannelGroup()
            .ChannelGroup("cg1")
            .ExecuteAsync();
        // snippet.end
    }

    static async Task RemoveFromGroup()
    {
        // snippet.remove
        PNResult<PNChannelGroupsRemoveChannelResult> rmChFromCgResponse = await pubnub.RemoveChannelsFromChannelGroup()
            .ChannelGroup("family")
            .Channels(new string[] {
                "son"
            })
            .ExecuteAsync();
        // snippet.end
    }

    static async Task DeleteGroup()
    {
        // snippet.delete
        PNResult<PNChannelGroupsDeleteGroupResult> delCgResponse = await pubnub.DeleteChannelGroup()
            .ChannelGroup("family")
            .ExecuteAsync();
        // snippet.end
    }
}