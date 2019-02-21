using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class MessageCountsRequestBuilder: PubNubNonSubBuilder<MessageCountsRequestBuilder, PNMessageCountsResult>, IPubNubNonSubscribeBuilder<MessageCountsRequestBuilder, PNMessageCountsResult>
    {      
        public MessageCountsRequestBuilder(PubNubUnity pn):base(pn, PNOperationType.PNMessageCountsOperation){

        }

        private string TimetokenToUse {get; set;}

        private List<string> ChannelTimetokensToUse {get; set;}
        private bool IncludeTimetokenInHistory;

        public MessageCountsRequestBuilder Channels(List<string> channelNames){
            ChannelsToUse = channelNames;
            return this;
        }

        public MessageCountsRequestBuilder ChannelTimetokens(List<string> channelTimetoken){
            ChannelTimetokensToUse = channelTimetoken;
            return this;
        }

        public MessageCountsRequestBuilder Timetoken(string timetoken){
            TimetokenToUse = timetoken;
            return this;
        }

        #region IPubNubBuilder implementation

        public void Async(Action<PNMessageCountsResult, PNStatus> callback)
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
            this.PubNubInstance.PNLog.WriteToLog(string.Format ("MessageCountsRequestBuilder: \nChannel {0} \nChannelTimetokens: {1} \nTimetokenToUse:{2}", string.Join(",", this.ChannelsToUse.ToArray()), string.Join(",", this.ChannelTimetokensToUse.ToArray()), this.TimetokenToUse), PNLoggingMethod.LevelInfo);
            #endif
            
            Uri request = BuildRequests.BuildMessageCountsRequest(
                ChannelsToUse.ToArray(),
                ChannelTimetokensToUse.ToArray(),
                TimetokenToUse,
                this.PubNubInstance,
                this.QueryParams
            );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 

        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //Returned JSON: `{"status": 200, "error": false, "error_message": "", "channels": {"my-channel1":1,"my-channel":2}}`
            PNMessageCountsResult pnMessageCountsResult = new PNMessageCountsResult();
            PNStatus pnStatus = new PNStatus();
            try{
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary != null) {
                    string message = Utility.ReadMessageFromResponseDictionary(dictionary, "error_message");
                    if(Utility.CheckDictionaryForError(dictionary, "error")){
                        pnMessageCountsResult = null;
                        pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
                    } else {
                        object objChannelsDict;
                        dictionary.TryGetValue("channels", out objChannelsDict);
                        if(objChannelsDict != null){
                            Dictionary<string, object> channelsDict = objChannelsDict as Dictionary<string, object>;
                            if(channelsDict==null){
                                pnMessageCountsResult = null;
                                pnStatus = base.CreateErrorResponseFromMessage("channelsResult dictionary is null", requestState, PNStatusCategory.PNUnknownCategory);
                            } else {
                                Dictionary<string, int> resultDict = new Dictionary<string, int>();
                                foreach(KeyValuePair<string, object> kvp in channelsDict){
                                    resultDict.Add(kvp.Key, Convert.ToInt32(kvp.Value));
                                }
                                pnMessageCountsResult.Channels = resultDict;
                            }
                        }
                    }
                } else {
                    pnMessageCountsResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage("Response dictionary is null", requestState, PNStatusCategory.PNUnknownCategory);
                }
            } catch (Exception ex) {
                pnMessageCountsResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnMessageCountsResult, pnStatus);
        }
    }
}

