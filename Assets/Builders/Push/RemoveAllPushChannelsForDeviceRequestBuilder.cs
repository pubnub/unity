using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class RemoveAllPushChannelsForDeviceRequestBuilder: PubNubNonSubBuilder<RemoveAllPushChannelsForDeviceRequestBuilder, PNPushRemoveAllChannelsResult>, IPubNubNonSubscribeBuilder<RemoveAllPushChannelsForDeviceRequestBuilder, PNPushRemoveAllChannelsResult>
    {      
        public RemoveAllPushChannelsForDeviceRequestBuilder(PubNubUnity pn):base(pn){

        }
        private string DeviceIDForPush{ get; set;}

        public void DeviceId(string deviceId){
            DeviceIDForPush = deviceId;
        }

        public PNPushType PushType {get;set;}

        #region IPubNubBuilder implementation

        public void Async(Action<PNPushRemoveAllChannelsResult, PNStatus> callback)
        {
            this.Callback = callback;
            Debug.Log ("RemoveAllPushChannelsForDeviceRequestBuilder Async");
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
            base.Async(callback, PNOperationType.PNRemoveAllPushNotificationsOperation, PNCurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.RespType = PNOperationType.PNRemoveAllPushNotificationsOperation;
            
            Uri request = BuildRequests.BuildRemoveAllDevicePushRequest(
                PushType, 
                DeviceIDForPush,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            );
            this.PubNubInstance.PNLog.WriteToLog(string.Format("Run PNPushRemoveAllChannelsResult {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //[1, "Removed Device"] 
            PNPushRemoveAllChannelsResult pnPushRemoveAllChannelsResult = new PNPushRemoveAllChannelsResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if (dictionary!=null && dictionary.ContainsKey("error") && dictionary["error"].Equals(true)){
                pnPushRemoveAllChannelsResult = null;
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
                    if(statusCode.Equals("0") || (!status.ToLower().Equals("removed device"))){
                        pnStatus.Error = true;
                    } else {
                        pnStatus.Error = false;
                        pnPushRemoveAllChannelsResult.Message = status;
                    }
                } else {
                    pnStatus.Error = true;
                }
            } else {
                pnPushRemoveAllChannelsResult = null;
                pnStatus.Error = true;
            }
            Callback(pnPushRemoveAllChannelsResult, pnStatus);
        }

        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }
        
        
    }
}

