using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class PublishFileMessageRequestBuilder: PubNubNonSubBuilder<PublishFileMessageRequestBuilder, PNPublishResult>, IPubNubNonSubscribeBuilder<PublishFileMessageRequestBuilder, PNPublishResult>
    {        
        private string PublishFileMessage { get; set;}
        private string PublishFileMessageFileName { get; set;}
        private string PublishFileMessageChannel { get; set;}
        private bool ShouldStoreInHistory = true;
        private bool UsePostMethod;
        private Dictionary<string, string> Metadata { get; set;}
        private bool ReplicateMessage = true;
        private int PublishFileMessageTTL { get; set;}
        private string PublishFileID { get; set;}

        private bool publishFileMessageAsIs;
        
        private readonly uint publishFileMessageCounter;
        public PublishFileMessageRequestBuilder(PubNubUnity pn, uint counter): base(pn, PNOperationType.PNPublishFileMessageOperation){
            this.publishFileMessageCounter = counter;
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
            if(string.IsNullOrEmpty(this.PublishFileMessageChannel)){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("Channel is empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);
                return;
            }
            if(this.PublishFileID == null){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("FileID is empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);
                return;
            }
            if(this.PublishFileMessage == null){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("MessageText is empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);
                return;
            }
            if(this.PublishFileMessageFileName == null){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("FileName is empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);
                return;
            }
                        
            base.Async(this);
        }
        #endregion

        public PublishFileMessageRequestBuilder PublishFileMessageAsIs(bool publishFileMessageMessageAsIs){
            this.publishFileMessageAsIs = publishFileMessageMessageAsIs;
            return this;
        }

        public PublishFileMessageRequestBuilder Channel(string channelName){
            PublishFileMessageChannel = channelName;
            return this;
        }

        public PublishFileMessageRequestBuilder ShouldStore(bool shouldStoreInHistory){
            ShouldStoreInHistory = shouldStoreInHistory;
            return this;
        }

        public PublishFileMessageRequestBuilder UsePost(bool usePostRequest){
            UsePostMethod = usePostRequest;
            return this;
        }

        public PublishFileMessageRequestBuilder Meta(Dictionary<string, string> metadata){
            Metadata = metadata;
            return this;
        }

        public PublishFileMessageRequestBuilder Replicate(bool replicateMessage){
            ReplicateMessage = replicateMessage;
            return this;
        }

        public PublishFileMessageRequestBuilder TTL(int publishFileMessageTTL){
            PublishFileMessageTTL = publishFileMessageTTL;
            return this;
        }

        public PublishFileMessageRequestBuilder FileID(string fileID){
            PublishFileID = fileID;
            return this;
        }

        public PublishFileMessageRequestBuilder MessageText(string message){
            PublishFileMessage = message;
            return this;
        }

        public PublishFileMessageRequestBuilder FileName(string fileName){
            PublishFileMessageFileName = fileName;
            return this;
        }        

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            var publishFileMessage = new { 
                message = new {
                    text = PublishFileMessage,
                },
                file = new {
                    name = PublishFileMessageFileName,
                    id = PublishFileID,
                },
            };

            string jsonMessage = (publishFileMessageAsIs) ? publishFileMessage.ToString () : Helpers.JsonEncodePublishMsg (publishFileMessage, this.PubNubInstance.PNConfig.CipherKey, this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            string jsonMetadata = string.Empty;
            if (this.Metadata!=null) {
                jsonMetadata = Helpers.JsonEncodePublishMsg (this.Metadata, "", this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            }

            if(UsePostMethod){
                requestState.httpMethod = HTTPMethod.Post;
                requestState.POSTData = jsonMessage;
            } 
            Uri request = BuildRequests.BuildPublishFileMessageRequest(
                    this.PublishFileMessageChannel,
                    jsonMessage,
                    this.ShouldStoreInHistory,
                    jsonMetadata,
                    publishFileMessageCounter,
                    this.PublishFileMessageTTL,
                    UsePostMethod,
                    this.ReplicateMessage,
                    this.PubNubInstance,
                    this.QueryParams
                );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            object[] c = deSerializedResult as object[];
            PNPublishResult pnPublishFileMessageResult = new PNPublishResult();
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
                    pnPublishFileMessageResult = null;
                    string message = "";
                    if(c.Length > 2){
                        message = c[2].ToString();
                    }
                    pnStatus = base.CreateErrorResponseFromMessage(string.Format("PublishFileMessage Failure: {0}", message), requestState, PNStatusCategory.PNBadRequestCategory);
                } else {
                    if(c.Length > 2){
                        pnPublishFileMessageResult.Timetoken = Utility.ValidateTimetoken(c[2].ToString(), false);
                    } else {
                        pnPublishFileMessageResult.Timetoken = 0;
                    }
                }
            } else {
                pnPublishFileMessageResult = null;
                pnStatus = base.CreateErrorResponseFromMessage("Response is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
            }
            Callback(pnPublishFileMessageResult, pnStatus);

        }

    }
}