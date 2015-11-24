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
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Process Response Exception: = {1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
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
            LoggingMethod.WriteToLog (string.Format ("DateTime {0} Exception= {1} for URL: {2}", DateTime.Now.ToString (), ex.ToString (), asynchRequestState.Request.RequestUri.ToString ()), LoggingMethod.LevelInfo);
            UrlRequestCommonExceptionHandler<T> (asynchRequestState.Type, asynchRequestState.Channels, asynchRequestState.Timeout, 
                asynchRequestState.UserCallback, asynchRequestState.ConnectCallback, asynchRequestState.ErrorCallback, false, errorLevel);
        }

        internal static void ProcessResponseCallbackWebExceptionHandler<T> (WebException webEx, RequestState<T> asynchRequestState, 
            string channel, PubnubErrorFilter.Level errorLevel)
        {
            if (webEx.ToString ().Contains ("Aborted")) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WebException: {1}", DateTime.Now.ToString (), webEx.ToString ()), LoggingMethod.LevelInfo);
            } else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WebException: {1}", DateTime.Now.ToString (), webEx.ToString ()), LoggingMethod.LevelError);
            }

            UrlRequestCommonExceptionHandler<T> (asynchRequestState.Type, asynchRequestState.Channels, asynchRequestState.Timeout,
                asynchRequestState.UserCallback, asynchRequestState.ConnectCallback, asynchRequestState.ErrorCallback, false, errorLevel);
        }

        static void FireMultiplexException<T>(ResponseType type, string[] channels, Action<T> userCallback, 
            Action<T> connectCallback, Action<PubnubClientError> errorCallback, bool resumeOnReconnect)
        {
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, UrlRequestCommonExceptionHandler for Subscribe/Presence", DateTime.Now.ToString()), LoggingMethod.LevelInfo);
            MultiplexExceptionEventArgs<T> mea = new MultiplexExceptionEventArgs<T>();
            mea.channels = channels;
            mea.connectCallback = connectCallback;
            mea.errorCallback = errorCallback;
            mea.resumeOnReconnect = resumeOnReconnect;
            mea.reconnectMaxTried = false;
            mea.requestType = type;
            mea.userCallback = userCallback;
            multiplexException.Raise(typeof(ExceptionHandlers), mea);
            //MultiplexExceptionHandler<T> (type, channels, userCallback, connectCallback, errorCallback, false, resumeOnReconnect);
        }

        internal static void UrlRequestCommonExceptionHandler<T> (ResponseType type, string[] channels, 
            bool requestTimeout, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback, 
            bool resumeOnReconnect, PubnubErrorFilter.Level errorLevel)
        {
            if (type == ResponseType.Subscribe || type == ResponseType.Presence) {
                FireMultiplexException<T>(type, channels, userCallback, connectCallback, errorCallback, resumeOnReconnect);
            } else if (type == ResponseType.Publish) {
                PublishExceptionHandler<T> (channels [0], requestTimeout, errorCallback, errorLevel);
            } else if (type == ResponseType.HereNow) {
                HereNowExceptionHandler<T> (channels [0], requestTimeout, errorCallback, errorLevel);
            } else if (type == ResponseType.DetailedHistory) {
                DetailedHistoryExceptionHandler<T> (channels [0], requestTimeout, errorCallback, errorLevel);
            } else if (type == ResponseType.Time) {
                TimeExceptionHandler<T> (requestTimeout, errorCallback, errorLevel);
            } else if (type == ResponseType.Leave) {
                //no action at this time
            } else if (type == ResponseType.PresenceHeartbeat) {
                //no action at this time
            } else if (type == ResponseType.GrantAccess || type == ResponseType.AuditAccess || type == ResponseType.RevokeAccess) {
            } else if (type == ResponseType.GetUserState) {
                GetUserStateExceptionHandler<T> (channels [0], requestTimeout, errorCallback, errorLevel);
            } else if (type == ResponseType.SetUserState) {
                SetUserStateExceptionHandler<T> (channels [0], requestTimeout, errorCallback, errorLevel);
            } else if (type == ResponseType.GlobalHereNow) {
                GlobalHereNowExceptionHandler<T> (requestTimeout, errorCallback, errorLevel);
            } else if (type == ResponseType.WhereNow) {
                WhereNowExceptionHandler<T> (channels [0], requestTimeout, errorCallback, errorLevel);
            }
        }

        internal static void PublishExceptionHandler<T> (string channelName, bool requestTimeout, 
            Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, JSON publish response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

                PubnubCallbacks.CallErrorCallback<T> (message, null, channelName, 
                    PubnubErrorCode.PublishOperationTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
            }
        }

        internal static void PAMAccessExceptionHandler<T> (string channelName, bool requestTimeout, 
            Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PAMAccessExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

                PubnubCallbacks.CallErrorCallback<T> (message, null, channelName, 
                    PubnubErrorCode.PAMAccessOperationTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
            }
        }

        internal static void WhereNowExceptionHandler<T> (string uuid, bool requestTimeout, 
            Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WhereNowExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

                PubnubCallbacks.CallErrorCallback<T> (message, null, uuid, 
                    PubnubErrorCode.WhereNowOperationTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
            }
        }

        internal static void HereNowExceptionHandler<T> (string channelName, bool requestTimeout, 
            Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, HereNowExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

                PubnubCallbacks.CallErrorCallback<T> (message, null, channelName, 
                    PubnubErrorCode.HereNowOperationTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
            }
        }

        internal static void GlobalHereNowExceptionHandler<T> (bool requestTimeout, 
            Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, GlobalHereNowExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

                PubnubCallbacks.CallErrorCallback<T> (message, null, "", 
                    PubnubErrorCode.GlobalHereNowOperationTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
            }
        }

        internal static void DetailedHistoryExceptionHandler<T> (string channelName, bool requestTimeout, 
            Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, DetailedHistoryExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

                PubnubCallbacks.CallErrorCallback<T> (message, null, channelName, 
                    PubnubErrorCode.DetailedHistoryOperationTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
            }
        }

        internal static void TimeExceptionHandler<T> (bool requestTimeout, 
            Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, TimeExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

                PubnubCallbacks.CallErrorCallback<T> (message, null, "", 
                    PubnubErrorCode.TimeOperationTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
            }
        }

        internal static void SetUserStateExceptionHandler<T> (string channelName, bool requestTimeout, 
            Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, SetUserStateExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

                PubnubCallbacks.CallErrorCallback<T> (message, null, channelName, 
                    PubnubErrorCode.SetUserStateTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
            }
        }

        internal static void GetUserStateExceptionHandler<T> (string channelName, bool requestTimeout, 
            Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
        {
            if (requestTimeout) {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, GetUserStateExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

                PubnubCallbacks.CallErrorCallback<T> (message, null, channelName, 
                    PubnubErrorCode.GetUserStateTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
            }
        }
    }
}
