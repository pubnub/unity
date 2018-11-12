using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class RemoveChannelsFromPushRequestBuilder: PubNubNonSubBuilder<RemoveChannelsFromPushRequestBuilder, PNPushRemoveChannelResult>, IPubNubNonSubscribeBuilder<RemoveChannelsFromPushRequestBuilder, PNPushRemoveChannelResult>
    {      
        public RemoveChannelsFromPushRequestBuilder(PubNubUnity pn):base(pn, PNOperationType.PNRemovePushNotificationsFromChannelsOperation){

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

        public void Async(Action<PNPushRemoveChannelResult, PNStatus> callback)
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
       
            base.Async(this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            
            Uri request = BuildRequests.BuildRemoveChannelPushRequest(
                string.Join(",", ChannelsToUse.ToArray()), 
                PushType, 
                DeviceIDForPush,
                this.PubNubInstance
            );

            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this);
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNPushRemoveChannelResult pnPushRemoveChannelResult = new PNPushRemoveChannelResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if(dictionary != null) {
                string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
                if(Utility.CheckDictionaryForError(dictionary, "error")){
                    pnPushRemoveChannelResult = null;
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
                        pnPushRemoveChannelResult = null;
                        pnStatus = base.CreateErrorResponseFromMessage(status, requestState, PNStatusCategory.PNUnknownCategory);
                    } else {
                        pnPushRemoveChannelResult.Message = status;
                    }
                } else {
                    pnPushRemoveChannelResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage("deSerializedResult object is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
                }
            }

            Callback(pnPushRemoveChannelResult, pnStatus);
        }

    }
}

