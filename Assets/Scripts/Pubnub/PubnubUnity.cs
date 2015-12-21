//Build Date: Nov 26, 2015
//ver3.6.9.0/Unity5
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

        private CoroutineClass coroutine;

        private string origin = "pubsub.pubnub.com";
        private string publishKey = "";
        private string subscribeKey = "";
        private string secretKey = "";
        private string cipherKey = "";
        private bool ssl = true;
        private static long lastSubscribeTimetoken = 0;
        private static long lastSubscribeTimetokenForNewMultiplex = 0;
        private const string build = "3.6.9.0";
        private static string pnsdkVersion = "PubNub-CSharp-Unity5/3.6.9.0";

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
        private SafeDictionary<string, long> multiChannelSubscribe = new SafeDictionary<string, long> ();
        private SafeDictionary<string, PubnubWebRequest> channelRequest = new SafeDictionary<string, PubnubWebRequest> ();
        private SafeDictionary<PubnubChannelCallbackKey, object> channelCallbacks = new SafeDictionary<PubnubChannelCallbackKey, object> ();
        private SafeDictionary<string, Dictionary<string, object>> channelLocalUserState = new SafeDictionary<string, Dictionary<string, object>> ();
        private SafeDictionary<string, Dictionary<string, object>> channelUserState = new SafeDictionary<string, Dictionary<string, object>> ();
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
                pubnubWebRequestCallbackIntervalInSeconds = value;
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
                pubnubOperationTimeoutIntervalInSeconds = value;
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

            this.publishKey = publishKey;
            this.subscribeKey = subscribeKey;
            this.secretKey = secretKey;
            this.cipherKey = cipherKey;
            this.ssl = sslOn;

            retriesExceeded = false;
            internetStatus = true;

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

        public void Subscribe<T> (string channel, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, requested subscribe for channel={1}", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);
            #endif

            MultiChannelSubscribeInit<T> (ResponseType.Subscribe, channel, userCallback, connectCallback, errorCallback);
        }

        #endregion

        #region "Publish"

        public bool Publish<T> (string channel, object message, bool storeInHistory, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            string originalMessage = (enableJsonEncodingForPublish) ? Helpers.JsonEncodePublishMsg (message, this.cipherKey, JsonPluggableLibrary) : message.ToString ();

            Uri request = BuildRequests.BuildPublishRequest (channel, originalMessage, storeInHistory, this.SessionUUID,
                this.ssl, this.Origin, this.AuthenticationKey, this.publishKey, this.subscribeKey, this.cipherKey, this.secretKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (new string[] { channel }, ResponseType.Publish, 
                false, userCallback, null, errorCallback, 0, false, 0, null);

            return UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "Presence"

        public void Presence<T> (string channel, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, requested presence for channel={1}", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);
            #endif

            MultiChannelSubscribeInit<T> (ResponseType.Presence, channel, userCallback, connectCallback, errorCallback);
        }

        #endregion

        #region "Detailed History"

        public bool DetailedHistory<T> (string channel, long start, long end, int count, bool reverse, bool includeToken, 
            Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildDetailedHistoryRequest (channel, start, end, count, reverse, includeToken, this.SessionUUID,
                this.ssl, this.Origin, this.AuthenticationKey, this.subscribeKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (new string[] { channel }, ResponseType.DetailedHistory, false, userCallback, null, errorCallback, 0, false, 0, null);

            return UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "HereNow"

        public bool HereNow<T> (string channel, bool showUUIDList, bool includeUserState, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildHereNowRequest (channel, showUUIDList, includeUserState, this.SessionUUID,
                this.ssl, this.Origin, this.AuthenticationKey, this.subscribeKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (new string[] { channel }, ResponseType.HereNow, false, userCallback, null, errorCallback, 0, false, 0, null);

            return UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "Global Here Now"

        public bool GlobalHereNow<T> (bool showUUIDList, bool includeUserState, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildGlobalHereNowRequest (showUUIDList, includeUserState, this.SessionUUID,
                this.ssl, this.Origin, this.AuthenticationKey, this.subscribeKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (null, ResponseType.GlobalHereNow, false, userCallback, null, errorCallback, 0, false, 0, null);

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

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (new string[] { uuid }, ResponseType.WhereNow, false, userCallback, null, errorCallback, 0, false, 0, null);

            UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "Unsubscribe Presence And Subscribe"

        public void PresenceUnsubscribe<T> (string channel, Action<T> userCallback, Action<T> connectCallback, Action<T> disconnectCallback, Action<PubnubClientError> errorCallback)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, requested presence-unsubscribe for channel(s)={1}", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);
            #endif
            MultiChannelUnSubscribeInit<T> (ResponseType.PresenceUnsubscribe, channel, userCallback, connectCallback, disconnectCallback, errorCallback);
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
        public void Unsubscribe<T> (string channel, Action<T> userCallback, Action<T> connectCallback, Action<T> disconnectCallback, Action<PubnubClientError> errorCallback)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, requested unsubscribe for channel(s)={1}", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);
            #endif
            MultiChannelUnSubscribeInit<T> (ResponseType.Unsubscribe, channel, userCallback, connectCallback, disconnectCallback, errorCallback);

        }

        #endregion

        #region "Time"

        public bool Time<T> (Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildTimeRequest (this.SessionUUID, this.ssl, this.Origin);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (null, ResponseType.Time, false, userCallback, null, errorCallback, 0, false, 0, null);
            return UrlProcessRequest<T> (request, requestState); 
        }

        #endregion

        #region "PAM"

        #region "Grant Access"

        public bool GrantAccess<T> (string channel, string authenticationKey, bool read, bool write, int ttl, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            Uri request = BuildRequests.BuildGrantAccessRequest (channel, read, write, ttl, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.publishKey, this.subscribeKey, this.cipherKey, this.secretKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (new string[] { channel }, ResponseType.GrantAccess, false, userCallback, null, errorCallback, 0, false, 0, null);

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

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (!string.IsNullOrEmpty (channel)? new string[] { channel } : null, ResponseType.AuditAccess, false, userCallback, null, errorCallback, 0, false, 0, null);

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

        public void SetUserState<T> (string channel, string uuid, string jsonUserState, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            if (!JsonPluggableLibrary.IsDictionaryCompatible (jsonUserState)) {
                throw new MissingMemberException ("Missing json format for user state");
            } else {
                Dictionary<string, object> deserializeUserState = JsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonUserState);
                if (deserializeUserState == null) {
                    throw new MissingMemberException ("Missing json format user state");
                } else {
                    string oldJsonState = GetLocalUserState (channel);
                    if (oldJsonState.Equals(jsonUserState)) {
                        string message = "No change in User State";

                        PubnubCallbacks.CallErrorCallback<T> (message, null, channel, 
                            PubnubErrorCode.UserStateUnchanged, PubnubErrorSeverity.Info, errorCallback, PubnubErrorLevel);

                        return;
                    }

                }
            }

            SharedSetUserState (channel, uuid, jsonUserState, userCallback, errorCallback);
        }

        public void SetUserState<T> (string channel, string uuid, KeyValuePair<string, object> keyValuePair, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            string key = keyValuePair.Key;

            int valueInt;
            double valueDouble;
            string currentUserState = "";

            string oldJsonState = GetLocalUserState (channel);
            if (keyValuePair.Value == null) {
                currentUserState = SetLocalUserState (channel, key, null);
            } else if (Int32.TryParse (keyValuePair.Value.ToString (), out valueInt)) {
                currentUserState = SetLocalUserState (channel, key, valueInt);
            } else if (Double.TryParse (keyValuePair.Value.ToString (), out valueDouble)) {
                currentUserState = SetLocalUserState (channel, key, valueDouble);
            } else {
                currentUserState = SetLocalUserState (channel, key, keyValuePair.Value.ToString ());
            }

            if (oldJsonState.Equals(currentUserState)) {
                string message = "No change in User State";

                PubnubCallbacks.CallErrorCallback<T> (message, null, channel, 
                    PubnubErrorCode.UserStateUnchanged, PubnubErrorSeverity.Info, errorCallback, PubnubErrorLevel);

                return;
            }

            if (currentUserState.Trim () == "") {
                currentUserState = "{}";
            }

            SharedSetUserState<T> (channel, uuid, currentUserState, userCallback, errorCallback);
        }

        #endregion

        #region "Get User State"

        public void GetUserState<T> (string channel, string uuid, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            if (string.IsNullOrEmpty (uuid)) {
                uuid = this.SessionUUID;
            }

            Uri request = BuildRequests.BuildGetUserStateRequest (channel, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.subscribeKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (new string[] { channel }, ResponseType.GetUserState, false, userCallback, null, errorCallback, 0, false, 0, null);

            UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "PubNub API Other Methods"

        public void TerminateCurrentSubscriberRequest ()
        {
            TerminateCurrentSubscriberRequest<string> ();
        }

        public void TerminateCurrentSubscriberRequest<T> ()
        {
            StopHeartbeat<T> ();
            StopPresenceHeartbeat<T> ();
            RequestState<T> reqState = StoredRequestState.Instance.GetStoredRequestState (CurrentRequestType.Subscribe) as RequestState<T>;
            coroutine.BounceRequest<T> (CurrentRequestType.Subscribe, reqState, true);
        }

        public void EndPendingRequests ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} ending open requests.", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            RequestState<string> reqStateSub = StoredRequestState.Instance.GetStoredRequestState (CurrentRequestType.Subscribe) as RequestState<string>;
            coroutine.BounceRequest<string> (CurrentRequestType.Subscribe, reqStateSub, false);
            RequestState<string> reqStateNonSub = StoredRequestState.Instance.GetStoredRequestState (CurrentRequestType.NonSubscribe) as RequestState<string>;
            coroutine.BounceRequest<string> (CurrentRequestType.NonSubscribe, reqStateNonSub, false);
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Request bounced.", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            StopHeartbeat ();
            StopPresenceHeartbeat ();
            ClearChannelCallback ();
            RemoveUserState ();
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} ClearChannelCallback and RemoveUserState complete.", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            ClearChannelRequest();
            ClearMultiChannelSubscribe();
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} ClearChannelRequest and ClearMultiChannelSubscribe complete.", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
        }

        public void CleanUp (){
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

        private void ClearChannelRequest ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Clear ChannelRequest", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            channelRequest.Clear ();
        }

        private void ClearMultiChannelSubscribe ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Clear MultiChannelSubscribe", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            multiChannelSubscribe.Clear ();
        }

        private void ClearChannelCallback ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Clear ChannelCallback", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            channelCallbacks.Clear ();
        }

        private void RemoveUserState ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} RemoveUserState from user state dictionary", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            channelLocalUserState.Clear ();
            channelUserState.Clear ();
        }

        #endregion

        #region "Change UUID"

        public void ChangeUUID (string newUUID)
        {
            if (string.IsNullOrEmpty (newUUID) || this.SessionUUID == newUUID) {
                return;
            }

            uuidChanged = true;

            string oldUUID = SessionUUID;

            SessionUUID = newUUID;

            string[] channels = Helpers.GetCurrentSubscriberChannels (multiChannelSubscribe);

            if (channels != null && channels.Length > 0) {
                Uri request = BuildRequests.BuildMultiChannelLeaveRequest (channels, oldUUID,
                    this.ssl, this.Origin, authenticationKey, this.subscribeKey);

                RequestState<string> requestState = BuildRequests.BuildRequestState<string> (channels, ResponseType.Leave, false, null, null, null, 0, false, 0, null);

                UrlProcessRequest<string> (request, requestState); 
            }

            TerminateCurrentSubscriberRequest<string> ();

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
                string[] channels = multiChannelSubscribe.Keys.ToArray<string> ();
                if (channels != null && channels.Length > 0) {
                    channels = channels.Where (s => s.Contains (Utility.PresenceChannelSuffix) == false).ToArray ();

                    if (channels != null && channels.Length > 0) {
                        isPresenceHearbeatRunning = true;
                        string channelsJsonState = BuildJsonUserState (channels, false);

                        Uri requestUrl = BuildRequests.BuildPresenceHeartbeatRequest (channels, channelsJsonState, this.SessionUUID,
                            this.ssl, this.Origin, authenticationKey, this.subscribeKey);

                        coroutine.PresenceHeartbeatCoroutineComplete += CoroutineCompleteHandler<T>;

                        //for heartbeat and presence heartbeat treat reconnect as pause
                        RequestState<T> requestState = BuildRequests.BuildRequestState<T> (pubnubRequestState.Channels, ResponseType.PresenceHeartbeat, pause, null, null, pubnubRequestState.ErrorCallback, pubnubRequestState.ID, false, 0, null);
                        StoredRequestState.Instance.SetRequestState (CurrentRequestType.PresenceHeartbeat, requestState);
                        coroutine.Run<T> (requestUrl.OriginalString, requestState, HeartbeatTimeout, pauseTime);
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PresenceHeartbeat running for {1}", DateTime.Now.ToString (), pubnubRequestState.ID), LoggingMethod.LevelInfo);
                        #endif
                    }
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
                RequestState<T> requestState = BuildRequests.BuildRequestState<T> (pubnubRequestState.Channels, ResponseType.Heartbeat, pause, pubnubRequestState.UserCallback, pubnubRequestState.ConnectCallback, pubnubRequestState.ErrorCallback, pubnubRequestState.ID, false, 0, null);
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
                string multiChannel = string.Join (",", cea.PubnubRequestState.Channels);

                PubnubCallbacks.CallErrorCallback<T> (cbMessage, null, multiChannel, 
                    PubnubErrorCode.YesInternet, PubnubErrorSeverity.Info, cea.PubnubRequestState.ErrorCallback, PubnubErrorLevel);

                string[] activeChannels = multiChannelSubscribe.Keys.ToArray<string> ();
                MultiplexExceptionHandler<T> (ResponseType.Subscribe, activeChannels, cea.PubnubRequestState.UserCallback, cea.PubnubRequestState.ConnectCallback, cea.PubnubRequestState.ErrorCallback, false, true);
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
				ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (cea.Message.ToString (), cea.PubnubRequestState.respType, cea.PubnubRequestState.Channels, 
                    true, cea.PubnubRequestState.UserCallback, cea.PubnubRequestState.ConnectCallback, 
                    cea.PubnubRequestState.ErrorCallback, false, PubnubErrorLevel);
            } else if (cea.IsError) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} NonSub Error={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
                #endif
				ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (cea.Message.ToString (), cea.PubnubRequestState.respType, cea.PubnubRequestState.Channels, 
                    false, cea.PubnubRequestState.UserCallback, cea.PubnubRequestState.ConnectCallback, 
                    cea.PubnubRequestState.ErrorCallback, false, PubnubErrorLevel);
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} NonSub Message={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelInfo);
                #endif
                var result = Helpers.WrapResultBasedOnResponseType<T> (cea.PubnubRequestState.respType, cea.Message, 
                    cea.PubnubRequestState.Channels, cea.PubnubRequestState.ErrorCallback, channelCallbacks, 
                    JsonPluggableLibrary, PubnubErrorLevel, this.cipherKey);

                Helpers.ProcessResponseCallbacks<T> (result, cea.PubnubRequestState, 
                    multiChannelSubscribe, this.cipherKey, channelCallbacks, JsonPluggableLibrary);
            }
        }

        private void ProcessCoroutineCompleteResponse<T> (CustomEventArgs<T> cea)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, In handler of event cea {1} RequestType CoroutineCompleteHandler {2}", DateTime.Now.ToString (), cea.PubnubRequestState.respType.ToString (), typeof(T)), LoggingMethod.LevelInfo);
            #endif
            switch (cea.PubnubRequestState.respType) {
            case ResponseType.Heartbeat:

                HeartbeatHandler<T> (cea);

                break;

            case ResponseType.PresenceHeartbeat:

                PresenceHeartbeatHandler<T> (cea);

                break;
            case ResponseType.Subscribe:
            case ResponseType.Presence:

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

				ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, cea.PubnubRequestState.respType, cea.PubnubRequestState.Channels, 
                    false, cea.PubnubRequestState.UserCallback, cea.PubnubRequestState.ConnectCallback, 
                    cea.PubnubRequestState.ErrorCallback, false, PubnubErrorLevel);
            } 
        }

        void ResponseCallbackNonErrorHandler<T> (CustomEventArgs<T> cea, RequestState<T> requestState, List<object> result){
            string jsonString = cea.Message;
            if (overrideTcpKeepAlive) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Aborting previous subscribe/presence requests having channel(s) UrlProcessResponseCallbackNonAsync", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                #endif
                coroutine.BounceRequest<T> (CurrentRequestType.Subscribe, requestState, false);
            }

            if (!jsonString.Equals("[]")) {
                result = Helpers.WrapResultBasedOnResponseType<T> (requestState.respType, jsonString, requestState.Channels, 
                     requestState.ErrorCallback, channelCallbacks, JsonPluggableLibrary, PubnubErrorLevel, this.cipherKey);

                ParseReceiedTimetoken<T> (requestState, result);
            }
            Helpers.ProcessResponseCallbacks<T> (result, requestState, multiChannelSubscribe, this.cipherKey, channelCallbacks, JsonPluggableLibrary);

            if (requestState.respType == ResponseType.Subscribe || requestState.respType == ResponseType.Presence) {
                foreach (string currentChannel in requestState.Channels) {
                    multiChannelSubscribe.AddOrUpdate (currentChannel, Convert.ToInt64 (result [1].ToString ()), (key, oldValue) => Convert.ToInt64 (result [1].ToString ()));
                }
            }
            switch (requestState.respType) {
            case ResponseType.Subscribe:
            case ResponseType.Presence:
                MultiplexInternalCallback<T> (requestState.respType, result, requestState.UserCallback, requestState.ConnectCallback, requestState.ErrorCallback, false);
                break;
            default:
                break;
            }
        }

        private void UrlProcessResponseCallbackNonAsync<T> (CustomEventArgs<T> cea)
        {
            List<object> result = new List<object> ();

            RequestState<T> requestState = cea.PubnubRequestState;

            string channel = "";
            if (requestState != null && requestState.Channels != null) {
                channel = string.Join (",", requestState.Channels);
            }
            try {
                if ((cea.IsError) || (cea.IsTimeout)) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Message: {1}", DateTime.Now.ToString (), cea.Message), LoggingMethod.LevelError);
                    #endif
                    ExceptionHandlers.ResponseCallbackErrorOrTimeoutHandler<T> (cea, requestState, channel, PubnubErrorLevel, JsonPluggableLibrary);

                } else {

                    ResponseCallbackNonErrorHandler<T> (cea, requestState, result);

                }
            } catch (WebException webEx) {

                ExceptionHandlers.ResponseCallbackWebExceptionHandler<T> (cea, requestState, webEx, channel, channelCallbacks, PubnubErrorLevel);
                
            } catch (Exception ex) {

                ExceptionHandlers.ResponseCallbackExceptionHandler<T> (cea, requestState, ex, channel, channelCallbacks, PubnubErrorLevel);
                
            }
        }

        protected void MultiplexExceptionHandler<T> (ResponseType type, string[] channels, Action<T> userCallback, 
            Action<T> connectCallback, Action<PubnubClientError> errorCallback, bool reconnectMaxTried, bool reconnect)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, in MultiplexExceptionHandler responsetype={1}", DateTime.Now.ToString (), type.ToString ()), LoggingMethod.LevelInfo);

            string channel = "";
            if (channels != null) {
                channel = string.Join (",", channels);
            }
            #endif

            if (reconnectMaxTried) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MAX retries reached. Exiting the subscribe for channel(s) = {1}", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);
                #endif

                string[] activeChannels = multiChannelSubscribe.Keys.ToArray<string> ();
                MultiChannelUnSubscribeInit<T> (ResponseType.Unsubscribe, string.Join (",", activeChannels), null, null, null, null);

                Helpers.CheckSubscribedChannelsAndSendCallbacks<T> (activeChannels.Where (filterChannel => !filterChannel.Contains (Utility.PresenceChannelSuffix)).ToArray (), 
                    false, type, NetworkCheckMaxRetries, channelCallbacks, PubnubErrorLevel);
                Helpers.CheckSubscribedChannelsAndSendCallbacks<T> (activeChannels.Where (filterChannel => filterChannel.Contains (Utility.PresenceChannelSuffix)).ToArray (), 
                    true, type, NetworkCheckMaxRetries, channelCallbacks, PubnubErrorLevel);

            } else {
                if (!internetStatus) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Subscribe channel={1} - No internet connection. ", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);
                    #endif
                    return;
                }

                List<object> result = new List<object> ();
                result.Add ("0");
                if (!EnableResumeOnReconnect && reconnect) {
                    result.Add (0); //send 0 time token to enable presence event
                } else {
                    result.Add (lastSubscribeTimetoken); //get last timetoken
                }
                result.Add (channels); //send channel name

                MultiplexInternalCallback<T> (type, result, userCallback, connectCallback, errorCallback, reconnect);
            }
        }

        private void HandleMultiplexException<T> (object sender, EventArgs ea)
        {
            MultiplexExceptionEventArgs<T> mea = ea as MultiplexExceptionEventArgs<T>;
            MultiplexExceptionHandler<T> (mea.responseType, mea.channels, mea.userCallback, 
                mea.connectCallback, mea.errorCallback, mea.reconnectMaxTried, mea.resumeOnReconnect);
        }

        #endregion

        #region "User State"

        private string AddOrUpdateOrDeleteLocalUserState (string channel, string userStateKey, object userStateValue)
        {
            string retJsonUserState = "";

            Dictionary<string, object> userStateDictionary = null;

            if (channelLocalUserState.ContainsKey (channel)) {
                userStateDictionary = channelLocalUserState [channel];
                if (userStateDictionary != null) {
                    if (userStateDictionary.ContainsKey (userStateKey)) {
                        if (userStateValue != null) {
                            userStateDictionary [userStateKey] = userStateValue;
                        } else {
                            userStateDictionary.Remove (userStateKey);
                        }
                    } else {
                        if (!string.IsNullOrEmpty (userStateKey) && userStateKey.Trim ().Length > 0 && userStateValue != null) {
                            userStateDictionary.Add (userStateKey, userStateValue);
                        }
                    }
                } else {
                    userStateDictionary = new Dictionary<string, object> ();
                    userStateDictionary.Add (userStateKey, userStateValue);
                }

                channelLocalUserState.AddOrUpdate (channel, userStateDictionary, (oldData, newData) => userStateDictionary);
            } else {
                if (!string.IsNullOrEmpty (userStateKey) && userStateKey.Trim ().Length > 0 && userStateValue != null) {
                    userStateDictionary = new Dictionary<string, object> ();
                    userStateDictionary.Add (userStateKey, userStateValue);

                    channelLocalUserState.AddOrUpdate (channel, userStateDictionary, (oldData, newData) => userStateDictionary);
                }
            }

            string jsonUserState = BuildJsonUserState (channel, true);
            if (jsonUserState != "") {
                retJsonUserState = string.Format ("{{{0}}}", jsonUserState);
            }
            return retJsonUserState;
        }

        private bool DeleteLocalUserState (string channel)
        {
            bool userStateDeleted = false;

            if (channelLocalUserState.ContainsKey (channel)) {
                Dictionary<string, object> returnedUserState = null;
                userStateDeleted = channelLocalUserState.TryRemove (channel, out returnedUserState);
            }

            return userStateDeleted;
        }

        private string BuildJsonUserState (string channel, bool local)
        {
            Dictionary<string, object> userStateDictionary = null;

            if (local) {
                if (channelLocalUserState.ContainsKey (channel)) {
                    userStateDictionary = channelLocalUserState [channel];
                }
            } else {
                if (channelUserState.ContainsKey (channel)) {
                    userStateDictionary = channelUserState [channel];
                }
            }

            StringBuilder jsonStateBuilder = new StringBuilder ();

            if (userStateDictionary != null) {
                string[] userStateKeys = userStateDictionary.Keys.ToArray<string> ();

                for (int keyIndex = 0; keyIndex < userStateKeys.Length; keyIndex++) {
                    string useStateKey = userStateKeys [keyIndex];
                    object userStateValue = userStateDictionary [useStateKey];
                    if (userStateValue == null) {
                        jsonStateBuilder.AppendFormat ("\"{0}\":{1}", useStateKey, string.Format ("\"{0}\"", "null"));
                    } else {
                        jsonStateBuilder.AppendFormat ("\"{0}\":{1}", useStateKey, (userStateValue.GetType ().ToString () == "System.String") ? string.Format ("\"{0}\"", userStateValue) : userStateValue);
                    }
                    if (keyIndex < userStateKeys.Length - 1) {
                        jsonStateBuilder.Append (",");
                    }
                }
            }

            return jsonStateBuilder.ToString ();
        }

        private string BuildJsonUserState (string[] channels, bool local)
        {
            string retJsonUserState = "";

            StringBuilder jsonStateBuilder = new StringBuilder ();

            if (channels != null) {
                for (int index = 0; index < channels.Length; index++) {
                    string currentJsonState = BuildJsonUserState (channels [index].ToString (), local);
                    if (!string.IsNullOrEmpty (currentJsonState)) {
                        currentJsonState = string.Format ("\"{0}\":{{{1}}}", channels [index].ToString (), currentJsonState);
                        if (jsonStateBuilder.Length > 0) {
                            jsonStateBuilder.Append (",");
                        }
                        jsonStateBuilder.Append (currentJsonState);
                    }
                }

                if (jsonStateBuilder.Length > 0) {
                    retJsonUserState = string.Format ("{{{0}}}", jsonStateBuilder.ToString ());
                }
            }

            return retJsonUserState;
        }

        private string SetLocalUserState (string channel, string userStateKey, int userStateValue)
        {
            return AddOrUpdateOrDeleteLocalUserState (channel, userStateKey, userStateValue);
        }

        private string SetLocalUserState (string channel, string userStateKey, double userStateValue)
        {
            return AddOrUpdateOrDeleteLocalUserState (channel, userStateKey, userStateValue);
        }

        private string SetLocalUserState (string channel, string userStateKey, string userStateValue)
        {
            return AddOrUpdateOrDeleteLocalUserState (channel, userStateKey, userStateValue);
        }

        internal string GetLocalUserState (string channel)
        {
            string retJsonUserState = "";
            StringBuilder jsonStateBuilder = new StringBuilder ();

            jsonStateBuilder.Append (BuildJsonUserState (channel, false));
            if (jsonStateBuilder.Length > 0) {
                retJsonUserState = string.Format ("{{{0}}}", jsonStateBuilder.ToString ());
            }

            return retJsonUserState;
        }

        private void SharedSetUserState<T> (string channel, string uuid, string jsonUserState, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            if (string.IsNullOrEmpty (uuid)) {
                uuid = this.SessionUUID;
            }

            Dictionary<string, object> deserializeUserState = JsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonUserState);
            if (channelUserState != null) {
                channelUserState.AddOrUpdate (channel.Trim (), deserializeUserState, (oldState, newState) => deserializeUserState);
            }
            if (channelLocalUserState != null) {
                channelLocalUserState.AddOrUpdate (channel.Trim (), deserializeUserState, (oldState, newState) => deserializeUserState);
            }

            Uri request = BuildRequests.BuildSetUserStateRequest (channel, jsonUserState, this.SessionUUID,
                this.ssl, this.Origin, authenticationKey, this.subscribeKey);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (new string[] { channel }, ResponseType.SetUserState, false, userCallback, null, errorCallback, 0, false, 0, null);

            UrlProcessRequest<T> (request, requestState);

            //bounce the long-polling subscribe requests to update user state
            TerminateCurrentSubscriberRequest<T> ();
        }

        #endregion

        #region "Helpers"

        void ParseReceiedTimetoken<T> (RequestState<T> requestState, List<object> result)
        {
            long receivedTimetoken = (result.Count > 1) ? Convert.ToInt64 (result [1].ToString ()) : 0;
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, receivedTimetoken = {1}", DateTime.Now.ToString (), receivedTimetoken), LoggingMethod.LevelInfo);
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

        private void RunRequests<T> (Uri requestUri, RequestState<T> pubnubRequestState, string channel)
        {
            if (pubnubRequestState.respType == ResponseType.Subscribe || pubnubRequestState.respType == ResponseType.Presence) {
                channelRequest.AddOrUpdate (channel, pubnubRequestState.Request, (key, oldState) => pubnubRequestState.Request);
                RequestState<T> pubnubRequestStateHB = pubnubRequestState;
                pubnubRequestStateHB.ID = DateTime.Now.Ticks;

                RunHeartbeat<T> (false, LocalClientHeartbeatInterval, pubnubRequestStateHB);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Heartbeat started", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                #endif
                if (pubnubRequestStateHB.Channels != null 
                    && pubnubRequestStateHB.Channels.Length > 0 
                    && (pubnubRequestStateHB.Channels.Where (s => s.Contains (Utility.PresenceChannelSuffix) == false).ToArray ().Length > 0) 
                    && (PresenceHeartbeatInterval > 0)){

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
            string channel = "";
            if (pubnubRequestState != null && pubnubRequestState.Channels != null) {
                channel = string.Join (",", pubnubRequestState.Channels);
            }

            try {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, In Url process request", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                #endif
                if (!channelRequest.ContainsKey (channel) && (pubnubRequestState.respType == ResponseType.Subscribe || pubnubRequestState.respType == ResponseType.Presence)) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, In Url process request check", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                    #endif
                    return false;
                }

                RunRequests<T> (requestUri, pubnubRequestState, channel);

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

                    PubnubCallbacks.CallErrorCallback<T> (ex, pubnubRequestState, channel, 
                        PubnubErrorCode.None, PubnubErrorSeverity.Critical, pubnubRequestState.ErrorCallback, PubnubErrorLevel);
                }
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} Exception={1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif

                ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, pubnubRequestState.respType, pubnubRequestState.Channels, 
                    false, pubnubRequestState.UserCallback, pubnubRequestState.ConnectCallback, 
                    pubnubRequestState.ErrorCallback, false, PubnubErrorLevel);
                return false;
            }
            return true;
        }

        private long AbortPreviousRequestAndFetchTimetoken<T>(string[] currentChannels, List<string> validChannels, 
            long lastSubscribeTimetokenForNewMultiplexChannel)
        {
            string multiChannelName = string.Join(",", currentChannels);
            if (channelRequest.ContainsKey(multiChannelName))
            {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, Aborting previous subscribe/presence requests having channel(s)={1}", DateTime.Now.ToString(), multiChannelName), LoggingMethod.LevelInfo);
                #endif
                channelRequest[multiChannelName] = null;
                PubnubWebRequest removedRequest;
                bool removedChannel = channelRequest.TryRemove(multiChannelName, out removedRequest);
                #if (ENABLE_PUBNUB_LOGGING)
                if (removedChannel)
                {
                    LoggingMethod.WriteToLog(string.Format("DateTime {0}, Success to remove channel(s)={1} from _channelRequest (MultiChannelUnSubscribeInit).", DateTime.Now.ToString(), multiChannelName), LoggingMethod.LevelInfo);
                }
                else
                {
                    LoggingMethod.WriteToLog(string.Format("DateTime {0}, Unable to remove channel(s)={1} from _channelRequest (MultiChannelUnSubscribeInit).", DateTime.Now.ToString(), multiChannelName), LoggingMethod.LevelInfo);
                }
                #endif
                lastSubscribeTimetokenForNewMultiplexChannel = lastSubscribeTimetoken;
                RequestState<T> reqState = StoredRequestState.Instance.GetStoredRequestState(CurrentRequestType.Subscribe) as RequestState<T>;
                coroutine.BounceRequest<T>(CurrentRequestType.Subscribe, reqState, false);
            }
            #if (ENABLE_PUBNUB_LOGGING)
            else
            {
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, Unable to capture channel(s)={1} from _channelRequest to abort request.", DateTime.Now.ToString(), multiChannelName), LoggingMethod.LevelInfo);
            }
            #endif
            return lastSubscribeTimetokenForNewMultiplexChannel;
        }

        List<string> UnsubscribeChannels<T>(string channel, List<string> validChannels)
        {
            Uri request = BuildRequests.BuildMultiChannelLeaveRequest(validChannels.ToArray(), this.SessionUUID, 
				this.ssl, this.Origin, authenticationKey, this.subscribeKey);
            RequestState<T> requestState = BuildRequests.BuildRequestState<T>(new string[] {
                channel
            }, ResponseType.Leave, false, null, null, null, 0, false, 0, null);
            UrlProcessRequest<T>(request, requestState);
            return validChannels;
        }

        void RemoveUnsubscribedChannelsAndDeleteUserState<T>(Action<T> disconnectCallback, Action<PubnubClientError> errorCallback, List<string> validChannels)
        {
            for (int index = 0; index < validChannels.Count; index++)
            {
                long timetokenValue;
                string channelToBeRemoved = validChannels[index].ToString();
                bool unsubscribeStatus = multiChannelSubscribe.TryRemove(channelToBeRemoved, out timetokenValue);
                if (unsubscribeStatus)
                {
                    string jsonString = string.Format("{0} Unsubscribed from {1}", (Utility.IsPresenceChannel(channelToBeRemoved)) ? "Presence" : "", channelToBeRemoved.Replace(Utility.PresenceChannelSuffix, ""));
                    List<object> result = Helpers.CreateJsonResponse(jsonString, channelToBeRemoved.Replace(Utility.PresenceChannelSuffix, ""), JsonPluggableLibrary);
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog(string.Format("DateTime {0}, JSON response={1}", DateTime.Now.ToString(), jsonString), LoggingMethod.LevelInfo);
                    #endif
                    PubnubCallbacks.GoToCallback<T>(result, disconnectCallback, JsonPluggableLibrary);
                    DeleteLocalUserState(channelToBeRemoved);
                }
                else
                {
                    string message = "Unsubscribe Error. Please retry the unsubscribe operation.";
                    PubnubErrorCode errorType = (Utility.IsPresenceChannel(channelToBeRemoved)) ? PubnubErrorCode.PresenceUnsubscribeFailed : PubnubErrorCode.UnsubscribeFailed;
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog(string.Format("DateTime {0}, channel={1} unsubscribe error", DateTime.Now.ToString(), channelToBeRemoved), LoggingMethod.LevelInfo);
                    #endif
                    PubnubCallbacks.CallErrorCallback<T>(message, null, channelToBeRemoved, errorType, PubnubErrorSeverity.Critical, errorCallback, PubnubErrorLevel);
                }
            }
        }

        void ContinueToSubscribeRestOfChannels<T>(ResponseType type, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback)
        {
            string[] channels = multiChannelSubscribe.Keys.ToArray<string>();
            if (channels != null && channels.Length > 0)
            {
                RequestState<T> state = new RequestState<T>();
                channelRequest.AddOrUpdate(string.Join(",", channels), state.Request, (key, oldValue) => state.Request);
                ResetInternetCheckSettings(channels);
                //Modify the value for type ResponseType. Presence or Subscrie is ok, but sending the close value would make sense
                if (string.Join(",", channels).IndexOf(Utility.PresenceChannelSuffix) > 0)
                {
                    type = ResponseType.Presence;
                }
                else
                {
                    type = ResponseType.Subscribe;
                }
                //Continue with any remaining channels for subscribe/presence
                MultiChannelSubscribeRequest<T>(type, channels, 0, userCallback, connectCallback, errorCallback, false);
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

        private void MultiChannelUnSubscribeInit<T> (ResponseType type, string channel, Action<T> userCallback, Action<T> connectCallback, Action<T> disconnectCallback, Action<PubnubClientError> errorCallback)
        {
            string[] rawChannels = channel.Split (',');
            List<string> validChannels = Helpers.GetValidChannels<T>(type, errorCallback, rawChannels, multiChannelSubscribe, PubnubErrorLevel);

            if (validChannels.Count > 0) {
                //Retrieve the current channels already subscribed previously and terminate them
                string[] currentChannels = multiChannelSubscribe.Keys.ToArray<string> ();
                if (currentChannels != null && currentChannels.Length > 0) {
                    AbortPreviousRequestAndFetchTimetoken<T>(currentChannels, validChannels, 0);

                    if (type == ResponseType.Unsubscribe) {
                        validChannels = UnsubscribeChannels<T>(channel, validChannels);
                    }
                }
                    
                //Remove the valid channels from subscribe list for unsubscribe 
                RemoveUnsubscribedChannelsAndDeleteUserState<T>(disconnectCallback, errorCallback, validChannels);

                //Get all the channels
                ContinueToSubscribeRestOfChannels<T>(type, userCallback, connectCallback, errorCallback);
            }

        }

        private void ResetInternetCheckSettings (string[] channels)
        {
            if (channels == null)
                return;
            retryCount = 0;
            internetStatus = true;
            retriesExceeded = false;
        }

        void AddValidChannelsToChannelCallbacks<T>(List<string> validChannels, ResponseType type, Action<T> userCallback, 
            Action<T> connectCallback, Action<PubnubClientError> errorCallback)
        {
            //Add the valid channels to the channels subscribe list for tracking
            for (int index = 0; index < validChannels.Count; index++)
            {
                string currentLoopChannel = validChannels[index].ToString();
                multiChannelSubscribe.GetOrAdd(currentLoopChannel, 0);
                PubnubChannelCallbackKey callbackKey = new PubnubChannelCallbackKey();
                callbackKey.Channel = currentLoopChannel;
                callbackKey.Type = type;
                PubnubChannelCallback<T> pubnubChannelCallbacks = new PubnubChannelCallback<T>();
                pubnubChannelCallbacks.Callback = userCallback;
                pubnubChannelCallbacks.ConnectCallback = connectCallback;
                pubnubChannelCallbacks.ErrorCallback = errorCallback;
                channelCallbacks.AddOrUpdate(callbackKey, pubnubChannelCallbacks, (key, oldValue) => pubnubChannelCallbacks);
            }
        }

        public void MultiChannelSubscribeInit<T> (ResponseType type, string channel, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback)
        {
            string[] rawChannels = channel.Split (',');
            List<string> validChannels = new List<string> ();

            ResetInternetCheckSettings (rawChannels);

            bool networkConnection = internetStatus;

            validChannels = Helpers.RemoveDuplicateChannelsAndCheckForAlreadySubscribedChannels<T>(type, channel, 
                errorCallback, rawChannels, validChannels, networkConnection, multiChannelSubscribe, PubnubErrorLevel);

            if (validChannels.Count > 0) {
                string[] currentChannels = multiChannelSubscribe.Keys.ToArray<string>();
                if (currentChannels != null && currentChannels.Length > 0)
                {
                    lastSubscribeTimetokenForNewMultiplex = AbortPreviousRequestAndFetchTimetoken<T>(currentChannels, validChannels, 0);
                }
                AddValidChannelsToChannelCallbacks<T>(validChannels, type, userCallback, connectCallback, errorCallback);

                //Get all the channels
                string[] channels = multiChannelSubscribe.Keys.ToArray<string> ();

                RequestState<T> state = new RequestState<T> ();
                channelRequest.AddOrUpdate (string.Join (",", channels), state.Request, (key, oldValue) => state.Request);
                MultiChannelSubscribeRequest<T> (type, channels, lastSubscribeTimetokenForNewMultiplex, userCallback, connectCallback, errorCallback, false);
            }
        }

        private bool CheckAllChannelsAreUnsubscribed<T>()
        {
            if (multiChannelSubscribe != null && multiChannelSubscribe.Count <= 0)
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

        private bool CheckSystemActiveAndRetriesExceeded<T>(ResponseType type, string[] channels, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback, string multiChannel)
        {
            if (pubnetSystemActive && retriesExceeded)
            {
            #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, Subscribe channel={1} - No internet connection. MAXed retries for internet ", DateTime.Now.ToString(), multiChannel), LoggingMethod.LevelInfo);
                #endif
                MultiplexExceptionHandler<T>(type, channels, userCallback, connectCallback, errorCallback, true, false);
                retriesExceeded = false;
                return true;
            }
            return false;
        }

        long SaveLastTimetoken(object timetoken)
        {
            long lastTimetoken = 0;
            long sentTimetoken = Convert.ToInt64(timetoken.ToString());
            long minimumTimetoken = multiChannelSubscribe.Min(token => token.Value);
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, lastSubscribeTimetokenForNewMultiplex={1}", DateTime.Now.ToString(), lastSubscribeTimetokenForNewMultiplex), LoggingMethod.LevelInfo);
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, sentTimetoken={1}", DateTime.Now.ToString(), sentTimetoken), LoggingMethod.LevelInfo);
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, lastSubscribeTimetoken={1}", DateTime.Now.ToString(), lastSubscribeTimetoken), LoggingMethod.LevelInfo);
            #endif
            if (minimumTimetoken == 0 || uuidChanged)
            {
                lastTimetoken = 0;
                uuidChanged = false;
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
        private void MultiChannelSubscribeRequest<T> (ResponseType type, string[] channels, object timetoken, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback, bool reconnect)
        {
            //Exit if the channel is unsubscribed
            if (CheckAllChannelsAreUnsubscribed<T>())
            {
                return;
            }

            string multiChannel = string.Join (",", channels);

            if (Helpers.CheckChannelsInMultiChannelSubscribeRequest(multiChannel, multiChannelSubscribe, channelRequest))
            {
                return;
            }

            if (CheckSystemActiveAndRetriesExceeded<T>(type, channels, userCallback, connectCallback, errorCallback, multiChannel))
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
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Building request for channel(s)={1} with timetoken={2}", DateTime.Now.ToString (), string.Join (",", channels), lastTimetoken), LoggingMethod.LevelInfo);
                #endif
                // Build URL
                string channelsJsonState = BuildJsonUserState (channels, false);

                Uri requestUrl = BuildRequests.BuildMultiChannelSubscribeRequest (channels, lastTimetoken, channelsJsonState, this.SessionUUID,
                    this.ssl, this.Origin, authenticationKey, this.subscribeKey);

                RequestState<T> pubnubRequestState = BuildRequests.BuildRequestState<T> (channels, type, reconnect, userCallback as Action<T>, connectCallback as Action<T>, errorCallback, 0, false, Convert.ToInt64 (timetoken.ToString ()), typeof(T));
                // Wait for message
                ExceptionHandlers.MultiplexException += HandleMultiplexException<T>;
                UrlProcessRequest<T> (requestUrl, pubnubRequestState);
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} method:_subscribe \n channel={1} \n timetoken={2} \n Exception Details={3}", DateTime.Now.ToString (), string.Join (",", channels), timetoken.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
                PubnubCallbacks.CallErrorCallback<T> (ex, null, string.Join (",", channels), 
                    PubnubErrorCode.None, PubnubErrorSeverity.Critical, errorCallback, PubnubErrorLevel);

                this.MultiChannelSubscribeRequest<T> (type, channels, timetoken, userCallback, connectCallback, errorCallback, false);
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
                string multiChannel = string.Join (",", pubnubRequestState.Channels);

                PubnubCallbacks.CallErrorCallback<T> (cbMessage, null, multiChannel, 
                    PubnubErrorCode.NoInternetRetryConnect, PubnubErrorSeverity.Warn, pubnubRequestState.ErrorCallback, PubnubErrorLevel);

            } else {
                retriesExceeded = true;
                string cbMessage = string.Format ("DateTime {0} Internet Disconnected. Retries exceeded {1}. Unsubscribing connected channels.", DateTime.Now.ToString (), NetworkCheckMaxRetries);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (cbMessage, LoggingMethod.LevelError);
                #endif

                //stop heartbeat.
                keepHearbeatRunning = false;
                coroutine.BounceRequest<T> (CurrentRequestType.Subscribe, pubnubRequestState, false);

                string multiChannel = string.Join (",", pubnubRequestState.Channels);
                PubnubCallbacks.CallErrorCallback<T> (cbMessage, null, multiChannel, 
                    PubnubErrorCode.NoInternetRetryConnect, PubnubErrorSeverity.Warn, pubnubRequestState.ErrorCallback, PubnubErrorLevel);

                string[] activeChannels = multiChannelSubscribe.Keys.ToArray<string> ();
                MultiplexExceptionHandler<T> (ResponseType.Subscribe, activeChannels, pubnubRequestState.UserCallback, pubnubRequestState.ConnectCallback, pubnubRequestState.ErrorCallback, true, false);
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
        private void MultiplexInternalCallback<T> (ResponseType type, object multiplexResult, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback, bool reconnect)
        {
            List<object> message = multiplexResult as List<object>;
            string[] channels = null;
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
                MultiChannelSubscribeRequest<T> (type, channels, (object)message [1], userCallback, connectCallback, errorCallback, reconnect);
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MultiplexInternalCallback message null or count < 3", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            }
            #endif    
        }

        #endregion

    }
}