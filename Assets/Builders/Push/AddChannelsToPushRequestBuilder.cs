using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class AddChannelsToPushRequestBuilder: PubNubNonSubBuilder<AddChannelsToPushRequestBuilder, PNPushAddChannelResult>, IPubNubNonSubscribeBuilder<AddChannelsToPushRequestBuilder, PNPushAddChannelResult>
    {      
        public AddChannelsToPushRequestBuilder(PubNubUnity pn):base(pn){
            Debug.Log ("AddChannelsToPushRequestBuilder Construct");

        }
        //private List<string> ChannelsToUse { get; set;}
        private string DeviceIDForPush{ get; set;}
        public void Channels(List<string> channels){
            ChannelsToUse = channels;
        }

        public void DeviceId(string deviceId){
            DeviceIDForPush = deviceId;
        }

        public PNPushType PushType {get;set;}
 
        #region IPubNubBuilder implementation

        public void Async(Action<PNPushAddChannelResult, PNStatus> callback)
        {
            this.Callback = callback;
            Debug.Log ("AddChannelsToPushRequestBuilder Async");
            if((ChannelsToUse == null) || ((ChannelsToUse != null) && (ChannelsToUse.Count <= 0))){
                Debug.Log("ChannelsToAdd null or empty");

                //TODO Send callback
                return;
            }

            if (string.IsNullOrEmpty (DeviceIDForPush)) {
                Debug.Log("DeviceId is empty");

                //TODO Send callback
                return;
            }

            if (PushType.Equals(PNPushType.None)) {
                Debug.Log("PNPushType not selected, using GCM");
                PushType = PNPushType.GCM;
                //TODO Send callback
                return;
            }
            base.Async(callback, PNOperationType.PNAddPushNotificationsOnChannelsOperation, PNCurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.RespType = PNOperationType.PNAddPushNotificationsOnChannelsOperation;
            
            Uri request = BuildRequests.BuildRegisterDevicePushRequest(
                string.Join(",", ChannelsToUse.ToArray()), 
                PushType, 
                DeviceIDForPush,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            );

            this.PubNubInstance.PNLog.WriteToLog(string.Format("Run PNAddPushNotificationsOnChannelsOperation {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //{"service" : "channel-registry","status"  : 200,"error"   : false,"message" : "OK"}
            //{"error": "Invalid device token"} 
            //[1, "Modified Channels"] 
            //{"error": "Use of the mobile push notifications API requires the Push Notifications add-on which is not enabled for this subscribe key. Login to your PubNub Dashboard Account and ADD the Push Notifications add-on. Contact support@pubnub.com if you require further assistance."} 
            //TODO read all values.
            PNPushAddChannelResult pnPushAddChannelResult = new PNPushAddChannelResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if (dictionary!=null && dictionary.ContainsKey("error") && dictionary["error"].Equals(true)){
                pnPushAddChannelResult = null;
                pnStatus.Error = true;
                //TODO create error data
            } else if(dictionary==null) {
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
                    if(statusCode.Equals("0") || (!status.ToLower().Equals("modified channels"))){
                        pnStatus.Error = true;
                    } else {
                        pnStatus.Error = false;
                        pnPushAddChannelResult.Message = status;
                    }
                } else {
                    pnStatus.Error = true;
                }
            } else {
                pnPushAddChannelResult = null;
                pnStatus.Error = true;
            }
            Callback(pnPushAddChannelResult, pnStatus);
        }
        
        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }
        
    }
}

