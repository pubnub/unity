using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class FetchMessagesRequestBuilder: PubNubNonSubBuilder<FetchMessagesRequestBuilder, PNFetchMessagesResult>, IPubNubNonSubscribeBuilder<FetchMessagesRequestBuilder, PNFetchMessagesResult>
    {      
        public FetchMessagesRequestBuilder(PubNubUnity pn):base(pn){

        }

        private List<string> HistoryChannel { get; set;}
        private long StartTime = -1;
        private long EndTime = -1;
        
        private const ushort MaxCount = 100;
        private ushort count = MaxCount;
        private ushort HistoryCount { 
            get {
                return count;
            } 
            set {
                if(value > MaxCount || value <= 0){ 
                    count = MaxCount; 
                } else {
                    count = value;
                }
            }
        }
        
        private bool ReverseHistory = false;
        private bool IncludeTimetokenInHistory = false;
        public FetchMessagesRequestBuilder IncludeTimetoken(bool includeTimetoken){
            IncludeTimetokenInHistory = includeTimetoken;
            return this;
        }

        public FetchMessagesRequestBuilder Reverse(bool reverse){
            ReverseHistory = reverse;
            return this;
        }

        public FetchMessagesRequestBuilder Start(long start){
            StartTime = start;
            return this;
        }

        public FetchMessagesRequestBuilder End(long end){
            EndTime = end;
            return this;
        }

        public FetchMessagesRequestBuilder Channel(List<string> channel){
            HistoryChannel = channel;
            return this;
        }

        public FetchMessagesRequestBuilder Count(ushort count){
            HistoryCount = count;
            return this;
        }
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNFetchMessagesResult, PNStatus> callback)
        {
            this.Callback = callback;
            Debug.Log ("FetchMessagesRequestBuilder Async");
            if((this.HistoryChannel == null) || ((this.HistoryChannel != null) && (this.HistoryChannel.Count <= 0))){
                Debug.Log("FetchMessagesRequestBuilder HistoryChannel is null or empty");

                //TODO Send callback
                return;
            }

            base.Async(callback, PNOperationType.PNFetchMessagesOperation, CurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState<PNFetchMessagesResult> requestState = new RequestState<PNFetchMessagesResult> ();
            requestState.RespType = PNOperationType.PNFetchMessagesOperation;
            
            Debug.Log ("FetchMessagesRequestBuilder Channel: " + this.HistoryChannel);
            Debug.Log ("FetchMessagesRequestBuilder Channel: " + this.StartTime);
            Debug.Log ("FetchMessagesRequestBuilder Channel: " + this.EndTime);
            Debug.Log ("FetchMessagesRequestBuilder Channel: " + this.HistoryCount);
            Debug.Log ("FetchMessagesRequestBuilder Channel: " + this.ReverseHistory);

            //TODO: start=0&end=0

            Uri request = BuildRequests.BuildFetchRequest(
                HistoryChannel.ToArray(),
                this.StartTime,
                this.EndTime,
                this.HistoryCount,
                this.ReverseHistory,
                this.IncludeTimetokenInHistory,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            );
            this.PubNubInstance.PNLog.WriteToLog(string.Format("Run PNFetchMessagesResult {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 

        }

        protected override void CreatePubNubResponse(object deSerializedResult){
            //{"status": 200, "error": false, "error_message": "", "channels": {"channel2":[{"message":{"text":"hey"},"timetoken":"15011678669001834"}],"channel1":[{"message":{"text":"hey"},"timetoken":"15011678623670911"}]}}
            PNFetchMessagesResult pnFetchMessagesResult = new PNFetchMessagesResult();
            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                PNStatus pnStatus = new PNStatus();
                if (dictionary!=null && dictionary.ContainsKey("error") && dictionary["error"].Equals(true)){
                    pnFetchMessagesResult = null;
                    pnStatus.Error = true;
                    object objErrorMessage;
                    dictionary.TryGetValue("error_message", out objErrorMessage);
                    //TODO create error data
                } else if(dictionary!=null) {
                    object objChannelsDict;
                    dictionary.TryGetValue("channels", out objChannelsDict);
                    if(objChannelsDict!=null){
                        Dictionary<string, List<PNMessageResult>> channelsResult;
                        pnStatus.Error = CreateFetchMessagesResult(objChannelsDict, out channelsResult);
                        
                        pnFetchMessagesResult.Channels = channelsResult;
                    }
                } else {
                    pnFetchMessagesResult = null;
                    pnStatus.Error = true;
                }
                Callback(pnFetchMessagesResult, pnStatus);
            } catch (Exception ex) {
                Debug.Log(ex.ToString());
                //throw ex;
            }
        }
        
        protected bool CreateFetchMessagesResult(object objChannelsDict, out Dictionary<string, List<PNMessageResult>> channelsResult ){
            Dictionary<string, object> channelsDict = objChannelsDict as Dictionary<string, object>;
            channelsResult = new Dictionary<string, List<PNMessageResult>>();

            if(channelsDict!=null){
                foreach(KeyValuePair<string, object> kvpair in channelsDict){
                    string channelName = kvpair.Key;
                    Debug.Log("channelName:" + channelName);
                    if(channelsResult.ContainsKey(channelName)){
                        Debug.Log("Channel name exists in dict, continuing :"+ channelName);
                        continue;
                    }
                    object[] channelDetails = kvpair.Value as object[];
                    List<PNMessageResult> lstMessageResult = new List<PNMessageResult>();
                    foreach(object messageData in channelDetails){
                        Dictionary<string, object> messageDataDict = messageData as Dictionary<string, object>;
                        if(messageDataDict!=null){
                            
                            object objPayload;
                            messageDataDict.TryGetValue("message", out objPayload);
                            
                            object objTimetoken;
                            messageDataDict.TryGetValue("timetoken", out objTimetoken);
                            long timetoken;
                            if(long.TryParse(objTimetoken.ToString(), out timetoken)){
                                Debug.Log("timetoken:" + timetoken.ToString());
                            }
                            
                            PNMessageResult pnMessageResult = new PNMessageResult(
                                channelName, 
                                channelName,
                                objPayload,
                                timetoken,
                                timetoken,
                                null,
                                ""
                            );
                            lstMessageResult.Add(pnMessageResult);
                        }
                    }
                    channelsResult.Add(channelName, lstMessageResult);
                }
                return false;
            }
            return true; 
        }
    }
}

