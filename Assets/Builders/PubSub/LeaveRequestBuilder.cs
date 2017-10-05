using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class LeaveRequestBuilder: PubNubNonSubBuilder<LeaveRequestBuilder, PNLeaveRequestResult>, IPubNubNonSubscribeBuilder<LeaveRequestBuilder, PNLeaveRequestResult>
    {      
        public LeaveRequestBuilder(PubNubUnity pn):base(pn, PNOperationType.PNLeaveOperation){

        }

        public void ChannelGroups(List<string> channelGroups){
            ChannelGroupsToUse = channelGroups;
        }
        
        public void Channels(List<string> channels){
            ChannelsToUse = channels;
        }
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNLeaveRequestResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;

            string channels = "";
            if(ChannelsToUse!= null){
                channels = string.Join(",", ChannelsToUse.ToArray());
            }
            string channelGroups = "";
            if(ChannelGroupsToUse!= null){
                channelGroups = string.Join(",", ChannelGroupsToUse.ToArray());
            }

            if(string.IsNullOrEmpty(channels) && (string.IsNullOrEmpty(channelGroups) )){
                Debug.Log("Both ChannelGroupsToLeave and ChannelsToLeave are empty, running unsubscribe all");
                channelGroups = Helpers.GetNamesFromChannelEntities(this.PubNubInstance.SubscriptionInstance.AllChannelGroups);
                channels = Helpers.GetNamesFromChannelEntities(this.PubNubInstance.SubscriptionInstance.AllChannels);
            }

            List<ChannelEntity> subscribedChannels = this.PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;

            List<ChannelEntity> newChannelEntities;
            bool channelsOrChannelGroupsAdded = this.PubNubInstance.SubscriptionInstance.RemoveDuplicatesCheckAlreadySubscribedAndGetChannels(
                OperationType,
                ChannelsToUse,
                ChannelGroupsToUse,
                true,
                out 
                newChannelEntities
            );

            //if (newChannelEntities.Count > 0) {
                //Retrieve the current channels already subscribed previously and terminate them
                this.PubNubInstance.SubWorker.AbortPreviousRequest(subscribedChannels);

                /* Uri request = BuildRequests.BuildLeaveRequest(
                    channels,
                    channelGroups,
                    this.PubNubInstance.SubscriptionInstance.CompiledUserState,
                    this.PubNubInstance.PNConfig.UUID,
                    this.PubNubInstance.PNConfig.Secure,
                    this.PubNubInstance.PNConfig.Origin,
                    this.PubNubInstance.PNConfig.AuthKey,
                    this.PubNubInstance.PNConfig.SubscribeKey,
                    this.PubNubInstance.Version,
                    this.PubNubInstance.Latency
                ); */
                Uri request = BuildRequests.BuildLeaveRequest(
                    channels,
                    channelGroups,
                    ref this.PubNubInstance
                );
                base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 

                //Remove the valid channels from subscribe list for unsubscribe
                RemoveUnsubscribedChannelsAndDeleteUserState(newChannelEntities);
                

                //Get all the channels
                //this.PubNubInstance.SubWorker.ContinueToSubscribeRestOfChannels(requestState.RespType);
                this.PubNubInstance.SubWorker.ContinueToSubscribeRestOfChannels();
            //} else {

            //}
            //this.PubNubInstance.SubscriptionInstance.Delete()

            
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //{"status": 200, "message": "OK", "action": "leave", "service": "Presence"}
            PNLeaveRequestResult pnLeaveRequestResult = new PNLeaveRequestResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if(dictionary != null) {
                string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
                if(Utility.CheckDictionaryForError(dictionary, "error")){
                    pnLeaveRequestResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
                } else {
                    pnLeaveRequestResult.Message = message;
                }
            } else {
                pnLeaveRequestResult = null;
                pnStatus = base.CreateErrorResponseFromMessage("Response dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);

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
                    string message = string.Format ("{0} Unsubscribed from {1}", (ce.ChannelID.IsPresenceChannel) ? "Presence" : "", channelToBeRemoved.Replace (Utility.PresenceChannelSuffix, ""));
                    //List<object> result = Helpers.CreateJsonResponse (jsonString, channelToBeRemoved.Replace (Utility.PresenceChannelSuffix, ""), JsonPluggableLibrary);
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("RemoveUnsubscribedChannelsAndDeleteUserState: JSON response={0}", message), PNLoggingMethod.LevelInfo);
                    #endif
                    PNStatus pnStatus = base.CreateErrorResponseFromMessage(message, null, PNStatusCategory.PNDisconnectedCategory);
                    Callback(null, pnStatus);

                    /* PubnubCallbacks.GoToCallback<T> (result, channelCallback.DisconnectCallback, JsonPluggableLibrary);*/
                } else {
                    string message = string.Format("Unsubscribe Error. Please retry the unsubscribe operation. channel{0}", channelToBeRemoved);
                    //PubnubErrorCode errorType = (ce.ChannelID.IsPresenceChannel) ? PubnubErrorCode.PresenceUnsubscribeFailed : PubnubErrorCode.UnsubscribeFailed;
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog(string.Format("RemoveUnsubscribedChannelsAndDeleteUserState: channel={0} unsubscribe error", channelToBeRemoved), PNLoggingMethod.LevelInfo);
                    #endif
                    /*PubnubCallbacks.CallErrorCallback<T>(message, channelCallback.ErrorCallback,errorType, PubnubErrorSeverity.Critical, PubnubErrorLevel);*/
                    PNStatus pnStatus = base.CreateErrorResponseFromMessage(message, null, PNStatusCategory.PNUnknownCategory);
                    Callback(null, pnStatus);
                }
            }
        }

        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }
        
    }
}

