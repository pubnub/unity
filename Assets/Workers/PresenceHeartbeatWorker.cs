using System;
using UnityEngine;

namespace PubNubAPI
{
    internal class PresenceHeartbeatWorker
    {  
        private bool keepPresenceHearbeatRunning = false;
        private bool isPresenceHearbeatRunning = false;

        private PNUnityWebRequest webRequest;
        private string webRequestId = "";
        private PubNubUnity PubNubInstance;
        internal PresenceHeartbeatWorker(PubNubUnity pn, PNUnityWebRequest webRequest){
            PubNubInstance  = pn;
            this.webRequest = webRequest;
            this.webRequest.PNLog = this.PubNubInstance.PNLog;
            //webRequest = PubNubInstance.GameObjectRef.AddComponent<PNUnityWebRequest> ();
            webRequest.WebRequestComplete += WebRequestCompleteHandler;
        }

        ~PresenceHeartbeatWorker(){
            CleanUp();
        }

        internal void CleanUp(){
            if (webRequest != null) {
                webRequest.WebRequestComplete -= WebRequestCompleteHandler;
                //UnityEngine.Object.Destroy (webRequest);
            }
        }

        private void WebRequestCompleteHandler (object sender, EventArgs ea)
        {
            Debug.Log("WebRequestCompleteHandler PHB");
            //CustomEventArgs<PNPresenceHeartbeatResult> cea = ea as CustomEventArgs<PNPresenceHeartbeatResult>;
            CustomEventArgs cea = ea as CustomEventArgs;

            try {
                if ((cea != null) && (cea.CurrRequestType.Equals(PNCurrentRequestType.PresenceHeartbeat))) {
                    Debug.Log(" PHB cea not null");
                    //if (cea.PubnubRequestState != null) {
                    PresenceHeartbeatHandler (cea);
                    /*}
                    #if (ENABLE_PUBNUB_LOGGING)
                    else {
                        this.PubNubInstance.PNLog.WriteToLog (string.Format ("CoroutineCompleteHandler: PubnubRequestState null"), PNLoggingMethod.LevelError);
                    }
                    #endif*/
                }
                /*#if (ENABLE_PUBNUB_LOGGING)
                else {
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("CoroutineCompleteHandler: cea null"), PNLoggingMethod.LevelError);
                }
                #endif*/
            } catch (Exception ex) {
                Debug.Log(ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("WebRequestCompleteHandler: Exception={0}", ex.ToString ()), PNLoggingMethod.LevelError);
                #endif

                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, cea.PubnubRequestState, false, false, PubnubErrorLevel);
            }
        }

