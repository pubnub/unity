using System;
using System.Collections.Generic;

namespace PubNubMessaging.Core
{
    public struct ChannelIdentity
    {
        public string ChannelOrChannelGroupName {get; set;}
        public bool IsChannelGroup {get; set;}

        public ChannelIdentity(string channelOrChannelGroupName, bool isChannelGroup){
            ChannelOrChannelGroupName = channelOrChannelGroupName;
            IsChannelGroup = isChannelGroup;
        }
    }

    public class ChannelParameters
    {
        public bool IsAwaitingConnectCallback {get; set;}
        public bool IsPresenceChannel {get; set;}
        public bool IsSubscribed {get; set;}
        public object Callbacks {get; set;}
        public Dictionary<string, object> UserState {get; set;}
        public Type TypeParameterType {get; set;}

        public ChannelParameters(){
            IsAwaitingConnectCallback = false;
            IsSubscribed = false;
            UserState = null;
            Callbacks = null;
            TypeParameterType = null;
            IsPresenceChannel = false;
        }
    }

    public class ChannelEntity
    {
        public ChannelIdentity ChannelID;
        public ChannelParameters ChannelParams;
        public ChannelEntity(ChannelIdentity channelID, ChannelParameters channelParams){
            this.ChannelID = channelID;
            this.ChannelParams = channelParams;
        }
    }

    public sealed class Subscription
    {
        private static volatile Subscription instance;
        private static object syncRoot = new Object();

        public static Subscription Instance
        {
            get 
            {
                if (instance == null) 
                {
                    lock (syncRoot) 
                    {
                        if (instance == null) 
                            instance = new Subscription();
                        
                        instance.ChannelsAndChannelGroupsAwaitingConnectCallback = new List<ChannelEntity> ();
                        instance.AllPresenceChannelsOrChannelGroups = new List<ChannelEntity> ();
                        instance.AllNonPresenceChannelsOrChannelGroups = new List<ChannelEntity> ();
                    }
                }

                return instance;
            }
        }

        public bool HasChannelGroups {get; private set;}

        public int CurrentSubscribedChannelsCount {
            get;
            private set;
        }

        public int CurrentSubscribedChannelGroupsCount {
            get;
            private set;
        }

        //public SafeDictionary<ChannelIdentity, ChannelParameters> SubscribedChannelsAndChannelGroupsAwaitingConnectCallback {
        public List<ChannelEntity> ChannelsAndChannelGroupsAwaitingConnectCallback {
            get;
            private set;
        }

        public List<ChannelEntity> SubscribedChannelsAndChannelGroups {
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
            //update SubscribedChannelsAndChannelGroupsAwaitingConnectCallback
            //and set isAwaitingConnectCallback false
        }

        public SafeDictionary<ChannelIdentity, ChannelParameters> ChannelEntitiesDictionary 
        {
            get {return channelEntitiesDictionary;}
        }

        private SafeDictionary<ChannelIdentity, ChannelParameters> channelEntitiesDictionary = new SafeDictionary<ChannelIdentity, ChannelParameters>();

        //public void Add(ChannelIdentity channelID, ChannelParameters channelParam){
        public void Subscribe(ChannelEntity channelEntity){
            if (!channelEntitiesDictionary.ContainsKey (channelEntity.ChannelID)) {
                channelEntitiesDictionary.Add (channelEntity.ChannelID, channelEntity.ChannelParams);
                ResetChannelsAndChannelGroups ();
            } else {
                channelEntitiesDictionary [channelEntity.ChannelID].Callbacks = channelEntity.ChannelParams.Callbacks;
                channelEntitiesDictionary [channelEntity.ChannelID].IsAwaitingConnectCallback = channelEntity.ChannelParams.IsAwaitingConnectCallback;
                channelEntitiesDictionary [channelEntity.ChannelID].IsPresenceChannel = channelEntity.ChannelParams.IsPresenceChannel;
                channelEntitiesDictionary [channelEntity.ChannelID].IsSubscribed = channelEntity.ChannelParams.IsSubscribed;
                channelEntitiesDictionary [channelEntity.ChannelID].TypeParameterType = channelEntity.ChannelParams.TypeParameterType;
                Dictionary<string, object> userState = channelEntitiesDictionary [channelEntity.ChannelID].UserState;
                if (userState == null) {
                    channelEntitiesDictionary [channelEntity.ChannelID].UserState = channelEntity.ChannelParams.UserState;
                }
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, channelEntities key found {1} {2}", DateTime.Now.ToString (), channelEntity.ChannelID.ChannelOrChannelGroupName, channelEntity.ChannelID.IsChannelGroup), LoggingMethod.LevelInfo);
                #endif
            }
        }

        public void Subscribe(ChannelEntity[] channelEntities){
            foreach (ChannelEntity ce in channelEntities) {
                Subscribe (ce);
            }
        }

        public void Delete(ChannelEntity channelEntity)
        {
            channelEntitiesDictionary.Remove(channelEntity.ChannelID);

            ResetChannelsAndChannelGroups ();
        }

