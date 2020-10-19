using System;
using UnityEngine;
using System.Linq;

namespace PubNubAPI
{
    public class PubNubUnity: PubNubUnityBase
    {
        public event EventHandler<EventArgs> SubscribeCallback; 
        public void RaiseEvent(EventArgs ea){
            if (SubscribeCallback != null) {
                SubscribeCallback.Raise (typeof(PubNubUnity), ea);
            }
        }

        public new void CleanUp (){
            #if (ENABLE_PUBNUB_LOGGING)
            base.PNLog.WriteToLog ("CleanUp: Destructing ", PNLoggingMethod.LevelInfo);
            #endif

            if (SubWorker != null) {
                SubWorker.CleanUp();
            }
            
            #if (ENABLE_PUBNUB_LOGGING)
            base.PNLog.WriteToLog (string.Format ("Clean up complete."), PNLoggingMethod.LevelInfo);
            #endif
            base.CleanUp();
        }

        public void Reconnect(){
            if(SubWorker != null){
                SubWorker.BounceRequest();
            }
        }

        public PubNubUnity (PNConfiguration pnConfiguration, GameObject gameObjectRef, IJsonLibrary jsonLibrary): base(pnConfiguration, gameObjectRef, jsonLibrary)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            base.PNLog.WriteToLog (string.Format("Init with UUID {0}", base.PNConfig.UUID), PNLoggingMethod.LevelInfo);
            #endif
            SubscriptionInstance = new Subscription (this);
            SubWorker = new SubscriptionWorker<SubscribeEnvelope>(this);             
            base.QManager.PubNubInstance = this;
            base.tokenManager = new TokenManager(this);

            //TODO test
            PNConfig.UUIDChanged += (sender, e) =>{
                if(SubWorker != null){
                    SubWorker.UUIDChanged = true;
                    SubWorker.BounceRequest();
                }
            };

            PNConfig.FilterExpressionChanged += (sender, e) =>{
                if(SubWorker != null){
                    SubWorker.BounceRequest();
                }
            };

        }

        public void AddListener(Action<PNStatus> statusCallback, Action<PNMessageResult> messageCallback, Action<PNPresenceEventResult> presenceCallback, Action<PNSignalEventResult> signalCallback, Action<PNUUIDEventResult> uuidCallback, Action<PNChannelEventResult> channelEventCallback, Action<PNMembershipEventResult> membershipCallback, Action<PNMessageActionsEventResult> messageActionsCallback)
        {
            SubscribeCallback += (object sender, EventArgs e) => {
                SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;

                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog("AddListener SubscribeCallback", PNLoggingMethod.LevelInfo);
                #endif
                
                if(mea!=null){
                    if(mea.Status != null && statusCallback != null){
                        statusCallback(mea.Status);
                    }
                    if(mea.MessageResult != null && messageCallback != null){
                        messageCallback(mea.MessageResult);
                    }
                    if(mea.PresenceEventResult != null && presenceCallback != null){
                        presenceCallback(mea.PresenceEventResult);
                    }
                    if(mea.SignalEventResult != null && signalCallback != null){
                        signalCallback(mea.SignalEventResult);
                    }
                    if(mea.UUIDEventResult != null && uuidCallback != null){
                        uuidCallback(mea.UUIDEventResult);
                    }
                    if(mea.ChannelEventResult != null && channelEventCallback != null){
                        channelEventCallback(mea.ChannelEventResult);
                    }
                    if(mea.MembershipEventResult != null && membershipCallback != null){
                        membershipCallback(mea.MembershipEventResult);
                    }
                    if(mea.MessageActionsEventResult != null && messageActionsCallback != null){
                        messageActionsCallback(mea.MessageActionsEventResult);
                    }

                }
            };
        }

        public SubscribeBuilder Subscribe(){
            return new SubscribeBuilder (this);
        }

        public TimeBuilder Time(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("TimeBuilder", PNLoggingMethod.LevelInfo);
            #endif
            
            return new TimeBuilder (this);
        }

        public WhereNowBuilder WhereNow(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("WhereNowBuilder", PNLoggingMethod.LevelInfo);
            #endif
            
            return new WhereNowBuilder (this);
        }

        public HereNowBuilder HereNow(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("HereNowBuilder", PNLoggingMethod.LevelInfo);
            #endif
            
            return new HereNowBuilder (this);
        }

        public HistoryBuilder History(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("HistoryBuilder", PNLoggingMethod.LevelInfo);
            #endif
            
            return new HistoryBuilder (this);
        }
        public FetchBuilder FetchMessages(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("FetchBuilder", PNLoggingMethod.LevelInfo);
            #endif
            
            return new FetchBuilder  (this);
        }

