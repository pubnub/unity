using System;
using UnityEngine;
using System.Linq;

namespace PubNubAPI
{
    public class PubNub
    {
        private const string build = "4.0";
        private static string pnsdkVersion = string.Format ("PubNub-CSharp-Unity/{0}", build);
        public static string Version {
            get {
                return pnsdkVersion;
            }
            set {
                pnsdkVersion = value;
            }
        }

        //TODO INotifyPropertyChanged
        public PNConfiguration PNConfig { get; set;}
        public string Test { get; set;}

        public event EventHandler<EventArgs> SusbcribeCallback; 
        public void RaiseEvent(EventArgs ea){
            if (SusbcribeCallback != null) {
                SusbcribeCallback.Raise (typeof(PubNub), ea);
            }
        }

        private IJsonLibrary jsonLibrary = null;

        public IJsonLibrary JsonLibrary {
            get {
                if (jsonLibrary == null)
                {
                    jsonLibrary = JSONSerializer.JsonLibrary;
                }
                return jsonLibrary;
            }

            set {
                if (value is IJsonLibrary) {
                    jsonLibrary = value;
                } else {
                    jsonLibrary = null;
                    throw new ArgumentException ("Missing or Incorrect JsonLibrary value");
                }
            }
        }

        public void CleanUp (){
            //publishMessageCounter.Reset ();

            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog ("CleanUp: Destructing coroutine", LoggingMethod.LevelInfo);
            #endif
            /*if (coroutine != null) {
                UnityEngine.Object.Destroy (coroutine);
            }*/
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog ("CleanUp: Destructing GameObject", LoggingMethod.LevelInfo);
            #endif
            /*if(localGobj && (gobj != null))
            {
                UnityEngine.Object.Destroy (gobj);
            }*/
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Clean up complete.", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
        }

        ~PubNub(){
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog ("Destructing PubnubUnity", LoggingMethod.LevelInfo);
            #endif
            this.CleanUp ();
        }

        /// <summary>
        /// Gets or sets the set game object.
        /// This method should be called before init
        /// </summary>
        /// <value>The set game object.</value>
        public static GameObject GameObjectRef { get; set;}
        private bool localGobj;

        public PubNub (PNConfiguration pnConfiguration)
        {
            Test = "saddsads";
            PNConfig = pnConfiguration;

            #if(UNITY_IOS)
            Version = string.Format("PubNub-CSharp-UnityIOS/{0}", build);
            #elif(UNITY_STANDALONE_WIN)
            Version = string.Format("PubNub-CSharp-UnityWin/{0}", build);
            #elif(UNITY_STANDALONE_OSX)
            Version = string.Format("PubNub-CSharp-UnityOSX/{0}", build);
            #elif(UNITY_ANDROID)
            Version = string.Format("PubNub-CSharp-UnityAndroid/{0}", build);
            #elif(UNITY_STANDALONE_LINUX)
            Version = string.Format("PubNub-CSharp-UnityLinux/{0}", build);
            #elif(UNITY_WEBPLAYER)
            Version = string.Format("PubNub-CSharp-UnityWeb/{0}", build);
            #elif(UNITY_WEBGL)
            Version = string.Format("PubNub-CSharp-UnityWebGL/{0}", build);
            #else
            Version = string.Format("PubNub-CSharp-Unity/{0}", build);
            #endif
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (Version, LoggingMethod.LevelInfo);
            #endif

            if (GameObjectRef == null) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog ("Initilizing new GameObject", LoggingMethod.LevelInfo);
                #endif
                GameObjectRef = new GameObject ("PubnubGameObject");
                localGobj = true;
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog ("Reusing already initialized GameObject", LoggingMethod.LevelInfo);
                #endif
                localGobj = false;
            }
            QueueManager queueManager = PubNub.GameObjectRef.AddComponent<QueueManager> ();
        }

        public void AddListener(Action<PNStatus> callback, Action<PNMessageResult> callback2, Action<PNPresenceEventResult> callback3)
        {
            SusbcribeCallback += (object sender, EventArgs e) => {
                SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;

                Debug.Log ("AddListener SusbcribeCallback");
                if(mea!=null){
                    if(mea.pnStatus != null){
                        callback(mea.pnStatus);
                    }
                    if(mea.pnmr != null){
                        callback2(mea.pnmr);
                    }
                    if(mea.pnper != null){
                        callback3(mea.pnper);
                    }
                }
            };
        }

        public SubscribeBuilder Subscribe(){
            return new SubscribeBuilder (this);
        }

        public TimeBuilder Time(){
            Debug.Log ("TimeBuilder");
            return new TimeBuilder (this);
        }

