using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace PubNubMessaging.Core
{
    #region EventExt and Args
    static class EventExtensions
    {
        public static void Raise<T> (this EventHandler<T> handler, object sender, T args)
            where T : EventArgs
        {
            if (handler != null) {
                handler (sender, args);
            }
        }
    }

    internal class CustomEventArgs<T> : EventArgs
    {
        internal string Message;
        internal RequestState<T> PubnubRequestState;
        internal bool IsError;
        internal bool IsTimeout;
        internal CurrentRequestType CurrRequestType;
    }
    #endregion

    #region CoroutineClass
    class CoroutineParams<T>
    {
        public string url;
        public int timeout;
        public int pause;
        public CurrentRequestType crt;
        public Type typeParameterType;
        public RequestState<T> requestState;

        public CoroutineParams (string url, int timeout, int pause, CurrentRequestType crt, Type typeParameterType, RequestState<T> requestState)
        {
            this.url = url;
            this.timeout = timeout;
            this.pause = pause;
            this.crt = crt;
            this.typeParameterType = typeParameterType;
            this.requestState = requestState;
        }
    }

    
    public enum CurrentRequestType
    {
        Heartbeat,
        PresenceHeartbeat,
        Subscribe,
        NonSubscribe
    }

    //Sending a IEnumerator from a complex object in StartCoroutine doesn't work for Web/WebGL
    //Dispose of www leads to random unhandled exceptions.
    //Generic methods dont work in StartCoroutine when the called with the string param name StartCoroutine("method", param)
    //StopCoroutine only works when the coroutine is started with string overload.
    class CoroutineClass : MonoBehaviour
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

        private bool isHearbeatComplete = false;
        private bool isPresenceHeartbeatComplete = false;
        private bool isSubscribeComplete = false;
        private bool isNonSubscribeComplete = false;

        private IEnumerator SubCoroutine;
        private IEnumerator SubTimeoutCoroutine;
        private IEnumerator NonSubCoroutine;
        private IEnumerator NonSubTimeoutCoroutine;
        private IEnumerator PresenceHeartbeatCoroutine;
        private IEnumerator PresenceHeartbeatTimeoutCoroutine;
        private IEnumerator HeartbeatCoroutine;
        private IEnumerator HeartbeatTimeoutCoroutine;
        private IEnumerator DelayRequestCoroutineHB;
        private IEnumerator DelayRequestCoroutinePHB;

        WWW subscribeWww;
        WWW heartbeatWww;
        WWW presenceHeartbeatWww;
        WWW nonSubscribeWww;

        private EventHandler<EventArgs> subCoroutineComplete;
        //Register single event handler
        public event EventHandler<EventArgs> SubCoroutineComplete {
            add {
                if (subCoroutineComplete == null || !subCoroutineComplete.GetInvocationList ().Contains (value)) {
                    subCoroutineComplete += value;
                }
            }
            remove {
                subCoroutineComplete -= value;
            }
        }

        private EventHandler<EventArgs> nonSubCoroutineComplete;
        //Register single event handler
        public event EventHandler<EventArgs> NonSubCoroutineComplete {
            add {
                if (nonSubCoroutineComplete == null || !nonSubCoroutineComplete.GetInvocationList ().Contains (value)) {
                    nonSubCoroutineComplete += value;
                }
            }
            remove {
                nonSubCoroutineComplete -= value;
            }
        }

        private EventHandler<EventArgs> presenceHeartbeatCoroutineComplete;
        //Register single event handler
        public event EventHandler<EventArgs> PresenceHeartbeatCoroutineComplete {
            add {
                if (presenceHeartbeatCoroutineComplete == null || !presenceHeartbeatCoroutineComplete.GetInvocationList ().Contains (value)) {
                    presenceHeartbeatCoroutineComplete += value;
                }
            }
            remove {
                presenceHeartbeatCoroutineComplete -= value;
            }
        }

        private EventHandler<EventArgs> heartbeatCoroutineComplete;
        //Register single event handler
        public event EventHandler<EventArgs> HeartbeatCoroutineComplete {
            add {
                if (heartbeatCoroutineComplete == null || !heartbeatCoroutineComplete.GetInvocationList ().Contains (value)) {
                    heartbeatCoroutineComplete += value;
                }
            }
            remove {
                heartbeatCoroutineComplete -= value;
            }
        }

        public void DelayStartCoroutine<T>(string url, RequestState<T> pubnubRequestState, int timeout, int pause, CurrentRequestType crt)
        {
            if (pubnubRequestState.Type == ResponseType.Heartbeat)
            {
                DelayRequestCoroutineHB = DelayRequest<T>(url, pubnubRequestState, timeout, pause, crt);
                StartCoroutine(DelayRequestCoroutineHB);
            }
            else
            {
                DelayRequestCoroutinePHB = DelayRequest<T>(url, pubnubRequestState, timeout, pause, crt);
                StartCoroutine(DelayRequestCoroutinePHB);
            }
        }

        public void Run<T> (string url, RequestState<T> pubnubRequestState, int timeout, int pause)
        {
            //for heartbeat and presence heartbeat treat reconnect as pause
            CurrentRequestType crt;
            if ((pubnubRequestState.Type == ResponseType.Heartbeat) || (pubnubRequestState.Type == ResponseType.PresenceHeartbeat)) {
                crt = CurrentRequestType.PresenceHeartbeat;
                if (pubnubRequestState.Type == ResponseType.Heartbeat) {
                    crt = CurrentRequestType.Heartbeat;
                }
                CheckComplete (crt);

                if (pubnubRequestState.Reconnect) {
                    DelayStartCoroutine<T>(url, pubnubRequestState, timeout, pause, crt);
                } else {
                    StartCoroutinesByName<T> (url, pubnubRequestState, timeout, pause, crt);
                }
            } else if ((pubnubRequestState.Type == ResponseType.Subscribe) || (pubnubRequestState.Type == ResponseType.Presence)) {
                crt = CurrentRequestType.Subscribe;
                #if (ENABLE_PUBNUB_LOGGING)
                if ((subscribeWww != null) && (!subscribeWww.isDone)) {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, subscribeWww running trying to abort {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
                    if (subscribeWww == null) {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, subscribeWww aborted {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
                    }
                }
                #endif
                StartCoroutinesByName<T> (url, pubnubRequestState, timeout, pause, crt);
            } else {
                crt = CurrentRequestType.NonSubscribe;
                CheckComplete (crt);
                StartCoroutinesByName<T> (url, pubnubRequestState, timeout, pause, crt);
            } 
        }

        private void StartCoroutinesByName<T> (string url, RequestState<T> pubnubRequestState, int timeout, int pause, CurrentRequestType crt)
        {
            CoroutineParams<T> cp = new CoroutineParams<T> (url, timeout, pause, crt, typeof(T), pubnubRequestState);

            if (crt == CurrentRequestType.Subscribe) {
                if((SubTimeoutCoroutine != null) && (!isSubscribeComplete)){
                    StopCoroutine (SubTimeoutCoroutine);
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Stopped existing timeout coroutine {1}", DateTime.Now.ToString (), cp.crt.ToString ()), LoggingMethod.LevelInfo);
                    #endif
                }
                    
                SubTimeoutCoroutine = CheckTimeoutSub<T> (cp);
                SubCoroutine = SendRequestSub<T> (cp);
                StartCoroutine (SubTimeoutCoroutine);
                StartCoroutine (SubCoroutine);
            } else if (crt == CurrentRequestType.NonSubscribe) {
                if((NonSubTimeoutCoroutine != null) && (!isNonSubscribeComplete)){
                    StopCoroutine (NonSubTimeoutCoroutine);
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Stopped existing timeout coroutine {1}", DateTime.Now.ToString (), cp.crt.ToString ()), LoggingMethod.LevelInfo);
                    #endif
                }

                NonSubTimeoutCoroutine = CheckTimeoutNonSub<T> (cp);
                NonSubCoroutine = SendRequestNonSub<T> (cp);
                StartCoroutine (NonSubTimeoutCoroutine);
                StartCoroutine (NonSubCoroutine);

            } else if (crt == CurrentRequestType.PresenceHeartbeat) {
                if((PresenceHeartbeatTimeoutCoroutine != null) && (!isPresenceHeartbeatComplete)){
                    StopCoroutine (PresenceHeartbeatTimeoutCoroutine);
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Stopped existing timeout coroutine {1}", DateTime.Now.ToString (), cp.crt.ToString ()), LoggingMethod.LevelInfo);
                    #endif
                }

                PresenceHeartbeatTimeoutCoroutine = CheckTimeoutPresenceHeartbeat<T> (cp);
                PresenceHeartbeatCoroutine = SendRequestPresenceHeartbeat<T> (cp);
                StartCoroutine (PresenceHeartbeatTimeoutCoroutine);
                StartCoroutine (PresenceHeartbeatCoroutine);

            } else if (crt == CurrentRequestType.Heartbeat) {
                if((HeartbeatTimeoutCoroutine != null) && (!isHearbeatComplete)){
                    StopCoroutine (HeartbeatTimeoutCoroutine);
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Stopped existing timeout coroutine {1}", DateTime.Now.ToString (), cp.crt.ToString ()), LoggingMethod.LevelInfo);
                    #endif
                }

                HeartbeatTimeoutCoroutine = CheckTimeoutHeartbeat<T> (cp);
                HeartbeatCoroutine = SendRequestHeartbeat<T> (cp);
                StartCoroutine (HeartbeatTimeoutCoroutine);
                StartCoroutine (HeartbeatCoroutine);
            }
        }

        public IEnumerator DelayRequest<T> (string url, RequestState<T> pubnubRequestState, int timeout, int pause, CurrentRequestType crt)
        {
            yield return new WaitForSeconds (pause); 
            StartCoroutinesByName<T> (url, pubnubRequestState, timeout, pause, crt);
        }

        public void ProcessResponse<T> (WWW www, CoroutineParams<T> cp)
        {
            try {
                if (www != null) {
                    SetComplete (cp.crt);
                    string message = "";
                    bool isError = false;

                    if (string.IsNullOrEmpty (www.error)) {
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WWW Sub {1} Message: {2}", DateTime.Now.ToString (), cp.crt.ToString (), www.text), LoggingMethod.LevelInfo);
                        #endif
                        message = www.text;
                        isError = false;
                    } else {
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WWW Sub {1} Error: {2}", DateTime.Now.ToString (), cp.crt.ToString (), www.error), LoggingMethod.LevelInfo);
                        #endif
                        message = www.error;
                        isError = true;
                    } 

                    #if (ENABLE_PUBNUB_LOGGING)
                    if (cp.requestState == null) {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WWW Sub request null2", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                    } else {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WWW Sub request2 {1} {2}", DateTime.Now.ToString (), cp.requestState.Type, cp.crt), LoggingMethod.LevelInfo);
                    }
                    #endif

                    FireEvent (message, isError, false, cp.requestState, cp.crt);
                } 
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, RunCoroutineSub {1}, Exception: {2}", DateTime.Now.ToString (), cp.crt.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
            }
        }

        public IEnumerator SendRequestSub<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, URL Sub {1} ", DateTime.Now.ToString (), cp.url.ToString ()), LoggingMethod.LevelInfo);
            #endif
            WWW www;

            isSubscribeComplete = false;

            subscribeWww = new WWW (cp.url);
            yield return subscribeWww;
            if ((subscribeWww != null) && (subscribeWww.isDone)) {
                www = subscribeWww;
            } else {
                www = null;
            }
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0},After www type  {1}", DateTime.Now.ToString (), typeof(T)), LoggingMethod.LevelError);
            #endif
            ProcessResponse<T> (www, cp);
        }

        public IEnumerator SendRequestNonSub<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, URL NonSub {1} ", DateTime.Now.ToString (), cp.url.ToString ()), LoggingMethod.LevelInfo);
            #endif
            WWW www;

            isNonSubscribeComplete = false;
            nonSubscribeWww = new WWW (cp.url);
            yield return nonSubscribeWww;
            if ((nonSubscribeWww != null) && (nonSubscribeWww.isDone)) {
                www = nonSubscribeWww;
            } else {
                www = null;
            }
             
            ProcessResponse (www, cp);
        }

        public IEnumerator SendRequestPresenceHeartbeat<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, URL PresenceHB {1} ", DateTime.Now.ToString (), cp.url.ToString ()), LoggingMethod.LevelInfo);
            #endif
            WWW www;

            isPresenceHeartbeatComplete = false;
            presenceHeartbeatWww = new WWW (cp.url);
            yield return presenceHeartbeatWww;
            if ((presenceHeartbeatWww != null) && (presenceHeartbeatWww.isDone)) {
                www = presenceHeartbeatWww;
            } else {
                www = null;
            }

            ProcessResponse (www, cp);
        }

        public IEnumerator SendRequestHeartbeat<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, URL Heartbeat {1} ", DateTime.Now.ToString (), cp.url.ToString ()), LoggingMethod.LevelInfo);
            #endif
            WWW www;

            isHearbeatComplete = false;
            heartbeatWww = new WWW (cp.url);
            yield return heartbeatWww;
            if ((heartbeatWww != null) && (heartbeatWww.isDone)) {
                www = heartbeatWww;
            } else {
                www = null;
            }

            ProcessResponse (www, cp);
        }

        public void CallFireEvent<T> (string message, bool isError, bool isTimeout, RequestState<T> pubnubRequestState, CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CallFireEvent RequestType {1} {2} {3}", DateTime.Now.ToString (), typeof(T), pubnubRequestState.GetType (), pubnubRequestState.Channels), LoggingMethod.LevelInfo);
            #endif
            FireEvent (message, isError, false, pubnubRequestState, cp.crt);
        }

        void SetComplete (CurrentRequestType crt)
        {
            try {
                if (crt == CurrentRequestType.Heartbeat) {
                    StopCoroutine (HeartbeatTimeoutCoroutine);
                    isHearbeatComplete = true;
                } else if (crt == CurrentRequestType.PresenceHeartbeat) {
                    StopCoroutine (PresenceHeartbeatTimeoutCoroutine);
                    isPresenceHeartbeatComplete = true;
                } else if (crt == CurrentRequestType.Subscribe) {
                    StopCoroutine (SubTimeoutCoroutine);
                    isSubscribeComplete = true;
                } else {
                    StopCoroutine (NonSubTimeoutCoroutine);
                    isNonSubscribeComplete = true;
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, SetComplete Complete {1}", DateTime.Now.ToString (), crt.ToString()), LoggingMethod.LevelInfo);
                #endif
            } catch (Exception ex) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, SetComplete Exception: ", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
            }

        }

        public bool CheckComplete (CurrentRequestType crt)
        {
            try {
                if (crt == CurrentRequestType.Heartbeat) {
                    if ((!isHearbeatComplete) && (heartbeatWww != null) && (!heartbeatWww.isDone)) {    
                        StopCoroutine (HeartbeatCoroutine);
                        return false;
                    }
                } else if (crt == CurrentRequestType.PresenceHeartbeat) {
                    if ((!isPresenceHeartbeatComplete) && (presenceHeartbeatWww != null) && (!presenceHeartbeatWww.isDone)) {
                        StopCoroutine (PresenceHeartbeatCoroutine);
                        return false;
                    }
                } else if (crt == CurrentRequestType.Subscribe) {
                    if ((!isSubscribeComplete) && (subscribeWww != null) && (!subscribeWww.isDone)) {
                        StopCoroutine (SubCoroutine);
                        return false;
                    }
                } else {
                    if ((!isNonSubscribeComplete) && (nonSubscribeWww != null) && (!nonSubscribeWww.isDone)) {
                        StopCoroutine (NonSubCoroutine);
                        return false;
                    }
                } 

            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, GetCompleteAndDispose Exception: ", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
            }

            return true;
        }

        void StopRunningCoroutines(CurrentRequestType crt)
        {
            if (crt == CurrentRequestType.Heartbeat)
            {
                if ((heartbeatWww != null) && (!heartbeatWww.isDone))
                {
                    heartbeatWww.Dispose();
                    heartbeatWww = null;
                    StopCoroutine(HeartbeatCoroutine);
                }
                if (DelayRequestCoroutineHB != null)
                {
                    StopCoroutine(DelayRequestCoroutineHB);
                }
            }
            else if (crt == CurrentRequestType.PresenceHeartbeat)
            {
                if ((presenceHeartbeatWww != null) && (!presenceHeartbeatWww.isDone))
                {
                    presenceHeartbeatWww.Dispose();
                    presenceHeartbeatWww = null;
                    StopCoroutine(PresenceHeartbeatCoroutine);
                }
                if (DelayRequestCoroutinePHB != null)
                {
                    StopCoroutine(DelayRequestCoroutinePHB);
                }
            }
            else if ((crt == CurrentRequestType.Subscribe) && (subscribeWww != null) && (!subscribeWww.isDone))
            {
                subscribeWww = null;
                StopCoroutine(SubCoroutine);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, Coroutine stopped Subscribe: ", DateTime.Now.ToString()), LoggingMethod.LevelInfo);
                #endif
            }
            else if ((crt == CurrentRequestType.NonSubscribe) && (nonSubscribeWww != null) && (!nonSubscribeWww.isDone))
            {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, Dispose nonSubscribeWww: ", DateTime.Now.ToString()), LoggingMethod.LevelInfo);
                #endif
                nonSubscribeWww.Dispose();
                nonSubscribeWww = null;
                StopCoroutine(NonSubCoroutine);
            }
        }

        public void BounceRequest<T> (CurrentRequestType crt, RequestState<T> pubnubRequestState, bool fireEvent)
        {
            try {
                StopRunningCoroutines(crt);

                SetComplete (crt);
                
                if ((pubnubRequestState != null) && (fireEvent)) {
                    FireEvent ("Aborted", true, false, pubnubRequestState, crt);
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, event fired {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
                    #endif
                }
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, BounceRequest Exception: {1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
            }
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, BounceRequest {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
            #endif
        }

        public void ProcessTimeout<T> (CoroutineParams<T> cp)
        {
            try {
                if (!CheckComplete (cp.crt)) {
                    if (cp.typeParameterType == typeof(string)) {
                        FireEvent ("Timed out", true, true, cp.requestState, cp.crt);
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WWW Error: {1} sec timeout", DateTime.Now.ToString (), cp.timeout.ToString ()), LoggingMethod.LevelInfo);
                        #endif
                    } else if (cp.typeParameterType == typeof(object)) { 
                        FireEvent ("Timed out", true, true, cp.requestState, cp.crt);
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WWW Error: {1} sec timeout", DateTime.Now.ToString (), cp.timeout.ToString ()), LoggingMethod.LevelInfo);
                        #endif
                    } else {
                        throw new Exception ("'string' and 'object' are the only types supported in generic method calls");
                    }
                }
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CheckTimeout: {1} {2}", DateTime.Now.ToString (), ex.ToString (), cp.crt.ToString ()), LoggingMethod.LevelError);
                #endif
            }
        }

        public IEnumerator CheckTimeoutSub<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, yielding: {1} sec timeout", DateTime.Now.ToString (), cp.timeout.ToString ()), LoggingMethod.LevelInfo);
            #endif
            yield return new WaitForSeconds (cp.timeout); 
            ProcessTimeout<T> (cp);
        }

        public IEnumerator CheckTimeoutNonSub<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, yielding: {1} sec timeout", DateTime.Now.ToString (), cp.timeout.ToString ()), LoggingMethod.LevelInfo);
            #endif
            yield return new WaitForSeconds (cp.timeout); 
            ProcessTimeout<T> (cp);
        }

        public IEnumerator CheckTimeoutPresenceHeartbeat<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, yielding: {1} sec timeout", DateTime.Now.ToString (), cp.timeout.ToString ()), LoggingMethod.LevelInfo);
            #endif
            yield return new WaitForSeconds (cp.timeout); 
            ProcessTimeout<T> (cp);
        }

        public IEnumerator CheckTimeoutHeartbeat<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, yielding: {1} sec timeout", DateTime.Now.ToString (), cp.timeout.ToString ()), LoggingMethod.LevelInfo);
            #endif
            yield return new WaitForSeconds (cp.timeout); 
            ProcessTimeout<T> (cp);
        }

        public void FireEvent<T> (string message, bool isError, bool isTimeout, RequestState<T> pubnubRequestState, CurrentRequestType crt)
        {
            CustomEventArgs<T> cea = new CustomEventArgs<T> ();
            cea.PubnubRequestState = pubnubRequestState;
            cea.Message = message;
            cea.IsError = isError;
            cea.IsTimeout = isTimeout;
            cea.CurrRequestType = crt;
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Raising Event of type : {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
            #endif

            if ((crt == CurrentRequestType.Heartbeat) && (heartbeatCoroutineComplete != null)) {
                heartbeatCoroutineComplete.Raise (this, cea);
            } else if ((crt == CurrentRequestType.PresenceHeartbeat) && (presenceHeartbeatCoroutineComplete != null)) {
                presenceHeartbeatCoroutineComplete.Raise (this, cea);
            } else if ((crt == CurrentRequestType.Subscribe) && (subCoroutineComplete != null)) {
                subCoroutineComplete.Raise (this, cea);
            } else if ((crt == CurrentRequestType.NonSubscribe) && (nonSubCoroutineComplete != null)) {
                nonSubCoroutineComplete.Raise (this, cea);
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Request Type not matched {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
            }
            #endif
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
