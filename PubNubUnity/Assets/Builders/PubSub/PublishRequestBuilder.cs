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
        private bool UsePostMethod = false;
        private Dictionary<string, string> Metadata { get; set;}
        private bool ReplicateMessage = true;
        private int PublishTtl { get; set;}

        private bool publishAsIs = false;
        
        private uint publishCounter;
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

        public PublishRequestBuilder Message(object message){
            PublishMessage = message;
            return this;
        }

        public PublishRequestBuilder PublishAsIs(bool publishAsIs){
            this.publishAsIs = publishAsIs;
            return this;
        }

        public PublishRequestBuilder Channel(string channel){
            PublishChannel = channel;
            return this;
        }

        public PublishRequestBuilder ShouldStore(bool shouldStore){
            ShouldStoreInHistory = shouldStore;
            return this;
        }

        public PublishRequestBuilder UsePost(bool usePost){
            UsePostMethod = usePost;
            return this;
        }

        public PublishRequestBuilder Meta(Dictionary<string, string> meta){
            Metadata = meta;
            return this;
        }

        public PublishRequestBuilder Replicate(bool replicate){
            ReplicateMessage = replicate;
            return this;
        }

        public PublishRequestBuilder Ttl(int ttl){
            PublishTtl = ttl;
            return this;
        }

        protected override void RunWebRequest(QueueManager qm){
            //TODO USe POST
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            //TODO publishAsIs with cipher
            string jsonMessage = (publishAsIs) ? PublishMessage.ToString () : Helpers.JsonEncodePublishMsg (PublishMessage, this.PubNubInstance.PNConfig.CipherKey, this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            string jsonMetadata = string.Empty;
            if (this.Metadata!=null) {
                jsonMetadata = Helpers.JsonEncodePublishMsg (this.Metadata, "", this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            }

            //Uri request;
            if(UsePostMethod){
                requestState.httpMethod = HTTPMethod.Post;
                //string postData = ;
                /*request = BuildRequests.BuildPublishRequestUsingPOST(
                    this.PublishChannel,
                    jsonMessage,
                    this.ShouldStoreInHistory,
                    jsonMetadata,
                    publishCounter,
                    this.PublishTtl,
                    UsePostMethod,
                    ref this.PubNubInstance,
                    out postData
                );*/
                //requestState.POSTData = string.Format("message={0}", jsonMessage);
                requestState.POSTData = jsonMessage;
            } else {

                /* Uri request = BuildRequests.BuildPublishRequest(
                    this.PublishChannel,
                    jsonMessage,
                    this.ShouldStoreInHistory,
                    jsonMetadata,
                    publishCounter,
                    this.PublishTtl,
                    this.PubNubInstance.PNConfig.UUID,
                    this.PubNubInstance.PNConfig.Secure,
                    this.PubNubInstance.PNConfig.Origin,
                    this.PubNubInstance.PNConfig.AuthKey,
                    this.PubNubInstance.PNConfig.PublishKey,
                    this.PubNubInstance.PNConfig.SubscribeKey,
                    this.PubNubInstance.PNConfig.CipherKey,
                    this.PubNubInstance.PNConfig.SecretKey,
                    this.PubNubInstance.Version
                ); 

                request = BuildRequests.BuildPublishRequest(
                    this.PublishChannel,
                    jsonMessage,
                    this.ShouldStoreInHistory,
                    jsonMetadata,
                    publishCounter,
                    this.PublishTtl,
                    UsePostMethod,
                    ref this.PubNubInstance
                );*/
            }
            Uri request = BuildRequests.BuildPublishRequest(
                    this.PublishChannel,
                    jsonMessage,
                    this.ShouldStoreInHistory,
                    jsonMetadata,
                    publishCounter,
                    this.PublishTtl,
                    UsePostMethod,
                    ref this.PubNubInstance
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
                if(statusCode.Equals("0") || (!status.ToLower().Equals("sent"))){
                    pnPublishResult = null;
                    string message = "";
                    if(c.Length > 2){
                        message = c[2].ToString();
                    }
                    pnStatus = base.CreateErrorResponseFromMessage(string.Format("Publish Failure: {0}", message), requestState, PNStatusCategory.PNBadRequestCategory);
                } else {
                    if(c.Length > 2){
                        pnPublishResult.Timetoken = Utility.ValidateTimetoken(c[2].ToString(), false);
                    }
                }
            } else {
                pnPublishResult = null;
                pnStatus = base.CreateErrorResponseFromMessage("Response is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
            }
            Callback(pnPublishResult, pnStatus);

        }

        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }
        
    }
}