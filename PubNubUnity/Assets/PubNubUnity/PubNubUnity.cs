using System;
using UnityEngine;
using System.Linq;

namespace PubNubAPI
{
    public class PubNubUnity: PubNubUnityBase
    {
        public event EventHandler<EventArgs> SusbcribeCallback; 
        public void RaiseEvent(EventArgs ea){
            if (SusbcribeCallback != null) {
                SusbcribeCallback.Raise (typeof(PubNubUnity), ea);
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

        /*~PubNubUnity(){
            #if (ENABLE_PUBNUB_LOGGING)
            base.PNLog.WriteToLog ("Destructing PubnubUnity", PNLoggingMethod.LevelInfo);
            #endif
            this.CleanUp ();
        }*/

        public void Reconnect(){
            if(SubWorker != null){
                SubWorker.BounceRequest();
            }
        }

        /// <summary>
        /// Gets or sets the set game object.
        /// This method should be called before init
        /// </summary>
        /// <value>The set game object.</value>
        //public GameObject GameObjectRef { get; set;}
        public PubNubUnity (PNConfiguration pnConfiguration, GameObject gameObjectRef, IJsonLibrary jsonLibrary): base(pnConfiguration, gameObjectRef, jsonLibrary)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            base.PNLog.WriteToLog (string.Format("Init with UUID {0}", base.PNConfig.UUID), PNLoggingMethod.LevelInfo);
            #endif
            SubscriptionInstance = new Subscription (this);
            SubWorker = new SubscriptionWorker<SubscribeEnvelope>(this); 
            base.QManager.PubNubInstance = this;

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

        public void AddListener(Action<PNStatus> callback, Action<PNMessageResult> callback2, Action<PNPresenceEventResult> callback3)
        {
            SusbcribeCallback += (object sender, EventArgs e) => {
                SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;

                Debug.Log ("AddListener SusbcribeCallback");
                if(mea!=null){
                    if(mea.Status != null){
                        callback(mea.Status);
                    }
                    if(mea.MessageResult != null){
                        callback2(mea.MessageResult);
                    }
                    if(mea.PresenceEventResult != null){
                        callback3(mea.PresenceEventResult);
                    }
                }
            };
        }

        public SubscribeBuilder Subscribe(){
            return new SubscribeBuilder (this);
        }

        public TimeBuilder Time(){
            Debug.Log ("TimeBuilder");
            return new TimeBuilder (this);
        }

        public WhereNowBuilder WhereNow(){
            Debug.Log ("WhereNowBuilder");
            return new WhereNowBuilder (this);
        }

        public HereNowBuilder HereNow(){
            Debug.Log ("HereNowBuilder");
            return new HereNowBuilder (this);
        }

        public HistoryBuilder History(){
            Debug.Log ("HistoryBuilder");
            return new HistoryBuilder (this);
        }
        public FetchBuilder FetchMessages(){
            Debug.Log ("FetchBuilder");
            return new FetchBuilder  (this);
        }

        public DeleteMessagesBuilder DeleteMessages(){
            return new DeleteMessagesBuilder (this);
        }

        public PublishBuilder Publish(){
            Debug.Log ("PublishBuilder");
            return new PublishBuilder (this, publishMessageCounter.NextValue());
        }
        public FireBuilder Fire(){
            Debug.Log ("FireBuilder");
            return new FireBuilder (this, publishMessageCounter.NextValue());
        }
        
        public UnsubscribeBuilder Unsubscribe(){
            Debug.Log ("Unsubscribe");
            return new UnsubscribeBuilder (this);
        }
        
        public UnsubscribeAllBuilder UnsubscribeAll(){
            Debug.Log ("UnsubscribeAll");
            return new UnsubscribeAllBuilder (this);
        }
        
        public GetStateBuilder GetPresenceState(){
            Debug.Log ("GetPresenceState");
            return new GetStateBuilder(this);
        }
        public SetStateBuilder SetPresenceState(){
            Debug.Log ("SetPresenceState");
            return new SetStateBuilder(this);
        }

        public AddChannelsToChannelGroupBuilder AddChannelsToChannelGroup(){
            Debug.Log ("AddChannelsToChannelGroupBuilder");
            return new AddChannelsToChannelGroupBuilder(this);
        }

        public DeleteChannelGroupBuilder DeleteChannelGroup(){
            Debug.Log ("DeleteChannelGroupBuilder");
            return new DeleteChannelGroupBuilder(this);
        }

        public RemoveChannelsFromGroupBuilder RemoveChannelsFromChannelGroup(){
            Debug.Log ("RemoveChannelsFromGroupBuilder");
            return new RemoveChannelsFromGroupBuilder(this);
        }

        public GetAllChannelsForGroupBuilder ListChannelsForChannelGroup(){
            Debug.Log ("GetAllChannelsForGroupBuilder");
            return new GetAllChannelsForGroupBuilder(this);
        }

        public AddChannelsToPushBuilder AddPushNotificationsOnChannels(){
            Debug.Log ("AddPushNotificationsOnChannels");
            return new AddChannelsToPushBuilder(this);
        }

        public ListPushProvisionsBuilder AuditPushChannelProvisions(){
            Debug.Log ("AuditPushChannelProvisions");
            return new ListPushProvisionsBuilder(this);
        }

        public RemoveAllPushChannelsForDeviceBuilder RemoveAllPushNotifications(){
            Debug.Log ("RemoveAllPushNotifications");
            return new RemoveAllPushChannelsForDeviceBuilder(this);
        }

        public RemoveChannelsFromPushBuilder RemovePushNotificationsFromChannels(){
            Debug.Log ("RemovePushNotificationsFromChannels");
            return new RemoveChannelsFromPushBuilder(this);
        }
    }
}