        public void ResetChannelsAndChannelGroups(){
            AllPresenceChannelsOrChannelGroups.Clear ();
            AllNonPresenceChannelsOrChannelGroups.Clear ();
            ChannelsAndChannelGroupsAwaitingConnectCallback.Clear ();
            SubscribedChannelsAndChannelGroups.Clear ();
            foreach (var ci in channelEntitiesDictionary) {
                if (ci.Value.IsSubscribed) {
                    if (ci.Key.IsChannelGroup) {
                        CurrentSubscribedChannelGroupsCount++;
                    } else {
                        CurrentSubscribedChannelsCount++;
                    }
                    SubscribedChannelsAndChannelGroups.Add (new ChannelEntity (ci.Key, ci.Value));

                    if (ci.Value.IsPresenceChannel) {
                        AllPresenceChannelsOrChannelGroups.Add (new ChannelEntity (ci.Key, ci.Value));
                    } else {
                        AllNonPresenceChannelsOrChannelGroups.Add (new ChannelEntity (ci.Key, ci.Value));
                    }

                    if (ci.Value.IsAwaitingConnectCallback) {
                        ChannelsAndChannelGroupsAwaitingConnectCallback.Add (new ChannelEntity (ci.Key, ci.Value));
                    }
                }
            }
            if(CurrentSubscribedChannelGroupsCount > 0){
                HasChannelGroups = true;
            }
            //SafeDictionary<ChannelIdentity, ChannelParameters> currentChannelsAndChannelGroups = new SafeDictionary<ChannelIdentity, ChannelParameters> ();
            //CurrentSubscribedChannelsAndChannelGroups = currentChannelsAndChannelGroups;
        }

        public bool UpdateOrAddUserStateOfEntity(ChannelEntity channelEntity, Dictionary<string, object> userState){
            //channelEntitiesDictionary [channelEntity.ChannelID].UserState = userState;
            bool stateChanged = false;
            if (channelEntitiesDictionary.ContainsKey (channelEntity.ChannelID)) {
                
                string newState = PubnubUnity.JsonPluggableLibrary.SerializeToJsonString (userState);
                if (channelEntitiesDictionary [channelEntity.ChannelID].UserState != null) {
                    string oldState = Helpers.BuildJsonUserState (channelEntitiesDictionary [channelEntity.ChannelID].UserState);
                    if (!oldState.Equals (newState)) {
                        channelEntitiesDictionary [channelEntity.ChannelID].UserState = userState;
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
            ResetChannelsAndChannelGroups ();
            CompiledUserState = Helpers.BuildJsonUserState (SubscribedChannelsAndChannelGroups);
            return stateChanged;
        }

        public void UpdateIsAwaitingConnectCallbacksOfEntity(List<ChannelEntity> channelEntity, bool isAwaitingConnectCallback){
            foreach (ChannelEntity ce in channelEntity) {
                //int index = channelEntities.IndexOf (ce);
                //if (index != -1)
                //channelEntities [index].ChannelParams.IsAwaitingConnectCallback = IsAwaitingConnectCallback;
                if(channelEntitiesDictionary.ContainsKey(ce.ChannelID))
                    channelEntitiesDictionary[ce.ChannelID].IsAwaitingConnectCallback = isAwaitingConnectCallback;
                
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
            SubscribedChannelsAndChannelGroups.Clear ();
            CompiledUserState = String.Empty;
            CurrentSubscribedChannelsCount = 0;
            CurrentSubscribedChannelGroupsCount = 0;
        }
    }


    //Combine request state
    /*public sealed class Subscription
    {
        public PubnubWebRequest Request;
        public PubnubWebResponse Response;
        public ResponseType RespType;
        public CurrentRequestType CurrRequestType;
        public bool Timeout;
        public bool Reconnect;
        public long Timetoken;
        public Type TypeParameterType;
        public long ID;

        /*public string Id { get; private set; }

        private Subscription(string id)
        {
            Id = id;
        }

        private static volatile Subscription instance;
        private static object syncRoot = new Object();

        public static Subscription Instance
        {
            get 
            {
                if (instance == null) 
                {
                    lock (syncRoot) 
                    {
                        if (instance == null) 
                            instance = new Subscription();
                    }
                }

                return instance;
            }
        }

        private Subscription()
        {                
            //Id = Guid.NewGuid();
        }

        public static SafeDictionary<string, string[]> CurrentSubscribedChannelsAndChannelGroups {
            get;
            set;
        }

        public static int CurrentSubscribedChannelsCount {
            get;
            set;
        }

        public static int CurrentSubscribedChannelGroupsCount {
            get;
            set;
        }

        public static SafeDictionary<string, string[]> SubscribedChannelsAndChannelGroupsAwaitingConnectCallback {
            get;
            set;
        }

        public static string CompiledUserState {
            get;
            set;
        }

        public static SafeDictionary<string, Dictionary<string, object>>  ChannelAndChannelGroupCallbacks {
            get;
            set;
        }

        public static bool ConnectCallbackSent {
            get;
            set;
            //update SubscribedChannelsAndChannelGroupsAwaitingConnectCallback
            //and set isAwaitingConnectCallback false
        }

        public static SafeDictionary<ChannelIdentity, ChannelParameters> ChannelEntities 
        {
            get {return channelEntities;}
        }

        public static SafeDictionary<ChannelIdentity, ChannelParameters> channelEntities = new SafeDictionary<ChannelIdentity, ChannelParameters>();
        public static IEnumerable<ChannelEntity> AllChannelEntities
        {
            get {return channelEntities;}
        }

        public static void Add(ChannelIdentity channelID, ChannelParameters channelParam){
            channelEntities.Add(channelID, channelParam);
            ResetChannelsAndChannelGroups ();
            return newSubscription;

        }
        public void Delete(Subscription itemToRemove)
        {
            channelEntities.Remove(itemToRemove);
            ResetChannelsAndChannelGroups ();
        }

        public void ResetChannelsAndChannelGroups(){
            SafeDictionary<string, string[]> currentChannelsAndChannelGroups = new SafeDictionary<string, string[]> ();
            CurrentSubscribedChannelsAndChannelGroups = currentChannelsAndChannelGroups;
        }

        public void ResetChannelsAndChannelGroups(){
            SafeDictionary<string, string[]> currentChannelsAndChannelGroups = new SafeDictionary<string, string[]> ();
            CurrentSubscribedChannelsAndChannelGroups = currentChannelsAndChannelGroups;
        }
    }*/
}

