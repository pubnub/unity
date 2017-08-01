using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Networking;

namespace PubNubAPI
{
    internal class PNUnityWebRequest: MonoBehaviour
    {
        #region "IL2CPP workarounds"

        //Got an exception when using JSON serialisation for [],
        //IL2CPP needs to know about the array type at compile time.
        //So please define private static filed like this:
        private static System.String[][] _unused;
        private static System.Int32[][] _unused2;
        private static System.Int64[][] _unused3;
        private static System.Int16[][] _unused4;
        private static System.UInt16[][] _unused5;
        private static System.UInt64[][] _unused6;
        private static System.UInt32[][] _unused7;
        private static System.Decimal[][] _unused8;
        private static System.Double[][] _unused9;
        private static System.Boolean[][] _unused91;
        private static System.Object[][] _unused92;

        private static long[][] _unused10;
        private static int[][] _unused11;
        private static float[][] _unused12;
        private static decimal[][] _unused13;
        private static uint[][] _unused14;
        private static ulong[][] _unused15;

        #endregion

        public float subscribeTimer = 310; 
        public float heartbeatTimer = 10;
        public float presenceHeartbeatTimer = 10;
        public float nonSubscribeTimer = 15;
        public float heartbeatPauseTimer = 10;
        public float presenceHeartbeatPauseTimer = 10;
        public float subscribePauseTimer = 10;

        internal bool isHearbeatComplete = false;
        internal bool isPresenceHeartbeatComplete = false;
        internal bool isSubscribeComplete = false;
        internal bool isNonSubscribeComplete = false;

        /*private IEnumerator SubWebRequest;
        private IEnumerator SubTimeoutWebRequest;
        private IEnumerator NonSubWebRequest;
        private IEnumerator NonSubTimeoutWebRequest;
        private IEnumerator PresenceHeartbeatWebRequest;
        private IEnumerator PresenceHeartbeatTimeoutWebRequest;
        private IEnumerator HeartbeatWebRequest;
        private IEnumerator HeartbeatTimeoutWebRequest;
        private IEnumerator DelayRequestWebRequestHB;
        private IEnumerator DelayRequestWebRequestPHB;
        private IEnumerator DelayRequestWebRequestSub;*/

        internal UnityWebRequest subscribeWww;
        internal UnityWebRequest heartbeatWww;
        internal UnityWebRequest presenceHeartbeatWww;
        internal UnityWebRequest nonSubscribeWww;

        internal bool runSubscribeTimer = false;
        internal bool runNonSubscribeTimer = false;
        internal bool runHeartbeatTimer = false;
        internal bool runPresenceHeartbeatTimer = false;
        internal bool runHeartbeatPauseTimer = false;
        internal bool runPresenceHeartbeatPauseTimer = false;
        internal bool runSubscribePauseTimer = false;

        public event EventHandler<EventArgs> heartbeatResumeEvent;

        public event EventHandler<EventArgs> HeartbeatResumeEvent {
            add {
                if (heartbeatResumeEvent == null || !heartbeatResumeEvent.GetInvocationList ().Contains (value)) {
                    heartbeatResumeEvent += value;
                }
            }
            remove {
                heartbeatResumeEvent -= value;
            }
        }

        public event EventHandler<EventArgs> presenceHeartbeatResumeEvent;

        public event EventHandler<EventArgs> PresenceHeartbeatResumeEvent {
            add {
                if (presenceHeartbeatResumeEvent == null || !presenceHeartbeatResumeEvent.GetInvocationList ().Contains (value)) {
                    presenceHeartbeatResumeEvent += value;
                }
            }
            remove {
                presenceHeartbeatResumeEvent -= value;
            }
        }

        public event EventHandler<EventArgs> subscribeResumeEvent;

        public event EventHandler<EventArgs> SubscribeResumeEvent {
            add {
                if (subscribeResumeEvent == null || !subscribeResumeEvent.GetInvocationList ().Contains (value)) {
                    subscribeResumeEvent += value;
                }
            }
            remove {
                subscribeResumeEvent -= value;
            }
        }

        public event EventHandler<EventArgs> subCompleteOrTimeoutEvent;

