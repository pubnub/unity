using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class RemoveChannelsFromPushRequestBuilder: PubNubNonSubBuilder<RemoveChannelsFromPushRequestBuilder, PNPushRemoveChannelResult>, IPubNubNonSubscribeBuilder<RemoveChannelsFromPushRequestBuilder, PNPushRemoveChannelResult>
    {      
        public RemoveChannelsFromPushRequestBuilder(PubNubUnity pn):base(pn){

        }

        private List<string> ChannelsToRemove { get; set;}
        private string DeviceIDForPush{ get; set;}
        public void Channels(List<string> channels){
            ChannelsToRemove = channels;
        }

        public void DeviceId(string deviceId){
            DeviceIDForPush = deviceId;
        }

        public PNPushType PushType {get;set;}
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNPushRemoveChannelResult, PNStatus> callback)
        {
            this.Callback = callback;
            Debug.Log ("RemoveChannelsFromPushRequestBuilder Async");
            base.Async(callback, PNOperationType.PNRemovePushNotificationsFromChannelsOperation, PNCurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState<PNPushRemoveChannelResult> requestState = new RequestState<PNPushRemoveChannelResult> ();
            requestState.RespType = PNOperationType.PNRemovePushNotificationsFromChannelsOperation;
            
            Uri request = BuildRequests.BuildRemoveChannelPushRequest(
                string.Join(",", ChannelsToRemove.ToArray()), 
                PushType, 
                DeviceIDForPush,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            );

            this.PubNubInstance.PNLog.WriteToLog(string.Format("Run PNPushRemoveChannelResult {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult){
            PNPushRemoveChannelResult pnPushRemoveChannelResult = new PNPushRemoveChannelResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if (dictionary!=null && dictionary.ContainsKey("error") && dictionary["error"].Equals(true)){
                pnPushRemoveChannelResult = null;
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
                        pnPushRemoveChannelResult.Message = status;
                    }
                } else {
                    pnStatus.Error = true;
                }
            } else {
                pnPushRemoveChannelResult = null;
                pnStatus.Error = true;
            }
            Callback(pnPushRemoveChannelResult, pnStatus);
        }

        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }
        
        
    }
}

