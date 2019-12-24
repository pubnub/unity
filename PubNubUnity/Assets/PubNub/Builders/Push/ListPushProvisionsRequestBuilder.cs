using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class ListPushProvisionsRequestBuilder: PubNubNonSubBuilder<ListPushProvisionsRequestBuilder, PNPushListProvisionsResult>, IPubNubNonSubscribeBuilder<ListPushProvisionsRequestBuilder, PNPushListProvisionsResult>
    {      
        public ListPushProvisionsRequestBuilder(PubNubUnity pn):base(pn, PNOperationType.PNPushNotificationEnabledChannelsOperation){
        }
        private string TopicForPush{ get; set;}
        private PNPushEnvironment EnvironmentForPush{ get; set;}
        public void Topic(string topic){
            TopicForPush = topic;
        }
        public void Environment(PNPushEnvironment environment){
            EnvironmentForPush = environment;
        }
        private string DeviceIDForPush{ get; set;}

        public void DeviceId(string deviceIdToPush){
            DeviceIDForPush = deviceIdToPush;
        }

        public PNPushType PushType {get;set;}
  
        #region IPubNubBuilder implementation

        public void Async(Action<PNPushListProvisionsResult, PNStatus> callback)
        {
            this.Callback = callback;
            if (string.IsNullOrEmpty (DeviceIDForPush)) {
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("DeviceId is empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);

                return;
            }

            if (PushType.Equals(PNPushType.None)) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog("PNPushType not selected, using GCM", PNLoggingMethod.LevelInfo);
                #endif               
                PushType = PNPushType.GCM;
            }
            
            if (PushType.Equals(PNPushType.APNS2) && (string.IsNullOrEmpty(TopicForPush))) {
                PNStatus pnStatus = base.CreateErrorResponseFromMessage(CommonText.APNS2TopicEmpty, null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);

                return;
            }
 
            base.Async(this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            
            Uri request;
            if(PushType.Equals(PNPushType.APNS2)){
                request = BuildRequests.BuildGetChannelsPushRequest(
                    PushType,
                    DeviceIDForPush,
                    this.PubNubInstance,
                    this.QueryParams,
                    TopicForPush,
                    EnvironmentForPush
                );
            } else {
                request = BuildRequests.BuildGetChannelsPushRequest(
                    PushType,
                    DeviceIDForPush,
                    this.PubNubInstance,
                    this.QueryParams
                );                
            }    

            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //Returned JSON `["channel1", "channel2"] `
            PNPushListProvisionsResult pnPushListProvisionsResult = new PNPushListProvisionsResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if(dictionary != null) {
                string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
                if(Utility.CheckDictionaryForError(dictionary, "error")){
                    pnPushListProvisionsResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
                }
            } else {
                object[] c = deSerializedResult as object[];
                
                if (c != null) {
                    pnPushListProvisionsResult.Channels = new List<string>();
                    foreach(string ch in c){
                        pnPushListProvisionsResult.Channels.Add(ch);
                    }
                } else {
                    pnPushListProvisionsResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage("Response dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
                }
            }

            Callback(pnPushListProvisionsResult, pnStatus);
        }

    }
}

