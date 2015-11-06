//Build Date: Oct 28, 2015
//ver3.6.9.0/Unity5
#if (UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_ANDROID || UNITY_IOS || UNITY_5 || UNITY_WEBGL)
#define USE_JSONFX_UNITY_IOS
//#define USE_MiniJSON
#endif

#if (__MonoCS__ && !UNITY_STANDALONE && !UNITY_WEBPLAYER)
#define TRACE
#endif

#if (USE_JSONFX_UNITY_IOS)
using Pathfinding.Serialization.JsonFx;
#elif (USE_MiniJSON)
using MiniJSON;
#endif
    
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Net;
using System.ComponentModel;

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
        private bool ssl = false;
        private static long lastSubscribeTimetoken = 0;
        private static long lastSubscribeTimetokenForNewMultiplex = 0;
        private string build = "3.6.9.0";
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

            #if (USE_MiniJSON)
            LoggingMethod.WriteToLog("USE_MiniJSON", LoggingMethod.LevelInfo);
            JsonPluggableLibrary = new MiniJSONObjectSerializer();
            #elif (USE_JSONFX_UNITY_IOS)
            LoggingMethod.WriteToLog ("USE_JSONFX_UNITY_IOS", LoggingMethod.LevelInfo);
            JsonPluggableLibrary = new JsonFxUnitySerializer ();
            #endif

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
            LoggingMethod.WriteToLog (Version, LoggingMethod.LevelInfo);

            if (gobj == null) {
                LoggingMethod.WriteToLog ("Initilizing new GameObject", LoggingMethod.LevelInfo);
                gobj = new GameObject ();  
                localGobj = true;
            } else {
                LoggingMethod.WriteToLog ("Reusing already initialized GameObject", LoggingMethod.LevelInfo);
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

            VerifyOrSetSessionUUID ();
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
            if (string.IsNullOrEmpty (channel) || string.IsNullOrEmpty (channel.Trim ())) {
                throw new ArgumentException ("Missing Channel");
            }
            if (userCallback == null) {
                throw new ArgumentException ("Missing userCallback");
            }
            if (connectCallback == null) {
                throw new ArgumentException ("Missing connectCallback");
            }
            if (errorCallback == null) {
                throw new ArgumentException ("Missing errorCallback");
            }
			if (JsonPluggableLibrary == null) {
                throw new NullReferenceException ("Missing Json Pluggable Library for Pubnub Instance");
            }

            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, requested subscribe for channel={1}", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);

            MultiChannelSubscribeInit<T> (ResponseType.Subscribe, channel, userCallback, connectCallback, errorCallback);
        }

        #endregion

        #region "Publish"

        public bool Publish<T> (string channel, object message, bool storeInHistory, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            if (string.IsNullOrEmpty (channel) || string.IsNullOrEmpty (channel.Trim ()) || message == null) {
                throw new ArgumentException ("Missing Channel or Message");
            }

            if (string.IsNullOrEmpty (this.publishKey) || string.IsNullOrEmpty (this.publishKey.Trim ()) || this.publishKey.Length <= 0) {
                throw new MissingMemberException ("Invalid publish key");
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

			string originalMessage = (enableJsonEncodingForPublish) ? JsonEncodePublishMsg (message) : message.ToString ();

			Uri request = Helpers.BuildPublishRequest (channel, originalMessage, storeInHistory, this.SessionUUID,
				this.ssl, this.Origin, this.AuthenticationKey, Version,
				this.publishKey, this.subscribeKey, this.cipherKey, this.secretKey);

            RequestState<T> requestState = Helpers.BuildRequestState<T> (new string[] { channel }, ResponseType.Publish, 
                false, userCallback, null, errorCallback, 0, false, 0, null);

            return UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "Presence"

        public void Presence<T> (string channel, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback)
        {
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

            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, requested presence for channel={1}", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);

            MultiChannelSubscribeInit<T> (ResponseType.Presence, channel, userCallback, connectCallback, errorCallback);
        }

        #endregion

        #region "Detailed History"

        public bool DetailedHistory<T> (string channel, long start, long end, int count, bool reverse, bool includeToken, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
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


			Uri request = Helpers.BuildDetailedHistoryRequest (channel, start, end, count, reverse, includeToken, this.SessionUUID,
				this.ssl, this.Origin, this.AuthenticationKey, Version, 
				this.subscribeKey);

            RequestState<T> requestState = Helpers.BuildRequestState<T> (new string[] { channel }, ResponseType.DetailedHistory, false, userCallback, null, errorCallback, 0, false, 0, null);

            return UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "HereNow"

        public bool HereNow<T> (string channel, bool showUUIDList, bool includeUserState, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
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

			Uri request = Helpers.BuildHereNowRequest (channel, showUUIDList, includeUserState, this.SessionUUID,
				this.ssl, this.Origin, this.AuthenticationKey, Version, this.subscribeKey);

            RequestState<T> requestState = Helpers.BuildRequestState<T> (new string[] { channel }, ResponseType.HereNow, false, userCallback, null, errorCallback, 0, false, 0, null);

            return UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "Global Here Now"

        public bool GlobalHereNow<T> (bool showUUIDList, bool includeUserState, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            if (userCallback == null) {
                throw new ArgumentException ("Missing userCallback");
            }
            if (errorCallback == null) {
                throw new ArgumentException ("Missing errorCallback");
            }
			if (JsonPluggableLibrary == null) {
                throw new NullReferenceException ("Missing Json Pluggable Library for Pubnub Instance");
            }

			Uri request = Helpers.BuildGlobalHereNowRequest (showUUIDList, includeUserState, this.SessionUUID,
				this.ssl, this.Origin, this.AuthenticationKey, Version, this.subscribeKey);

            RequestState<T> requestState = Helpers.BuildRequestState<T> (null, ResponseType.GlobalHereNow, false, userCallback, null, errorCallback, 0, false, 0, null);

            return UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "WhereNow"

        public void WhereNow<T> (string uuid, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            if (userCallback == null) {
                throw new ArgumentException ("Missing userCallback");
            }
            if (errorCallback == null) {
                throw new ArgumentException ("Missing errorCallback");
            }
			if (JsonPluggableLibrary == null) {
                throw new NullReferenceException ("Missing Json Pluggable Library for Pubnub Instance");
            }

            if (string.IsNullOrEmpty (uuid)) {
                uuid = this.SessionUUID;
            }
			Uri request = Helpers.BuildWhereNowRequest (uuid, this.SessionUUID,
				this.ssl, this.Origin, this.AuthenticationKey, Version, this.subscribeKey);

            RequestState<T> requestState = Helpers.BuildRequestState<T> (new string[] { uuid }, ResponseType.WhereNow, false, userCallback, null, errorCallback, 0, false, 0, null);

            UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "Unsubscribe Presence And Subscribe"

        public void PresenceUnsubscribe<T> (string channel, Action<T> userCallback, Action<T> connectCallback, Action<T> disconnectCallback, Action<PubnubClientError> errorCallback)
        {
            if (string.IsNullOrEmpty (channel) || string.IsNullOrEmpty (channel.Trim ())) {
                throw new ArgumentException ("Missing Channel");
            }
            if (userCallback == null) {
                throw new ArgumentException ("Missing userCallback");
            }
            if (connectCallback == null) {
                throw new ArgumentException ("Missing connectCallback");
            }
            if (disconnectCallback == null) {
                throw new ArgumentException ("Missing disconnectCallback");
            }
            if (errorCallback == null) {
                throw new ArgumentException ("Missing errorCallback");
            }
			if (JsonPluggableLibrary == null) {
                throw new NullReferenceException ("Missing Json Pluggable Library for Pubnub Instance");
            }

            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, requested presence-unsubscribe for channel(s)={1}", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);
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
            if (string.IsNullOrEmpty (channel) || string.IsNullOrEmpty (channel.Trim ())) {
                throw new ArgumentException ("Missing Channel");
            }
            if (userCallback == null) {
                throw new ArgumentException ("Missing userCallback");
            }
            if (connectCallback == null) {
                throw new ArgumentException ("Missing connectCallback");
            }
            if (disconnectCallback == null) {
                throw new ArgumentException ("Missing disconnectCallback");
            }
            if (errorCallback == null) {
                throw new ArgumentException ("Missing errorCallback");
            }
			if (JsonPluggableLibrary == null) {
                throw new NullReferenceException ("Missing Json Pluggable Library for Pubnub Instance");
            }

            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, requested unsubscribe for channel(s)={1}", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);
            MultiChannelUnSubscribeInit<T> (ResponseType.Unsubscribe, channel, userCallback, connectCallback, disconnectCallback, errorCallback);

        }

        #endregion

        #region "Time"

        public bool Time<T> (Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            if (userCallback == null) {
                throw new ArgumentException ("Missing userCallback");
            }
            if (errorCallback == null) {
                throw new ArgumentException ("Missing errorCallback");
            }
			if (JsonPluggableLibrary == null) {
                throw new NullReferenceException ("Missing Json Pluggable Library for Pubnub Instance");
            }

			Uri request = Helpers.BuildTimeRequest (this.SessionUUID, this.ssl, this.Origin, Version);

            RequestState<T> requestState = Helpers.BuildRequestState<T> (null, ResponseType.Time, false, userCallback, null, errorCallback, 0, false, 0, null);
            return UrlProcessRequest<T> (request, requestState); 
        }

        #endregion

        #region "PAM"

        #region "Grant Access"

        public bool GrantAccess<T> (string channel, string authenticationKey, bool read, bool write, int ttl, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            if (string.IsNullOrEmpty (this.secretKey) || string.IsNullOrEmpty (this.secretKey.Trim ()) || this.secretKey.Length <= 0) {
                throw new MissingMemberException ("Invalid secret key");
            }

			Uri request = Helpers.BuildGrantAccessRequest (channel, read, write, ttl, this.SessionUUID,
				this.ssl, this.Origin, authenticationKey, Version, 
				this.publishKey, this.subscribeKey, this.cipherKey, this.secretKey);

            RequestState<T> requestState = Helpers.BuildRequestState<T> (new string[] { channel }, ResponseType.GrantAccess, false, userCallback, null, errorCallback, 0, false, 0, null);

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
                        multiChannels [index] = string.Format ("{0}-pnpres", multiChannels [index]);
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
            if (string.IsNullOrEmpty (this.secretKey) || string.IsNullOrEmpty (this.secretKey.Trim ()) || this.secretKey.Length <= 0) {
                throw new MissingMemberException ("Invalid secret key");
            }

			Uri request = Helpers.BuildAuditAccessRequest (channel, this.SessionUUID,
				this.ssl, this.Origin, authenticationKey, Version, 
				this.publishKey, this.subscribeKey, this.cipherKey, this.secretKey);

            RequestState<T> requestState = Helpers.BuildRequestState<T> (!string.IsNullOrEmpty (channel)? new string[] { channel } : null, ResponseType.AuditAccess, false, userCallback, null, errorCallback, 0, false, 0, null);

            UrlProcessRequest<T> (request, requestState);
        }

        #endregion

        #region "Audit Presence"

        public void AuditPresenceAccess<T> (string channel, string authenticationKey, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            string[] multiChannels = channel.Split (',');
            if (multiChannels.Length > 0) {
                for (int index = 0; index < multiChannels.Length; index++) {
                    multiChannels [index] = string.Format ("{0}-pnpres", multiChannels [index]);
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
            if (string.IsNullOrEmpty (channel) || string.IsNullOrEmpty (channel.Trim ())) {
                throw new ArgumentException ("Missing Channel");
            }
            if (string.IsNullOrEmpty (jsonUserState) || string.IsNullOrEmpty (jsonUserState.Trim ())) {
                throw new ArgumentException ("Missing User State");
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

						PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Info, PubnubMessageSource.Client,
                            channel, errorCallback, message, PubnubErrorCode.UserStateUnchanged, null, null);
                        return;
                    }

                }
            }

            SharedSetUserState (channel, uuid, jsonUserState, userCallback, errorCallback);
        }

        public void SetUserState<T> (string channel, string uuid, KeyValuePair<string, object> keyValuePair, Action<T> userCallback, Action<PubnubClientError> errorCallback)
        {
            if (string.IsNullOrEmpty (channel) || string.IsNullOrEmpty (channel.Trim ())) {
                throw new ArgumentException ("Missing Channel");
            }
            if (userCallback == null) {
                throw new ArgumentException ("Missing userCallback");
            }
            if (errorCallback == null) {
                throw new ArgumentException ("Missing errorCallback");
            }

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

				PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Info, PubnubMessageSource.Client,
                    channel, errorCallback, message, PubnubErrorCode.UserStateUnchanged, null, null);
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

            if (string.IsNullOrEmpty (uuid)) {
                uuid = this.sessionUUID;
            }

			Uri request = Helpers.BuildGetUserStateRequest (channel, this.SessionUUID,
				this.ssl, this.Origin, authenticationKey, Version, this.subscribeKey);

            RequestState<T> requestState = Helpers.BuildRequestState<T> (new string[] { channel }, ResponseType.GetUserState, false, userCallback, null, errorCallback, 0, false, 0, null);

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
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} ending open requests.", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            RequestState<string> reqStateSub = StoredRequestState.Instance.GetStoredRequestState (CurrentRequestType.Subscribe) as RequestState<string>;
            coroutine.BounceRequest<string> (CurrentRequestType.Subscribe, reqStateSub, false);
            RequestState<string> reqStateNonSub = StoredRequestState.Instance.GetStoredRequestState (CurrentRequestType.NonSubscribe) as RequestState<string>;
            coroutine.BounceRequest<string> (CurrentRequestType.NonSubscribe, reqStateNonSub, false);

            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Request bounced.", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            StopHeartbeat ();
            StopPresenceHeartbeat ();
            RemoveChannelCallback ();
            RemoveUserState ();
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} RemoveChannelCallback and RemoveUserState complete.", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
        }

        public void CleanUp (){
            LoggingMethod.WriteToLog ("Destructing coroutine", LoggingMethod.LevelInfo);
            if (coroutine != null) {
                UnityEngine.Object.Destroy (coroutine);
            }
            LoggingMethod.WriteToLog ("Destructing GameObject", LoggingMethod.LevelInfo);
            if(localGobj && (gobj != null))
            {
                UnityEngine.Object.Destroy (gobj);
            }

            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Clean up complete.", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
        }

        private void RemoveChannelCallback ()
        {
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} RemoveChannelCallback from dictionary", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            channelCallbacks.Clear ();
        }

        private void RemoveUserState ()
        {
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} RemoveUserState from user state dictionary", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            channelLocalUserState.Clear ();
            channelUserState.Clear ();
        }

        public Guid GenerateGuid ()
        {
            return Guid.NewGuid ();
        }

        #endregion

        #region "Change UUID"

        public void ChangeUUID (string newUUID)
        {
            if (string.IsNullOrEmpty (newUUID) || sessionUUID == newUUID) {
                return;
            }

            uuidChanged = true;

            string oldUUID = sessionUUID;

            sessionUUID = newUUID;

            string[] channels = GetCurrentSubscriberChannels ();

            if (channels != null && channels.Length > 0) {
				Uri request = Helpers.BuildMultiChannelLeaveRequest (channels, oldUUID,
					this.ssl, this.Origin, authenticationKey, Version, this.subscribeKey);

                RequestState<string> requestState = Helpers.BuildRequestState<string> (channels, ResponseType.Leave, false, null, null, null, 0, false, 0, null);

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
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Stopping Heartbeat ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
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
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Stopping PresenceHeartbeat ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            keepPresenceHearbeatRunning = false;
            isPresenceHearbeatRunning = false;
            coroutine.PresenceHeartbeatCoroutineComplete -= CoroutineCompleteHandler<T>;
            coroutine.BounceRequest<T> (CurrentRequestType.PresenceHeartbeat, null, false);
        }

		void StartPresenceHeartbeat<T> (bool pause, int pauseTime, RequestState<T> pubnubRequestState)
		{
			try {
				string[] subscriberChannels = pubnubRequestState.Channels.Where (s => s.Contains ("-pnpres") == false).ToArray ();
				if (subscriberChannels != null && subscriberChannels.Length > 0) {
					isPresenceHearbeatRunning = true;
					string[] channels = multiChannelSubscribe.Keys.ToArray<string> ();
					string channelsJsonState = BuildJsonUserState (channels, false);

					Uri requestUrl = Helpers.BuildPresenceHeartbeatRequest (channels, channelsJsonState, this.SessionUUID,
						this.ssl, this.Origin, authenticationKey, Version, this.subscribeKey);

					coroutine.PresenceHeartbeatCoroutineComplete += CoroutineCompleteHandler<T>;

					//for heartbeat and presence heartbeat treat reconnect as pause
					RequestState<T> requestState = Helpers.BuildRequestState<T> (pubnubRequestState.Channels, ResponseType.PresenceHeartbeat, pause, null, null, pubnubRequestState.ErrorCallback, pubnubRequestState.ID, false, 0, null);
					StoredRequestState.Instance.SetRequestState (CurrentRequestType.PresenceHeartbeat, requestState);
					coroutine.Run<T> (requestUrl.OriginalString, requestState, HeartbeatTimeout, pauseTime);
					#if (WRITE_LOG)
					LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PresenceHeartbeat running for {1}", DateTime.Now.ToString (), pubnubRequestState.ID), LoggingMethod.LevelInfo);
					#endif
				}
			}
			catch (Exception ex) {
				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PresenceHeartbeat exception {1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
			}
		}

        void RunPresenceHeartbeat<T> (bool pause, int pauseTime, RequestState<T> pubnubRequestState)
        {
            keepPresenceHearbeatRunning = true;
            if (!isPresenceHearbeatRunning) {
				StartPresenceHeartbeat<T> (pause, pauseTime, pubnubRequestState);
            } else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PresenceHeartbeat Running ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            }
        }

		void StartHeartbeat<T> (bool pause, int pauseTime, RequestState<T> pubnubRequestState)
		{
			try {
				isHearbeatRunning = true;
				Uri requestUrl = Helpers.BuildTimeRequest (this.SessionUUID,
					this.ssl, this.Origin, Version);

				coroutine.HeartbeatCoroutineComplete += CoroutineCompleteHandler<T>;
				//for heartbeat and presence heartbeat treat reconnect as pause
				RequestState<T> requestState = Helpers.BuildRequestState<T> (pubnubRequestState.Channels, ResponseType.Heartbeat, pause, pubnubRequestState.UserCallback, pubnubRequestState.ConnectCallback, pubnubRequestState.ErrorCallback, pubnubRequestState.ID, false, 0, null);
				StoredRequestState.Instance.SetRequestState (CurrentRequestType.Heartbeat, requestState);
				coroutine.Run<T> (requestUrl.OriginalString, requestState, HeartbeatTimeout, pauseTime);
				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Heartbeat running for {1}", DateTime.Now.ToString (), pubnubRequestState.ID), LoggingMethod.LevelInfo);
			}
			catch (Exception ex) {
				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Heartbeat exception {1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
			}
		}

        void RunHeartbeat<T> (bool pause, int pauseTime, RequestState<T> pubnubRequestState)
        {
            keepHearbeatRunning = true;
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, In Runheartbeat {1}", DateTime.Now.ToString (), isHearbeatRunning.ToString ()), LoggingMethod.LevelInfo);
            if (!isHearbeatRunning) {
				StartHeartbeat<T> (pause, pauseTime, pubnubRequestState);
            } else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Heartbeat Running ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            }
        }

        #endregion

        #region "CoroutineHandlers"

		private void InternetConnectionAvailableHandler<T>(CustomEventArgs<T> cea){
			internetStatus = true;
			retriesExceeded = false;
			if (retryCount > 0) {
				string cbMessage = string.Format ("DateTime {0} Internet Connection Available.", DateTime.Now.ToString ());
				LoggingMethod.WriteToLog (cbMessage, LoggingMethod.LevelInfo);
				string multiChannel = string.Join (",", cea.PubnubRequestState.Channels);
				PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Info, PubnubMessageSource.Client, multiChannel, cea.PubnubRequestState.ErrorCallback, cbMessage, PubnubErrorCode.YesInternet, null, null);
				string[] activeChannels = multiChannelSubscribe.Keys.ToArray<string> ();
				MultiplexExceptionHandler<T> (ResponseType.Subscribe, activeChannels, cea.PubnubRequestState.UserCallback, cea.PubnubRequestState.ConnectCallback, cea.PubnubRequestState.ErrorCallback, false, true);
			}
			retryCount = 0;
		}

		private void HeartbeatHandler<T> (CustomEventArgs<T> cea){
			if (cea.IsTimeout || cea.IsError) {
				RetryLoop<T> (cea.PubnubRequestState);
				LoggingMethod.WriteToLog (string.Format ("DateTime {0} Heartbeat timeout={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
			} else {
				InternetConnectionAvailableHandler<T> (cea);
			}
			isHearbeatRunning = false;
			if (keepHearbeatRunning) {
				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Restarting Heartbeat {1}", DateTime.Now.ToString (), cea.PubnubRequestState.ID), LoggingMethod.LevelInfo);
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
			if (cea.IsTimeout || cea.IsError) {
				LoggingMethod.WriteToLog (string.Format ("DateTime {0} Presence Heartbeat timeout={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
			}
			else {
				LoggingMethod.WriteToLog (string.Format ("DateTime {0} Presence Heartbeat response: {1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelInfo);
			}
			if (keepPresenceHearbeatRunning) {
				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Restarting PresenceHeartbeat ID {1}", DateTime.Now.ToString (), cea.PubnubRequestState.ID), LoggingMethod.LevelInfo);
				RunPresenceHeartbeat<T> (true, PresenceHeartbeatInterval, cea.PubnubRequestState);
			}
		}

		private void SubscribePresenceHanlder<T> (CustomEventArgs<T> cea){
			if (cea.IsTimeout || CheckRequestTimeoutMessageInError (cea)) {
				LoggingMethod.WriteToLog (string.Format ("DateTime {0} Sub timeout={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
			} else if (cea.IsError) {
				LoggingMethod.WriteToLog (string.Format ("DateTime {0} Sub Error={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
			} else {
				LoggingMethod.WriteToLog (string.Format ("DateTime {0} Sub Message={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelInfo);
			}
			UrlProcessResponseCallbackNonAsync<T> (cea);
		}

		private void NonSubscribeHandler<T> (CustomEventArgs<T> cea){
			if (cea.IsTimeout || CheckRequestTimeoutMessageInError (cea)) {
				LoggingMethod.WriteToLog (string.Format ("DateTime {0} NonSub timeout={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
				UrlRequestCommonExceptionHandler<T> (cea.PubnubRequestState.Type, cea.PubnubRequestState.Channels, true, cea.PubnubRequestState.UserCallback, cea.PubnubRequestState.ConnectCallback, cea.PubnubRequestState.ErrorCallback, false);
			} else if (cea.IsError) {
				LoggingMethod.WriteToLog (string.Format ("DateTime {0} NonSub Error={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
				UrlRequestCommonExceptionHandler<T> (cea.PubnubRequestState.Type, cea.PubnubRequestState.Channels, false, cea.PubnubRequestState.UserCallback, cea.PubnubRequestState.ConnectCallback, cea.PubnubRequestState.ErrorCallback, false);
			} else {
				LoggingMethod.WriteToLog (string.Format ("DateTime {0} NonSub Message={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelInfo);
				var result = WrapResultBasedOnResponseType<T> (cea.PubnubRequestState.Type, cea.Message, cea.PubnubRequestState.Channels, cea.PubnubRequestState.Reconnect, cea.PubnubRequestState.Timetoken, cea.PubnubRequestState.ErrorCallback);
				ProcessResponseCallbacks<T> (result, cea.PubnubRequestState);
			}
		}

		private void ProcessCoroutineCompleteResponse<T> (CustomEventArgs<T> cea)
		{
			LoggingMethod.WriteToLog (string.Format ("DateTime {0}, In handler of event cea {1} RequestType CoroutineCompleteHandler {2}", DateTime.Now.ToString (), cea.PubnubRequestState.Type.ToString (), typeof(T)), LoggingMethod.LevelInfo);
			switch (cea.PubnubRequestState.Type) {
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
                    } else {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0} PubnubRequestState null", DateTime.Now.ToString ()), LoggingMethod.LevelError);
                    }
                } else {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0} cea null", DateTime.Now.ToString ()), LoggingMethod.LevelError);
                }
            } catch (Exception ex) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} Exception={1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                UrlRequestCommonExceptionHandler<T> (cea.PubnubRequestState.Type, cea.PubnubRequestState.Channels, false, cea.PubnubRequestState.UserCallback, cea.PubnubRequestState.ConnectCallback, cea.PubnubRequestState.ErrorCallback, false);
            } 
        }

        private bool CheckRequestTimeoutMessageInError<T>(CustomEventArgs<T> cea){
            if (cea.IsError && cea.Message.ToString().Contains ("The request timed out.")) {
                return true;
            } else {
                return false;
            }
        }

        private void ProcessResponseCallbackExceptionHandler<T> (Exception ex, RequestState<T> asynchRequestState)
        {
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Exception= {1} for URL: {2}", DateTime.Now.ToString (), ex.ToString (), asynchRequestState.Request.RequestUri.ToString ()), LoggingMethod.LevelInfo);
            UrlRequestCommonExceptionHandler<T> (asynchRequestState.Type, asynchRequestState.Channels, asynchRequestState.Timeout, asynchRequestState.UserCallback, asynchRequestState.ConnectCallback, asynchRequestState.ErrorCallback, false);
        }

        private void ProcessResponseCallbackWebExceptionHandler<T> (WebException webEx, RequestState<T> asynchRequestState, string channel)
        {
            if (webEx.ToString ().Contains ("Aborted")) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WebException: {1}", DateTime.Now.ToString (), webEx.ToString ()), LoggingMethod.LevelInfo);
            } else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WebException: {1}", DateTime.Now.ToString (), webEx.ToString ()), LoggingMethod.LevelError);
            }

            UrlRequestCommonExceptionHandler<T> (asynchRequestState.Type, asynchRequestState.Channels, asynchRequestState.Timeout,
                asynchRequestState.UserCallback, asynchRequestState.ConnectCallback, asynchRequestState.ErrorCallback, false);
        }

		void ResponseCallbackErrorOrTimeoutHandler<T> (CustomEventArgs<T> cea, RequestState<T> requestState, string channel){

			WebException webEx = new WebException (cea.Message);

			if ((cea.Message.Contains ("NameResolutionFailure")
				|| cea.Message.Contains ("ConnectFailure")
				|| cea.Message.Contains ("ServerProtocolViolation")
				|| cea.Message.Contains ("ProtocolError")
			)) {
				webEx = new WebException ("Network connnect error", WebExceptionStatus.ConnectFailure);

				PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Warn, PubnubMessageSource.Client, channel, requestState.ErrorCallback, cea.Message, PubnubErrorCode.NoInternetRetryConnect, null, null);
			} else if (cea.IsTimeout || CheckRequestTimeoutMessageInError(cea)) {
				//CallErrorCallback (PubnubErrorSeverity.Warn, PubnubMessageSource.Client, channel, requestState.ErrorCallback, cea.Message, PubnubErrorCode.NoInternetRetryConnect, null, null);
			} else if ((cea.Message.Contains ("403")) 
				|| (cea.Message.Contains ("java.io.FileNotFoundException")) 
				|| ((Version.Contains("UnityWeb")) && (cea.Message.Contains ("Failed downloading")))) {
				PubnubClientError error = new PubnubClientError (403, PubnubErrorSeverity.Critical, cea.Message, PubnubMessageSource.Server, requestState.Request, requestState.Response, cea.Message, channel);
				PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback);
			} else if (cea.Message.Contains ("500")) {
				PubnubClientError error = new PubnubClientError (500, PubnubErrorSeverity.Critical, cea.Message, PubnubMessageSource.Server, requestState.Request, requestState.Response, cea.Message, channel);
				PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback);
			} else if (cea.Message.Contains ("502")) {
				PubnubClientError error = new PubnubClientError (503, PubnubErrorSeverity.Critical, cea.Message, PubnubMessageSource.Server, requestState.Request, requestState.Response, cea.Message, channel);
				PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback);
			} else if (cea.Message.Contains ("503")) {
				PubnubClientError error = new PubnubClientError (503, PubnubErrorSeverity.Critical, cea.Message, PubnubMessageSource.Server, requestState.Request, requestState.Response, cea.Message, channel);
				PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback);
			} else if (cea.Message.Contains ("504")) {
				PubnubClientError error = new PubnubClientError (504, PubnubErrorSeverity.Critical, cea.Message, PubnubMessageSource.Server, requestState.Request, requestState.Response, cea.Message, channel);
				PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback);
			} else if (cea.Message.Contains ("414")) {
				PubnubClientError error = new PubnubClientError (414, PubnubErrorSeverity.Critical, cea.Message, PubnubMessageSource.Server, requestState.Request, requestState.Response, cea.Message, channel);
				PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback);
			} else {
				PubnubClientError error = new PubnubClientError (400, PubnubErrorSeverity.Critical, cea.Message, PubnubMessageSource.Server, requestState.Request, requestState.Response, cea.Message, channel);
				PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback);
			}
			ProcessResponseCallbackWebExceptionHandler<T> (webEx, requestState, channel);
		}

		void ResponseCallbackNonErrorHandler<T> (CustomEventArgs<T> cea, RequestState<T> requestState, List<object> result){
			string jsonString = cea.Message;
			if (overrideTcpKeepAlive) {
				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Aborting previous subscribe/presence requests having channel(s) UrlProcessResponseCallbackNonAsync", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
				coroutine.BounceRequest<T> (CurrentRequestType.Subscribe, requestState, false);
			}

			if (jsonString != "[]") {
				result = WrapResultBasedOnResponseType<T> (requestState.Type, jsonString, requestState.Channels, requestState.Reconnect, requestState.Timetoken, requestState.ErrorCallback);
			}
			ProcessResponseCallbacks<T> (result, requestState);
			if (requestState.Type == ResponseType.Subscribe || requestState.Type == ResponseType.Presence) {
				foreach (string currentChannel in requestState.Channels) {
					multiChannelSubscribe.AddOrUpdate (currentChannel, Convert.ToInt64 (result [1].ToString ()), (key, oldValue) => Convert.ToInt64 (result [1].ToString ()));
				}
			}
			switch (requestState.Type) {
			case ResponseType.Subscribe:
			case ResponseType.Presence:
				MultiplexInternalCallback<T> (requestState.Type, result, requestState.UserCallback, requestState.ConnectCallback, requestState.ErrorCallback, false);
				break;
			default:
				break;
			}
		}

		void ResponseCallbackWebExceptionHandler<T> (CustomEventArgs<T> cea){
		}

		void ResponseCallbackExceptionHandler<T> (CustomEventArgs<T> cea){
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
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Message: {1}", DateTime.Now.ToString (), cea.Message), LoggingMethod.LevelError);

					ResponseCallbackErrorOrTimeoutHandler<T> (cea, requestState, channel);

                    
                } else {
					ResponseCallbackNonErrorHandler<T> (cea, requestState, result);

                }
            } catch (WebException webEx) {
				ResponseCallbackWebExceptionHandler<T> (cea);
                //TODO:fetch response code
                PubnubErrorCode errorType = PubnubErrorCodeHelper.GetErrorType (webEx.Status, webEx.Message);
                int statusCode = (int)errorType;
                string errorDescription = PubnubErrorCodeDescription.GetStatusCodeDescription (errorType);
                if (requestState.Channels != null || requestState.Type == ResponseType.Time) {
                    if (requestState.Type == ResponseType.Subscribe
                        || requestState.Type == ResponseType.Presence) {
                        if (webEx.Message.IndexOf ("The request was aborted: The request was canceled") == -1
                            || webEx.Message.IndexOf ("Machine suspend mode enabled. No request will be processed.") == -1) {
                            for (int index = 0; index < requestState.Channels.Length; index++) {
								var callbackKey = Helpers.GetPubnubChannelCallbackKey (requestState.Channels [index].ToString (), requestState.Type);

                                if (channelCallbacks.Count > 0 && channelCallbacks.ContainsKey (callbackKey)) {
                                    object callbackObject;
                                    bool channelAvailable = channelCallbacks.TryGetValue (callbackKey, out callbackObject);
                                    PubnubChannelCallback<T> currentPubnubCallback = null;
                                    if (channelAvailable) {
                                        currentPubnubCallback = callbackObject as PubnubChannelCallback<T>;
                                    }
                                    if (currentPubnubCallback != null && currentPubnubCallback.ErrorCallback != null) {
                                        PubnubClientError error = new PubnubClientError (statusCode, PubnubErrorSeverity.Warn, true, webEx.Message, webEx, PubnubMessageSource.Client, requestState.Request, requestState.Response, errorDescription, activeChannel);
                                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PubnubClientError = {1}", DateTime.Now.ToString (), error.ToString ()), LoggingMethod.LevelInfo);
										PubnubCallbacks.GoToCallback (error, currentPubnubCallback.ErrorCallback);
                                    }
                                }

                            }
                        }
                    } else {
                        PubnubClientError error = new PubnubClientError (statusCode, PubnubErrorSeverity.Warn, true, webEx.Message, webEx, PubnubMessageSource.Client, requestState.Request, requestState.Response, errorDescription, channel);
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PubnubClientError = {1}", DateTime.Now.ToString (), error.ToString ()), LoggingMethod.LevelInfo);
						PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback);
                    }
                }
                ProcessResponseCallbackWebExceptionHandler<T> (webEx, requestState, channel);
            } catch (Exception ex) {
				ResponseCallbackExceptionHandler<T> (cea);
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Process Response Exception: = {1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                if (requestState.Channels != null) {
                    if (requestState.Type == ResponseType.Subscribe
                        || requestState.Type == ResponseType.Presence) {
                        for (int index = 0; index < requestState.Channels.Length; index++) {
                            //string activeChannel = requestState.Channels [index].ToString ();

							var callbackKey = Helpers.GetPubnubChannelCallbackKey (requestState.Channels [index].ToString (), requestState.Type);

                            if (channelCallbacks.Count > 0 && channelCallbacks.ContainsKey (callbackKey)) {
                                PubnubChannelCallback<T> currentPubnubCallback = channelCallbacks [callbackKey] as PubnubChannelCallback<T>;
                                PubnubErrorCode errorType = PubnubErrorCodeHelper.GetErrorType (ex);
                                int statusCode = (int)errorType;
                                string errorDescription = PubnubErrorCodeDescription.GetStatusCodeDescription (errorType);
                                PubnubClientError error = new PubnubClientError (statusCode, PubnubErrorSeverity.Critical, true, ex.Message, ex, PubnubMessageSource.Client, requestState.Request, requestState.Response, errorDescription, channel);

                                if (currentPubnubCallback != null && currentPubnubCallback.ErrorCallback != null) {
									PubnubCallbacks.GoToCallback (error, currentPubnubCallback.ErrorCallback);
                                }
                            }
                        }
                    } else {
                        PubnubErrorCode errorType = PubnubErrorCodeHelper.GetErrorType (ex);
                        int statusCode = (int)errorType;
                        string errorDescription = PubnubErrorCodeDescription.GetStatusCodeDescription (errorType);
                        PubnubClientError error = new PubnubClientError (statusCode, PubnubErrorSeverity.Critical, true, ex.Message, ex, PubnubMessageSource.Client, requestState.Request, requestState.Response, errorDescription, channel);

						PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback);
                    }
                }
                ProcessResponseCallbackExceptionHandler<T> (ex, requestState);
            }
        }

        #endregion

        

        #region "Exception Handlers"

        protected void UrlRequestCommonExceptionHandler<T> (ResponseType type, string[] channels, bool requestTimeout, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback, bool resumeOnReconnect)
        {
            if (type == ResponseType.Subscribe || type == ResponseType.Presence) {
                MultiplexExceptionHandler<T> (type, channels, userCallback, connectCallback, errorCallback, false, resumeOnReconnect);
            } else if (type == ResponseType.Publish) {
                PublishExceptionHandler<T> (channels [0], requestTimeout, errorCallback);
            } else if (type == ResponseType.HereNow) {
                HereNowExceptionHandler<T> (channels [0], requestTimeout, errorCallback);
            } else if (type == ResponseType.DetailedHistory) {
                DetailedHistoryExceptionHandler<T> (channels [0], requestTimeout, errorCallback);
            } else if (type == ResponseType.Time) {
                TimeExceptionHandler<T> (requestTimeout, errorCallback);
            } else if (type == ResponseType.Leave) {
                //no action at this time
            } else if (type == ResponseType.PresenceHeartbeat) {
                //no action at this time
            } else if (type == ResponseType.GrantAccess || type == ResponseType.AuditAccess || type == ResponseType.RevokeAccess) {
            } else if (type == ResponseType.GetUserState) {
                GetUserStateExceptionHandler<T> (channels [0], requestTimeout, errorCallback);
            } else if (type == ResponseType.SetUserState) {
                SetUserStateExceptionHandler<T> (channels [0], requestTimeout, errorCallback);
            } else if (type == ResponseType.GlobalHereNow) {
                GlobalHereNowExceptionHandler<T> (requestTimeout, errorCallback);
            } else if (type == ResponseType.WhereNow) {
                WhereNowExceptionHandler<T> (channels [0], requestTimeout, errorCallback);
            }
        }

        private void PublishExceptionHandler<T> (string channelName, bool requestTimeout, Action<PubnubClientError> errorCallback)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, JSON publish response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                    channelName, errorCallback, message,
                    PubnubErrorCode.PublishOperationTimeout, null, null);
            }
        }

        private void PAMAccessExceptionHandler<T> (string channelName, bool requestTimeout, Action<PubnubClientError> errorCallback)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PAMAccessExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                    channelName, errorCallback, message,
                    PubnubErrorCode.PAMAccessOperationTimeout, null, null);
            }
        }

        private void WhereNowExceptionHandler<T> (string uuid, bool requestTimeout, Action<PubnubClientError> errorCallback)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WhereNowExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                    uuid, errorCallback, message, PubnubErrorCode.WhereNowOperationTimeout, null, null);
            }
        }

        private void HereNowExceptionHandler<T> (string channelName, bool requestTimeout, Action<PubnubClientError> errorCallback)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, HereNowExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                    channelName, errorCallback, message,
                    PubnubErrorCode.HereNowOperationTimeout, null, null);
            }
        }

        private void GlobalHereNowExceptionHandler<T> (bool requestTimeout, Action<PubnubClientError> errorCallback)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, GlobalHereNowExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                    "", errorCallback, message, PubnubErrorCode.GlobalHereNowOperationTimeout, null, null);
            }
        }

        private void DetailedHistoryExceptionHandler<T> (string channelName, bool requestTimeout, Action<PubnubClientError> errorCallback)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, DetailedHistoryExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                    channelName, errorCallback, message, 
                    PubnubErrorCode.DetailedHistoryOperationTimeout, null, null);
            }
        }

        private void TimeExceptionHandler<T> (bool requestTimeout, Action<PubnubClientError> errorCallback)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, TimeExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                    "", errorCallback, message, PubnubErrorCode.TimeOperationTimeout, null, null);
            }
        }

        private void SetUserStateExceptionHandler<T> (string channelName, bool requestTimeout, Action<PubnubClientError> errorCallback)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, SetUserStateExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                    channelName, errorCallback, message,
                    PubnubErrorCode.SetUserStateTimeout, null, null);
            }
        }

        private void GetUserStateExceptionHandler<T> (string channelName, bool requestTimeout, Action<PubnubClientError> errorCallback)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, GetUserStateExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                    channelName, errorCallback, message,
                    PubnubErrorCode.GetUserStateTimeout, null, null);
            }
        }

        protected void MultiplexExceptionHandler<T> (ResponseType type, string[] channels, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback, bool reconnectMaxTried, bool reconnect)
        {
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, in MultiplexExceptionHandler responsetype={1}", DateTime.Now.ToString (), type.ToString ()), LoggingMethod.LevelInfo);
            string channel = "";
            if (channels != null) {
                channel = string.Join (",", channels);
            }

            if (reconnectMaxTried) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MAX retries reached. Exiting the subscribe for channel(s) = {1}", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);

                string[] activeChannels = multiChannelSubscribe.Keys.ToArray<string> ();
                MultiChannelUnSubscribeInit<T> (ResponseType.Unsubscribe, string.Join (",", activeChannels), null, null, null, null);

                string[] subscribeChannels = activeChannels.Where (filterChannel => !filterChannel.Contains ("-pnpres")).ToArray ();
                string[] presenceChannels = activeChannels.Where (filterChannel => filterChannel.Contains ("-pnpres")).ToArray ();

                if (subscribeChannels != null && subscribeChannels.Length > 0) {
                    for (int index = 0; index < subscribeChannels.Length; index++) {
                        string message = string.Format ("Unsubscribed after {0} failed retries", pubnubNetworkCheckRetries);
                        string activeChannel = subscribeChannels [index].ToString ();

                        PubnubChannelCallbackKey callbackKey = new PubnubChannelCallbackKey ();
                        callbackKey.Channel = activeChannel;
                        callbackKey.Type = type;

                        if (channelCallbacks.Count > 0 && channelCallbacks.ContainsKey (callbackKey)) {
                            PubnubChannelCallback<T> currentPubnubCallback = channelCallbacks [callbackKey] as PubnubChannelCallback<T>;
                            if (currentPubnubCallback != null && currentPubnubCallback.Callback != null) {
								PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                                    activeChannel, currentPubnubCallback.ErrorCallback, message, 
                                    PubnubErrorCode.UnsubscribedAfterMaxRetries, null, null);
                            }
                        }

                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Subscribe JSON network error response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);
                    }

                }
                if (presenceChannels != null && presenceChannels.Length > 0) {
                    for (int index = 0; index < presenceChannels.Length; index++) {
                        string message = string.Format ("Presence Unsubscribed after {0} failed retries", pubnubNetworkCheckRetries);
                        string activeChannel = presenceChannels [index].ToString ();

                        PubnubChannelCallbackKey callbackKey = new PubnubChannelCallbackKey ();
                        callbackKey.Channel = activeChannel;
                        callbackKey.Type = type;

                        if (channelCallbacks.Count > 0 && channelCallbacks.ContainsKey (callbackKey)) {
                            PubnubChannelCallback<T> currentPubnubCallback = channelCallbacks [callbackKey] as PubnubChannelCallback<T>;
                            if (currentPubnubCallback != null && currentPubnubCallback.Callback != null) {
								PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                                    activeChannel, currentPubnubCallback.ErrorCallback, message, 
                                    PubnubErrorCode.PresenceUnsubscribedAfterMaxRetries, null, null);
                            }
                        }

                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Presence-Subscribe JSON network error response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);
                    }
                }

            } else {
                if (!internetStatus) {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Subscribe channel={1} - No internet connection. ", DateTime.Now.ToString (), channel), LoggingMethod.LevelInfo);
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
                uuid = this.sessionUUID;
            }

			Dictionary<string, object> deserializeUserState = JsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonUserState);
            if (channelUserState != null) {
                channelUserState.AddOrUpdate (channel.Trim (), deserializeUserState, (oldState, newState) => deserializeUserState);
            }
            if (channelLocalUserState != null) {
                channelLocalUserState.AddOrUpdate (channel.Trim (), deserializeUserState, (oldState, newState) => deserializeUserState);
            }

			Uri request = Helpers.BuildSetUserStateRequest (channel, jsonUserState, this.SessionUUID,
				this.ssl, this.Origin, authenticationKey, Version, this.subscribeKey);

            RequestState<T> requestState = Helpers.BuildRequestState<T> (new string[] { channel }, ResponseType.SetUserState, false, userCallback, null, errorCallback, 0, false, 0, null);

            UrlProcessRequest<T> (request, requestState);

            //bounce the long-polling subscribe requests to update user state
            TerminateCurrentSubscriberRequest<T> ();
        }

        #endregion

        #region "Helpers"

        protected void OnPubnubWebRequestTimeout<T> (object state, bool timeout)
        {
            if (timeout && state != null) {
                RequestState<T> currentState = state as RequestState<T>;
                if (currentState != null) {
                    PubnubWebRequest request = currentState.Request;
                    if (request != null) {
                        string currentMultiChannel = (currentState.Channels == null) ? "" : string.Join (",", currentState.Channels);
                        LoggingMethod.WriteToLog (string.Format ("DateTime: {0}, OnPubnubWebRequestTimeout: client request timeout reached.Request abort for channel = {1}", DateTime.Now.ToString (), currentMultiChannel), LoggingMethod.LevelInfo);
                        currentState.Timeout = true;
                        if ((currentState.Type == ResponseType.Subscribe) || (currentState.Type == ResponseType.Presence)) {
                            coroutine.BounceRequest<T> (CurrentRequestType.Subscribe, currentState, false);
                        } else {
                            coroutine.BounceRequest<T> (CurrentRequestType.NonSubscribe, currentState, false);
                        }
                    }
                } else {
                    LoggingMethod.WriteToLog (string.Format ("DateTime: {0}, OnPubnubWebRequestTimeout: client request timeout reached. However state is unknown", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                }
            }
        }

        private bool UrlProcessRequest<T> (Uri requestUri, RequestState<T> pubnubRequestState)
        {
            string channel = "";
            if (pubnubRequestState != null && pubnubRequestState.Channels != null) {
                channel = string.Join (",", pubnubRequestState.Channels);
            }

            try {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, In Url process request", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                if (!channelRequest.ContainsKey (channel) && (pubnubRequestState.Type == ResponseType.Subscribe || pubnubRequestState.Type == ResponseType.Presence)) {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, In Url process request check", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                    return false;
                }
                if (pubnubRequestState.Type == ResponseType.Subscribe || pubnubRequestState.Type == ResponseType.Presence) {
                    channelRequest.AddOrUpdate (channel, pubnubRequestState.Request, (key, oldState) => pubnubRequestState.Request);
                    RequestState<T> pubnubRequestStateHB = pubnubRequestState;
                    pubnubRequestStateHB.ID = DateTime.Now.Ticks;
                    //currentHBID = pubnubRequestStateHB.ID;
                    RunHeartbeat<T> (false, LocalClientHeartbeatInterval, pubnubRequestStateHB);
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Heartbeat started", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);

                    if (pubnubRequestStateHB.Channels != null && pubnubRequestStateHB.Channels.Length > 0 && pubnubRequestStateHB.Channels.Where (s => s.Contains ("-pnpres") == false).ToArray ().Length > 0) {
                        if (PresenceHeartbeatInterval > 0) {
                            RunPresenceHeartbeat<T> (false, PresenceHeartbeatInterval, pubnubRequestStateHB);
                        }
                    }
                    StoredRequestState.Instance.SetRequestState (CurrentRequestType.Subscribe, pubnubRequestState);
                    coroutine.SubCoroutineComplete += CoroutineCompleteHandler<T>;
                    coroutine.Run<T> (requestUri.OriginalString, pubnubRequestState, SubscribeTimeout, 0);
                } else {
                    StoredRequestState.Instance.SetRequestState (CurrentRequestType.NonSubscribe, pubnubRequestState);
                    coroutine.NonSubCoroutineComplete += CoroutineCompleteHandler<T>;
                    coroutine.Run<T> (requestUri.OriginalString, pubnubRequestState, NonSubscribeTimeout, 0);
                }

                return true;
            } catch (UnityEngine.MissingReferenceException ex) {
                //check Exception=UnityEngine.MissingReferenceException: The object of type 'CoroutineClass' has been destroyed but you are still trying to access it.
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Exception={1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                return false;
            } catch (System.NullReferenceException ex) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Exception={1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                return false;
            } catch (System.Exception ex) {
                if (pubnubRequestState != null && pubnubRequestState.ErrorCallback != null) {
                    string multiChannel = (pubnubRequestState.Channels != null) ? string.Join (",", pubnubRequestState.Channels) : "";

					PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                        multiChannel, pubnubRequestState.ErrorCallback, ex, pubnubRequestState.Request, pubnubRequestState.Response);
                }
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} Exception={1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);

                UrlRequestCommonExceptionHandler<T> (pubnubRequestState.Type, pubnubRequestState.Channels, false, pubnubRequestState.UserCallback, pubnubRequestState.ConnectCallback, pubnubRequestState.ErrorCallback, false);
                return false;
            }
        }

        private void MultiChannelUnSubscribeInit<T> (ResponseType type, string channel, Action<T> userCallback, Action<T> connectCallback, Action<T> disconnectCallback, Action<PubnubClientError> errorCallback)
        {
            string[] rawChannels = channel.Split (',');
            List<string> validChannels = new List<string> ();

            if (rawChannels.Length > 0) {
                for (int index = 0; index < rawChannels.Length; index++) {
                    if (rawChannels [index].Trim ().Length > 0) {
                        string channelName = rawChannels [index].Trim ();
                        if (type == ResponseType.PresenceUnsubscribe) {
                            channelName = string.Format ("{0}-pnpres", channelName);
                        }
                        if (!multiChannelSubscribe.ContainsKey (channelName)) {
                            string message = string.Format ("{0}Channel Not Subscribed", (IsPresenceChannel (channelName)) ? "Presence " : "");

                            PubnubErrorCode errorType = (IsPresenceChannel (channelName)) ? PubnubErrorCode.NotPresenceSubscribed : PubnubErrorCode.NotSubscribed;

                            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, channel={1} unsubscribe response={2}", DateTime.Now.ToString (), channelName, message), LoggingMethod.LevelInfo);

							PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Info, PubnubMessageSource.Client,
                                channelName, errorCallback, message, errorType, null, null);
                        } else {
                            validChannels.Add (channelName);
                        }
                    } else {
                        string message = "Invalid Channel Name For Unsubscribe";

                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, channel={1} unsubscribe response={2}", DateTime.Now.ToString (), rawChannels [index], message), LoggingMethod.LevelInfo);

						PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Info, PubnubMessageSource.Client,
                            rawChannels [index], errorCallback, message, PubnubErrorCode.InvalidChannel,
                            null, null);
                    }
                }
            }

            if (validChannels.Count > 0) {
                //Retrieve the current channels already subscribed previously and terminate them
                string[] currentChannels = multiChannelSubscribe.Keys.ToArray<string> ();
                if (currentChannels != null && currentChannels.Length > 0) {
                    string multiChannelName = string.Join (",", currentChannels);
                    if (channelRequest.ContainsKey (multiChannelName)) {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Aborting previous subscribe/presence requests having channel(s)={1}", DateTime.Now.ToString (), multiChannelName), LoggingMethod.LevelInfo);
                        channelRequest [multiChannelName] = null;

                        PubnubWebRequest removedRequest;
                        bool removedChannel = channelRequest.TryRemove (multiChannelName, out removedRequest);
                        if (removedChannel) {
                            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Success to remove channel(s)={1} from _channelRequest (MultiChannelUnSubscribeInit).", DateTime.Now.ToString (), multiChannelName), LoggingMethod.LevelInfo);
                        } else {
                            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Unable to remove channel(s)={1} from _channelRequest (MultiChannelUnSubscribeInit).", DateTime.Now.ToString (), multiChannelName), LoggingMethod.LevelInfo);
                        }
                        RequestState<T> reqState = StoredRequestState.Instance.GetStoredRequestState (CurrentRequestType.Subscribe) as RequestState<T>;
                        coroutine.BounceRequest<T> (CurrentRequestType.Subscribe, reqState, false);
                        //StopHeartbeat<T> ();
                    } else {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Unable to capture channel(s)={1} from _channelRequest to abort request.", DateTime.Now.ToString (), multiChannelName), LoggingMethod.LevelInfo);
                    }

                    if (type == ResponseType.Unsubscribe) {
						Uri request = Helpers.BuildMultiChannelLeaveRequest (validChannels.ToArray (), "",
							this.ssl, this.Origin, authenticationKey, Version, this.subscribeKey);

                        RequestState<T> requestState = Helpers.BuildRequestState<T> (new string[] { channel }, ResponseType.Leave, false, null, null, null, 0, false, 0, null);

                        UrlProcessRequest<T> (request, requestState); 
                    }
                }
                    
                //Remove the valid channels from subscribe list for unsubscribe 
                for (int index = 0; index < validChannels.Count; index++) {
                    long timetokenValue;
                    string channelToBeRemoved = validChannels [index].ToString ();
                    bool unsubscribeStatus = multiChannelSubscribe.TryRemove (channelToBeRemoved, out timetokenValue);
                    if (unsubscribeStatus) {
                        List<object> result = new List<object> ();
                        string jsonString = string.Format ("[1, \"{0}Unsubscribed from {1}\"]", (IsPresenceChannel (channelToBeRemoved)) ? "Presence " : "", channelToBeRemoved.Replace ("-pnpres", ""));
						result = JsonPluggableLibrary.DeserializeToListOfObject (jsonString);
                        result.Add (channelToBeRemoved.Replace ("-pnpres", ""));
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, JSON response={1}", DateTime.Now.ToString (), jsonString), LoggingMethod.LevelInfo);
						PubnubCallbacks.GoToCallback<T> (result, disconnectCallback);

                        DeleteLocalUserState (channelToBeRemoved);
                    } else {
                        string message = "Unsubscribe Error. Please retry the unsubscribe operation.";

                        PubnubErrorCode errorType = (IsPresenceChannel (channelToBeRemoved)) ? PubnubErrorCode.PresenceUnsubscribeFailed : PubnubErrorCode.UnsubscribeFailed;

                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, channel={1} unsubscribe error", DateTime.Now.ToString (), channelToBeRemoved), LoggingMethod.LevelInfo);

						PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                            channelToBeRemoved, errorCallback, message, errorType, null, null);

                    }
                }

                //Get all the channels
                string[] channels = multiChannelSubscribe.Keys.ToArray<string> ();

                if (channels != null && channels.Length > 0) {
                    RequestState<T> state = new RequestState<T> ();
                    channelRequest.AddOrUpdate (string.Join (",", channels), state.Request, (key, oldValue) => state.Request);

                    ResetInternetCheckSettings (channels);

                    //Modify the value for type ResponseType. Presence or Subscrie is ok, but sending the close value would make sense
                    if (string.Join (",", channels).IndexOf ("-pnpres") > 0) {
                        type = ResponseType.Presence;
                    } else {
                        type = ResponseType.Subscribe;
                    }

                    //Continue with any remaining channels for subscribe/presence
                    MultiChannelSubscribeRequest<T> (type, channels, 0, userCallback, connectCallback, errorCallback, false);
                } else {
                    if (isHearbeatRunning) {
                        StopHeartbeat<T> ();
                    }
                    if (isPresenceHearbeatRunning) {
                        StopPresenceHeartbeat<T> ();
                    }
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, All channels are Unsubscribed. Further subscription was stopped", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                }
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

        private void MultiChannelSubscribeInit<T> (ResponseType type, string channel, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback)
        {
            string[] rawChannels = channel.Split (',');
            List<string> validChannels = new List<string> ();

            ResetInternetCheckSettings (rawChannels);
            bool networkConnection = internetStatus;

            if (rawChannels.Length > 0 && networkConnection) {
                if (rawChannels.Length != rawChannels.Distinct ().Count ()) {
                    rawChannels = rawChannels.Distinct ().ToArray ();
                    string message = "Detected and removed duplicate channels";

					PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Info, PubnubMessageSource.Client,
                        channel, errorCallback, message, PubnubErrorCode.DuplicateChannel, null, null);
                }

                for (int index = 0; index < rawChannels.Length; index++) {
                    if (rawChannels [index].Trim ().Length > 0) {
                        string channelName = rawChannels [index].Trim ();
                        if (type == ResponseType.Presence) {
                            channelName = string.Format ("{0}-pnpres", channelName);
                        }
                        if (multiChannelSubscribe.ContainsKey (channelName)) {
                            string message = string.Format ("{0}Already subscribed", (IsPresenceChannel (channelName)) ? "Presence " : "");

                            PubnubErrorCode errorType = (IsPresenceChannel (channelName)) ? PubnubErrorCode.AlreadyPresenceSubscribed : PubnubErrorCode.AlreadySubscribed;

							PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Info, PubnubMessageSource.Client,
                                channelName.Replace ("-pnpres", ""), errorCallback, message, errorType, null, null);
                        } else {
                            validChannels.Add (channelName);
                        }
                    }
                }
            }

            lastSubscribeTimetokenForNewMultiplex = 0;

            if (validChannels.Count > 0) {
                //Retrieve the current channels already subscribed previously and terminate them
                string[] currentChannels = multiChannelSubscribe.Keys.ToArray<string> ();
                if (currentChannels != null && currentChannels.Length > 0) {
                    string multiChannelName = string.Join (",", currentChannels);
                    if (channelRequest.ContainsKey (multiChannelName)) {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Aborting previous subscribe/presence requests having channel(s)={1}", DateTime.Now.ToString (), multiChannelName), LoggingMethod.LevelInfo);
                        channelRequest [multiChannelName] = null;

                        PubnubWebRequest removedRequest;
                        channelRequest.TryRemove (multiChannelName, out removedRequest);
                        bool removedChannel = channelRequest.TryRemove (multiChannelName, out removedRequest);
                        if (removedChannel) {
                            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Success to remove channel(s)={1} from _channelRequest (MultiChannelSubscribeInit).", DateTime.Now.ToString (), multiChannelName), LoggingMethod.LevelInfo);
                        } else {
                            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Unable to remove channel(s)={1} from _channelRequest (MultiChannelSubscribeInit).", DateTime.Now.ToString (), multiChannelName), LoggingMethod.LevelInfo);
                        }

                        //retrieve and store current timetoken here
                        lastSubscribeTimetokenForNewMultiplex = lastSubscribeTimetoken;
                        RequestState<T> reqState = StoredRequestState.Instance.GetStoredRequestState (CurrentRequestType.Subscribe) as RequestState<T>;
                        coroutine.BounceRequest<T> (CurrentRequestType.Subscribe, reqState, false);
                    } else {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Unable to capture channel(s)={1} from _channelRequest to abort request.", DateTime.Now.ToString (), multiChannelName), LoggingMethod.LevelInfo);
                    }
                }
                //Add the valid channels to the channels subscribe list for tracking
                for (int index = 0; index < validChannels.Count; index++) {
                    string currentLoopChannel = validChannels [index].ToString ();
                    multiChannelSubscribe.GetOrAdd (currentLoopChannel, 0);

                    PubnubChannelCallbackKey callbackKey = new PubnubChannelCallbackKey ();
                    callbackKey.Channel = currentLoopChannel;
                    callbackKey.Type = type;

                    PubnubChannelCallback<T> pubnubChannelCallbacks = new PubnubChannelCallback<T> ();
                    pubnubChannelCallbacks.Callback = userCallback;
                    pubnubChannelCallbacks.ConnectCallback = connectCallback;
                    pubnubChannelCallbacks.ErrorCallback = errorCallback;

                    channelCallbacks.AddOrUpdate (callbackKey, pubnubChannelCallbacks, (key, oldValue) => pubnubChannelCallbacks);
                }

                //Get all the channels
                string[] channels = multiChannelSubscribe.Keys.ToArray<string> ();

                RequestState<T> state = new RequestState<T> ();
                channelRequest.AddOrUpdate (string.Join (",", channels), state.Request, (key, oldValue) => state.Request);
                MultiChannelSubscribeRequest<T> (type, channels, lastSubscribeTimetokenForNewMultiplex, userCallback, connectCallback, errorCallback, false);
            }
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
        private void MultiChannelSubscribeRequest<T> (ResponseType type, string[] channels, object timetoken, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback, bool reconnect)
        {
            //Exit if the channel is unsubscribed
            if (multiChannelSubscribe != null && multiChannelSubscribe.Count <= 0) {
                StopHeartbeat<T> ();
                if (isPresenceHearbeatRunning) {
                    StopPresenceHeartbeat<T> ();
                }

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, All channels are Unsubscribed. Further subscription was stopped", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                return;
            }

            string multiChannel = string.Join (",", channels);
            if (!channelRequest.ContainsKey (multiChannel)) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MultiChannelSubscribeRequest _channelRequest doesnt contain {1}", DateTime.Now.ToString (), multiChannel), LoggingMethod.LevelInfo);    
                string[] currentChannels = multiChannelSubscribe.Keys.ToArray<string> ();
                if (currentChannels != null && currentChannels.Length > 0) {
                    string currentSubChannels = string.Join (",", currentChannels);
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, using existing channels: {1}", DateTime.Now.ToString (), currentSubChannels), LoggingMethod.LevelInfo);    
                } else {
                    return;    
                }
            }

            if (pubnetSystemActive && retriesExceeded) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Subscribe channel={1} - No internet connection. MAXed retries for internet ", DateTime.Now.ToString (), multiChannel), LoggingMethod.LevelInfo);
                MultiplexExceptionHandler<T> (type, channels, userCallback, connectCallback, errorCallback, true, false);
                retriesExceeded = false;
                return;
            }

            // Begin recursive subscribe
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MultiChannelSubscribeRequest ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);    
            try {
                long lastTimetoken = 0;
                long sentTimetoken = Convert.ToInt64 (timetoken.ToString ());
                long minimumTimetoken = multiChannelSubscribe.Min (token => token.Value);
                long maximumTimetoken = multiChannelSubscribe.Max (token => token.Value);
                //LoggingMethod.WriteToLog (string.Format ("DateTime {0}, maximumTimetoken={1}", DateTime.Now.ToString (), maximumTimetoken), LoggingMethod.LevelInfo);
                //LoggingMethod.WriteToLog (string.Format ("DateTime {0}, minimumTimetoken={1}", DateTime.Now.ToString (), minimumTimetoken), LoggingMethod.LevelInfo);
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, lastSubscribeTimetokenForNewMultiplex={1}", DateTime.Now.ToString (), lastSubscribeTimetokenForNewMultiplex), LoggingMethod.LevelInfo);
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, sentTimetoken={1}", DateTime.Now.ToString (), sentTimetoken), LoggingMethod.LevelInfo);
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, lastSubscribeTimetoken={1}", DateTime.Now.ToString (), lastSubscribeTimetoken), LoggingMethod.LevelInfo);

                if (minimumTimetoken == 0 || uuidChanged) {
                    lastTimetoken = 0;
                    uuidChanged = false;
                } else {
                    //override lastTimetoken when lastSubscribeTimetokenForNewMultiplex is set.
                    //this is done to use the timetoken prior to the latest response from the server 
                    //and is true in case new channels are added to the subscribe list.
                    if(!sentTimetoken.Equals(0) && !lastSubscribeTimetokenForNewMultiplex.Equals(0) && !lastSubscribeTimetoken.Equals(lastSubscribeTimetokenForNewMultiplex)){
                        lastTimetoken = lastSubscribeTimetokenForNewMultiplex;
                        lastSubscribeTimetokenForNewMultiplex = 0;
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Using lastSubscribeTimetokenForNewMultiplex={1}", DateTime.Now.ToString (), lastSubscribeTimetokenForNewMultiplex), LoggingMethod.LevelInfo);
                    } else if(sentTimetoken.Equals(0)){
                        lastTimetoken = sentTimetoken;
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Using sentTimetoken={1}", DateTime.Now.ToString (), sentTimetoken), LoggingMethod.LevelInfo);
                    } else {
                        lastTimetoken = sentTimetoken;
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Using sentTimetoken={1}", DateTime.Now.ToString (), sentTimetoken), LoggingMethod.LevelInfo);
                    }

                    if(lastSubscribeTimetoken.Equals(lastSubscribeTimetokenForNewMultiplex)){
                        lastSubscribeTimetokenForNewMultiplex = 0;
                    }
                }
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Building request for channel(s)={1} with timetoken={2}", DateTime.Now.ToString (), string.Join (",", channels), lastTimetoken), LoggingMethod.LevelInfo);
                // Build URL
				string channelsJsonState = BuildJsonUserState (channels, false);

				Uri requestUrl = Helpers.BuildMultiChannelSubscribeRequest (channels, lastTimetoken, channelsJsonState, this.SessionUUID,
					this.ssl, this.Origin, authenticationKey, Version, this.subscribeKey);

                RequestState<T> pubnubRequestState = Helpers.BuildRequestState<T> (channels, type, reconnect, userCallback as Action<T>, connectCallback as Action<T>, errorCallback, 0, false, Convert.ToInt64 (timetoken.ToString ()), typeof(T));
                // Wait for message
                UrlProcessRequest<T> (requestUrl, pubnubRequestState);
            } catch (Exception ex) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} method:_subscribe \n channel={1} \n timetoken={2} \n Exception Details={3}", DateTime.Now.ToString (), string.Join (",", channels), timetoken.ToString (), ex.ToString ()), LoggingMethod.LevelError);

				PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                    string.Join (",", channels), errorCallback, ex, null, null);

                this.MultiChannelSubscribeRequest<T> (type, channels, timetoken, userCallback, connectCallback, errorCallback, false);
            }
        }

        private void RetryLoop<T> (RequestState<T> pubnubRequestState)
        {
            internetStatus = false;
            retryCount++;
            if (retryCount <= NetworkCheckMaxRetries) {
                string cbMessage = string.Format ("DateTime {0} Internet Disconnected, retrying. Retry count {1} of {2}", DateTime.Now.ToString (), retryCount.ToString (), NetworkCheckMaxRetries);    
                LoggingMethod.WriteToLog (cbMessage, LoggingMethod.LevelError);
                string multiChannel = string.Join (",", pubnubRequestState.Channels);
				PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Warn, PubnubMessageSource.Client, multiChannel, pubnubRequestState.ErrorCallback, cbMessage, PubnubErrorCode.NoInternetRetryConnect, null, null);
            } else {
                retriesExceeded = true;
                string cbMessage = string.Format ("DateTime {0} Internet Disconnected. Retries exceeded {1}. Unsubscribing connected channels.", DateTime.Now.ToString (), NetworkCheckMaxRetries);
                LoggingMethod.WriteToLog (cbMessage, LoggingMethod.LevelError);

                //stop heartbeat.
                keepHearbeatRunning = false;
                coroutine.BounceRequest<T> (CurrentRequestType.Subscribe, pubnubRequestState, false);

                //TODO: Fire callbacks
                string multiChannel = string.Join (",", pubnubRequestState.Channels);
				PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Warn, PubnubMessageSource.Client, multiChannel, pubnubRequestState.ErrorCallback, cbMessage, PubnubErrorCode.NoInternetRetryConnect, null, null);
                string[] activeChannels = multiChannelSubscribe.Keys.ToArray<string> ();
                MultiplexExceptionHandler<T> (ResponseType.Subscribe, activeChannels, pubnubRequestState.UserCallback, pubnubRequestState.ConnectCallback, pubnubRequestState.ErrorCallback, true, false);
            }
        }

        private bool IsPresenceChannel (string channel)
        {
            if (channel.LastIndexOf ("-pnpres") > 0) {
                return true;
            } else {
                return false;
            }
        }

        private string[] GetCurrentSubscriberChannels ()
        {
            string[] channels = null;
            if (multiChannelSubscribe != null && multiChannelSubscribe.Keys.Count > 0) {
                channels = multiChannelSubscribe.Keys.ToArray<string> ();
            }

            return channels;
        }

        private void VerifyOrSetSessionUUID ()
        {
            if (string.IsNullOrEmpty (this.SessionUUID) || string.IsNullOrEmpty (this.SessionUUID.Trim ())) {
                this.SessionUUID = Guid.NewGuid ().ToString ();
            }
        }

        private void ProcessResponseCallbacks<T> (List<object> result, RequestState<T> asynchRequestState)
        {
            if (result != null && result.Count >= 1 && asynchRequestState.UserCallback != null) {
                ResponseToConnectCallback<T> (result, asynchRequestState.Type, asynchRequestState.Channels, asynchRequestState.ConnectCallback);
                ResponseToUserCallback<T> (result, asynchRequestState.Type, asynchRequestState.Channels, asynchRequestState.UserCallback);
            }
        }

        private void ResponseToUserCallback<T> (List<object> result, ResponseType type, string[] channels, Action<T> userCallback)
        {
            string[] messageChannels;
            switch (type) {
            case ResponseType.Subscribe:
            case ResponseType.Presence:
                //try{
                var messages = (from item in result
                                select item as object).ToArray ();
                //var messages = result.ToArray ();
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, result: {1}", DateTime.Now.ToString (), result.ToString ()), LoggingMethod.LevelInfo);
                if (messages != null && messages.Length > 0) {
                    //object[] messageList = messages [0] as object[];
                    /*object[] messageList  = ((IEnumerable)messages[0]).Cast<object>()
                        .Select(x => x)
                        .ToArray();*/
                    //object[] messageList = messages[0].Cast<object>().ToArray();

                    //object[] messageList = Array.ConvertAll<object, object>(messages[0] as object[], element => element);
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, messageList typeOF: {1}", DateTime.Now.ToString (), messages [0].GetType ().ToString ()), LoggingMethod.LevelInfo);
                    var messageList = messages [0] as object[];
                    int i = 0;
                    foreach (object o in result) {
                        if (i == 0) {
                            IList collection = (IList)o;
                            messageList = new object[collection.Count];
                            bool added = false;
                            int j = 0;
                            foreach (object c in collection) {
                                //LoggingMethod.WriteToLog (string.Format ("DateTime {0}, collection: {1} type: {2}", DateTime.Now.ToString (), c.ToString (), c.GetType ().ToString ()), LoggingMethod.LevelInfo);
                                if ((c.GetType () == typeof(System.Int32)) || (c.GetType () == typeof(System.Double)) || (c.GetType () == typeof(System.Int64)) || (c.GetType () == typeof(System.Boolean))) {
                                    added = true;
                                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, collection: {1} in type: {2}", DateTime.Now.ToString (), c.ToString (), c.GetType ().ToString ()), LoggingMethod.LevelInfo);
                                    messageList [j] = c;

                                } else if (c.GetType () == typeof(System.String)) {

                                    added = true;
                                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, collection: {1} in type: {2}", DateTime.Now.ToString (), c.ToString (), c.GetType ().ToString ()), LoggingMethod.LevelInfo);
                                    messageList [j] = c.ToString ();
                                } else {
                                    try{
                                        messageList [j] = c;
                                        added = true;
                                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, collection other types: {1} in type: {2}", DateTime.Now.ToString (), c.ToString (), c.GetType ().ToString ()), LoggingMethod.LevelInfo);
                                    }catch (Exception ex){
                                        added = false;
                                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, collection other types: {1} in type: {2}, exception {3} ", DateTime.Now.ToString (), c.ToString (), c.GetType ().ToString (), ex.ToString()), LoggingMethod.LevelInfo);
                                    }
                                }
                                j++;
                            }

                            if (!added) {
                                collection.CopyTo (messageList, 0);
                            }
                        }
                        i++;
                    }

                    messageChannels = messages [2].ToString ().Split (',');
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, (messageChannels: {1}", DateTime.Now.ToString (), messageChannels.ToString ()), LoggingMethod.LevelInfo);
                    if (messageList != null && messageList.Length > 0) {
                        for (int messageIndex = 0; messageIndex < messageList.Length; messageIndex++) {
                            string currentChannel = (messageChannels.Length == 1) ? (string)messageChannels [0] : (string)messageChannels [messageIndex];
                            List<object> itemMessage = new List<object> ();
                            if (currentChannel.Contains ("-pnpres")) {
                                itemMessage.Add (messageList [messageIndex]);
                            } else {
                                //decrypt the subscriber message if cipherkey is available
                                if (this.cipherKey.Length > 0) {
                                    object decodeMessage;
                                    try {
                                        PubnubCrypto aes = new PubnubCrypto (this.cipherKey);
                                        string decryptMessage = aes.Decrypt (messageList [messageIndex].ToString ());
										decodeMessage = (decryptMessage == "**DECRYPT ERROR**") ? decryptMessage : JsonPluggableLibrary.DeserializeToObject (decryptMessage);
                                    } catch (Exception decryptEx) {
                                        decodeMessage = messageList [messageIndex].ToString ();
                                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, decodeMessage Exception: {1}", DateTime.Now.ToString (), decryptEx.ToString ()), LoggingMethod.LevelError);
                                    }
                                    itemMessage.Add (decodeMessage);
                                } else {
                                    itemMessage.Add (messageList [messageIndex]);
                                }
                            }
                            itemMessage.Add (messages [1].ToString ());
                            itemMessage.Add (currentChannel.Replace ("-pnpres", ""));


                            PubnubChannelCallbackKey callbackKey = new PubnubChannelCallbackKey ();
                            callbackKey.Channel = currentChannel;
                            callbackKey.Type = (currentChannel.LastIndexOf ("-pnpres") == -1) ? ResponseType.Subscribe : ResponseType.Presence;
                            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, currentChannel: {1}", DateTime.Now.ToString (), currentChannel.ToString ()), LoggingMethod.LevelInfo);
                            if (channelCallbacks.Count > 0 && channelCallbacks.ContainsKey (callbackKey)) {
                                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, typeof(T): {1}", DateTime.Now.ToString (), typeof(T).ToString ()), LoggingMethod.LevelInfo);
                                if ((typeof(T) == typeof(string) && channelCallbacks [callbackKey].GetType ().Name.Contains ("[System.String]")) ||
                                    (typeof(T) == typeof(object) && channelCallbacks [callbackKey].GetType ().Name.Contains ("[System.Object]"))) {
                                    PubnubChannelCallback<T> currentPubnubCallback = channelCallbacks [callbackKey] as PubnubChannelCallback<T>;
                                    if (currentPubnubCallback != null && currentPubnubCallback.Callback != null) {
										PubnubCallbacks.GoToCallback<T> (itemMessage, currentPubnubCallback.Callback);
                                    }
                                } else if (channelCallbacks [callbackKey].GetType ().FullName.Contains ("[System.String")) {
                                    PubnubChannelCallback<string> retryPubnubCallback = channelCallbacks [callbackKey] as PubnubChannelCallback<string>;
                                    if (retryPubnubCallback != null && retryPubnubCallback.Callback != null) {
										PubnubCallbacks.GoToCallback (itemMessage, retryPubnubCallback.Callback);
                                    }
                                } else if (channelCallbacks [callbackKey].GetType ().FullName.Contains ("[System.Object")) {
                                    PubnubChannelCallback<object> retryPubnubCallback = channelCallbacks [callbackKey] as PubnubChannelCallback<object>;
                                    if (retryPubnubCallback != null && retryPubnubCallback.Callback != null) {
										PubnubCallbacks.GoToCallback (itemMessage, retryPubnubCallback.Callback);
                                    }
                                }

                            }
                        }
                    }
                }
                break;
            case ResponseType.Publish:
                if (result != null && result.Count > 0) {
					PubnubCallbacks.GoToCallback<T> (result, userCallback);
                }
                break;
            case ResponseType.DetailedHistory:
                if (result != null && result.Count > 0) {
					PubnubCallbacks.GoToCallback<T> (result, userCallback);
                }
                break;
            case ResponseType.HereNow:
                if (result != null && result.Count > 0) {
					PubnubCallbacks.GoToCallback<T> (result, userCallback);
                }
                break;
            case ResponseType.GlobalHereNow:
                if (result != null && result.Count > 0) {
					PubnubCallbacks.GoToCallback<T> (result, userCallback);
                }
                break;
            case ResponseType.WhereNow:
                if (result != null && result.Count > 0) {
					PubnubCallbacks.GoToCallback<T> (result, userCallback);
                }
                break;
            case ResponseType.Time:
                if (result != null && result.Count > 0) {
					PubnubCallbacks.GoToCallback<T> (result, userCallback);
                }
                break;
            case ResponseType.Leave:
                //No response to callback
                break;
            case ResponseType.GrantAccess:
            case ResponseType.AuditAccess:
            case ResponseType.RevokeAccess:
            case ResponseType.GetUserState:
            case ResponseType.SetUserState:
                if (result != null && result.Count > 0) {
					PubnubCallbacks.GoToCallback<T> (result, userCallback);
                }
                break;
            default:
                break;
            }
        }

        
        private void ResponseToConnectCallback<T> (List<object> result, ResponseType type, string[] channels, Action<T> connectCallback)
        {
            //Check callback exists and make sure previous timetoken = 0
            if (channels != null && connectCallback != null
                && channels.Length > 0) {
                IEnumerable<string> newChannels = from channel in multiChannelSubscribe
                                                  where channel.Value == 0
                                                  select channel.Key;
                foreach (string channel in newChannels) {
                    string jsonString = "";
                    List<object> connectResult = new List<object> ();
                    switch (type) {
                    case ResponseType.Subscribe:
                        jsonString = string.Format ("[1, \"Connected\"]");
						connectResult = JsonPluggableLibrary.DeserializeToListOfObject (jsonString);
                        connectResult.Add (channel);

                        PubnubChannelCallbackKey callbackKey = new PubnubChannelCallbackKey ();
                        callbackKey.Channel = channel;
                        callbackKey.Type = type;

                        if (channelCallbacks.Count > 0 && channelCallbacks.ContainsKey (callbackKey)) {
                            PubnubChannelCallback<T> currentPubnubCallback = channelCallbacks [callbackKey] as PubnubChannelCallback<T>;
                            if (currentPubnubCallback != null && currentPubnubCallback.ConnectCallback != null) {
								PubnubCallbacks.GoToCallback<T> (connectResult, currentPubnubCallback.ConnectCallback);
                            }
                        }
                        break;
                    case ResponseType.Presence:
                        jsonString = string.Format ("[1, \"Presence Connected\"]");
						connectResult = JsonPluggableLibrary.DeserializeToListOfObject (jsonString);
                        connectResult.Add (channel.Replace ("-pnpres", ""));

                        PubnubChannelCallbackKey pCallbackKey = new PubnubChannelCallbackKey ();
                        pCallbackKey.Channel = channel;
                        pCallbackKey.Type = type;

                        if (channelCallbacks.Count > 0 && channelCallbacks.ContainsKey (pCallbackKey)) {
                            PubnubChannelCallback<T> currentPubnubCallback = channelCallbacks [pCallbackKey] as PubnubChannelCallback<T>;
                            if (currentPubnubCallback != null && currentPubnubCallback.ConnectCallback != null) {
								PubnubCallbacks.GoToCallback<T> (connectResult, currentPubnubCallback.ConnectCallback);
                            }
                        }
                        break;
                    default:
                        break;
                    }
                }
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
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Lost Channel Name for resubscribe", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                return;
            }
            //
            if (message != null && message.Count >= 3) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MultiChannelSubscribeRequest {1}", DateTime.Now.ToString (), message [1].ToString()), LoggingMethod.LevelInfo);    
                MultiChannelSubscribeRequest<T> (type, channels, (object)message [1], userCallback, connectCallback, errorCallback, reconnect);
                //MultiChannelSubscribeRequest<T> (type, channels, (object)sentTimetoken, userCallback, connectCallback, errorCallback, false);
            } else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MultiplexInternalCallback message null or count < 3", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            }
        }

        /// <summary>
        /// Gets the result by wrapping the json response based on the request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="jsonString"></param>
        /// <param name="channels"></param>
        /// <param name="reconnect"></param>
        /// <param name="lastTimetoken"></param>
        /// <param name="errorCallback"></param>
        /// <returns></returns>
        private List<object> WrapResultBasedOnResponseType<T> (ResponseType type, string jsonString, string[] channels, bool reconnect, long lastTimetoken, Action<PubnubClientError> errorCallback)
        {
            List<object> result = new List<object> ();

            try {
                string multiChannel = (channels != null) ? string.Join (",", channels) : "";
                if (!string.IsNullOrEmpty (jsonString)) {
                    //if (!string.IsNullOrEmpty (jsonString)) {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, jsonString = {1}", DateTime.Now.ToString (), jsonString), LoggingMethod.LevelInfo);
					object deSerializedResult = JsonPluggableLibrary.DeserializeToObject (jsonString);
                        List<object> result1 = ((IEnumerable)deSerializedResult).Cast<object> ().ToList ();

                        if (result1 != null && result1.Count > 0) {
                            result = result1;
                        }

                        switch (type) {
                        case ResponseType.Publish:
                            result.Add (multiChannel);
                            break;
                        case ResponseType.History:
                            if (this.cipherKey.Length > 0) {
                                List<object> historyDecrypted = new List<object> ();
                                PubnubCrypto aes = new PubnubCrypto (this.cipherKey);
                                foreach (object message in result) {
                                    historyDecrypted.Add (aes.Decrypt (message.ToString ()));
                                }
                                History = historyDecrypted;
                            } else {
                                History = result;
                            }
                            break;
                        case ResponseType.DetailedHistory:
                            result = DecodeDecryptLoop (result, channels, errorCallback);
                            result.Add (multiChannel);
                            break;
                        case ResponseType.HereNow:
						Dictionary<string, object> dictionary = JsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonString);
                            result = new List<object> ();
                            result.Add (dictionary);
                            result.Add (multiChannel);
                            break;
                        case ResponseType.GlobalHereNow:
						Dictionary<string, object> globalHereNowDictionary = JsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonString);
                            result = new List<object> ();
                            result.Add (globalHereNowDictionary);
                            break;
                        case ResponseType.WhereNow:
						Dictionary<string, object> whereNowDictionary = JsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonString);
                            result = new List<object> ();
                            result.Add (whereNowDictionary);
                            result.Add (multiChannel);
                            break;
                        case ResponseType.Time:
                            Int64[] c = deSerializedResult as Int64[];
                            if ((c != null) && (c.Length > 0)) {
                                //string s = c [0].ToString ();

                                result = new List<object> ();
                                //result.Add (s);
                                result.Add (c [0]);
                                //LoggingMethod.WriteToLog (string.Format ("DateTime {0}, added to result ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                            }
                            /*var messages = (from item in deSerializedResult
                                select item as object).ToArray ();
                            if (messages != null && messages.Length > 0) {
                                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, add to result", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                                result = new List<object> ();
                                //double lTt = Convert.ToDouble (messages[0].ToString());
                                //result.Add(lTt);
                                string t = messages[0].ToString();
                                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, t= {1}", DateTime.Now.ToString (), t), LoggingMethod.LevelInfo);
                                result.Add(t);
                                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, added to result ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                            }  */
                                
                            /*long lTt;

                            /*Int64[] res = _jsonPluggableLibrary.DeserializeToObject (jsonString);
                            if(res.Count>0){
                                LoggingMethod.WriteToLog (string.Format ("DateTime {0},  try parse on {1}", DateTime.Now.ToString (), res[0].ToString ()), LoggingMethod.LevelInfo);
                                if (long.TryParse (res[0].ToString (), out lTt)) {
                                    result = new List<object> ();
                                    result.Add (lTt);
                                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, in try parse ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                                }
                            } else {
                                LoggingMethod.WriteToLog (string.Format ("DateTime {0},  default time response {1}", DateTime.Now.ToString (), res.ToString ()), LoggingMethod.LevelInfo);
                            }*/

                            break;
                        case ResponseType.Subscribe:
                        case ResponseType.Presence:
                            result.Add (multiChannel);
                            long receivedTimetoken = (result.Count > 1) ? Convert.ToInt64 (result [1].ToString ()) : 0;
                            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, receivedTimetoken = {1}", DateTime.Now.ToString (), receivedTimetoken), LoggingMethod.LevelInfo);

                            //if(lastSubscribeTimetoken == 0){
                            lastSubscribeTimetoken = receivedTimetoken;
                            //}
                            if (!enableResumeOnReconnect) {
                                lastSubscribeTimetoken = receivedTimetoken;
                            } else {
                                //do nothing. keep last subscribe token
                            }
                            if (reconnect) {
                                if (enableResumeOnReconnect) {
                                    //do nothing. keep last subscribe token
                                } else {
                                    lastSubscribeTimetoken = receivedTimetoken;
                                }
                            }
                            break;
                        case ResponseType.Leave:
                            result.Add (multiChannel);
                            break;
                        case ResponseType.GrantAccess:
                        case ResponseType.AuditAccess:
                        case ResponseType.RevokeAccess:
						Dictionary<string, object> grantDictionary = JsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonString);
                            result = new List<object> ();
                            result.Add (grantDictionary);
                            result.Add (multiChannel);
                            break;
                        case ResponseType.GetUserState:
                        case ResponseType.SetUserState:
						Dictionary<string, object> userStateDictionary = JsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonString);
                            result = new List<object> ();
                            result.Add (userStateDictionary);
                            result.Add (multiChannel);
                            break;
                        default:
                            break;
                        }
                        ;//switch stmt end
                    //}
                } else {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, json string null ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                }
            } catch (Exception ex) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WrapResultBasedOnResponseType exception: {1} ", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                if (channels != null) {
                    if (type == ResponseType.Subscribe
                        || type == ResponseType.Presence) {
                        for (int index = 0; index < channels.Length; index++) {
                            string activeChannel = channels [index].ToString ();
                            PubnubChannelCallbackKey callbackKey = new PubnubChannelCallbackKey ();
                            callbackKey.Channel = activeChannel;
                            callbackKey.Type = type;

                            if (channelCallbacks.Count > 0 && channelCallbacks.ContainsKey (callbackKey)) {
                                PubnubChannelCallback<T> currentPubnubCallback = channelCallbacks [callbackKey] as PubnubChannelCallback<T>;
                                if (currentPubnubCallback != null && currentPubnubCallback.ErrorCallback != null) {
									PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                                        activeChannel, currentPubnubCallback.ErrorCallback, ex, null, null);
                                }
                            }
                        }
                    } else {
                        if (errorCallback != null) {
							PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                                string.Join (",", channels), errorCallback, ex, null, null);
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        

        #region "Encoding and Crypto"

        private string JsonEncodePublishMsg (object originalMessage)
        {
			string message = JsonPluggableLibrary.SerializeToJsonString (originalMessage);


            if (this.cipherKey.Length > 0) {
                PubnubCrypto aes = new PubnubCrypto (this.cipherKey);
                string encryptMessage = aes.Encrypt (message);
				message = JsonPluggableLibrary.SerializeToJsonString (encryptMessage);
            }

            return message;
        }
        //TODO: Identify refactoring
        private List<object> DecodeDecryptLoop (List<object> message, string[] channels, Action<PubnubClientError> errorCallback)
        {
            List<object> returnMessage = new List<object> ();
            if (this.cipherKey.Length > 0) {
                PubnubCrypto aes = new PubnubCrypto (this.cipherKey);
                var myObjectArray = (from item in message
                                     select item as object).ToArray ();
                IEnumerable enumerable = myObjectArray [0] as IEnumerable;
                if (enumerable != null) {
                    List<object> receivedMsg = new List<object> ();
                    foreach (object element in enumerable) {
                        string decryptMessage = "";
                        try {
                            decryptMessage = aes.Decrypt (element.ToString ());
                        } catch (Exception ex) {
                            decryptMessage = "**DECRYPT ERROR**";

                            string multiChannel = string.Join (",", channels);

							PubnubCallbacks.CallErrorCallback (PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                                multiChannel, errorCallback, ex, null, null);
                        }
						object decodeMessage = (decryptMessage == "**DECRYPT ERROR**") ? decryptMessage : JsonPluggableLibrary.DeserializeToObject (decryptMessage);
                        receivedMsg.Add (decodeMessage);
                    }
                    returnMessage.Add (receivedMsg);
                }

                for (int index = 1; index < myObjectArray.Length; index++) {
                    returnMessage.Add (myObjectArray [index]);
                }
                return returnMessage;
            } else {
                var myObjectArray = (from item in message
                                     select item as object).ToArray ();
                IEnumerable enumerable = myObjectArray [0] as IEnumerable;
                if (enumerable != null) {
                    List<object> receivedMessage = new List<object> ();
                    foreach (object element in enumerable) {
                        receivedMessage.Add (element);
                    }
                    returnMessage.Add (receivedMessage);
                }
                for (int index = 1; index < myObjectArray.Length; index++) {
                    returnMessage.Add (myObjectArray [index]);
                }
                return returnMessage;
            }
        }    

        #endregion

    }

    #region "Json Pluggable Library"
    public interface IJsonPluggableLibrary
    {
        bool IsArrayCompatible (string jsonString);

        bool IsDictionaryCompatible (string jsonString);

        string SerializeToJsonString (object objectToSerialize);

        List<object> DeserializeToListOfObject (string jsonString);

        object DeserializeToObject (string jsonString);
        //T DeserializeToObject<T>(string jsonString);
        Dictionary<string, object> DeserializeToDictionaryOfObject (string jsonString);
    }

    #if (USE_JSONFX_UNITY_IOS)
    public class JsonFxUnitySerializer : IJsonPluggableLibrary
    {
        public bool IsArrayCompatible (string jsonString)
        {
            return false;
        }

        public bool IsDictionaryCompatible (string jsonString)
        {
            return true;
        }

        public string SerializeToJsonString (object objectToSerialize)
        {
            string json = JsonWriter.Serialize (objectToSerialize); 
            return PubnubCryptoBase.ConvertHexToUnicodeChars (json);
        }

        public List<object> DeserializeToListOfObject (string jsonString)
        {
            var output = JsonReader.Deserialize<object[]> (jsonString) as object[];
            List<object> messageList = output.Cast<object> ().ToList ();
            return messageList;
        }

        public object DeserializeToObject (string jsonString)
        {
            var output = JsonReader.Deserialize<object> (jsonString) as object;
            return output;
        }

        public Dictionary<string, object> DeserializeToDictionaryOfObject (string jsonString)
        {
            //LoggingMethod.WriteToLog ("jsonstring:" + jsonString, LoggingMethod.LevelInfo);                   
            object obj = DeserializeToObject (jsonString);
            Dictionary<string, object> stateDictionary = new Dictionary<string, object> ();
            Dictionary<string, object> message = (Dictionary<string, object>)obj;
            if (message != null) {
                foreach (KeyValuePair<String, object> kvp in message) {
                    stateDictionary.Add (kvp.Key, kvp.Value);
                }
            }
            return stateDictionary;
        }
    }
    #elif (USE_MiniJSON)
    public class MiniJSONObjectSerializer : IJsonPluggableLibrary
    {
        public bool IsArrayCompatible (string jsonString)
        {
            return jsonString.Trim().StartsWith("[");
        }

        public bool IsDictionaryCompatible (string jsonString)
        {
            return jsonString.Trim().StartsWith("{");
        }

        public string SerializeToJsonString (object objectToSerialize)
        {
            string json = Json.Serialize (objectToSerialize); 
            return PubnubCryptoBase.ConvertHexToUnicodeChars (json);
        }

        public List<object> DeserializeToListOfObject (string jsonString)
        {
            return Json.Deserialize (jsonString) as List<object>;
        }

        public object DeserializeToObject (string jsonString)
        {
            return Json.Deserialize (jsonString) as object;
        }

        public Dictionary<string, object> DeserializeToDictionaryOfObject (string jsonString)
        {
            return Json.Deserialize (jsonString) as Dictionary<string, object>;
        }
    }
    #endif
    #endregion
}