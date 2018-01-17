using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class DeleteChannelGroupRequestBuilder: PubNubNonSubBuilder<DeleteChannelGroupRequestBuilder, PNChannelGroupsDeleteGroupResult>, IPubNubNonSubscribeBuilder<DeleteChannelGroupRequestBuilder, PNChannelGroupsDeleteGroupResult>
    {      
        public DeleteChannelGroupRequestBuilder(PubNubUnity pn):base(pn, PNOperationType.PNRemoveGroupOperation){
        }
        private string ChannelGroupToDelete { get; set;}

        public void ChannelGroup(string channelGroupName){
            ChannelGroupToDelete = channelGroupName;
            ChannelGroupsToUse = new List<string>{ChannelGroupToDelete};
        }
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNChannelGroupsDeleteGroupResult, PNStatus> callback)
        {
            this.Callback = callback;

            if (string.IsNullOrEmpty (ChannelGroupToDelete)) {
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("ChannelGroup to delete to empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);

                return;
            }
            base.Async(this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = base.OperationType;

            Uri request = BuildRequests.BuildRemoveChannelsFromChannelGroupRequest(
                null, 
                "", 
                ChannelGroupToDelete,
                this.PubNubInstance
            );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNChannelGroupsDeleteGroupResult pnChannelGroupsDeleteGroupResult = new PNChannelGroupsDeleteGroupResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if(dictionary != null) {
                string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
                if(Utility.CheckDictionaryForError(dictionary, "error")){
                    pnChannelGroupsDeleteGroupResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
                } else {
                    pnChannelGroupsDeleteGroupResult.Message = message;
                }
            } else {
                pnChannelGroupsDeleteGroupResult = null;
                pnStatus = base.CreateErrorResponseFromMessage("Response dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);

            }
            Callback(pnChannelGroupsDeleteGroupResult, pnStatus);
        }
        
    }
}

