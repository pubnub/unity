//Build Date: May 24, 2016
//ver3.7/Unity5
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Net;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#if DEBUG
[assembly:InternalsVisibleTo("Assembly-CSharp-Editor")]
#endif

namespace PubNubMessaging.Core
{
    public class PubnubUnity
    {

        #region "Events"

        // Common property changed event
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged (string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) {
                handler (this, new PropertyChangedEventArgs (propertyName));
            }
        }

        #endregion

        #region "Class variables"

        private static GameObject gobj;
        private bool localGobj;
        private Counter publishMessageCounter;

        private CoroutineClass coroutine;

        private string origin = "pubsub.pubnub.com";
        private string publishKey = "";
        private string subscribeKey = "";
        private string secretKey = "";
        private string cipherKey = "";
        private bool ssl = true;
        private static long lastSubscribeTimetoken = 0;
        private static long lastSubscribeTimetokenForNewMultiplex = 0;
        private const string build = "3.7";
        private static string pnsdkVersion = "PubNub-CSharp-Unity5/3.7";

        private int pubnubWebRequestCallbackIntervalInSeconds = 310;
        private int pubnubOperationTimeoutIntervalInSeconds = 15;
        private int pubnubHeartbeatTimeoutIntervalInSeconds = 10;
        private int pubnubNetworkTcpCheckIntervalInSeconds = 15;
        private int pubnubNetworkCheckRetries = 50;
        private int pubnubWebRequestRetryIntervalInSeconds = 10;
        private int pubnubPresenceHeartbeatInSeconds = 0;
        private int presenceHeartbeatIntervalInSeconds = 0;
        private bool enableResumeOnReconnect = true;
        private bool uuidChanged = false;
        public bool overrideTcpKeepAlive = true;
        private bool enableJsonEncodingForPublish = true;
        private LoggingMethod.Level pubnubLogLevel = LoggingMethod.Level.Info;
        private PubnubErrorFilter.Level errorLevel = PubnubErrorFilter.Level.Info;
        bool resetTimetoken = false;

        //private SafeDictionary<string, SubscribeOptions> multiChannelSubscribe = new SafeDictionary<string, SubscribeOptions> ();
        //private SafeDictionary<string, PubnubWebRequest> channelRequest = new SafeDictionary<string, PubnubWebRequest> ();
        //private SafeDictionary<PubnubChannelCallbackKey, object> channelCallbacks = new SafeDictionary<PubnubChannelCallbackKey, object> ();
        /*private SafeDictionary<string, Dictionary<string, object>> channelLocalUserState = new SafeDictionary<string, Dictionary<string, object>> ();
        private SafeDictionary<string, Dictionary<string, object>> channelUserState = new SafeDictionary<string, Dictionary<string, object>> ();
        private SafeDictionary<string, Dictionary<string, object>> channelGroupLocalUserState = new SafeDictionary<string, Dictionary<string, object>> ();
        private SafeDictionary<string, Dictionary<string, object>> channelGroupUserState = new SafeDictionary<string, Dictionary<string, object>> ();*/
        private static bool pubnetSystemActive = true;

        private bool keepHearbeatRunning = false;
        private bool isHearbeatRunning = false;

        private bool keepPresenceHearbeatRunning = false;
        private bool isPresenceHearbeatRunning = false;

        private bool internetStatus = true;
        private bool retriesExceeded = false;

        private int retryCount = 0;

        #endregion

        #region "Properties"

        public string PublishKey{
            get{
                return publishKey;
            }
        }

        public string SubscribeKey{
            get{
                return subscribeKey;
            }
        }

        public string SecretKey{
            get{
                return secretKey;
            }
        }

        /// <summary>
        /// Gets or sets the set game object.
        /// This method should be called before init
        /// </summary>
        /// <value>The set game object.</value>
        public static GameObject SetGameObject {
            get {
                return gobj;
            }
            set {
                gobj = value;
            }
        }

        private static bool failClientNetworkForTesting = false;
        private static bool machineSuspendMode = false;

        private static bool SimulateNetworkFailForTesting {
            get {
                return failClientNetworkForTesting;
            }

            set {
                failClientNetworkForTesting = value;
            }
             
        }

        private static bool MachineSuspendMode {
            get {
                return machineSuspendMode;
            }
            set {
                machineSuspendMode = value;
            }
        }

        public static string Version {
            get {
                return pnsdkVersion;
            }
            set {
                pnsdkVersion = value;
            }
        }

        private List<object> history = new List<object> ();

        public List<object> History {
            get { return history; }
            set {
                history = value;
                RaisePropertyChanged ("History");
            }
        }

        public int SubscribeTimeout {
            get {
                return pubnubWebRequestCallbackIntervalInSeconds;
            }

            set {
                #if(UNITY_IOS)
                pubnubWebRequestCallbackIntervalInSeconds = Utility.CheckTimeoutValue(value);
                #else
                pubnubWebRequestCallbackIntervalInSeconds = value;
                #endif
            }
        }

        public int HeartbeatTimeout {
            get {
                return pubnubHeartbeatTimeoutIntervalInSeconds;
            }

            set {
                pubnubHeartbeatTimeoutIntervalInSeconds = value;
            }
        }

        public int NonSubscribeTimeout {
            get {
                return pubnubOperationTimeoutIntervalInSeconds;
            }

            set {
                #if(UNITY_IOS)
                pubnubOperationTimeoutIntervalInSeconds = Utility.CheckTimeoutValue(value);
                #else
                pubnubOperationTimeoutIntervalInSeconds = value;
                #endif
            }
        }

        public int NetworkCheckMaxRetries {
            get {
                return pubnubNetworkCheckRetries;
            }

            set {
                pubnubNetworkCheckRetries = value;
            }
        }

        public int NetworkCheckRetryInterval {
            get {
                return pubnubWebRequestRetryIntervalInSeconds;
            }

            set {
                pubnubWebRequestRetryIntervalInSeconds = value;
            }
        }

        public int LocalClientHeartbeatInterval {
            get {
                return pubnubNetworkTcpCheckIntervalInSeconds;
            }

            set {
                pubnubNetworkTcpCheckIntervalInSeconds = value;
            }
        }

        public bool EnableResumeOnReconnect {
            get {
                return enableResumeOnReconnect;
            }
            set {
                enableResumeOnReconnect = value;
            }
        }

        public bool EnableJsonEncodingForPublish {
            get {
                return enableJsonEncodingForPublish;
            }
            set {
                enableJsonEncodingForPublish = value;
            }
        }

        private string authenticationKey = "";

        public string AuthenticationKey {
            get {
                return authenticationKey;
            }

            set {
                authenticationKey = value;
            }
        }

        private IPubnubUnitTest pubnubUnitTest;

        public virtual IPubnubUnitTest PubnubUnitTest {
            get {
                return pubnubUnitTest;
            }
            set {
                pubnubUnitTest = value;
            }
        }

        private static IJsonPluggableLibrary jsonPluggableLibrary = null;

        public static IJsonPluggableLibrary JsonPluggableLibrary {
            get {
                if (jsonPluggableLibrary == null)
                {
                    jsonPluggableLibrary = JSONSerializer.JsonPluggableLibrary;
                }
                return jsonPluggableLibrary;
            }

            set {
                if (value is IJsonPluggableLibrary) {
                    jsonPluggableLibrary = value;
                } else {
                    jsonPluggableLibrary = null;
                    throw new ArgumentException ("Missing or Incorrect JsonPluggableLibrary value");
                }
            }
        }

        public string Origin {
            get {
                return origin;
            }

            set {
                origin = value;
            }
        }

        private string sessionUUID = "";

        public string SessionUUID {
            get {
                if (string.IsNullOrEmpty (sessionUUID) || string.IsNullOrEmpty (sessionUUID.Trim ())) {
                    sessionUUID = Guid.NewGuid ().ToString ();
                }

                return sessionUUID;
            }
            set {
                sessionUUID = value;
            }
        }

        /// <summary>
        /// This property sets presence expiry timeout.
        /// Presence expiry value in seconds.
        /// </summary>
        public int PresenceHeartbeat {
            get {
                return pubnubPresenceHeartbeatInSeconds;
            }

            set {
                if (value <= 0 || value > 320) {
                    pubnubPresenceHeartbeatInSeconds = 0;
                } else {
                    pubnubPresenceHeartbeatInSeconds = value;
                }
                if (pubnubPresenceHeartbeatInSeconds != 0) {
                    presenceHeartbeatIntervalInSeconds = (pubnubPresenceHeartbeatInSeconds / 2) - 1;
                }
                TerminateCurrentSubscriberRequest();
            }
        }

        public int PresenceHeartbeatInterval {
            get {
                return presenceHeartbeatIntervalInSeconds;
            }

            set {
                presenceHeartbeatIntervalInSeconds = value;
                if (presenceHeartbeatIntervalInSeconds >= pubnubPresenceHeartbeatInSeconds) {
                    presenceHeartbeatIntervalInSeconds = (pubnubPresenceHeartbeatInSeconds / 2) - 1;
                }
            }
        }

