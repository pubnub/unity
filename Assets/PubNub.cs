using System;
using UnityEngine;
using System.Linq;

namespace PubNubAPI
{
    public class PubNub{
        public PNConfiguration PNConfig { get; set;}
        private PubNubUnity pnUnity;

        public GameObject GameObjectRef {
            get {
                return pnUnity.GameObjectRef;
            }
        }

        public event EventHandler<EventArgs> SusbcribeCallback; 
        private void RaiseEvent(EventArgs ea){
            if (SusbcribeCallback != null) {
                SusbcribeCallback.Raise (typeof(PubNub), ea);
            }
        }

        public string Version {
            get {
                return pnUnity.Version;
            }
        }
        
        public PubNub (PNConfiguration pnConfiguration): this(pnConfiguration, null, null)
        {

        }

        public PubNub (PNConfiguration pnConfiguration, GameObject gameObjectRef) : this(pnConfiguration, gameObjectRef, null)
        {
        }

        public PubNub (PNConfiguration pnConfiguration, GameObject gameObjectRef, IJsonLibrary jsonLibrary)
        {
            pnUnity = new PubNubUnity(pnConfiguration, gameObjectRef, jsonLibrary);
            
            pnUnity.SusbcribeCallback += (sender, e) => { //; //+= (pnStatus, pnMessageResut, pnPresenceEventResult) => {
                SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;

                Debug.Log ("SusbcribeCallback PN");
                RaiseEvent(mea);
            };
        }

         public SubscribeBuilder Subscribe(){
            return pnUnity.Subscribe();
        }

        public TimeBuilder Time(){
            Debug.Log ("TimeBuilder");
            return pnUnity.Time();
        }

        public WhereNowBuilder WhereNow(){
            Debug.Log ("WhereNowBuilder");
            return pnUnity.WhereNow();
        }

        public HereNowBuilder HereNow(){
            Debug.Log ("HereNowBuilder");
            return pnUnity.HereNow();
        }

        public HistoryBuilder History(){
            Debug.Log ("HistoryBuilder");
            return pnUnity.History();
        }

        public PublishBuilder Publish(){
            Debug.Log ("PubBuilder");
            return pnUnity.Publish();
        }
        public GetStateBuilder GetPresenceState(){
            Debug.Log ("GetPresenceState");
            return pnUnity.GetPresenceState();
        }

        public AddChannelsToChannelGroupBuilder AddChannelsToChannelGroup(){
            Debug.Log ("AddChannelsToChannelGroupBuilder");
            return pnUnity.AddChannelsToChannelGroup();
        }

        public DeleteChannelGroupBuilder DeleteChannelsFromChannelGroup(){
            Debug.Log ("DeleteChannelGroupBuilder");
            return pnUnity.DeleteChannelsFromChannelGroup();
        }

        public RemoveChannelsFromGroupBuilder RemoveChannelsFromChannelGroup(){
            Debug.Log ("RemoveChannelsFromGroupBuilder");
            return pnUnity.RemoveChannelsFromChannelGroup();
        }

        public GetAllChannelsForGroupBuilder ListChannelsForChannelGroup(){
            Debug.Log ("GetAllChannelsForGroupBuilder");
            return pnUnity.ListChannelsForChannelGroup();
        }

        public AddChannelsToPushBuilder AddPushNotificationsOnChannels(){
            Debug.Log ("AddChannelsToPushBuilder");
            return pnUnity.AddPushNotificationsOnChannels();
        }

        public ListPushProvisionsBuilder AuditPushChannelProvisions(){
            Debug.Log ("AuditPushChannelProvisions");
            return pnUnity.AuditPushChannelProvisions();
        }

        public RemoveAllPushChannelsForDeviceBuilder RemoveAllPushNotifications(){
            Debug.Log ("RemoveAllPushNotifications");
            return pnUnity.RemoveAllPushNotifications();
        }

        public RemoveChannelsFromPushBuilder RemovePushNotificationsFromChannels(){
            Debug.Log ("RemovePushNotificationsFromChannels");
            return pnUnity.RemovePushNotificationsFromChannels();
        }

        public void AddListener(Action<PNStatus> callback, Action<PNMessageResult> callback2, Action<PNPresenceEventResult> callback3)
        {
            pnUnity.AddListener(callback, callback2, callback3);
        }


    }
}