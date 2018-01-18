using System;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class LeaveRequestBuilder: PubNubNonSubBuilder<LeaveRequestBuilder, PNLeaveRequestResult>, IPubNubNonSubscribeBuilder<LeaveRequestBuilder, PNLeaveRequestResult>
    {      
        public LeaveRequestBuilder(PubNubUnity pn):base(pn, PNOperationType.PNLeaveOperation){

        }

        public void ChannelGroups(List<string> channelGroupNames){
            ChannelGroupsToUse = channelGroupNames;
        }
        
        public void Channels(List<string> channelNames){
            ChannelsToUse = channelNames;
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
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog(string.Format ("Both ChannelGroupsToLeave and ChannelsToLeave are empty, running unsubscribe all"), PNLoggingMethod.LevelInfo);
                #endif                                    

                channelGroups = Helpers.GetNamesFromChannelEntities(this.PubNubInstance.SubscriptionInstance.AllChannelGroups, true);
                channels = Helpers.GetNamesFromChannelEntities(this.PubNubInstance.SubscriptionInstance.AllChannels, false);
                ChannelsToUse = Helpers.CreateListOfStringFromListOfChannelEntity(this.PubNubInstance.SubscriptionInstance.AllChannels);
                ChannelGroupsToUse = Helpers.CreateListOfStringFromListOfChannelEntity(this.PubNubInstance.SubscriptionInstance.AllChannelGroups);
            }

            List<ChannelEntity> subscribedChannels = this.PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;

            List<ChannelEntity> newChannelEntities;
            this.PubNubInstance.SubscriptionInstance.TryRemoveDuplicatesCheckAlreadySubscribedAndGetChannels(
                OperationType,
                ChannelsToUse,
                ChannelGroupsToUse,
                true,
                out 
                newChannelEntities
            );

            //Retrieve the current channels already subscribed previously and terminate them
            this.PubNubInstance.SubWorker.AbortPreviousRequest(subscribedChannels);

            if(!this.PubNubInstance.PNConfig.SuppressLeaveEvents){
                Uri request = BuildRequests.BuildLeaveRequest(
                    channels,
                    channelGroups,
                    this.PubNubInstance
                );
                base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
            }

            RemoveUnsubscribedChannelsAndDeleteUserState(newChannelEntities);
                

            //Get all the channels
            this.PubNubInstance.SubWorker.ContinueToSubscribeRestOfChannels();            
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //Returned JSON `{"status": 200, "message": "OK", "action": "leave", "service": "Presence"}`
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
                if (this.PubNubInstance.SubscriptionInstance.Delete (ce)) {
                    string message = string.Format ("{0} Unsubscribed from {1}", (ce.ChannelID.IsPresenceChannel) ? "Presence" : "", channelToBeRemoved.Replace (Utility.PresenceChannelSuffix, ""));
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("RemoveUnsubscribedChannelsAndDeleteUserState: JSON response={0}", message), PNLoggingMethod.LevelInfo);
                    #endif
                    PNStatus pnStatus = base.CreateStatusResponseFromMessage(false, message, null, PNStatusCategory.PNConnectedCategory);
                    Callback(null, pnStatus);
                } else {
                    string message = string.Format("Unsubscribe Error. Please retry the unsubscribe operation. channel{0}", channelToBeRemoved);
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog(string.Format("RemoveUnsubscribedChannelsAndDeleteUserState: channel={0} unsubscribe error", channelToBeRemoved), PNLoggingMethod.LevelInfo);
                    #endif
                    PNStatus pnStatus = base.CreateErrorResponseFromMessage(message, null, PNStatusCategory.PNUnknownCategory);
                    Callback(null, pnStatus);
                }
            }
        }
    }
}

