using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class LeaveRequestBuilder: PubNubNonSubBuilder<LeaveRequestBuilder, PNLeaveRequestResult>, IPubNubNonSubscribeBuilder<LeaveRequestBuilder, PNLeaveRequestResult>
    {      
        public LeaveRequestBuilder(PubNubUnity pn):base(pn){

        }
        private List<string> ChannelGroupsToLeave { get; set;}
        private List<string> ChannelsToLeave { get; set;}

        public void ChannelGroups(List<string> channelGroup){
            ChannelGroupsToLeave = channelGroup;
        }
        
        public void Channels(List<string> channels){
            ChannelsToLeave = channels;
        }
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNLeaveRequestResult, PNStatus> callback)
        {
            this.Callback = callback;
            Debug.Log ("LeaveRequestBuilder Async");

            base.Async(callback, PNOperationType.PNLeaveOperation, CurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState<PNLeaveRequestResult> requestState = new RequestState<PNLeaveRequestResult> ();
            requestState.RespType = PNOperationType.PNLeaveOperation;

            string channels = "";
            if(ChannelsToLeave!= null){
                channels = string.Join(",", ChannelsToLeave.ToArray());
            }
            string channelGroups = "";
            if(ChannelGroupsToLeave!= null){
                channelGroups = string.Join(",", ChannelGroupsToLeave.ToArray());
            }

            if(string.IsNullOrEmpty(channels) && (string.IsNullOrEmpty(channelGroups) )){
                Debug.Log("Both ChannelGroupsToLeave and ChannelsToLeave are empty, running unsubscribe all");
                channelGroups = Helpers.GetNamesFromChannelEntities(this.PubNubInstance.SubscriptionInstance.AllChannelGroups);
                channels = Helpers.GetNamesFromChannelEntities(this.PubNubInstance.SubscriptionInstance.AllChannels);
            }


            Uri request = BuildRequests.BuildLeaveRequest(
                channels,
                channelGroups,
                this.PubNubInstance.SubscriptionInstance.CompiledUserState,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            );
            this.PubNubInstance.PNLog.WriteToLog(string.Format("Run PNLeaveRequestResult {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult){
            //{"status": 200, "message": "OK", "action": "leave", "service": "Presence"}
            PNLeaveRequestResult pnLeaveRequestResult = new PNLeaveRequestResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if (dictionary!=null && dictionary.ContainsKey("error") && dictionary["error"].Equals(true)){
                pnLeaveRequestResult = null;
                pnStatus.Error = true;
                //TODO create error data
            } else if(dictionary!=null) {
                object objMessage;
                dictionary.TryGetValue("message", out objMessage);
                if(objMessage!=null){
                    pnLeaveRequestResult.Message = objMessage.ToString();
                }
            } else {
                pnLeaveRequestResult = null;
                pnStatus.Error = true;
            }
            Callback(pnLeaveRequestResult, pnStatus);
        }
        
    }
}

