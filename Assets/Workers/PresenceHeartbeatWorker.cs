using System;
using UnityEngine;

namespace PubNubAPI
{
    internal class PresenceHeartbeatWorker
    {  
        private bool keepPresenceHearbeatRunning = false;
        private bool isPresenceHearbeatRunning = false;

        private PNUnityWebRequest webRequest;
        private PubNubUnity PubNubInstance { get; set;}
        internal PresenceHeartbeatWorker(PubNubUnity pn, PNUnityWebRequest webRequest){
            PubNubInstance  = pn;
            this.webRequest = webRequest;
            //webRequest = PubNubInstance.GameObjectRef.AddComponent<PNUnityWebRequest> ();
            webRequest.PresenceHeartbeatWebRequestComplete += WebRequestCompleteHandler;
        }

        ~PresenceHeartbeatWorker(){
            CleanUp();
        }

        internal void CleanUp(){
            if (webRequest != null) {
                webRequest.PresenceHeartbeatWebRequestComplete -= WebRequestCompleteHandler;
                //UnityEngine.Object.Destroy (webRequest);
            }
        }

        private void WebRequestCompleteHandler (object sender, EventArgs ea)
        {
            Debug.Log("WebRequestCompleteHandler PHB");
            CustomEventArgs<PNPresenceHeartbeatResult> cea = ea as CustomEventArgs<PNPresenceHeartbeatResult>;

            try {
                if (cea != null) {
                    Debug.Log(" PHB cea not null");
                    //if (cea.PubnubRequestState != null) {
                    PresenceHeartbeatHandler (cea);
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
                Debug.Log(ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CoroutineCompleteHandler: Exception={1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif

                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, cea.PubnubRequestState, false, false, PubnubErrorLevel);
            }
        }

        internal void StopPresenceHeartbeat ()
        {
            keepPresenceHearbeatRunning = false;
            if (isPresenceHearbeatRunning || webRequest.CheckIfRequestIsRunning(CurrentRequestType.PresenceHeartbeat))
            {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Stopping PresenceHeartbeat ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                #endif
                
                isPresenceHearbeatRunning = false;
                webRequest.AbortRequest<PNPresenceHeartbeatResult> (CurrentRequestType.PresenceHeartbeat, null, false);
            }
        }

        private void PresenceHeartbeatHandler (CustomEventArgs<PNPresenceHeartbeatResult> cea){
            Debug.Log(string.Format ("PresenceHeartbeatHandler keepPresenceHearbeatRunning={0} isPresenceHearbeatRunning={1}", keepPresenceHearbeatRunning, isPresenceHearbeatRunning));
            isPresenceHearbeatRunning = false;

            #if (ENABLE_PUBNUB_LOGGING)
            if (cea.IsTimeout || cea.IsError) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PresenceHeartbeatHandler: Presence Heartbeat timeout={1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelError);
            }else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PresenceHeartbeatHandler: Presence Heartbeat response: {1}", DateTime.Now.ToString (), cea.Message.ToString ()), LoggingMethod.LevelInfo);
            }
            #endif

            if (keepPresenceHearbeatRunning) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PresenceHeartbeatHandler: Restarting PresenceHeartbeat ID {1}", DateTime.Now.ToString (), cea.PubnubRequestState.ID), LoggingMethod.LevelInfo);
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

                    RequestState<PNPresenceHeartbeatResult> requestState = new RequestState<PNPresenceHeartbeatResult> ();
                    requestState.RespType = PNOperationType.PNPresenceHeartbeatOperation;
                    
                    Uri request = BuildRequests.BuildPresenceHeartbeatRequest(
                        Helpers.GetNamesFromChannelEntities(PubNubInstance.SubscriptionInstance.AllNonPresenceChannelsOrChannelGroups, false),
                        Helpers.GetNamesFromChannelEntities(PubNubInstance.SubscriptionInstance.AllNonPresenceChannelsOrChannelGroups, true),
                        channelsJsonState,
                        this.PubNubInstance.PNConfig.UUID,
                        this.PubNubInstance.PNConfig.Secure,
                        this.PubNubInstance.PNConfig.Origin,
                        this.PubNubInstance.PNConfig.AuthKey,
                        this.PubNubInstance.PNConfig.SubscribeKey,
                        this.PubNubInstance.Version
                    );
                    Debug.Log(string.Format ("DateTime {0}, presenceheartbeat: request.OriginalString {1} ", DateTime.Now.ToString (), request.OriginalString ));

                    webRequest.Run<PNPresenceHeartbeatResult>(request.OriginalString, requestState, PubNubInstance.PNConfig.NonSubscribeTimeout, pauseTime, pause);

                    //for heartbeat and presence heartbeat treat reconnect as pause
                    /*RequestState<T> requestState = BuildRequests.BuildRequestState<T> (pubnubRequestState.ChannelEntities, ResponseType.PresenceHeartbeat,
                        pause, pubnubRequestState.ID, false, 0, null);
                    StoredRequestState.Instance.SetRequestState (CurrentRequestType.PresenceHeartbeat, requestState);
                    coroutine.Run<T> (requestUrl.OriginalString, requestState, HeartbeatTimeout, pauseTime);*/
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, StartPresenceHeartbeat: PresenceHeartbeat running for {1}", DateTime.Now.ToString (), pubnubRequestState.ID), LoggingMethod.LevelInfo);
                    #endif
                }
            }
            catch (Exception ex) {
                Debug.Log(ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, StartPresenceHeartbeat: PresenceHeartbeat exception {1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
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
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, RunPresenceHeartbeat: PresenceHeartbeat Already Running ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            }
            #endif
        }

    }
}