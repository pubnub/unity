using System;

namespace PubNubAPI
{
    public enum PNOperationType 
    {
        PNSubscribeOperation,
        PNPresenceOperation,
        PNUnsubscribeOperation,
        PNPresenceUnsubscribeOperation,

        PNPublishOperation,
        PNFireOperation,
        PNLeaveOperation,
        PNHistoryOperation,
        PNDeleteMessagesOperation,
        PNFetchMessagesOperation,

        PNWhereNowOperation,

        PNHeartbeatOperation,
        PNPresenceHeartbeatOperation,
        PNSetStateOperation,
        PNGetStateOperation,
        PNAddChannelsToGroupOperation,
        PNRemoveChannelsFromGroupOperation,
        PNChannelGroupsOperation,
        PNRemoveGroupOperation,
        PNChannelsForGroupOperation,
        PNPushNotificationEnabledChannelsOperation,
        PNAddPushNotificationsOnChannelsOperation,
        PNRemovePushNotificationsFromChannelsOperation,
        PNRemoveAllPushNotificationsOperation,
        PNTimeOperation,

        PNHereNowOperation
    }

}

