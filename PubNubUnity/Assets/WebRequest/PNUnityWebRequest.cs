using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Networking;
using System.Text;

namespace PubNubAPI
{
    internal class PNUnityWebRequest: MonoBehaviour
    {
        #region "IL2CPP workarounds"
        internal PNLoggingMethod PNLog;

        private object syncRoot = new System.Object();

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

        public const float timerConst = 0; 
        public float timer = timerConst; 

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

        SafeDictionary<string, UnityWebRequestWrapper> currentWebRequests = new SafeDictionary<string, UnityWebRequestWrapper> ();

        /*internal UnityWebRequestWrapper subscribeRequest;
        internal UnityWebRequestWrapper heartbeatRequest;
        internal UnityWebRequestWrapper presenceHeartbeatRequest;
        internal UnityWebRequestWrapper nonSubscribeRequest;

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

        //public event EventHandler<EventArgs> subscribeResumeEvent;

        public event EventHandler<EventArgs> ResumeEvent; /*{
            add {
                if (subscribeResumeEvent == null || !subscribeResumeEvent.GetInvocationList ().Contains (value)) {
                    subscribeResumeEvent += value;
                }
            }
            remove {
                subscribeResumeEvent -= value;
            }
        }*/

        //public event EventHandler<EventArgs> completeOrTimeoutEvent;

        public event EventHandler<EventArgs> CompleteOrTimeoutEvent; /*{
            add {
                if (completeOrTimeoutEvent == null || !completeOrTimeoutEvent.GetInvocationList ().Contains (value)) {
                    completeOrTimeoutEvent += value;
                }
            }
            remove {
                completeOrTimeoutEvent -= value;
            }
        }*/

        /*public event EventHandler<EventArgs> nonsubCompleteOrTimeoutEvent;

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
        }*/

        internal CurrentRequestTypeEventArgs CreateCurrentRequestTypeEventArgs(UnityWebRequestWrapper unityWebRequestWrapper, bool isTimeout){
            CurrentRequestTypeEventArgs crtEa = new CurrentRequestTypeEventArgs();
            crtEa.WebRequestWrapper = unityWebRequestWrapper;
            crtEa.IsTimeout = isTimeout;
            return crtEa;
        }

        /*SafeDictionary<PNCurrentRequestType, object> storedWebRequestParams = new SafeDictionary<PNCurrentRequestType, object> ();

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
        }*/

        internal void RaiseEvents(bool isTimeout, UnityWebRequestWrapper unityWebRequestWrapper, string key)
        {
            StopTimeoutsAndComplete (unityWebRequestWrapper, key, false);
            
            if (isTimeout) {
                ProcessTimeout (unityWebRequestWrapper, key, unityWebRequestWrapper.IsComplete);
            } else {
                ProcessResponse (unityWebRequestWrapper, key);
            }
            //CompleteOrTimeoutEvent.Raise(this, CreateCurrentRequestTypeEventArgs(unityWebRequestWrapper, isTimeout));

            /*switch(crt){
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
            }*/
        }

