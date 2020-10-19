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
        PNMessageCountsOperation,
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
        PNHereNowOperation,
        PNSignalOperation,
        PNGetMessageActionsOperation,
        PNAddMessageActionsOperation,
        PNRemoveMessageActionsOperation,
    	PNHistoryWithActionsOperation,	    	    
	    PNGetMembershipsOperation,
	    // PNGetChannelMembersOperation is the enum used to get members in the Object API.
	    PNGetChannelMembersOperation,
	    // PNManageMembershipsOperation is the enum used to manage memberships in the Object API.
	    PNManageMembershipsOperation,
	    // PNManageChannelMembersOperation is the enum used to manage members in the Object API.
	    PNManageChannelMembersOperation,
	    // PNSetChannelMembersOperation is the enum used to Set Members in the Object API.
	    PNSetChannelMembersOperation,
	    // PNSetMembershipsOperation is the enum used to Set Memberships in the Object API.
	    PNSetMembershipsOperation,
	    // PNRemoveChannelMetadataOperation is the enum used to Remove Channel Metadata in the Object API.
	    PNRemoveChannelMetadataOperation,
	    // PNRemoveUUIDMetadataOperation is the enum used to Remove UUID Metadata in the Object API.
	    PNRemoveUUIDMetadataOperation,
	    // PNGetAllChannelMetadataOperation is the enum used to Get All Channel Metadata in the Object API.
	    PNGetAllChannelMetadataOperation,
	    // PNGetAllUUIDMetadataOperation is the enum used to Get All UUID Metadata in the Object API.
	    PNGetAllUUIDMetadataOperation,
	    // PNGetUUIDMetadataOperation is the enum used to Get UUID Metadata in the Object API.
	    PNGetUUIDMetadataOperation,
	    // PNRemoveMembershipsOperation is the enum used to Remove Memberships in the Object API.
	    PNRemoveMembershipsOperation,
	    // PNRemoveChannelMembersOperation is the enum used to Remove Members in the Object API.
	    PNRemoveChannelMembersOperation,
	    // PNSetUUIDMetadataOperation is the enum used to Set UUID Metadata in the Object API.
	    PNSetUUIDMetadataOperation,
	    // PNSetChannelMetadataOperation is the enum used to Set Channel Metadata in the Object API.
	    PNSetChannelMetadataOperation,
	    // PNGetChannelMetadataOperation is the enum used to Get Channel Metadata in the Object API.
	    PNGetChannelMetadataOperation,
        PNGrantTokenOperation,
        // PNDeleteFileOperation is the enum used for DeleteFile requests.
        PNDeleteFileOperation,
        // PNDownloadFileOperation is the enum used for DownloadFile requests.
        PNDownloadFileOperation,
        // PNGetFileURLOperation is the enum used for GetFileURL requests.
        PNGetFileURLOperation,
        // PNListFilesOperation is the enum used for ListFiles requests.
        PNListFilesOperation,
        // PNSendFileOperation is the enum used for SendFile requests.
        PNSendFileOperation,
        // PNSendFileToS3Operation is the enum used for v requests.
        PNSendFileToS3Operation,
        // PNPublishFileMessageOperation is the enum used for PublishFileMessage requests.
        PNPublishFileMessageOperation,       
    }

}

