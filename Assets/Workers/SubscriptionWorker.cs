using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PubNubAPI
{
    internal class SusbcribeEventEventArgs : EventArgs
    {
        public PNStatus Status;
        public PNPresenceEventResult PresenceEventResult;
        public PNMessageResult MessageResult;
    }

    public class SubscriptionWorker<U>
    {
        private PNUnityWebRequest webRequest;
        private PubNubUnity PubNubInstance;

        private HeartbeatWorker hbWorker;
        private PresenceHeartbeatWorker phbWorker;
        private string webRequestId = ""; 

        private bool reconnect = false;
        private bool internetStatus = true;

        bool enableResumeOnReconnect;


        //Allow one instance only        
        public SubscriptionWorker (PubNubUnity pn)
        { 
            PubNubInstance = pn;
            webRequest = PubNubInstance.GameObjectRef.AddComponent<PNUnityWebRequest> ();
            webRequest.WebRequestComplete += WebRequestCompleteHandler;
            this.webRequest.PNLog = this.PubNubInstance.PNLog;
            hbWorker = new HeartbeatWorker(pn, webRequest);
            hbWorker.InternetAvailable += InternetAvailableHandler;
            hbWorker.InternetDisconnected += InternetDisconnectedHandler;
            hbWorker.RetriesExceeded += RetriesExceededHandler;
            phbWorker = new PresenceHeartbeatWorker(pn, webRequest);
            enableResumeOnReconnect = this.PubNubInstance.PNConfig.ReconnectionPolicy.Equals(PNReconnectionPolicy.LINEAR) | this.PubNubInstance.PNConfig.ReconnectionPolicy.Equals(PNReconnectionPolicy.EXPONENTIAL);
        }

        void InternetAvailableHandler(object sender, EventArgs ea){
            /*PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (cbMessage, cea.PubnubRequestState,
                    PubnubErrorSeverity.Info, PubnubErrorCode.YesInternet, PubnubErrorLevel);*/

            //MultiplexExceptionHandler (ResponseType.SubscribeV2, false, true);
            reconnect = true;
            internetStatus = true;
            Debug.Log("Internet available");

            if(
                PubNubInstance.PNConfig.HeartbeatNotificationOption.Equals(PNHeartbeatNotificationOption.All)
                //|| PubNubInstance.PNConfig.HeartbeatNotificationOption.Equals(PNHeartbeatNotificationOption.Failures)
            ){
                PNStatus pnStatus = Helpers.CreatePNStatus(
                        PNStatusCategory.PNReconnectedCategory,
                        "",
                        null,
                        true,
                        PNOperationType.PNSubscribeOperation,
                        PubNubInstance.SubscriptionInstance.AllChannels,
                        PubNubInstance.SubscriptionInstance.AllChannelGroups,
                        null,
                        this.PubNubInstance
                    );

                CreateEventArgsAndRaiseEvent(pnStatus);
            }

            ExceptionHandler (null);

        }

        void InternetDisconnectedHandler(object sender, EventArgs ea){
            /*PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (cbMessage, pubnubRequestState,
                    PubnubErrorSeverity.Warn, PubnubErrorCode.NoInternetRetryConnect, PubnubErrorLevel);*/
            Debug.Log("Internet disconnected");
            AbortPreviousRequest(null);
            internetStatus = false;
            if(
                PubNubInstance.PNConfig.HeartbeatNotificationOption.Equals(PNHeartbeatNotificationOption.All)
                || PubNubInstance.PNConfig.HeartbeatNotificationOption.Equals(PNHeartbeatNotificationOption.Failures)
            ){
                PNStatus pnStatus = Helpers.CreatePNStatus(
                        PNStatusCategory.PNDisconnectedCategory,
                        "",
                        null,
                        true,
                        PNOperationType.PNSubscribeOperation,
                        PubNubInstance.SubscriptionInstance.AllChannels,
                        PubNubInstance.SubscriptionInstance.AllChannelGroups,
                        null,
                        this.PubNubInstance
                    );

                CreateEventArgsAndRaiseEvent(pnStatus);
            }
        }

        void RetriesExceededHandler(object sender, EventArgs ea){
            //coroutine.BounceRequest<T> (CurrentRequestType.Subscribe, null, false);
            BounceRequest();

                /*PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (cbMessage, pubnubRequestState,
                    PubnubErrorSeverity.Warn, PubnubErrorCode.NoInternetRetryConnect, PubnubErrorLevel);*/
            Debug.Log("Retries exceeded");  
            hbWorker.ResetInternetCheckSettings();

            #if (ENABLE_PUBNUB_LOGGING)
            List<ChannelEntity> channelEntities = PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;
            string channelGroups = Helpers.GetNamesFromChannelEntities (channelEntities, true);
            string channels = Helpers.GetNamesFromChannelEntities (channelEntities, false);
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("ExceptionHandler: MAX retries reached. Exiting the subscribe for channels = {0} and channelgroups = {1}", channels, channelGroups), PNLoggingMethod.LevelInfo);
            #endif

            UnsubscribeAllBuilder unsubBuilder = new UnsubscribeAllBuilder(this.PubNubInstance);
            unsubBuilder.Async((result, status) => {
                Debug.Log ("in UnsubscribeAll");
                if(status.Error){
                    Debug.Log (string.Format("In Example, UnsubscribeAll Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category));
                } else {
                    Debug.Log (string.Format("In UnsubscribeAll, result: {0}", result.Message));
                }
            });

            if(
                PubNubInstance.PNConfig.HeartbeatNotificationOption.Equals(PNHeartbeatNotificationOption.All)
                || PubNubInstance.PNConfig.HeartbeatNotificationOption.Equals(PNHeartbeatNotificationOption.Failures)
            ){
                PNStatus pnStatus = Helpers.CreatePNStatus(
                        PNStatusCategory.PNReconnectionAttemptsExhausted,
                        "",
                        null,
                        true,
                        PNOperationType.PNSubscribeOperation,
                        PubNubInstance.SubscriptionInstance.AllChannels,
                        PubNubInstance.SubscriptionInstance.AllChannelGroups,
                        null,
                        this.PubNubInstance
                    );

                CreateEventArgsAndRaiseEvent(pnStatus);
            }
            /*Helpers.CheckSubscribedChannelsAndSendCallbacks<T> (PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups,
                type, NetworkCheckMaxRetries, PubnubErrorLevel);*/
                
        }

        ~SubscriptionWorker(){
            CleanUp();
        }

        public void CleanUp(){
            if (webRequest != null) {
                webRequest.WebRequestComplete -= WebRequestCompleteHandler;
                UnityEngine.Object.Destroy (webRequest);
            }
            if(hbWorker != null){
                hbWorker.CleanUp();
            }
            if(phbWorker != null){
                phbWorker.CleanUp();
            }
        }
        private bool resetTimetoken = false;
        private bool uuidChanged = false;
        public bool UUIDChanged{
            get{
                return uuidChanged;
            }
            set{
                uuidChanged = value;
            }
        }

        private long lastSubscribeTimetoken = 0;
        private long lastSubscribeTimetokenForNewMultiplex = 0;

        private string region = "";

        /*void Start(){
            Debug.Log("SubscriptionWorker start");
        }
        void Update(){

            Debug.Log("SubscriptionWorker Update");
        }*/

        //private static volatile SubscriptionWorker<U> instance;
        //private static object syncRoot = new System.Object();

        /*public static SubscriptionWorker<U> Instance
        {
            get 
            {
                if (instance == null) 
                {
                    lock (syncRoot) 
                    {
                        if (instance == null) {
                            instance = new SubscriptionWorker<U> ();
                            //instance.webRequest = PubNub.GameObjectRef.AddComponent<PNUnityWebRequest> ();
                            //instance.webRequest.SubWebRequestComplete += instance.WebRequestCompleteHandler;

                        }
                    }
                }

                return instance;
            }
        }*/

        public void BounceRequest(){
            AbortPreviousRequest(null);
            ContinueToSubscribeRestOfChannels();
        }

        public void AbortPreviousRequest(List<ChannelEntity> existingChannels)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format("AbortPreviousRequest: Aborting previous subscribe/presence requests having channel(s)={0} and ChannelGroup(s) = {1}", Helpers.GetNamesFromChannelEntities(existingChannels, false), Helpers.GetNamesFromChannelEntities(existingChannels, true)), PNLoggingMethod.LevelInfo);
            #endif

            //webRequest.AbortRequest<U>(PNCurrentRequestType.Subscribe, null, false);
            webRequest.AbortRequest(webRequestId, false);
        }

        public void ContinueToSubscribeRestOfChannels()
        {
            List<ChannelEntity> subscribedChannels = this.PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;

            if (subscribedChannels != null && subscribedChannels.Count > 0)
            {
                //TODO
                Debug.Log("ContinueToSubscribeRestOfChannels: " + subscribedChannels.Count());
                hbWorker.ResetInternetCheckSettings();
                RunSubscribeRequest (0, false);

                //Modify the value for type ResponseType. Presence or Subscrie is ok, but sending the close value would make sense
                /*if (this.PubNubInstance.SubscriptionInstance.HasPresenceChannels)
                {
                    type = PNOperationType.PNPresenceOperation;
                }
                else
                {
                    type = PNOperationType.PNSubscribeOperation;
                }
                //Continue with any remaining channels for subscribe/presence
                RequestState<T> reqState = StoredRequestState.Instance.GetStoredRequestState (CurrentRequestType.Subscribe) as RequestState<T>;
                if (reqState == null) {
                    if (typeof(T).Equals (typeof(object))) {
                        RequestState<object> reqStateStr = StoredRequestState.Instance.GetStoredRequestState (CurrentRequestType.Subscribe) as RequestState<object>;
                        MultiChannelSubscribeRequest<string> (type, 0, false);
                    } else if (typeof(T).Equals (typeof(string))) {
                        RequestState<string> reqStateObj = StoredRequestState.Instance.GetStoredRequestState (CurrentRequestType.Subscribe) as RequestState<string>;
                        MultiChannelSubscribeRequest<object> (type, 0, false);
                    } else {
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PubNubInstance.PNLog.WriteToLog (string.Format ("DateTime {0}, ContinueToSubscribeRestOfChannels: reqState none matched", DateTime.Now.ToString ()), PNLoggingMethod.LevelInfo);
                        #endif
                    }
                } else {
                    RequestState<T> reqStateStr = StoredRequestState.Instance.GetStoredRequestState (CurrentRequestType.Subscribe) as RequestState<T>;
                    MultiChannelSubscribeRequest<T> (type, 0, false);
                }*/
            }
            else
            {
                hbWorker.StopHeartbeat();
                phbWorker.StopPresenceHeartbeat();
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog(string.Format("ContinueToSubscribeRestOfChannels: All channels are Unsubscribed. Further subscription was stopped"), PNLoggingMethod.LevelInfo);
                #endif
                //ExceptionHandlers.MultiplexException -= HandleMultiplexException<T>;
            }
        }

        //public void Add (PNOperationType pnOpType, object pnBuilder, RequestState<SubscribeRequestBuilder> reqState){
        public void Add (long timetokenToUse, List<ChannelEntity> existingChannels){
            //Abort existing request
            try{
                //Debug.Log("in add:" + reqState.Reconnect + this.PubNubInstance.Test);
                //SubscribeRequestBuilder subscribeBuilder = (SubscribeRequestBuilder)pnBuilder;

                //reconnect = false;
                Debug.Log("after add");

                bool internetStatus = true;
                if (internetStatus) {

                    if (!timetokenToUse.Equals (0)) {
                        lastSubscribeTimetokenForNewMultiplex = timetokenToUse;
                    } else if (existingChannels.Count > 0) {
                        lastSubscribeTimetokenForNewMultiplex = lastSubscribeTimetoken;
                    }
                    AbortPreviousRequest (existingChannels);
                    RunSubscribeRequest (0, false);
                }
                
               
                /*EventHandler handler = PubNub.SusbcribeCallback;
                if (handler != null) {
                    Debug.Log ("Raising SusbcribeEvent");
                    handler (typeof(SubscriptionWorker), mea);
                } else {
                    Debug.Log ("SusbcribeEvent null");
                }*/
            }catch (Exception ex){
                Debug.Log (ex.ToString());
            }

        }

        /*public void MultiChannelSubscribeInit<T> (PNOperationType respType, string channel, string channelGroup, long timetokenToUse
            )
        {
            string[] rawChannels = channel.Split (',');
            string[] rawChannelGroups = channelGroup.Split (',');

            List<ChannelEntity> subscribedChannels = Subscription.Instance.AllSubscribedChannelsAndChannelGroups;

            ResetInternetCheckSettings ();

            List<ChannelEntity> newChannelEntities;
            bool channelsOrChannelGroupsAdded = Helpers.RemoveDuplicatesCheckAlreadySubscribedAndGetChannels<T> (respType, null, rawChannels, rawChannelGroups,
                PubnubErrorLevel, false, out newChannelEntities);

            if ((channelsOrChannelGroupsAdded) && (internetStatus)) {
                Subscription.Instance.Add (newChannelEntities);

                #if (ENABLE_PUBNUB_LOGGING)
                Helpers.LogChannelEntitiesDictionary ();
                #endif

                if (!timetokenToUse.Equals (0)) {
                    lastSubscribeTimetokenForNewMultiplex = timetokenToUse;
                } else if (subscribedChannels.Count > 0) {
                    lastSubscribeTimetokenForNewMultiplex = lastSubscribeTimetoken;
                }
                AbortPreviousRequest<T> (subscribedChannels);
                MultiChannelSubscribeRequest<T> (respType, 0, false);
            }
            #if (ENABLE_PUBNUB_LOGGING)
            else {
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("MultiChannelSubscribeInit: channelsOrChannelGroupsAdded {1}, internet status {2}",
             channelsOrChannelGroupsAdded.ToString (), internetStatus.ToString ()), PNLoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
            }
            #endif
        }*/

        private bool CheckAllChannelsAreUnsubscribed()
        {
            if (PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups.Count <=0)
            {
                hbWorker.StopHeartbeat();
                phbWorker.StopPresenceHeartbeat();
                // ExceptionHandlers.MultiplexException -= HandleMultiplexException<T>;

                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog(string.Format("CheckAllChannelsAreUnsubscribed: All channels are Unsubscribed. Further subscription was stopped"), PNLoggingMethod.LevelInfo);
                #endif
                return true;
            }
            return false;
        }


        long SaveLastTimetoken(long timetoken)
        {
            long lastTimetoken = 0;
            long sentTimetoken = timetoken;
            #if (ENABLE_PUBNUB_LOGGING)
            StringBuilder sbLogger = new StringBuilder();
            sbLogger.AppendFormat("SaveLastTimetoken: lastSubscribeTimetokenForNewMultiplex={0}\n", lastSubscribeTimetokenForNewMultiplex);
            sbLogger.AppendFormat("SaveLastTimetoken: sentTimetoken={0}\n", sentTimetoken.ToString());
            sbLogger.AppendFormat("SaveLastTimetoken: lastSubscribeTimetoken={0}\n", lastSubscribeTimetoken);
            #endif
            if (resetTimetoken || uuidChanged)
            {
                lastTimetoken = 0;
                uuidChanged = false;
                resetTimetoken = false;
                #if (ENABLE_PUBNUB_LOGGING)
                sbLogger.AppendFormat("SaveLastTimetoken: resetTimetoken\n");
                #endif
            }
            else
            {
                //override lastTimetoken when lastSubscribeTimetokenForNewMultiplex is set.
                //this is done to use the timetoken prior to the latest response from the server
                //and is true in case new channels are added to the subscribe list.
                if (!sentTimetoken.Equals(0) && !lastSubscribeTimetokenForNewMultiplex.Equals(0))
                {
                    lastTimetoken = lastSubscribeTimetokenForNewMultiplex;
                    lastSubscribeTimetokenForNewMultiplex = 0;
                    #if (ENABLE_PUBNUB_LOGGING)
                    sbLogger.AppendFormat("SaveLastTimetoken: Using lastSubscribeTimetokenForNewMultiplex={0}\n", lastTimetoken);
                    #endif
                }
                else {
                    lastTimetoken = sentTimetoken;
                    #if (ENABLE_PUBNUB_LOGGING)
                    sbLogger.AppendFormat("SaveLastTimetoken2: Using sentTimetoken={0}\n", sentTimetoken);
                    #endif
                }
            }
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("{0} ", sbLogger.ToString()), PNLoggingMethod.LevelInfo);
            #endif

            return lastTimetoken;
        }

        private void RunSubscribeRequest (long timetoken, bool reconnect)
        {
            //Exit if the channel is unsubscribed
            Debug.Log("in  RunSubscribeRequest");
            if (CheckAllChannelsAreUnsubscribed())
            {
                Debug.Log("All channels unsubscribed");
                return;
            }
			List<ChannelEntity> channelEntities = PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;

            // Begin recursive subscribe
            try {
                long lastTimetoken = SaveLastTimetoken(timetoken);

                hbWorker.RunHeartbeat (false, hbWorker.HeartbeatInterval);
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("RunRequests: Heartbeat started"), PNLoggingMethod.LevelInfo);
                #endif
                if (PubNubInstance.PNConfig.PresenceInterval > 0){
                    phbWorker.RunPresenceHeartbeat(false, PubNubInstance.PNConfig.PresenceInterval);
                }

                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("MultiChannelSubscribeRequest: Building request for {0} with timetoken={1}", Helpers.GetNamesFromChannelEntities(channelEntities), lastTimetoken), PNLoggingMethod.LevelInfo);
                #endif
                // Build URL
				string channelsJsonState = PubNubInstance.SubscriptionInstance.CompiledUserState;
                //TODO fix and remove
                channelsJsonState = this.PubNubInstance.SubscriptionInstance.CompiledUserState;

                string channels = Helpers.GetNamesFromChannelEntities(channelEntities, false);
                string channelGroups = Helpers.GetNamesFromChannelEntities(channelEntities, true);

                //v2
                string filterExpr = (!string.IsNullOrEmpty(this.PubNubInstance.PNConfig.FilterExpression)) ? this.PubNubInstance.PNConfig.FilterExpression : string.Empty;
                Uri requestUrl = BuildRequests.BuildSubscribeRequest (
                    channels,
                    channelGroups, 
                    lastTimetoken.ToString(), 
                    channelsJsonState,
                    region,
                    filterExpr,
                    ref this.PubNubInstance
                );
                /*Uri requestUrl = BuildRequests.BuildSubscribeRequest (
                    channels,
                    channelGroups, 
                    lastTimetoken.ToString(), 
                    channelsJsonState,
                    this.PubNubInstance.PNConfig.UUID, 
                    region,
                    filterExpr, 
                    true, 
                    PubNubInstance.PNConfig.Origin, 
                    this.PubNubInstance.PNConfig.AuthKey, 
                    PubNubInstance.PNConfig.SubscribeKey, 
                    this.PubNubInstance.PNConfig.PresenceTimeout,
                    PubNubInstance.Version
                );*/
                
                /*Uri requestUrl = BuildRequests.BuildMultiChannelSubscribeRequest (channels,
                    channelGroups, lastTimetoken.ToString(), channelsJsonState, this.SessionUUID, this.Region,
                    filterExpr, this.ssl, this.Origin, authenticationKey, this.subscribeKey, this.PresenceHeartbeat);*/


                //RequestState<T> pubnubRequestState = BuildRequests.BuildRequestState<T> (channelEntities, type, reconnect,
                    //0, false, Convert.ToInt64 (timetoken.ToString ()), typeof(T));
                // Wait for message
                //ExceptionHandlers.MultiplexException += HandleMultiplexException<T>;

                //UrlProcessRequest<T> (requestUrl, pubnubRequestState);
                Debug.Log ("RunSubscribeRequest " + requestUrl.OriginalString);

                //RequestState<SubscribeEnvelope> requestState = new RequestState<SubscribeEnvelope> ();
                RequestState requestState = new RequestState ();
                //requestState.ChannelEntities = channelEntities;
                requestState.OperationType = PNOperationType.PNSubscribeOperation;
                requestState.URL = requestUrl.OriginalString; 
                requestState.Timeout = PubNubInstance.PNConfig.SubscribeTimeout;
                requestState.Pause = 0;
                requestState.Reconnect = reconnect;
                //requestState.ChannelEntities = channelEntities;

                //PNCallback<T> timeCallback = new PNTimeCallback<T> (callback);
                //http://ps.pndsn.com/v2/presence/sub-key/sub-c-5c4fdcc6-c040-11e5-a316-0619f8945a4f/uuid/UUID_WhereNow?pnsdk=PubNub-Go%2F3.14.0&uuid=UUID_WhereNow
                webRequestId = webRequest.Run(requestState);

            } catch (Exception ex) {
                Debug.Log("in  MultiChannelSubscribeRequest" + ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("MultiChannelSubscribeRequest: method:_subscribe \n channel={0} \n timetoken={1} \n Exception Details={2}", Helpers.GetNamesFromChannelEntities(channelEntities), timetoken.ToString (), ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
                //PubnubCallbacks.CallErrorCallback<T> (ex, channelEntities,
                  //  PubnubErrorCode.None, PubnubErrorSeverity.Critical, PubnubErrorLevel);

                this.RunSubscribeRequest (timetoken, false);
            }
        }

        SubscribeEnvelope ParseReceiedJSONV2 (string jsonString)
        {
            if (!string.IsNullOrEmpty (jsonString)) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("ParseReceiedJSONV2: jsonString = {0}",  jsonString), PNLoggingMethod.LevelInfo);
                #endif
                
                //this doesnt work on JSONFx for Unity in case a string is passed in an variable of type object
                //SubscribeEnvelope resultSubscribeEnvelope = jsonPluggableLibrary.Deserialize<SubscribeEnvelope>(jsonString);
                object resultSubscribeEnvelope = PubNubInstance.JsonLibrary.DeserializeToObject(jsonString);
                SubscribeEnvelope subscribeEnvelope = new SubscribeEnvelope ();

                if (resultSubscribeEnvelope is Dictionary<string, object>) {

                    Dictionary<string, object> message = (Dictionary<string, object>)resultSubscribeEnvelope;
					subscribeEnvelope.TimetokenMeta = Helpers.CreateTimetokenMetadata (message ["t"], "Subscribe TT: ", ref this.PubNubInstance.PNLog);
					subscribeEnvelope.Messages = Helpers.CreateListOfSubscribeMessage (message ["m"], ref this.PubNubInstance.PNLog);

                    return subscribeEnvelope;
                } else {
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("ParseReceiedJSONV2: resultSubscribeEnvelope is not dict"), PNLoggingMethod.LevelError);
                    #endif

                    return null;
                }
            } else {
                return null;
            }

        }

        void ParseReceiedTimetoken (bool reconnect, long receivedTimetoken)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("ParseReceiedTimetoken: receivedTimetoken = {0}", receivedTimetoken.ToString()), PNLoggingMethod.LevelInfo);
            #endif
            lastSubscribeTimetoken = receivedTimetoken;

            //TODO 
            if (!enableResumeOnReconnect) {
                lastSubscribeTimetoken = receivedTimetoken;
            }
            else {
                //do nothing. keep last subscribe token
            }
            
            /*if (requestState.Reconnect) {
                if (enableResumeOnReconnect) {
                    //do nothing. keep last subscribe token
                }
                else {
                    lastSubscribeTimetoken = receivedTimetoken;
                }
            }*/
        }

        //private void  SubscribePresenceHanlder (CustomEventArgs<U> cea)
        private void  SubscribePresenceHanlder (CustomEventArgs cea)
        {
            //Debug.Log("in WebRequestCompleteHandler " + typeof(U));

            //try {
                
                //if ((cea != null) && (cea.CurrRequestType.Equals(PNCurrentRequestType.Subscribe))) {
                    Debug.Log("WebRequestCompleteHandler FireEvent");
                    SubscribeEnvelope resultSubscribeEnvelope = null;
                    string jsonString = cea.Message;
                    if (!jsonString.Equals("[]")) {
                        resultSubscribeEnvelope = ParseReceiedJSONV2 (jsonString);
                    
                    //resultSubscribeEnvelope.Messages
                        switch (cea.PubNubRequestState.OperationType) {
                        case PNOperationType.PNSubscribeOperation:
                        case PNOperationType.PNPresenceOperation:
                            ProcessResponse (ref resultSubscribeEnvelope, cea.PubNubRequestState);
                            if ((resultSubscribeEnvelope != null) && (resultSubscribeEnvelope.TimetokenMeta != null)) {
                                ParseReceiedTimetoken (reconnect, resultSubscribeEnvelope.TimetokenMeta.Timetoken);
                                this.region = resultSubscribeEnvelope.TimetokenMeta.Region;

                                RunSubscribeRequest (resultSubscribeEnvelope.TimetokenMeta.Timetoken, false);
                            }

                            else {
                                #if (ENABLE_PUBNUB_LOGGING)
                                this.PubNubInstance.PNLog.WriteToLog (string.Format ("ResponseCallbackNonErrorHandler ERROR: Couldn't extract timetoken, initiating fresh subscribe request. \nJSON response:\n {0}", jsonString), PNLoggingMethod.LevelError);
                                #endif
                                RunSubscribeRequest (0, false);
                            }
                            Debug.Log("WebRequestCompleteHandler ");
                            break;
                        default:
                            break;
                        }
                    }
                    
                    //cea.PubnubRequestState.ChannelEntities
                    //pns.AffectedChannels = rawChannels;
                    //pns.AffectedChannelGroups = rawChannelGroups;

                    //PNMessageResult pnmr = new PNMessageResult ("a", "b", "p", 11232234, 13431241234, null, "");

                    //PNPresenceEventResult pnper = new PNPresenceEventResult ("a", "b", "join", 11232234, 13431241234, null, null, "", 1, "");

                    
                    //TODO identify from T instead of request state
                    /*RequestState<T> requestState = cea.PubnubRequestState;        
                    Debug.Log ("inCoroutineCompleteHandler " + requestState.RespType);
                    switch(requestState.RespType){
                    case PNOperationType.PNSubscribeOperation:*/
                        //PNTimeCallback<T> timeCallback = new PNTimeCallback<T> ();

                    /*PNMessageResult pnMessageResult = new PNMessageResult();
                    pnMessageResult.Channel = cea.Message;
                        PNStatus pnStatus = new PNStatus();
                        pnStatus.Error = false;
                        /*if (pnTimeResult is T) {
                        //return (T)pnTimeResult;
                        //Callback((T)pnTimeResult, pnStatus);
                        } else {*/
                        /*try {
                            //return (T)Convert.ChangeType(pnTimeResult, typeof(T));
                            Debug.Log ("Callback");
                        Callback((SubscribeBuilder)Convert.ChangeType(pnTimeResult, typeof(SubscribeBuilder)), pnStatus);

                            Debug.Log ("After Callback");
                        } catch (InvalidCastException ice) {
                            //return default(T);
                            Debug.Log (ice.ToString());
                            throw ice;
                        }
                        //}

                        //T pnTimeResult2 = (T)pnTimeResult as object;
                        //Callback(pnTimeResult2, pnStatus);
                        //PNTimeResult pnTimeResult2 = (T)pnTimeResult;
                        //timeCallback.OnResponse(pnTimeResult, pnStatus);

                        /*if (cea.PubnubRequestState != null) {
                        ProcessCoroutineCompleteResponse<T> (cea);
                        }*/
                       /* break;
                    
                    default:
                        Debug.Log ("default");
                        break;
                    }

                    #if (ENABLE_PUBNUB_LOGGING)
                    else {
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("CoroutineCompleteHandler: PubnubRequestState null", DateTime.Now.ToString ()), PNLoggingMethod.LevelError);
                    }
                    #endif*/
                //}
                //#if (ENABLE_PUBNUB_LOGGING)
                /*else {
                    //this.PubNubInstance.PNLog.WriteToLog 
                    Debug.Log(string.Format ("CoroutineCompleteHandler: cea null"));//, PNLoggingMethod.LevelError);
                }*/
                //#endif
            /*} catch (Exception ex) {
                Debug.Log (ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("CoroutineCompleteHandler: Exception={0}",  ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
                if(cea!=null){
                    ExceptionHandler(cea.PubNubRequestState);
                } else {
                    ExceptionHandler(null);
                }

                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, cea.PubnubRequestState,
                //  false, false, PubnubErrorLevel);
            }*/
        }

        internal void ProcessResponse (ref SubscribeEnvelope resultSubscribeEnvelope, RequestState pnRequestState)
        {
            if (resultSubscribeEnvelope != null) {
                SendResponseToConnectCallback (pnRequestState);
                if (resultSubscribeEnvelope.Messages != null) {
                    ResponseToUserCallbackForSubscribe (resultSubscribeEnvelope.Messages);
                } else {
                    
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("ProcessResponseCallbacksV2: resultSubscribeEnvelope.Messages null"), PNLoggingMethod.LevelError);
                    #endif
                }
            } 
        }

        internal void SendResponseToConnectCallback (RequestState pnRequestState)
        {
            Debug.Log("SendResponseToConnectCallback");
            int count = PubNubInstance.SubscriptionInstance.ChannelsAndChannelGroupsAwaitingConnectCallback.Count;
            if(count > 0){
                bool updateIsAwaitingConnectCallback = false;
                for (int i=0; i<count; i++){
                //foreach(ChannelEntity ce in PubNubInstance.SubscriptionInstance.ChannelsAndChannelGroupsAwaitingConnectCallback){
                    ChannelEntity ce = PubNubInstance.SubscriptionInstance.ChannelsAndChannelGroupsAwaitingConnectCallback[i];
                    updateIsAwaitingConnectCallback = true;
                    //if(ce.ChannelID.IsPresenceChannel){
                        //Send presence channel conneted status
                    PNStatus pnStatus = Helpers.CreatePNStatus(
                        PNStatusCategory.PNConnectedCategory,
                        "",
                        null,
                        false,
                        PNOperationType.PNSubscribeOperation,
                        ce,
                        pnRequestState,
                        PubNubInstance
                    );

                    CreateEventArgsAndRaiseEvent(pnStatus);
                    //} else {
                        //Send channel conneted status
                    //}
                }
                if (updateIsAwaitingConnectCallback) {
                    PubNubInstance.SubscriptionInstance.UpdateIsAwaitingConnectCallbacksOfEntity (PubNubInstance.SubscriptionInstance.ChannelsAndChannelGroupsAwaitingConnectCallback, false);
                }
            }
            
        }

        internal void FindChannelEntityAndCallback (SubscribeMessage subscribeMessage, ChannelIdentity ci){
            /*ChannelEntity channelEntity = PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups.Find(x => x.ChannelID.Equals(ci));
            if (channelEntity != null) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog(string.Format("FindChannelEntityAndCallback: \n ChannelEntity : {0}  cg?={1} ispres?={2}", channelEntity.ChannelID.ChannelOrChannelGroupName, channelEntity.ChannelID.IsChannelGroup.ToString(), channelEntity.ChannelID.IsPresenceChannel.ToString()), PNLoggingMethod.LevelInfo);
                #endif*/

                //PubnubChannelCallback<T> channelCallbacks = ce.ChannelParams.Callbacks as PubnubChannelCallback<T>;
                bool isPresenceChannel  = Utility.IsPresenceChannel(subscribeMessage.Channel);

                //#if (PUBNUB_PS_V2_RESPONSE)
                /*object messageResult;
                if(isPresenceChannel)
                {
                    PNPresenceEventResult subMessageResult; 
                    CreatePNPresenceEventResult(subscribeMessage, out subMessageResult);
                    messageResult = subMessageResult;
                } else {
                    PNMessageResult subMessageResult; 
                    CreatePNMessageResult(subscribeMessage, out subMessageResult);
                    messageResult = subMessageResult;
                }*/
                /*#else
                List<object> itemMessage;
                AddMessageToListV2(cipherKey, jsonPluggableLibrary, subscribeMessage, ce, out itemMessage);
                #endif*/
                PNStatus pns = new PNStatus ();
                pns.Error = false;
                SusbcribeEventEventArgs mea = new SusbcribeEventEventArgs();
                mea.Status = pns;

                if (((subscribeMessage.SubscriptionMatch.Contains (".*")) && isPresenceChannel) || (isPresenceChannel)){
                    /*#if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog(string.Format("FindChannelEntityAndCallback: Wildcard match ChannelEntity : {0} ", channelEntity.ChannelID.ChannelOrChannelGroupName), PNLoggingMethod.LevelInfo);
                    #endif*/
                    Debug.Log("Raising presence message event ");
                    PNPresenceEventResult subMessageResult; 
                    CreatePNPresenceEventResult(subscribeMessage, out subMessageResult);
                    mea.PresenceEventResult = subMessageResult;
                    PubNubInstance.RaiseEvent (mea);
                    /*#if (PUBNUB_PS_V2_RESPONSE)
                    PubnubCallbacks.GoToCallback<T>(messageResult, channelCallbacks.WildcardPresenceCallback, jsonPluggableLibrary);
                    #else
                    PubnubCallbacks.GoToCallback<T> (itemMessage, channelCallbacks.WildcardPresenceCallback, jsonPluggableLibrary);
                    #endif*/
                } else {
                    PNMessageResult subMessageResult; 
                    CreatePNMessageResult(subscribeMessage, out subMessageResult);
                    Debug.Log("Raising message event ");
                    if(!string.IsNullOrEmpty(this.PubNubInstance.PNConfig.CipherKey) && (this.PubNubInstance.PNConfig.CipherKey.Length > 0)){
                        subMessageResult.Payload = Helpers.DecodeMessage(PubNubInstance.PNConfig.CipherKey, subMessageResult.Payload, PNOperationType.PNSubscribeOperation, ref this.PubNubInstance);
                    } 
                    mea.MessageResult = subMessageResult;
                    PubNubInstance.RaiseEvent (mea);
                    /*if(channelCallbacks!=null){
                        #if (PUBNUB_PS_V2_RESPONSE)
                            //PubnubCallbacks.GoToCallback<T>(messageResult, channelCallbacks.SuccessCallback, jsonPluggableLibrary);
                        #else
                            //PubnubCallbacks.GoToCallback<T> (itemMessage, channelCallbacks.SuccessCallback, jsonPluggableLibrary);
                        #endif
                    }else{
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PubNubInstance.PNLog.WriteToLog(string.Format("DateTime {0}, FindChannelEntityAndCallback: channelCallbacks null ", DateTime.Now.ToString()
                        ), PNLoggingMethod.LevelInfo);
                        #endif
                    }*/
                }
                //Debug.Log("cea"+ cea.Message);
                
                
                /*mea.pnMessageResult = pnmr;
                mea.pnPresenceEventResult = pnper;*/
                

                

            /*}
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                StringBuilder sbChannelEntity = new StringBuilder();
                foreach( ChannelEntity channelEntity in channelEntity){
                    sbChannelEntity.AppendFormat("ChannelEntity : {0} cg?={1} ispres?={2}", channelEntity.ChannelID.ChannelOrChannelGroupName,
                        channelEntity.ChannelID.IsChannelGroup.ToString(),
                        channelEntity.ChannelID.IsPresenceChannel.ToString()
                    );
                }
                this.PubNubInstance.PNLog.WriteToLog(string.Format("FindChannelEntityAndCallback: ChannelEntity : null ci.name {0} \nChannelEntities: \n {1}", ci.ChannelOrChannelGroupName, sbChannelEntity.ToString()), PNLoggingMethod.LevelInfo);
            }
            #endif*/

        }

        internal void ResponseToUserCallbackForSubscribe(List<SubscribeMessage> subscribeMessages)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format("In ResponseToUserCallbackForSubscribeV2"), PNLoggingMethod.LevelInfo);
            #endif
            if (subscribeMessages.Count >= this.PubNubInstance.PNConfig.MessageQueueOverflowCount)
            {
                //TODO
                /*StatusBuilder statusBuilder = new StatusBuilder(pubnubConfig, jsonLib);
                PNStatus status = statusBuilder.CreateStatusResponse(type, PNStatusCategory.PNRequestMessageCountExceededCategory, asyncRequestState, (int)HttpStatusCode.OK, null);
                Announce(status);*/
            }
             
            foreach (SubscribeMessage subscribeMessage in subscribeMessages){
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("ResponseToUserCallbackForSubscribeV2:\n SubscribeMessage:\n" +
                    "shard : {0},\n" +
                    "subscriptionMatch: {1},\n" +
                    "channel: {2},\n" +
                    "payload: {3},\n" +
                    "flags: {4},\n" +
                    "issuingClientId: {5},\n" +
                    "subscribeKey: {6},\n" +
                    "sequenceNumber: {7},\n" +
                    "originatingTimetoken tt: {8},\n" +
                    "originatingTimetoken region: {9},\n" +
                    "publishMetadata tt: {10},\n" +
                    "publishMetadata region: {11},\n" +
                    "userMetadata {12} \n",
                    subscribeMessage.Shard,
                    subscribeMessage.SubscriptionMatch,
                    subscribeMessage.Channel,
                    subscribeMessage.Payload.ToString(),
                    subscribeMessage.Flags,
                    subscribeMessage.IssuingClientId,
                    subscribeMessage.SubscribeKey,
                    subscribeMessage.SequenceNumber,
                    (subscribeMessage.OriginatingTimetoken != null) ? subscribeMessage.OriginatingTimetoken.Timetoken.ToString() : "",
                    (subscribeMessage.OriginatingTimetoken != null) ? subscribeMessage.OriginatingTimetoken.Region : "",
                    (subscribeMessage.PublishTimetokenMetadata != null) ? subscribeMessage.PublishTimetokenMetadata.Timetoken.ToString() : "",
                    (subscribeMessage.PublishTimetokenMetadata  != null) ? subscribeMessage.PublishTimetokenMetadata.Region : "",
                    (subscribeMessage.UserMetadata != null) ? subscribeMessage.UserMetadata.ToString() : "null"),
                    PNLoggingMethod.LevelInfo);
                #endif

                bool isPresenceChannel = Utility.IsPresenceChannel(subscribeMessage.Channel);
                if (string.IsNullOrEmpty(subscribeMessage.SubscriptionMatch) || subscribeMessage.Channel.Equals (subscribeMessage.SubscriptionMatch)) {
                    //channel

                    ChannelIdentity ci = new ChannelIdentity (subscribeMessage.Channel, false, isPresenceChannel);
                    FindChannelEntityAndCallback (subscribeMessage, ci);
                } else if (subscribeMessage.SubscriptionMatch.Contains (".*") && isPresenceChannel){
                    //wildcard presence channel

                    ChannelIdentity ci = new ChannelIdentity (subscribeMessage.SubscriptionMatch, false, false);
                    FindChannelEntityAndCallback (subscribeMessage, ci);
                } else if (subscribeMessage.SubscriptionMatch.Contains (".*")){
                    //wildcard channel
                    ChannelIdentity ci = new ChannelIdentity (subscribeMessage.SubscriptionMatch, false, isPresenceChannel);
                    FindChannelEntityAndCallback (subscribeMessage, ci);

                } else {
                    ChannelIdentity ci = new ChannelIdentity(subscribeMessage.SubscriptionMatch, true, isPresenceChannel);
                    FindChannelEntityAndCallback (subscribeMessage, ci);

                    //ce will be the cg and subscriptionMatch will have the cg name
                }

            }
        }


        /*private void RetryLoop<T> (RequestState<T> pubnubRequestState)
        {
            internetStatus = false;
            retryCount++;
            if (retryCount <= NetworkCheckMaxRetries) {
                string cbMessage = string.Format ("Internet Disconnected, retrying. Retry count {0} of {1}",
                    retryCount.ToString (), NetworkCheckMaxRetries);
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format("RetryLoop: {1}",  cbMessage), PNLoggingMethod.LevelError);
                #endif
                PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (cbMessage, pubnubRequestState,
                    PubnubErrorSeverity.Warn, PubnubErrorCode.NoInternetRetryConnect, PubnubErrorLevel);

            } else {
                retriesExceeded = true;
                string cbMessage = string.Format ("Internet Disconnected. Retries exceeded {0}. Unsubscribing connected channels.",
                    NetworkCheckMaxRetries);
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format("RetryLoop: {1}",  cbMessage), PNLoggingMethod.LevelError);
                #endif

                //stop heartbeat.
                StopHeartbeat<T>();
                //reset internetStatus
                ResetInternetCheckSettings();

                coroutine.BounceRequest<T> (CurrentRequestType.Subscribe, null, false);

                PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (cbMessage, pubnubRequestState,
                    PubnubErrorSeverity.Warn, PubnubErrorCode.NoInternetRetryConnect, PubnubErrorLevel);


                MultiplexExceptionHandler<T> (ResponseType.SubscribeV2, true, false);
            }
        }*/

        #region "Heartbeats"

        /*void StopHeartbeat ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("DateTime {0}, Stopping Heartbeat ", DateTime.Now.ToString ()), PNLoggingMethod.LevelInfo);
            #endif
            keepHearbeatRunning = false;
            isHearbeatRunning = false;
            hbWorker.StopHeartbeat();
            //webRequest.HeartbeatCoroutineComplete -= CoroutineCompleteHandler<PNOperationType.PNHeartbeatOperation>;
            //webRequest.AbortRequest<> (CurrentRequestType.Heartbeat, null, false);
        }

        void StopPresenceHeartbeat ()
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("DateTime {0}, Stopping PresenceHeartbeat ", DateTime.Now.ToString ()), PNLoggingMethod.LevelInfo);
            #endif
            keepPresenceHearbeatRunning = false;
            isPresenceHearbeatRunning = false;

            phbWorker.StopPresenceHeartbeat();
            //webRequest.PresenceHeartbeatCoroutineComplete -= CoroutineCompleteHandler<T>;
            //webRequest.AbortRequest<> (CurrentRequestType.PresenceHeartbeat, null, false);
        }*/

        public void CreateEventArgsAndRaiseEvent(PNStatus pnStatus){
            SusbcribeEventEventArgs mea = new SusbcribeEventEventArgs();
            mea.Status = pnStatus;

            PubNubInstance.RaiseEvent (mea);
        }

        protected void ExceptionHandler (RequestState pnRequestState)
        {
            List<ChannelEntity> channelEntities = PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("InExceptionHandler: responsetype"), PNLoggingMethod.LevelInfo);
            string channelGroups = Helpers.GetNamesFromChannelEntities (channelEntities, true);
            string channels = Helpers.GetNamesFromChannelEntities (channelEntities, false);

            #endif

            
            if (!internetStatus) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("ExceptionHandler: Subscribe channels = {0} and channelgroups = {1} - No internet connection. ", channels, channelGroups), PNLoggingMethod.LevelInfo);
                #endif
                if(this.PubNubInstance.PNConfig.ReconnectionPolicy.Equals(PNReconnectionPolicy.NONE)){
                    PNStatus pnStatus = Helpers.CreatePNStatus(
                            PNStatusCategory.PNDisconnectedCategory,
                            "",
                            null,
                            true,
                            PNOperationType.PNSubscribeOperation,
                            PubNubInstance.SubscriptionInstance.AllChannels,
                            PubNubInstance.SubscriptionInstance.AllChannelGroups,
                            null,
                            this.PubNubInstance
                        );

                    CreateEventArgsAndRaiseEvent(pnStatus);
                }
                return;
            }

            long tt = lastSubscribeTimetoken;
            if (!enableResumeOnReconnect && reconnect) {
                tt =0; //send 0 time token to enable presence event
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("ExceptionHandler: Reconnect true and EnableResumeOnReconnect false sending tt = 0. "), PNLoggingMethod.LevelInfo);
                #endif

            }
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("ExceptionHandler: sending tt = {0}. ", tt.ToString()), PNLoggingMethod.LevelInfo);
            }
            #endif


            RunSubscribeRequest (tt, reconnect);

            
        }

        /*private void HandleMultiplexException<T> (object sender, EventArgs ea)
        {
            ExceptionHandlerEventArgs<T> mea = ea as ExceptionHandlerEventArgs<T>;
            ExceptionHandler (mea.reconnectMaxTried);
        }*/

        /*private void ProcessCoroutineCompleteResponse<T> (CustomEventArgs<T> cea)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("DateTime {0}, ProcessCoroutineCompleteResponse: In handler of event cea {1} RequestType CoroutineCompleteHandler {2}", DateTime.Now.ToString (), cea.PubnubRequestState.RespType.ToString (), typeof(T)), PNLoggingMethod.LevelInfo);
            #endif
            switch (cea.PubnubRequestState.RespType) {
            case ResponseType.Heartbeat:

                HeartbeatHandler<T> (cea);

                break;

            case ResponseType.PresenceHeartbeat:

                PresenceHeartbeatHandler<T> (cea);

                break;
            default:

                SubscribePresenceHanlder<T> (cea);

                break;
            }
        }*/

        private void WebRequestCompleteHandler (object sender, EventArgs ea)
        {
            //CustomEventArgs<U> cea = ea as CustomEventArgs<U>;
            CustomEventArgs cea = ea as CustomEventArgs;

            try {
                if ((cea != null) && (cea.CurrRequestType.Equals(PNCurrentRequestType.Subscribe))) {
                    PNStatus pnStatus;
                    if (Helpers.CheckErrorTypeAndCallback<U>(cea, this.PubNubInstance, out pnStatus)){
                        Debug.Log("Is Error true");
                        ExceptionHandler(cea.PubNubRequestState);
                        
                        CreateEventArgsAndRaiseEvent(pnStatus);
                    } else {
                        SubscribePresenceHanlder (cea);    
                    }
                    /*}
                    #if (ENABLE_PUBNUB_LOGGING)
                    else {
                        this.PubNubInstance.PNLog.WriteToLog (string.Format ("DateTime {0}, WebRequestCompleteHandler: PubnubRequestState null", DateTime.Now.ToString ()), PNLoggingMethod.LevelError);
                    }
                    #endif*/
                }
                /*else {
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog (string.Format ("WebRequestCompleteHandler: cea null"), PNLoggingMethod.LevelError);                    
                    #endif
                    ExceptionHandler(cea.PubNubRequestState);
                }*/
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("WebRequestCompleteHandler: Exception={0}", ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
                ExceptionHandler(cea.PubNubRequestState);
                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, cea.PubnubRequestState, false, false, PubnubErrorLevel);
            }
        }

        #endregion

        internal void CreatePNMessageResult(SubscribeMessage subscribeMessage, out PNMessageResult messageResult)
        {
            long timetoken = (subscribeMessage.PublishTimetokenMetadata != null) ? subscribeMessage.PublishTimetokenMetadata.Timetoken : 0;
            long originatingTimetoken = (subscribeMessage.OriginatingTimetoken != null) ? subscribeMessage.OriginatingTimetoken.Timetoken : 0;
            messageResult = new PNMessageResult (
                subscribeMessage.SubscriptionMatch.Replace(Utility.PresenceChannelSuffix, ""), 
                subscribeMessage.Channel.Replace(Utility.PresenceChannelSuffix, ""), 
                subscribeMessage.Payload, 
                timetoken,
                originatingTimetoken,
                subscribeMessage.UserMetadata,
                subscribeMessage.IssuingClientId
            );

        }

        internal PNPresenceEvent CreatePNPresenceEvent (object payload)
        {
            Dictionary<string, object> pnPresenceEventDict = (Dictionary<string, object>)payload;
            string log = "";
            int occupancy = 0;
            if(Utility.CheckKeyAndParseInt(pnPresenceEventDict, "occupancy", "occupancy", out log, out occupancy)){
                Debug.Log("occupancy:" + occupancy);
            }
            PNPresenceEvent pnPresenceEvent = new PNPresenceEvent (
                (pnPresenceEventDict.ContainsKey("action"))?pnPresenceEventDict["action"].ToString():"",
                (pnPresenceEventDict.ContainsKey("uuid"))?pnPresenceEventDict["uuid"].ToString():"",
                occupancy,
                Utility.CheckKeyAndParseLong(pnPresenceEventDict, "timestamp", "timestamp", out log),
                (pnPresenceEventDict.ContainsKey("state"))?pnPresenceEventDict["state"]:null,
                Utility.CheckKeyAndConvertObjToStringArr((pnPresenceEventDict.ContainsKey("join"))?pnPresenceEventDict["join"]:null),
                Utility.CheckKeyAndConvertObjToStringArr((pnPresenceEventDict.ContainsKey("leave"))?pnPresenceEventDict["leave"]:null),
                Utility.CheckKeyAndConvertObjToStringArr((pnPresenceEventDict.ContainsKey("timeout"))?pnPresenceEventDict["timeout"]:null)
            );
            //"action": "join", "timestamp": 1473952169, "uuid": "a7acb27c-f1da-4031-a2cc-58656196b06d", "occupancy": 1
            //"action": "interval", "timestamp": 1490700797, "occupancy": 3, "join": ["Client-odx4y", "test"]

            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("Action: {0} \nTimestamp: {1} \nOccupancy: {2}\nUUID: {3}", pnPresenceEvent.Action, pnPresenceEvent.Timestamp, pnPresenceEvent.Occupancy, pnPresenceEvent.UUID), PNLoggingMethod.LevelInfo);
            #endif

            return pnPresenceEvent;
        }

        internal void CreatePNPresenceEventResult(SubscribeMessage subscribeMessage, out PNPresenceEventResult messageResult)
        {
            long timetoken = (subscribeMessage.PublishTimetokenMetadata != null) ? subscribeMessage.PublishTimetokenMetadata.Timetoken : 0;
            PNPresenceEvent pnPresenceEvent = CreatePNPresenceEvent(subscribeMessage.Payload);

            messageResult = new PNPresenceEventResult (
                subscribeMessage.SubscriptionMatch.Replace(Utility.PresenceChannelSuffix, ""), 
                subscribeMessage.Channel.Replace(Utility.PresenceChannelSuffix, ""), 
                pnPresenceEvent.Action,
                timetoken,
                pnPresenceEvent.Timestamp,
                subscribeMessage.UserMetadata,
                pnPresenceEvent.State,
                pnPresenceEvent.UUID,
                pnPresenceEvent.Occupancy,
                subscribeMessage.IssuingClientId,
                pnPresenceEvent.Join,
                pnPresenceEvent.Leave,
                pnPresenceEvent.Timeout
                );
        }

    }
}

