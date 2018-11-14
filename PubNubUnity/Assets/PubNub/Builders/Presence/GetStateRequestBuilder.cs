using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class GetStateRequestBuilder: PubNubNonSubBuilder<GetStateRequestBuilder, PNGetStateResult>, IPubNubNonSubscribeBuilder<GetStateRequestBuilder, PNGetStateResult>
    {
        private string uuid { get; set;}

        public GetStateRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNGetStateOperation){
        }

        public void UUID(string uuidForState){
            this.uuid = uuidForState;
        }

        public void Channels(List<string> channelNames){
            ChannelsToUse = channelNames;
        }

        public void ChannelGroups(List<string> channelGroupNames){
            ChannelGroupsToUse = channelGroupNames;
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNGetStateResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            if (string.IsNullOrEmpty (uuid)) {
                uuid = this.PubNubInstance.PNConfig.UUID;
            }

            string channels = "";
            if((ChannelsToUse != null) && (ChannelsToUse.Count>0)){
                channels = String.Join(",", ChannelsToUse.ToArray());
            }

            string channelGroups = "";
            if((ChannelGroupsToUse != null) && (ChannelGroupsToUse.Count>0)){
                channelGroups = String.Join(",", ChannelGroupsToUse.ToArray());
            }

            Uri request = BuildRequests.BuildGetStateRequest(
                channels,
                channelGroups,
                uuid,
                this.PubNubInstance
            );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //Returned JSON: `{"status": 200, "message": "OK", "payload": {"channels": {"channel1": {"k": "v"}, "channel2": {}}}, "uuid": "pn-c5a12d424054a3688066572fb955b7a0", "service": "Presence"}`

            PNGetStateResult pnGetStateResult = new PNGetStateResult();
            
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if(dictionary != null) {
                string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
                if(Utility.CheckDictionaryForError(dictionary, "error")){
                    pnGetStateResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
                } else {
                    object objPayload;
                    dictionary.TryGetValue("payload", out objPayload);

                    if(objPayload!=null){
                        Dictionary<string, object> payload = objPayload as Dictionary<string, object>;
                        object objChannelsDict;
                        payload.TryGetValue("channels", out objChannelsDict);

                        if(objChannelsDict!=null){
                            Dictionary<string, object> channelsDict = objPayload as Dictionary<string, object>;
                            #if (ENABLE_PUBNUB_LOGGING)
                            foreach(KeyValuePair<string, object> kvp in channelsDict){
                                this.PubNubInstance.PNLog.WriteToLog(string.Format ("KVP: {0} {1}", kvp.Key, kvp.Value), PNLoggingMethod.LevelInfo);
                            }
                            #endif
                            pnGetStateResult.StateByChannels = channelsDict;
                        } else {
                            pnGetStateResult.StateByChannels = payload;
                        }
                
                    } else {
                        pnGetStateResult = null;
                        pnStatus = base.CreateErrorResponseFromMessage("payload dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
                    }
                }
            } else {
                pnGetStateResult = null;
                pnStatus = base.CreateErrorResponseFromMessage("Response dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
            }
            Callback(pnGetStateResult, pnStatus);
        }

    }
}

