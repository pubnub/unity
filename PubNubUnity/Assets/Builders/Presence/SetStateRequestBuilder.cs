using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
/*#if NETFX_CORE
using System.Reflection;
#endif*/

namespace PubNubAPI
{
    public class SetStateRequestBuilder: PubNubNonSubBuilder<SetStateRequestBuilder, PNSetStateResult>, IPubNubNonSubscribeBuilder<SetStateRequestBuilder, PNSetStateResult>
    {
        List<ChannelEntity> ChannelEntities = null;
        //private List<string> ChannelsToUse { get; set;}
        //private List<string> ChannelGroupsToUse { get; set;}

        private string uuid { get; set;}
        private Dictionary<string, object> UserState { get; set;}

        public SetStateRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNSetStateOperation){
        }

        public void UUID(string uuid){
            this.uuid = uuid;
        }

        public void State(Dictionary<string, object> state){
            this.UserState = state;
        }

        public void Channels(List<string> channels){
            ChannelsToUse = channels;
        }

        public void ChannelGroups(List<string> channelGroups){
            ChannelGroupsToUse = channelGroups;
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNSetStateResult, PNStatus> callback)
        {
            this.Callback = callback;
            //TODO validate state here
            try{
                if(UserState!=null){
                    /*#if NETFX_CORE
                    Type t = UserState.GetTypeInfo();
                    #else
                    Type t = UserState.GetType();
                    #endif    
                    
                    bool isDict = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>);
                    if(!isDict){
                        PNStatus pnStatus = base.CreateErrorResponseFromMessage("State is not of type Dictionary<,>", null, PNStatusCategory.PNBadRequestCategory);
                        Callback(null, pnStatus);
                    } else {*/
                        //string userState = "";

                        if (CheckAndAddExistingUserState (
                            ChannelsToUse, 
                            ChannelGroupsToUse,
                            UserState, 
                            false,
                            uuid, 
                            this.PubNubInstance.PNConfig.UUID,
                            //out userState, 
                            out ChannelEntities
                        )) {
                            base.Async(this);

                            //SharedSetUserState(ChannelsForState, ChannelGroupsForState, channelEntities, uuid, UserState);
                        } else {
                            PNStatus pnStatus = base.CreateErrorResponseFromMessage("State not changed", null, PNStatusCategory.PNUnknownCategory);
                            Callback(null, pnStatus);
                        }
                    //}
                }
            } catch (Exception ex){
                PNStatus pnStatus = base.CreateErrorResponseFromException(ex, null, PNStatusCategory.PNUnknownCategory);
                Callback(null, pnStatus);
            }
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = PNOperationType.PNWhereNowOperation;

            string channels = "";
            if((ChannelsToUse != null) && (ChannelsToUse.Count>0)){
                channels = String.Join(",", ChannelsToUse.ToArray());
            }

            string channelGroups = "";
            if((ChannelGroupsToUse != null) && (ChannelGroupsToUse.Count>0)){
                channelGroups = String.Join(",", ChannelGroupsToUse.ToArray());
            }

            if (string.IsNullOrEmpty (uuid)) {
                uuid = this.PubNubInstance.PNConfig.UUID;
            }
            //TODO add state to instance
            /* Uri request = BuildRequests.BuildSetStateRequest(
                channels,
                channelGroups,
                Helpers.BuildJsonUserState(ChannelEntities),
                uuid,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            ); */
            Uri request = BuildRequests.BuildSetStateRequest(
                channels,
                channelGroups,
                Helpers.BuildJsonUserState(ChannelEntities),
                uuid,
                ref this.PubNubInstance
            );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        internal bool UpdateOrAddUserStateOfEntity(string channel, bool isChannelGroup, Dictionary<string, object> userState, bool edit, bool isForOtherUUID, ref List<ChannelEntity> channelEntities)
        {
            ChannelEntity ce = Helpers.CreateChannelEntity (channel, false, isChannelGroup, userState, ref this.PubNubInstance.PNLog);
            bool stateChanged = false;

            if (isForOtherUUID) {
                ce.ChannelParams.UserState = userState;
                channelEntities.Add (ce);
                stateChanged = true;
            } else {
                stateChanged = this.PubNubInstance.SubscriptionInstance.UpdateOrAddUserStateOfEntity (ref ce, userState, edit);
                if (!stateChanged) {
                    PNStatus pnStatus = base.CreateErrorResponseFromMessage("No change in User State", null, PNStatusCategory.PNUnknownCategory);
                    Callback(null, pnStatus);
                } else {
                    channelEntities.Add (ce);
                }
            }
            return stateChanged;
        }

        internal bool UpdateOrAddUserStateOfEntities(List<string> channelsOrChannelGroups, bool isChannelGroup, Dictionary<string, object> userState, bool edit, bool isForOtherUUID, ref List<ChannelEntity> channelEntities, ref bool stateChanged){
            if(channelsOrChannelGroups != null){
                foreach (string ch in channelsOrChannelGroups) {
                    if (!string.IsNullOrEmpty (ch)) {
                        bool changeState = UpdateOrAddUserStateOfEntity (ch, false, userState, edit, isForOtherUUID, ref channelEntities);
                        if (changeState && !stateChanged) {
                            stateChanged = true;
                        }
                    }
                }
            }
            return stateChanged;
        }

        internal bool CheckAndAddExistingUserState(List<string> channels, List<string> channelGroups, Dictionary<string, object> userState, bool edit, string uuid, string sessionUUID, out List<ChannelEntity> channelEntities)
        {
            bool stateChanged = false;
            bool isForOtherUUID = false;
            channelEntities = new List<ChannelEntity> ();
            if (!string.IsNullOrEmpty (uuid) && !sessionUUID.Equals (uuid)) {
                isForOtherUUID = true;
            } 
            UpdateOrAddUserStateOfEntities(channels, false, userState, edit, isForOtherUUID, ref channelEntities, ref stateChanged);
            UpdateOrAddUserStateOfEntities(channelGroups, true, userState, edit, isForOtherUUID, ref channelEntities, ref stateChanged);

            //returnUserState = Helpers.BuildJsonUserState(channelEntities);

            return stateChanged;
        }

        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            //{"status": 200, "message": "OK", "payload": {"channels": {"channel1": {"k": "v"}, "channel2": {}}}, "uuid": "pn-c5a12d424054a3688066572fb955b7a0", "service": "Presence"}

            //TODO read all values.
            
            PNSetStateResult pnSetStateResult = new PNSetStateResult();
            //pnGetStateResult
            
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if(dictionary != null) {
                string message = Utility.ReadMessageFromResponseDictionary(dictionary, "message");
                if(Utility.CheckDictionaryForError(dictionary, "error")){
                    pnSetStateResult = null;
                    pnStatus = base.CreateErrorResponseFromMessage(message, requestState, PNStatusCategory.PNUnknownCategory);
                } else {
                    object objPayload;
                    dictionary.TryGetValue("payload", out objPayload);

                    if(objPayload!=null){
                        Dictionary<string, object> payload = objPayload as Dictionary<string, object>;
                        object objChannelsDict;
                        payload.TryGetValue("channels", out objChannelsDict);
                        //TODO NO CG
                        //payload.TryGetValue("channelGroups", out objChannelsDict);

                        if(objChannelsDict!=null){
                            Dictionary<string, object> channelsDict = objPayload as Dictionary<string, object>;
                            foreach(KeyValuePair<string, object> kvp in channelsDict){
                                Debug.Log("KVP:" + kvp.Key + kvp.Value);
                            }
                            pnSetStateResult.StateByChannels = channelsDict;
                        } else {
                            pnSetStateResult.StateByChannels = payload;
                        }
                
                    } else {
                        pnSetStateResult = null;
                        pnStatus = base.CreateErrorResponseFromMessage("Payload dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
                    }
                }
            } else {
                pnSetStateResult = null;
                pnStatus = base.CreateErrorResponseFromMessage("Response dictionary is null", requestState, PNStatusCategory.PNMalformedResponseCategory);
            }
            Callback(pnSetStateResult, pnStatus);
        }
       
    }
}

