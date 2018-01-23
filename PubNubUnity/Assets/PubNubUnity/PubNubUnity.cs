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

                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog("AddListener SusbcribeCallback", PNLoggingMethod.LevelInfo);
                #endif
                
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
    }
}

