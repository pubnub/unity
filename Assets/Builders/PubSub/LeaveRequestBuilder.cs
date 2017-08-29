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

            List<ChannelEntity> subscribedChannels = this.PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;

            List<ChannelEntity> newChannelEntities;
            bool channelsOrChannelGroupsAdded = this.PubNubInstance.SubscriptionInstance.RemoveDuplicatesCheckAlreadySubscribedAndGetChannels(
                requestState.RespType,
                ChannelsToLeave,
                ChannelGroupsToLeave,
                true,
                out 
                newChannelEntities
            );

            if (newChannelEntities.Count > 0) {
                //Retrieve the current channels already subscribed previously and terminate them
                this.PubNubInstance.SubWorker.AbortPreviousRequest(subscribedChannels);

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

                //Remove the valid channels from subscribe list for unsubscribe
                RemoveUnsubscribedChannelsAndDeleteUserState(newChannelEntities);
                

                //Get all the channels
                //this.PubNubInstance.SubWorker.ContinueToSubscribeRestOfChannels(requestState.RespType);
                this.PubNubInstance.SubWorker.ContinueToSubscribeRestOfChannels();
            }
            //this.PubNubInstance.SubscriptionInstance.Delete()

            
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
        
        void RemoveUnsubscribedChannelsAndDeleteUserState(List<ChannelEntity> channelEntities)
        {
            foreach(ChannelEntity ce in channelEntities)
            {
                string channelToBeRemoved = ce.ChannelID.ChannelOrChannelGroupName;
                //PubnubChannelCallback<T> channelCallback = ce.ChannelParams.Callbacks as PubnubChannelCallback<T>;
                if (this.PubNubInstance.SubscriptionInstance.Delete (ce)) {
                    string jsonString = string.Format ("{0} Unsubscribed from {1}", (ce.ChannelID.IsPresenceChannel) ? "Presence" : "", channelToBeRemoved.Replace (Utility.PresenceChannelSuffix, ""));
                    //List<object> result = Helpers.CreateJsonResponse (jsonString, channelToBeRemoved.Replace (Utility.PresenceChannelSuffix, ""), JsonPluggableLibrary);
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("RemoveUnsubscribedChannelsAndDeleteUserState: JSON response={0}", jsonString), PNLoggingMethod.LevelInfo);
                    #endif
                    /* PubnubCallbacks.GoToCallback<T> (result, channelCallback.DisconnectCallback, JsonPluggableLibrary);*/
                } else {
                    string message = string.Format("Unsubscribe Error. Please retry the unsubscribe operation. channel{0}", channelToBeRemoved);
                    //PubnubErrorCode errorType = (ce.ChannelID.IsPresenceChannel) ? PubnubErrorCode.PresenceUnsubscribeFailed : PubnubErrorCode.UnsubscribeFailed;
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog(string.Format("RemoveUnsubscribedChannelsAndDeleteUserState: channel={0} unsubscribe error", channelToBeRemoved), PNLoggingMethod.LevelInfo);
                    #endif
                    /*PubnubCallbacks.CallErrorCallback<T>(message, channelCallback.ErrorCallback,errorType, PubnubErrorSeverity.Critical, PubnubErrorLevel);*/
                }
            }
        }
        
    }
}

