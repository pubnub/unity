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

        private readonly object syncRoot = new System.Object();

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

        public const float timerConst = 0; 
        public float timer = timerConst; 

        SafeDictionary<string, UnityWebRequestWrapper> currentWebRequests = new SafeDictionary<string, UnityWebRequestWrapper> ();

        public event EventHandler<EventArgs> CompleteOrTimeoutEvent; 
        internal CurrentRequestTypeEventArgs CreateCurrentRequestTypeEventArgs(UnityWebRequestWrapper unityWebRequestWrapper, bool isTimeout){
            CurrentRequestTypeEventArgs crtEa = new CurrentRequestTypeEventArgs();
            crtEa.WebRequestWrapper = unityWebRequestWrapper;
            crtEa.IsTimeout = isTimeout;
            return crtEa;
        }

        internal void RaiseEvents(bool isTimeout, UnityWebRequestWrapper unityWebRequestWrapper, string key)
        {
            StopTimeoutsAndComplete (unityWebRequestWrapper, key, false);
            
            if (isTimeout) {
                ProcessTimeout (unityWebRequestWrapper, key, unityWebRequestWrapper.IsComplete);
            } else {
                ProcessResponse (unityWebRequestWrapper, key);
            }
        }

        internal void CheckElapsedTime(UnityWebRequestWrapper unityWebRequestWrapper, string key)
        {
            if (unityWebRequestWrapper.Timer <= 0) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: timeout {0}",  unityWebRequestWrapper.CurrentRequestType.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
                RaiseEvents (true, unityWebRequestWrapper, key);
            } else if ((unityWebRequestWrapper != null) && ((unityWebRequestWrapper.CurrentUnityWebRequest != null) && (unityWebRequestWrapper.CurrentUnityWebRequest.isDone))) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: done {0}",  unityWebRequestWrapper.CurrentRequestType.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
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

            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("CheckElapsedTime: timer {0}",  unityWebRequestWrapper.Timer.ToString ()), PNLoggingMethod.LevelInfo);
                #endif
            }
        }

        internal void CheckPauseTime(UnityWebRequestWrapper unityWebRequestWrapper, string key)
        {
            if (unityWebRequestWrapper.PauseTimer <= 0) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("pause timeout {0} {1}",  unityWebRequestWrapper.CurrentRequestType.ToString(), unityWebRequestWrapper.PauseTimer.ToString()), PNLoggingMethod.LevelInfo);
                #endif

                StopTimeoutsAndComplete (unityWebRequestWrapper, key, true);
                
                StartWebRequests(unityWebRequestWrapper, key);
            } 
        }

        void Start(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("REDUCE_PUBNUB_COROUTINES is set."), PNLoggingMethod.LevelInfo);
            #endif
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

        }

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

        }

        public event EventHandler<EventArgs> WebRequestComplete;

        string GenerateAndValidateWebRequestId(){
            string newId = Guid.NewGuid ().ToString ();
            if(!currentWebRequests.ContainsKey(newId)){
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("newId: {0}",  newId), PNLoggingMethod.LevelInfo);
                #endif  

                return newId;
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("newId, Duplicate found: {0}",  newId), PNLoggingMethod.LevelInfo);
                #endif  
                
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
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("BounceRequest: Exception: {0}",  ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
            }
        }

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
           
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("pubnubRequestState: {0}", pubnubRequestState.OperationType), PNLoggingMethod.LevelInfo);
            #endif  
            
            string webRequestId = "";
            UnityWebRequestWrapper webRequestWrapper = new UnityWebRequestWrapper(crt, pubnubRequestState);
            
            lock(syncRoot){
                webRequestId = GenerateAndValidateWebRequestId();
                currentWebRequests.Add(webRequestId, webRequestWrapper);
            }

            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("Run crt: {0}", crt), PNLoggingMethod.LevelInfo);
            #endif  
            
            if (delayStart) {
                webRequestWrapper.PauseTimer = pubnubRequestState.Pause;
                webRequestWrapper.RunPauseTimer = true;
            } else {
                StartWebRequests(webRequestWrapper, webRequestId);
            }
            return webRequestId;

        }

        internal void GetOrPostOrDelete(ref UnityWebRequestWrapper unityWebRequestWrapper){
            if(unityWebRequestWrapper.CurrentRequestState.httpMethod.Equals(HTTPMethod.Post)){
                unityWebRequestWrapper.CurrentUnityWebRequest = new UnityWebRequest(unityWebRequestWrapper.URL);
                unityWebRequestWrapper.CurrentUnityWebRequest.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(unityWebRequestWrapper.CurrentRequestState.POSTData));
                unityWebRequestWrapper.CurrentUnityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                unityWebRequestWrapper.CurrentUnityWebRequest.method = UnityWebRequest.kHttpVerbPOST;
                unityWebRequestWrapper.CurrentUnityWebRequest.SetRequestHeader("Content-Type","application/json"); //x-www-form-urlencoded");///application/json");
                
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("POST Data : {1} \nURL:{0}", unityWebRequestWrapper.URL, unityWebRequestWrapper.CurrentRequestState.POSTData), PNLoggingMethod.LevelInfo);
                #endif
                
            } else if(unityWebRequestWrapper.CurrentRequestState.httpMethod.Equals(HTTPMethod.Delete)) {
                unityWebRequestWrapper.CurrentUnityWebRequest = new UnityWebRequest(unityWebRequestWrapper.URL);
                unityWebRequestWrapper.CurrentUnityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                unityWebRequestWrapper.CurrentUnityWebRequest.method = UnityWebRequest.kHttpVerbDELETE;
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("Delete \nURL:{0}", unityWebRequestWrapper.URL), PNLoggingMethod.LevelInfo);
                #endif
            } else {
                unityWebRequestWrapper.CurrentUnityWebRequest = UnityWebRequest.Get (unityWebRequestWrapper.URL);
            } 
        }

        internal void StartWebRequests(UnityWebRequestWrapper unityWebRequestWrapper, string key)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("StartWebRequestsByName: URL {1} {0}", unityWebRequestWrapper.URL, unityWebRequestWrapper.CurrentRequestType.ToString()), PNLoggingMethod.LevelInfo);
            #endif

            unityWebRequestWrapper.CurrentRequestState.StartRequestTicks = DateTime.UtcNow.Ticks;
            unityWebRequestWrapper.RunTimer = true;
            unityWebRequestWrapper.IsComplete = false;
            GetOrPostOrDelete(ref unityWebRequestWrapper);

            UnityWebRequestAsyncOperation asyncOp = unityWebRequestWrapper.CurrentUnityWebRequest.SendWebRequest();
            currentWebRequests[key] = unityWebRequestWrapper;

            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("StartWebRequestsByName: {0} running",  unityWebRequestWrapper.CurrentRequestType.ToString ()), PNLoggingMethod.LevelInfo);
            this.PNLog.WriteToLog (string.Format ("StartWebRequests POST Data : {1} \nURL : {0}", unityWebRequestWrapper.URL, unityWebRequestWrapper.CurrentRequestState.POSTData), PNLoggingMethod.LevelInfo);
            #endif
        }

        public void ProcessResponse (UnityWebRequestWrapper unityWebRequestWrapper, string key)
        {
            try {
                
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("ProcessResponse: Process Request {0}, url: {1}, OPType: {2}",  unityWebRequestWrapper.CurrentRequestType.ToString (), unityWebRequestWrapper.URL, unityWebRequestWrapper.CurrentRequestState.OperationType), PNLoggingMethod.LevelInfo);
                #endif

                if (unityWebRequestWrapper.CurrentUnityWebRequest != null) {
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
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PNLog.WriteToLog (string.Format ("message: {0}", message), PNLoggingMethod.LevelInfo);
                    #endif

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
                    this.PNLog.WriteToLog ("BEFORE FireEvent", PNLoggingMethod.LevelInfo);                        
                    #endif
                    FireEvent (message, isError, false, unityWebRequestWrapper.CurrentRequestState, unityWebRequestWrapper.CurrentRequestType, key);
                } 
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("ProcessResponse: RunWebRequestSub {0}, Exception: {1}",  unityWebRequestWrapper.CurrentRequestType.ToString (), ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
            }
        }

        public void ProcessTimeout (UnityWebRequestWrapper unityWebRequestWrapper, string key, bool isComplete)
        {
            try {

                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("ProcessTimeout: {0}",  unityWebRequestWrapper.CurrentRequestType.ToString ()), PNLoggingMethod.LevelInfo);
                #endif

                if(!isComplete){
                    AbortRequest(key, false);
                }
                unityWebRequestWrapper.CurrentRequestState.ResponseCode = 0;
                unityWebRequestWrapper.CurrentRequestState.URL = unityWebRequestWrapper.URL;
                FireEvent ("Timed out", true, true, unityWebRequestWrapper.CurrentRequestState, unityWebRequestWrapper.CurrentRequestType, key);
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("ProcessTimeout: WWW Error: {0} sec timeout",  unityWebRequestWrapper.Timeout.ToString ()), PNLoggingMethod.LevelInfo);
                #endif

            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PNLog.WriteToLog (string.Format ("ProcessTimeout: {0} {1}",  ex.ToString (), unityWebRequestWrapper.CurrentRequestType.ToString ()), PNLoggingMethod.LevelError);
                #endif
            }
        }

        public void FireEvent (string message, bool isError, bool isTimeout, RequestState pubnubRequestState, PNCurrentRequestType crt, string key)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PNLog.WriteToLog (string.Format ("FireEvent {0}", crt), PNLoggingMethod.LevelError);
            #endif
            
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

        }
    }

}