        public LoggingMethod.Level PubnubLogLevel {
            get {
                return pubnubLogLevel;
            }

            set {
                pubnubLogLevel = value;
                LoggingMethod.LogLevel = pubnubLogLevel;
            }
        }

        public PubnubErrorFilter.Level PubnubErrorLevel {
            get {
                return errorLevel;
            }

            set {
                errorLevel = value;
                PubnubErrorFilter.ErrorLevel = errorLevel;
            }
        }

        public object FilterExpr{ get; set;}
        public string Region{ get; set;}

        #endregion

        #region "Constructors"

        public PubnubUnity (string publishKey, string subscribeKey, string secretKey, string cipherKey, bool sslOn)
        {
            this.Init (publishKey, subscribeKey, secretKey, cipherKey, sslOn);
        }

        public PubnubUnity (string publishKey, string subscribeKey, string secretKey)
        {
            this.Init (publishKey, subscribeKey, secretKey, "", false);
        }

        public PubnubUnity (string publishKey, string subscribeKey)
        {
            this.Init (publishKey, subscribeKey, "", "", false);
        }

        #endregion

        #region "Init"

        private void Init (string publishKey, string subscribeKey, string secretKey, string cipherKey, bool sslOn)
        {
            LoggingMethod.LogLevel = pubnubLogLevel;
            PubnubErrorFilter.ErrorLevel = errorLevel;

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
            Version = string.Format("PubNub-CSharp-Unity5/{0}", build);
            #endif
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (Version, LoggingMethod.LevelInfo);
            #endif

            if (gobj == null) {
            #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog ("Initilizing new GameObject", LoggingMethod.LevelInfo);
            #endif
                gobj = new GameObject ();  
                localGobj = true;
            } else {
            #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog ("Reusing already initialized GameObject", LoggingMethod.LevelInfo);
            #endif
                localGobj = false;
            }

            coroutine = gobj.AddComponent<CoroutineClass> ();    
            coroutine.subscribeTimer = SubscribeTimeout;
            coroutine.nonSubscribeTimer = NonSubscribeTimeout;
            coroutine.heartbeatTimer = HeartbeatTimeout;
            coroutine.presenceHeartbeatTimer = HeartbeatTimeout;
            coroutine.heartbeatPauseTimer = NetworkCheckRetryInterval;
            coroutine.presenceHeartbeatPauseTimer = NetworkCheckRetryInterval;

            this.publishKey = publishKey;
            this.subscribeKey = subscribeKey;
            this.secretKey = secretKey;
            this.cipherKey = cipherKey;
            this.ssl = sslOn;

            retriesExceeded = false;
            internetStatus = true;

            publishMessageCounter = new Counter ();

            #if(UNITY_ANDROID || UNITY_STANDALONE || UNITY_IOS)
            ServicePointManager.ServerCertificateValidationCallback = ValidatorUnity;
            #endif

        }

        #endregion

        #region "ValidatorUnity"

        #if(UNITY_ANDROID || MONOTOUCH || __IOS__||UNITY_STANDALONE || UNITY_IOS)
        /// <summary>
        /// Workaround for the bug described here 
        /// https://bugzilla.xamarin.com/show_bug.cgi?id=6501
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="certificate">Certificate.</param>
        /// <param name="chain">Chain.</param>
        /// <param name="sslPolicyErrors">Ssl policy errors.</param>
        static bool ValidatorUnity (object sender,
                                          System.Security.Cryptography.X509Certificates.X509Certificate
            certificate,
                                          System.Security.Cryptography.X509Certificates.X509Chain chain,
                                          System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        #endif
        #endregion

        #region "PubNub API Channel Methods"

        #region "Subscribe"

        public void Subscribe<T> (string channel, string channelGroup, long timetoken, Action<T> userCallback, Action<T> connectCallback, 
            Action<T> wildcardPresenceCallback, Action<PubnubClientError> errorCallback)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, requested subscribe for channel={1}", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);
            #endif

            MultiChannelSubscribeInit<T> (ResponseType.SubscribeV2, channel, channelGroup, timetoken, userCallback, connectCallback, 
                errorCallback, wildcardPresenceCallback);
        }

        #endregion

        #region "Publish"

        public bool Publish<T> (string channel, object message, bool storeInHistory, object metadata,
            Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            string jsonMessage = (enableJsonEncodingForPublish) ? Helpers.JsonEncodePublishMsg (message, this.cipherKey, JsonPluggableLibrary) : message.ToString ();
            string jsonMetadata = (enableJsonEncodingForPublish) ? Helpers.JsonEncodePublishMsg (metadata, this.cipherKey, JsonPluggableLibrary) : message.ToString ();

            List<ChannelEntity> channelEntity = Helpers.CreateChannelEntity (new string[] {channel}, false, false, null, userCallback, null, errorCallback, null, null);

            Uri request = BuildRequests.BuildPublishRequest (channel, jsonMessage, storeInHistory, this.SessionUUID,
                this.ssl, this.Origin, this.AuthenticationKey, this.publishKey, this.subscribeKey, this.cipherKey, 
                this.secretKey, jsonMetadata, this.publishMessageCounter.NextValue());

            //RequestState<T> requestState = BuildRequests.BuildRequestState<T> (new string[] { channel }, null, ResponseType.Publish, 
            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntity, ResponseType.Publish, 
                false, 0, false, 0, null);

            return UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "Presence"

        public void Presence<T> (string channel, string channelGroup, long timetoken, Action<T> userCallback, Action<T> connectCallback, 
            Action<PubnubClientError> errorCallback)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, requested presence for channel={1}", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);
            #endif

