using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class HistoryBuilder: PubNubNonSubBuilder<HistoryBuilder>, IPubNubNonSubscribeBuilder<HistoryBuilder, PNHistoryResult>
    {
        public string HistoryChannel { get; private set;}
        public long StartTime { get; private set;}
        public long EndTime { get; private set;}
        private ushort count = 100;
        private const ushort MaxCount = 100;
        public ushort HistoryCount { 
            get {
                return count;
            } 
            private set {
                if(value > 100 || value <= 0){ 
                    count = 100; 
                } else {
                    count = value;
                }
            }
        }
        private Action<PNHistoryResult, PNStatus> Callback;
        public bool ReverseHistory { get; private set;}
        public bool IncludeTimetokenInHistory { get; private set;}
        public HistoryBuilder(PubNubUnity pn): base(pn){
            Debug.Log ("HistoryBuilder Construct");
        }

        public HistoryBuilder IncludeTimetoken(bool includeTimetoken){
            IncludeTimetokenInHistory = includeTimetoken;
            return this;
        }

        public HistoryBuilder Reverse(bool reverse){
            ReverseHistory = reverse;
            return this;
        }

        public HistoryBuilder Start(long start){
            StartTime = start;
            return this;
        }

        public HistoryBuilder End(long end){
            EndTime = end;
            return this;
        }

        public HistoryBuilder Channel(string channel){
            HistoryChannel = channel;
            return this;
        }

        public HistoryBuilder Count(ushort count){
            HistoryCount = count;
            return this;
        }

        #region IPubNubBuilder implementation

        public void Async(Action<PNHistoryResult, PNStatus> callback)
        {
            //TODO: Add history channel check
            Callback = callback;
            Debug.Log ("PNHistoryBuilder Async");
            base.Async<PNHistoryResult>(callback, PNOperationType.PNHistoryOperation, CurrentRequestType.NonSubscribe, this);
        }

         protected override void RunWebRequest(QueueManager qm){

            RequestState<PNHistoryResult> requestState = new RequestState<PNHistoryResult> ();
            requestState.RespType = PNOperationType.PNHistoryOperation;

            Debug.Log ("HistoryBuilder Channel: " + this.HistoryChannel);
            Debug.Log ("HistoryBuilder Channel: " + this.StartTime);
            Debug.Log ("HistoryBuilder Channel: " + this.EndTime);
            Debug.Log ("HistoryBuilder Channel: " + this.HistoryCount);


            Uri request = BuildRequests.BuildHistoryRequest(
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
            );
            this.PubNubInstance.PNLog.WriteToLog(string.Format("RunHistoryRequest {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest<PNHistoryResult>(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 

        }

        protected override void CreatePubNubResponse(object deSerializedResult){
            //[[{"message":{"text":"hey"},"timetoken":14985452911089049}],14985452911089049,14985452911089049] 
            //[[{"text":"hey"}],14985452911089049,14985452911089049]
            try{
                PNHistoryResult pnHistoryResult = new PNHistoryResult();
                var myObjectArray = deSerializedResult as object[];
                if(myObjectArray != null){
                    foreach (var it in myObjectArray){
                        Debug.Log(it);
                    }
                    Dictionary<string, object>[] historyMessages = deSerializedResult as Dictionary<string, object>[];
                    foreach(var historyMessage in historyMessages){
                        if(this.PubNubInstance.PNConfig.CipherKey.Length > 0){
                            PubnubCrypto aes = new PubnubCrypto (this.PubNubInstance.PNConfig.CipherKey);
                            //Decrypt();
                            //aes.Decrypt();
                        } 
                        //non encrypted, 
                        if(Utility.IsDictionary(historyMessage)){
                            //user: dictionary
                            //pn: message -> user, timetoken
                            
                        } else {
                                //user: string, long, array
                            
                        }

                    }

                    if(myObjectArray.Length>=1){
                        pnHistoryResult.StartTimetoken = Utility.ValidateTimetoken(myObjectArray[1].ToString(), true);
                    }

                    if(myObjectArray.Length>=2){
                        pnHistoryResult.EndTimetoken = Utility.ValidateTimetoken(myObjectArray[2].ToString(), true);
                    }
                }
            } catch (Exception ex) {
                throw ex;
            }
            /*result = DecodeDecryptLoop (result, pubnubRequestState.ChannelEntities, cipherKey, jsonPluggableLibrary, errorLevel);
            result.Add (multiChannel);*/
        }
        #endregion
    }
}

