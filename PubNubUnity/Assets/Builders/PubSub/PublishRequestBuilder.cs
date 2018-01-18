using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class PublishRequestBuilder: PubNubNonSubBuilder<PublishRequestBuilder, PNPublishResult>, IPubNubNonSubscribeBuilder<PublishRequestBuilder, PNPublishResult>
    {        
        private object PublishMessage { get; set;}
        private string PublishChannel { get; set;}
        private bool ShouldStoreInHistory = true;
        private bool UsePostMethod;
        private Dictionary<string, string> Metadata { get; set;}
        private bool ReplicateMessage = true;
        private int PublishTtl { get; set;}

        private bool publishAsIs;
        
        private readonly uint publishCounter;
        public PublishRequestBuilder(PubNubUnity pn, uint counter): base(pn, PNOperationType.PNPublishOperation){
            this.publishCounter = counter;
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNPublishResult, PNStatus> callback)
        {
            this.Callback = callback;
            if(string.IsNullOrEmpty(this.PubNubInstance.PNConfig.PublishKey)){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("Publish Key is empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);
                return;
            }
            if(string.IsNullOrEmpty(this.PublishChannel)){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("Channel is empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);
                return;
            }
            if(this.PublishMessage == null){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("Message is empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);
                return;
            }
            
            base.Async(this);
        }
        #endregion

        public PublishRequestBuilder Message(object messageToPublish){
            PublishMessage = messageToPublish;
            return this;
        }

        public PublishRequestBuilder PublishAsIs(bool publishMessageAsIs){
            this.publishAsIs = publishMessageAsIs;
            return this;
        }

        public PublishRequestBuilder Channel(string channelName){
            PublishChannel = channelName;
            return this;
        }

        public PublishRequestBuilder ShouldStore(bool shouldStoreInHistory){
            ShouldStoreInHistory = shouldStoreInHistory;
            return this;
        }

        public PublishRequestBuilder UsePost(bool usePostRequest){
            UsePostMethod = usePostRequest;
            return this;
        }

        public PublishRequestBuilder Meta(Dictionary<string, string> metadata){
            Metadata = metadata;
            return this;
        }

        public PublishRequestBuilder Replicate(bool replicateMessage){
            ReplicateMessage = replicateMessage;
            return this;
        }

        public PublishRequestBuilder Ttl(int publishTTL){
            PublishTtl = publishTTL;
            return this;
        }

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            string jsonMessage = (publishAsIs) ? PublishMessage.ToString () : Helpers.JsonEncodePublishMsg (PublishMessage, this.PubNubInstance.PNConfig.CipherKey, this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            string jsonMetadata = string.Empty;
            if (this.Metadata!=null) {
                jsonMetadata = Helpers.JsonEncodePublishMsg (this.Metadata, "", this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            }

            if(UsePostMethod){
                requestState.httpMethod = HTTPMethod.Post;
                requestState.POSTData = jsonMessage;
            } 
            Uri request = BuildRequests.BuildPublishRequest(
                    this.PublishChannel,
                    jsonMessage,
                    this.ShouldStoreInHistory,
                    jsonMetadata,
                    publishCounter,
                    this.PublishTtl,
                    UsePostMethod,
                    this.PubNubInstance
                );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            object[] c = deSerializedResult as object[];
            PNPublishResult pnPublishResult = new PNPublishResult();
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
                    pnPublishResult = null;
                    string message = "";
                    if(c.Length > 2){
                        message = c[2].ToString();
                    }
                    pnStatus = base.CreateErrorResponseFromMessage(string.Format("Publish Failure: {0}", message), requestState, PNStatusCategory.PNBadRequestCategory);
                } else {
                    if(c.Length > 2){
                        pnPublishResult.Timetoken = Utility.ValidateTimetoken(c[2].ToString(), false);
                    } else {
                        pnPublishResult.Timetoken = 0;
                    }
                }
            } else {
                pnPublishResult = null;
                pnStatus = base.CreateErrorResponseFromMessage("Response is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
            }
            Callback(pnPublishResult, pnStatus);

        }

    }
}