using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PubNubAPI
{
    public class DeleteMessagesRequestBuilder: PubNubNonSubBuilder<DeleteMessagesRequestBuilder, PNDeleteMessagesResult>, IPubNubNonSubscribeBuilder<DeleteMessagesRequestBuilder, PNDeleteMessagesResult>
    {
        private string HistoryChannel { get; set;}
        private long StartTime = -1;
        private long EndTime = -1;
        
        public void Start(long start){
            this.StartTime = start;
        }

        public void End(long end){
            this.EndTime = end;
        }

        public void Channel(string channel){
            this.HistoryChannel = channel;
        }

        public DeleteMessagesRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNDeleteMessagesOperation){
        }

        #region IPubNubBuilder implementation

        public void Async(Action<PNDeleteMessagesResult, PNStatus> callback)
        {
            base.Callback = callback;
            if(string.IsNullOrEmpty(this.HistoryChannel)){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("DeleteHistory Channel is empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);                

                return;
            }

            base.Async(this);
        }

         protected override void RunWebRequest(QueueManager qm){

            RequestState requestState = new RequestState ();
            requestState.OperationType = base.OperationType;
            requestState.httpMethod = HTTPMethod.Delete;

            Debug.Log ("DeleteRequestBuilder Channel: " + this.HistoryChannel);
            Debug.Log ("DeleteRequestBuilder StartTime: " + this.StartTime);
            Debug.Log ("DeleteRequestBuilder EndTime: " + this.EndTime);

            Uri request = BuildRequests.BuildDeleteMessagesRequest(
                this.HistoryChannel,
                this.StartTime,
                this.EndTime,
                ref this.PubNubInstance
            );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 

        }
 
        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //[[{"text":"hey"},{"text":"hey"},{"text":"hey"},{"text":"hey"}],15011678612673119,15011678623670911]
            PNDeleteMessagesResult pnDeleteMessagesResult = new PNDeleteMessagesResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if(dictionary != null) {
                string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
                if(Utility.CheckDictionaryForError(dictionary, "error")){
                    pnDeleteMessagesResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
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
                        if(statusCode.Equals("0")){
                            pnDeleteMessagesResult = null;
                            pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);                            
                        } else {
                            pnStatus.Error = false;
                            pnDeleteMessagesResult.Message = status;
                        }
                    } else {
                        pnDeleteMessagesResult = null;
                        pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);                            
                    }
                }
            } else {
                pnDeleteMessagesResult = null;
                pnStatus = base.CreateErrorResponseFromMessage("Response dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);

            }
            
            Callback(pnDeleteMessagesResult, pnStatus);

        }

        #endregion
    }
}

