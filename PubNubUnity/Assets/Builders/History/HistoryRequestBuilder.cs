using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PubNubAPI
{
    public class HistoryRequestBuilder: PubNubNonSubBuilder<HistoryRequestBuilder, PNHistoryResult>, IPubNubNonSubscribeBuilder<HistoryRequestBuilder, PNHistoryResult>
    {
        private string HistoryChannel { get; set;}
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
        
        private bool ReverseHistory;
        private bool IncludeTimetokenInHistory;
        public HistoryRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNHistoryOperation){
        }

        public HistoryRequestBuilder IncludeTimetoken(bool include){
            IncludeTimetokenInHistory = include;
            return this;
        }

        public HistoryRequestBuilder Reverse(bool reverseHistory){
            ReverseHistory = reverseHistory;
            return this;
        }

        public HistoryRequestBuilder Start(long startTime){
            StartTime = startTime;
            return this;
        }

        public HistoryRequestBuilder End(long endTime){
            EndTime = endTime;
            return this;
        }

        public HistoryRequestBuilder Channel(string channelName){
            HistoryChannel = channelName;
            ChannelsToUse = new List<string>{HistoryChannel};
            return this;
        }

        public HistoryRequestBuilder Count(ushort historyCount){
            HistoryCount = historyCount;
            return this;
        }

        #region IPubNubBuilder implementation

        public void Async(Action<PNHistoryResult, PNStatus> callback)
        {
            base.Callback = callback;

            if(string.IsNullOrEmpty(this.HistoryChannel)){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("HistoryChannel is empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);
                return;
            }

            base.Async(this);
        }

         protected override void RunWebRequest(QueueManager qm){

            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format ("HistoryRequestBuilder: \nChannel {0} \nStartTime: {1} \nthis.EndTime:{2} \nthis.HistoryCount:{3} \nthis.ReverseHistory:{4}", string.Join(",", this.ChannelsToUse.ToArray()), this.StartTime, this.EndTime, this.HistoryCount, this.ReverseHistory), PNLoggingMethod.LevelInfo);
            #endif
            //TODO: start=0&end=0

            Uri request = BuildRequests.BuildHistoryRequest(
                this.HistoryChannel,
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
            //[[{"text":"hey"},{"text":"hey"},{"text":"hey"},{"text":"hey"}],15011678612673119,15011678623670911]
            PNHistoryResult pnHistoryResult = new PNHistoryResult();
            pnHistoryResult.Messages = new List<PNHistoryItemResult>();

            PNStatus pnStatus = new PNStatus();
            try{
                List<object> result = ((IEnumerable)deSerializedResult).Cast<object> ().ToList ();
                if(result != null){

                    var historyResponseArray = (from item in result
                            select item as object).ToArray ();
                    #if (ENABLE_PUBNUB_LOGGING)
                    foreach(var h in historyResponseArray){
                        this.PubNubInstance.PNLog.WriteToLog(string.Format ("HistoryRequestBuilder: {0}", h.ToString()), PNLoggingMethod.LevelInfo);
                    }
                    #endif                            

                    if(historyResponseArray.Length >= 1){
                        //TODO add checks
                        if(!ExtractMessages(historyResponseArray, ref pnHistoryResult)){
                            pnHistoryResult = null;
                            pnStatus = base.CreateErrorResponseFromMessage("ExtractMessages failed", requestState, PNStatusCategory.PNMalformedResponseCategory);
                        } 
                    }

                    if(historyResponseArray.Length > 1){
                        pnHistoryResult.StartTimetoken = Utility.ValidateTimetoken(historyResponseArray[1].ToString(), true);
                    }

                    if(historyResponseArray.Length > 2){
                        pnHistoryResult.EndTimetoken = Utility.ValidateTimetoken(historyResponseArray[2].ToString(), true);
                    }
                }
            } catch (Exception ex) {
                pnHistoryResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnHistoryResult, pnStatus);

        }

         private void ExtractMessageWithTimetokens( object element, string cipherKey, out PNHistoryItemResult pnHistoryItemResult){
            //[[{"message":{"text":"hey"},"timetoken":14985452911089049}],14985452911089049,14985452911089049] 
            //[[{"message":{"text":"hey"},"timetoken":14986549102032676},{"message":"E8VOcbfrYqLyHMtoVGv9UQ==","timetoken":14986619049105442},{"message":"E8VOcbfrYqLyHMtoVGv9UQ==","timetoken":14986619291068634}],14986549102032676,14986619291068634]
            pnHistoryItemResult = new PNHistoryItemResult();
            Dictionary<string, object> historyMessage = element as Dictionary<string, object>;
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format ("ExtractMessageWithTimetokens: historyMessage {0}", historyMessage), PNLoggingMethod.LevelInfo);
            #endif                            
            object v;
            historyMessage.TryGetValue("message", out v);
            if(!string.IsNullOrEmpty(cipherKey) && (cipherKey.Length > 0)){
                //TODO: handle exception
                pnHistoryItemResult.Entry = Helpers.DecodeMessage(cipherKey, v, OperationType, this.PubNubInstance);
            } else {
                pnHistoryItemResult.Entry = v;
            }
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format ("ExtractMessageWithTimetokens: v {0}", pnHistoryItemResult.Entry), PNLoggingMethod.LevelInfo);
            #endif                            

            object t;
            historyMessage.TryGetValue("timetoken", out t);
            pnHistoryItemResult.Timetoken = Utility.ValidateTimetoken(t.ToString(), false);
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format ("ExtractMessageWithTimetokens: t {0}", t), PNLoggingMethod.LevelInfo);
            #endif                            
            
        }

        private void ExtractMessage( object element, string cipherKey, out PNHistoryItemResult pnHistoryItemResult){
            //[[{"text":"hey"}],14985452911089049,14985452911089049]
            //[[{"text":"hey"},"E8VOcbfrYqLyHMtoVGv9UQ==","E8VOcbfrYqLyHMtoVGv9UQ=="],14986549102032676,14986619291068634]
            pnHistoryItemResult = new PNHistoryItemResult();
            if(!string.IsNullOrEmpty(cipherKey) && (cipherKey.Length > 0)){
                //TODO: handle exception
                pnHistoryItemResult.Entry = Helpers.DecodeMessage(cipherKey, element, OperationType, this.PubNubInstance);
            } else {
                pnHistoryItemResult.Entry = element;
            }
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format ("ExtractMessage: v {0}", pnHistoryItemResult.Entry), PNLoggingMethod.LevelInfo);
            #endif                            

        }

        private bool ExtractMessages(object[] historyResponseArray, ref PNHistoryResult pnHistoryResult){
            IEnumerable enumerable = historyResponseArray [0] as IEnumerable;
            if (enumerable != null) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog(string.Format ("enumerable: {0}", enumerable), PNLoggingMethod.LevelInfo);
                #endif                            
                
                foreach (object elem in enumerable) {
                    var element = elem;
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog(string.Format ("element: {0}", element), PNLoggingMethod.LevelInfo);
                    #endif                     
                    PNHistoryItemResult pnHistoryItemResult;

                    if(this.IncludeTimetokenInHistory){
                        ExtractMessageWithTimetokens(element, this.PubNubInstance.PNConfig.CipherKey, out pnHistoryItemResult);
                    } else {
                        ExtractMessage(element, this.PubNubInstance.PNConfig.CipherKey, out pnHistoryItemResult);
                    }
                    pnHistoryResult.Messages.Add(pnHistoryItemResult);
                }
                return true;
            }
            return false;
        }
        #endregion
    }
}

