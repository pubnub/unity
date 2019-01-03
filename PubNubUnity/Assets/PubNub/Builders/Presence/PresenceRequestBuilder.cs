using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class PresenceHeartbeatRequestBuilder: PubNubNonSubBuilder<PresenceHeartbeatRequestBuilder, PNPresenceHeartbeatResult>, IPubNubNonSubscribeBuilder<PresenceHeartbeatRequestBuilder, PNPresenceHeartbeatResult>
    {
        private bool connected { get; set;}
        List<ChannelEntity> ChannelEntities;
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
            ChannelEntities = new List<ChannelEntity>();

            string channels = "";
            if((ChannelsToUse != null) && (ChannelsToUse.Count>0)){
                ChannelsToUse.RemoveAll(t => t.Contains(Utility.PresenceChannelSuffix));
                string[] chArr = ChannelsToUse.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToArray();
                channels = String.Join(",", chArr);
                ChannelEntities.AddRange(Helpers.CreateChannelEntity(chArr, false, false, null, PubNubInstance.PNLog));
            }

            string channelGroups = "";
            if((ChannelGroupsToUse != null) && (ChannelGroupsToUse.Count>0)){
                ChannelGroupsToUse.RemoveAll(t => t.Contains(Utility.PresenceChannelSuffix));
                string[] cgArr = ChannelGroupsToUse.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToArray();
                channelGroups = String.Join(",", cgArr);
                ChannelEntities.AddRange(Helpers.CreateChannelEntity(cgArr, false, true, null, PubNubInstance.PNLog));
            }

            if(connected){
                PubNubInstance.SubWorker.PHBWorker.RunIndependentOfSubscribe = true;
                PubNubInstance.SubWorker.PHBWorker.ChannelGroups = channelGroups;
                PubNubInstance.SubWorker.PHBWorker.Channels = channels;
                if(UserState!=null){
                    PubNubInstance.SubWorker.PHBWorker.State = Helpers.BuildJsonUserState(ChannelEntities);
                } else {
                    PubNubInstance.SubWorker.PHBWorker.State = "";
                }
                
                PubNubInstance.SubWorker.PHBWorker.RunPresenceHeartbeat(false, PubNubInstance.PNConfig.PresenceInterval);
            } else {
                PubNubInstance.SubWorker.PHBWorker.RunIndependentOfSubscribe = false;
                PubNubInstance.SubWorker.PHBWorker.StopPresenceHeartbeat();
            }
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //Returned JSON: `{"status": 200, "message": "OK", "payload": {"channels": {"channel1": {"k": "v"}, "channel2": {}}}, "uuid": "pn-c5a12d424054a3688066572fb955b7a0", "service": "Presence"}`

            // PNGetStateResult pnGetStateResult = new PNGetStateResult();
            
            // Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            // PNStatus pnStatus = new PNStatus();
            // if(dictionary != null) {
            //     string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
            //     if(Utility.CheckDictionaryForError(dictionary, "error")){
            //         pnGetStateResult = null;
            //         pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
            //     } else {
            //         object objPayload;
            //         dictionary.TryGetValue("payload", out objPayload);

            //         if(objPayload!=null){
            //             Dictionary<string, object> payload = objPayload as Dictionary<string, object>;
            //             object objChannelsDict;
            //             payload.TryGetValue("channels", out objChannelsDict);

            //             if(objChannelsDict!=null){
            //                 Dictionary<string, object> channelsDict = objPayload as Dictionary<string, object>;
            //                 #if (ENABLE_PUBNUB_LOGGING)
            //                 foreach(KeyValuePair<string, object> kvp in channelsDict){
            //                     this.PubNubInstance.PNLog.WriteToLog(string.Format ("KVP: {0} {1}", kvp.Key, kvp.Value), PNLoggingMethod.LevelInfo);
            //                 }
            //                 #endif
            //                 pnGetStateResult.StateByChannels = channelsDict;
            //             } else {
            //                 pnGetStateResult.StateByChannels = payload;
            //             }
                
            //         } else {
            //             pnGetStateResult = null;
            //             pnStatus = base.CreateErrorResponseFromMessage("payload dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
            //         }
            //     }
            // } else {
            //     pnGetStateResult = null;
            //     pnStatus = base.CreateErrorResponseFromMessage("Response dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
            // }
            // Callback(pnGetStateResult, pnStatus);
        }

    }
}

