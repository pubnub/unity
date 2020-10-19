using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PubNub{
        public PNConfiguration PNConfig { get; set;}
        private PubNubUnity pnUnity;
        private bool cleanedUp = true;
        private readonly IJsonLibrary jsonLibrary;

        private readonly GameObject gameObj;

        public GameObject GameObjectRef {
            get {
                PubNubUnityInitializationAfterCleanup();
                return pnUnity.GameObjectRef;
            }
        }
 
        private void PubNubUnityInitializationAfterCleanup(){
            if(cleanedUp){
                pnUnity = new PubNubUnity(PNConfig, gameObj, jsonLibrary);
            
                pnUnity.SubscribeCallback += (sender, e) => { 
                    SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.pnUnity.PNLog.WriteToLog("SubscribeCallback PN", PNLoggingMethod.LevelInfo);
                    #endif
                    RaiseEvent(mea);
                };

                cleanedUp = false;
            }
        }

        public event EventHandler<EventArgs> SubscribeCallback; 

        [System.Obsolete("This is an obsolete event, use SubscribeCallback")]
        public event EventHandler<EventArgs> SusbcribeCallback; 
        private void RaiseEvent(EventArgs ea){
            if (SubscribeCallback != null) {
                try{
                    SubscribeCallback.Raise (typeof(PubNub), ea);
                }catch (Exception ex) {
                    throw new PubNubUserException(ex.Message, ex);
                }
            }
        }

        public string Version {
            get 
            {
                PubNubUnityInitializationAfterCleanup();
                return pnUnity.Version;
            }
        }
        
        public PubNub (PNConfiguration pnConfiguration): this(pnConfiguration, null, null)
        {

        }

        public PubNub (PNConfiguration pnConfiguration, GameObject gameObjectRef) : this(pnConfiguration, gameObjectRef, null)
        {
        }

        private PubNub (PNConfiguration pnConfiguration, GameObject gameObjectRef, IJsonLibrary jsonLibrary)
        {
            this.jsonLibrary = jsonLibrary;
            this.gameObj = gameObjectRef;
            this.PNConfig = pnConfiguration;
            PubNubUnityInitializationAfterCleanup();
        }

        public void CleanUp (){
            if(pnUnity != null){
                pnUnity.CleanUp();
                pnUnity = null;
            }
            cleanedUp = true;
        }

        ~PubNub(){
            #if (ENABLE_PUBNUB_LOGGING)
            if((pnUnity!=null) && (pnUnity.PNLog!=null)){
                pnUnity.PNLog.WriteToLog ("Destructing PubNub", PNLoggingMethod.LevelInfo);
            }
            #endif
            this.CleanUp ();
        }

        public IJsonLibrary JsonLibrary{
            get{
                if(pnUnity != null){
                    return pnUnity.JsonLibrary;
                } else {
                    return null;
                }
            }
        }

        public void Reconnect(){
            PubNubUnityInitializationAfterCleanup();
            pnUnity.Reconnect();
        }

        public SubscribeBuilder Subscribe(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.Subscribe();
        }

        public TimeBuilder Time(){            
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.Time();
        }

        public WhereNowBuilder WhereNow(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.WhereNow();
        }

        public HereNowBuilder HereNow(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.HereNow();
        }

        public HistoryBuilder History(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.History();
        }

        public FetchBuilder FetchMessages(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.FetchMessages();
        }

        public MessageCountsBuilder MessageCounts(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.MessageCounts();
        }

        public DeleteMessagesBuilder DeleteMessages(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.DeleteMessages();
        }

        public PublishBuilder Publish(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.Publish();
        }

        public FireBuilder Fire(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.Fire();
        }

        public SignalBuilder Signal(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.Signal();
        }

        public UnsubscribeBuilder Unsubscribe(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.Unsubscribe();
        }

        public UnsubscribeAllBuilder UnsubscribeAll(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.UnsubscribeAll();
        }

        public PresenceHeartbeatBuilder Presence(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.Presence();
        }

        public GetStateBuilder GetPresenceState(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.GetPresenceState();
        }

        public SetStateBuilder SetPresenceState(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.SetPresenceState();
        }

        public AddChannelsToChannelGroupBuilder AddChannelsToChannelGroup(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.AddChannelsToChannelGroup();
        }

        public DeleteChannelGroupBuilder DeleteChannelGroup(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.DeleteChannelGroup();
        }

        public RemoveChannelsFromGroupBuilder RemoveChannelsFromChannelGroup(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.RemoveChannelsFromChannelGroup();
        }

        public GetAllChannelsForGroupBuilder ListChannelsForChannelGroup(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.ListChannelsForChannelGroup();
        }

        public AddChannelsToPushBuilder AddPushNotificationsOnChannels(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.AddPushNotificationsOnChannels();
        }

        public ListPushProvisionsBuilder AuditPushChannelProvisions(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.AuditPushChannelProvisions();
        }

        public RemoveAllPushChannelsForDeviceBuilder RemoveAllPushNotifications(){
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.RemoveAllPushNotifications();
        }

        public RemoveChannelsFromPushBuilder RemovePushNotificationsFromChannels(){            
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.RemovePushNotificationsFromChannels();            
        }

        public SetUUIDMetadataBuilder SetUUIDMetadata(){            
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.SetUUIDMetadata();            
        }

        public RemoveUUIDMetadataBuilder RemoveUUIDMetadata(){            
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.RemoveUUIDMetadata();            
        }

        public GetUUIDMetadataBuilder GetUUIDMetadata(){            
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.GetUUIDMetadata();            
        }

        public GetAllUUIDMetadataBuilder GetAllUUIDMetadata(){            
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.GetAllUUIDMetadata();            
        }

        public SetChannelMetadataBuilder SetChannelMetadata()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.SetChannelMetadata();
        }

        public RemoveChannelMetadataBuilder RemoveChannelMetadata()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.RemoveChannelMetadata();
        }                        
        public GetChannelMetadataBuilder GetChannelMetadata()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.GetChannelMetadata();
        }
        public GetAllChannelMetadataBuilder GetAllChannelMetadata()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.GetAllChannelMetadata();
        }
        public GetChannelMembersBuilder GetChannelMembers()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.GetChannelMembers();
        }
        public GetMembershipsBuilder GetMemberships()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.GetMemberships();
        }
        public ManageChannelMembersBuilder ManageChannelMembers()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.ManageChannelMembers();
        }
       public SetMembershipsBuilder SetMemberships()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.SetMemberships();
        }  
        public RemoveMembershipsBuilder RemoveMemberships()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.RemoveMemberships();
        } 
       public SetChannelMembersBuilder SetChannelMembers()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.SetChannelMembers();
        }  
        public RemoveChannelMembersBuilder RemoveChannelMembers()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.RemoveChannelMembers();
        }               
        public ManageMembershipsBuilder ManageMemberships()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.ManageMemberships();
        }
        public AddMessageActionsBuilder AddMessageActions()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.AddMessageActions();
        }
        public RemoveMessageActionsBuilder RemoveMessageActions()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.RemoveMessageActions();
        }
        public GetMessageActionsBuilder GetMessageActions()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.GetMessageActions();
        }

        // Only used for an integration test
        internal GrantTokenBuilder GrantToken()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.GrantToken();
        }

        public void SetToken(string token)
        {
            PubNubUnityInitializationAfterCleanup();
            pnUnity.TokenMgr.StoreToken(token);
        }

        public void SetTokens(List<string> tokens)
        {
            PubNubUnityInitializationAfterCleanup();
            pnUnity.TokenMgr.StoreTokens(tokens);
        }

        public SendFileBuilder SendFile()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.SendFile();
        }

        public ListFilesBuilder ListFiles()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.ListFiles();
        }

        public GetFileURLBuilder GetFileURL()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.GetFileURL();
        }

        public DownloadFileBuilder DownloadFile()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.DownloadFile();
        }

        public DeleteFileBuilder DeleteFile()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.DeleteFile();
        }

        public PublishFileMessageBuilder PublishFileMessage()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.PublishFileMessage();
        }

        public GrantResourcesWithPermissions GetTokens()
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.TokenMgr.GetAllTokens();
        }

        public GrantResourcesWithPermissions GetTokensByResource(PNResourceType resourceType)
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.TokenMgr.GetTokensByResource(resourceType);
        }

        public string GetToken(string resourceID, PNResourceType resourceType)
        {
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.TokenMgr.GetToken(resourceID, resourceType);
        }

        public void ResetTokenManager(List<string> tokens)
        {
            PubNubUnityInitializationAfterCleanup();
            pnUnity.TokenMgr.CleanUp();
        }

        public void AddListener(Action<PNStatus> statusCallback, Action<PNMessageResult> messageCallback, Action<PNPresenceEventResult> presenceCallback)
        {
            PubNubUnityInitializationAfterCleanup();
            pnUnity.AddListener(statusCallback, messageCallback, presenceCallback, null, null, null, null, null);
        }

        public void AddListener(Action<PNStatus> statusCallback, Action<PNMessageResult> messageCallback, Action<PNPresenceEventResult> presenceCallback, Action<PNSignalEventResult> signalCallback)
        {
            PubNubUnityInitializationAfterCleanup();
            pnUnity.AddListener(statusCallback, messageCallback, presenceCallback, signalCallback, null, null, null, null);
        }

        public void AddListener(Action<PNStatus> statusCallback, Action<PNMessageResult> messageCallback, Action<PNPresenceEventResult> presenceCallback, Action<PNSignalEventResult> signalCallback, Action<PNUUIDEventResult> uuidEventCallback, Action<PNChannelEventResult> channelEventCallback, Action<PNMembershipEventResult> membershipCallback)
        {
            PubNubUnityInitializationAfterCleanup();
            pnUnity.AddListener(statusCallback, messageCallback, presenceCallback, signalCallback, uuidEventCallback, channelEventCallback, membershipCallback, null);
        }

        public void AddListener(Action<PNStatus> statusCallback, Action<PNMessageResult> messageCallback, Action<PNPresenceEventResult> presenceCallback, Action<PNSignalEventResult> signalCallback, Action<PNUUIDEventResult> uuidEventCallback, Action<PNChannelEventResult> channelEventCallback, Action<PNMembershipEventResult> membershipCallback, Action<PNMessageActionsEventResult> messageActionsCallback)
        {
            PubNubUnityInitializationAfterCleanup();
            pnUnity.AddListener(statusCallback, messageCallback, presenceCallback, signalCallback, uuidEventCallback, channelEventCallback, membershipCallback, messageActionsCallback);
        }

        public static long TranslateDateTimeToPubnubUnixNanoSeconds (DateTime dotNetUTCDateTime)
        {
            return Utility.TranslateDateTimeToPubnubUnixNanoSeconds (dotNetUTCDateTime);
        }

        public static DateTime TranslatePubnubUnixNanoSecondsToDateTime (long unixNanoSecondTime)
        {
            return Utility.TranslatePubnubUnixNanoSecondsToDateTime (unixNanoSecondTime);
        }

    }
}