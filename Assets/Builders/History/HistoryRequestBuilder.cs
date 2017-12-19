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
        
        private bool ReverseHistory = false;
        private bool IncludeTimetokenInHistory = false;
        public HistoryRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNHistoryOperation){
        }

        public HistoryRequestBuilder IncludeTimetoken(bool includeTimetoken){
            IncludeTimetokenInHistory = includeTimetoken;
            return this;
        }

        public HistoryRequestBuilder Reverse(bool reverse){
            ReverseHistory = reverse;
            return this;
        }

        public HistoryRequestBuilder Start(long start){
            StartTime = start;
            return this;
        }

        public HistoryRequestBuilder End(long end){
            EndTime = end;
            return this;
        }

        public HistoryRequestBuilder Channel(string channel){
            HistoryChannel = channel;
            ChannelsToUse = new List<string>(){HistoryChannel};
            return this;
        }

        public HistoryRequestBuilder Count(ushort count){
            HistoryCount = count;
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

            /* Uri request = BuildRequests.BuildHistoryRequest(
                this.HistoryChannel,
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
            ); */
            Uri request = BuildRequests.BuildHistoryRequest(
                this.HistoryChannel,
                this.StartTime,
                this.EndTime,
                this.HistoryCount,
                this.ReverseHistory,
                this.IncludeTimetokenInHistory,
                ref this.PubNubInstance
            );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }
 
        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }

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
                    foreach(var h in historyResponseArray){
                        Debug.Log(h.ToString());
                    }

                    if(historyResponseArray.Length >= 1){
                        //TODO add checks
                        if(!ExtractMessages(historyResponseArray, ref pnHistoryResult)){
                            pnHistoryResult = null;
                            pnStatus = base.CreateErrorResponseFromMessage("ExtractMessages failed", requestState, PNStatusCategory.PNMalformedResponseCategory);
                        } 
                    }

                    if(historyResponseArray.Length > 1){
                        pnHistoryResult.StartTimetoken = Utility.ValidateTimetoken(historyResponseArray[1].ToString(), true);
                        Debug.Log(pnHistoryResult.StartTimetoken);
                    }

                    if(historyResponseArray.Length > 2){
                        pnHistoryResult.EndTimetoken = Utility.ValidateTimetoken(historyResponseArray[2].ToString(), true);
                        Debug.Log(pnHistoryResult.EndTimetoken);
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
            Debug.Log("historyMessage" + historyMessage.ToString());
            object v;
            historyMessage.TryGetValue("message", out v);
            if(!string.IsNullOrEmpty(cipherKey) && (cipherKey.Length > 0)){
                //TODO: handle exception
                pnHistoryItemResult.Entry = Helpers.DecodeMessage(cipherKey, v, OperationType, ref this.PubNubInstance);
            } else {
                pnHistoryItemResult.Entry = v;
            }
            Debug.Log(" v "+pnHistoryItemResult.Entry);

            object t;
            historyMessage.TryGetValue("timetoken", out t);
            pnHistoryItemResult.Timetoken = Utility.ValidateTimetoken(t.ToString(), false);
            Debug.Log(" t " + t.ToString());
            
        }

        private void ExtractMessage( object element, string cipherKey, out PNHistoryItemResult pnHistoryItemResult){
            //[[{"text":"hey"}],14985452911089049,14985452911089049]
            //[[{"text":"hey"},"E8VOcbfrYqLyHMtoVGv9UQ==","E8VOcbfrYqLyHMtoVGv9UQ=="],14986549102032676,14986619291068634]
            pnHistoryItemResult = new PNHistoryItemResult();
            if(!string.IsNullOrEmpty(cipherKey) && (cipherKey.Length > 0)){
                //TODO: handle exception
                pnHistoryItemResult.Entry = Helpers.DecodeMessage(cipherKey, element, OperationType, ref this.PubNubInstance);
            } else {
                pnHistoryItemResult.Entry = element;
            }
            Debug.Log(" v "+pnHistoryItemResult.Entry);
        }

        private bool ExtractMessages(object[] historyResponseArray, ref PNHistoryResult pnHistoryResult){
            IEnumerable enumerable = historyResponseArray [0] as IEnumerable;
            if (enumerable != null) {
                Debug.Log("enumerable" + enumerable.ToString());
                foreach (object elem in enumerable) {
                    var element = elem;
                    Debug.Log("element:" + element.ToString());
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

