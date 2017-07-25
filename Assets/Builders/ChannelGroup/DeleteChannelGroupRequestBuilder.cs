using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class DeleteChannelGroupRequestBuilder: PubNubNonSubBuilder<DeleteChannelGroupRequestBuilder, PNChannelGroupsDeleteGroupResult>, IPubNubNonSubscribeBuilder<DeleteChannelGroupRequestBuilder, PNChannelGroupsDeleteGroupResult>
    {      
        public DeleteChannelGroupRequestBuilder(PubNubUnity pn):base(pn){

        }
        private string ChannelGroupToDelete { get; set;}

        public void ChannelGroup(string channelGroup){
            ChannelGroupToDelete = channelGroup;
        }
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNChannelGroupsDeleteGroupResult, PNStatus> callback)
        {
            this.Callback = callback;
            Debug.Log ("DeleteChannelGroupRequestBuilder Async");

            if (string.IsNullOrEmpty (ChannelGroupToDelete)) {
                Debug.Log("ChannelGroup to delete to empty");
                return;
            }
            base.Async(callback, PNOperationType.PNRemoveGroupOperation, CurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState<PNChannelGroupsDeleteGroupResult> requestState = new RequestState<PNChannelGroupsDeleteGroupResult> ();
            requestState.RespType = PNOperationType.PNRemoveGroupOperation;

            Uri request = BuildRequests.BuildRemoveChannelsFromChannelGroupRequest(
                null, 
                "", 
                ChannelGroupToDelete,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            );
            this.PubNubInstance.PNLog.WriteToLog(string.Format("RunPNChannelGroupsAddChannel {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult){
            PNChannelGroupsDeleteGroupResult pnChannelGroupsDeleteGroupResult = new PNChannelGroupsDeleteGroupResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if (dictionary!=null && dictionary.ContainsKey("error") && dictionary["error"].Equals(true)){
                pnChannelGroupsDeleteGroupResult = null;
                pnStatus.Error = true;
                //TODO create error data
            } else if(dictionary!=null) {
                object objMessage;
                dictionary.TryGetValue("message", out objMessage);
                if(objMessage!=null){
                    pnChannelGroupsDeleteGroupResult.Message = objMessage.ToString();
                }
            } else {
                pnChannelGroupsDeleteGroupResult = null;
                pnStatus.Error = true;
            }
            Callback(pnChannelGroupsDeleteGroupResult, pnStatus);
        }
        
    }
}

