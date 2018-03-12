using System;
using UnityEngine;

namespace PubNubAPI
{
    internal class HeartbeatWorker
    {  
        public event EventHandler<EventArgs> InternetDisconnected;
        public event EventHandler<EventArgs> InternetAvailable;
        public event EventHandler<EventArgs> RetriesExceeded;
        private readonly PNUnityWebRequest webRequest;
        private readonly PubNubUnity PubNubInstance;
        private bool keepHearbeatRunning;
        private bool isHearbeatRunning;

        private string webRequestId = "";

        private bool internetStatus = true;
        private bool retriesExceeded;
        private int heartbeatInterval = 10;
        public int HeartbeatInterval{
            get {return heartbeatInterval;}
            set {heartbeatInterval = value;}
        }

        private const int MINEXPONENTIALBACKOFF = 1;
        private const int MAXEXPONENTIALBACKOFF = 32;

        private int retryCount;

        internal HeartbeatWorker(PubNubUnity pn, PNUnityWebRequest webRequest){
            PubNubInstance  = pn;
            this.webRequest = webRequest;
            webRequest.WebRequestComplete += WebRequestCompleteHandler;
            this.webRequest.PNLog = this.PubNubInstance.PNLog;
            #if (ENABLE_PUBNUB_LOGGING)
            if(this.PubNubInstance.PNConfig.ReconnectionPolicy.Equals(PNReconnectionPolicy.NONE)){
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("reconnection policy is disabled, please handle reconnection manually."), PNLoggingMethod.LevelWarning);
            } else {
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("reconnection policy set to {0}.", this.PubNubInstance.PNConfig.ReconnectionPolicy), PNLoggingMethod.LevelWarning);    
            }
            #endif

        }

        ~HeartbeatWorker(){
            CleanUp();            
        }

        internal void CleanUp(){
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog ("HeartbeatWorker Cleanup", PNLoggingMethod.LevelWarning);
            #endif
            
            if (webRequest != null) {
                webRequest.WebRequestComplete -= WebRequestCompleteHandler;
                webRequest.AbortRequest(webRequestId, false);
            }
        }

        private void WebRequestCompleteHandler (object sender, EventArgs ea)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog ("WebRequestCompleteHandler HB", PNLoggingMethod.LevelWarning);
            #endif
            
            CustomEventArgs cea = ea as CustomEventArgs;
            
            try {
                if ((cea != null) && (cea.CurrRequestType.Equals(PNCurrentRequestType.Heartbeat))) {
                    HeartbeatHandler (cea);
                }
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("WebRequestCompleteHandler: Exception={0}", ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
            }
        }

        internal void StopHeartbeat ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("StopHeartbeat keepHearbeatRunning={0} isHearbeatRunning={1}", keepHearbeatRunning, isHearbeatRunning), PNLoggingMethod.LevelWarning);
            #endif
            
            keepHearbeatRunning = false;
            if (isHearbeatRunning){
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("Stopping Heartbeat keepHearbeatRunning={0} isHearbeatRunning={1}", keepHearbeatRunning, isHearbeatRunning), PNLoggingMethod.LevelInfo);
                #endif
                
                isHearbeatRunning = false;
                webRequest.AbortRequest (webRequestId, false);
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
                if(!this.PubNubInstance.PNConfig.ReconnectionPolicy.Equals(PNReconnectionPolicy.NONE)){
                    isHearbeatRunning = true;

                    Uri request = BuildRequests.BuildTimeRequest(
                        this.PubNubInstance
                    );

                    RequestState requestState = new RequestState ();
                    requestState.OperationType = PNOperationType.PNHeartbeatOperation;
                    requestState.URL = request.OriginalString; 
                    requestState.Timeout = PubNubInstance.PNConfig.NonSubscribeTimeout;
                    requestState.Pause = pauseTime;
                    requestState.Reconnect = pause;     

                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("heartbeat: request.OriginalString {0} ", request.OriginalString ), PNLoggingMethod.LevelInfo);
                    #endif
                    
                    webRequestId = webRequest.Run(requestState);
                    
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("StartHeartbeat: Heartbeat running"), PNLoggingMethod.LevelInfo);
                    #endif
                } 
            }
            catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("StartHeartbeat: Heartbeat exception {0}", ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
            }
        }

        internal void RunHeartbeat (bool pause, int pauseTime)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("RunHeartbeat keepHearbeatRunning={0} isHearbeatRunning={1}", keepHearbeatRunning, isHearbeatRunning), PNLoggingMethod.LevelInfo);
            #endif
            
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

        private void HeartbeatHandler (CustomEventArgs cea){    
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("HeartbeatHandler keepHearbeatRunning={0} isHearbeatRunning={1} cea.iserror {2}", keepHearbeatRunning, isHearbeatRunning, cea.IsError), PNLoggingMethod.LevelInfo);
            #endif
            
            if (cea.IsTimeout || cea.IsError) {
                RetryLoop ();
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("HeartbeatHandler: Heartbeat timeout={0}", cea.Message.ToString ()), PNLoggingMethod.LevelError);
                #endif
            } else {
                InternetConnectionAvailableHandler (cea);
            }
            isHearbeatRunning = false;
            if (keepHearbeatRunning) {

                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("HeartbeatHandler: Restarting Heartbeat, internetStatus: {0}", internetStatus), PNLoggingMethod.LevelInfo);
                #endif

                int interval = HeartbeatInterval;
                if(this.PubNubInstance.PNConfig.ReconnectionPolicy.Equals(PNReconnectionPolicy.EXPONENTIAL)){
                    interval = (int)(Math.Pow(2, retryCount) - 1);
                    if (interval > MAXEXPONENTIALBACKOFF)
                    {
                        interval = MINEXPONENTIALBACKOFF;
                        retryCount = 1;
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PubNubInstance.PNLog.WriteToLog (string.Format ("HeartbeatHandler: interval > MAXEXPONENTIALBACKOFF, interval: {0} at {1}", interval, DateTime.UtcNow.ToString()), PNLoggingMethod.LevelInfo);
                        #endif

                    }
                    else if (interval < 1)
                    {
                        interval = MINEXPONENTIALBACKOFF;
                        retryCount = 1;
                    }
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("HeartbeatHandler: Restarting Heartbeat, interval: {0} at {1}", interval, DateTime.UtcNow.ToString()), PNLoggingMethod.LevelInfo);
                    #endif

                }

                if (internetStatus) {
                    RunHeartbeat (true, interval);
                }
                else {
                    RunHeartbeat (true, interval);
                }
            }
        }

        private void RetryLoop ()
        {
            internetStatus = false;
            retryCount++;
            if (retryCount <= PubNubInstance.PNConfig.MaximumReconnectionRetries) {
                InternetDisconnected.Raise(this, null);
                
                #if (ENABLE_PUBNUB_LOGGING)
                string cbMessage = string.Format ("Internet Disconnected, retrying. Retry count {0} of {1}", retryCount.ToString (), PubNubInstance.PNConfig.MaximumReconnectionRetries);
                this.PubNubInstance.PNLog.WriteToLog (string.Format("RetryLoop: {0}", cbMessage), PNLoggingMethod.LevelError);
                #endif
                

            } else {
                RetriesExceeded.Raise(this, null);
                retriesExceeded = true;
                
                #if (ENABLE_PUBNUB_LOGGING)
                string cbMessage = string.Format ("Internet Disconnected. Retries exceeded {0}. Unsubscribing connected channels.", PubNubInstance.PNConfig.MaximumReconnectionRetries);
                this.PubNubInstance.PNLog.WriteToLog (string.Format("RetryLoop: {0}", cbMessage), PNLoggingMethod.LevelError);
                #endif

                //stop heartbeat.
                StopHeartbeat();
                //reset internetStatus
                ResetInternetCheckSettings();
            }
        }        

        private void InternetConnectionAvailableHandler(CustomEventArgs cea){    
            internetStatus = true;
            retriesExceeded = false;
            
            if (retryCount > 0) {
                InternetAvailable.Raise(this, null);
                #if (ENABLE_PUBNUB_LOGGING)
                string cbMessage = string.Format ("InternetConnectionAvailableHandler: Internet Connection Available.");
                this.PubNubInstance.PNLog.WriteToLog (cbMessage, PNLoggingMethod.LevelInfo);
                #endif
            }
            retryCount = 0;
        }
  
    }
}