            MultiChannelSubscribeInit<T> (ResponseType.PresenceV2, channel, channelGroup, timetoken, userCallback, connectCallback, errorCallback, null);
        }

        #endregion

        #region "Detailed History"

        public bool DetailedHistory<T> (string channel, long start, long end, int count, bool reverse, bool includeToken, 
            Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            string[] channels = new string[] { channel };
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, channels={1}, channel={2}", DateTime.Now.ToString (), channels.Length, channel), LoggingMethod.LevelInfo);
            #endif

            List<ChannelEntity> channelEntity = Helpers.CreateChannelEntity<T> (channels, false, false, null, userCallback, null, errorCallback, null, null);

            Uri request = BuildRequests.BuildDetailedHistoryRequest (channel, start, end, count, reverse, includeToken, this.SessionUUID,
                this.ssl, this.Origin, this.AuthenticationKey, this.subscribeKey);

            //RequestState<T> requestState = BuildRequests.BuildRequestState<T> (new string[] { channel }, null, ResponseType.DetailedHistory, false, 
            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntity, ResponseType.DetailedHistory, false, 
                0, false, 0, null);

            return UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "HereNow"

        public bool HereNow<T> (string channel, string channelGroup, bool showUUIDList, bool includeUserState, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            List<ChannelEntity> channelEntity = Helpers.CreateChannelEntity (new string[] {channel}, false, false, null, userCallback, null, errorCallback, null, null);

            List<ChannelEntity> channelGroupEntity = Helpers.CreateChannelEntity (new string[] {channelGroup}, false, true, null, userCallback, null, errorCallback, null, null);

            channelEntity.AddRange (channelGroupEntity);

            Uri request = BuildRequests.BuildHereNowRequest (channel, channelGroup, showUUIDList, includeUserState, this.SessionUUID,
                this.ssl, this.Origin, this.AuthenticationKey, this.subscribeKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntity, ResponseType.HereNow, false, 
                0, false, 0, null);

            return UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "Global Here Now"

        public bool GlobalHereNow<T> (bool showUUIDList, bool includeUserState, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildGlobalHereNowRequest (showUUIDList, includeUserState, this.SessionUUID,
                this.ssl, this.Origin, this.AuthenticationKey, this.subscribeKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (userCallback, errorCallback,
                ResponseType.GlobalHereNow, false, 0, false, 0, null, "");

            return UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "WhereNow"

        public void WhereNow<T> (string uuid, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            if (string.IsNullOrEmpty (uuid)) {
                uuid = this.SessionUUID;
            }
            Uri request = BuildRequests.BuildWhereNowRequest (uuid, this.SessionUUID,
                this.ssl, this.Origin, this.AuthenticationKey, this.subscribeKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (userCallback, errorCallback, ResponseType.WhereNow, false, 
                0, false, 0, null, uuid);

            UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "Unsubscribe Presence And Subscribe"

        public void PresenceUnsubscribe<T> (string channel, string channelGroup, Action<T> userCallback, Action<T> connectCallback, Action<T> disconnectCallback, 
            Action<PubnubClientError> errorCallback)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, requested presence-unsubscribe for channel(s)={1}", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);
            #endif
            MultiChannelUnsubscribeInit<T> (ResponseType.PresenceUnsubscribe, channel, channelGroup, userCallback, 
                connectCallback, disconnectCallback, errorCallback);
        }

        /// <summary>
        /// To unsubscribe a channel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="userCallback"></param>
        /// <param name="connectCallback"></param>
        /// <param name="disconnectCallback"></param>
        /// <param name="errorCallback"></param>
        public void Unsubscribe<T> (string channel, string channelGroup, Action<T> userCallback, Action<T> connectCallback, Action<T> disconnectCallback, 
            Action<T> wildcardPresenceCallback, Action<PubnubClientError> errorCallback)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, requested unsubscribe for channel(s)={1}", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);
            #endif
            MultiChannelUnsubscribeInit<T> (ResponseType.Unsubscribe, channel, channelGroup, userCallback, 
                connectCallback, disconnectCallback, errorCallback);

        }

        #endregion

        #region "Time"

        public bool Time<T> (Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildTimeRequest (this.SessionUUID, this.ssl, this.Origin);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (userCallback, errorCallback, ResponseType.Time, false, 
                0, false, 0, null, "");
            return UrlProcessRequest<T> (request, requestState); 
        }

        #endregion

        #region "PAM"

        #region "Grant Access"

        public bool GrantAccess<T> (string channel, string authenticationKey, bool read, bool write, int ttl, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            List<ChannelEntity> channelEntity = Helpers.CreateChannelEntity (new string[] {channel}, false, false, null, 
                userCallback, null, errorCallback, null, null);

            Uri request = BuildRequests.BuildGrantAccessRequest (channel, read, write, ttl, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.publishKey, this.subscribeKey, this.cipherKey, this.secretKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntity, ResponseType.GrantAccess, false,
                0, false, 0, null);

            return UrlProcessRequest<T> (request, requestState); 
        }

        #endregion

        #region "Grant Presence Access"

        public bool GrantPresenceAccess<T> (string channel, string authenticationKey, bool read, bool write, int ttl, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            string[] multiChannels = channel.Split (',');
            if (multiChannels.Length > 0) {
                for (int index = 0; index < multiChannels.Length; index++) {
                    if (!string.IsNullOrEmpty (multiChannels [index]) && multiChannels [index].Trim ().Length > 0) {
                        multiChannels [index] = string.Format ("{0}{1}", multiChannels [index], Utility.PresenceChannelSuffix);
                    } else {
                        throw new MissingMemberException ("Invalid channel");
                    }
                }
            }
            string presenceChannel = string.Join (",", multiChannels);
            return GrantAccess (presenceChannel, authenticationKey, read, write, ttl, userCallback, errorCallback);
        }

        #endregion

        #region "Audit Access"

        public void AuditAccess<T> (string channel, string authenticationKey, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildAuditAccessRequest (channel, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.publishKey, this.subscribeKey, this.cipherKey, this.secretKey);

            RequestState<T> requestState = null;
            if (string.IsNullOrEmpty (channel)) {
                requestState = BuildRequests.BuildRequestState<T> (userCallback, errorCallback, 
                    ResponseType.AuditAccess, false, 0, false, 0, null, "");
            } else {
                List<ChannelEntity> channelEntity = Helpers.CreateChannelEntity (new string[] {channel}, false, false, null, 
                    userCallback, null, errorCallback, null, null);

                requestState = BuildRequests.BuildRequestState<T> (channelEntity, 
                     ResponseType.AuditAccess, false, 0, false, 0, null);
            }

            UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "Audit Presence"

        public void AuditPresenceAccess<T> (string channel, string authenticationKey, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            string[] multiChannels = channel.Split (',');
            if (multiChannels.Length > 0) {
                for (int index = 0; index < multiChannels.Length; index++) {
                    multiChannels [index] = string.Format ("{0}{1}", multiChannels [index], Utility.PresenceChannelSuffix);
                }
            }
            string presenceChannel = string.Join (",", multiChannels);
            AuditAccess<T> (presenceChannel, authenticationKey, userCallback, errorCallback);
        }

        #endregion

        #endregion

        #region "Set User State"

        public void SetUserState<T> (string channel, string channelGroup, string uuid, string jsonUserState, 
            Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            if (!JsonPluggableLibrary.IsDictionaryCompatible (jsonUserState)) {
                throw new MissingMemberException ("Missing json format for user state");
            } else {
                Dictionary<string, object> deserializeUserState = JsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonUserState);
                if (deserializeUserState == null) {
                    throw new MissingMemberException ("Missing json format user state");
                } else {
                    string userState = "";
                    List<ChannelEntity> channelEntities;
                    if (Helpers.CheckAndAddExistingUserState<T> (channel, channelGroup, deserializeUserState, errorCallback, errorLevel,
                        out userState, out channelEntities
                    )) {
                        SharedSetUserState<T> (channel, channelGroup,
                            channelEntities, uuid, userState);
                    }
                }
            }
        }

        public void SetUserState<T> (string channel, string channelGroup, string uuid, KeyValuePair<string, object> keyValuePair, 
            Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            string userState = "";
            List<ChannelEntity> channelEntities;
            if (Helpers.CheckAndAddExistingUserState<T> (channel, channelGroup, 
                new Dictionary<string, object> { { keyValuePair.Key, keyValuePair.Value } }, errorCallback, errorLevel, out userState, out channelEntities
            )) {

                SharedSetUserState<T> (channel, channelGroup, channelEntities, uuid, userState);
            }
        }

        #endregion

        #region "Get User State"

        public void GetUserState<T> (string channel, string channelGroup, string uuid, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            if (string.IsNullOrEmpty (uuid)) {
                uuid = this.SessionUUID;
            }

            List<ChannelEntity> channelEntity = Helpers.CreateChannelEntity (new string[] {channel}, false, false, null, 
                userCallback, null, errorCallback, null, null);

            List<ChannelEntity> channelGroupsEntity = Helpers.CreateChannelEntity (new string[] {channelGroup}, false, true, null, 
                userCallback, null, errorCallback, null, null);

            channelEntity.AddRange (channelGroupsEntity);

            Uri request = BuildRequests.BuildGetUserStateRequest (channel, channelGroup, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.subscribeKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelGroupsEntity, ResponseType.GetUserState, false, 
                0, false, 0, null);

            UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "MobilePush"

        public void RegisterDeviceForPush<T>(string channel, PushTypeService pushType, string pushToken, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildRegisterDevicePushRequest(channel, pushType, pushToken, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.subscribeKey);

            List<ChannelEntity> channelEntity = Helpers.CreateChannelEntity (new string[] {channel}, false, false, null, 
                userCallback, null, errorCallback, null, null);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntity, 
                ResponseType.PushRegister, false, 0, false, 0, null);

            UrlProcessRequest<T>(request, requestState);
        }

        public void UnregisterDeviceForPush<T>(PushTypeService pushType, string pushToken, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildUnregisterDevicePushRequest(pushType, pushToken, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.subscribeKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (userCallback, errorCallback, 
                ResponseType.PushUnregister, false, 0, false, 0, null, "");
            
            UrlProcessRequest<T>(request, requestState);
        }

        public void RemoveChannelForDevicePush<T>(string channel, PushTypeService pushType, string pushToken, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildRemoveChannelPushRequest(channel, pushType, pushToken, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.subscribeKey);

            List<ChannelEntity> channelEntity = Helpers.CreateChannelEntity (new string[] {channel}, false, false, null, 
                userCallback, null, errorCallback, null, null);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntity, 
                ResponseType.PushRemove, false, 0, false, 0, null);
            
            UrlProcessRequest<T>(request, requestState);
        }

        public void GetChannelsForDevicePush<T>(PushTypeService pushType, string pushToken, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildGetChannelsPushRequest(pushType, pushToken, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.subscribeKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (userCallback, errorCallback,
                ResponseType.PushGet, false, 0, false, 0, null, "");
            
            UrlProcessRequest<T>(request, requestState);
        }

        #endregion

        #region "Channel Groups"
        public void AddChannelsToChannelGroup<T>(string[] channels, string nameSpace, string groupName, 
            Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildAddChannelsToChannelGroupRequest(channels, nameSpace, groupName, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.subscribeKey);

            //List<ChannelEntity> channelEntity = Helpers.CreateChannelEntity (channels, false, false, null, 
              //  userCallback, null, errorCallback, null, null);

            List<ChannelEntity> channelGroupEntity = Helpers.CreateChannelEntity (new string[] {groupName}, false, true, null, 
                userCallback, null, errorCallback, null, null);

            //channelEntity.AddRange (channelGroupEntity);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelGroupEntity, 
                ResponseType.ChannelGroupAdd, false, 0, false, 0, null 
                 
            );

            UrlProcessRequest<T>(request, requestState);
        }

        public void RemoveChannelsFromChannelGroup<T>(string[] channels, string nameSpace, string groupName, 
            Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildRemoveChannelsFromChannelGroupRequest(channels, nameSpace, groupName, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.subscribeKey);

            //List<ChannelEntity> channelEntity = Helpers.CreateChannelEntity (channels, false, false, null, 
              //  userCallback, null, errorCallback, null, null);

            List<ChannelEntity> channelGroupEntity = Helpers.CreateChannelEntity (new string[] {groupName}, false, true, null, 
                userCallback, null, errorCallback, null, null);

            //channelEntity.AddRange (channelGroupEntity);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelGroupEntity, 
                ResponseType.ChannelGroupRemove, false, 0, false, 0, null
            );

            UrlProcessRequest<T>(request, requestState);
        }

        public void RemoveChannelGroup<T>(string nameSpace, string groupName, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildRemoveChannelsFromChannelGroupRequest(null, nameSpace, groupName, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.subscribeKey);

            List<ChannelEntity> channelGroupEntity = Helpers.CreateChannelEntity (new string[] {groupName}, false, true, null, 
                userCallback, null, errorCallback, null, null);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelGroupEntity,
                ResponseType.ChannelGroupRemove, false, 0, false, 0, null 
            );

            UrlProcessRequest<T>(request, requestState);
        }

        public void GetChannelsForChannelGroup<T>(string nameSpace, string groupName, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildGetChannelsForChannelGroupRequest(nameSpace, groupName, false, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.subscribeKey);

            List<ChannelEntity> channelGroupEntity = Helpers.CreateChannelEntity (new string[] {groupName}, false, true, null, 
                userCallback, null, errorCallback, null, null);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelGroupEntity,
                ResponseType.ChannelGroupGet, false, 0, false, 0, null 
                );

            UrlProcessRequest<T>(request, requestState);
        }

        public void GetAllChannelGroups<T>(string nameSpace, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildGetChannelsForChannelGroupRequest(nameSpace, "", true, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.subscribeKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (userCallback, errorCallback,
                ResponseType.ChannelGroupGet, false, 0, false, 0, null, 
                ""
            );

            UrlProcessRequest<T>(request, requestState);
        }

        public void ChannelGroupAuditAccess<T>(string channelGroup, string authenticationKey, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildChannelGroupAuditAccessRequest (channelGroup, this.SessionUUID,
            this.ssl, this.Origin, authenticationKey, this.publishKey, this.subscribeKey, this.cipherKey, this.secretKey);

            List<ChannelEntity> channelGroupEntity = Helpers.CreateChannelEntity (new string[] {channelGroup}, false, true, null, 
                userCallback, null, errorCallback, null, null);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelGroupEntity, 
                ResponseType.ChannelGroupAuditAccess, false, 0, false, 0, null 
            );

            UrlProcessRequest<T> (request, requestState);
        }

        public bool ChannelGroupGrantAccess<T>(string channelGroup, string authenticationKey, bool read, bool write, bool manage, 
            int ttl, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildChannelGroupGrantAccessRequest(channelGroup, read, write, manage, ttl, this.SessionUUID,
            this.ssl, this.Origin, authenticationKey, this.publishKey, this.subscribeKey, this.cipherKey, this.secretKey);

            List<ChannelEntity> channelGroupEntity = Helpers.CreateChannelEntity (new string[] {channelGroup}, false, true, null, 
                userCallback, null, errorCallback, null, null);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelGroupEntity, 
                ResponseType.ChannelGroupGrantAccess, false, 0, false, 0, null
            );

            return UrlProcessRequest<T>(request, requestState);
        }

        #endregion         

        #region "PubNub API Other Methods"

        public void TerminateCurrentSubscriberRequest ()
        {
            TerminateCurrentSubscriberRequest<object> ();
        }

        public void TerminateCurrentSubscriberRequest<T> ()
        {
            StopHeartbeat<T> ();
            StopPresenceHeartbeat<T> ();
            RequestState<T> reqState = StoredRequestState.Instance.GetStoredRequestState (CurrentRequestType.Subscribe) as RequestState<T>;
            coroutine.BounceRequest<T> (CurrentRequestType.Subscribe, reqState, true);
        }

        public void EndPendingRequests<T> ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} ending open requests.", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            RequestState<T> reqStateSub = StoredRequestState.Instance.GetStoredRequestState (CurrentRequestType.Subscribe) as RequestState<T>;
            coroutine.BounceRequest<T> (CurrentRequestType.Subscribe, reqStateSub, false);
            RequestState<T> reqStateNonSub = StoredRequestState.Instance.GetStoredRequestState (CurrentRequestType.NonSubscribe) as RequestState<T>;
            coroutine.BounceRequest<T> (CurrentRequestType.NonSubscribe, reqStateNonSub, false);
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Request bounced.", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            StopHeartbeat ();
            StopPresenceHeartbeat ();
            //ClearChannelCallback ();
            //RemoveUserState ();
            /*#if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} ClearChannelCallback and RemoveUserState complete.", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif*/
            //ClearChannelRequest();
            //ClearMultiChannelSubscribe();
            Subscription.Instance.CleanUp();
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Subscription cleanup complete.", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
        }

        public void ResetPublishMessageCounter (){
            publishMessageCounter.Reset ();
        }

        public void CleanUp (){
            publishMessageCounter.Reset ();
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog ("Destructing coroutine", LoggingMethod.LevelInfo);
            #endif
            if (coroutine != null) {
                UnityEngine.Object.Destroy (coroutine);
            }
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog ("Destructing GameObject", LoggingMethod.LevelInfo);
            #endif
            if(localGobj && (gobj != null))
            {
                UnityEngine.Object.Destroy (gobj);
            }
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Clean up complete.", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
        }

        /*private void ClearChannelRequest ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Clear ChannelRequest", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            channelRequest.Clear ();
        }*/

        /*private void ClearMultiChannelSubscribe ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Clear MultiChannelSubscribe", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif

            multiChannelSubscribe.Clear ();
        }*/

        /*private void ClearChannelCallback ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Clear ChannelCallback", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            channelCallbacks.Clear ();
        }*/

        /*private void RemoveUserState ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} RemoveUserState from user state dictionary", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            channelLocalUserState.Clear ();
            channelUserState.Clear ();
        }*/

        #endregion

        #region "Change UUID"

        public void ChangeUUID<T> (string newUUID)
        {
            if (string.IsNullOrEmpty (newUUID) || this.SessionUUID == newUUID) {
                return;
            }

            uuidChanged = true;

            string oldUUID = SessionUUID;

            SessionUUID = newUUID;

            if (Subscription.Instance.HasChannelsOrChannelGroups) {
                Uri request = BuildRequests.BuildMultiChannelLeaveRequest (Helpers.GetNamesFromChannelEntities(Subscription.Instance.AllChannels, false), 
                    Helpers.GetNamesFromChannelEntities(Subscription.Instance.AllChannelGroups, true), oldUUID,
                    this.ssl, this.Origin, authenticationKey, this.subscribeKey);

                RequestState<T> requestState = BuildRequests.BuildRequestState<T> (Subscription.Instance.AllSubscribedChannelsAndChannelGroups, 
                    ResponseType.Leave, false, 0, false, 0, null);

                UrlProcessRequest<T> (request, requestState); 
            }

            TerminateCurrentSubscriberRequest<T> ();

        }
    
        #endregion

        #endregion

        #region "Heartbeats"

        void StopHeartbeat ()
        {
            StopHeartbeat<object> ();
        }

        void StopHeartbeat<T> ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Stopping Heartbeat ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            keepHearbeatRunning = false;
            isHearbeatRunning = false;
            coroutine.HeartbeatCoroutineComplete -= CoroutineCompleteHandler<T>;
            coroutine.BounceRequest<T> (CurrentRequestType.Heartbeat, null, false);
        }

        void StopPresenceHeartbeat ()
        {
            StopPresenceHeartbeat<object> ();
        }

        void StopPresenceHeartbeat<T> ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Stopping PresenceHeartbeat ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            keepPresenceHearbeatRunning = false;
            isPresenceHearbeatRunning = false;
            coroutine.PresenceHeartbeatCoroutineComplete -= CoroutineCompleteHandler<T>;
            coroutine.BounceRequest<T> (CurrentRequestType.PresenceHeartbeat, null, false);
        }

        void StartPresenceHeartbeat<T> (bool pause, int pauseTime, RequestState<T> pubnubRequestState)
        {
            try {
                //string[] channels = multiChannelSubscribe.Keys.ToArray<string> ();
                //if (channels != null && channels.Length > 0) {
                    //channels = channels.Where (s => s.Contains (Utility.PresenceChannelSuffix) == false).ToArray ();

                    //if (channels != null && channels.Length > 0) {
            if(Subscription.Instance.AllPresenceChannelsOrChannelGroups.Count > 0){
                        isPresenceHearbeatRunning = true;
                    string channelsJsonState = Subscription.Instance.CompiledUserState;

                    Uri requestUrl = BuildRequests.BuildPresenceHeartbeatRequest (Helpers.GetNamesFromChannelEntities(Subscription.Instance.AllChannels, false), 
                        Helpers.GetNamesFromChannelEntities(Subscription.Instance.AllChannelGroups, true), channelsJsonState, this.SessionUUID,
                            this.ssl, this.Origin, authenticationKey, this.subscribeKey);

                        coroutine.PresenceHeartbeatCoroutineComplete += CoroutineCompleteHandler<T>;

                        //for heartbeat and presence heartbeat treat reconnect as pause
            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (pubnubRequestState.ChannelEntities, ResponseType.PresenceHeartbeat, 
                        pause, pubnubRequestState.ID, false, 0, null);
                        StoredRequestState.Instance.SetRequestState (CurrentRequestType.PresenceHeartbeat, requestState);
                        coroutine.Run<T> (requestUrl.OriginalString, requestState, HeartbeatTimeout, pauseTime);
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PresenceHeartbeat running for {1}", DateTime.Now.ToString (), pubnubRequestState.ID), LoggingMethod.LevelInfo);
                        #endif
                    //}
            }
            }
            catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PresenceHeartbeat exception {1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
            }
        }

        void RunPresenceHeartbeat<T> (bool pause, int pauseTime, RequestState<T> pubnubRequestState)
        {
            keepPresenceHearbeatRunning = true;
            if (!isPresenceHearbeatRunning) {
                StartPresenceHeartbeat<T> (pause, pauseTime, pubnubRequestState);
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PresenceHeartbeat Running ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            }
            #endif
        }

        void StartHeartbeat<T> (bool pause, int pauseTime, RequestState<T> pubnubRequestState)
        {
            try {
                isHearbeatRunning = true;
                Uri requestUrl = BuildRequests.BuildTimeRequest (this.SessionUUID,
                    this.ssl, this.Origin);

                coroutine.HeartbeatCoroutineComplete += CoroutineCompleteHandler<T>;
                //for heartbeat and presence heartbeat treat reconnect as pause
                RequestState<T> requestState = BuildRequests.BuildRequestState<T> (pubnubRequestState.ChannelEntities, 
                    ResponseType.Heartbeat, pause, pubnubRequestState.ID, false, 0, null);
                StoredRequestState.Instance.SetRequestState (CurrentRequestType.Heartbeat, requestState);
                coroutine.Run<T> (requestUrl.OriginalString, requestState, HeartbeatTimeout, pauseTime);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Heartbeat running for {1}", DateTime.Now.ToString (), pubnubRequestState.ID), LoggingMethod.LevelInfo);
                #endif
            }
            catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Heartbeat exception {1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
            }
        }

        void RunHeartbeat<T> (bool pause, int pauseTime, RequestState<T> pubnubRequestState)
        {
            keepHearbeatRunning = true;
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, In Runheartbeat {1}", DateTime.Now.ToString (), isHearbeatRunning.ToString ()), LoggingMethod.LevelInfo);
            #endif
            if (!isHearbeatRunning) {
                StartHeartbeat<T> (pause, pauseTime, pubnubRequestState);
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Heartbeat Running ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            }
            #endif
        }

        #endregion

        #region "Handlers"
        private void InternetConnectionAvailableHandler<T>(CustomEventArgs<T> cea){
            internetStatus = true;
            retriesExceeded = false;
            if (retryCount > 0) {
                string cbMessage = string.Format ("DateTime {0} Internet Connection Available.", DateTime.Now.ToString ());
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (cbMessage, LoggingMethod.LevelInfo);
                #endif

                PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (cbMessage, cea.PubnubRequestState, 
                    PubnubErrorSeverity.Info, PubnubErrorCode.YesInternet, PubnubErrorLevel);

                MultiplexExceptionHandler<T> (ResponseType.Subscribe, false, true);
            }
            retryCount = 0;
        }

        private void HeartbeatHandler<T> (CustomEventArgs<T> cea){
            if (cea.IsTimeout || cea.IsError) {
                RetryLoop<T> (cea.PubnubRequestState);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} Heartbeat timeout={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
                #endif
            } else {
                InternetConnectionAvailableHandler<T> (cea);
            }
            isHearbeatRunning = false;
            if (keepHearbeatRunning) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Restarting Heartbeat {1}", DateTime.Now.ToString (), cea.PubnubRequestState.ID), LoggingMethod.LevelInfo);
                #endif
                if (internetStatus) {
                    RunHeartbeat<T> (true, LocalClientHeartbeatInterval, cea.PubnubRequestState);
                }
                else {
                    RunHeartbeat<T> (true, NetworkCheckRetryInterval, cea.PubnubRequestState);
                }
            }
        }

        private void PresenceHeartbeatHandler<T> (CustomEventArgs<T> cea){
            isPresenceHearbeatRunning = false;

            #if (ENABLE_PUBNUB_LOGGING)
            if (cea.IsTimeout || cea.IsError) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} Presence Heartbeat timeout={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
            }else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} Presence Heartbeat response: {1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelInfo);
            }
            #endif

            if (keepPresenceHearbeatRunning) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Restarting PresenceHeartbeat ID {1}", DateTime.Now.ToString (), cea.PubnubRequestState.ID), LoggingMethod.LevelInfo);
                #endif
                RunPresenceHeartbeat<T> (true, PresenceHeartbeatInterval, cea.PubnubRequestState);
            }
        }

        private void SubscribePresenceHanlder<T> (CustomEventArgs<T> cea){
            #if (ENABLE_PUBNUB_LOGGING)
            if (cea.IsTimeout || Utility.CheckRequestTimeoutMessageInError (cea)) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} Sub timeout={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
            } else if (cea.IsError) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} Sub Error={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
            } else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} Sub Message={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelInfo);
            }
            #endif

            UrlProcessResponseCallbackNonAsync<T> (cea);
        }

        private void NonSubscribeHandler<T> (CustomEventArgs<T> cea){
            if (cea.IsTimeout || Utility.CheckRequestTimeoutMessageInError (cea)) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} NonSub timeout={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
                #endif
                ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (cea.Message.ToString (), cea.PubnubRequestState, true,
                    false, PubnubErrorLevel);
            } else if (cea.IsError) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} NonSub Error={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
                #endif
                ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (cea.Message.ToString (), cea.PubnubRequestState, false, false, PubnubErrorLevel);
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} NonSub Message={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelInfo);
                #endif
                /*var result = Helpers.WrapResultBasedOnResponseType<T> (cea.PubnubRequestState.RespType, cea.Message, 
                    cea.PubnubRequestState.Channels, cea.PubnubRequestState.ErrorCallback, channelCallbacks, 
                    JsonPluggableLibrary, PubnubErrorLevel, this.cipherKey);*/

                List<object> result = new List<object>();
                Helpers.WrapResultBasedOnResponseType<T> (cea.PubnubRequestState, cea.Message, 
                    JsonPluggableLibrary, PubnubErrorLevel, this.cipherKey, ref result);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} result={1}", DateTime.Now.ToString (), (result!=null)?result.Count.ToString():"null"), LoggingMethod.LevelInfo);
                #endif

                Helpers.ProcessResponseCallbacks<T> (ref result, cea.PubnubRequestState, 
                    this.cipherKey, JsonPluggableLibrary);
            }
        }

        private void ProcessCoroutineCompleteResponse<T> (CustomEventArgs<T> cea)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, In handler of event cea {1} RequestType CoroutineCompleteHandler {2}", DateTime.Now.ToString (), cea.PubnubRequestState.RespType.ToString (), typeof(T)), LoggingMethod.LevelInfo);
            #endif
            switch (cea.PubnubRequestState.RespType) {
            case ResponseType.Heartbeat:

                HeartbeatHandler<T> (cea);

                break;

            case ResponseType.PresenceHeartbeat:

                PresenceHeartbeatHandler<T> (cea);

                break;
            case ResponseType.Subscribe:
            case ResponseType.Presence:
            case ResponseType.SubscribeV2:
            case ResponseType.PresenceV2:

                SubscribePresenceHanlder<T> (cea);

                break;
            default:

                NonSubscribeHandler<T> (cea);

                break;
            }
        }

        private void CoroutineCompleteHandler<T> (object sender, EventArgs ea)
        {
            CustomEventArgs<T> cea = ea as CustomEventArgs<T>;

            try {
                if (cea != null) {
                    if (cea.PubnubRequestState != null) {
                        ProcessCoroutineCompleteResponse<T> (cea);
                    } 
                    #if (ENABLE_PUBNUB_LOGGING)
                    else {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0} PubnubRequestState null", DateTime.Now.ToString ()), LoggingMethod.LevelError);
                    }
                    #endif
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0} cea null", DateTime.Now.ToString ()), LoggingMethod.LevelError);
                }
                #endif
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} Exception={1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif

                ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, cea.PubnubRequestState, 
                    false, false, PubnubErrorLevel);
            } 
        }

        void ResponseCallbackNonErrorHandler<T> (CustomEventArgs<T> cea, RequestState<T> requestState){
            //List<object> result = new List<object> ();
            SubscribeEnvelope resultSubscribeEnvelope = null;
            string jsonString = cea.Message;
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Aborting previous subscribe/presence requests having channel(s) UrlProcessResponseCallbackNonAsync", 
                DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            coroutine.BounceRequest<T> (CurrentRequestType.Subscribe, requestState, false);

            if (!jsonString.Equals("[]")) {
                /*result = Helpers.WrapResultBasedOnResponseType<T> (requestState.RespType, jsonString, requestState.Channels, 
                     requestState.ErrorCallback, channelCallbacks, JsonPluggableLibrary, PubnubErrorLevel, this.cipherKey);*/

                //Helpers.WrapResultBasedOnResponseType<T> (requestState, jsonString, JsonPluggableLibrary, 
                    //PubnubErrorLevel, this.cipherKey, ref result);
                //ParseReceiedTimetoken<T> (requestState, result);
                resultSubscribeEnvelope = ParseReceiedJSONV2<T> (requestState, jsonString);

            }

            /*if (requestState.RespType == ResponseType.Subscribe || requestState.RespType == ResponseType.Presence) {
                foreach (ChannelEntity ce in requestState.ChannelEntities) {
                    multiChannelSubscribe.AddOrUpdate (currentChannel, Convert.ToInt64 (result [1].ToString ()), (key, oldValue) => Convert.ToInt64 (result [1].ToString ()));
                }
            }*/
            switch (requestState.RespType) {
            /*case ResponseType.Subscribe:
            case ResponseType.Presence:
                Helpers.ProcessResponseCallbacks<T> (ref result, requestState, this.cipherKey, JsonPluggableLibrary);
                MultiplexInternalCallback<T> (requestState.RespType, result, false);
                break;*/
            case ResponseType.SubscribeV2:
            case ResponseType.PresenceV2:
                Helpers.ProcessResponseCallbacksV2<T> (ref resultSubscribeEnvelope, requestState, this.cipherKey, JsonPluggableLibrary);
                if ((resultSubscribeEnvelope != null) && (resultSubscribeEnvelope.TimetokenMeta != null)) {
                    ParseReceiedTimetoken<T> (requestState, resultSubscribeEnvelope.TimetokenMeta.Timetoken);
                    /*#if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Calling MultiChannelSubscribeRequest \n" +
                        "Timetoken: {1}", 
                        DateTime.Now.ToString (), resultSubscribeEnvelope.TimetokenMeta.Timetoken), LoggingMethod.LevelError);
                    #endif*/

                    MultiChannelSubscribeRequest<T> (requestState.RespType, resultSubscribeEnvelope.TimetokenMeta.Timetoken, false);
                    /*long parsedInt;
                    if (Int64.TryParse (resultSubscribeEnvelope.t.Timetoken, out parsedInt)) {
                        MultiChannelSubscribeRequest<T> (requestState.RespType, parsedInt, false);
                    }
                    #if (ENABLE_PUBNUB_LOGGING)
                    else {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Timetoken parsing failed: {1}", 
                            DateTime.Now.ToString (), resultSubscribeEnvelope.t.Timetoken), LoggingMethod.LevelError);
                    }
                    #endif*/
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ERROR: Couldn't extract timetoken, initiating fresh subscribe request. \nJSON response:\n {1}", 
                        DateTime.Now.ToString (), jsonString), LoggingMethod.LevelError);
                    MultiChannelSubscribeRequest<T> (requestState.RespType, 0, false);
                }
                #endif
                break;
            default:
                break;
            }
        }

        private void UrlProcessResponseCallbackNonAsync<T> (CustomEventArgs<T> cea)
        {
            RequestState<T> requestState = cea.PubnubRequestState;

            try {
                if ((cea.IsError) || (cea.IsTimeout)) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Message: {1}", DateTime.Now.ToString (), cea.Message), LoggingMethod.LevelError);
                    #endif
                    ExceptionHandlers.ResponseCallbackErrorOrTimeoutHandler<T> (cea, requestState, PubnubErrorLevel, JsonPluggableLibrary);

                } else {

                    ResponseCallbackNonErrorHandler<T> (cea, requestState);

                }
            } catch (WebException webEx) {

                ExceptionHandlers.ResponseCallbackWebExceptionHandler<T> (cea, requestState, webEx, PubnubErrorLevel);
                
            } catch (Exception ex) {

                ExceptionHandlers.ResponseCallbackExceptionHandler<T> (cea, requestState, ex, PubnubErrorLevel);
                
            }
        }

        protected void MultiplexExceptionHandler<T> (ResponseType type, bool reconnectMaxTried, bool reconnect)
        {
            List<ChannelEntity> channelEntities = Subscription.Instance.AllSubscribedChannelsAndChannelGroups;
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, in MultiplexExceptionHandler responsetype={1}", DateTime.Now.ToString (), type.ToString ()), LoggingMethod.LevelInfo);
            #endif
            string channelGroups = Helpers.GetNamesFromChannelEntities (channelEntities, true);
            string channels = Helpers.GetNamesFromChannelEntities (channelEntities, false);

            if (reconnectMaxTried) {
                
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MAX retries reached. Exiting the subscribe for channels = {1} and channelgroups = {2}", 
                    DateTime.Now.ToString (), channels, channelGroups), LoggingMethod.LevelInfo);
                #endif

                MultiChannelUnsubscribeInit<T> (ResponseType.Unsubscribe, channels, channelGroups, null, null, null, null);

                Helpers.CheckSubscribedChannelsAndSendCallbacks<T> (Subscription.Instance.AllSubscribedChannelsAndChannelGroups, 
                    type, NetworkCheckMaxRetries, PubnubErrorLevel);

            } else {
                if (!internetStatus) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Subscribe channels = {1} and channelgroups = {2} - No internet connection. ", 
                        DateTime.Now.ToString (), channels, channelGroups), LoggingMethod.LevelInfo);
                    #endif
                    return;
                }

                long tt = lastSubscribeTimetoken;
                if (!EnableResumeOnReconnect && reconnect) {
                    tt =0; //send 0 time token to enable presence event
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Reconnect true and EnableResumeOnReconnect false sending tt = 0. ", 
                        DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                    #endif

                } 
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, sending tt = {1}. ", 
                        DateTime.Now.ToString (), tt.ToString()), LoggingMethod.LevelInfo);
                }
                #endif


                MultiChannelSubscribeRequest<T> (type, tt, reconnect);

            }
        }

        private void HandleMultiplexException<T> (object sender, EventArgs ea)
        {
            MultiplexExceptionEventArgs<T> mea = ea as MultiplexExceptionEventArgs<T>;
            MultiplexExceptionHandler<T> (mea.responseType, mea.reconnectMaxTried, mea.resumeOnReconnect);
        }

        #endregion

        #region "User State"
        private void SharedSetUserState<T> (string channel, string channelGroup, List<ChannelEntity> channelEntities, string uuid, string jsonUserState
        )
        {
            if (string.IsNullOrEmpty (uuid)) {
                uuid = this.SessionUUID;
            }

            Uri request = BuildRequests.BuildSetUserStateRequest (channel, channelGroup, jsonUserState, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.subscribeKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntities, ResponseType.SetUserState, false, 
                0, false, 0, null);

            UrlProcessRequest<T> (request, requestState);

            //bounce the long-polling subscribe requests to update user state
            TerminateCurrentSubscriberRequest<T> ();
        }

        #endregion

        #region "Helpers"

        SubscribeEnvelope ParseReceiedJSONV2<T> (RequestState<T> requestState, string jsonString)
        {
            if (!string.IsNullOrEmpty (jsonString)) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, jsonString = {1}", DateTime.Now.ToString (), jsonString), LoggingMethod.LevelInfo);
                #endif
                //SubscribeEnvelope resultSubscribeEnvelope = jsonPluggableLibrary.Deserialize<SubscribeEnvelope>(jsonString);
                object resultSubscribeEnvelope = jsonPluggableLibrary.DeserializeToObject(jsonString);
                SubscribeEnvelope subscribeEnvelope = new SubscribeEnvelope ();

                if (resultSubscribeEnvelope is Dictionary<string, object>) {

                    Dictionary<string, object> message = (Dictionary<string, object>)resultSubscribeEnvelope;
                    subscribeEnvelope.TimetokenMeta = Helpers.CreateTimetokenMetadata (message ["t"]);
                    subscribeEnvelope.Messages = Helpers.CreateListOfSubscribeMessage (message ["m"]);
                      
                    return subscribeEnvelope;
                } else {
                    #if (ENABLE_PUBNUB_LOGGING)

                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, resultSubscribeEnvelope is not dict", 
                        DateTime.Now.ToString ()), LoggingMethod.LevelError);
                
                    #endif
                    
                    return null;
                }
            } else {
                return null;
            }

        }

        void ParseReceiedTimetoken<T> (RequestState<T> requestState, long receivedTimetoken)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, receivedTimetoken = {1}", 
                DateTime.Now.ToString (), receivedTimetoken.ToString()),
                LoggingMethod.LevelInfo);
            #endif
            lastSubscribeTimetoken = receivedTimetoken;
            if (!enableResumeOnReconnect) {
                lastSubscribeTimetoken = receivedTimetoken;
            }
            else {
                //do nothing. keep last subscribe token
            }
            if (requestState.Reconnect) {
                if (enableResumeOnReconnect) {
                    //do nothing. keep last subscribe token
                }
                else {
                    lastSubscribeTimetoken = receivedTimetoken;
                }
            }
        }

        private void RunRequests<T> (Uri requestUri, RequestState<T> pubnubRequestState)
        {
            if (pubnubRequestState.RespType.Equals(ResponseType.Subscribe) || pubnubRequestState.RespType.Equals(ResponseType.Presence)
                || pubnubRequestState.RespType.Equals(ResponseType.SubscribeV2) || pubnubRequestState.RespType.Equals(ResponseType.PresenceV2)
            ) {
                RequestState<T> pubnubRequestStateHB = pubnubRequestState;
                pubnubRequestStateHB.ID = DateTime.Now.Ticks;

                RunHeartbeat<T> (false, LocalClientHeartbeatInterval, pubnubRequestStateHB);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Heartbeat started", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                #endif
                if (Subscription.Instance.HasPresenceChannels && (PresenceHeartbeatInterval > 0)){
                        RunPresenceHeartbeat<T> (false, PresenceHeartbeatInterval, pubnubRequestStateHB);
                }
                StoredRequestState.Instance.SetRequestState (CurrentRequestType.Subscribe, pubnubRequestState);
                coroutine.SubCoroutineComplete += CoroutineCompleteHandler<T>;
                coroutine.Run<T> (requestUri.OriginalString, pubnubRequestState, SubscribeTimeout, 0);
            }
            else {
                StoredRequestState.Instance.SetRequestState (CurrentRequestType.NonSubscribe, pubnubRequestState);
                coroutine.NonSubCoroutineComplete += CoroutineCompleteHandler<T>;
                coroutine.Run<T> (requestUri.OriginalString, pubnubRequestState, NonSubscribeTimeout, 0);
            }
        }

        private bool UrlProcessRequest<T> (Uri requestUri, RequestState<T> pubnubRequestState)
        {
            try {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, In Url process request", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                #endif

                RunRequests<T> (requestUri, pubnubRequestState);

            } catch (UnityEngine.MissingReferenceException ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Exception={1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
                return false;
            } catch (System.NullReferenceException ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Exception={1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
                return false;
            } catch (System.Exception ex) {
                if (pubnubRequestState != null && pubnubRequestState.ErrorCallback != null) {

                    PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (ex, pubnubRequestState, 
                        PubnubErrorSeverity.Critical, PubnubErrorCode.None, PubnubErrorLevel);
                }
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} Exception={1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif

                ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, pubnubRequestState, 
                    false, false, PubnubErrorLevel);
                return false;
            }
            return true;
        }

        private void AbortPreviousRequestAndFetchTimetoken<T>(List<ChannelEntity> existingChannels)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, Aborting previous subscribe/presence requests having channel(s)={1} and ChannelGroup(s) = {2}", 
                DateTime.Now.ToString(), Helpers.GetNamesFromChannelEntities(existingChannels, false), 
                Helpers.GetNamesFromChannelEntities(existingChannels, true)), LoggingMethod.LevelInfo);
            #endif
            RequestState<T> reqState = StoredRequestState.Instance.GetStoredRequestState(CurrentRequestType.Subscribe) as RequestState<T>;
            coroutine.BounceRequest<T>(CurrentRequestType.Subscribe, reqState, false);
        }

        void RemoveUnsubscribedChannelsAndDeleteUserState<T>(List<ChannelEntity> channelEntities)
        {
            foreach(ChannelEntity ce in channelEntities)
            {
                string channelToBeRemoved = ce.ChannelID.ChannelOrChannelGroupName;
                PubnubChannelCallback<T> channelCallback = ce.ChannelParams.Callbacks as PubnubChannelCallback<T>;
                if (Subscription.Instance.Delete (ce)) {
                    string jsonString = string.Format ("{0} Unsubscribed from {1}", (ce.ChannelID.IsPresenceChannel) ? "Presence" : "", channelToBeRemoved.Replace (Utility.PresenceChannelSuffix, ""));
                    List<object> result = Helpers.CreateJsonResponse (jsonString, channelToBeRemoved.Replace (Utility.PresenceChannelSuffix, ""), JsonPluggableLibrary);
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, JSON response={1}", DateTime.Now.ToString (), jsonString), LoggingMethod.LevelInfo);
                    #endif
                    PubnubCallbacks.GoToCallback<T> (result, channelCallback.DisconnectCallback, JsonPluggableLibrary);
                } else {
                    string message = string.Format("Unsubscribe Error. Please retry the unsubscribe operation. channel{0}", channelToBeRemoved);
                    PubnubErrorCode errorType = (ce.ChannelID.IsPresenceChannel) ? PubnubErrorCode.PresenceUnsubscribeFailed : PubnubErrorCode.UnsubscribeFailed;
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog(string.Format("DateTime {0}, channel={1} unsubscribe error", DateTime.Now.ToString(), channelToBeRemoved), LoggingMethod.LevelInfo);
                    #endif
                    PubnubCallbacks.CallErrorCallback<T>(message, channelCallback.ErrorCallback, 
                        errorType, PubnubErrorSeverity.Critical, PubnubErrorLevel);
                }
            }

      
        }

        void ContinueToSubscribeRestOfChannels<T>(ResponseType type, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback)
        {
            List<ChannelEntity> subscribedChannels = Subscription.Instance.AllSubscribedChannelsAndChannelGroups;

            if (subscribedChannels != null && subscribedChannels.Count > 0)
            {
                ResetInternetCheckSettings(subscribedChannels);
                //Modify the value for type ResponseType. Presence or Subscrie is ok, but sending the close value would make sense
                if (Subscription.Instance.HasPresenceChannels)
                {
                    //type = ResponseType.Presence;
                    type = ResponseType.PresenceV2;
                }
                else
                {
                    //type = ResponseType.Subscribe;
                    type = ResponseType.SubscribeV2;
                }
                //Continue with any remaining channels for subscribe/presence
                MultiChannelSubscribeRequest<T>(type, 0, false);
            }
            else
            {
                if (isHearbeatRunning)
                {
                    StopHeartbeat<T>();
                }
                if (isPresenceHearbeatRunning)
                {
                    StopPresenceHeartbeat<T>();
                }
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, All channels are Unsubscribed. Further subscription was stopped", DateTime.Now.ToString()), LoggingMethod.LevelInfo);
                #endif
                ExceptionHandlers.MultiplexException -= HandleMultiplexException<T>;
            }
        }

        private void MultiChannelUnsubscribeInit<T> (ResponseType respType, string channel, string channelGroups, Action<T> userCallback, 
            Action<T> connectCallback, Action<T> disconnectCallback, Action<PubnubClientError> errorCallback)
        {
            string[] rawChannels = channel.Split (',');
            string[] rawChannelGroups = channelGroups.Split (',');
            List<ChannelEntity> subscribedChannels = Subscription.Instance.AllSubscribedChannelsAndChannelGroups;

            List<ChannelEntity> newChannelEntities;
            bool channelsOrChannelGroupsAdded = Helpers.RemoveDuplicatesCheckAlreadySubscribedAndGetChannels<T>(respType, userCallback, connectCallback,
                errorCallback, null, disconnectCallback, rawChannels, rawChannelGroups, PubnubErrorLevel, true, out newChannelEntities);
            
            if (newChannelEntities.Count > 0) {
                //Retrieve the current channels already subscribed previously and terminate them
                AbortPreviousRequestAndFetchTimetoken<T>(subscribedChannels);

                if (respType == ResponseType.Unsubscribe) {
                    Uri request = BuildRequests.BuildMultiChannelLeaveRequest(channel, channelGroups, this.SessionUUID, 
                        this.ssl, this.Origin, authenticationKey, this.subscribeKey);
                    RequestState<T> requestState = BuildRequests.BuildRequestState<T>(newChannelEntities, ResponseType.Leave, false, 0, false, 0, null);
                    UrlProcessRequest<T>(request, requestState);

                }
                //Remove the valid channels from subscribe list for unsubscribe 
                RemoveUnsubscribedChannelsAndDeleteUserState<T>(newChannelEntities);

                //Get all the channels
                ContinueToSubscribeRestOfChannels<T>(respType, userCallback, connectCallback, errorCallback);
            }
        }

        private void ResetInternetCheckSettings (List<ChannelEntity> subscribedChannels)
        {
            if (subscribedChannels == null || subscribedChannels.Count <= 0)
                return;
            retryCount = 0;
            internetStatus = true;
            retriesExceeded = false;
        }

        public void MultiChannelSubscribeInit<T> (ResponseType respType, string channel, string channelGroup, long timetokenToUse, 
            Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback, Action<T> wildcardPresenceCallback)
        {
            string[] rawChannels = channel.Split (',');
            string[] rawChannelGroups = channelGroup.Split (',');

            List<ChannelEntity> subscribedChannels = Subscription.Instance.AllSubscribedChannelsAndChannelGroups;

            ResetInternetCheckSettings (subscribedChannels);

            List<ChannelEntity> newChannelEntities;
            bool channelsOrChannelGroupsAdded = Helpers.RemoveDuplicatesCheckAlreadySubscribedAndGetChannels<T>(respType, userCallback, connectCallback,
                errorCallback, wildcardPresenceCallback, null, rawChannels, rawChannelGroups, PubnubErrorLevel, false, out newChannelEntities);

            if (channelsOrChannelGroupsAdded && internetStatus) {
                resetTimetoken = true;
                Subscription.Instance.Add (newChannelEntities);

                #if (ENABLE_PUBNUB_LOGGING)
                Helpers.LogChannelEntitiesDictionary();
                #endif

                if (!timetokenToUse.Equals (0)) {
                    lastSubscribeTimetokenForNewMultiplex = timetokenToUse;
                } 
                else if(subscribedChannels.Count>0)
                {
                    AbortPreviousRequestAndFetchTimetoken<T> (subscribedChannels);
                    lastSubscribeTimetokenForNewMultiplex = lastSubscribeTimetoken;
                }
                MultiChannelSubscribeRequest<T> (respType, lastSubscribeTimetokenForNewMultiplex, false);
            }
        }

        private bool CheckAllChannelsAreUnsubscribed<T>()
        {
            if (Subscription.Instance.AllSubscribedChannelsAndChannelGroups.Count <=0)
            {
                StopHeartbeat<T>();
                if (isPresenceHearbeatRunning)
                {
                    StopPresenceHeartbeat<T>();
                }
                ExceptionHandlers.MultiplexException -= HandleMultiplexException<T>;
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, All channels are Unsubscribed. Further subscription was stopped", DateTime.Now.ToString()), LoggingMethod.LevelInfo);
                #endif
                return true;
            }
            return false;
        }

        private bool CheckSystemActiveAndRetriesExceeded<T>(ResponseType type, List<ChannelEntity> channelEntities)
        {
            if (pubnetSystemActive && retriesExceeded)
            {
            #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, Subscribe channel={1} - No internet connection. MAXed retries for internet ", 
                    DateTime.Now.ToString(), Helpers.GetNamesFromChannelEntities(channelEntities)), LoggingMethod.LevelInfo);
                #endif
                MultiplexExceptionHandler<T>(type, true, false);
                retriesExceeded = false;
                return true;
            }
            return false;
        }

        long SaveLastTimetoken(long timetoken)
        {
            long lastTimetoken = 0;
            long sentTimetoken = timetoken;//Convert.ToInt64(timetoken.ToString());
            //long minimumTimetoken = multiChannelSubscribe.Min(token => token.Value);
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, lastSubscribeTimetokenForNewMultiplex={1}", DateTime.Now.ToString(), lastSubscribeTimetokenForNewMultiplex), LoggingMethod.LevelInfo);
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, sentTimetoken={1}", DateTime.Now.ToString(), sentTimetoken.ToString()), LoggingMethod.LevelInfo);
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, lastSubscribeTimetoken={1}", DateTime.Now.ToString(), lastSubscribeTimetoken), LoggingMethod.LevelInfo);
            #endif
            if (resetTimetoken || uuidChanged)
            {
                lastTimetoken = 0;
                uuidChanged = false;
                resetTimetoken = false;
            }
            else
            {
                //override lastTimetoken when lastSubscribeTimetokenForNewMultiplex is set.
                //this is done to use the timetoken prior to the latest response from the server 
                //and is true in case new channels are added to the subscribe list.
                if (!sentTimetoken.Equals(0) && !lastSubscribeTimetokenForNewMultiplex.Equals(0) && !lastSubscribeTimetoken.Equals(lastSubscribeTimetokenForNewMultiplex))
                {
                    lastTimetoken = lastSubscribeTimetokenForNewMultiplex;
                    lastSubscribeTimetokenForNewMultiplex = 0;
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog(string.Format("DateTime {0}, Using lastSubscribeTimetokenForNewMultiplex={1}", DateTime.Now.ToString(), lastSubscribeTimetokenForNewMultiplex), LoggingMethod.LevelInfo);
                    #endif
                }
                else
                    if (sentTimetoken.Equals(0))
                    {
                        lastTimetoken = sentTimetoken;
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog(string.Format("DateTime {0}, Using sentTimetoken={1}", DateTime.Now.ToString(), sentTimetoken), LoggingMethod.LevelInfo);
                        #endif
                    }
                    else
                    {
                        lastTimetoken = sentTimetoken;
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog(string.Format("DateTime {0}, Using sentTimetoken={1}", DateTime.Now.ToString(), sentTimetoken), LoggingMethod.LevelInfo);
                        #endif
                    }
                if (lastSubscribeTimetoken.Equals(lastSubscribeTimetokenForNewMultiplex))
                {
                    lastSubscribeTimetokenForNewMultiplex = 0;
                }
            }
            return lastTimetoken;
        }

        /// <summary>
        /// Multi-Channel Subscribe Request - private method for Subscribe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="channels"></param>
        /// <param name="timetoken"></param>
        /// <param name="userCallback"></param>
        /// <param name="connectCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="reconnect"></param>
        /// 
        private void MultiChannelSubscribeRequest<T> (ResponseType type, long timetoken, bool reconnect)
        {
            //Exit if the channel is unsubscribed
            if (CheckAllChannelsAreUnsubscribed<T>())
            {
                return;
            }
            List<ChannelEntity> channelEntities = Subscription.Instance.AllSubscribedChannelsAndChannelGroups;
            if (CheckSystemActiveAndRetriesExceeded<T>(type, channelEntities))
            {
                return;
            }

            // Begin recursive subscribe
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MultiChannelSubscribeRequest ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);    
            #endif
            try {
                long lastTimetoken = SaveLastTimetoken(timetoken);
    
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Building request for {1} with timetoken={2}", DateTime.Now.ToString (), 
                    Helpers.GetNamesFromChannelEntities(channelEntities), lastTimetoken), LoggingMethod.LevelInfo);
                #endif
                // Build URL
                string channelsJsonState = Subscription.Instance.CompiledUserState;

                string channels = Helpers.GetNamesFromChannelEntities(channelEntities, false);
                string channelGroups = Helpers.GetNamesFromChannelEntities(channelEntities, true);

                //v1
                /*Uri requestUrl = BuildRequests.BuildMultiChannelSubscribeRequest (channels,
                    channelGroups,
                    lastTimetoken, channelsJsonState, this.SessionUUID,
                    this.ssl, this.Origin, authenticationKey, this.subscribeKey);*/
                //v2
                string filterExpr = (this.FilterExpr!=null) ? Helpers.JsonEncodePublishMsg (this.FilterExpr, this.cipherKey, JsonPluggableLibrary) : "";
                Uri requestUrl = BuildRequests.BuildMultiChannelSubscribeRequestV2 (channels,
                    channelGroups, lastTimetoken.ToString(), channelsJsonState, this.SessionUUID, this.Region, 
                    filterExpr, this.ssl, this.Origin, authenticationKey, this.subscribeKey);

                RequestState<T> pubnubRequestState = BuildRequests.BuildRequestState<T> (channelEntities, type, reconnect, 
                     0, false, Convert.ToInt64 (timetoken.ToString ()), typeof(T));
                // Wait for message
                ExceptionHandlers.MultiplexException += HandleMultiplexException<T>;
                UrlProcessRequest<T> (requestUrl, pubnubRequestState);
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} method:_subscribe \n channel={1} \n timetoken={2} \n Exception Details={3}",
                    DateTime.Now.ToString (), Helpers.GetNamesFromChannelEntities(channelEntities), timetoken.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
                PubnubCallbacks.CallErrorCallback<T> (ex, channelEntities, 
                    PubnubErrorCode.None, PubnubErrorSeverity.Critical, PubnubErrorLevel);

                this.MultiChannelSubscribeRequest<T> (type, timetoken, false);
            }
        }

        private void RetryLoop<T> (RequestState<T> pubnubRequestState)
        {
            internetStatus = false;
            retryCount++;
            if (retryCount <= NetworkCheckMaxRetries) {
                string cbMessage = string.Format ("DateTime {0} Internet Disconnected, retrying. Retry count {1} of {2}", DateTime.Now.ToString (), retryCount.ToString (), NetworkCheckMaxRetries);    
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (cbMessage, LoggingMethod.LevelError);
                #endif
                PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (cbMessage, pubnubRequestState, 
                    PubnubErrorSeverity.Warn, PubnubErrorCode.NoInternetRetryConnect, PubnubErrorLevel);

            } else {
                retriesExceeded = true;
                string cbMessage = string.Format ("DateTime {0} Internet Disconnected. Retries exceeded {1}. Unsubscribing connected channels.", DateTime.Now.ToString (), NetworkCheckMaxRetries);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (cbMessage, LoggingMethod.LevelError);
                #endif

                //stop heartbeat.
                keepHearbeatRunning = false;
                coroutine.BounceRequest<T> (CurrentRequestType.Subscribe, pubnubRequestState, false);

                PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (cbMessage, pubnubRequestState, 
                    PubnubErrorSeverity.Warn, PubnubErrorCode.NoInternetRetryConnect, PubnubErrorLevel);


                MultiplexExceptionHandler<T> (ResponseType.SubscribeV2, true, false);
            }
        }

        /// <summary>
        /// Check the response of the REST API and call for re-subscribe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="multiplexResult"></param>
        /// <param name="userCallback"></param>
        /// <param name="connectCallback"></param>
        /// <param name="errorCallback"></param>
        /*private void MultiplexInternalCallback<T> (ResponseType type, object multiplexResult, bool reconnect)
        {
            List<object> message = multiplexResult as List<object>;
            string[] channels;
            if (message != null && message.Count >= 3) {
                if (message [message.Count - 1] is string[]) {
                    channels = message [message.Count - 1] as string[];
                } else {
                    channels = message [message.Count - 1].ToString ().Split (',') as string[];
                }
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Lost Channel Name for resubscribe", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                #endif
                return;
            }

            if (message != null && message.Count >= 3) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MultiChannelSubscribeRequest {1}", DateTime.Now.ToString (), message [1].ToString()), LoggingMethod.LevelInfo);    
                #endif
                MultiChannelSubscribeRequest<T> (type, (object)message [1], reconnect);
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MultiplexInternalCallback message null or count < 3", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            }
            #endif    
        }*/

       
        #endregion

    }
}