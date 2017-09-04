//#define ENABLE_PUBNUB_LOGGING
using System;
using UnityEngine;

namespace PubNubAPI
{
    internal class HeartbeatWorker
    {  
        public event EventHandler<EventArgs> InternetDisconnected;
        public event EventHandler<EventArgs> InternetAvailable;
        public event EventHandler<EventArgs> RetriesExceeded;
        private PNUnityWebRequest webRequest;
        private PubNubUnity PubNubInstance { get; set;}
        private bool keepHearbeatRunning = false;
        private bool isHearbeatRunning = false;

        private bool internetStatus = true;
        private bool retriesExceeded = false;

        private int retryCount = 0;

        internal HeartbeatWorker(PubNubUnity pn, PNUnityWebRequest webRequest){
            PubNubInstance  = pn;
            this.webRequest = webRequest;
            //webRequest = PubNubInstance.GameObjectRef.AddComponent<PNUnityWebRequest> ();
            webRequest.HeartbeatWebRequestComplete += WebRequestCompleteHandler;
            this.webRequest.PNLog = this.PubNubInstance.PNLog;
            Debug.Log("HeartbeatWorker HB");
        }

        ~HeartbeatWorker(){
            CleanUp();            
        }

        internal void CleanUp(){
            Debug.Log("HeartbeatWorker Cleanup");
            if (webRequest != null) {
                webRequest.HeartbeatWebRequestComplete -= WebRequestCompleteHandler;
                //UnityEngine.Object.Destroy (webRequest);
            }
        }