        internal void CheckElapsedTime(UnityWebRequestWrapper unityWebRequestWrapper, string key)
        {
            if (unityWebRequestWrapper.Timer <= 0) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: timeout {0}",  unityWebRequestWrapper.CurrentRequestType.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
                Debug.Log ("CheckElapsedTime timer");
                RaiseEvents (true, unityWebRequestWrapper, key);
            } else if ((unityWebRequestWrapper != null) && ((unityWebRequestWrapper.CurrentUnityWebRequest != null) && (unityWebRequestWrapper.CurrentUnityWebRequest.isDone))) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: done {0}",  unityWebRequestWrapper.CurrentRequestType.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
                Debug.Log ("CheckElapsedTime done");
                RaiseEvents (false, unityWebRequestWrapper, key);
            } else if ((unityWebRequestWrapper.Timer > 0) && ((unityWebRequestWrapper.CurrentUnityWebRequest != null) && (unityWebRequestWrapper.CurrentUnityWebRequest.isDone))) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: www null timer running {0}",  unityWebRequestWrapper.CurrentRequestType.ToString ()), PNLoggingMethod.LevelInfo);
                #endif  

            } else if ((unityWebRequestWrapper.Timer > 0) && (unityWebRequestWrapper == null)) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: www null request not running timer running {0}",  unityWebRequestWrapper.CurrentRequestType.ToString ()), PNLoggingMethod.LevelInfo);
                #endif  

            } else if ((unityWebRequestWrapper.Timer > 0) && ((unityWebRequestWrapper.CurrentUnityWebRequest != null) && (!unityWebRequestWrapper.CurrentUnityWebRequest.isDone))) {
                #if (ENABLE_PUBNUB_LOGGING)
                //this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: request running timer running {0}",  unityWebRequestWrapper.CurrentRequestType.ToString ()), PNLoggingMethod.LevelInfo);
                #endif  

            /*} else if ((unityWebRequestWrapper.Timer > 0) && (unityWebRequestWrapper == null)) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: www null timer running {0}",  unityWebRequestWrapper.CurrentRequestType.ToString ()), PNLoggingMethod.LevelInfo);
                #endif*/

            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: timer {0}",  unityWebRequestWrapper.Timer.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
            }
        }

        /*internal bool CheckIfRequestIsRunning(PNCurrentRequestType crt){
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
        }*/

        internal void CheckPauseTime(UnityWebRequestWrapper unityWebRequestWrapper, string key)
        {
            if (unityWebRequestWrapper.PauseTimer <= 0) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("pause timeout {0} {1}",  unityWebRequestWrapper.CurrentRequestType.ToString(), unityWebRequestWrapper.PauseTimer.ToString()), PNLoggingMethod.LevelInfo);
                #endif

                StopTimeoutsAndComplete (unityWebRequestWrapper, key, true);
                
                //ResumeEvent.Raise(this, CreateCurrentRequestTypeEventArgs(unityWebRequestWrapper, false));
                StartWebRequests(unityWebRequestWrapper, key);


                /*switch (unityWebRequestWrapper.CurrentRequestType) {
                case PNCurrentRequestType.Heartbeat:
                    heartbeatResumeEvent.Raise (this, CreateCurrentRequestTypeEventArgs(unityWebRequestWrapper.CurrentRequestType, false));
                    break;
                case PNCurrentRequestType.PresenceHeartbeat:
                    presenceHeartbeatResumeEvent.Raise (this, CreateCurrentRequestTypeEventArgs(unityWebRequestWrapper.CurrentRequestType, false));
                    break;
                case PNCurrentRequestType.Subscribe:
                    subscribeResumeEvent.Raise (this, CreateCurrentRequestTypeEventArgs(unityWebRequestWrapper.CurrentRequestType, false));
                    break;
                }*/
            } 
        }

        void Start(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("REDUCE_PUBNUB_COROUTINES is set."), PNLoggingMethod.LevelInfo);
            #endif
            //ResumeEvent += WebRequestClass_ResumeEvent;
            //CompleteOrTimeoutEvent += WebRequestClass_CompleteEvent;
        }

        void Update() {
            timer -= Time.deltaTime;
            if(timer <= 0){
                List<string> keys = new List<string>(currentWebRequests.Keys);
                foreach(string key in keys){
                    UnityWebRequestWrapper unityWebRequestWrapper;
                    if(currentWebRequests.TryGetValue(key, out unityWebRequestWrapper)){
                        if(unityWebRequestWrapper.RunTimer){
                            unityWebRequestWrapper.Timer -= Time.deltaTime;
                            currentWebRequests[key] = unityWebRequestWrapper;
                            CheckElapsedTime (unityWebRequestWrapper, key);
                        }
                        if(unityWebRequestWrapper.RunPauseTimer){
                            unityWebRequestWrapper.PauseTimer -= Time.deltaTime;
                            currentWebRequests[key] = unityWebRequestWrapper;
                            CheckPauseTime (unityWebRequestWrapper, key);
                        }
                    }
                }
                timer = timerConst;
            }
            /*if (runSubscribeTimer) {
                subscribeTimer -= Time.deltaTime;
                CheckElapsedTime (PNCurrentRequestType.Subscribe, subscribeTimer, subscribeRequest);
            }
            if (runHeartbeatTimer) {
                heartbeatTimer -= Time.deltaTime;
                CheckElapsedTime (PNCurrentRequestType.Heartbeat, heartbeatTimer, heartbeatRequest);
            }
            if (runPresenceHeartbeatTimer) {
                presenceHeartbeatTimer -= Time.deltaTime;
                CheckElapsedTime (PNCurrentRequestType.PresenceHeartbeat, presenceHeartbeatTimer, presenceHeartbeatRequest);
            }
            if (runNonSubscribeTimer) {
                nonSubscribeTimer -= Time.deltaTime;
                CheckElapsedTime (PNCurrentRequestType.NonSubscribe, nonSubscribeTimer, nonSubscribeRequest);
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
            }*/
        }

        /*void WebRequestClass_CompleteEvent<T> (object sender, EventArgs e)
        {
            Debug.Log ("in WebRequestClass_CompleteEvent");
            CurrentRequestTypeEventArgs crtEa = e as CurrentRequestTypeEventArgs;
            
            if (crtEa != null) {
                //WebRequestParams<T> cp = GetWebRequestParams<T> (crtEa.CurrRequestType) as WebRequestParams<T>;


                if (crtEa.IsTimeout) {
                    ProcessTimeout<T> (crtEa.WebRequestWrapper);
                } else {
                    ProcessResponse<T> (crtEa.WebRequestWrapper);
                    switch (crtEa.CurrRequestType) {
                    case PNCurrentRequestType.Subscribe:
                        Debug.Log ("in WebRequestClass_CompleteEvent switch");
                        ProcessResponse<T> (subscribeRequest, cp);
                        break;
                    case PNCurrentRequestType.Heartbeat:
                        Debug.Log ("in Heartbeat switch");
                        ProcessResponse<T> (heartbeatRequest, cp);
                        break;
                    case PNCurrentRequestType.PresenceHeartbeat:
                        ProcessResponse<T> (presenceHeartbeatRequest, cp);
                        break;
                    case PNCurrentRequestType.NonSubscribe:
                        ProcessResponse<T> (nonSubscribeRequest, cp);
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
        }*/

        /*public void RemoveEventHandler<T>(PNCurrentRequestType crt, bool removeHeartbeats){
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
        }*/

        internal void StopTimeoutsAndComplete(UnityWebRequestWrapper unityWebRequestWrapper, string key, bool pauseOnly)
        {
            unityWebRequestWrapper.PauseTimer = 0;
            unityWebRequestWrapper.RunPauseTimer = false;

            if(!pauseOnly){
                unityWebRequestWrapper.RunTimer = false;
                unityWebRequestWrapper.Timer = 0;
                unityWebRequestWrapper.IsComplete = true;
            }
            
            currentWebRequests[key] = unityWebRequestWrapper;
            /*switch (crt) {
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
            }*/
        }

        public event EventHandler<EventArgs> WebRequestComplete;

        /*private EventHandler<EventArgs> subWebRequestComplete;
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
        }*/

        /*public void DelayStartWebRequest<T>(UnityWebRequestWrapper unityWebRequestWrapper)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("DelayStartWebRequest delay: {0} {1}",  unityWebRequestWrapper.Pause.ToString(), unityWebRequestWrapper.CurrentRequestType.ToString()), PNLoggingMethod.LevelInfo);
            #endif
            StartWebRequests(unityWebRequestWrapper);

            //WebRequestParams<T> cp = new WebRequestParams<T> (url, timeout, pause, crt, typeof(T), pubnubRequestState);
            //SetWebRequestParams<T> (crt, cp);

            if (unityWebRequestWrapper.CurrentRequestState.RespType == PNOperationType.PNHeartbeatOperation){
                heartbeatPauseTimer = unityWebRequestWrapper.Pause;
                HeartbeatResumeEvent += WebRequestClass_ResumeEvent<T>;
                runHeartbeatPauseTimer = true;
            } else if (unityWebRequestWrapper.CurrentRequestState.RespType == PNOperationType.PNPresenceHeartbeatOperation){
                presenceHeartbeatPauseTimer = unityWebRequestWrapper.Pause;
                PresenceHeartbeatResumeEvent += WebRequestClass_ResumeEvent<T>;
                runPresenceHeartbeatPauseTimer = true;
            } else {
                subscribePauseTimer = unityWebRequestWrapper.Pause;
                SubscribeResumeEvent += WebRequestClass_ResumeEvent<T>;
                runSubscribePauseTimer = true;
            }
        }*/

        string GenerateAndValidateWebRequestId(){
            string newId = Guid.NewGuid ().ToString ();
            if(!currentWebRequests.ContainsKey(newId)){
                Debug.Log("newId:" + newId);
                return newId;
            } else {
                Debug.Log("newId, Duplicate found: " + newId);
                return GenerateAndValidateWebRequestId();
            }
        }

        public void CleanUp(){
            //TODO Abort All
        }

        public void AbortRequest(string webRequestId, bool fireEvent){
            try {
                UnityWebRequestWrapper unityWebRequestWrapper;
                if(currentWebRequests.TryGetValue(webRequestId, out unityWebRequestWrapper)){
                    if(unityWebRequestWrapper!=null){
                        StopTimeoutsAndComplete(unityWebRequestWrapper, webRequestId, false);

                        if((unityWebRequestWrapper.CurrentUnityWebRequest != null) && (!unityWebRequestWrapper.CurrentUnityWebRequest.isDone)){
                            unityWebRequestWrapper.CurrentUnityWebRequest.Abort();
                            unityWebRequestWrapper.CurrentUnityWebRequest.Dispose();
                        }
                        currentWebRequests.Remove(webRequestId);// = unityWebRequestWrapper;
                        unityWebRequestWrapper.CurrentRequestState.ResponseCode = 0;
                        unityWebRequestWrapper.CurrentRequestState.URL =  unityWebRequestWrapper.URL;
                        if(fireEvent){
                            FireEvent ("Aborted", true, false, unityWebRequestWrapper.CurrentRequestState,  unityWebRequestWrapper.CurrentRequestType, webRequestId);
                            #if (ENABLE_PUBNUB_LOGGING)
                            this.PNLog.WriteToLog (string.Format ("BounceRequest: event fired {0}",  unityWebRequestWrapper.CurrentRequestState.ToString ()), PNLoggingMethod.LevelInfo);
                            #endif
                        }
                        #if (ENABLE_PUBNUB_LOGGING) 
                        else {
                            this.PNLog.WriteToLog (string.Format ("BounceRequest: event NOT fired {0}",  unityWebRequestWrapper.CurrentRequestState.ToString ()), PNLoggingMethod.LevelInfo);
                        }
                        #endif
                    }
                }
            } catch (Exception ex) {
                Debug.Log(ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("BounceRequest: Exception: {0}",  ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
            }
        }

        //public string Run (string url, RequestState pubnubRequestState, int timeout, int pause, bool reconnect)
        public string Run (RequestState pubnubRequestState)
        {
            if(!string.IsNullOrEmpty(pubnubRequestState.WebRequestId)){
                AbortRequest (pubnubRequestState.WebRequestId, false);
            }

            bool delayStart = false;
            //for heartbeat and presence heartbeat treat reconnect as pause
            PNCurrentRequestType crt;
            switch (pubnubRequestState.OperationType){
                case PNOperationType.PNHeartbeatOperation:
                crt = PNCurrentRequestType.Heartbeat;
                delayStart = pubnubRequestState.Reconnect;
                break;
                case PNOperationType.PNPresenceHeartbeatOperation:
                crt = PNCurrentRequestType.PresenceHeartbeat;
                delayStart = pubnubRequestState.Reconnect;
                break;
                case PNOperationType.PNSubscribeOperation:
                crt = PNCurrentRequestType.Subscribe;
                if (pubnubRequestState.Pause > 0) {
                    delayStart = true;
                }
                break;
                default:
                crt = PNCurrentRequestType.NonSubscribe;
                break;
            }
           
            //string wwwUrl;
            Debug.Log("pubnubRequestState:"+ pubnubRequestState.OperationType);
            string webRequestId = "";
            UnityWebRequestWrapper webRequestWrapper = new UnityWebRequestWrapper(crt, pubnubRequestState);
            
            lock(syncRoot){
                webRequestId = GenerateAndValidateWebRequestId();
                currentWebRequests.Add(webRequestId, webRequestWrapper);
            }

            //CheckComplete (crt, out wwwUrl);
            Debug.Log ("Run crt"+ crt);
            if (delayStart) {
                webRequestWrapper.PauseTimer = pubnubRequestState.Pause;
                webRequestWrapper.RunPauseTimer = true;
                //ResumeEvent += WebRequestClass_ResumeEvent<T>;
                //DelayStartWebRequest<T>(webRequestWrapper);
            } else {
                StartWebRequests(webRequestWrapper, webRequestId);
            }
            return webRequestId;
            /*if ((pubnubRequestState.RespType == PNOperationType.PNHeartbeatOperation) || (pubnubRequestState.RespType == PNOperationType.PNPresenceHeartbeatOperation)) {
                crt = PNCurrentRequestType.PresenceHeartbeat;
                if (pubnubRequestState.RespType == PNOperationType.PNHeartbeatOperation) {
                    crt = PNCurrentRequestType.Heartbeat;
                }
                

                if (reconnect) {
                    DelayStartWebRequest<T>(webRequestWrapper);
                } else {
                    StartWebRequestsByName<T> (webRequestWrapper);
                }
                Debug.Log("crt:"+ crt);
            } else if (pubnubRequestState.RespType.Equals(PNOperationType.PNSubscribeOperation) || pubnubRequestState.RespType.Equals(PNOperationType.PNPresenceOperation)
            ) {
                crt = PNCurrentRequestType.Subscribe;
                
                CheckComplete (crt, out wwwUrl);

                #if (ENABLE_PUBNUB_LOGGING)
                if ((subscribeRequest != null) && (!subscribeRequest.CurrentUnityWebRequest.isDone)) {
                this.PNLog.WriteToLog (string.Format ("Run: subscribeWww running trying to abort {0}",  crt.ToString ()), PNLoggingMethod.LevelInfo);
                if (subscribeRequest == null) {
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
            } */
        }

        internal void GetOrPostOrDelete(ref UnityWebRequestWrapper unityWebRequestWrapper){
            if(unityWebRequestWrapper.CurrentRequestState.httpMethod.Equals(HTTPMethod.Post)){
                unityWebRequestWrapper.CurrentUnityWebRequest = new UnityWebRequest(unityWebRequestWrapper.URL);
                unityWebRequestWrapper.CurrentUnityWebRequest.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(unityWebRequestWrapper.CurrentRequestState.POSTData));
                //unityWebRequestWrapper.CurrentUnityWebRequest.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes("\"text message\""));
                unityWebRequestWrapper.CurrentUnityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                //UnityWebRequest.Post (unityWebRequestWrapper.URL, unityWebRequestWrapper.CurrentRequestState.POSTData);
                unityWebRequestWrapper.CurrentUnityWebRequest.method = UnityWebRequest.kHttpVerbPOST;
                unityWebRequestWrapper.CurrentUnityWebRequest.SetRequestHeader("Content-Type","application/json"); //x-www-form-urlencoded");///application/json");
                
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("POST Data : {1} \nURL:{0}", unityWebRequestWrapper.URL, unityWebRequestWrapper.CurrentRequestState.POSTData), PNLoggingMethod.LevelInfo);
                #endif
                
            } else if(unityWebRequestWrapper.CurrentRequestState.httpMethod.Equals(HTTPMethod.Delete)) {
                unityWebRequestWrapper.CurrentUnityWebRequest = new UnityWebRequest(unityWebRequestWrapper.URL);
                unityWebRequestWrapper.CurrentUnityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                unityWebRequestWrapper.CurrentUnityWebRequest.method = UnityWebRequest.kHttpVerbDELETE;
                //unityWebRequestWrapper.CurrentUnityWebRequest = UnityWebRequest.Delete (unityWebRequestWrapper.URL);
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("Delete \nURL:{0}", unityWebRequestWrapper.URL), PNLoggingMethod.LevelInfo);
                #endif
            } else {
                unityWebRequestWrapper.CurrentUnityWebRequest = UnityWebRequest.Get (unityWebRequestWrapper.URL);
            } 
        }

        internal void StartWebRequests(UnityWebRequestWrapper unityWebRequestWrapper, string key)
        {
            //WebRequestParams<T> cp = new WebRequestParams<T> (url, timeout, pause, crt, typeof(T), pubnubRequestState);

            //SetWebRequestParams<T> (crt, cp);
            
            
            //StopTimeouts(crt);
            
            Debug.Log ("Run StartWebRequestsByName");
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("StartWebRequestsByName: URL {1} {0}", unityWebRequestWrapper.URL, unityWebRequestWrapper.CurrentRequestType.ToString()), PNLoggingMethod.LevelInfo);
            #endif

            unityWebRequestWrapper.CurrentRequestState.StartRequestTicks = DateTime.UtcNow.Ticks;
            unityWebRequestWrapper.RunTimer = true;
            //CompleteOrTimeoutEvent += WebRequestClass_CompleteEvent<T>;
            unityWebRequestWrapper.IsComplete = false;
            GetOrPostOrDelete(ref unityWebRequestWrapper);

            AsyncOperation async = unityWebRequestWrapper.CurrentUnityWebRequest.Send ();
            currentWebRequests[key] = unityWebRequestWrapper;

            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("StartWebRequestsByName: {0} running",  unityWebRequestWrapper.CurrentRequestType.ToString ()), PNLoggingMethod.LevelInfo);
            this.PNLog.WriteToLog (string.Format ("StartWebRequests POST Data : {1} \nURL : {0}", unityWebRequestWrapper.URL, unityWebRequestWrapper.CurrentRequestState.POSTData), PNLoggingMethod.LevelInfo);
            #endif
            

            /*if (crt == PNCurrentRequestType.Subscribe) {
                subscribeTimer = timeout;
                runSubscribeTimer = true;
                SubCompleteOrTimeoutEvent += WebRequestClass_CompleteEvent<T>;
                isSubscribeComplete = false;
                Debug.Log ("Run Subscribe StartWebRequestsByName");
                //subscribeRequest = new UnityWebRequestWrapper(unityWebRequestWrapper);
                subscribeRequest.CurrentUnityWebRequest =  UnityWebRequest.Get (url);
                
                AsyncOperation async = subscribeRequest.CurrentUnityWebRequest.Send ();
                
               
               
            } else if (crt == PNCurrentRequestType.NonSubscribe) {
                nonSubscribeTimer = timeout;
                runNonSubscribeTimer = true;
                NonsubCompleteOrTimeoutEvent += WebRequestClass_CompleteEvent<T>;
                isNonSubscribeComplete = false;
                Debug.Log ("Run NonSubscribe StartWebRequestsByName");
                nonSubscribeRequest = UnityWebRequest.Get (cp.url);
                Debug.Log ("After NonSubscribe StartWebRequestsByName");
                AsyncOperation async = nonSubscribeRequest.Send ();

                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("StartWebRequestsByName: {0} running",  cp.crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
            } else if (crt == PNCurrentRequestType.PresenceHeartbeat) {
                presenceHeartbeatTimer = timeout;
                runPresenceHeartbeatTimer = true;
                PresenceHeartbeatCompleteOrTimeoutEvent += WebRequestClass_CompleteEvent<T>;
                isPresenceHeartbeatComplete = false;
                Debug.Log ("Run PresenceHeartbeat StartWebRequestsByName");
                presenceHeartbeatRequest = UnityWebRequest.Get  (cp.url);
                AsyncOperation async = presenceHeartbeatRequest.Send ();
                
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
                heartbeatRequest = UnityWebRequest.Get (cp.url);
                AsyncOperation async = heartbeatRequest.Send ();
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("StartWebRequestsByName: {0} running",  cp.crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
            }*/
        }

        /*public IEnumerator DelayRequest<T> (UnityWebRequestWrapper unityWebRequestWrapper)
        {
            yield return new WaitForSeconds (unityWebRequestWrapper.Pause); 
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("DelayRequest timeout  {0}",  unityWebRequestWrapper.CurrentRequestType.ToString()), PNLoggingMethod.LevelInfo);
            #endif

            StartWebRequestsByName<T> (unityWebRequestWrapper);
        }*/

        /*internal void SetComplete (PNCurrentRequestType crt)
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

        }   */         

        /*public bool CheckComplete (PNCurrentRequestType crt, out string wwwUrl)
        {
            try {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckComplete:  {0}", crt.ToString ()), PNLoggingMethod.LevelInfo);
                #endif


                StopTimeouts(crt);

                if (crt == PNCurrentRequestType.Heartbeat) {
                    if ((!isHearbeatComplete) && ((heartbeatRequest != null) && (!heartbeatRequest.isDone))) { 
                        wwwUrl = heartbeatRequest.URL;
                        heartbeatRequest.Abort();
                        heartbeatRequest.Dispose();
                        #if(!UNITY_ANDROID)
                        heartbeatWww.Dispose();
                        #endif
                        heartbeatRequest = null;
                        return false;
                    }
                } else if (crt == PNCurrentRequestType.PresenceHeartbeat) {
                    if ((!isPresenceHeartbeatComplete) && ((presenceHeartbeatRequest != null) && (!presenceHeartbeatRequest.isDone))) {
                        wwwUrl = presenceHeartbeatRequest.URL;
                        #if(!UNITY_ANDROID)
                        presenceHeartbeatRequest.Dispose();
                        #endif
                        
                        presenceHeartbeatRequest = null;
                        
                        return false;
                    }
                } else if (crt == PNCurrentRequestType.Subscribe) {
                    if ((!isSubscribeComplete) && ((subscribeRequest != null) && (!subscribeRequest.isDone))) {
                        wwwUrl = subscribeRequest.URL;
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PNLog.WriteToLog (string.Format ("CheckComplete: DISPOSING WWW"), PNLoggingMethod.LevelError);
                        #endif

                        //TODO: Remove flag when unity bug is fixed. Currenlty calling this on Android hangs the whole app. 
                        //Not fixed for Android as of Unity 5.3.5p4
                        #if(!UNITY_ANDROID)
                        subscribeRequest.Dispose();
                        #endif

                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PNLog.WriteToLog (string.Format ("CheckComplete: WWW disposed"), PNLoggingMethod.LevelError);
                        #endif

                        subscribeRequest = null;
                        return false;
                    }
                } else {
                    if ((!isNonSubscribeComplete) && ((nonSubscribeRequest != null) && (!nonSubscribeRequest.isDone))) {
                        wwwUrl = nonSubscribeRequest.URL;
                        Debug.Log("Aborting");
                        nonSubscribeRequest.Abort();
                        #if(!UNITY_ANDROID)
                        nonSubscribeRequest.Dispose();
                        #endif
                        nonSubscribeRequest = null;
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
        }*/

        public void ProcessResponse (UnityWebRequestWrapper unityWebRequestWrapper, string key)
        {
            try {
                
                Debug.Log("in process response -> " + unityWebRequestWrapper.CurrentRequestState.OperationType);
                //RemoveEventHandler(cp.crt, false);

                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("ProcessResponse: Process Request {0}, url: {1} ",  unityWebRequestWrapper.CurrentRequestType.ToString (), unityWebRequestWrapper.URL), PNLoggingMethod.LevelInfo);
                #endif

                if (unityWebRequestWrapper.CurrentUnityWebRequest != null) {
                    //SetComplete (cp.crt);
                    string message = "";
                    bool isError = false;
                    if(unityWebRequestWrapper.CurrentUnityWebRequest.downloadHandler != null){
                        message = unityWebRequestWrapper.CurrentUnityWebRequest.downloadHandler.text;
                    }

                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PNLog.WriteToLog(string.Format("ProcessResponse: unityWebRequestWrapper.CurrentUnityWebRequest.isNetworkError {0}\n unityWebRequestWrapper.CurrentUnityWebRequest.isHttpError {1}", unityWebRequestWrapper.CurrentUnityWebRequest.isNetworkError, unityWebRequestWrapper.CurrentUnityWebRequest.isHttpError), PNLoggingMethod.LevelInfo);
                    #endif

#if (NETFX_CORE)
#if (ENABLE_PUBNUB_LOGGING)
                    this.PNLog.WriteToLog (string.Format ("ProcessResponse: WWW Sub NETFX_CORE {0}\n Message: {1}\n URL: {2}", unityWebRequestWrapper.CurrentRequestType.ToString (), unityWebRequestWrapper.CurrentUnityWebRequest.error, unityWebRequestWrapper.URL), PNLoggingMethod.LevelInfo);
#endif
                    if(!string.IsNullOrEmpty(unityWebRequestWrapper.CurrentUnityWebRequest.error)){
                        message = string.Format ("{0}\"Error\": \"{1}\", \"Description\": {2}{3}", "{", unityWebRequestWrapper.CurrentUnityWebRequest.error, message, "}");
                        isError = true;
                    }
#else
                    if ((!unityWebRequestWrapper.CurrentUnityWebRequest.isNetworkError) 
                        && (!unityWebRequestWrapper.CurrentUnityWebRequest.isHttpError))
                    {
#if (ENABLE_PUBNUB_LOGGING)
                        this.PNLog.WriteToLog (string.Format ("ProcessResponse: WWW Sub {0}\n Message: {1}\n URL: {2}", unityWebRequestWrapper.CurrentRequestType.ToString (), unityWebRequestWrapper.CurrentUnityWebRequest.error, unityWebRequestWrapper.URL), PNLoggingMethod.LevelInfo);
#endif
                        isError = false;
                        
                    } else {
#if (ENABLE_PUBNUB_LOGGING)
                        this.PNLog.WriteToLog (string.Format ("ProcessResponse: WWW Sub {0}\n Error: {1},\n text: {2}\n URL: {3}", unityWebRequestWrapper.CurrentRequestType.ToString (), unityWebRequestWrapper.CurrentUnityWebRequest.error, message, unityWebRequestWrapper.URL), PNLoggingMethod.LevelInfo);
#endif
                        message = string.Format ("{0}\"Error\": \"{1}\", \"Description\": {2}{3}", "{", unityWebRequestWrapper.CurrentUnityWebRequest.error, message, "}");
                        isError = true;
                    }
#endif

                    if((!unityWebRequestWrapper.CurrentUnityWebRequest.isNetworkError) 
                        && (!unityWebRequestWrapper.CurrentUnityWebRequest.isHttpError))
                    {
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PNLog.WriteToLog (string.Format ("ProcessResponse: WWW Sub {0}\n Message: {1}\n URL: {2}", unityWebRequestWrapper.CurrentRequestType.ToString (), unityWebRequestWrapper.CurrentUnityWebRequest.error, unityWebRequestWrapper.URL), PNLoggingMethod.LevelInfo);
                        #endif
                        isError = false;
                        
                    } else {
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PNLog.WriteToLog (string.Format ("ProcessResponse: WWW Sub {0}\n Error: {1},\n text: {2}\n URL: {3}", unityWebRequestWrapper.CurrentRequestType.ToString (), unityWebRequestWrapper.CurrentUnityWebRequest.error, message, unityWebRequestWrapper.URL), PNLoggingMethod.LevelInfo);
                        #endif
                        message = string.Format ("{0}\"Error\": \"{1}\", \"Description\": {2}{3}", "{", unityWebRequestWrapper.CurrentUnityWebRequest.error, message, "}");
                        isError = true;
                    } 
                    Debug.Log("message:" + message);
                    
                    if (unityWebRequestWrapper.CurrentRequestState != null) {
                        unityWebRequestWrapper.CurrentRequestState.EndRequestTicks = DateTime.UtcNow.Ticks;
                        unityWebRequestWrapper.CurrentRequestState.ResponseCode = unityWebRequestWrapper.CurrentUnityWebRequest.responseCode;
                        unityWebRequestWrapper.CurrentRequestState.URL = unityWebRequestWrapper.CurrentUnityWebRequest.url;                    

                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PNLog.WriteToLog (string.Format ("ProcessResponse: WWW Sub request2 {0} \n{1} \nresponseCode: {2}", unityWebRequestWrapper.CurrentRequestState.OperationType, unityWebRequestWrapper.CurrentRequestType, unityWebRequestWrapper.CurrentRequestState.ResponseCode), PNLoggingMethod.LevelInfo);
                        #endif
                    } 
                    #if (ENABLE_PUBNUB_LOGGING)
                    else {
                        this.PNLog.WriteToLog (string.Format ("ProcessResponse: WWW Sub request null2"), PNLoggingMethod.LevelInfo);                        
                    }
                    #endif
                    Debug.Log("BEFORE FireEvent");
                    FireEvent (message, isError, false, unityWebRequestWrapper.CurrentRequestState, unityWebRequestWrapper.CurrentRequestType, key);
                } 
            } catch (Exception ex) {
                Debug.Log(ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("ProcessResponse: RunWebRequestSub {0}, Exception: {1}",  unityWebRequestWrapper.CurrentRequestType.ToString (), ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
            }
        }

        /*public void AbortRequest (PNCurrentRequestType crt, RequestState pubnubRequestState, bool fireEvent)
        {
            try {
                string wwwUrl;
                CheckComplete (crt, out wwwUrl);
                SetComplete (crt);                

                if ((pubnubRequestState != null) && fireEvent) {
                    pubnubRequestState.ResponseCode = 0;
                    pubnubRequestState.URL = wwwUrl;
                    FireEvent ("Aborted", true, false, pubnubRequestState, crt);
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
        }*/

        public void ProcessTimeout (UnityWebRequestWrapper unityWebRequestWrapper, string key, bool isComplete)
        {
            try {
                //RemoveEventHandler<T>(cp.crt, false);

                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("ProcessTimeout: {0}",  unityWebRequestWrapper.CurrentRequestType.ToString ()), PNLoggingMethod.LevelInfo);
                #endif

                //if (!CheckComplete (cp.crt, out wwwUrl)) {
                    //if ((cp.typeParameterType == typeof(string)) || (cp.typeParameterType == typeof(object))) {
                    if(!isComplete){
                        AbortRequest(key, false);
                    }
                    unityWebRequestWrapper.CurrentRequestState.ResponseCode = 0;
                    unityWebRequestWrapper.CurrentRequestState.URL = unityWebRequestWrapper.URL;
                    FireEvent ("Timed out", true, true, unityWebRequestWrapper.CurrentRequestState, unityWebRequestWrapper.CurrentRequestType, key);
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PNLog.WriteToLog (string.Format ("ProcessTimeout: WWW Error: {0} sec timeout",  unityWebRequestWrapper.Timeout.ToString ()), PNLoggingMethod.LevelInfo);
                    #endif
                    /*} else {
                        throw new Exception ("'string' and 'object' are the only types supported in generic method calls");
                    }*/
                //}

            } catch (Exception ex) {
                Debug.Log(ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("ProcessTimeout: {0} {1}",  ex.ToString (), unityWebRequestWrapper.CurrentRequestType.ToString ()), PNLoggingMethod.LevelError);
                #endif
            }
        }

        public void FireEvent (string message, bool isError, bool isTimeout, RequestState pubnubRequestState, PNCurrentRequestType crt, string key)
        {
            Debug.Log(" FireEvent" + crt);
            CustomEventArgs cea = new CustomEventArgs ();
            cea.PubNubRequestState = pubnubRequestState;
            cea.Message = message;
            cea.IsError = isError;
            cea.IsTimeout = isTimeout;
            cea.CurrRequestType = crt;
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("FireEvent: Raising Event of type : {0}",  crt.ToString ()), PNLoggingMethod.LevelInfo);
            #endif

            WebRequestComplete.Raise (this, cea);
            currentWebRequests.Remove(key);

            /*if ((crt == PNCurrentRequestType.Heartbeat) && (heartbeatWebRequestComplete != null)) {
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
            #endif*/
        }
    }

    /*#region WebRequestClass
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
    #endregion*/


    /*#region "PubnubWebResponse and PubnubWebRequest"
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
    #endregion*/
}