        public event EventHandler<EventArgs> SubCompleteOrTimeoutEvent {
            add {
                if (subCompleteOrTimeoutEvent == null || !subCompleteOrTimeoutEvent.GetInvocationList ().Contains (value)) {
                    subCompleteOrTimeoutEvent += value;
                }
            }
            remove {
                subCompleteOrTimeoutEvent -= value;
            }
        }

        public event EventHandler<EventArgs> nonsubCompleteOrTimeoutEvent;

        public event EventHandler<EventArgs> NonsubCompleteOrTimeoutEvent {
            add {
                if (nonsubCompleteOrTimeoutEvent == null || !nonsubCompleteOrTimeoutEvent.GetInvocationList ().Contains (value)) {
                    nonsubCompleteOrTimeoutEvent += value;
                }
            }
            remove {
                nonsubCompleteOrTimeoutEvent -= value;
            }
        }

        public event EventHandler<EventArgs> heartbeatCompleteOrTimeoutEvent;

        public event EventHandler<EventArgs> HeartbeatCompleteOrTimeoutEvent {
            add {
                if (heartbeatCompleteOrTimeoutEvent == null || !heartbeatCompleteOrTimeoutEvent.GetInvocationList ().Contains (value)) {
                    heartbeatCompleteOrTimeoutEvent += value;
                }
            }
            remove {
                heartbeatCompleteOrTimeoutEvent -= value;
            }
        }

        public event EventHandler<EventArgs> presenceHeartbeatCompleteOrTimeoutEvent;

        public event EventHandler<EventArgs> PresenceHeartbeatCompleteOrTimeoutEvent {
            add {
                if (presenceHeartbeatCompleteOrTimeoutEvent == null || !presenceHeartbeatCompleteOrTimeoutEvent.GetInvocationList ().Contains (value)) {
                    presenceHeartbeatCompleteOrTimeoutEvent += value;
                }
            }
            remove {
                presenceHeartbeatCompleteOrTimeoutEvent -= value;
            }
        }

        internal CurrentRequestTypeEventArgs CreateCurrentRequestTypeEventArgs(CurrentRequestType crt, bool isTimeout){
            CurrentRequestTypeEventArgs crtEa = new CurrentRequestTypeEventArgs();
            crtEa.CurrRequestType = crt;
            crtEa.IsTimeout = isTimeout;
            return crtEa;
        }

        SafeDictionary<CurrentRequestType, object> storedWebRequestParams = new SafeDictionary<CurrentRequestType, object> ();

        internal object GetWebRequestParams<T> (CurrentRequestType aKey){
            if (storedWebRequestParams.ContainsKey (aKey)) {
                if (storedWebRequestParams.ContainsKey (aKey)) {
                    return storedWebRequestParams [aKey];
                }
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("GetWebRequestParams returning false", DateTime.Now.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif
            }
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("GetWebRequestParams returning null", DateTime.Now.ToString ()), LoggingMethod.LevelError);
            #endif
            return null;
        }

        internal void SetWebRequestParams<T> (CurrentRequestType key, WebRequestParams<T> cp){
            object storeCp = cp as object;
            storedWebRequestParams.AddOrUpdate (key, storeCp, (oldData, newData) => storeCp);
        }

