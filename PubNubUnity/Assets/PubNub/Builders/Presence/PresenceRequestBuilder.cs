using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class PresenceHeartbeatRequestBuilder: PubNubNonSubBuilder<PresenceHeartbeatRequestBuilder, PNPresenceHeartbeatResult>, IPubNubNonSubscribeBuilder<PresenceHeartbeatRequestBuilder, PNPresenceHeartbeatResult>
    {
        private bool connected { get; set;}        
        private Dictionary<string, object> UserState { get; set;}
        public PresenceHeartbeatRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNPresenceHeartbeatOperation){
        }

        public void Connected(bool connected){
            this.connected = connected;
        }
        public void State(Dictionary<string, object> state){
            this.UserState = state;
        }

        public void Channels(List<string> channelNames){
            ChannelsToUse = channelNames;
        }

        public void ChannelGroups(List<string> channelGroupNames){
            ChannelGroupsToUse = channelGroupNames;
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNPresenceHeartbeatResult, PNStatus> callback)
        {
            this.Callback = callback;
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            List<ChannelEntity> channelEntities = new List<ChannelEntity>();

            string channels = "";
            if((ChannelsToUse != null) && (ChannelsToUse.Count>0)){
                ChannelsToUse.RemoveAll(t => t.Contains(Utility.PresenceChannelSuffix));
                string[] chArr = ChannelsToUse.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToArray();
                channels = String.Join(",", chArr);
                channelEntities.AddRange(Helpers.CreateChannelEntity(chArr, false, false, null, PubNubInstance.PNLog));
            }

            string channelGroups = "";
            if((ChannelGroupsToUse != null) && (ChannelGroupsToUse.Count>0)){
                ChannelGroupsToUse.RemoveAll(t => t.Contains(Utility.PresenceChannelSuffix));
                string[] cgArr = ChannelGroupsToUse.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToArray();
                channelGroups = String.Join(",", cgArr);
                channelEntities.AddRange(Helpers.CreateChannelEntity(cgArr, false, true, null, PubNubInstance.PNLog));
            }

            if(connected){
                PubNubInstance.SubWorker.PHBWorker.RunIndependentOfSubscribe = true;
                PubNubInstance.SubWorker.PHBWorker.ChannelGroups = channelGroups;
                PubNubInstance.SubWorker.PHBWorker.Channels = channels;
                if(UserState!=null){
                    PubNubInstance.SubWorker.PHBWorker.State = Helpers.BuildJsonUserState(channelEntities);
                } else {
                    PubNubInstance.SubWorker.PHBWorker.State = "";
                }
                PubNubInstance.SubWorker.PHBWorker.StopPresenceHeartbeat();
                PubNubInstance.SubWorker.PHBWorker.RunPresenceHeartbeat(false, PubNubInstance.PNConfig.PresenceInterval);
            } else {
                PubNubInstance.SubWorker.PHBWorker.RunIndependentOfSubscribe = false;
                PubNubInstance.SubWorker.PHBWorker.ChannelGroups = channelGroups;
                PubNubInstance.SubWorker.PHBWorker.Channels = channels;
                PubNubInstance.SubWorker.PHBWorker.StopPresenceHeartbeat();
                PubNubInstance.SubWorker.PHBWorker.RunPresenceHeartbeat(false, PubNubInstance.PNConfig.PresenceInterval);
            }
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            //No processing here, processing done in async method
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //No processing here
        }

    }
}