        public MessageCountsBuilder MessageCounts(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("MessageCountsBuilder", PNLoggingMethod.LevelInfo);
            #endif
            
            return new MessageCountsBuilder  (this);
        }

        public DeleteMessagesBuilder DeleteMessages(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("DeleteMessagesBuilder", PNLoggingMethod.LevelInfo);
            #endif
            
            return new DeleteMessagesBuilder (this);
        }

        public PublishBuilder Publish(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("PublishBuilder", PNLoggingMethod.LevelInfo);
            #endif
            
            return new PublishBuilder (this, publishMessageCounter.NextValue());
        }
        public FireBuilder Fire(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("FireBuilder", PNLoggingMethod.LevelInfo);
            #endif
            
            return new FireBuilder (this, publishMessageCounter.NextValue());
        }
        
        public SignalBuilder Signal(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("SignalBuilder", PNLoggingMethod.LevelInfo);
            #endif
            
            return new SignalBuilder (this);
        }
        
        public UnsubscribeBuilder Unsubscribe(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("Unsubscribe", PNLoggingMethod.LevelInfo);
            #endif
            
            return new UnsubscribeBuilder (this);
        }
        
        public UnsubscribeAllBuilder UnsubscribeAll(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("UnsubscribeAll", PNLoggingMethod.LevelInfo);
            #endif
            
            return new UnsubscribeAllBuilder (this);
        }

        public PresenceHeartbeatBuilder Presence(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("Presence", PNLoggingMethod.LevelInfo);
            #endif
            
            return new PresenceHeartbeatBuilder(this);
        }
        
        public GetStateBuilder GetPresenceState(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("GetPresenceState", PNLoggingMethod.LevelInfo);
            #endif
            
            return new GetStateBuilder(this);
        }
        public SetStateBuilder SetPresenceState(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("SetPresenceState", PNLoggingMethod.LevelInfo);
            #endif
            
            return new SetStateBuilder(this);
        }

        public AddChannelsToChannelGroupBuilder AddChannelsToChannelGroup(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("AddChannelsToChannelGroupBuilder", PNLoggingMethod.LevelInfo);
            #endif
            
            return new AddChannelsToChannelGroupBuilder(this);
        }

        public DeleteChannelGroupBuilder DeleteChannelGroup(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("DeleteChannelGroupBuilder", PNLoggingMethod.LevelInfo);
            #endif
            
            return new DeleteChannelGroupBuilder(this);
        }

        public RemoveChannelsFromGroupBuilder RemoveChannelsFromChannelGroup(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("RemoveChannelsFromGroupBuilder", PNLoggingMethod.LevelInfo);
            #endif
            
            return new RemoveChannelsFromGroupBuilder(this);
        }

        public GetAllChannelsForGroupBuilder ListChannelsForChannelGroup(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("GetAllChannelsForGroupBuilder", PNLoggingMethod.LevelInfo);
            #endif

            return new GetAllChannelsForGroupBuilder(this);
        }

        public AddChannelsToPushBuilder AddPushNotificationsOnChannels(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("AddPushNotificationsOnChannels", PNLoggingMethod.LevelInfo);
            #endif
            
            return new AddChannelsToPushBuilder(this);
        }

        public ListPushProvisionsBuilder AuditPushChannelProvisions(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("AuditPushChannelProvisions", PNLoggingMethod.LevelInfo);
            #endif
            
            return new ListPushProvisionsBuilder(this);
        }

        public RemoveAllPushChannelsForDeviceBuilder RemoveAllPushNotifications(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("RemoveAllPushNotifications", PNLoggingMethod.LevelInfo);
            #endif
            
            return new RemoveAllPushChannelsForDeviceBuilder(this);
        }

        public RemoveChannelsFromPushBuilder RemovePushNotificationsFromChannels(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("RemovePushNotificationsFromChannels", PNLoggingMethod.LevelInfo);
            #endif
            
            return new RemoveChannelsFromPushBuilder(this);
        }
        public SetUUIDMetadataBuilder SetUUIDMetadata(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("SetUUIDMetadata", PNLoggingMethod.LevelInfo);
            #endif
            
            return new SetUUIDMetadataBuilder(this);
        }
        public RemoveUUIDMetadataBuilder RemoveUUIDMetadata(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("RemoveUUIDMetadata", PNLoggingMethod.LevelInfo);
            #endif
            
            return new RemoveUUIDMetadataBuilder(this);
        }
        public GetUUIDMetadataBuilder GetUUIDMetadata(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("GetUUIDMetadata", PNLoggingMethod.LevelInfo);
            #endif
            
            return new GetUUIDMetadataBuilder(this);
        }
        public GetAllUUIDMetadataBuilder GetAllUUIDMetadata(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("GetAllUUIDMetadata", PNLoggingMethod.LevelInfo);
            #endif
            
            return new GetAllUUIDMetadataBuilder(this);
        }
        public SetChannelMetadataBuilder SetChannelMetadata()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("SetChannelMetadata", PNLoggingMethod.LevelInfo);
            #endif

