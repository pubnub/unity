using System;
using System.Collections.Generic;
using System.Net;

namespace PubNubMessaging.Core
{
    #region "Channel callback"
    internal struct PubnubChannelCallbackKey
    {
        public string Channel;
        public ResponseType Type;
    }

    internal class PubnubChannelCallback<T>
    {
        public Action<T> Callback;
        public Action<PubnubClientError> ErrorCallback;
        public Action<T> ConnectCallback;
        public Action<T> DisconnectCallback;
        public PubnubChannelCallback ()
        {
            Callback = null;
            ConnectCallback = null;
            DisconnectCallback = null;
            ErrorCallback = null;
        }
    }

    public enum CallbackType
    {
        User,
        Connect,
        Error,
        Disconnect
    }
    #endregion

    internal static class PubnubCallbacks
    {
        internal static void CallCallback<T>(PubnubChannelCallbackKey callbackKey, SafeDictionary<PubnubChannelCallbackKey, object> channelCallbacks, 
            IJsonPluggableLibrary jsonPluggableLibrary, List<object> itemMessage)
        {
            PubnubChannelCallback<T> currentPubnubCallback = channelCallbacks[callbackKey] as PubnubChannelCallback<T>;
            if (currentPubnubCallback != null && currentPubnubCallback.Callback != null)
            {
                GoToCallback<T>(itemMessage, currentPubnubCallback.Callback, jsonPluggableLibrary);
            }
        }

        internal static void CallCallbackKnownType<T>(PubnubChannelCallbackKey callbackKey, SafeDictionary<PubnubChannelCallbackKey, object> channelCallbacks, 
            IJsonPluggableLibrary jsonPluggableLibrary, List<object> itemMessage)
        {
            PubnubChannelCallback<T> currentPubnubCallback = channelCallbacks[callbackKey] as PubnubChannelCallback<T>;
            if (currentPubnubCallback != null && currentPubnubCallback.Callback != null)
            {
                GoToCallback(itemMessage, currentPubnubCallback.Callback, jsonPluggableLibrary);
            }
        }


