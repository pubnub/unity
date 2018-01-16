using System;
using UnityEngine;
using System.Linq;

namespace PubNubAPI
{
    public class PubNub{
        public PNConfiguration PNConfig { get; set;}
        private PubNubUnity pnUnity;
        private bool cleanedUp = true;
        private IJsonLibrary jsonLibrary = null;

        private GameObject gameObj;

        public GameObject GameObjectRef {
            get {
                PubNubUnityInitializationAfterCleanup();
                return pnUnity.GameObjectRef;
            }
        }
 
        private void PubNubUnityInitializationAfterCleanup(){
            if(cleanedUp){
                pnUnity = new PubNubUnity(PNConfig, gameObj, jsonLibrary);
            
                pnUnity.SusbcribeCallback += (sender, e) => { 
                    SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;

                    Debug.Log ("SusbcribeCallback PN");
                    RaiseEvent(mea);
                };

                cleanedUp = false;
            }
        }

        public event EventHandler<EventArgs> SusbcribeCallback; 
        private void RaiseEvent(EventArgs ea){
            if (SusbcribeCallback != null) {
                SusbcribeCallback.Raise (typeof(PubNub), ea);
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
            Debug.Log ("TimeBuilder");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.Time();
        }

        public WhereNowBuilder WhereNow(){
            Debug.Log ("WhereNowBuilder");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.WhereNow();
        }

        public HereNowBuilder HereNow(){
            Debug.Log ("HereNowBuilder");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.HereNow();
        }

        public HistoryBuilder History(){
            Debug.Log ("HistoryBuilder");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.History();
        }

        public FetchBuilder FetchMessages(){
            Debug.Log ("FetchBuilder");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.FetchMessages();
        }

        public DeleteMessagesBuilder DeleteMessages(){
            Debug.Log ("DeleteBuilder");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.DeleteMessages();
        }

        public PublishBuilder Publish(){
            Debug.Log ("PubBuilder");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.Publish();
        }

        public FireBuilder Fire(){
            Debug.Log ("Fire Builder");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.Fire();
        }

        public UnsubscribeBuilder Unsubscribe(){
            Debug.Log ("UnsubscribeBuilder");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.Unsubscribe();
        }

        public UnsubscribeAllBuilder UnsubscribeAll(){
            Debug.Log ("UnsubscribeAll");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.UnsubscribeAll();
        }

        public GetStateBuilder GetPresenceState(){
            Debug.Log ("GetPresenceState");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.GetPresenceState();
        }

        public SetStateBuilder SetPresenceState(){
            Debug.Log ("SetPresenceState");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.SetPresenceState();
        }

        public AddChannelsToChannelGroupBuilder AddChannelsToChannelGroup(){
            Debug.Log ("AddChannelsToChannelGroupBuilder");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.AddChannelsToChannelGroup();
        }

        public DeleteChannelGroupBuilder DeleteChannelGroup(){
            Debug.Log ("DeleteChannelGroupBuilder");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.DeleteChannelGroup();
        }

        public RemoveChannelsFromGroupBuilder RemoveChannelsFromChannelGroup(){
            Debug.Log ("RemoveChannelsFromGroupBuilder");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.RemoveChannelsFromChannelGroup();
        }

        public GetAllChannelsForGroupBuilder ListChannelsForChannelGroup(){
            Debug.Log ("GetAllChannelsForGroupBuilder");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.ListChannelsForChannelGroup();
        }

        public AddChannelsToPushBuilder AddPushNotificationsOnChannels(){
            Debug.Log ("AddChannelsToPushBuilder");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.AddPushNotificationsOnChannels();
        }

        public ListPushProvisionsBuilder AuditPushChannelProvisions(){
            Debug.Log ("AuditPushChannelProvisions");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.AuditPushChannelProvisions();
        }

        public RemoveAllPushChannelsForDeviceBuilder RemoveAllPushNotifications(){
            Debug.Log ("RemoveAllPushNotifications");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.RemoveAllPushNotifications();
        }

        public RemoveChannelsFromPushBuilder RemovePushNotificationsFromChannels(){
            Debug.Log ("RemovePushNotificationsFromChannels");
            PubNubUnityInitializationAfterCleanup();
            return pnUnity.RemovePushNotificationsFromChannels();            
        }

        public void AddListener(Action<PNStatus> callback, Action<PNMessageResult> callback2, Action<PNPresenceEventResult> callback3)
        {
            PubNubUnityInitializationAfterCleanup();
            pnUnity.AddListener(callback, callback2, callback3);
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