            return new SetChannelMetadataBuilder(this);
        }

        public RemoveChannelMetadataBuilder RemoveChannelMetadata()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("RemoveChannelMetadata", PNLoggingMethod.LevelInfo);
            #endif

            return new RemoveChannelMetadataBuilder(this);
        }
        public GetChannelMetadataBuilder GetChannelMetadata()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("GetChannelMetadata", PNLoggingMethod.LevelInfo);
            #endif

            return new GetChannelMetadataBuilder(this);
        }

        public GetAllChannelMetadataBuilder GetAllChannelMetadata()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("GetAllChannelMetadata", PNLoggingMethod.LevelInfo);
            #endif

            return new GetAllChannelMetadataBuilder(this);
        }

        public GetChannelMembersBuilder GetChannelMembers()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("GetChannelMembers", PNLoggingMethod.LevelInfo);
            #endif

            return new GetChannelMembersBuilder(this);
        }

        public GrantTokenBuilder GrantToken()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("GrantToken", PNLoggingMethod.LevelInfo);
            #endif

            return new GrantTokenBuilder(this);
        }

        public GetMembershipsBuilder GetMemberships()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("GetMemberships", PNLoggingMethod.LevelInfo);
            #endif

            return new GetMembershipsBuilder(this);
        }
        public ManageChannelMembersBuilder ManageChannelMembers()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("ManageChannelMembers", PNLoggingMethod.LevelInfo);
            #endif

            return new ManageChannelMembersBuilder(this);
        }
        public SetMembershipsBuilder SetMemberships()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("SetMemberships", PNLoggingMethod.LevelInfo);
            #endif

            return new SetMembershipsBuilder(this);
        }
        public RemoveMembershipsBuilder RemoveMemberships()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("RemoveMemberships", PNLoggingMethod.LevelInfo);
            #endif

            return new RemoveMembershipsBuilder(this);
        }
        public SetChannelMembersBuilder SetChannelMembers()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("SetChannelMembers", PNLoggingMethod.LevelInfo);
            #endif

            return new SetChannelMembersBuilder(this);
        }
        public RemoveChannelMembersBuilder RemoveChannelMembers()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("RemoveChannelMembers", PNLoggingMethod.LevelInfo);
            #endif

            return new RemoveChannelMembersBuilder(this);
        } 
        public ManageMembershipsBuilder ManageMemberships()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("ManageMemberships", PNLoggingMethod.LevelInfo);
            #endif

            return new ManageMembershipsBuilder(this);
        }                                
        public AddMessageActionsBuilder AddMessageActions()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("AddMessageActions", PNLoggingMethod.LevelInfo);
            #endif

            return new AddMessageActionsBuilder(this);
        }                                
        public RemoveMessageActionsBuilder RemoveMessageActions()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("RemoveMessageActions", PNLoggingMethod.LevelInfo);
            #endif

            return new RemoveMessageActionsBuilder(this);
        }                                
        public GetMessageActionsBuilder GetMessageActions()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("GetMessageActions", PNLoggingMethod.LevelInfo);
            #endif

            return new GetMessageActionsBuilder(this);
        }      

        public SendFileBuilder SendFile()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("SendFileBuilder", PNLoggingMethod.LevelInfo);
            #endif

            return new SendFileBuilder(this);
        }

        public ListFilesBuilder ListFiles()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("ListFiles", PNLoggingMethod.LevelInfo);
            #endif

            return new ListFilesBuilder(this);
        }

        public GetFileURLBuilder GetFileURL()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("GetFileURL", PNLoggingMethod.LevelInfo);
            #endif

            return new GetFileURLBuilder(this);
        }

        public DownloadFileBuilder DownloadFile()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("DownloadFile", PNLoggingMethod.LevelInfo);
            #endif

            return new DownloadFileBuilder(this);
        }

        public DeleteFileBuilder DeleteFile()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("DeleteFile", PNLoggingMethod.LevelInfo);
            #endif

            return new DeleteFileBuilder(this);
        }

        public PublishFileMessageBuilder PublishFileMessage()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog("PublishFileMessage", PNLoggingMethod.LevelInfo);
            #endif

            return new PublishFileMessageBuilder(this, publishMessageCounter.NextValue());
        }                                
    }
}