        internal void StopPresenceHeartbeat ()
        {
            keepPresenceHearbeatRunning = false;
            //if (isPresenceHearbeatRunning || webRequest.CheckIfRequestIsRunning(PNCurrentRequestType.PresenceHeartbeat))
            if (isPresenceHearbeatRunning)
            {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("Stopping PresenceHeartbeat "), PNLoggingMethod.LevelInfo);
                #endif
                
                isPresenceHearbeatRunning = false;
                //webRequest.AbortRequest<PNPresenceHeartbeatResult> (PNCurrentRequestType.PresenceHeartbeat, null, false);
                webRequest.AbortRequest (webRequestId);
            }
        }

        //private void PresenceHeartbeatHandler (CustomEventArgs<PNPresenceHeartbeatResult> cea){
        private void PresenceHeartbeatHandler (CustomEventArgs cea){
            Debug.Log(string.Format ("PresenceHeartbeatHandler keepPresenceHearbeatRunning={0} isPresenceHearbeatRunning={1}", keepPresenceHearbeatRunning, isPresenceHearbeatRunning));
            isPresenceHearbeatRunning = false;

            #if (ENABLE_PUBNUB_LOGGING)
            if (cea.IsTimeout || cea.IsError) {
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("PresenceHeartbeatHandler: Presence Heartbeat timeout={0}", cea.Message.ToString ()), PNLoggingMethod.LevelError);
            }else {
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("PresenceHeartbeatHandler: Presence Heartbeat response: {0}", cea.Message.ToString ()), PNLoggingMethod.LevelInfo);
            }
            #endif

            if (keepPresenceHearbeatRunning) {
                #if (ENABLE_PUBNUB_LOGGING)
                //this.PubNubInstance.PNLog.WriteToLog (string.Format ("PresenceHeartbeatHandler: Restarting PresenceHeartbeat ID {0}", cea.PubnubRequestState.ID), PNLoggingMethod.LevelInfo);
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("PresenceHeartbeatHandler: Restarting PresenceHeartbeat"), PNLoggingMethod.LevelInfo);
                #endif
                RunPresenceHeartbeat (true, PubNubInstance.PNConfig.PresenceInterval);
            }
        }

        void StartPresenceHeartbeat (bool pause, int pauseTime)
        {
            try {
                if(PubNubInstance.SubscriptionInstance.AllNonPresenceChannelsOrChannelGroups.Count > 0){
                    isPresenceHearbeatRunning = true;
                    string channelsJsonState = PubNubInstance.SubscriptionInstance.CompiledUserState;

                    /* Uri request = BuildRequests.BuildPresenceHeartbeatRequest(
                        Helpers.GetNamesFromChannelEntities(PubNubInstance.SubscriptionInstance.AllNonPresenceChannelsOrChannelGroups, false),
                        Helpers.GetNamesFromChannelEntities(PubNubInstance.SubscriptionInstance.AllNonPresenceChannelsOrChannelGroups, true),
                        channelsJsonState,
                        this.PubNubInstance.PNConfig.UUID,
                        this.PubNubInstance.PNConfig.Secure,
                        this.PubNubInstance.PNConfig.Origin,
                        this.PubNubInstance.PNConfig.AuthKey,
                        this.PubNubInstance.PNConfig.SubscribeKey,
                        this.PubNubInstance.Version
                    ); */

                    Uri request = BuildRequests.BuildPresenceHeartbeatRequest(
                        Helpers.GetNamesFromChannelEntities(PubNubInstance.SubscriptionInstance.AllNonPresenceChannelsOrChannelGroups, false),
                        Helpers.GetNamesFromChannelEntities(PubNubInstance.SubscriptionInstance.AllNonPresenceChannelsOrChannelGroups, true),
                        channelsJsonState,
                        ref this.PubNubInstance
                    );

                    RequestState requestState = new RequestState ();
                    requestState.OperationType = PNOperationType.PNPresenceHeartbeatOperation;
                    requestState.URL = request.OriginalString; 
                    requestState.Timeout = PubNubInstance.PNConfig.NonSubscribeTimeout;
                    requestState.Pause = pauseTime;
                    requestState.Reconnect = pause;

                    Debug.Log(string.Format ("presenceheartbeat: request.OriginalString {0} ", request.OriginalString ));

                    webRequestId = webRequest.Run(requestState);

                    //for heartbeat and presence heartbeat treat reconnect as pause
                    /*RequestState<T> requestState = BuildRequests.BuildRequestState<T> (pubnubRequestState.ChannelEntities, ResponseType.PresenceHeartbeat,
                        pause, pubnubRequestState.ID, false, 0, null);
                    StoredRequestState.Instance.SetRequestState (CurrentRequestType.PresenceHeartbeat, requestState);
                    coroutine.Run<T> (requestUrl.OriginalString, requestState, HeartbeatTimeout, pauseTime);*/
                    #if (ENABLE_PUBNUB_LOGGING)
                    //this.PubNubInstance.PNLog.WriteToLog (string.Format ("StartPresenceHeartbeat: PresenceHeartbeat running for {1}", pubnubRequestState.ID), PNLoggingMethod.LevelInfo);
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("StartPresenceHeartbeat: PresenceHeartbeat running "), PNLoggingMethod.LevelInfo);
                    #endif
                }
            }
            catch (Exception ex) {
                Debug.Log(ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("StartPresenceHeartbeat: PresenceHeartbeat exception {0}", ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
            }
        }
        
        internal void RunPresenceHeartbeat (bool pause, int pauseTime)
        {
            Debug.Log(string.Format ("RunPresenceHeartbeat keepPresenceHearbeatRunning={0} isPresenceHearbeatRunning={1}", keepPresenceHearbeatRunning, isPresenceHearbeatRunning));
            keepPresenceHearbeatRunning = true;
            if (!isPresenceHearbeatRunning) {
                StartPresenceHeartbeat (pause, pauseTime);
            }
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("RunPresenceHeartbeat: PresenceHeartbeat Already Running "), PNLoggingMethod.LevelInfo);
            }
            #endif
        }

    }
}