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

        internal HeartbeatWorker(PubNubUnity pn){
            PubNubInstance  = pn;
            webRequest = PubNubInstance.GameObjectRef.AddComponent<PNUnityWebRequest> ();
            webRequest.HeartbeatWebRequestComplete += WebRequestCompleteHandler;
        }

        ~HeartbeatWorker(){
            CleanUp();            
        }

        internal void CleanUp(){
            if (webRequest != null) {
                webRequest.HeartbeatWebRequestComplete -= WebRequestCompleteHandler;
                UnityEngine.Object.Destroy (webRequest);
            }
        }

        private void WebRequestCompleteHandler (object sender, EventArgs ea)
        {
            CustomEventArgs<PNTimeResult> cea = ea as CustomEventArgs<PNTimeResult>;

            try {
                if (cea != null) {
                    //if (cea.PubnubRequestState != null) {
                    HeartbeatHandler (cea);
                    /*}
                    #if (ENABLE_PUBNUB_LOGGING)
                    else {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CoroutineCompleteHandler: PubnubRequestState null", DateTime.Now.ToString ()), LoggingMethod.LevelError);
                    }
                    #endif*/
                }
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CoroutineCompleteHandler: cea null", DateTime.Now.ToString ()), LoggingMethod.LevelError);
                }
                #endif
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CoroutineCompleteHandler: Exception={1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif

                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, cea.PubnubRequestState, false, false, PubnubErrorLevel);
            }
        }

        internal void StopHeartbeat ()
        {
            keepHearbeatRunning = false;
            if (isHearbeatRunning || webRequest.CheckIfRequestIsRunning(CurrentRequestType.Heartbeat)){
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Stopping Heartbeat ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                #endif
                
                isHearbeatRunning = false;
                //webRequest.HeartbeatCoroutineComplete -= CoroutineCompleteHandler<PNOperationType.PNHeartbeatOperation>;
                webRequest.AbortRequest<PNTimeResult> (CurrentRequestType.Heartbeat, null, false);
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
                requestState.RespType = PNOperationType.PNTimeOperation;
            
                Uri request = BuildRequests.BuildTimeRequest(
                    this.PubNubInstance.PNConfig.UUID,
                    this.PubNubInstance.PNConfig.Secure,
                    this.PubNubInstance.PNConfig.Origin,
                    this.PubNubInstance.Version
                );

                webRequest.Run<PNTimeResult>(request.OriginalString, requestState, PubNubInstance.PNConfig.NonSubscribeTimeout, pauseTime, pause);
                
                //for heartbeat and presence heartbeat treat reconnect as pause
                /* RequestState<T> requestState = BuildRequests.BuildRequestState<T> (pubnubRequestState.ChannelEntities,
                    ResponseType.Heartbeat, pause, pubnubRequestState.ID, false, 0, null);
                StoredRequestState.Instance.SetRequestState (CurrentRequestType.Heartbeat, requestState);
                coroutine.Run<T> (requestUrl.OriginalString, requestState, HeartbeatTimeout, pauseTime);*/
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, StartHeartbeat: Heartbeat running for {1}", DateTime.Now.ToString (), pubnubRequestState.ID), LoggingMethod.LevelInfo);
                #endif
            }
            catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, StartHeartbeat: Heartbeat exception {1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
            }
        }

        internal void RunHeartbeat (bool pause, int pauseTime)
        {
            keepHearbeatRunning = true;
            if (!isHearbeatRunning) {
                StartHeartbeat (pause, pauseTime);
            }
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, RunHeartbeat: Heartbeat Already Running ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            }
            #endif
        }

        private void HeartbeatHandler (CustomEventArgs<PNTimeResult> cea){
            if (cea.IsTimeout || cea.IsError) {
                RetryLoop ();
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, HeartbeatHandler: Heartbeat timeout={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
                #endif
            } else {
                InternetConnectionAvailableHandler (cea);
            }
            isHearbeatRunning = false;
            if (keepHearbeatRunning) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, HeartbeatHandler: Restarting Heartbeat {1}", DateTime.Now.ToString (), cea.PubnubRequestState.ID), LoggingMethod.LevelInfo);
                #endif
                if (internetStatus) {
                    RunHeartbeat (true, PubNubInstance.PNConfig.PresenceInterval);
                }
                else {
                    RunHeartbeat (true, PubNubInstance.PNConfig.HeartbeatInterval);
                }
            }
        }

        private void RetryLoop ()
        {
            internetStatus = false;
            retryCount++;
            if (retryCount <= PubNubInstance.PNConfig.MaximumReconnectionRetries) {
                string cbMessage = string.Format ("Internet Disconnected, retrying. Retry count {0} of {1}", retryCount.ToString (), PubNubInstance.PNConfig.MaximumReconnectionRetries);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format("DateTime {0}, RetryLoop: {1}", DateTime.Now.ToString (), cbMessage), LoggingMethod.LevelError);
                #endif
                /*PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (cbMessage, pubnubRequestState,
                    PubnubErrorSeverity.Warn, PubnubErrorCode.NoInternetRetryConnect, PubnubErrorLevel);*/

            } else {
                retriesExceeded = true;
                string cbMessage = string.Format ("Internet Disconnected. Retries exceeded {0}. Unsubscribing connected channels.", PubNubInstance.PNConfig.MaximumReconnectionRetries);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format("DateTime {0}, RetryLoop: {1}", DateTime.Now.ToString (), cbMessage), LoggingMethod.LevelError);
                #endif

                //stop heartbeat.
                StopHeartbeat();
                //reset internetStatus
                ResetInternetCheckSettings();

                //coroutine.BounceRequest<T> (CurrentRequestType.Subscribe, null, false);

                /*PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (cbMessage, pubnubRequestState,
                    PubnubErrorSeverity.Warn, PubnubErrorCode.NoInternetRetryConnect, PubnubErrorLevel);*/


                //MultiplexExceptionHandler<T> (ResponseType.SubscribeV2, true, false);
            }
        }        

        private void InternetConnectionAvailableHandler(CustomEventArgs<PNTimeResult> cea){
            internetStatus = true;
            retriesExceeded = false;
            if (retryCount > 0) {
                string cbMessage = string.Format ("DateTime {0}, InternetConnectionAvailableHandler: Internet Connection Available.", DateTime.Now.ToString ());
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (cbMessage, LoggingMethod.LevelInfo);
                #endif

                /*PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (cbMessage, cea.PubnubRequestState,
                    PubnubErrorSeverity.Info, PubnubErrorCode.YesInternet, PubnubErrorLevel);*/

               // MultiplexExceptionHandler (ResponseType.SubscribeV2, false, true);
            }
            retryCount = 0;
        }
  
    }
}