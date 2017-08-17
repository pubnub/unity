using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PubNubAPI
{
    internal class SusbcribeEventEventArgs : EventArgs
    {
        public PNStatus pnStatus;
        public PNPresenceEventResult pnPresenceEventResult;
        public PNMessageResult pnMessageResult;
    }

    public class SubscriptionWorker<U>
    {
        private PNUnityWebRequest webRequest;
        private PubNubUnity PubNubInstance { get; set;}

        private HeartbeatWorker hbWorker;
        private PresenceHeartbeatWorker phbWorker;

        //Allow one instance only        
        public SubscriptionWorker (PubNubUnity pn)
        {
            PubNubInstance = pn;
            webRequest = PubNubInstance.GameObjectRef.AddComponent<PNUnityWebRequest> ();
            webRequest.SubWebRequestComplete += WebRequestCompleteHandler;
            hbWorker = new HeartbeatWorker(pn);
            phbWorker = new PresenceHeartbeatWorker(pn);
        }

        ~SubscriptionWorker(){
            CleanUp();
        }

        public void CleanUp(){
            if (webRequest != null) {
                webRequest.SubWebRequestComplete -= WebRequestCompleteHandler;
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
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, AbortPreviousRequest: Aborting previous subscribe/presence requests having channel(s)={1} and ChannelGroup(s) = {2}",
                DateTime.Now.ToString(), Helpers.GetNamesFromChannelEntities(existingChannels, false),
                Helpers.GetNamesFromChannelEntities(existingChannels, true)), LoggingMethod.LevelInfo);
            #endif

            webRequest.AbortRequest<U>(CurrentRequestType.Subscribe, null, false);
        }

        public void ContinueToSubscribeRestOfChannels()
        {
            List<ChannelEntity> subscribedChannels = this.PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;

            if (subscribedChannels != null && subscribedChannels.Count > 0)
            {
                //TODO
                hbWorker.ResetInternetCheckSettings();
                MultiChannelSubscribeRequest (PNOperationType.PNSubscribeOperation, 0, false);

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
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ContinueToSubscribeRestOfChannels: reqState none matched", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
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
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, ContinueToSubscribeRestOfChannels: All channels are Unsubscribed. Further subscription was stopped", DateTime.Now.ToString()), LoggingMethod.LevelInfo);
                #endif
                //ExceptionHandlers.MultiplexException -= HandleMultiplexException<T>;
            }
        }

        //public void Add (PNOperationType pnOpType, object pnBuilder, RequestState<SubscribeRequestBuilder> reqState){
        public void Add (PNOperationType pnOpType, long timetokenToUse, List<ChannelEntity> existingChannels){
            //Abort existing request
            try{
                //Debug.Log("in add:" + reqState.Reconnect + this.PubNubInstance.Test);
                //SubscribeRequestBuilder subscribeBuilder = (SubscribeRequestBuilder)pnBuilder;

                //reconnect = false;
                Debug.Log("after add");

                bool internetStatus = true;
                if (internetStatus) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    Helpers.LogChannelEntitiesDictionary ();
                    #endif

                    if (!timetokenToUse.Equals (0)) {
                        lastSubscribeTimetokenForNewMultiplex = timetokenToUse;
                    } else if (existingChannels.Count > 0) {
                        lastSubscribeTimetokenForNewMultiplex = lastSubscribeTimetoken;
                    }
                    AbortPreviousRequest (existingChannels);
                    MultiChannelSubscribeRequest (pnOpType, 0, false);
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
            LoggingMethod.WriteToLog (string.Format ("MultiChannelSubscribeInit: channelsOrChannelGroupsAdded {1}, internet status {2}",
             channelsOrChannelGroupsAdded.ToString (), internetStatus.ToString ()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
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
                LoggingMethod.WriteToLog(string.Format("CheckAllChannelsAreUnsubscribed: All channels are Unsubscribed. Further subscription was stopped", DateTime.Now.ToString()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
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
                if (!sentTimetoken.Equals(0) && !lastSubscribeTimetokenForNewMultiplex.Equals(0) && !lastSubscribeTimetoken.Equals(lastSubscribeTimetokenForNewMultiplex))
                {
                    lastTimetoken = lastSubscribeTimetokenForNewMultiplex;
                    lastSubscribeTimetokenForNewMultiplex = 0;
                    #if (ENABLE_PUBNUB_LOGGING)
                    sbLogger.AppendFormat("SaveLastTimetoken: Using lastSubscribeTimetokenForNewMultiplex={0}\n", lastTimetoken);
                    #endif
                }
                else
                    if (sentTimetoken.Equals(0))
                    {
                        lastTimetoken = sentTimetoken;
                        #if (ENABLE_PUBNUB_LOGGING)
                        sbLogger.AppendFormat("SaveLastTimetoken: Using sentTimetoken={0}\n", sentTimetoken);
                        #endif
                    }
                    else
                    {
                        lastTimetoken = sentTimetoken;
                        #if (ENABLE_PUBNUB_LOGGING)
                        sbLogger.AppendFormat("SaveLastTimetoken: Using sentTimetoken={0}\n", sentTimetoken);
                        #endif
                    }
                if (lastSubscribeTimetoken.Equals(lastSubscribeTimetokenForNewMultiplex))
                {
                    lastSubscribeTimetokenForNewMultiplex = 0;
                }
            }
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("{1} ", 
            sbLogger.ToString()), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
            #endif

            return lastTimetoken;
        }

        private void MultiChannelSubscribeRequest (PNOperationType type, long timetoken, bool reconnect)
        {
            //Exit if the channel is unsubscribed
            Debug.Log("in  MultiChannelSubscribeRequest");
            if (CheckAllChannelsAreUnsubscribed())
            {
                //return;
            }
			List<ChannelEntity> channelEntities = PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;

            // Begin recursive subscribe
            try {
                long lastTimetoken = SaveLastTimetoken(timetoken);

                hbWorker.RunHeartbeat (false, PubNubInstance.PNConfig.HeartbeatInterval);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, RunRequests: Heartbeat started", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                #endif
                if (PubNubInstance.PNConfig.PresenceInterval > 0){
                    phbWorker.RunPresenceHeartbeat(false, PubNubInstance.PNConfig.PresenceInterval);
                }

                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("MultiChannelSubscribeRequest: Building request for {1} with timetoken={2}",
                    Helpers.GetNamesFromChannelEntities(channelEntities), lastTimetoken), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
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
                    this.PubNubInstance.PNConfig.UUID, 
                    region,
                    filterExpr, 
                    true, 
                    PubNubInstance.PNConfig.Origin, 
                    this.PubNubInstance.PNConfig.AuthKey, 
                    PubNubInstance.PNConfig.SubscribeKey, 
                    this.PubNubInstance.PNConfig.PresenceTimeout,
                    PubNubInstance.Version
                );
                
                /*Uri requestUrl = BuildRequests.BuildMultiChannelSubscribeRequest (channels,
                    channelGroups, lastTimetoken.ToString(), channelsJsonState, this.SessionUUID, this.Region,
                    filterExpr, this.ssl, this.Origin, authenticationKey, this.subscribeKey, this.PresenceHeartbeat);*/


                //RequestState<T> pubnubRequestState = BuildRequests.BuildRequestState<T> (channelEntities, type, reconnect,
                    //0, false, Convert.ToInt64 (timetoken.ToString ()), typeof(T));
                // Wait for message
                //ExceptionHandlers.MultiplexException += HandleMultiplexException<T>;

                //UrlProcessRequest<T> (requestUrl, pubnubRequestState);
                Debug.Log ("RunSubscribeRequest coroutine" + requestUrl.OriginalString);

                RequestState<SubscribeEnvelope> requestState = new RequestState<SubscribeEnvelope> ();
                //requestState.ChannelEntities = channelEntities;
                requestState.RespType = PNOperationType.PNSubscribeOperation;
                //requestState.ChannelEntities = channelEntities;

                //PNCallback<T> timeCallback = new PNTimeCallback<T> (callback);
                //http://ps.pndsn.com/v2/presence/sub-key/sub-c-5c4fdcc6-c040-11e5-a316-0619f8945a4f/uuid/UUID_WhereNow?pnsdk=PubNub-Go%2F3.14.0&uuid=UUID_WhereNow
                webRequest.Run<SubscribeEnvelope>(requestUrl.OriginalString, requestState, 310, 0, reconnect);

            } catch (Exception ex) {
                Debug.Log("in  MultiChannelSubscribeRequest" + ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("MultiChannelSubscribeRequest: method:_subscribe \n channel={1} \n timetoken={2} \n Exception Details={3}",
                 Helpers.GetNamesFromChannelEntities(channelEntities), timetoken.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
                //PubnubCallbacks.CallErrorCallback<T> (ex, channelEntities,
                  //  PubnubErrorCode.None, PubnubErrorSeverity.Critical, PubnubErrorLevel);

                this.MultiChannelSubscribeRequest (type, timetoken, false);
            }
        }

        SubscribeEnvelope ParseReceiedJSONV2 (RequestState<U> requestState, string jsonString)
        {
            if (!string.IsNullOrEmpty (jsonString)) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("ParseReceiedJSONV2: jsonString = {1}",  jsonString), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                #endif
                
                //this doesnt work on JSONFx for Unity in case a string is passed in an variable of type object
                //SubscribeEnvelope resultSubscribeEnvelope = jsonPluggableLibrary.Deserialize<SubscribeEnvelope>(jsonString);
                object resultSubscribeEnvelope = PubNubInstance.JsonLibrary.DeserializeToObject(jsonString);
                SubscribeEnvelope subscribeEnvelope = new SubscribeEnvelope ();

                if (resultSubscribeEnvelope is Dictionary<string, object>) {

                    Dictionary<string, object> message = (Dictionary<string, object>)resultSubscribeEnvelope;
					subscribeEnvelope.TimetokenMeta = Helpers.CreateTimetokenMetadata (message ["t"], "Subscribe TT: ", PubNubInstance.PNConfig.LogVerbosity);
					subscribeEnvelope.Messages = Helpers.CreateListOfSubscribeMessage (message ["m"], PubNubInstance.PNConfig.LogVerbosity);

                    return subscribeEnvelope;
                } else {
                    #if (ENABLE_PUBNUB_LOGGING)

                    LoggingMethod.WriteToLog (string.Format ("ParseReceiedJSONV2: resultSubscribeEnvelope is not dict",
                        DateTime.Now.ToString ()), LoggingMethod.LevelError);

                    #endif

                    return null;
                }
            } else {
                return null;
            }

        }

        void ParseReceiedTimetoken (RequestState<U> requestState, long receivedTimetoken)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("ParseReceiedTimetoken: receivedTimetoken = {1}",
             receivedTimetoken.ToString()),
            LoggingMethod.LevelInfo);
            #endif
            lastSubscribeTimetoken = receivedTimetoken;

            bool enableResumeOnReconnect = this.PubNubInstance.PNConfig.ReconnectionPolicy.Equals(PNReconnectionPolicy.LINEAR) | this.PubNubInstance.PNConfig.ReconnectionPolicy.Equals(PNReconnectionPolicy.EXPONENTIAL);

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

        private void SubscribePresenceHanlder (CustomEventArgs<U> cea)
        {
            Debug.Log("in WebRequestCompleteHandler " + typeof(U));

            try {
                
                if (cea != null) {
                    Debug.Log("WebRequestCompleteHandler FireEvent");
                    SubscribeEnvelope resultSubscribeEnvelope = null;
                    string jsonString = cea.Message;
                    if (!jsonString.Equals("[]")) {
                        resultSubscribeEnvelope = ParseReceiedJSONV2 (cea.PubnubRequestState, jsonString);
                    }
                    //resultSubscribeEnvelope.Messages

                    switch (cea.PubnubRequestState.RespType) {
                    case PNOperationType.PNSubscribeOperation:
                    case PNOperationType.PNPresenceOperation:
                        ProcessResponse (ref resultSubscribeEnvelope);
                        if ((resultSubscribeEnvelope != null) && (resultSubscribeEnvelope.TimetokenMeta != null)) {
                            ParseReceiedTimetoken (cea.PubnubRequestState, resultSubscribeEnvelope.TimetokenMeta.Timetoken);
                            this.region = resultSubscribeEnvelope.TimetokenMeta.Region;

                            MultiChannelSubscribeRequest (cea.PubnubRequestState.RespType, resultSubscribeEnvelope.TimetokenMeta.Timetoken, false);
                        }

                        else {
                            #if (ENABLE_PUBNUB_LOGGING)
                            LoggingMethod.WriteToLog (string.Format ("ResponseCallbackNonErrorHandler ERROR: Couldn't extract timetoken, initiating fresh subscribe request. \nJSON response:\n {1}",
                                 jsonString), LoggingMethod.LevelError);
                            #endif
                            MultiChannelSubscribeRequest (cea.PubnubRequestState.RespType, 0, false);
                        }
                        Debug.Log("WebRequestCompleteHandler ");
                        break;
                    default:
                        break;
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
                    LoggingMethod.WriteToLog (string.Format ("CoroutineCompleteHandler: PubnubRequestState null", DateTime.Now.ToString ()), LoggingMethod.LevelError);
                    }
                    #endif*/
                }
                //#if (ENABLE_PUBNUB_LOGGING)
                else {
                    //LoggingMethod.WriteToLog 
                    Debug.Log(string.Format ("CoroutineCompleteHandler: cea null", DateTime.Now.ToString ()));//, LoggingMethod.LevelError);
                }
                //#endif
            } catch (Exception ex) {
                Debug.Log (ex.ToString());
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("CoroutineCompleteHandler: Exception={1}",  ex.ToString ()), LoggingMethod.LevelError);
                #endif

                //ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (ex.Message, cea.PubnubRequestState,
                //  false, false, PubnubErrorLevel);
            }
        }

        internal void ProcessResponse (ref SubscribeEnvelope resultSubscribeEnvelope)
        {
            
            if (resultSubscribeEnvelope != null) {
                //Helpers.ResponseToConnectCallback<T> (asynchRequestState, jsonPluggableLibrary);
                if (resultSubscribeEnvelope.Messages != null) {
                    ResponseToUserCallbackForSubscribe (resultSubscribeEnvelope.Messages);
                }
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessResponseCallbacksV2: resultSubscribeEnvelope.Messages null", 
                        DateTime.Now.ToString ()), LoggingMethod.LevelError);
                }
                #endif

            } 
        }

        internal void FindChannelEntityAndCallback (SubscribeMessage subscribeMessage, ChannelIdentity ci){
            ChannelEntity ce = PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups.Find(x => x.ChannelID.Equals(ci));
            if (ce != null) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, FindChannelEntityAndCallback: \n ChannelEntity : {1}  cg?={2} ispres?={3}", DateTime.Now.ToString(),
                    ce.ChannelID.ChannelOrChannelGroupName, ce.ChannelID.IsChannelGroup.ToString(),
                    ce.ChannelID.IsPresenceChannel.ToString()
                ), LoggingMethod.LevelInfo);
                #endif

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
                
                SusbcribeEventEventArgs mea = new SusbcribeEventEventArgs();
                mea.pnStatus = pns;

                if (((subscribeMessage.SubscriptionMatch.Contains (".*")) && isPresenceChannel) || (isPresenceChannel)){
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog(string.Format("DateTime {0}, FindChannelEntityAndCallback: Wildcard match ChannelEntity : {1} ", DateTime.Now.ToString(),
                        ce.ChannelID.ChannelOrChannelGroupName
                    ), LoggingMethod.LevelInfo);
                    #endif
                    Debug.Log("Raising presence message event ");
                    PNPresenceEventResult subMessageResult; 
                    CreatePNPresenceEventResult(subscribeMessage, out subMessageResult);
                    mea.pnPresenceEventResult = subMessageResult;
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
                    mea.pnMessageResult = subMessageResult;
                    PubNubInstance.RaiseEvent (mea);
                    /*if(channelCallbacks!=null){
                        #if (PUBNUB_PS_V2_RESPONSE)
                            //PubnubCallbacks.GoToCallback<T>(messageResult, channelCallbacks.SuccessCallback, jsonPluggableLibrary);
                        #else
                            //PubnubCallbacks.GoToCallback<T> (itemMessage, channelCallbacks.SuccessCallback, jsonPluggableLibrary);
                        #endif
                    }else{
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog(string.Format("DateTime {0}, FindChannelEntityAndCallback: channelCallbacks null ", DateTime.Now.ToString()
                        ), LoggingMethod.LevelInfo);
                        #endif
                    }*/
                }
                //Debug.Log("cea"+ cea.Message);
                
                
                /*mea.pnMessageResult = pnmr;
                mea.pnPresenceEventResult = pnper;*/
                

                

            }
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                StringBuilder sbChannelEntity = new StringBuilder();
                foreach( ChannelEntity channelEntity in channelEntities){
                    sbChannelEntity.AppendFormat("ChannelEntity : {0} cg?={1} ispres?={2}", channelEntity.ChannelID.ChannelOrChannelGroupName,
                        channelEntity.ChannelID.IsChannelGroup.ToString(),
                        channelEntity.ChannelID.IsPresenceChannel.ToString()
                    );
                }
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, FindChannelEntityAndCallback: ChannelEntity : null ci.name {1} \nChannelEntities: \n {2}", DateTime.Now.ToString(),
                    ci.ChannelOrChannelGroupName, sbChannelEntity.ToString()), LoggingMethod.LevelInfo);
            }
            #endif

        }

        internal void ResponseToUserCallbackForSubscribe(List<SubscribeMessage> subscribeMessages)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, In ResponseToUserCallbackForSubscribeV2", DateTime.Now.ToString()
                ), LoggingMethod.LevelInfo);
            #endif
             
            foreach (SubscribeMessage subscribeMessage in subscribeMessages){
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ResponseToUserCallbackForSubscribeV2:\n SubscribeMessage:\n" +
                    "shard : {1},\n" +
                    "subscriptionMatch: {2},\n" +
                    "channel: {3},\n" +
                    "payload: {4},\n" +
                    "flags: {5},\n" +
                    "issuingClientId: {6},\n" +
                    "subscribeKey: {7},\n" +
                    "sequenceNumber: {8},\n" +
                    "originatingTimetoken tt: {9},\n" +
                    "originatingTimetoken region: {10},\n" +
                    "publishMetadata tt: {11},\n" +
                    "publishMetadata region: {12},\n" +
                    "userMetadata {13} \n",
                    DateTime.Now.ToString (), 
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
                    LoggingMethod.LevelInfo);
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
                LoggingMethod.WriteToLog (string.Format("RetryLoop: {1}",  cbMessage), LoggingMethod.LevelError);
                #endif
                PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (cbMessage, pubnubRequestState,
                    PubnubErrorSeverity.Warn, PubnubErrorCode.NoInternetRetryConnect, PubnubErrorLevel);

            } else {
                retriesExceeded = true;
                string cbMessage = string.Format ("Internet Disconnected. Retries exceeded {0}. Unsubscribing connected channels.",
                    NetworkCheckMaxRetries);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format("RetryLoop: {1}",  cbMessage), LoggingMethod.LevelError);
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
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Stopping Heartbeat ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
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
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Stopping PresenceHeartbeat ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            #endif
            keepPresenceHearbeatRunning = false;
            isPresenceHearbeatRunning = false;

            phbWorker.StopPresenceHeartbeat();
            //webRequest.PresenceHeartbeatCoroutineComplete -= CoroutineCompleteHandler<T>;
            //webRequest.AbortRequest<> (CurrentRequestType.PresenceHeartbeat, null, false);
        }*/

        protected void MultiplexExceptionHandler (PNOperationType type, bool reconnectMaxTried, bool reconnect)
        {
            List<ChannelEntity> channelEntities = PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MultiplexExceptionHandler: responsetype={1}", DateTime.Now.ToString (), type.ToString ()), LoggingMethod.LevelInfo);
            #endif
            string channelGroups = Helpers.GetNamesFromChannelEntities (channelEntities, true);
            string channels = Helpers.GetNamesFromChannelEntities (channelEntities, false);

            if (reconnectMaxTried) {

                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MultiplexExceptionHandler: MAX retries reached. Exiting the subscribe for channels = {1} and channelgroups = {2}",
                    DateTime.Now.ToString (), channels, channelGroups), LoggingMethod.LevelInfo);
                #endif

                /*MultiChannelUnsubscribeInit<T> (ResponseType.Unsubscribe, channels, channelGroups, null, null, null, null);

                Helpers.CheckSubscribedChannelsAndSendCallbacks<T> (PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups,
                    type, NetworkCheckMaxRetries, PubnubErrorLevel);
                retriesExceeded = false;*/
            } else {
                /*if (!internetStatus) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MultiplexExceptionHandler: Subscribe channels = {1} and channelgroups = {2} - No internet connection. ",
                        DateTime.Now.ToString (), channels, channelGroups), LoggingMethod.LevelInfo);
                    #endif
                    return;
                }*/

                long tt = lastSubscribeTimetoken;
                /*if (!EnableResumeOnReconnect && reconnect) {
                    tt =0; //send 0 time token to enable presence event
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MultiplexExceptionHandler: Reconnect true and EnableResumeOnReconnect false sending tt = 0. ",
                        DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                    #endif

                }
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, MultiplexExceptionHandler: sending tt = {1}. ",
                        DateTime.Now.ToString (), tt.ToString()), LoggingMethod.LevelInfo);
                }
                #endif*/


                MultiChannelSubscribeRequest (type, tt, reconnect);

            }
        }

        private void HandleMultiplexException<T> (object sender, EventArgs ea)
        {
            /*MultiplexExceptionEventArgs<T> mea = ea as MultiplexExceptionEventArgs<T>;
            MultiplexExceptionHandler<T> (mea.responseType, mea.reconnectMaxTried, mea.resumeOnReconnect);*/
        }

        /*private void ProcessCoroutineCompleteResponse<T> (CustomEventArgs<T> cea)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessCoroutineCompleteResponse: In handler of event cea {1} RequestType CoroutineCompleteHandler {2}", DateTime.Now.ToString (), cea.PubnubRequestState.RespType.ToString (), typeof(T)), LoggingMethod.LevelInfo);
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
            CustomEventArgs<U> cea = ea as CustomEventArgs<U>;

            try {
                if (cea != null) {
                    //if (cea.PubnubRequestState != null) {
                        SubscribePresenceHanlder (cea);
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
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Action: {1} \nTimestamp: {2} \nOccupancy: {3}\nUUID: {4}", 
                DateTime.Now.ToString (), pnPresenceEvent.Action, pnPresenceEvent.Timestamp, 
                pnPresenceEvent.Occupancy,
                pnPresenceEvent.UUID
            ), 
                LoggingMethod.LevelInfo);
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

