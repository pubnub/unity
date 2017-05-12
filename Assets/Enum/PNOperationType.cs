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

        PNHistoryOperation,
        PNFetchMessagesOperation,

        PNWhereNowOperation,

        PNHeartbeatOperation,
        PNPresenceHeartbeatOperation,

        PNSetStateOperation,
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

        PNHereNowOperation,
        PNGetState,
        PNAccessManagerAudit,
        PNAccessManagerGrant
    }

}

