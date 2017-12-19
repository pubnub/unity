using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class RemoveChannelsFromGroupRequestBuilder: PubNubNonSubBuilder<RemoveChannelsFromGroupRequestBuilder, PNChannelGroupsRemoveChannelResult>, IPubNubNonSubscribeBuilder<RemoveChannelsFromGroupRequestBuilder, PNChannelGroupsRemoveChannelResult>
    {      
        public RemoveChannelsFromGroupRequestBuilder(PubNubUnity pn):base(pn, PNOperationType.PNRemoveChannelsFromGroupOperation){

        }
        //private List<string> ChannelsToUse { get; set;}
        public void Channels(List<string> channels){
            ChannelsToUse = channels;
        }
        private string ChannelGroupToDelete { get; set;}

        public void ChannelGroup(string channelGroup){
            ChannelGroupToDelete = channelGroup;
            ChannelGroupsToUse = new List<string>(){ChannelGroupToDelete};
        }
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNChannelGroupsRemoveChannelResult, PNStatus> callback)
        {
            this.Callback = callback;
            if((ChannelsToUse == null) || ((ChannelsToUse != null) && (ChannelsToUse.Count <= 0))){
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("ChannelsToRemove null or empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);
                return;
            }

            if (string.IsNullOrEmpty (ChannelGroupToDelete)) {
                PNStatus pnStatus = base.CreateErrorResponseFromMessage("ChannelGroup to delete from is empty", null, PNStatusCategory.PNBadRequestCategory);
                Callback(null, pnStatus);

                return;
            }
            base.Async(this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState();
            requestState.OperationType = base.OperationType;
            
            /* Uri request = BuildRequests.BuildRemoveChannelsFromChannelGroupRequest(
                ChannelsToUse.ToArray(), 
                "", 
                ChannelGroupToDelete,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            ); */
            Uri request = BuildRequests.BuildRemoveChannelsFromChannelGroupRequest(
                ChannelsToUse.ToArray(), 
                "", 
                ChannelGroupToDelete,
                ref this.PubNubInstance
            );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }
        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNChannelGroupsRemoveChannelResult pnChannelGroupsRemoveChannelResult = new PNChannelGroupsRemoveChannelResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if (dictionary != null){
                string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
                if(Utility.CheckDictionaryForError(dictionary, "error")){
                    pnChannelGroupsRemoveChannelResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
                } else {
                    pnChannelGroupsRemoveChannelResult.Message = message;
                }
                
            } else {
                pnChannelGroupsRemoveChannelResult = null;
                pnStatus = base.CreateErrorResponseFromMessage("Response dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
            }
            
            Callback(pnChannelGroupsRemoveChannelResult, pnStatus);
        }
        
    }
}

