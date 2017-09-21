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

        public DeleteMessagesRequestBuilder(PubNubUnity pn): base(pn){
            Debug.Log ("DeleteRequestBuilder Construct");
        }

        #region IPubNubBuilder implementation

        public void Async(Action<PNDeleteMessagesResult, PNStatus> callback)
        {
            //TODO: Add history channel check
            base.Callback = callback;
            Debug.Log ("DeleteRequestBuilder Async");

            if(string.IsNullOrEmpty(this.HistoryChannel)){
                Debug.Log("DeleteRequestBuilder HistoryChannel is empty");

                return;
            }

            base.Async(callback, PNOperationType.PNDeleteMessagesOperation, PNCurrentRequestType.NonSubscribe, this);
        }

         protected override void RunWebRequest(QueueManager qm){

            RequestState requestState = new RequestState ();
            requestState.RespType = PNOperationType.PNDeleteMessagesOperation;
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
            this.PubNubInstance.PNLog.WriteToLog(string.Format("RunDeleteRequestBuilder {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 

        }
 
        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //[[{"text":"hey"},{"text":"hey"},{"text":"hey"},{"text":"hey"}],15011678612673119,15011678623670911]
            PNDeleteMessagesResult pnDeleteMessagesResult = new PNDeleteMessagesResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if (dictionary!=null && dictionary.ContainsKey("error") && dictionary["error"].Equals(true)){
                pnDeleteMessagesResult = null;
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
                    if(statusCode.Equals("0")){
                        pnStatus.Error = true;
                    } else {
                        pnStatus.Error = false;
                        pnDeleteMessagesResult.Message = status;
                    }
                } else {
                    pnStatus.Error = true;
                }
            } else {
                pnDeleteMessagesResult = null;
                pnStatus.Error = true;
            }
            Callback(pnDeleteMessagesResult, pnStatus);

        }

        #endregion
    }
}

