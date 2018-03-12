using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class FetchMessagesRequestBuilder: PubNubNonSubBuilder<FetchMessagesRequestBuilder, PNFetchMessagesResult>, IPubNubNonSubscribeBuilder<FetchMessagesRequestBuilder, PNFetchMessagesResult>
    {      
        public FetchMessagesRequestBuilder(PubNubUnity pn):base(pn, PNOperationType.PNFetchMessagesOperation){

        }

        private long StartTime = -1;
        private long EndTime = -1;
        
        private const ushort MaxCount = 25;
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
        
        private bool ReverseHistory;
        private bool IncludeTimetokenInHistory;
        public FetchMessagesRequestBuilder IncludeTimetoken(bool include){
            IncludeTimetokenInHistory = include;
            return this;
        }

        public FetchMessagesRequestBuilder Reverse(bool reverseHistory){
            ReverseHistory = reverseHistory;
            return this;
        }

        public FetchMessagesRequestBuilder Start(long startTime){
            StartTime = startTime;
            return this;
        }

        public FetchMessagesRequestBuilder End(long endTime){
            EndTime = endTime;
            return this;
        }

        public FetchMessagesRequestBuilder Channel(List<string> channelNames){
            ChannelsToUse = channelNames;
            return this;
        }

        public FetchMessagesRequestBuilder Count(ushort historyCount){
            HistoryCount = historyCount;
            return this;
        }
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNFetchMessagesResult, PNStatus> callback)
        {
            this.Callback = callback;
            if((this.ChannelsToUse == null) || ((this.ChannelsToUse != null) && (this.ChannelsToUse.Count <= 0))){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("HistoryChannel is null or empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);

                return;
            }

            base.Async(this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = base.OperationType;

            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format ("FetchMessagesRequestBuilder: \nChannel {0} \nStartTime: {1} \nthis.EndTime:{2} \nthis.HistoryCount:{3} \nthis.ReverseHistory:{4}", string.Join(",", this.ChannelsToUse.ToArray()), this.StartTime, this.EndTime, this.HistoryCount, this.ReverseHistory), PNLoggingMethod.LevelInfo);
            #endif
            
            //TODO: start=0&end=0

            Uri request = BuildRequests.BuildFetchRequest(
                ChannelsToUse.ToArray(),
                this.StartTime,
                this.EndTime,
                this.HistoryCount,
                this.ReverseHistory,
                this.IncludeTimetokenInHistory,
                this.PubNubInstance
            );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 

        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //Returned JSON: `{"status": 200, "error": false, "error_message": "", "channels": {"channel2":[{"message":{"text":"hey"},"timetoken":"15011678669001834"}],"channel1":[{"message":{"text":"hey"},"timetoken":"15011678623670911"}]}}`
            PNFetchMessagesResult pnFetchMessagesResult = new PNFetchMessagesResult();
            PNStatus pnStatus = new PNStatus();
            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary != null) {
                    string message = Utility.ReadMessageFromResponseDictionary(dictionary, "error_message");
                    if(Utility.CheckDictionaryForError(dictionary, "error")){
                        pnFetchMessagesResult = null;
                        pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
                    } else {
                        object objChannelsDict;
                        dictionary.TryGetValue("channels", out objChannelsDict);
                        if(objChannelsDict != null){
                            Dictionary<string, List<PNMessageResult>> channelsResult;
                            if(!CreateFetchMessagesResult(objChannelsDict, out channelsResult)){
                                pnFetchMessagesResult = null;
                                pnStatus = base.CreateErrorResponseFromMessage("channelsResult dictionary is null", requestState, PNStatusCategory.PNUnknownCategory);
                            } else {
                                pnFetchMessagesResult.Channels = channelsResult;
                            }
                        }
                    }
                } else {
                    pnFetchMessagesResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage("Response dictionary is null", requestState, PNStatusCategory.PNUnknownCategory);
                }
            } catch (Exception ex) {
                pnFetchMessagesResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnFetchMessagesResult, pnStatus);
        }
        
        protected bool CreateFetchMessagesResult(object objChannelsDict, out Dictionary<string, List<PNMessageResult>> channelsResult ){
            Dictionary<string, object> channelsDict = objChannelsDict as Dictionary<string, object>;
            channelsResult = new Dictionary<string, List<PNMessageResult>>();

            if(channelsDict!=null){
                foreach(KeyValuePair<string, object> kvpair in channelsDict){
                    string channelName = kvpair.Key;
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog(string.Format ("CreateFetchMessagesResult: \nChannel {0}", channelName), PNLoggingMethod.LevelInfo);
                    #endif
                    
                    if(channelsResult.ContainsKey(channelName)){
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PubNubInstance.PNLog.WriteToLog(string.Format ("CreateFetchMessagesResult: Channel name {0} exists in dict, continuing.", channelName), PNLoggingMethod.LevelInfo);
                        #endif
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
                                #if (ENABLE_PUBNUB_LOGGING)
                                this.PubNubInstance.PNLog.WriteToLog(string.Format ("CreateFetchMessagesResult: timetoken {0}.", timetoken.ToString()), PNLoggingMethod.LevelInfo);
                                #endif
                            }
                            if(!string.IsNullOrEmpty(this.PubNubInstance.PNConfig.CipherKey) && (this.PubNubInstance.PNConfig.CipherKey.Length > 0)){
                                //TODO: handle exception
                                objPayload = Helpers.DecodeMessage(this.PubNubInstance.PNConfig.CipherKey, objPayload, OperationType, this.PubNubInstance);
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
                return true;
            }
            return false; 
        }
    }
}

