using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class SignalRequestBuilder: PubNubNonSubBuilder<SignalRequestBuilder, PNSignalResult>, IPubNubNonSubscribeBuilder<SignalRequestBuilder, PNSignalResult>
    {        
        private object SignalMessage { get; set;}
        private string SignalChannel { get; set;}
        
        public SignalRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNSignalOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNSignalResult, PNStatus> callback)
        {
            this.Callback = callback;
            if(string.IsNullOrEmpty(this.PubNubInstance.PNConfig.PublishKey)){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("Publish Key is empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);
                return;
            }
            if(string.IsNullOrEmpty(this.SignalChannel)){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("Channel is empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);
                return;
            }
            if(this.SignalMessage == null){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("Message is empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);
                return;
            }
            
            base.Async(this);
        }
        #endregion

        public SignalRequestBuilder Message(object messageToPublish){
            SignalMessage = messageToPublish;
            return this;
        }

        public SignalRequestBuilder Channel(string channelName){
            SignalChannel = channelName;
            return this;
        }        

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            string jsonMessage = Helpers.JsonEncodePublishMsg (SignalMessage, "", this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            Uri request = BuildRequests.BuildSignalRequest(
                    this.SignalChannel,
                    jsonMessage,
                    this.PubNubInstance,
                    this.QueryParams
                );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            object[] c = deSerializedResult as object[];
            PNSignalResult pnSignalResult = new PNSignalResult();
            PNStatus pnStatus = new PNStatus();
            if (c != null) {
                string status = "";
                string statusCode = "0";
                if(c.Length > 0){
                    statusCode = c[0].ToString();
                }
                if(c.Length > 1){
                    status = c[1].ToString();
                }
                if(statusCode.Equals("0") || (!status.ToLowerInvariant().Equals("sent"))){
                    pnSignalResult = null;
                    string message = "";
                    if(c.Length > 2){
                        message = c[2].ToString();
                    }
                    pnStatus = base.CreateErrorResponseFromMessage(string.Format("Signal Failure: {0}", message), requestState, PNStatusCategory.PNBadRequestCategory);
                } else {
                    if(c.Length > 2){
                        pnSignalResult.Timetoken = Utility.ValidateTimetoken(c[2].ToString(), false);
                    } else {
                        pnSignalResult.Timetoken = 0;
                    }
                }
            } else {
                pnSignalResult = null;
                pnStatus = base.CreateErrorResponseFromMessage("Response is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
            }
            Callback(pnSignalResult, pnStatus);

        }

    }
}