using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PubNubMessaging.Core
{
    internal class MultiplexExceptionEventArgs<T> : EventArgs
    {
        internal string[] channels;
        internal bool reconnectMaxTried;
        internal bool resumeOnReconnect;
        internal ResponseType requestType;
        internal Action<T> userCallback; 
        internal Action<T> connectCallback; 
        internal Action<PubnubClientError> errorCallback;
    }

    public class ExceptionHandlers
    {
        private static EventHandler<EventArgs> multiplexException;
        public static event EventHandler<EventArgs> MultiplexException {
            add {
                if (multiplexException == null || !multiplexException.GetInvocationList ().Contains (value)) {
                    multiplexException += value;
                }
            }
            remove {
                multiplexException -= value;
            }
        }

        internal static void ResponseCallbackErrorOrTimeoutHandler<T> (CustomEventArgs<T> cea, RequestState<T> requestState, string channel, 
            PubnubErrorFilter.Level errorLevel, IJsonPluggableLibrary jsonPluggableLibrary){

            WebException webEx = new WebException (cea.Message);

            if ((cea.Message.Contains ("NameResolutionFailure")
                || cea.Message.Contains ("ConnectFailure")
                || cea.Message.Contains ("ServerProtocolViolation")
                || cea.Message.Contains ("ProtocolError")
            )) {
                webEx = new WebException ("Network connnect error", WebExceptionStatus.ConnectFailure);

                PubnubCallbacks.CallErrorCallback<T> (cea.Message, null, channel, 
                    PubnubErrorCode.NoInternetRetryConnect, PubnubErrorSeverity.Warn, requestState.ErrorCallback, errorLevel);

            } else if (cea.IsTimeout || Utility.CheckRequestTimeoutMessageInError(cea)) {
            } else if ((cea.Message.Contains ("403")) 
                || (cea.Message.Contains ("java.io.FileNotFoundException")) 
                || ((PubnubUnity.Version.Contains("UnityWeb")) && (cea.Message.Contains ("Failed downloading")))) {
                PubnubClientError error = new PubnubClientError (403, PubnubErrorSeverity.Critical, cea.Message, PubnubMessageSource.Server, requestState.Request, requestState.Response, cea.Message, channel);
                PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback, jsonPluggableLibrary);
            } else if (cea.Message.Contains ("500")) {
                PubnubClientError error = new PubnubClientError (500, PubnubErrorSeverity.Critical, cea.Message, PubnubMessageSource.Server, requestState.Request, requestState.Response, cea.Message, channel);
                PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback, jsonPluggableLibrary);
            } else if (cea.Message.Contains ("502")) {
                PubnubClientError error = new PubnubClientError (503, PubnubErrorSeverity.Critical, cea.Message, PubnubMessageSource.Server, requestState.Request, requestState.Response, cea.Message, channel);
                PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback, jsonPluggableLibrary);
            } else if (cea.Message.Contains ("503")) {
                PubnubClientError error = new PubnubClientError (503, PubnubErrorSeverity.Critical, cea.Message, PubnubMessageSource.Server, requestState.Request, requestState.Response, cea.Message, channel);
                PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback, jsonPluggableLibrary);
            } else if (cea.Message.Contains ("504")) {
                PubnubClientError error = new PubnubClientError (504, PubnubErrorSeverity.Critical, cea.Message, PubnubMessageSource.Server, requestState.Request, requestState.Response, cea.Message, channel);
                PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback, jsonPluggableLibrary);
            } else if (cea.Message.Contains ("414")) {
                PubnubClientError error = new PubnubClientError (414, PubnubErrorSeverity.Critical, cea.Message, PubnubMessageSource.Server, requestState.Request, requestState.Response, cea.Message, channel);
                PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback, jsonPluggableLibrary);
            } else {
                PubnubClientError error = new PubnubClientError (400, PubnubErrorSeverity.Critical, cea.Message, PubnubMessageSource.Server, requestState.Request, requestState.Response, cea.Message, channel);
                PubnubCallbacks.GoToCallback (error, requestState.ErrorCallback, jsonPluggableLibrary);
            }
            ProcessResponseCallbackWebExceptionHandler<T> (webEx, requestState, channel, errorLevel);
        }

        internal static void ResponseCallbackWebExceptionHandler<T> (CustomEventArgs<T> cea, RequestState<T> requestState, 
            WebException webEx, string channel, SafeDictionary<PubnubChannelCallbackKey, object> channelCallbacks, 
            PubnubErrorFilter.Level errorLevel){
            if (requestState.Channels != null || requestState.Type == ResponseType.Time) {

                if (requestState.Type == ResponseType.Subscribe
                    || requestState.Type == ResponseType.Presence) {

                    if (webEx.Message.IndexOf ("The request was aborted: The request was canceled") == -1
                        || webEx.Message.IndexOf ("Machine suspend mode enabled. No request will be processed.") == -1) {

                        PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (webEx, requestState, 
                            PubnubErrorSeverity.Warn, channelCallbacks, 
                            true, errorLevel);
                    }
                } else {
                    PubnubCallbacks.CallErrorCallback<T> (webEx, requestState, channel, 
                        PubnubErrorSeverity.Warn, requestState.ErrorCallback, errorLevel);
                }
            }
            ProcessResponseCallbackWebExceptionHandler<T> (webEx, requestState, channel, errorLevel);
        }

        internal static void ResponseCallbackExceptionHandler<T> (CustomEventArgs<T> cea, RequestState<T> requestState, 
            Exception ex, string channel, SafeDictionary<PubnubChannelCallbackKey, object> channelCallbacks,
            PubnubErrorFilter.Level errorLevel){

            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Process Response Exception: = {1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
            #endif
            if (requestState.Channels != null) {

                if (requestState.Type == ResponseType.Subscribe
                    || requestState.Type == ResponseType.Presence) {

                    PubnubCallbacks.FireErrorCallbacksForAllChannels (ex, requestState, 
                        PubnubErrorSeverity.Warn, channelCallbacks, 
                        false, PubnubErrorCode.None, errorLevel);
                } else {

                    PubnubCallbacks.CallErrorCallback<T> (ex, requestState, channel, 
                        PubnubErrorCode.None, PubnubErrorSeverity.Critical, requestState.ErrorCallback, errorLevel);
                }
            }
            ProcessResponseCallbackExceptionHandler<T> (ex, requestState, errorLevel);
        }

        internal static void ProcessResponseCallbackExceptionHandler<T> (Exception ex, RequestState<T> asynchRequestState, 
            PubnubErrorFilter.Level errorLevel)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Exception= {1} for URL: {2}", DateTime.Now.ToString (), ex.ToString (), asynchRequestState.Request.RequestUri.ToString ()), LoggingMethod.LevelInfo);
            #endif
			UrlRequestCommonExceptionHandler<T> (ex.Message, asynchRequestState.Type, asynchRequestState.Channels, asynchRequestState.Timeout, 
                asynchRequestState.UserCallback, asynchRequestState.ConnectCallback, asynchRequestState.ErrorCallback, false, errorLevel);
        }

        internal static void ProcessResponseCallbackWebExceptionHandler<T> (WebException webEx, RequestState<T> asynchRequestState, 
            string channel, PubnubErrorFilter.Level errorLevel)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            if (webEx.ToString ().Contains ("Aborted")) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WebException: {1}", DateTime.Now.ToString (), webEx.ToString ()), LoggingMethod.LevelInfo);
            } else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WebException: {1}", DateTime.Now.ToString (), webEx.ToString ()), LoggingMethod.LevelError);
            }
            #endif

			UrlRequestCommonExceptionHandler<T> (webEx.Message, asynchRequestState.Type, asynchRequestState.Channels, asynchRequestState.Timeout,
                asynchRequestState.UserCallback, asynchRequestState.ConnectCallback, asynchRequestState.ErrorCallback, false, errorLevel);
        }

        static void FireMultiplexException<T>(ResponseType type, string[] channels, Action<T> userCallback, 
            Action<T> connectCallback, Action<PubnubClientError> errorCallback, bool resumeOnReconnect)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, UrlRequestCommonExceptionHandler for Subscribe/Presence", DateTime.Now.ToString()), LoggingMethod.LevelInfo);
            #endif
            MultiplexExceptionEventArgs<T> mea = new MultiplexExceptionEventArgs<T>();
            mea.channels = channels;
            mea.connectCallback = connectCallback;
            mea.errorCallback = errorCallback;
            mea.resumeOnReconnect = resumeOnReconnect;
            mea.reconnectMaxTried = false;
            mea.requestType = type;
            mea.userCallback = userCallback;

            multiplexException.Raise(typeof(ExceptionHandlers), mea);
        }

		internal static void UrlRequestCommonExceptionHandler<T> (string message, ResponseType type, string[] channels, 
            bool requestTimeout, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback, 
            bool resumeOnReconnect, PubnubErrorFilter.Level errorLevel)
        {
            switch (type)
            {
                case ResponseType.Presence:
                case ResponseType.Subscribe:
                    FireMultiplexException<T>(type, channels, userCallback, connectCallback, errorCallback, resumeOnReconnect);
                    break;
                case ResponseType.GlobalHereNow:
                case ResponseType.Time:
                    CommonExceptionHandler<T>(message, "", requestTimeout, errorCallback, errorLevel, type);
                    break;
                case ResponseType.Leave:
                case ResponseType.PresenceHeartbeat:
                    break;
                default:
                    CommonExceptionHandler<T> (message, channels [0], requestTimeout, errorCallback, errorLevel, type);
                    break;
                    
            }
        }

		internal static void CommonExceptionHandler<T> (string message, string channelName, bool requestTimeout, 
			Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel, ResponseType responseType)
        {
			if (requestTimeout) {
				message = "Operation Timeout";
				#if (ENABLE_PUBNUB_LOGGING)
				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, {1} response={2}", DateTime.Now.ToString (), responseType.ToString (), message), LoggingMethod.LevelInfo);
				#endif

				PubnubCallbacks.CallErrorCallback<T> (message, null, channelName, 
					Helpers.GetTimeOutErrorCode (responseType), PubnubErrorSeverity.Critical, errorCallback, errorLevel);
			} else {
				#if (ENABLE_PUBNUB_LOGGING)
				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, {1} response={2}", DateTime.Now.ToString (), responseType.ToString (), message), LoggingMethod.LevelInfo);
				#endif

				PubnubCallbacks.CallErrorCallback<T> (message, null, channelName, 
					PubnubErrorCode.None, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
			}
        }
    }
}
