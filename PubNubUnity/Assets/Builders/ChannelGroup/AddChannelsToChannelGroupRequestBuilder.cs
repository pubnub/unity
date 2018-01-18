using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class AddChannelsToChannelGroupRequestBuilder: PubNubNonSubBuilder<AddChannelsToChannelGroupRequestBuilder, PNChannelGroupsAddChannelResult>, IPubNubNonSubscribeBuilder<AddChannelsToChannelGroupRequestBuilder, PNChannelGroupsAddChannelResult>
    {
        private string ChannelGroupToAdd { get; set;}

        public AddChannelsToChannelGroupRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNAddChannelsToGroupOperation){
        }

        public void Channels(List<string> channelNames){
            ChannelsToUse = channelNames;
        }

        public void ChannelGroup(string channelGroupName){
            ChannelGroupToAdd = channelGroupName;
            ChannelGroupsToUse = new List<string>{ChannelGroupToAdd};
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNChannelGroupsAddChannelResult, PNStatus> callback)
        {
            this.Callback = callback;

            if((ChannelsToUse == null) || ((ChannelsToUse != null) && (ChannelsToUse.Count <= 0))){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("ChannelsToAdd null or empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);

                return;
            }

            if (string.IsNullOrEmpty (ChannelGroupToAdd)) {
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("ChannelGroup to add to empty.", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);
                return;
            }

            base.Async(this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = base.OperationType;

            Uri request = BuildRequests.BuildAddChannelsToChannelGroupRequest(
                ChannelsToUse.ToArray(), 
                "", 
                ChannelGroupToAdd,
                this.PubNubInstance
            );
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format("RunPNChannelGroupsAddChannel {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            #endif
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //Returned JSON: `{"service" : "channel-registry","status"  : 200,"error"   : false,"message" : "OK"}`

            PNChannelGroupsAddChannelResult pnChannelGroupsAddChannelResult = new PNChannelGroupsAddChannelResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if (dictionary != null){
                string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
                if(Utility.CheckDictionaryForError(dictionary, "error")){
                    pnChannelGroupsAddChannelResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
                } else {
                    pnChannelGroupsAddChannelResult.Message = message;
                }
                
            } else {
                pnChannelGroupsAddChannelResult = null;
                pnStatus = base.CreateErrorResponseFromMessage("Response dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
            }
            Callback(pnChannelGroupsAddChannelResult, pnStatus);
        }
       
    }
}

