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
        
        public void Start(long startTime){
            this.StartTime = startTime;
        }

        public void End(long endTime){
            this.EndTime = endTime;
        }

        public void Channel(string channelName){
            this.HistoryChannel = channelName;
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

            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format ("DeleteRequestBuilder: \nChannel {0} \nStartTime: {1} \nthis.EndTime:{2}", this.HistoryChannel, this.StartTime, this.EndTime), PNLoggingMethod.LevelInfo);
            #endif

            Uri request = BuildRequests.BuildDeleteMessagesRequest(
                this.HistoryChannel,
                this.StartTime,
                this.EndTime,
                this.PubNubInstance
            );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 

        }
 
        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //Returned Code `{"status": 200, "error": false, "error_message": ""} `
            //Returned Code `{"status": 403, "error": true, "error_message": "Use of the history API requires the Storage & Playback which is not enabled for this subscribe key. Login to your PubNub Dashboard Account and enable Storage & Playback. Contact support@pubnub.com if you require further assistance.", "channels": {}}`
            PNDeleteMessagesResult pnDeleteMessagesResult = new PNDeleteMessagesResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if(dictionary != null) {
                string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
                if(Utility.CheckDictionaryForError(dictionary, "error")){
                    pnDeleteMessagesResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
                } else {
                    pnDeleteMessagesResult.Message = message;
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

