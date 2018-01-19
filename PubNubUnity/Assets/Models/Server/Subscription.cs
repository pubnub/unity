using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PubNubAPI
{
    public sealed class Subscription
    {
        private readonly PubNubUnity PubNubInstance;

        public Subscription(PubNubUnity pn){
            PubNubInstance = pn;
            ChannelsAndChannelGroupsAwaitingConnectCallback = new List<ChannelEntity> ();
            AllPresenceChannelsOrChannelGroups = new List<ChannelEntity> ();
            AllNonPresenceChannelsOrChannelGroups = new List<ChannelEntity> ();
            AllChannels = new List<ChannelEntity> ();
            AllChannelGroups = new List<ChannelEntity> ();
            AllSubscribedChannelsAndChannelGroups  = new List<ChannelEntity> ();
        }

        public bool HasChannelGroups {get; private set;}
        public bool HasPresenceChannels {get; private set;}
        public bool HasChannelsOrChannelGroups {get; private set;}
        public bool HasChannels {get; private set;}

        public int CurrentSubscribedChannelsCount {
            get;
            private set;
        }

        public int CurrentSubscribedChannelGroupsCount {
            get;
            private set;
        }

        public List<ChannelEntity> ChannelsAndChannelGroupsAwaitingConnectCallback {
            get;
            private set;
        }

        public List<ChannelEntity> AllChannels {
            get;
            private set;
        }

        public List<ChannelEntity> AllChannelGroups {
            get;
            private set;
        }

        public List<ChannelEntity> AllSubscribedChannelsAndChannelGroups {
            get;
            private set;
        }

        public List<ChannelEntity> AllPresenceChannelsOrChannelGroups {
            get;
            private set;
        }

        public List<ChannelEntity> AllNonPresenceChannelsOrChannelGroups {
            get;
            private set;
        }

        public string CompiledUserState {
            get;
            private set;
        }

        public bool ConnectCallbackSent {
            get;
            private set;
        }

        public SafeDictionary<ChannelIdentity, ChannelParameters> ChannelEntitiesDictionary 
        {
            get {return channelEntitiesDictionary;}
        }

        private SafeDictionary<ChannelIdentity, ChannelParameters> channelEntitiesDictionary = new SafeDictionary<ChannelIdentity, ChannelParameters>();

        public void Add(ChannelEntity channelEntity, bool reset){

            if (!channelEntitiesDictionary.ContainsKey (channelEntity.ChannelID)) {
                channelEntity.ChannelParams.IsSubscribed = true;
                channelEntitiesDictionary.Add (channelEntity.ChannelID, channelEntity.ChannelParams);
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("Add: channelEntities key add {0} {1} {2}", channelEntity.ChannelID.ChannelOrChannelGroupName, channelEntity.ChannelID.IsChannelGroup, channelEntity.ChannelParams.IsSubscribed), PNLoggingMethod.LevelInfo);
                #endif
            } else {
                channelEntitiesDictionary [channelEntity.ChannelID].IsAwaitingConnectCallback = channelEntity.ChannelParams.IsAwaitingConnectCallback;
                channelEntitiesDictionary [channelEntity.ChannelID].IsSubscribed = true;
                Dictionary<string, object> userState = channelEntitiesDictionary [channelEntity.ChannelID].UserState;
                if (userState == null) {
                    channelEntitiesDictionary [channelEntity.ChannelID].UserState = channelEntity.ChannelParams.UserState;
                }
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("Add: channelEntities key update {0} {1} {2}", channelEntity.ChannelID.ChannelOrChannelGroupName, channelEntity.ChannelID.IsChannelGroup, channelEntity.ChannelParams.IsSubscribed), PNLoggingMethod.LevelInfo);
                #endif
            }
            if (reset) {
                ResetChannelsAndChannelGroupsAndBuildState ();
            }
        }

        public void Add(List<ChannelEntity> channelEntities){
            foreach (ChannelEntity ce in channelEntities) {
                Add (ce, false);
            }
            ResetChannelsAndChannelGroupsAndBuildState ();
        }

        public bool Delete(ChannelEntity channelEntity)
        {
            ChannelParameters cp;
            bool bDeleted = channelEntitiesDictionary.TryRemove(channelEntity.ChannelID, out cp);
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("Delete: channelEntities key found {0} {1}", channelEntity.ChannelID.ChannelOrChannelGroupName, bDeleted.ToString()), PNLoggingMethod.LevelInfo);
            #endif

            ResetChannelsAndChannelGroupsAndBuildState ();
            return bDeleted;
        }

        public void ResetChannelsAndChannelGroupsAndBuildState(){
            AllPresenceChannelsOrChannelGroups.Clear ();
            AllNonPresenceChannelsOrChannelGroups.Clear ();
            ChannelsAndChannelGroupsAwaitingConnectCallback.Clear ();
            AllChannels.Clear ();
            AllChannelGroups.Clear ();
            AllSubscribedChannelsAndChannelGroups.Clear ();
            HasChannelGroups = false;
            HasChannels = false;
            HasPresenceChannels = false;
            HasChannelsOrChannelGroups = false;
            CurrentSubscribedChannelsCount = 0;
            CurrentSubscribedChannelGroupsCount = 0;

            foreach (var ci in channelEntitiesDictionary) {
                if (ci.Value.IsSubscribed) {
                    if (ci.Key.IsChannelGroup) {
                        CurrentSubscribedChannelGroupsCount++;
                        AllChannelGroups.Add(new ChannelEntity (ci.Key, ci.Value));
                    } else {
                        CurrentSubscribedChannelsCount++;
                        AllChannels.Add(new ChannelEntity (ci.Key, ci.Value));
                    }
                    AllSubscribedChannelsAndChannelGroups.Add (new ChannelEntity (ci.Key, ci.Value));

                    if (ci.Key.IsPresenceChannel) {
                        AllPresenceChannelsOrChannelGroups.Add (new ChannelEntity (ci.Key, ci.Value));
                    } else {
                        AllNonPresenceChannelsOrChannelGroups.Add (new ChannelEntity (ci.Key, ci.Value));
                    }

                    if (ci.Value.IsAwaitingConnectCallback) {
                        ChannelsAndChannelGroupsAwaitingConnectCallback.Add (new ChannelEntity (ci.Key, ci.Value));
                    }
                }
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("ResetChannelsAndChannelGroupsAndBuildState: channelEntities subscription key/val {0} {1}", ci.Key.ChannelOrChannelGroupName, ci.Value.IsSubscribed), PNLoggingMethod.LevelInfo);
                #endif
            }
            if(CurrentSubscribedChannelGroupsCount > 0){
                HasChannelGroups = true;
            }
            if(CurrentSubscribedChannelsCount > 0){
                HasChannels = true;
            }
            if (AllPresenceChannelsOrChannelGroups.Count > 0) {
                HasPresenceChannels = true;
            }
            if(HasChannels || HasChannelGroups){
                HasChannelsOrChannelGroups = true;
            }
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format("PrevCompiledUserState: {0}", CompiledUserState), PNLoggingMethod.LevelInfo);
            #endif
            
            CompiledUserState = Helpers.BuildJsonUserState (AllSubscribedChannelsAndChannelGroups);
            
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format("CompiledUserState: {0}", CompiledUserState), PNLoggingMethod.LevelInfo);
            #endif
        }

        public Dictionary<string, object> EditUserState(Dictionary<string, object> newUserState, 
            Dictionary<string, object> oldUserState, bool edit)
        {
            if(newUserState != null){
                string[] userStateKeys = newUserState.Keys.ToArray<string> ();
                for (int keyIndex = 0; keyIndex < userStateKeys.Length; keyIndex++) {
                    string userStateKey = userStateKeys [keyIndex];
                    object userStateObj = newUserState [userStateKey];

                    if(oldUserState.ContainsKey(userStateKey)){
                        if(userStateObj != null){
                            oldUserState[userStateKey] = userStateObj;
                        } else {
                            oldUserState.Remove(userStateKey);
                        }
                    } else {
                        if(userStateObj != null){
                            oldUserState.Add(userStateKey, userStateObj);
                        }
                    }

                }
            }
            #if (ENABLE_PUBNUB_LOGGING)
            foreach(KeyValuePair<string, object> kvp in oldUserState){
                this.PubNubInstance.PNLog.WriteToLog(string.Format("EditUserState: userstate kvp: {0}, {1}, edit: {2}\n", kvp.Key, kvp.Value, edit), PNLoggingMethod.LevelInfo);
            }
            #endif
            return oldUserState;
        }

        public bool TryUpdateOrAddUserStateOfEntity(ref ChannelEntity channelEntity, Dictionary<string, object> userState, bool edit){
            bool stateChanged = false;
            if (channelEntitiesDictionary.ContainsKey (channelEntity.ChannelID)) {

                string newState = this.PubNubInstance.JsonLibrary.SerializeToJsonString (userState);
                if (channelEntitiesDictionary [channelEntity.ChannelID].UserState != null) {
                    string oldState = Helpers.BuildJsonUserState (channelEntitiesDictionary [channelEntity.ChannelID].UserState);
                    if (!oldState.Equals (newState)) {
                        channelEntitiesDictionary [channelEntity.ChannelID].UserState = EditUserState(userState, 
                            channelEntitiesDictionary [channelEntity.ChannelID].UserState, edit);

                        stateChanged = true;
                    }
                } else {
                    channelEntitiesDictionary [channelEntity.ChannelID].UserState = userState;
                    stateChanged = true;
                }

            } else {
                channelEntity.ChannelParams.UserState = userState;
                channelEntity.ChannelParams.IsSubscribed = false;
                channelEntitiesDictionary.Add (channelEntity.ChannelID, channelEntity.ChannelParams);
                stateChanged = true;
            }
            if(stateChanged){
                channelEntity.ChannelParams.UserState = channelEntitiesDictionary[channelEntity.ChannelID].UserState;
            }
            ResetChannelsAndChannelGroupsAndBuildState ();

            return stateChanged;
        }

        public void UpdateIsAwaitingConnectCallbacksOfEntity(List<ChannelEntity> channelEntity, bool isAwaitingConnectCallback){
            foreach (ChannelEntity ce in channelEntity) {
                if (channelEntitiesDictionary.ContainsKey (ce.ChannelID)) {
                    channelEntitiesDictionary [ce.ChannelID].IsAwaitingConnectCallback = isAwaitingConnectCallback;
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("UpdateIsAwaitingConnectCallbacksOfEntity key/val1 {0} {1}", ce.ChannelID.ChannelOrChannelGroupName, ce.ChannelID.IsChannelGroup.ToString()), PNLoggingMethod.LevelInfo);
                    #endif
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                else 
                {
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("UpdateIsAwaitingConnectCallbacksOfEntity not found key/val1 {0} {1}}", ce.ChannelID.ChannelOrChannelGroupName, ce.ChannelID.IsChannelGroup.ToString()), PNLoggingMethod.LevelInfo);
                    LogChannelEntitiesDictionary();
                }
                #endif
            }

            bool connectCallbackSent = true;

            ChannelsAndChannelGroupsAwaitingConnectCallback.Clear ();
            foreach (var ci in channelEntitiesDictionary) {
                if (ci.Value.IsAwaitingConnectCallback) {
                    connectCallbackSent = false;
                    ChannelsAndChannelGroupsAwaitingConnectCallback.Add (new ChannelEntity(ci.Key, ci.Value));
                }
            }

            ConnectCallbackSent = connectCallbackSent;
        }

        public void CleanUp(){
            ConnectCallbackSent = false;
            ChannelsAndChannelGroupsAwaitingConnectCallback.Clear ();
            channelEntitiesDictionary.Clear ();
            AllPresenceChannelsOrChannelGroups.Clear ();
            AllNonPresenceChannelsOrChannelGroups.Clear ();
            AllChannels.Clear ();
            AllChannelGroups.Clear ();
            AllSubscribedChannelsAndChannelGroups.Clear ();
            CompiledUserState = String.Empty;
            CurrentSubscribedChannelsCount = 0;
            CurrentSubscribedChannelGroupsCount = 0;
            HasChannelGroups = false;
            HasChannels = false;
            HasChannelsOrChannelGroups = false;
            HasPresenceChannels = false;
        }

        #if (ENABLE_PUBNUB_LOGGING)
        internal void LogChannelEntitiesDictionary(){
            StringBuilder sbLogs = new StringBuilder();
            foreach (var ci in ChannelEntitiesDictionary) {
                sbLogs.AppendFormat("\nChannelEntitiesDictionary \nChannelOrChannelGroupName:{0} \nIsChannelGroup:{1} \nIsPresenceChannel:{2}\n", ci.Key.ChannelOrChannelGroupName, ci.Key.IsChannelGroup.ToString(), ci.Key.IsPresenceChannel.ToString());
            }
            
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("LogChannelEntitiesDictionary: Count: {1} \n{0}", sbLogs.ToString(), ChannelEntitiesDictionary.Count), PNLoggingMethod.LevelInfo);            
        }
        #endif

        public bool TryRemoveDuplicatesCheckAlreadySubscribedAndGetChannels(PNOperationType type, List<string> rawChannels, List<string> rawChannelGroups, bool unsubscribeCheck, out List<ChannelEntity> channelEntities)
        {
            bool bReturn = false;
            bool channelAdded = false;
            bool channelGroupAdded = false;
            channelEntities = new List<ChannelEntity> ();
            if (rawChannels != null && rawChannels.Count > 0) {
                channelAdded = RemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon(type, rawChannels, false, unsubscribeCheck, ref channelEntities);
            }

            if (rawChannelGroups != null && rawChannelGroups.Count > 0) {
                channelGroupAdded = RemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon(type, rawChannelGroups, true, unsubscribeCheck, ref channelEntities);
            }

            bReturn = channelAdded || channelGroupAdded;

            return bReturn;
        }

        internal bool RemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon(PNOperationType type, List<string> channelsOrChannelGroups, bool isChannelGroup, bool unsubscribeCheck, ref List<ChannelEntity> channelEntities)
        {
            bool bReturn = false;
            if (channelsOrChannelGroups.Count > 0) {

                channelsOrChannelGroups = channelsOrChannelGroups.Where(x => !string.IsNullOrEmpty(x)).ToList();

                if (channelsOrChannelGroups.Count != channelsOrChannelGroups.Distinct ().Count ()) {
                    channelsOrChannelGroups = channelsOrChannelGroups.Distinct ().ToList ();
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("RemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon: distinct channelsOrChannelGroups len={0}, channelsOrChannelGroups = {1}", channelsOrChannelGroups.Count, string.Join(",", channelsOrChannelGroups.ToArray())), PNLoggingMethod.LevelInfo);
                    #endif

                    string channel = string.Join (",", Helpers.GetDuplicates (channelsOrChannelGroups).Distinct<string> ().ToArray<string> ());
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("RemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon: duplicates channelsOrChannelGroups {0}", channel), PNLoggingMethod.LevelInfo);
                    #endif

                }
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("RemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon: channelsOrChannelGroups len={0}, channelsOrChannelGroups = {1}", channelsOrChannelGroups.Count, string.Join(",", channelsOrChannelGroups.ToArray())), PNLoggingMethod.LevelInfo);
                #endif
                
                bReturn = CreateChannelEntityAndAddToSubscribe (type, channelsOrChannelGroups, isChannelGroup, unsubscribeCheck, ref channelEntities, PubNubInstance);
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("RemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon: channelsOrChannelGroups len <=0"), PNLoggingMethod.LevelInfo);
                #endif
            }
            return bReturn;
        }
        internal bool CreateChannelEntityAndAddToSubscribe(PNOperationType type, List<string> rawChannels, bool isChannelGroup, bool unsubscribeCheck, ref List<ChannelEntity> channelEntities, PubNubUnity pn)
        {
            bool bReturn = false;    
            for (int index = 0; index < rawChannels.Count; index++)
            {
                string channelName = rawChannels[index].Trim();

                if (channelName.Length > 0) {
                    if((type == PNOperationType.PNPresenceOperation) || (type == PNOperationType.PNPresenceUnsubscribeOperation)) {
                        channelName = string.Format ("{0}{1}", channelName, Utility.PresenceChannelSuffix);
                    }

                    #if (ENABLE_PUBNUB_LOGGING)
                    LogChannelEntitiesDictionary();
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("CreateChannelEntityAndAddToSubscribe: channel={0}",  channelName), PNLoggingMethod.LevelInfo);
                    #endif

                    //create channelEntity
                    ChannelEntity ce = Helpers.CreateChannelEntity (channelName, true, isChannelGroup, null, this.PubNubInstance.PNLog);

                    bool channelIsSubscribed = false;
                    if (ChannelEntitiesDictionary.ContainsKey (ce.ChannelID)){
                        channelIsSubscribed = ChannelEntitiesDictionary [ce.ChannelID].IsSubscribed;
                    }

                    if (unsubscribeCheck) {
                        if (!channelIsSubscribed) {
                            string message = string.Format ("{0}Channel Not Subscribed", (ce.ChannelID.IsPresenceChannel) ? "Presence " : "");
                            #if (ENABLE_PUBNUB_LOGGING)
                            this.PubNubInstance.PNLog.WriteToLog (string.Format ("CreateChannelEntityAndAddToSubscribe: channel={0} response={1}",  channelName, message), PNLoggingMethod.LevelInfo);
                            #endif
                        } else {
                            channelEntities.Add (ce);
                            bReturn = true;
                        }
                    } else {
                        if (channelIsSubscribed) {
                            string message = string.Format ("{0}Already subscribed", (ce.ChannelID.IsPresenceChannel) ? "Presence " : "");
                            #if (ENABLE_PUBNUB_LOGGING)
                            this.PubNubInstance.PNLog.WriteToLog (string.Format ("CreateChannelEntityAndAddToSubscribe: channel={0} response={1}",  channelName, message), PNLoggingMethod.LevelInfo);
                            #endif
                        } else {
                            channelEntities.Add (ce);
                            bReturn = true;
                        }
                    }
                } else {
                    #if (ENABLE_PUBNUB_LOGGING)
                    string message = "Invalid Channel Name";
                    if (isChannelGroup) {
                    message = "Invalid Channel Group Name";
                    }

                    this.PubNubInstance.PNLog.WriteToLog(string.Format("CreateChannelEntityAndAddToSubscribe: channel={0} response={1}", channelName, message), PNLoggingMethod.LevelInfo);
                    #endif
                }
            }
            return bReturn;
        }
    }
}