        public WhereNowBuilder WhereNow(){
            Debug.Log ("WhereNowBuilder");
            return new WhereNowBuilder (this);
        }
       /* #region "PubNub API Channel Methods"

        #region "Subscribe Methods"

        public void Subscribe<T> (string channel, Action<T> subscribeCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback)
        {
            Subscribe<T> (channel, "", "", subscribeCallback, connectCallback, null, errorCallback);
        }

        public void Subscribe (string channel, Action<object> subscribeCallback, Action<object> connectCallback, Action<PubnubClientError> errorCallback)
        {
            Subscribe<object> (channel, "", "", subscribeCallback, connectCallback, null, errorCallback);
        }

        public void Subscribe (string channel, Action<object> subscribeCallback, Action<object> connectCallback,
            Action<object> wildcardPresenceCallback, Action<PubnubClientError> errorCallback)
        {
            Subscribe<object> (channel, "", "", subscribeCallback, connectCallback, wildcardPresenceCallback, errorCallback);
        }

        public void Subscribe<T> (string channel, string channelGroup, Action<T> subscribeCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<PubnubClientError> errorCallback)
        {
            Subscribe<T> (channel, channelGroup, "", subscribeCallback, connectCallback, wildcardPresenceCallback, errorCallback);
        }

        public void Subscribe<T> (string channel, Action<T> subscribeCallback, Action<T> connectCallback,
            Action<PubnubClientError> errorCallback, string timetoken)
        {
            Subscribe<T> (channel, "", timetoken, subscribeCallback, connectCallback, null, errorCallback);
        }

        public void Subscribe<T> (string channel, string channelGroup, string timetoken, Action<T> subscribeCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckChannelOrChannelGroup(channel, channelGroup);
            Utility.CheckCallback(subscribeCallback, CallbackType.Success);
            Utility.CheckCallback(connectCallback, CallbackType.Connect);
            Utility.CheckCallback(errorCallback, CallbackType.Error);
            Utility.CheckJSONPluggableLibrary();
            long lTimetoken = Utility.ValidateTimetoken(timetoken, true);

            pubnub.Subscribe<T> (channel, channelGroup, lTimetoken, subscribeCallback, connectCallback, wildcardPresenceCallback, errorCallback);
        }

        public void Subscribe<T> (string channel, string channelGroup, Action<T> subscribeCallback, Action<T> connectCallback,
            Action<PubnubClientError> errorCallback)
        {
            Subscribe<T> (channel, channelGroup, "", subscribeCallback, connectCallback, null, errorCallback);
        }

        public void Subscribe (string channel, string channelGroup, Action<object> subscribeCallback, Action<object> connectCallback,
            Action<object> wildcardPresenceCallback, Action<PubnubClientError> errorCallback)
        {
            Subscribe<object> (channel, channelGroup, "", subscribeCallback, connectCallback, wildcardPresenceCallback, errorCallback);
        }

        public void Subscribe (string channel, string channelGroup, Action<object> subscribeCallback, Action<object> connectCallback,
            Action<PubnubClientError> errorCallback)
        {
            Subscribe<object> (channel, channelGroup, "", subscribeCallback, connectCallback, null, errorCallback);
        }

        #endregion

        #region "Publish Methods"
        public bool Publish(string channel, object message, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return Publish<object>(channel, message, true, null, userCallback, errorCallback);
        }

        public bool Publish<T>(string channel, object message, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return Publish<T>(channel, message, true, null, userCallback, errorCallback);
        }

        public bool Publish(string channel, object message, bool storeInHistory, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return Publish<object> (channel, message, storeInHistory, null, userCallback, errorCallback);
        }

        public bool Publish<T>(string channel, object message, bool storeInHistory, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return Publish<T> (channel, message, storeInHistory, null, userCallback, errorCallback);
        }

        public bool Publish(string channel, object message, Dictionary<string, string> metadata, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return Publish<object>(channel, message, true, metadata, userCallback, errorCallback);
        }

        public bool Publish<T>(string channel, object message, Dictionary<string, string> metadata, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return Publish<T>(channel, message, true, metadata, userCallback, errorCallback);
        }

        public bool Publish(string channel, object message, bool storeInHistory, Dictionary<string, string> metadata, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return Publish<object> (channel, message, storeInHistory, metadata, userCallback, errorCallback);
        }

        public bool Publish<T>(string channel, object message, bool storeInHistory, Dictionary<string, string> metadata, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return Publish<T>(channel, message, storeInHistory, metadata, -1, userCallback, errorCallback);
        }

        public bool Publish<T>(string channel, object message, bool storeInHistory, Dictionary<string, string> metadata, int ttl, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckChannel(channel);
            Utility.CheckMessage(message);

            Utility.CheckPublishKey(pubnub.PublishKey);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            Utility.CheckJSONPluggableLibrary();

            return pubnub.Publish<T>(channel, message, storeInHistory, metadata, ttl, userCallback, errorCallback);
        }

        public bool Publish(string channel, object message, bool storeInHistory, Dictionary<string, string> metadata, int ttl, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return Publish<object> (channel, message, storeInHistory, metadata, ttl, userCallback, errorCallback);
        }

        #endregion

        #region "Presence Methods"
        public void Presence<T> (string channel, string channelGroup, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback)
        {
            Presence<T> (channel, channelGroup, "", userCallback, connectCallback, errorCallback);
        }

        public void Presence<T> (string channel, string channelGroup, string timetoken, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckChannelOrChannelGroup(channel, channelGroup);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(connectCallback, CallbackType.Connect);
            Utility.CheckCallback(errorCallback, CallbackType.Error);
            Utility.CheckJSONPluggableLibrary();
            long lTimetoken = Utility.ValidateTimetoken(timetoken, true);

            pubnub.Presence<T> (channel, channelGroup, lTimetoken, userCallback, connectCallback, errorCallback);
        }

        public void Presence (string channel, string channelGroup, Action<object> userCallback, Action<object> connectCallback, Action<PubnubClientError> errorCallback)
        {
            Presence<object> (channel, channelGroup, "", userCallback, connectCallback, errorCallback);
        }

        public void Presence (string channel, Action<object> userCallback, Action<object> connectCallback, Action<PubnubClientError> errorCallback)
        {
            Presence<object> (channel, "", "", userCallback, connectCallback, errorCallback);
        }

        public void Presence<T> (string channel, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback)
        {
            Presence<T> (channel, "", "", userCallback, connectCallback, errorCallback);
        }

        public void Presence<T> (string channel, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback, string timetoken)
        {
            Presence<T> (channel, "", timetoken, userCallback, connectCallback, errorCallback);
        }
        #endregion

        #region "DetailedHistory Methods"
        public bool DetailedHistory (string channel, long start, long end, int count, bool reverse, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return DetailedHistory<object> (channel, start, end, count, reverse, false, userCallback, errorCallback);
        }

        public bool DetailedHistory<T> (string channel, long start, long end, int count, bool reverse, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return DetailedHistory<T> (channel, start, end, count, reverse, false, userCallback, errorCallback);
        }

        public bool DetailedHistory (string channel, long start, Action<object> userCallback, Action<PubnubClientError> errorCallback, bool reverse)
        {
            return DetailedHistory<object> (channel, start, -1, -1, reverse, false, userCallback, errorCallback);
        }

        public bool DetailedHistory<T> (string channel, long start, Action<T> userCallback, Action<PubnubClientError> errorCallback, bool reverse)
        {
            return DetailedHistory<T> (channel, start, -1, -1, reverse, false, userCallback, errorCallback);
        }

        public bool DetailedHistory (string channel, int count, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return DetailedHistory<object> (channel, -1, -1, count, false, false, userCallback, errorCallback);
        }

        public bool DetailedHistory<T> (string channel, int count, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return DetailedHistory<T> (channel, -1, -1, count, false, false, userCallback, errorCallback);
        }

        public bool DetailedHistory (string channel, long start, long end, int count, bool reverse, bool includeTimetoken, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return DetailedHistory<object> (channel, start, end, count, reverse, includeTimetoken, userCallback, errorCallback);
        }

        public bool DetailedHistory<T> (string channel, long start, long end, int count, bool reverse, bool includeTimetoken, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckChannel(channel);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);
            Utility.CheckJSONPluggableLibrary();

            if (string.IsNullOrEmpty (channel) || string.IsNullOrEmpty (channel.Trim ())) {
                throw new ArgumentException ("Missing Channel");
            }
            if (userCallback == null) {
                throw new ArgumentException ("Missing userCallback");
            }
            if (errorCallback == null) {
                throw new ArgumentException ("Missing errorCallback");
            }
            if (JsonPluggableLibrary == null) {
                throw new NullReferenceException ("Missing Json Pluggable Library for Pubnub Instance");
            }

            return pubnub.DetailedHistory<T> (channel, start, end, count, reverse, includeTimetoken, userCallback, errorCallback);
        }

        public bool DetailedHistory (string channel, long start, bool includeTimetoken, Action<object> userCallback, Action<PubnubClientError> errorCallback, bool reverse)
        {
            return DetailedHistory<object> (channel, start, -1, -1, false, includeTimetoken, userCallback, errorCallback);
        }

        public bool DetailedHistory<T> (string channel, long start, bool includeTimetoken, Action<T> userCallback, Action<PubnubClientError> errorCallback, bool reverse)
        {
            return DetailedHistory<T> (channel, start, -1, -1, false, includeTimetoken, userCallback, errorCallback);
        }

        public bool DetailedHistory (string channel, int count, bool includeTimetoken, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return DetailedHistory<object> (channel, -1, -1, count, false, includeTimetoken, userCallback, errorCallback);
        }

        public bool DetailedHistory<T> (string channel, int count, bool includeTimetoken, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return DetailedHistory<T> (channel, -1, -1, count, false, includeTimetoken, userCallback, errorCallback);
        }
        #endregion

        #region "HereNow Methods"
        public bool HereNow (string channel, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return HereNow<object> (channel, true, false, userCallback, errorCallback);
        }

        public bool HereNow (string channel, bool showUUIDList, bool includeUserState, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return HereNow<object> (channel, showUUIDList, includeUserState, userCallback, errorCallback);
        }

        public bool HereNow<T> (string channel, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return HereNow<T> (channel, true, false, userCallback, errorCallback);
        }

        public bool HereNow<T> (string channel, bool showUUIDList, bool includeUserState,
            Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return HereNow<T> (channel, "", showUUIDList, includeUserState, userCallback, errorCallback);
        }

        public bool HereNow (string channel, string channelGroup, bool showUUIDList, bool includeUserState,
            Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return HereNow<object>(channel, channelGroup, showUUIDList, includeUserState, userCallback, errorCallback);
        }

        public bool HereNow<T> (string channel, string channelGroup, bool showUUIDList, bool includeUserState, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckChannelOrChannelGroup(channel, channelGroup);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);
            Utility.CheckJSONPluggableLibrary();

            return pubnub.HereNow<T> (channel, channelGroup, showUUIDList, includeUserState, userCallback, errorCallback);
        }
        #endregion

        #region "GlobalHereNow Methods"
        public bool GlobalHereNow (bool showUUIDList, bool includeUserState, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GlobalHereNow<object> (showUUIDList, includeUserState, userCallback, errorCallback);
        }

        public bool GlobalHereNow<T> (bool showUUIDList, bool includeUserState, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);
            Utility.CheckJSONPluggableLibrary();

            return pubnub.GlobalHereNow<T> (showUUIDList, includeUserState, userCallback, errorCallback);
        }

        public bool GlobalHereNow (Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GlobalHereNow<object> (true, false, userCallback, errorCallback);
        }

        public bool GlobalHereNow<T> (Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GlobalHereNow<T> (true, false, userCallback, errorCallback);
        }
        #endregion

        #region "WhereNow Methods"
        public void WhereNow (string uuid, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            WhereNow<object> (uuid, userCallback, errorCallback);
        }

        public void WhereNow<T> (string uuid, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);
            Utility.CheckJSONPluggableLibrary();

            pubnub.WhereNow<T> (uuid, userCallback, errorCallback);
        }
        #endregion

        #region "Unsubscribe Methods"
        public void Unsubscribe<T> (string channel, string channelGroup, Action<T> userCallback, Action<T> connectCallback, Action<T> disconnectCallback,
            Action<T> wildcardPresenceCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckChannelOrChannelGroup(channel, channelGroup);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(connectCallback, CallbackType.Connect);
            Utility.CheckCallback(disconnectCallback, CallbackType.Disconnect);
            Utility.CheckCallback(errorCallback, CallbackType.Error);
            Utility.CheckJSONPluggableLibrary();

            pubnub.Unsubscribe<T> (channel, channelGroup, userCallback, connectCallback, disconnectCallback, wildcardPresenceCallback, errorCallback);
        }

        public void Unsubscribe (string channel, string channelGroup, Action<object> userCallback, Action<object> connectCallback, Action<object> disconnectCallback, Action<PubnubClientError> errorCallback)
        {
            Unsubscribe<object> (channel, channelGroup, userCallback, connectCallback, disconnectCallback, null, errorCallback);
        }

        public void Unsubscribe (string channel, Action<object> userCallback, Action<object> connectCallback, Action<object> disconnectCallback, Action<PubnubClientError> errorCallback)
        {
            Unsubscribe<object> (channel, "", userCallback, connectCallback, disconnectCallback, null, errorCallback);
        }

        public void Unsubscribe<T> (string channel, Action<T> userCallback, Action<T> connectCallback, Action<T> disconnectCallback,
            Action<PubnubClientError> errorCallback)
        {
            Unsubscribe<T> (channel, "", userCallback, connectCallback, disconnectCallback, null, errorCallback);
        }

        public void Unsubscribe<T> (string channel, string channelGroup, Action<T> userCallback, Action<T> connectCallback, Action<T> disconnectCallback,
            Action<PubnubClientError> errorCallback)
        {
            Unsubscribe<T> (channel, channelGroup, userCallback, connectCallback, disconnectCallback, null, errorCallback);
        }
        #endregion

        #region "PresenceUnsubscribe Methods"
        public void PresenceUnsubscribe (string channel, Action<object> userCallback, Action<object> connectCallback, Action<object> disconnectCallback, Action<PubnubClientError> errorCallback)
        {
            PresenceUnsubscribe<object> (channel, "", userCallback, connectCallback, disconnectCallback, errorCallback);
        }

        public void PresenceUnsubscribe<T> (string channel, Action<T> userCallback, Action<T> connectCallback, Action<T> disconnectCallback, Action<PubnubClientError> errorCallback)
        {
            PresenceUnsubscribe<T> (channel, "", userCallback, connectCallback, disconnectCallback, errorCallback);
        }

        public void PresenceUnsubscribe (string channel, string channelGroup, Action<object> userCallback, Action<object> connectCallback, Action<object> disconnectCallback, Action<PubnubClientError> errorCallback)
        {
            PresenceUnsubscribe<object> (channel, channelGroup, userCallback, connectCallback, disconnectCallback, errorCallback);
        }

        public void PresenceUnsubscribe<T> (string channel, string channelGroup, Action<T> userCallback, Action<T> connectCallback, Action<T> disconnectCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckChannelOrChannelGroup(channel, channelGroup);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(connectCallback, CallbackType.Connect);
            Utility.CheckCallback(disconnectCallback, CallbackType.Disconnect);
            Utility.CheckCallback(errorCallback, CallbackType.Error);
            Utility.CheckJSONPluggableLibrary();

            pubnub.PresenceUnsubscribe<T> (channel, channelGroup, userCallback, connectCallback, disconnectCallback, errorCallback);
        }
        #endregion

        #region "Time Methods"
        public bool Time (Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return Time<object> (userCallback, errorCallback);
        }

        public bool Time<T> (Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);
            Utility.CheckJSONPluggableLibrary();

            return pubnub.Time<T> (userCallback, errorCallback);
        }
        #endregion

        #region "AuditAccess Methods"
        public void AuditAccess<T> (string channel, string authenticationKey, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckSecretKey(pubnub.SecretKey);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            pubnub.AuditAccess<T> (channel, authenticationKey, userCallback, errorCallback);
        }

        public void AuditAccess<T> (string channel, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            AuditAccess<T> (channel, "",  userCallback, errorCallback);
        }

        public void AuditAccess<T> (Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            AuditAccess<T> ("", "", userCallback, errorCallback);
        }

        public void AuditAccess (string channel, string authenticationKey, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            AuditAccess<object> (channel, authenticationKey, userCallback, errorCallback);
        }

        public void AuditAccess (string channel, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            AuditAccess<object> (channel, "", userCallback, errorCallback);
        }

        public void AuditAccess (Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            AuditAccess<object> ("", "", userCallback, errorCallback);
        }
        #endregion

        #region "Channel Group PAM"
        public void ChannelGroupAuditAccess<T>(string channelGroup, string authenticationKey, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckSecretKey(pubnub.SecretKey);
            channelGroup = Utility.CheckChannelGroup(channelGroup, false);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            pubnub.ChannelGroupAuditAccess<T>(channelGroup, authenticationKey, userCallback, errorCallback);
        }

        public void ChannelGroupAuditAccess<T>(string channelGroup, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            ChannelGroupAuditAccess<T>(channelGroup, userCallback, errorCallback);
        }

        public void ChannelGroupAuditAccess<T>(Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            ChannelGroupAuditAccess<T>(userCallback, errorCallback);
        }

        public void ChannelGroupAuditPresenceAccess<T>(string channelGroup, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            ChannelGroupAuditPresenceAccess<T>(channelGroup, userCallback, errorCallback);
        }

        public void ChannelGroupAuditPresenceAccess<T>(string channelGroup, string authenticationKey, Action<T> userCallback,
            Action<PubnubClientError> errorCallback)
        {
            Utility.CheckSecretKey(pubnub.SecretKey);
            channelGroup = Utility.CheckChannelGroup(channelGroup, true);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            pubnub.ChannelGroupAuditAccess<T>(channelGroup, authenticationKey, userCallback, errorCallback);
        }

        public bool ChannelGroupGrantAccess<T>(string channelGroup, bool read, bool manage, int ttl, Action<T> userCallback,
            Action<PubnubClientError> errorCallback)
        {
            return ChannelGroupGrantAccess<T>(channelGroup, "", read, manage, ttl, userCallback, errorCallback);
        }

        public bool ChannelGroupGrantAccess<T>(string channelGroup, bool read, bool manage, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return ChannelGroupGrantAccess<T>(channelGroup, read, manage, -1, userCallback, errorCallback);
        }

        public bool ChannelGroupGrantAccess<T>(string channelGroup, string authenticationKey, bool read, bool manage, int ttl, Action<T> userCallback,
            Action<PubnubClientError> errorCallback)
        {
            Utility.CheckSecretKey(pubnub.SecretKey);
            channelGroup = Utility.CheckChannelGroup(channelGroup, false);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            return pubnub.ChannelGroupGrantAccess<T>(channelGroup, authenticationKey, read, false, manage, ttl, userCallback, errorCallback);
        }

        public bool ChannelGroupGrantAccess<T>(string channelGroup, string authenticationKey, bool read, bool manage, Action<T> userCallback,
            Action<PubnubClientError> errorCallback)
        {
            return ChannelGroupGrantAccess<T>(channelGroup, authenticationKey, read, manage, -1, userCallback, errorCallback);
        }


        public bool ChannelGroupGrantPresenceAccess<T>(string channelGroup, bool read, bool manage, Action<T> userCallback,
            Action<PubnubClientError> errorCallback)
        {
            return ChannelGroupGrantPresenceAccess<T>(channelGroup, read, manage, -1, userCallback, errorCallback);
        }

        public bool ChannelGroupGrantPresenceAccess<T>(string channelGroup, bool read, bool manage, int ttl, Action<T> userCallback,
            Action<PubnubClientError> errorCallback)
        {
            return ChannelGroupGrantPresenceAccess(channelGroup, read, manage, ttl, userCallback, errorCallback);
        }

        public bool ChannelGroupGrantPresenceAccess<T>(string channelGroup, string authenticationKey, bool read, bool manage,
            Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return ChannelGroupGrantPresenceAccess<T>(channelGroup, authenticationKey, read, manage, -1, userCallback, errorCallback);
        }

        public bool ChannelGroupGrantPresenceAccess<T>(string channelGroup, string authenticationKey, bool read, bool manage, int ttl,
            Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckSecretKey(pubnub.SecretKey);
            channelGroup = Utility.CheckChannelGroup(channelGroup, true);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            return pubnub.ChannelGroupGrantAccess(channelGroup, authenticationKey, read, false, manage, ttl, userCallback, errorCallback);
        }
        #endregion

        #region "AuditPresenceAccess Methods"
        public void AuditPresenceAccess (string channel, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            AuditPresenceAccess<object> (channel, "",  userCallback, errorCallback);
        }

        public void AuditPresenceAccess (string channel, string authenticationKey, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            AuditPresenceAccess<object> (channel, authenticationKey, userCallback, errorCallback);
        }

        public void AuditPresenceAccess<T> (string channel, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            AuditPresenceAccess<T> (channel, "", userCallback, errorCallback);
        }

        public void AuditPresenceAccess<T> (string channel, string authenticationKey, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckSecretKey(pubnub.SecretKey);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            pubnub.AuditPresenceAccess<T> (channel, authenticationKey, userCallback, errorCallback);
        }
        #endregion

        #region "GrantAccess Methods"
        public bool GrantAccess<T> (string channel, bool read, bool write, int ttl, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GrantAccess<T> (channel, "", read, write, ttl, userCallback, errorCallback);
        }

        public bool GrantAccess<T> (string channel, bool read, bool write, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GrantAccess<T> (channel, "", read, write, -1, userCallback, errorCallback);
        }

        public bool GrantAccess<T> (string channel, string authenticationKey, bool read, bool write, int ttl, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckSecretKey(pubnub.SecretKey);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            return pubnub.GrantAccess<T> (channel, authenticationKey, read, write, ttl, userCallback, errorCallback);
        }

        public bool GrantAccess<T> (string channel, string authenticationKey, bool read, bool write, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GrantAccess<T> (channel, authenticationKey, read, write, -1, userCallback, errorCallback);
        }

        public bool GrantAccess (string channel, bool read, bool write, int ttl, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GrantAccess<object> (channel, "", read, write, ttl, userCallback, errorCallback);
        }

        public bool GrantAccess (string channel, bool read, bool write, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GrantAccess<object> (channel, "", read, write, -1, userCallback, errorCallback);
        }

        public bool GrantAccess (string channel, string authenticationKey, bool read, bool write, int ttl, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GrantAccess<object> (channel, authenticationKey, read, write, ttl, userCallback, errorCallback);
        }

        public bool GrantAccess (string channel, string authenticationKey, bool read, bool write, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GrantAccess<object> (channel, authenticationKey, read, write, -1, userCallback, errorCallback);
        }
        #endregion

        #region "GrantPresenceAccess Methods"
        public bool GrantPresenceAccess<T> (string channel, bool read, bool write, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GrantPresenceAccess<T> (channel, "", read, write, -1, userCallback, errorCallback);
        }

        public bool GrantPresenceAccess<T> (string channel, bool read, bool write, int ttl, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GrantPresenceAccess<T> (channel, "", read, write, ttl, userCallback, errorCallback);
        }

        public bool GrantPresenceAccess<T> (string channel, string authenticationKey, bool read, bool write, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GrantPresenceAccess<T> (channel, authenticationKey, read, write, -1, userCallback, errorCallback);
        }

        public bool GrantPresenceAccess<T> (string channel, string authenticationKey, bool read, bool write, int ttl, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckSecretKey(pubnub.SecretKey);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            return pubnub.GrantPresenceAccess<T> (channel, authenticationKey, read, write, ttl, userCallback, errorCallback);
        }

        public bool GrantPresenceAccess (string channel, bool read, bool write, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GrantPresenceAccess<object> (channel, "", read, write, -1, userCallback, errorCallback);
        }

        public bool GrantPresenceAccess (string channel, bool read, bool write, int ttl, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GrantPresenceAccess<object> (channel, "", read, write, ttl, userCallback, errorCallback);
        }

        public bool GrantPresenceAccess (string channel, string authenticationKey, bool read, bool write, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GrantPresenceAccess<object> (channel, authenticationKey, read, write, -1, userCallback, errorCallback);
        }

        public bool GrantPresenceAccess (string channel, string authenticationKey, bool read, bool write, int ttl, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            return GrantPresenceAccess<object> (channel, authenticationKey, read, write, ttl, userCallback, errorCallback);
        }
        #endregion

        #region "SetUserState Methods"
        public void SetUserState(string channel, string channelGroup, string uuid, string jsonUserState, Action<object> userCallback,
            Action<PubnubClientError> errorCallback)
        {
            SetUserState<object>(channel, channelGroup, uuid, jsonUserState, userCallback, errorCallback);
        }

        public void SetUserState<T>(string channel, string jsonUserState, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            SetUserState<T>(channel, "", "", jsonUserState, userCallback, errorCallback);
        }

        public void SetUserState<T> (string channel, string uuid, string jsonUserState, Action<T> userCallback,
            Action<PubnubClientError> errorCallback)
        {
            SetUserState<T> (channel, "", uuid, jsonUserState, userCallback, errorCallback);
        }

        public void SetUserState<T> (string channel, string channelGroup, string uuid, string jsonUserState, Action<T> userCallback,
            Action<PubnubClientError> errorCallback)
        {
            Utility.CheckChannelOrChannelGroup(channel, channelGroup);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);
            Utility.CheckJSONPluggableLibrary();

            Utility.CheckUserState(jsonUserState);

            pubnub.SetUserState<T> (channel, channelGroup, uuid, jsonUserState, userCallback, errorCallback);
        }

        public void SetUserState (string channel, string jsonUserState, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            SetUserState<object> (channel, "", "", jsonUserState, userCallback, errorCallback);
        }

        public void SetUserState (string channel, string uuid, string jsonUserState, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            SetUserState<object> (channel, "", uuid, jsonUserState, userCallback, errorCallback);
        }

        public void SetUserState<T> (string channel, string uuid, System.Collections.Generic.KeyValuePair<string, object> keyValuePair,
            Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            SetUserState<T> (channel, "", uuid, keyValuePair, userCallback, errorCallback);
        }

        public void SetUserState<T> (string channel, string channelGroup, string uuid, System.Collections.Generic.KeyValuePair<string, object> keyValuePair,
            Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckChannelOrChannelGroup(channel, channelGroup);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);
            Utility.CheckJSONPluggableLibrary();

            pubnub.SetUserState<T> (channel, channelGroup, uuid, keyValuePair, userCallback, errorCallback);
        }

        public void SetUserState<T> (string channel, System.Collections.Generic.KeyValuePair<string, object> keyValuePair,
            Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            SetUserState<T> (channel, "", "", keyValuePair, userCallback, errorCallback);
        }

        public void SetUserState (string channel, string uuid, System.Collections.Generic.KeyValuePair<string, object> keyValuePair, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            SetUserState<object> (channel, "", uuid, keyValuePair, userCallback, errorCallback);
        }

        public void SetUserState (string channel, System.Collections.Generic.KeyValuePair<string, object> keyValuePair,
            Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            SetUserState<object> (channel, "", "", keyValuePair, userCallback, errorCallback);
        }

        #endregion

        #region "GetUserState Methods"
        public void GetUserState<T> (string channel, string uuid, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            GetUserState<T> (channel, "", uuid, userCallback, errorCallback);
        }

        public void GetUserState<T> (string channel, string channelGroup, string uuid, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckChannelOrChannelGroup(channel, channelGroup);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);
            Utility.CheckJSONPluggableLibrary();

            pubnub.GetUserState<T> (channel, channelGroup, uuid, userCallback, errorCallback);
        }

        public void GetUserState<T> (string channel, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            GetUserState<T> (channel, "", userCallback, errorCallback);
        }

        public void GetUserState (string channel, string uuid, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            GetUserState<object> (channel, uuid, userCallback, errorCallback);
        }

        public void GetUserState (string channel, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            GetUserState<object> (channel, "", userCallback, errorCallback);
        }

        public void GetUserState(string channel, string channelGroup, string uuid, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            GetUserState<object>(channel, channelGroup, uuid, userCallback, errorCallback);
        }
        #endregion

        #region "Mobile Push"
        public void RegisterDeviceForPush(string channel, PushTypeService pushType, string pushToken, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            RegisterDeviceForPush<object>(channel, pushType, pushToken, userCallback, errorCallback);
        }

        public void RegisterDeviceForPush<T>(string channel, PushTypeService pushType, string pushToken, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckChannel(channel);
            Utility.CheckString(pushToken, "Push token");
            Utility.CheckPushType(pushType);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            pubnub.RegisterDeviceForPush<T>(channel, pushType, pushToken, userCallback, errorCallback);
        }

        public void UnregisterDeviceForPush(PushTypeService pushType, string pushToken, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            UnregisterDeviceForPush<object>(pushType, pushToken, userCallback, errorCallback);
        }

        public void UnregisterDeviceForPush<T>(PushTypeService pushType, string pushToken, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckString(pushToken, "Push token");
            Utility.CheckPushType(pushType);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            pubnub.UnregisterDeviceForPush<T>(pushType, pushToken, userCallback, errorCallback);
        }

        public void RemoveChannelForDevicePush(string channel, PushTypeService pushType, string pushToken, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            RemoveChannelForDevicePush<object>(channel, pushType, pushToken, userCallback, errorCallback);
        }

        public void RemoveChannelForDevicePush<T>(string channel, PushTypeService pushType, string pushToken, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckChannel(channel);
            Utility.CheckString(pushToken, "Push token");
            Utility.CheckPushType(pushType);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            pubnub.RemoveChannelForDevicePush<T>(channel, pushType, pushToken, userCallback, errorCallback);
        }

        public void GetChannelsForDevicePush(PushTypeService pushType, string pushToken, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            GetChannelsForDevicePush<object>(pushType, pushToken, userCallback, errorCallback);
        }

        public void GetChannelsForDevicePush<T>(PushTypeService pushType, string pushToken, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckString(pushToken, "Push token");
            Utility.CheckPushType(pushType);
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            pubnub.GetChannelsForDevicePush<T> (pushType, pushToken, userCallback, errorCallback);
        }

        #endregion

        #region "PubNub API Channel Group Methods"
        public void AddChannelsToChannelGroup(string[] channels, string channelGroup, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            AddChannelsToChannelGroup<object>(channels, channelGroup, userCallback, errorCallback);
        }

        public void AddChannelsToChannelGroup<T>(string[] channels, string channelGroup, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckChannels(channels);
            Utility.CheckString(channelGroup, "Channel Group");
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            pubnub.AddChannelsToChannelGroup<T>(channels, "",  channelGroup, userCallback, errorCallback);
        }

        public void RemoveChannelsFromChannelGroup<T>(string[] channels, string channelGroup, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckChannels(channels);
            Utility.CheckString(channelGroup, "Channel Group");
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            pubnub.RemoveChannelsFromChannelGroup<T>(channels, "",  channelGroup, userCallback, errorCallback);
        }

        public void RemoveChannelsFromChannelGroup(string[] channels, string channelGroup, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            RemoveChannelsFromChannelGroup<object>(channels, channelGroup, userCallback, errorCallback);
        }

        public void RemoveChannelGroup(string channelGroup, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            RemoveChannelGroup<object>(channelGroup, userCallback, errorCallback);
        }

        public void RemoveChannelGroup<T>(string channelGroup, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckString(channelGroup, "Channel Group");
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            pubnub.RemoveChannelGroup<T>("", channelGroup, userCallback, errorCallback);
        }

        public void GetChannelsForChannelGroup(string channelGroup, Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            GetChannelsForChannelGroup<object>(channelGroup, userCallback, errorCallback);
        }

        public void GetChannelsForChannelGroup<T>(string channelGroup, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckString(channelGroup, "Channel Group");
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            pubnub.GetChannelsForChannelGroup<T>("", channelGroup, userCallback, errorCallback);
        }

        public void GetAllChannelGroups(Action<object> userCallback, Action<PubnubClientError> errorCallback)
        {
            GetAllChannelGroups<object>(userCallback, errorCallback);
        }

        public void GetAllChannelGroups<T>(Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Utility.CheckCallback(userCallback, CallbackType.Success);
            Utility.CheckCallback(errorCallback, CallbackType.Error);

            pubnub.GetAllChannelGroups<T>("", userCallback, errorCallback);
        }

        #endregion

        #endregion

        #region "PubNub API Other Methods"

        public void TerminateCurrentSubscriberRequest<T> ()
        {
            pubnub.TerminateCurrentSubscriberRequest<T> ();
        }

        public void ResetPublishMessageCounter ()
        {
            pubnub.ResetPublishMessageCounter ();
        }

        public void EndPendingRequests<T> ()
        {
            pubnub.EndPendingRequests<T> ();
        }

        public void CleanUp (){
            pubnub.CleanUp ();
        }

        public Guid GenerateGuid ()
        {
            return Utility.GenerateGuid ();
        }

        public void ChangeUUID<T> (string newUUID)
        {
            pubnub.ChangeUUID<T> (newUUID);
        }

        public void TerminateCurrentSubscriberRequest ()
        {
            TerminateCurrentSubscriberRequest<object> ();
        }

        public void EndPendingRequests ()
        {
            EndPendingRequests<object> ();
        }

        public void ChangeUUID (string newUUID)
        {
            ChangeUUID<object> (newUUID);
        }

        public static long TranslateDateTimeToPubnubUnixNanoSeconds (DateTime dotNetUTCDateTime)
        {
            return Utility.TranslateDateTimeToPubnubUnixNanoSeconds (dotNetUTCDateTime);
        }

        public static DateTime TranslatePubnubUnixNanoSecondsToDateTime (long unixNanoSecondTime)
        {
            return Utility.TranslatePubnubUnixNanoSecondsToDateTime (unixNanoSecondTime);
        }

        #endregion

        #region "Properties"
        public string FilterExpression {
            get { return pubnub.FilterExpr; }
            set { pubnub.FilterExpr = value; }
        }

        public string AuthenticationKey {
            get { return pubnub.AuthenticationKey; }
            set { pubnub.AuthenticationKey = value; }
        }

        public LoggingMethod.Level PubnubLogLevel {
            get { return pubnub.PubnubLogLevel; }
            set { pubnub.PubnubLogLevel = value; }
        }

        public PubnubErrorFilter.Level PubnubErrorLevel {
            get { return pubnub.PubnubErrorLevel; }
            set { pubnub.PubnubErrorLevel = value; }
        }

        public int LocalClientHeartbeatInterval {
            get { return pubnub.LocalClientHeartbeatInterval; }
            set { pubnub.LocalClientHeartbeatInterval = value; }
        }

        public int NetworkCheckRetryInterval {
            get { return pubnub.NetworkCheckRetryInterval; }
            set { pubnub.NetworkCheckRetryInterval = value; }
        }

        public int NetworkCheckMaxRetries {
            get { return pubnub.NetworkCheckMaxRetries; }
            set { pubnub.NetworkCheckMaxRetries = value; }
        }

        public int NonSubscribeTimeout {
            get { return pubnub.NonSubscribeTimeout; }
            set { pubnub.NonSubscribeTimeout = value; }
        }

        public int SubscribeTimeout {
            get { return pubnub.SubscribeTimeout; }
            set { pubnub.SubscribeTimeout = value; }
        }

        public bool EnableResumeOnReconnect {
            get { return pubnub.EnableResumeOnReconnect; }
            set { pubnub.EnableResumeOnReconnect = value; }
        }

        public string SessionUUID {
            get { return pubnub.SessionUUID; }
            set { pubnub.SessionUUID = value; }
        }

        public string Origin {
            get { return pubnub.Origin; }
            set { pubnub.Origin = value; }
        }

        public int PresenceHeartbeat {
            get {
                return pubnub.PresenceHeartbeat;
            }
            set {
                pubnub.PresenceHeartbeat = value;
            }
        }

        public int PresenceHeartbeatInterval {
            get {
                return pubnub.PresenceHeartbeatInterval;
            }
            set {
                pubnub.PresenceHeartbeatInterval = value;
            }
        }

        public IPubnubUnitTest PubnubUnitTest {
            get {
                return pubnub.PubnubUnitTest;
            }
            set {
                pubnub.PubnubUnitTest = value;
            }
        }

        public bool EnableJsonEncodingForPublish {
            get {
                return pubnub.EnableJsonEncodingForPublish;
            }
            set {
                pubnub.EnableJsonEncodingForPublish = value;
            }
        }

        public IJsonPluggableLibrary JsonPluggableLibrary {
            get {
                return PubnubUnity.JsonPluggableLibrary;
            }
            set {
                PubnubUnity.JsonPluggableLibrary = value;
            }
        }

        public string Version{
            get {
                return PubnubUnity.Version;
            }
        }

        /// <summary>
        /// Gets or sets the set game object.
        /// This method should be called before init
        /// </summary>
        /// <value>The set game object.</value>
        public static GameObject SetGameObject {
            get {
                return PubnubUnity.SetGameObject;
            }
            set {
                PubnubUnity.SetGameObject = value;
            }
        }

        #endregion

        #region "Constructors"

        PubnubUnity pubnub;

        public Pubnub (string publishKey, string subscribeKey, string secretKey, string cipherKey, bool sslOn)
        {
            pubnub = new PubnubUnity (publishKey, subscribeKey, secretKey, cipherKey, sslOn);
        }

        public Pubnub (string publishKey, string subscribeKey, string secretKey)
        {
            pubnub = new PubnubUnity (publishKey, subscribeKey, secretKey);
        }

        public Pubnub (string publishKey, string subscribeKey)
        {
            pubnub = new PubnubUnity (publishKey, subscribeKey);
        }

        #endregion

    */
    }
}

