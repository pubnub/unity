using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetAllChannelsForGroupRequestBuilder: PubNubNonSubBuilder<GetAllChannelsForGroupRequestBuilder, PNChannelGroupsAllChannelsResult>, IPubNubNonSubscribeBuilder<GetAllChannelsForGroupRequestBuilder, PNChannelGroupsAllChannelsResult>
    {      
        public GetAllChannelsForGroupRequestBuilder(PubNubUnity pn):base(pn, PNOperationType.PNChannelsForGroupOperation){
        }
        private string ChannelGroupToList { get; set;}

        public void ChannelGroup(string channelGroupName){
            ChannelGroupToList = channelGroupName;
            ChannelGroupsToUse = new List<string>{ChannelGroupToList};
        }
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNChannelGroupsAllChannelsResult, PNStatus> callback)
        {
            this.Callback = callback;
            if (string.IsNullOrEmpty (ChannelGroupToList)) {
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("ChannelGroup to list to empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);
                return;
            }
            base.Async(this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = base.OperationType;
            
            Uri request = BuildRequests.BuildGetChannelsForChannelGroupRequest(
                "",
                ChannelGroupToList,
                false,
                this.PubNubInstance
            );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //Returned JSON `{"status": 200, "payload": {"channels": ["channel1", "channel2"], "group": "channelGroup1"}, "service": "channel-registry", "error": false}`
            
            PNChannelGroupsAllChannelsResult pnChannelGroupsAllChannelsResult = new PNChannelGroupsAllChannelsResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if (dictionary != null){
                string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
                if(Utility.CheckDictionaryForError(dictionary, "error")){
                    pnChannelGroupsAllChannelsResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
                } else {
                    object objPayload;
                    dictionary.TryGetValue("payload", out objPayload);
                    if(objPayload!=null){
                        
                        Dictionary<string, object> payload = objPayload as Dictionary<string, object>;
                        object objChannelsArray;
                        payload.TryGetValue("channels", out objChannelsArray);
                        if(objChannelsArray != null){
                            string[] channelsArray = objChannelsArray as string[];
                            if(channelsArray != null){
                                pnChannelGroupsAllChannelsResult.Channels = new List<string>();
                                foreach(string str in channelsArray){
                                    #if (ENABLE_PUBNUB_LOGGING)
                                    this.PubNubInstance.PNLog.WriteToLog(string.Format ("strchannelsArray: {0}", str), PNLoggingMethod.LevelInfo);
                                    #endif

                                    pnChannelGroupsAllChannelsResult.Channels.Add(str);
                                }
                            }
                        } 
                    }
                } 
            } else {
                pnChannelGroupsAllChannelsResult = null;
                pnStatus = base.CreateErrorResponseFromMessage("Response dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);

            }
            Callback(pnChannelGroupsAllChannelsResult, pnStatus);
        }
        
    }
}

