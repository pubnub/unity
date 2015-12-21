using System;

namespace PubNubMessaging.Core
{

    public class ReconnectState<T>
    {
        public string[] Channels;
        public ResponseType Type;
        public Action<T> Callback;
        public Action<PubnubClientError> ErrorCallback;
        public Action<T> ConnectCallback;
        public object Timetoken;
        public bool Reconnect;

        public ReconnectState ()
        {
            Channels = null;
            Callback = null;
            ConnectCallback = null;
            Timetoken = null;
            Reconnect = false;
        }
    }
    #region "States and ResposeTypes"
    public enum ResponseType
    {
        Publish,
        History,
        Time,
        Subscribe,
        Presence,
        HereNow,
        Heartbeat,
        DetailedHistory,
        Leave,
        Unsubscribe,
        PresenceUnsubscribe,
        GrantAccess,
        AuditAccess,
        RevokeAccess,
        PresenceHeartbeat,
        SetUserState,
        GetUserState,
        WhereNow,
        GlobalHereNow
    }

    internal class InternetState<T>
    {
        public Action<bool> Callback;
        public Action<PubnubClientError> ErrorCallback;
        public string[] Channels;

        public InternetState ()
        {
            Callback = null;
            ErrorCallback = null;
            Channels = null;
        }
    }

    public class StoredRequestState
    {

        private static volatile StoredRequestState instance;
        private static readonly object syncRoot = new Object ();

        private StoredRequestState ()
        {
        }

        public static StoredRequestState Instance {
            get {
                if (instance == null) {
                    lock (syncRoot) {
                        if (instance == null)
                            instance = new StoredRequestState ();
                    }
                }

                return instance;
            }
        }

        SafeDictionary<CurrentRequestType, object> requestStates = new SafeDictionary<CurrentRequestType, object> ();

        public void SetRequestState (CurrentRequestType key, object requestState)
        {
            object reqState = requestState as object;
            requestStates.AddOrUpdate (key, reqState, (oldData, newData) => reqState);
        }

        public object GetStoredRequestState (CurrentRequestType aKey)
        {
            if (requestStates.ContainsKey (aKey)) {
                if (requestStates.ContainsKey (aKey)) {
                    return requestStates [aKey];
                }
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, returning false", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                #endif
            }
            return null;
        }

    }

    public class RequestState<T>
    {
        public Action<T> UserCallback;
        public Action<PubnubClientError> ErrorCallback;
        public Action<T> ConnectCallback;
        public PubnubWebRequest Request;
        public PubnubWebResponse Response;
        public ResponseType respType;
        public string[] Channels;
        public bool Timeout;
        public bool Reconnect;
        public long Timetoken;
        public Type TypeParameterType;
        public long ID;

        public RequestState ()
        {
            UserCallback = null;
            ConnectCallback = null;
            Request = null;
            Response = null;
            Channels = null;
            ID = 0;
        }

        public RequestState (RequestState<T> requestState)
        {
            Channels = requestState.Channels;
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Channels {1}", DateTime.Now.ToString (), Channels.ToString ()), LoggingMethod.LevelInfo);
            #endif
            ConnectCallback = requestState.ConnectCallback as Action<T>;
            ErrorCallback = requestState.ErrorCallback;
            Reconnect = requestState.Reconnect;
            Request = requestState.Request;
            Response = requestState.Response;
            Timeout = requestState.Timeout;
            Timetoken = requestState.Timetoken;
            TypeParameterType = requestState.TypeParameterType;
            UserCallback = requestState.UserCallback as Action<T>;
            ID = requestState.ID;
			respType = requestState.respType;
        }

        public void SetRequestState<U> (
            string[] channels, 
            Action<T> connectCallback, 
            Action<PubnubClientError> errorCallback,
            bool reconnect,
            PubnubWebRequest request,
            PubnubWebResponse response,
            bool timeout,
            long timetoken,
            Type typeParameterType,
            Action<T> userCallback,
            long id
        )
        {
            Channels = channels;
            ConnectCallback = connectCallback as Action<T>;
            ErrorCallback = errorCallback;
            Reconnect = reconnect;
            Request = request;
            Response = response;
            Timeout = timeout;
            Timetoken = timetoken;
            TypeParameterType = typeParameterType;
            UserCallback = userCallback as Action<T>;
            ID = id;
        }
    }

    #endregion
}

