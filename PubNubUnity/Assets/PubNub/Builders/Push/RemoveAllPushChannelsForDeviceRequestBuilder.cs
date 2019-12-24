using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class RemoveAllPushChannelsForDeviceRequestBuilder: PubNubNonSubBuilder<RemoveAllPushChannelsForDeviceRequestBuilder, PNPushRemoveAllChannelsResult>, IPubNubNonSubscribeBuilder<RemoveAllPushChannelsForDeviceRequestBuilder, PNPushRemoveAllChannelsResult>
    {      
        public RemoveAllPushChannelsForDeviceRequestBuilder(PubNubUnity pn):base(pn, PNOperationType.PNRemoveAllPushNotificationsOperation){
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

        public void DeviceId(string deviceIdForPush){
            DeviceIDForPush = deviceIdForPush;
        }

        public PNPushType PushType {get;set;}

        #region IPubNubBuilder implementation

        public void Async(Action<PNPushRemoveAllChannelsResult, PNStatus> callback)
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
                request = BuildRequests.BuildRemoveAllDevicePushRequest(
                    PushType, 
                    DeviceIDForPush,
                    this.PubNubInstance,
                    this.QueryParams,
                    TopicForPush,
                    EnvironmentForPush
                );
            } else {
                request = BuildRequests.BuildRemoveAllDevicePushRequest(
                    PushType, 
                    DeviceIDForPush,
                    this.PubNubInstance,
                    this.QueryParams
                );
            }    

            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //Returned JSON `[1, "Removed Device"] `
            PNPushRemoveAllChannelsResult pnPushRemoveAllChannelsResult = new PNPushRemoveAllChannelsResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if(dictionary != null) {
                string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
                if(Utility.CheckDictionaryForError(dictionary, "error")){
                    pnPushRemoveAllChannelsResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
                }
            } else {
                object[] c = deSerializedResult as object[];
                
                if (c != null) {
                    string status = "";
                    string statusCode = "0";
                    if(c.Length > 0){
                        statusCode = c[0].ToString();
                    }
                    if(c.Length > 1){
                        status = c[1].ToString();
                    }
                    if(statusCode.Equals("0") || (!status.ToLowerInvariant().Equals("removed device"))){
                        pnPushRemoveAllChannelsResult = null;
                        pnStatus = base.CreateErrorResponseFromMessage(status, requestState, PNStatusCategory.PNUnknownCategory);
                    } else {
                        pnPushRemoveAllChannelsResult.Message = status;
                    }
                } else {
                    pnPushRemoveAllChannelsResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage("deSerializedResult object is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
                }
            } 

            Callback(pnPushRemoveAllChannelsResult, pnStatus);
        }
        
    }
}