        private void WebRequestCompleteHandler (object sender, EventArgs ea)
        {
            Debug.Log("WebRequestCompleteHandler HB");
            CustomEventArgs<PNTimeResult> cea = ea as CustomEventArgs<PNTimeResult>;
            
            try {
                if (cea != null) {
                    //if (cea.PubnubRequestState != null) {
                    HeartbeatHandler (cea);
                    /*}
                    #if (ENABLE_PUBNUB_LOGGING)
                    else {
                        this.PubNubInstance.PNLog.WriteToLog (string.Format ("CoroutineCompleteHandler: PubnubRequestState null"), PNLoggingMethod.LevelError);
                    }
                    #endif*/
                }
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("CoroutineCompleteHandler: cea null"), PNLoggingMethod.LevelError);
                }
                #endif
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("CoroutineCompleteHandler: Exception={0}", ex.ToString ()), PNLoggingMethod.LevelError);
                #endif

                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, cea.PubnubRequestState, false, false, PubnubErrorLevel);
            }
        }

        internal void StopHeartbeat ()
        {
            Debug.Log(string.Format ("StopHeartbeat keepHearbeatRunning={0} isHearbeatRunning={1}", keepHearbeatRunning, isHearbeatRunning));
            keepHearbeatRunning = false;
            if (isHearbeatRunning || webRequest.CheckIfRequestIsRunning(PNCurrentRequestType.Heartbeat)){
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("Stopping Heartbeat "), PNLoggingMethod.LevelInfo);
                #endif
                Debug.Log(string.Format ("Stopping Heartbeat keepHearbeatRunning={0} isHearbeatRunning={1}", keepHearbeatRunning, isHearbeatRunning));
                
                isHearbeatRunning = false;
                //webRequest.HeartbeatCoroutineComplete -= CoroutineCompleteHandler<PNOperationType.PNHeartbeatOperation>;
                webRequest.AbortRequest<PNTimeResult> (PNCurrentRequestType.Heartbeat, null, false);
            }
        }

        internal void ResetInternetCheckSettings ()
        {
            retryCount = 0;
            internetStatus = true;
            retriesExceeded = false;
        }

        void StartHeartbeat (bool pause, int pauseTime)
        {
            try {
                isHearbeatRunning = true;
                /*Uri requestUrl = BuildRequests.BuildTimeRequest (this.SessionUUID,
                    this.ssl, this.Origin);

                coroutine.HeartbeatCoroutineComplete += CoroutineCompleteHandler<T>;*/
                RequestState<PNTimeResult> requestState = new RequestState<PNTimeResult> ();
                requestState.RespType = PNOperationType.PNHeartbeatOperation;
            
                Uri request = BuildRequests.BuildTimeRequest(
                    this.PubNubInstance.PNConfig.UUID,
                    this.PubNubInstance.PNConfig.Secure,
                    this.PubNubInstance.PNConfig.Origin,
                    this.PubNubInstance.Version
                );

                Debug.Log(string.Format ("heartbeat: request.OriginalString {0} ", request.OriginalString ));
                webRequest.Run<PNTimeResult>(request.OriginalString, requestState, PubNubInstance.PNConfig.NonSubscribeTimeout, pauseTime, pause);
                
                //for heartbeat and presence heartbeat treat reconnect as pause
                /* RequestState<T> requestState = BuildRequests.BuildRequestState<T> (pubnubRequestState.ChannelEntities,
                    ResponseType.Heartbeat, pause, pubnubRequestState.ID, false, 0, null);
                StoredRequestState.Instance.SetRequestState (CurrentRequestType.Heartbeat, requestState);
                coroutine.Run<T> (requestUrl.OriginalString, requestState, HeartbeatTimeout, pauseTime);*/
                #if (ENABLE_PUBNUB_LOGGING)
                //this.PubNubInstance.PNLog.WriteToLog (string.Format ("StartHeartbeat: Heartbeat running for {1}", pubnubRequestState.ID), PNLoggingMethod.LevelInfo);
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("StartHeartbeat: Heartbeat running"), PNLoggingMethod.LevelInfo);
                #endif
            }
            catch (Exception ex) {
                Debug.Log(ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                //this.PubNubInstance.PNLog.WriteToLog (string.Format ("StartHeartbeat: Heartbeat exception {1}", ex.ToString ()), PNLoggingMethod.LevelError);
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("StartHeartbeat: Heartbeat exception {0}", ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
            }
        }

        internal void RunHeartbeat (bool pause, int pauseTime)
        {
            Debug.Log(string.Format ("RunHeartbeat keepHearbeatRunning={0} isHearbeatRunning={1}", keepHearbeatRunning, isHearbeatRunning));
            keepHearbeatRunning = true;
            if (!isHearbeatRunning) {
                StartHeartbeat (pause, pauseTime);
            }
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("RunHeartbeat: Heartbeat Already Running "), PNLoggingMethod.LevelInfo);
            }
            #endif
        }

        private void HeartbeatHandler (CustomEventArgs<PNTimeResult> cea){
            Debug.Log(string.Format ("HeartbeatHandler keepHearbeatRunning={0} isHearbeatRunning={1} cea.iserror {2}", keepHearbeatRunning, isHearbeatRunning, cea.IsError));
            if (cea.IsTimeout || cea.IsError) {
                RetryLoop ();
                #if (ENABLE_PUBNUB_LOGGING)
                //this.PubNubInstance.PNLog.WriteToLog (string.Format ("HeartbeatHandler: Heartbeat timeout={1}", cea.Message.ToString ()), PNPNLoggingMethod.LevelError);
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("HeartbeatHandler: Heartbeat timeout={0}", cea.Message.ToString ()), PNLoggingMethod.LevelError);
                #endif
            } else {
                InternetConnectionAvailableHandler (cea);
            }
            isHearbeatRunning = false;
            if (keepHearbeatRunning) {

                #if (ENABLE_PUBNUB_LOGGING)
                //this.PubNubInstance.PNLog.WriteToLog (string.Format ("HeartbeatHandler: Restarting Heartbeat {0}", cea.PubnubRequestState.ID), PNLoggingMethod.LevelInfo);
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("HeartbeatHandler: Restarting Heartbeat"), PNLoggingMethod.LevelInfo);
                #endif
                if (internetStatus) {
                    Debug.Log("HeartbeatHandler internetStatus");
                    RunHeartbeat (true, PubNubInstance.PNConfig.HeartbeatInterval);
                }
                else {
                    Debug.Log("HeartbeatHandler !internetStatus");
                    RunHeartbeat (true, PubNubInstance.PNConfig.HeartbeatInterval);
                }
            }
        }

        private void RetryLoop ()
        {
            internetStatus = false;
            retryCount++;
            if (retryCount <= PubNubInstance.PNConfig.MaximumReconnectionRetries) {
                InternetDisconnected.Raise(this, null);
                string cbMessage = string.Format ("Internet Disconnected, retrying. Retry count {0} of {1}", retryCount.ToString (), PubNubInstance.PNConfig.MaximumReconnectionRetries);
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format("RetryLoop: {0}", cbMessage), PNLoggingMethod.LevelError);
                #endif
                

            } else {
                RetriesExceeded.Raise(this, null);
                retriesExceeded = true;
                string cbMessage = string.Format ("Internet Disconnected. Retries exceeded {0}. Unsubscribing connected channels.", PubNubInstance.PNConfig.MaximumReconnectionRetries);
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format("RetryLoop: {0}", cbMessage), PNLoggingMethod.LevelError);
                #endif

                //stop heartbeat.
                StopHeartbeat();
                //reset internetStatus
                ResetInternetCheckSettings();
            }
        }        

        private void InternetConnectionAvailableHandler(CustomEventArgs<PNTimeResult> cea){
            internetStatus = true;
            retriesExceeded = false;
            
            if (retryCount > 0) {
                InternetAvailable.Raise(this, null);
                string cbMessage = string.Format ("InternetConnectionAvailableHandler: Internet Connection Available.");
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (cbMessage, PNLoggingMethod.LevelInfo);
                #endif
            }
            retryCount = 0;
        }
  
    }
}