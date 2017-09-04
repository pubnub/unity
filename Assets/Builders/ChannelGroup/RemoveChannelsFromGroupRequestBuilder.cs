using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class RemoveChannelsFromGroupRequestBuilder: PubNubNonSubBuilder<RemoveChannelsFromGroupRequestBuilder, PNChannelGroupsRemoveChannelResult>, IPubNubNonSubscribeBuilder<RemoveChannelsFromGroupRequestBuilder, PNChannelGroupsRemoveChannelResult>
    {      
        public RemoveChannelsFromGroupRequestBuilder(PubNubUnity pn):base(pn){

        }
        private List<string> ChannelsToRemove { get; set;}
        public void Channels(List<string> channels){
            ChannelsToRemove = channels;
        }
        private string ChannelGroupToDelete { get; set;}

        public void ChannelGroup(string channelGroup){
            ChannelGroupToDelete = channelGroup;
        }
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNChannelGroupsRemoveChannelResult, PNStatus> callback)
        {
            this.Callback = callback;
            Debug.Log ("RemoveChannelsFromGroupRequestBuilder Async");
            if((ChannelsToRemove == null) || ((ChannelsToRemove != null) && (ChannelsToRemove.Count <= 0))){
                Debug.Log("ChannelsToRemove null or empty");

                //TODO Send callback
                return;
            }

            if (string.IsNullOrEmpty (ChannelGroupToDelete)) {
                Debug.Log("ChannelGroup to delete from is empty");

                //TODO Send callback
                return;
            }
            base.Async(callback, PNOperationType.PNRemoveChannelsFromGroupOperation, PNCurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState<PNChannelGroupsRemoveChannelResult> requestState = new RequestState<PNChannelGroupsRemoveChannelResult> ();
            requestState.RespType = PNOperationType.PNRemoveChannelsFromGroupOperation;
            
            Uri request = BuildRequests.BuildRemoveChannelsFromChannelGroupRequest(
                ChannelsToRemove.ToArray(), 
                "", 
                ChannelGroupToDelete,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            );
            this.PubNubInstance.PNLog.WriteToLog(string.Format("Run PNRemoveChannelsFromGroupOperation {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }
        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }

        protected override void CreatePubNubResponse(object deSerializedResult){
            PNChannelGroupsRemoveChannelResult pnChannelGroupsRemoveChannelResult = new PNChannelGroupsRemoveChannelResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if (dictionary!=null && dictionary.ContainsKey("error") && dictionary["error"].Equals(true)){
                pnChannelGroupsRemoveChannelResult = null;
                pnStatus.Error = true;
                //TODO create error data
            } else if(dictionary!=null) {
                object objMessage;
                dictionary.TryGetValue("message", out objMessage);
                if(objMessage!=null){
                    pnChannelGroupsRemoveChannelResult.Message = objMessage.ToString();
                }
            } else {
                pnChannelGroupsRemoveChannelResult = null;
                pnStatus.Error = true;
            }
            Callback(pnChannelGroupsRemoveChannelResult, pnStatus);
        }
        
    }
}