        internal static void SendCallbacksBasedOnType<T>(SafeDictionary<PubnubChannelCallbackKey, object> channelCallbacks, 
            IJsonPluggableLibrary jsonPluggableLibrary, string currentChannel, List<object> itemMessage)
        {
            var callbackKey = PubnubCallbacks.GetPubnubChannelCallbackKey(currentChannel, 
                (Utility.IsPresenceChannel(currentChannel)) ? ResponseType.Presence : ResponseType.Subscribe);
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, currentChannel: {1}", DateTime.Now.ToString(), 
                currentChannel.ToString()), LoggingMethod.LevelInfo);
            #endif
            if (channelCallbacks.Count > 0 && channelCallbacks.ContainsKey(callbackKey))
            {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, typeof(T): {1}", DateTime.Now.ToString(), typeof(T).ToString()), LoggingMethod.LevelInfo);
                #endif
                if ((typeof(T) == typeof(string) && channelCallbacks[callbackKey].GetType().Name.Contains("[System.String]")) 
                    || (typeof(T) == typeof(object) && channelCallbacks[callbackKey].GetType().Name.Contains("[System.Object]")))
                {
                    CallCallback<T>(callbackKey, channelCallbacks, jsonPluggableLibrary, itemMessage);
                }
                else if (channelCallbacks[callbackKey].GetType().FullName.Contains("[System.String"))
                {
                    CallCallbackKnownType<string>(callbackKey, channelCallbacks, jsonPluggableLibrary, itemMessage);
                }
                else if (channelCallbacks[callbackKey].GetType().FullName.Contains("[System.Object"))
                {
                    CallCallbackKnownType<object>(callbackKey, channelCallbacks, jsonPluggableLibrary, itemMessage);
                }
            }
        }

        internal static void SendConnectCallback<T> (IJsonPluggableLibrary jsonPluggableLibrary, 
            List<object> connectResult, string channel, ResponseType type, SafeDictionary<PubnubChannelCallbackKey, object> channelCallbacks){

            var callbackKey = GetPubnubChannelCallbackKey (channel, type);

            if (channelCallbacks.Count > 0 && channelCallbacks.ContainsKey (callbackKey)) {
                PubnubChannelCallback<T> currentPubnubCallback = channelCallbacks [callbackKey] as PubnubChannelCallback<T>;
                if (currentPubnubCallback != null && currentPubnubCallback.ConnectCallback != null) {
                    PubnubCallbacks.GoToCallback<T> (connectResult, currentPubnubCallback.ConnectCallback, jsonPluggableLibrary);
                }
            }
        }

        internal static void FireErrorCallbacksForAllChannels<T> (WebException webEx, RequestState<T> requestState, 
            PubnubErrorSeverity severity, SafeDictionary<PubnubChannelCallbackKey, 
            object> channelCallbacks, bool callbackObjectType,  PubnubErrorFilter.Level errorLevel){

            for (int index = 0; index < requestState.Channels.Length; index++) {
                string activeChannel = requestState.Channels [index].ToString ();
                PubnubClientError error = Helpers.CreatePubnubClientError<T> (webEx, requestState, activeChannel, 
                    severity);

                FireErrorCallback<T> (activeChannel, channelCallbacks, 
                    callbackObjectType, requestState.respType, errorLevel, error);
            }
        }

        internal static void FireErrorCallbacksForAllChannels<T> (Exception ex, RequestState<T> requestState, 
            PubnubErrorSeverity severity, SafeDictionary<PubnubChannelCallbackKey, 
            object> channelCallbacks, bool callbackObjectType, PubnubErrorCode errorType, 
            PubnubErrorFilter.Level errorLevel){

            for (int index = 0; index < requestState.Channels.Length; index++) {
                string activeChannel = requestState.Channels [index].ToString ();
                PubnubClientError error = Helpers.CreatePubnubClientError<T> (ex, requestState, 
                    activeChannel, errorType, severity);

                PubnubCallbacks.FireErrorCallback<T> (requestState.Channels [index].ToString (), channelCallbacks, 
                    callbackObjectType, requestState.respType, errorLevel, error);
            }
        }

        internal static void FireErrorCallbacksForAllChannels<T> (Exception ex, string[] channels,
            PubnubErrorSeverity severity, SafeDictionary<PubnubChannelCallbackKey, 
            object> channelCallbacks, bool callbackObjectType, PubnubErrorCode errorType, 
            ResponseType responseType, PubnubErrorFilter.Level errorLevel){

            for (int index = 0; index < channels.Length; index++) {
                string activeChannel = channels [index].ToString ();
                PubnubClientError error = Helpers.CreatePubnubClientError<T> (ex, null, 
                    activeChannel, errorType, severity);

                PubnubCallbacks.FireErrorCallback<T> (channels [index].ToString (), channelCallbacks, 
                    callbackObjectType, responseType, errorLevel, error);
            }
        }

        internal static void FireErrorCallbacksForAllChannels<T> (string message, RequestState<T> requestState, string[] channels,
            PubnubErrorSeverity severity, SafeDictionary<PubnubChannelCallbackKey, 
            object> channelCallbacks, bool callbackObjectType, PubnubErrorCode errorType, 
            ResponseType responseType, PubnubErrorFilter.Level errorLevel){

            for (int index = 0; index < channels.Length; index++) {
                string activeChannel = channels [index].ToString ();
                PubnubClientError error = Helpers.CreatePubnubClientError<T> (message, requestState, 
                    activeChannel, errorType, severity);

                PubnubCallbacks.FireErrorCallback<T> (activeChannel, channelCallbacks, 
                    callbackObjectType, responseType, errorLevel, error);
            }
        }

        internal static void FireErrorCallback<T> (string activeChannel, SafeDictionary<PubnubChannelCallbackKey, 
            object> channelCallbacks, bool callbackObjectType, ResponseType responseType, PubnubErrorFilter.Level errorLevel, 
            PubnubClientError error){

            var callbackKey = GetPubnubChannelCallbackKey (activeChannel, responseType);
            PubnubChannelCallback<T> currentPubnubCallback = null;

            if (channelCallbacks.Count > 0 && channelCallbacks.ContainsKey (callbackKey)) {
                if(callbackObjectType){
                    object callbackObject;
                    bool channelAvailable = channelCallbacks.TryGetValue (callbackKey, out callbackObject);

                    if (channelAvailable) {
                        currentPubnubCallback = callbackObject as PubnubChannelCallback<T>;
                    }
                } else {
                    currentPubnubCallback = channelCallbacks [callbackKey] as PubnubChannelCallback<T>;
                }
                if (currentPubnubCallback != null && currentPubnubCallback.ErrorCallback != null) {
                    GoToCallback (error, currentPubnubCallback.ErrorCallback, errorLevel);
                }

            }
        }

        internal static PubnubChannelCallbackKey GetPubnubChannelCallbackKey(string activeChannel, ResponseType responseType){
            PubnubChannelCallbackKey callbackKey = new PubnubChannelCallbackKey ();
            callbackKey.Channel = activeChannel;
            callbackKey.Type = responseType;
            return callbackKey;
        }

        #region "Error Callbacks"

        internal static void CallErrorCallback<T>(WebException webEx, 
            RequestState<T> requestState, string channel, PubnubErrorSeverity severity,
            Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel){

            PubnubClientError clientError = Helpers.CreatePubnubClientError (webEx, requestState, channel, severity);

            GoToCallback (clientError, errorCallback, errorLevel);
        }

        internal static void CallErrorCallback<T>(Exception ex, 
            RequestState<T> requestState, string channel, PubnubErrorCode errorCode, PubnubErrorSeverity severity,
            Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel){

            PubnubClientError clientError = Helpers.CreatePubnubClientError (ex, requestState, channel, errorCode, severity);

            GoToCallback (clientError, errorCallback, errorLevel);
        }

        internal static void CallErrorCallback<T>(string message, 
            RequestState<T> requestState, string channel, PubnubErrorCode errorCode, PubnubErrorSeverity severity,
            Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel){

            PubnubClientError clientError = Helpers.CreatePubnubClientError<T> (message, requestState, channel, 
                errorCode, severity);

            GoToCallback (clientError, errorCallback, errorLevel);
        }

        private static void JsonResponseToCallback<T> (List<object> result, Action<T> callback, IJsonPluggableLibrary jsonPluggableLibrary)
        {
            string callbackJson = "";

            if (typeof(T) == typeof(string)) {
                callbackJson = jsonPluggableLibrary.SerializeToJsonString (result);

                Action<string> castCallback = callback as Action<string>;
                castCallback (callbackJson);
            }
        }

        private static void JsonResponseToCallback<T> (object result, Action<T> callback, IJsonPluggableLibrary jsonPluggableLibrary)
        {
            string callbackJson = "";

            if (typeof(T) == typeof(string)) {
                try {
                    callbackJson = jsonPluggableLibrary.SerializeToJsonString (result);
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, after _jsonPluggableLibrary.SerializeToJsonString {1}", DateTime.Now.ToString (), callbackJson), LoggingMethod.LevelInfo);
                    #endif
                    Action<string> castCallback = callback as Action<string>;
                    castCallback (callbackJson);
                } catch (Exception ex) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, JsonResponseToCallback = {1} ", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);        
                    #endif
                }
            }
        }

        internal static void GoToCallback<T> (object result, Action<T> Callback, IJsonPluggableLibrary jsonPluggableLibrary)
        {
            if (Callback != null) {
                if (typeof(T) == typeof(string)) {
                    JsonResponseToCallback (result, Callback, jsonPluggableLibrary);
                } else {
                    Callback ((T)(object)result);
                }
            }
        }

        internal static void GoToCallback (object result, Action<string> Callback, IJsonPluggableLibrary jsonPluggableLibrary)
        {
            if (Callback != null) {
                JsonResponseToCallback (result, Callback, jsonPluggableLibrary);
            }
        }

        internal static void GoToCallback (object result, Action<object> Callback)
        {
            if (Callback != null) {
                Callback (result);
            }
        }

        internal static void GoToCallback (PubnubClientError error, Action<PubnubClientError> Callback, PubnubErrorFilter.Level errorLevel)
        {
            if (Callback != null && error != null) {
                if ((int)error.Severity <= (int)errorLevel) { //Checks whether the error serverity falls in the range of error filter level
                    //Do not send 107 = PubnubObjectDisposedException
                    //Do not send 105 = WebRequestCancelled
                    //Do not send 130 = PubnubClientMachineSleep
                    if (error.StatusCode != 107
                        && error.StatusCode != 105
                        && error.StatusCode != 130
                        && error.StatusCode != 4040) { //Error Code that should not go out
                        Callback (error);
                    }
                }
            }
        }


        #endregion
    }
}

