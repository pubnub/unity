using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class AddChannelsToPushRequestBuilder: PubNubNonSubBuilder<AddChannelsToPushRequestBuilder, PNPushAddChannelResult>, IPubNubNonSubscribeBuilder<AddChannelsToPushRequestBuilder, PNPushAddChannelResult>
    {      
        public AddChannelsToPushRequestBuilder(PubNubUnity pn):base(pn, PNOperationType.PNAddPushNotificationsOnChannelsOperation){
        }
        private string DeviceIDForPush{ get; set;}
        public void Channels(List<string> channelNames){
            ChannelsToUse = channelNames;
        }

        public void DeviceId(string deviceIdForPush){
            DeviceIDForPush = deviceIdForPush;
        }

        public PNPushType PushType {get;set;}
 
        #region IPubNubBuilder implementation

        public void Async(Action<PNPushAddChannelResult, PNStatus> callback)
        {
            this.Callback = callback;
            if((ChannelsToUse == null) || ((ChannelsToUse != null) && (ChannelsToUse.Count <= 0))){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("ChannelsToAdd null or empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);

                return;
            }

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
            base.Async(this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            
            Uri request = BuildRequests.BuildRegisterDevicePushRequest(
                string.Join(",", ChannelsToUse.ToArray()), 
                PushType, 
                DeviceIDForPush,
                this.PubNubInstance
            );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //Returned JSON `{"service" : "channel-registry","status"  : 200,"error"   : false,"message" : "OK"}`
            //Returned JSON `{"error": "Invalid device token"} `
            //Returned JSON `[1, "Modified Channels"] `
            //Returned JSON `{"error": "Use of the mobile push notifications API requires the Push Notifications add-on which is not enabled for this subscribe key. Login to your PubNub Dashboard Account and ADD the Push Notifications add-on. Contact support@pubnub.com if you require further assistance."} `
            PNPushAddChannelResult pnPushAddChannelResult = new PNPushAddChannelResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
             if(dictionary != null) {
                string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
                if(Utility.CheckDictionaryForError(dictionary, "error")){
                    pnPushAddChannelResult = null;
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
                    if(statusCode.Equals("0") || (!status.ToLowerInvariant().Equals("modified channels"))){
                        pnPushAddChannelResult = null;
                        pnStatus = base.CreateErrorResponseFromMessage(status, requestState, PNStatusCategory.PNUnknownCategory);
                    } else {
                        pnPushAddChannelResult.Message = status;
                    }
                } else {
                    pnPushAddChannelResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage("deSerializedResult object is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
                }
            } 
            Callback(pnPushAddChannelResult, pnStatus);
        }
        
    }
}

