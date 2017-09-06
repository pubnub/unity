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
        internal PNLoggingMethod PNLog;

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

        internal CurrentRequestTypeEventArgs CreateCurrentRequestTypeEventArgs(PNCurrentRequestType crt, bool isTimeout){
            CurrentRequestTypeEventArgs crtEa = new CurrentRequestTypeEventArgs();
            crtEa.CurrRequestType = crt;
            crtEa.IsTimeout = isTimeout;
            return crtEa;
        }

        SafeDictionary<PNCurrentRequestType, object> storedWebRequestParams = new SafeDictionary<PNCurrentRequestType, object> ();

        internal object GetWebRequestParams<T> (PNCurrentRequestType aKey){
            if (storedWebRequestParams.ContainsKey (aKey)) {
                if (storedWebRequestParams.ContainsKey (aKey)) {
                    return storedWebRequestParams [aKey];
                }
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("GetWebRequestParams returning false"), PNLoggingMethod.LevelInfo);
                #endif
            }
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("GetWebRequestParams returning null"), PNLoggingMethod.LevelError);
            #endif
            return null;
        }

        internal void SetWebRequestParams<T> (PNCurrentRequestType key, WebRequestParams<T> cp){
            object storeCp = cp as object;
            storedWebRequestParams.AddOrUpdate (key, storeCp, (oldData, newData) => storeCp);
        }

        internal void RaiseEvents(bool isTimeout, PNCurrentRequestType crt)
        {
            StopTimeouts (crt);
            switch(crt){
            case PNCurrentRequestType.Subscribe:
                subCompleteOrTimeoutEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, isTimeout));
                break;
            case PNCurrentRequestType.Heartbeat:
                heartbeatCompleteOrTimeoutEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, isTimeout));
                break;
            case PNCurrentRequestType.PresenceHeartbeat:
                presenceHeartbeatCompleteOrTimeoutEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, isTimeout));
                break;
            case PNCurrentRequestType.NonSubscribe:
                nonsubCompleteOrTimeoutEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, isTimeout));
                break;
            default:
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("No matching crt"), PNLoggingMethod.LevelInfo);
                #endif

                break;
            }
        }

        internal void CheckElapsedTime(PNCurrentRequestType crt, float timer, UnityWebRequest www)
        {
            if (timer <= 0) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: timeout {0}",  crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
                Debug.Log ("CheckElapsedTime timer");
                RaiseEvents (true, crt);
            } else if ((www != null) && (www.isDone)) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: done {0}",  crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
                Debug.Log ("CheckElapsedTime done");
                RaiseEvents (false, crt);
            } else if ((timer > 0) && (www == null) && (CheckIfRequestIsRunning(crt))) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: www null request running {0}",  crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif  

            } else if ((timer > 0) && (www == null) && (!CheckIfRequestIsRunning(crt))) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: www null request not running timer running {0}",  crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif  

            } else if ((timer > 0) && (!CheckIfRequestIsRunning(crt))) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: request not running timer running {0}",  crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif  

            } else if ((timer > 0) && (www == null)) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: www null timer running {0}",  crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif

            } else {
                /*#if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: timer {0}",  timer.ToString ()), PNLoggingMethod.LevelInfo);
                #endif*/
            }
        }

        internal bool CheckIfRequestIsRunning(PNCurrentRequestType crt){
            switch (crt) {
            case PNCurrentRequestType.Subscribe:
                return (!isSubscribeComplete)? true: false;
            case PNCurrentRequestType.Heartbeat:
                return (!isHearbeatComplete)? true: false;
            case PNCurrentRequestType.PresenceHeartbeat:
                return (!isPresenceHeartbeatComplete)? true: false;
            case PNCurrentRequestType.NonSubscribe:
                return (!isNonSubscribeComplete)? true: false;
            default:
                return false;
            }
        }

        internal void CheckPauseTime(PNCurrentRequestType crt, float timer)
        {
            if (timer <= 0) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("pause timeout {0}",  crt.ToString()), PNLoggingMethod.LevelInfo);
                #endif

                StopTimeouts (crt);

                switch (crt) {
                case PNCurrentRequestType.Heartbeat:
                    heartbeatResumeEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, false));
                    break;
                case PNCurrentRequestType.PresenceHeartbeat:
                    presenceHeartbeatResumeEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, false));
                    break;
                case PNCurrentRequestType.Subscribe:
                    subscribeResumeEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, false));
                    break;
                }
            } 
        }

        void Start(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("REDUCE_PUBNUB_COROUTINES is set."), PNLoggingMethod.LevelInfo);
            #endif
        }

        void Update() {
            if (runSubscribeTimer) {
                subscribeTimer -= Time.deltaTime;
                CheckElapsedTime (PNCurrentRequestType.Subscribe, subscribeTimer, subscribeWww);
            }
            if (runHeartbeatTimer) {
                heartbeatTimer -= Time.deltaTime;
                CheckElapsedTime (PNCurrentRequestType.Heartbeat, heartbeatTimer, heartbeatWww);
            }
            if (runPresenceHeartbeatTimer) {
                presenceHeartbeatTimer -= Time.deltaTime;
                CheckElapsedTime (PNCurrentRequestType.PresenceHeartbeat, presenceHeartbeatTimer, presenceHeartbeatWww);
            }
            if (runNonSubscribeTimer) {
                nonSubscribeTimer -= Time.deltaTime;
                CheckElapsedTime (PNCurrentRequestType.NonSubscribe, nonSubscribeTimer, nonSubscribeWww);
            }
            if (runPresenceHeartbeatPauseTimer) {
                presenceHeartbeatPauseTimer -= Time.deltaTime;
                CheckPauseTime (PNCurrentRequestType.PresenceHeartbeat, presenceHeartbeatPauseTimer);
            }
            if (runHeartbeatPauseTimer) {
                heartbeatPauseTimer -= Time.deltaTime;
                CheckPauseTime (PNCurrentRequestType.Heartbeat, heartbeatPauseTimer);
            }
            if (runSubscribePauseTimer) {
                subscribePauseTimer -= Time.deltaTime;
                CheckPauseTime (PNCurrentRequestType.Subscribe, subscribePauseTimer);
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
                    case PNCurrentRequestType.Subscribe:
                        Debug.Log ("in WebRequestClass_CompleteEvent switch");
                        ProcessResponse<T> (subscribeWww, cp);
                        break;
                    case PNCurrentRequestType.Heartbeat:
                        Debug.Log ("in Heartbeat switch");
                        ProcessResponse<T> (heartbeatWww, cp);
                        break;
                    case PNCurrentRequestType.PresenceHeartbeat:
                        ProcessResponse<T> (presenceHeartbeatWww, cp);
                        break;
                    case PNCurrentRequestType.NonSubscribe:
                        ProcessResponse<T> (nonSubscribeWww, cp);
                        break;
                    default:
                        break;
                    }
                }
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CurrentRequestTypeEventArgs null"), PNLoggingMethod.LevelInfo);
                #endif
            }
        }

        void WebRequestClass_ResumeEvent<T> (object sender, EventArgs e){
            CurrentRequestTypeEventArgs crtEa = e as CurrentRequestTypeEventArgs;
            WebRequestParams<T> cp = GetWebRequestParams<T> (crtEa.CurrRequestType) as WebRequestParams<T>;

            StartWebRequestsByName<T> (cp.url, cp.requestState, cp.timeout, cp.pause, cp.crt);
        }

        public void RemoveEventHandler<T>(PNCurrentRequestType crt, bool removeHeartbeats){
            switch (crt) {
            case PNCurrentRequestType.Heartbeat:
                if (removeHeartbeats) {
                    HeartbeatCompleteOrTimeoutEvent -= WebRequestClass_CompleteEvent<T>;
                    HeartbeatResumeEvent -= WebRequestClass_ResumeEvent<T>;
                }
                break;
            case PNCurrentRequestType.PresenceHeartbeat:
                if (removeHeartbeats) {
                    PresenceHeartbeatCompleteOrTimeoutEvent -= WebRequestClass_CompleteEvent<T>;
                    PresenceHeartbeatResumeEvent -= WebRequestClass_ResumeEvent<T>;
                }
                break;
            case PNCurrentRequestType.Subscribe:
                SubCompleteOrTimeoutEvent -= WebRequestClass_CompleteEvent<T>;
                SubscribeResumeEvent -= WebRequestClass_ResumeEvent<T>;
                break;
            case PNCurrentRequestType.NonSubscribe:
                NonsubCompleteOrTimeoutEvent -= WebRequestClass_CompleteEvent<T>;
                break;
            default:
                break;
            }            
        }

        internal void StopTimeouts(PNCurrentRequestType crt){
            switch (crt) {
            case PNCurrentRequestType.Heartbeat:
                runHeartbeatTimer = false;
                heartbeatTimer = 0;
                runHeartbeatPauseTimer = false;
                heartbeatPauseTimer = 0;
                break;
            case PNCurrentRequestType.PresenceHeartbeat:
                runPresenceHeartbeatTimer = false;
                presenceHeartbeatTimer = 0;
                runPresenceHeartbeatPauseTimer = false;
                presenceHeartbeatPauseTimer = 0;
                break;
            case PNCurrentRequestType.Subscribe:
                runSubscribeTimer = false;
                subscribeTimer = 0;
                runSubscribePauseTimer = false;
                subscribePauseTimer = 0;

                break;
            case PNCurrentRequestType.NonSubscribe:
                runNonSubscribeTimer = false;
                nonSubscribeTimer = 0;
                break;
            default:
                break;
            }
        }

        private EventHandler<EventArgs> subWebRequestComplete;
        //TODO remove single instance
        public event EventHandler<EventArgs> SubWebRequestComplete {
            add {
                if (subWebRequestComplete == null || !subWebRequestComplete.GetInvocationList ().Contains (value)) {
                    subWebRequestComplete += value;
                } else {
                    Debug.Log("SubWebRequestComplete instantiating more than one instance");
                }
            }
            remove {
                subWebRequestComplete -= value;
            }
        }

        private EventHandler<EventArgs> nonSubWebRequestComplete;
        //TODO remove single instance
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
        //TODO remove single instance
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
        //TODO remove single instance
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

        public void DelayStartWebRequest<T>(string url, RequestState pubnubRequestState, int timeout, int pause, PNCurrentRequestType crt)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("DelayStartWebRequest delay: {0} {1}",  pause.ToString(), crt.ToString()), PNLoggingMethod.LevelInfo);
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

        public void Run<T> (string url, RequestState pubnubRequestState, int timeout, int pause, bool reconnect)
        {
            //for heartbeat and presence heartbeat treat reconnect as pause
            PNCurrentRequestType crt;
            string wwwUrl;
            Debug.Log("pubnubRequestState:"+ pubnubRequestState.RespType);
            if ((pubnubRequestState.RespType == PNOperationType.PNHeartbeatOperation) || (pubnubRequestState.RespType == PNOperationType.PNPresenceHeartbeatOperation)) {
                crt = PNCurrentRequestType.PresenceHeartbeat;
                if (pubnubRequestState.RespType == PNOperationType.PNHeartbeatOperation) {
                    crt = PNCurrentRequestType.Heartbeat;
                }
                CheckComplete (crt, out wwwUrl);

                if (reconnect) {
                    DelayStartWebRequest<T>(url, pubnubRequestState, timeout, pause, crt);
                } else {
                    StartWebRequestsByName<T> (url, pubnubRequestState, timeout, pause, crt);
                }
                Debug.Log("crt:"+ crt);
            } else if (pubnubRequestState.RespType.Equals(PNOperationType.PNSubscribeOperation) || pubnubRequestState.RespType.Equals(PNOperationType.PNPresenceOperation)
            ) {
                crt = PNCurrentRequestType.Subscribe;
                Debug.Log ("Run crt"+ crt);
                CheckComplete (crt, out wwwUrl);

                #if (ENABLE_PUBNUB_LOGGING)
                if ((subscribeWww != null) && (!subscribeWww.isDone)) {
                this.PNLog.WriteToLog (string.Format ("Run: subscribeWww running trying to abort {0}",  crt.ToString ()), PNLoggingMethod.LevelInfo);
                if (subscribeWww == null) {
                this.PNLog.WriteToLog (string.Format ("Run: subscribeWww aborted {0}",  crt.ToString ()), PNLoggingMethod.LevelInfo);
                }
                }
                #endif
                if (pause > 0) {
                    DelayStartWebRequest<T>(url, pubnubRequestState, timeout, pause, crt);
                } else {
                    StartWebRequestsByName<T> (url, pubnubRequestState, timeout, pause, crt);
                }
            } else {
                crt = PNCurrentRequestType.NonSubscribe;
                Debug.Log ("Run crt");
                CheckComplete (crt, out wwwUrl);

                StartWebRequestsByName<T> (url, pubnubRequestState, timeout, pause, crt);
            } 
        }

        internal void StartWebRequestsByName<T> (string url, RequestState pubnubRequestState, int timeout, int pause, PNCurrentRequestType crt)
        {
            WebRequestParams<T> cp = new WebRequestParams<T> (url, timeout, pause, crt, typeof(T), pubnubRequestState);

            SetWebRequestParams<T> (crt, cp);
            StopTimeouts(crt);
            Debug.Log ("Run StartWebRequestsByName");
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("StartWebRequestsByName: URL {1} {0}", cp.url.ToString (), crt.ToString()), PNLoggingMethod.LevelInfo);
            #endif

            if (crt == PNCurrentRequestType.Subscribe) {
                subscribeTimer = timeout;
                runSubscribeTimer = true;
                SubCompleteOrTimeoutEvent += WebRequestClass_CompleteEvent<T>;
                isSubscribeComplete = false;
                Debug.Log ("Run Subscribe StartWebRequestsByName");
                subscribeWww =  UnityWebRequest.Get (cp.url);
                AsyncOperation async = subscribeWww.Send ();
               
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("StartWebRequestsByName: {0} running",  cp.crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
            } else if (crt == PNCurrentRequestType.NonSubscribe) {
                nonSubscribeTimer = timeout;
                runNonSubscribeTimer = true;
                NonsubCompleteOrTimeoutEvent += WebRequestClass_CompleteEvent<T>;
                isNonSubscribeComplete = false;
                Debug.Log ("Run NonSubscribe StartWebRequestsByName");
                nonSubscribeWww = UnityWebRequest.Get (cp.url);
                Debug.Log ("After NonSubscribe StartWebRequestsByName");
                AsyncOperation async = nonSubscribeWww.Send ();

                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("StartWebRequestsByName: {0} running",  cp.crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
            } else if (crt == PNCurrentRequestType.PresenceHeartbeat) {
                presenceHeartbeatTimer = timeout;
                runPresenceHeartbeatTimer = true;
                PresenceHeartbeatCompleteOrTimeoutEvent += WebRequestClass_CompleteEvent<T>;
                isPresenceHeartbeatComplete = false;
                Debug.Log ("Run PresenceHeartbeat StartWebRequestsByName");
                presenceHeartbeatWww = UnityWebRequest.Get  (cp.url);
                AsyncOperation async = presenceHeartbeatWww.Send ();
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("StartWebRequestsByName: {0} running",  cp.crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif

            } else if (crt == PNCurrentRequestType.Heartbeat) {
                Debug.Log (" StartWebRequestsByName" + crt);
                heartbeatTimer = timeout;
                runHeartbeatTimer = true;
                HeartbeatCompleteOrTimeoutEvent += WebRequestClass_CompleteEvent<T>;
                isHearbeatComplete = false;
                Debug.Log ("Run Heartbeat StartWebRequestsByName");
                heartbeatWww = UnityWebRequest.Get (cp.url);
                AsyncOperation async = heartbeatWww.Send ();
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("StartWebRequestsByName: {0} running",  cp.crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
            }
        }

        public IEnumerator DelayRequest<T> (string url, RequestState pubnubRequestState, int timeout, int pause, PNCurrentRequestType crt)
        {
            yield return new WaitForSeconds (pause); 
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("DelayRequest timeout  {0}",  crt.ToString()), PNLoggingMethod.LevelInfo);
            #endif

            StartWebRequestsByName<T> (url, pubnubRequestState, timeout, pause, crt);
        }

        internal void SetComplete (PNCurrentRequestType crt)
        {
            try {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("In SetComplete:  {0}", crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif

                StopTimeouts(crt);
                if (crt == PNCurrentRequestType.Heartbeat) {
                    isHearbeatComplete = true;
                } else if (crt == PNCurrentRequestType.PresenceHeartbeat) {
                    isPresenceHeartbeatComplete = true;
                } else if (crt == PNCurrentRequestType.Subscribe) {

                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PNLog.WriteToLog (string.Format ("SetComplete: {0}",  crt.ToString ()), PNLoggingMethod.LevelInfo);
                    #endif

                    isSubscribeComplete = true;
                } else {
                    isNonSubscribeComplete = true;
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("SetComplete: Complete {0}",  crt.ToString()), PNLoggingMethod.LevelInfo);
                #endif
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("SetComplete: Exception: {0}",  ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
            }

        }            

        public bool CheckComplete (PNCurrentRequestType crt, out string wwwUrl)
        {
            try {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckComplete:  {0}", crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif


                StopTimeouts(crt);

                if (crt == PNCurrentRequestType.Heartbeat) {
                    if ((!isHearbeatComplete) && ((heartbeatWww != null) && (!heartbeatWww.isDone))) { 
                        wwwUrl = heartbeatWww.url;
                        heartbeatWww.Abort();
                        heartbeatWww.Dispose();
                        /*#if(!UNITY_ANDROID)
                        heartbeatWww.Dispose();
                        #endif*/
                        heartbeatWww = null;
                        return false;
                    }
                } else if (crt == PNCurrentRequestType.PresenceHeartbeat) {
                    if ((!isPresenceHeartbeatComplete) && ((presenceHeartbeatWww != null) && (!presenceHeartbeatWww.isDone))) {
                        wwwUrl = presenceHeartbeatWww.url;
                        #if(!UNITY_ANDROID)
                        presenceHeartbeatWww.Dispose();
                        #endif
                        
                        presenceHeartbeatWww = null;
                        
                        return false;
                    }
                } else if (crt == PNCurrentRequestType.Subscribe) {
                    if ((!isSubscribeComplete) && ((subscribeWww != null) && (!subscribeWww.isDone))) {
                        wwwUrl = subscribeWww.url;
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PNLog.WriteToLog (string.Format ("CheckComplete: DISPOSING WWW"), PNLoggingMethod.LevelError);
                        #endif

                        //TODO: Remove flag when unity bug is fixed. Currenlty calling this on Android hangs the whole app. 
                        //Not fixed for Android as of Unity 5.3.5p4
                        #if(!UNITY_ANDROID)
                        subscribeWww.Dispose();
                        #endif

                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PNLog.WriteToLog (string.Format ("CheckComplete: WWW disposed"), PNLoggingMethod.LevelError);
                        #endif

                        subscribeWww = null;
                        return false;
                    }
                } else {
                    if ((!isNonSubscribeComplete) && ((nonSubscribeWww != null) && (!nonSubscribeWww.isDone))) {
                        wwwUrl = nonSubscribeWww.url;
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
                Debug.Log(ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckComplete: Exception: {0}",  ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
            }
            wwwUrl = "";
            return true;
        }

        public void ProcessResponse<T> (UnityWebRequest www, WebRequestParams<T> cp)
        {
            try {
                Debug.Log("in process response -> " + cp.requestState.RespType);
                RemoveEventHandler<T>(cp.crt, false);

                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("ProcessResponse: Process Request {0}, url: {1} ", cp.crt.ToString (), www.url), PNLoggingMethod.LevelInfo);
                #endif

                if (www != null) {
                    SetComplete (cp.crt);
                    string message = "";
                    bool isError = false;

                    if(!www.isError) {
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PNLog.WriteToLog (string.Format ("ProcessResponse: WWW Sub {0}\n Message: {1}\n URL: {2}", cp.crt.ToString (), www.error, www.url), PNLoggingMethod.LevelInfo);
                        #endif
                        isError = false;
                        message = www.downloadHandler.text;
                    } else {
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PNLog.WriteToLog (string.Format ("ProcessResponse: WWW Sub {0}\n Error: {1}\n, text: {2}\n URL: {3}", cp.crt.ToString (), www.error, www.downloadHandler.text, www.url), PNLoggingMethod.LevelInfo);
                        #endif
                        message = string.Format ("{0}\"Error\": \"{1}\", \"Description\": {2}{3}", "{", www.error, www.downloadHandler.text, "}");
                        isError = true;
                    } 
                    Debug.Log(message);
                    #if (ENABLE_PUBNUB_LOGGING)
                    if (cp.requestState == null) {
                        this.PNLog.WriteToLog (string.Format ("ProcessResponse: WWW Sub request null2"), PNLoggingMethod.LevelInfo);
                    } else {
                        this.PNLog.WriteToLog (string.Format ("ProcessResponse: WWW Sub request2 {0} {1}", cp.requestState.RespType, cp.crt), PNLoggingMethod.LevelInfo);
                    }
                    #endif
                    Debug.Log("BEFORE FireEvent");
                    cp.requestState.ResponseCode = www.responseCode;
                    cp.requestState.URL = www.url;                    
                    FireEvent<T> (message, isError, false, cp.requestState, cp.crt);
                } 
            } catch (Exception ex) {
                Debug.Log(ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("ProcessResponse: RunWebRequestSub {0}, Exception: {1}",  cp.crt.ToString (), ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
            }
        }

        public void AbortRequest<T> (PNCurrentRequestType crt, RequestState pubnubRequestState, bool fireEvent)
        {
            try {
                string wwwUrl;
                CheckComplete (crt, out wwwUrl);
                SetComplete (crt);                

                if ((pubnubRequestState != null) && fireEvent) {
                    pubnubRequestState.ResponseCode = 0;
                    pubnubRequestState.URL = wwwUrl;
                    FireEvent<T> ("Aborted", true, false, pubnubRequestState, crt);
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PNLog.WriteToLog (string.Format ("BounceRequest: event fired {0}",  crt.ToString ()), PNLoggingMethod.LevelInfo);
                    #endif
                }
            } catch (Exception ex) {
                Debug.Log(ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("BounceRequest: Exception: {0}",  ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
            }
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("BounceRequest: {0}",  crt.ToString ()), PNLoggingMethod.LevelInfo);
            #endif
        }

        public void ProcessTimeout<T> (WebRequestParams<T> cp)
        {
            try {
                RemoveEventHandler<T>(cp.crt, false);

                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("ProcessTimeout: {0}",  cp.crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
                string wwwUrl;

                if (!CheckComplete (cp.crt, out wwwUrl)) {
                    //if ((cp.typeParameterType == typeof(string)) || (cp.typeParameterType == typeof(object))) {
                        cp.requestState.ResponseCode = 0;
                        cp.requestState.URL = wwwUrl;
                        FireEvent<T> ("Timed out", true, true, cp.requestState, cp.crt);
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PNLog.WriteToLog (string.Format ("ProcessTimeout: WWW Error: {0} sec timeout",  cp.timeout.ToString ()), PNLoggingMethod.LevelInfo);
                        #endif
                    /*} else {
                        throw new Exception ("'string' and 'object' are the only types supported in generic method calls");
                    }*/
                }

            } catch (Exception ex) {
                Debug.Log(ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("ProcessTimeout: {0} {1}",  ex.ToString (), cp.crt.ToString ()), PNLoggingMethod.LevelError);
                #endif
            }
        }

        public void FireEvent<T> (string message, bool isError, bool isTimeout, RequestState pubnubRequestState, PNCurrentRequestType crt)
        {
            Debug.Log(" FireEvent" + crt);
            CustomEventArgs<T> cea = new CustomEventArgs<T> ();
            cea.PubNubRequestState = pubnubRequestState;
            cea.Message = message;
            cea.IsError = isError;
            cea.IsTimeout = isTimeout;
            cea.CurrRequestType = crt;
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("FireEvent: Raising Event of type : {0}",  crt.ToString ()), PNLoggingMethod.LevelInfo);
            #endif

            if ((crt == PNCurrentRequestType.Heartbeat) && (heartbeatWebRequestComplete != null)) {
                Debug.Log("heartbeatWebRequestComplete FireEvent" + cea.Message + typeof(T));
                heartbeatWebRequestComplete.Raise (this, cea);
            } else if ((crt == PNCurrentRequestType.PresenceHeartbeat) && (presenceHeartbeatWebRequestComplete != null)) {
                presenceHeartbeatWebRequestComplete.Raise (this, cea);
            } else if ((crt == PNCurrentRequestType.Subscribe) && (subWebRequestComplete != null)) {
                Debug.Log("Subscribe FireEvent" + cea.Message + typeof(T));
                subWebRequestComplete.Raise (this, cea);
            } else if ((crt == PNCurrentRequestType.NonSubscribe) && (nonSubWebRequestComplete != null)) {
                nonSubWebRequestComplete.Raise (this, cea);
            } 
            
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                this.PNLog.WriteToLog (string.Format ("FireEvent: Request Type not matched {0}",  crt.ToString ()), PNLoggingMethod.LevelInfo);
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
        public PNCurrentRequestType crt;
        public Type typeParameterType;
        public RequestState requestState;

        public WebRequestParams (string url, int timeout, int pause, PNCurrentRequestType crt, Type typeParameterType, RequestState requestState)
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