        internal void RaiseEvents(bool isTimeout, CurrentRequestType crt)
        {
            StopTimeouts (crt);
            switch(crt){
            case CurrentRequestType.Subscribe:
                subCompleteOrTimeoutEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, isTimeout));
                break;
            case CurrentRequestType.Heartbeat:
                heartbeatCompleteOrTimeoutEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, isTimeout));
                break;
            case CurrentRequestType.PresenceHeartbeat:
                presenceHeartbeatCompleteOrTimeoutEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, isTimeout));
                break;
            case CurrentRequestType.NonSubscribe:
                nonsubCompleteOrTimeoutEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, isTimeout));
                break;
            default:
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("No matching crt", DateTime.Now.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif

                break;
            }
        }

        internal void CheckElapsedTime(CurrentRequestType crt, float timer, UnityWebRequest www)
        {
            if (timer <= 0) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("CheckElapsedTime: timeout {1}",  crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif
                Debug.Log ("CheckElapsedTime timer");
                RaiseEvents (true, crt);
            } else if ((www != null) && (www.isDone)) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("CheckElapsedTime: done {1}",  crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif
                Debug.Log ("CheckElapsedTime done");
                RaiseEvents (false, crt);
            } else if ((timer > 0) && (www == null) && (CheckIfRequestIsRunning(crt))) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("CheckElapsedTime: www null request running {1}",  crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif  

            } else if ((timer > 0) && (www == null) && (!CheckIfRequestIsRunning(crt))) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("CheckElapsedTime: www null request not running timer running {1}",  crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif  

            } else if ((timer > 0) && (!CheckIfRequestIsRunning(crt))) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("CheckElapsedTime: request not running timer running {1}",  crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif  

            } else if ((timer > 0) && (www == null)) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("CheckElapsedTime: www null timer running {1}",  crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif

            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                //LoggingMethod.WriteToLog (string.Format ("CheckElapsedTime: timer {1}",  timer.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif
            }
        }

        internal bool CheckIfRequestIsRunning(CurrentRequestType crt){
            switch (crt) {
            case CurrentRequestType.Subscribe:
                return (!isSubscribeComplete)? true: false;
            case CurrentRequestType.Heartbeat:
                return (!isHearbeatComplete)? true: false;
            case CurrentRequestType.PresenceHeartbeat:
                return (!isPresenceHeartbeatComplete)? true: false;
            case CurrentRequestType.NonSubscribe:
                return (!isNonSubscribeComplete)? true: false;
            default:
                return false;
            }
        }

        internal void CheckPauseTime(CurrentRequestType crt, float timer)
        {
            if (timer <= 0) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("pause timeout {1}",  crt.ToString()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif

                StopTimeouts (crt);

                switch (crt) {
                case CurrentRequestType.Heartbeat:
                    heartbeatResumeEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, false));
                    break;
                case CurrentRequestType.PresenceHeartbeat:
                    presenceHeartbeatResumeEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, false));
                    break;
                case CurrentRequestType.Subscribe:
                    subscribeResumeEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, false));
                    break;
                }
            } 
        }

        void Start(){
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("REDUCE_PUBNUB_COROUTINES is set.",
            DateTime.Now.ToString ()
            ), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
            #endif
        }

        void Update() {
            if (runSubscribeTimer) {
                subscribeTimer -= Time.deltaTime;
                CheckElapsedTime (CurrentRequestType.Subscribe, subscribeTimer, subscribeWww);
            }
            if (runHeartbeatTimer) {
                heartbeatTimer -= Time.deltaTime;
                CheckElapsedTime (CurrentRequestType.Heartbeat, heartbeatTimer, heartbeatWww);
            }
            if (runPresenceHeartbeatTimer) {
                presenceHeartbeatTimer -= Time.deltaTime;
                CheckElapsedTime (CurrentRequestType.PresenceHeartbeat, presenceHeartbeatTimer, presenceHeartbeatWww);
            }
            if (runNonSubscribeTimer) {
                nonSubscribeTimer -= Time.deltaTime;
                CheckElapsedTime (CurrentRequestType.NonSubscribe, nonSubscribeTimer, nonSubscribeWww);
            }
            if (runPresenceHeartbeatPauseTimer) {
                presenceHeartbeatPauseTimer -= Time.deltaTime;
                CheckPauseTime (CurrentRequestType.PresenceHeartbeat, presenceHeartbeatPauseTimer);
            }
            if (runHeartbeatPauseTimer) {
                heartbeatPauseTimer -= Time.deltaTime;
                CheckPauseTime (CurrentRequestType.Heartbeat, heartbeatPauseTimer);
            }
            if (runSubscribePauseTimer) {
                subscribePauseTimer -= Time.deltaTime;
                CheckPauseTime (CurrentRequestType.Subscribe, subscribePauseTimer);
            }
        }

        void WebRequestClass_CompleteEvent<T> (object sender, EventArgs e)
        {
            Debug.Log ("in WebRequestClass_CompleteEvent");
            CurrentRequestTypeEventArgs crtEa = e as CurrentRequestTypeEventArgs;
            if (crtEa != null) {
                WebRequestParams<T> cp = GetWebRequestParams<T> (crtEa.CurrRequestType) as WebRequestParams<T>;

                if (crtEa.IsTimeout) {
                    ProcessTimeout<T> (cp);
                } else {
                    switch (crtEa.CurrRequestType) {
                    case CurrentRequestType.Subscribe:
                        Debug.Log ("in WebRequestClass_CompleteEvent switch");
                        ProcessResponse<T> (subscribeWww, cp);
                        break;
                    case CurrentRequestType.Heartbeat:
                        ProcessResponse<T> (heartbeatWww, cp);
                        break;
                    case CurrentRequestType.PresenceHeartbeat:
                        ProcessResponse<T> (presenceHeartbeatWww, cp);
                        break;
                    case CurrentRequestType.NonSubscribe:
                        ProcessResponse<T> (nonSubscribeWww, cp);
                        break;
                    default:
                        break;
                    }
                }
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("CurrentRequestTypeEventArgs null", DateTime.Now.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif
            }
        }

        void WebRequestClass_ResumeEvent<T> (object sender, EventArgs e){
            CurrentRequestTypeEventArgs crtEa = e as CurrentRequestTypeEventArgs;
            WebRequestParams<T> cp = GetWebRequestParams<T> (crtEa.CurrRequestType) as WebRequestParams<T>;

            StartWebRequestsByName<T> (cp.url, cp.requestState, cp.timeout, cp.pause, cp.crt);
        }

        public void RemoveEventHandler<T>(CurrentRequestType crt, bool removeHeartbeats){
            switch (crt) {
            case CurrentRequestType.Heartbeat:
                if (removeHeartbeats) {
                    HeartbeatCompleteOrTimeoutEvent -= WebRequestClass_CompleteEvent<T>;
                    HeartbeatResumeEvent -= WebRequestClass_ResumeEvent<T>;
                }
                break;
            case CurrentRequestType.PresenceHeartbeat:
                if (removeHeartbeats) {
                    PresenceHeartbeatCompleteOrTimeoutEvent -= WebRequestClass_CompleteEvent<T>;
                    PresenceHeartbeatResumeEvent -= WebRequestClass_ResumeEvent<T>;
                }
                break;
            case CurrentRequestType.Subscribe:
                SubCompleteOrTimeoutEvent -= WebRequestClass_CompleteEvent<T>;
                SubscribeResumeEvent -= WebRequestClass_ResumeEvent<T>;
                break;
            case CurrentRequestType.NonSubscribe:
                NonsubCompleteOrTimeoutEvent -= WebRequestClass_CompleteEvent<T>;
                break;
            default:
                break;
            }            
        }

        internal void StopTimeouts(CurrentRequestType crt){
            switch (crt) {
            case CurrentRequestType.Heartbeat:
                runHeartbeatTimer = false;
                heartbeatTimer = 0;
                runHeartbeatPauseTimer = false;
                heartbeatPauseTimer = 0;
                break;
            case CurrentRequestType.PresenceHeartbeat:
                runPresenceHeartbeatTimer = false;
                presenceHeartbeatTimer = 0;
                runPresenceHeartbeatPauseTimer = false;
                presenceHeartbeatPauseTimer = 0;
                break;
            case CurrentRequestType.Subscribe:
                runSubscribeTimer = false;
                subscribeTimer = 0;
                runSubscribePauseTimer = false;
                subscribePauseTimer = 0;

                break;
            case CurrentRequestType.NonSubscribe:
                runNonSubscribeTimer = false;
                nonSubscribeTimer = 0;
                break;
            default:
                break;
            }
        }

        private EventHandler<EventArgs> subWebRequestComplete;

        public event EventHandler<EventArgs> SubWebRequestComplete {
            add {
                if (subWebRequestComplete == null || !subWebRequestComplete.GetInvocationList ().Contains (value)) {
                    subWebRequestComplete += value;
                }
            }
            remove {
                subWebRequestComplete -= value;
            }
        }

        private EventHandler<EventArgs> nonSubWebRequestComplete;

        public event EventHandler<EventArgs> NonSubWebRequestComplete {
            add {
                if (nonSubWebRequestComplete == null || !nonSubWebRequestComplete.GetInvocationList ().Contains (value)) {
                    nonSubWebRequestComplete += value;
                }
            }
            remove {
                nonSubWebRequestComplete -= value;
            }
        }

        private EventHandler<EventArgs> presenceHeartbeatWebRequestComplete;

        public event EventHandler<EventArgs> PresenceHeartbeatWebRequestComplete {
            add {
                if (presenceHeartbeatWebRequestComplete == null || !presenceHeartbeatWebRequestComplete.GetInvocationList ().Contains (value)) {
                    presenceHeartbeatWebRequestComplete += value;
                }
            }
            remove {
                presenceHeartbeatWebRequestComplete -= value;
            }
        }

        private EventHandler<EventArgs> heartbeatWebRequestComplete;

        public event EventHandler<EventArgs> HeartbeatWebRequestComplete {
            add {
                if (heartbeatWebRequestComplete == null || !heartbeatWebRequestComplete.GetInvocationList ().Contains (value)) {
                    heartbeatWebRequestComplete += value;
                }
            }
            remove {
                heartbeatWebRequestComplete -= value;
            }
        }

        public void DelayStartWebRequest<T>(string url, RequestState<T> pubnubRequestState, int timeout, int pause, CurrentRequestType crt)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DelayStartWebRequest delay: {1} {2}",  pause.ToString(), crt.ToString()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
            #endif

            WebRequestParams<T> cp = new WebRequestParams<T> (url, timeout, pause, crt, typeof(T), pubnubRequestState);
            SetWebRequestParams<T> (crt, cp);

            if (pubnubRequestState.RespType == PNOperationType.PNHeartbeatOperation){
                heartbeatPauseTimer = pause;
                HeartbeatResumeEvent += WebRequestClass_ResumeEvent<T>;
                runHeartbeatPauseTimer = true;
            } else if (pubnubRequestState.RespType == PNOperationType.PNPresenceHeartbeatOperation){
                presenceHeartbeatPauseTimer = pause;
                PresenceHeartbeatResumeEvent += WebRequestClass_ResumeEvent<T>;
                runPresenceHeartbeatPauseTimer = true;
            } else {
                subscribePauseTimer = pause;
                SubscribeResumeEvent += WebRequestClass_ResumeEvent<T>;
                runSubscribePauseTimer = true;
            }
        }


        public void Run<T> (string url, RequestState<T> pubnubRequestState, int timeout, int pause)
        {
            //for heartbeat and presence heartbeat treat reconnect as pause
            CurrentRequestType crt;
            if ((pubnubRequestState.RespType == PNOperationType.PNHeartbeatOperation) || (pubnubRequestState.RespType == PNOperationType.PNHeartbeatOperation)) {
                crt = CurrentRequestType.PresenceHeartbeat;
                if (pubnubRequestState.RespType == PNOperationType.PNHeartbeatOperation) {
                    crt = CurrentRequestType.Heartbeat;
                }
                CheckComplete (crt);

                if (pubnubRequestState.Reconnect) {
                    DelayStartWebRequest<T>(url, pubnubRequestState, timeout, pause, crt);
                } else {
                    StartWebRequestsByName<T> (url, pubnubRequestState, timeout, pause, crt);
                }
            } else if (pubnubRequestState.RespType.Equals(PNOperationType.PNSubscribeOperation) || pubnubRequestState.RespType.Equals(PNOperationType.PNPresenceOperation)
            ) {
                crt = CurrentRequestType.Subscribe;
                Debug.Log ("Run crt"+ crt);
                CheckComplete (crt);

                #if (ENABLE_PUBNUB_LOGGING)
                if ((subscribeWww != null) && (!subscribeWww.isDone)) {
                LoggingMethod.WriteToLog (string.Format ("Run: subscribeWww running trying to abort {1}",  crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                if (subscribeWww == null) {
                LoggingMethod.WriteToLog (string.Format ("Run: subscribeWww aborted {1}",  crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                }
                }
                #endif
                if (pause > 0) {
                    DelayStartWebRequest<T>(url, pubnubRequestState, timeout, pause, crt);
                } else {
                    StartWebRequestsByName<T> (url, pubnubRequestState, timeout, pause, crt);
                }
            } else {
                crt = CurrentRequestType.NonSubscribe;
                Debug.Log ("Run crt");
                CheckComplete (crt);

                StartWebRequestsByName<T> (url, pubnubRequestState, timeout, pause, crt);
            } 
        }

        internal void StartWebRequestsByName<T> (string url, RequestState<T> pubnubRequestState, int timeout, int pause, CurrentRequestType crt)
        {
            WebRequestParams<T> cp = new WebRequestParams<T> (url, timeout, pause, crt, typeof(T), pubnubRequestState);

            SetWebRequestParams<T> (crt, cp);
            StopTimeouts(crt);
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("StartWebRequestsByName: URL {2} {1}", 
             cp.url.ToString (), crt.ToString()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
            #endif

            if (crt == CurrentRequestType.Subscribe) {
                subscribeTimer = timeout;
                runSubscribeTimer = true;
                SubCompleteOrTimeoutEvent += WebRequestClass_CompleteEvent<T>;
                isSubscribeComplete = false;
                Debug.Log ("Run Subscribe StartWebRequestsByName");
                subscribeWww =  UnityWebRequest.Get (cp.url);
                AsyncOperation async = subscribeWww.Send ();
               
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("StartWebRequestsByName: {1} running",  cp.crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif
            } else if (crt == CurrentRequestType.NonSubscribe) {
                nonSubscribeTimer = timeout;
                runNonSubscribeTimer = true;
                NonsubCompleteOrTimeoutEvent += WebRequestClass_CompleteEvent<T>;
                isNonSubscribeComplete = false;
                Debug.Log ("Run StartWebRequestsByName");
                nonSubscribeWww = UnityWebRequest.Get (cp.url);
                Debug.Log ("After StartWebRequestsByName");
                AsyncOperation async = nonSubscribeWww.Send ();

                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("StartWebRequestsByName: {1} running",  cp.crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif
            } else if (crt == CurrentRequestType.PresenceHeartbeat) {
                presenceHeartbeatTimer = timeout;
                runPresenceHeartbeatTimer = true;
                PresenceHeartbeatCompleteOrTimeoutEvent += WebRequestClass_CompleteEvent<T>;
                isPresenceHeartbeatComplete = false;
                presenceHeartbeatWww = UnityWebRequest.Get  (cp.url);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("StartWebRequestsByName: {1} running",  cp.crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif

            } else if (crt == CurrentRequestType.Heartbeat) {
                heartbeatTimer = timeout;
                runHeartbeatTimer = true;
                HeartbeatCompleteOrTimeoutEvent += WebRequestClass_CompleteEvent<T>;
                isHearbeatComplete = false;
                heartbeatWww = UnityWebRequest.Get (cp.url);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("StartWebRequestsByName: {1} running",  cp.crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif
            }
        }

        public IEnumerator DelayRequest<T> (string url, RequestState<T> pubnubRequestState, int timeout, int pause, CurrentRequestType crt)
        {
            yield return new WaitForSeconds (pause); 
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DelayRequest timeout  {1}",  crt.ToString()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
            #endif

            StartWebRequestsByName<T> (url, pubnubRequestState, timeout, pause, crt);
        }

        internal void SetComplete (CurrentRequestType crt)
        {
            try {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("In SetComplete:  {1}",  
                crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif

                StopTimeouts(crt);
                if (crt == CurrentRequestType.Heartbeat) {
                    isHearbeatComplete = true;
                } else if (crt == CurrentRequestType.PresenceHeartbeat) {
                    isPresenceHeartbeatComplete = true;
                } else if (crt == CurrentRequestType.Subscribe) {

                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("SetComplete: {1}",  crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                    #endif

                    isSubscribeComplete = true;
                } else {
                    isNonSubscribeComplete = true;
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("SetComplete: Complete {1}",  crt.ToString()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("SetComplete: Exception: {1}",  ex.ToString ()), LoggingMethod.LevelError);
                #endif
            }

        }            

        public bool CheckComplete (CurrentRequestType crt)
        {
            try {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("CheckComplete:  {1}", 
                 crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif


                StopTimeouts(crt);

                if (crt == CurrentRequestType.Heartbeat) {
                    if ((!isHearbeatComplete) && ((heartbeatWww != null) && (!heartbeatWww.isDone))) { 
                        heartbeatWww.Abort();
                        heartbeatWww.Dispose();
                        /*#if(!UNITY_ANDROID)
                        heartbeatWww.Dispose();
                        #endif*/
                        heartbeatWww = null;
                        return false;
                    }
                } else if (crt == CurrentRequestType.PresenceHeartbeat) {
                    if ((!isPresenceHeartbeatComplete) && ((presenceHeartbeatWww != null) && (!presenceHeartbeatWww.isDone))) {
                        #if(!UNITY_ANDROID)
                        presenceHeartbeatWww.Dispose();
                        #endif
                        presenceHeartbeatWww = null;
                        return false;
                    }
                } else if (crt == CurrentRequestType.Subscribe) {
                    if ((!isSubscribeComplete) && ((subscribeWww != null) && (!subscribeWww.isDone))) {

                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("CheckComplete: DISPOSING WWW", DateTime.Now.ToString ()), LoggingMethod.LevelError);
                        #endif

                        //TODO: Remove flag when unity bug is fixed. Currenlty calling this on Android hangs the whole app. 
                        //Not fixed for Android as of Unity 5.3.5p4
                        #if(!UNITY_ANDROID)
                        subscribeWww.Dispose();
                        #endif

                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("CheckComplete: WWW disposed", DateTime.Now.ToString ()), LoggingMethod.LevelError);
                        #endif

                        subscribeWww = null;
                        return false;
                    }
                } else {
                    if ((!isNonSubscribeComplete) && ((nonSubscribeWww != null) && (!nonSubscribeWww.isDone))) {
                        Debug.Log("Aborting");
                        nonSubscribeWww.Abort();
                        #if(!UNITY_ANDROID)
                        nonSubscribeWww.Dispose();
                        #endif
                        nonSubscribeWww = null;
                        Debug.Log("Aborted");
                        return false;
                    }
                }

            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("CheckComplete: Exception: {1}",  ex.ToString ()), LoggingMethod.LevelError);
                #endif
            }
            return true;
        }

        public void ProcessResponse<T> (UnityWebRequest www, WebRequestParams<T> cp)
        {
            try {
                Debug.Log("in process response -> " + cp.requestState.RespType);
                RemoveEventHandler<T>(cp.crt, false);

                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("ProcessResponse: Process Request {1}, url: {2} ", 
                 cp.crt.ToString (), www.url), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif

                if (www != null) {
                    SetComplete (cp.crt);
                    string message = "";
                    bool isError = false;

                    if(!www.isError) {
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("ProcessResponse: WWW Sub {1}\n Message: {2}\n URL: {3}", 
                         cp.crt.ToString (), www.error, www.url), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                        #endif
                        isError = false;
                        message = www.downloadHandler.text;
                    } else {
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("ProcessResponse: WWW Sub {1}\n Error: {2}\n, text: {3}\n URL: {4}", 
                             cp.crt.ToString (), www.error, www.downloadHandler.text, www.url), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                        #endif
                        message = string.Format ("{0}\"Error\": \"{1}\", \"Description\": {2}{3}", "{", www.error, www.downloadHandler.text, "}");
                        isError = true;
                    } 
                    Debug.Log(message);
                    #if (ENABLE_PUBNUB_LOGGING)
                    if (cp.requestState == null) {
                    LoggingMethod.WriteToLog (string.Format ("ProcessResponse: WWW Sub request null2", 
                    DateTime.Now.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                    } else {
                    LoggingMethod.WriteToLog (string.Format ("ProcessResponse: WWW Sub request2 {1} {2}", 
                     cp.requestState.RespType, cp.crt), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                    }
                    #endif
                    Debug.Log("BEFORE FireEvent");
                    FireEvent (message, isError, false, cp.requestState, cp.crt);
                } 
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("ProcessResponse: RunWebRequestSub {1}, Exception: {2}",  cp.crt.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
            }
        }

        public void BounceRequest<T> (CurrentRequestType crt, RequestState<T> pubnubRequestState, bool fireEvent)
        {
            try {
                CheckComplete (crt);
                SetComplete (crt);

                if ((pubnubRequestState != null) && fireEvent) {
                    FireEvent ("Aborted", true, false, pubnubRequestState, crt);
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("BounceRequest: event fired {1}",  crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                    #endif
                }
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("BounceRequest: Exception: {1}",  ex.ToString ()), LoggingMethod.LevelError);
                #endif
            }
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("BounceRequest: {1}",  crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
            #endif
        }

        public void ProcessTimeout<T> (WebRequestParams<T> cp)
        {
            try {
                RemoveEventHandler<T>(cp.crt, false);

                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("ProcessTimeout: {1}",  cp.crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif

                if (!CheckComplete (cp.crt)) {
                    if ((cp.typeParameterType == typeof(string)) || (cp.typeParameterType == typeof(object))) {
                        FireEvent ("Timed out", true, true, cp.requestState, cp.crt);
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("ProcessTimeout: WWW Error: {1} sec timeout",  cp.timeout.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                        #endif
                    } else {
                        throw new Exception ("'string' and 'object' are the only types supported in generic method calls");
                    }
                }

            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("ProcessTimeout: {1} {2}",  ex.ToString (), cp.crt.ToString ()), LoggingMethod.LevelError);
                #endif
            }
        }

        public void FireEvent<T> (string message, bool isError, bool isTimeout, RequestState<T> pubnubRequestState, CurrentRequestType crt)
        {
            Debug.Log(" FireEvent");
            CustomEventArgs<T> cea = new CustomEventArgs<T> ();
            cea.PubnubRequestState = pubnubRequestState;
            cea.Message = message;
            cea.IsError = isError;
            cea.IsTimeout = isTimeout;
            cea.CurrRequestType = crt;
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("FireEvent: Raising Event of type : {1}",  crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
            #endif

            if ((crt == CurrentRequestType.Heartbeat) && (heartbeatWebRequestComplete != null)) {
                heartbeatWebRequestComplete.Raise (this, cea);
            } else if ((crt == CurrentRequestType.PresenceHeartbeat) && (presenceHeartbeatWebRequestComplete != null)) {
                presenceHeartbeatWebRequestComplete.Raise (this, cea);
            } else if ((crt == CurrentRequestType.Subscribe) && (subWebRequestComplete != null)) {
                Debug.Log("Subscribe FireEvent");
                subWebRequestComplete.Raise (this, cea);
            } else if ((crt == CurrentRequestType.NonSubscribe) && (nonSubWebRequestComplete != null)) {
                nonSubWebRequestComplete.Raise (this, cea);
            } 
            
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                
            LoggingMethod.WriteToLog (string.Format ("FireEvent: Request Type not matched {1}",  crt.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
            }
            #endif
        }
    }

    #region WebRequestClass
    internal class WebRequestParams<T>
    {
        public string url;
        public int timeout;
        public int pause;
        public CurrentRequestType crt;
        public Type typeParameterType;
        public RequestState<T> requestState;

        public WebRequestParams (string url, int timeout, int pause, CurrentRequestType crt, Type typeParameterType, RequestState<T> requestState)
        {
            this.url = url;
            this.timeout = timeout;
            this.pause = pause;
            this.crt = crt;
            this.typeParameterType = typeParameterType;
            this.requestState = requestState;
        }
    }
    #endregion


    #region "PubnubWebResponse and PubnubWebRequest"
    public class PubnubWebResponse
    {
        WWW www;

        public PubnubWebResponse (WWW www)
        {
            this.www = www;
        }

        public string ResponseUri {
            get {
                return www.url;
            }
        }

        public Dictionary<string, string> Headers {
            get {
                return www.responseHeaders;
            }
        }
    }

    public class PubnubWebRequest
    {
        WWW www;

        public PubnubWebRequest (WWW www)
        {
            this.www = www;
        }

        public string RequestUri {
            get {
                return www.url;
            }
        }

        public Dictionary<string, string> Headers {
            get {
                return www.responseHeaders;
            }
        }

    }
    #endregion
}