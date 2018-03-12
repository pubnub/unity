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
        private readonly PNUnityWebRequest webRequest;
        private readonly PubNubUnity PubNubInstance;

        private readonly HeartbeatWorker hbWorker;
        private readonly PresenceHeartbeatWorker phbWorker;
        private string webRequestId = ""; 

        private bool reconnect = false;
        private bool internetStatus = true;

        readonly bool enableResumeOnReconnect;


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
            reconnect = true;
            internetStatus = true;
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog("Internet available", PNLoggingMethod.LevelInfo);
            #endif            

            if(
                PubNubInstance.PNConfig.HeartbeatNotificationOption.Equals(PNHeartbeatNotificationOption.All)
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
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog("Internet disconnected", PNLoggingMethod.LevelInfo);
            #endif            
            
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
            BounceRequest();

            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog("Retries exceeded", PNLoggingMethod.LevelInfo);
            #endif            
             
            hbWorker.ResetInternetCheckSettings();

            #if (ENABLE_PUBNUB_LOGGING)
            List<ChannelEntity> channelEntities = PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;
            string channelGroups = Helpers.GetNamesFromChannelEntities (channelEntities, true);
            string channels = Helpers.GetNamesFromChannelEntities (channelEntities, false);
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("ExceptionHandler: MAX retries reached. Exiting the subscribe for channels = {0} and channelgroups = {1}", channels, channelGroups), PNLoggingMethod.LevelInfo);
            #endif

            UnsubscribeAllBuilder unsubBuilder = new UnsubscribeAllBuilder(this.PubNubInstance);
            unsubBuilder.Async((result, status) => {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog("in UnsubscribeAll", PNLoggingMethod.LevelInfo);
                if(status.Error){
                    this.PubNubInstance.PNLog.WriteToLog (string.Format("In Example, UnsubscribeAll Error: {0} {1} {2}", status.StatusCode, status.ErrorData, status.Category), PNLoggingMethod.LevelInfo);
                } else {
                    this.PubNubInstance.PNLog.WriteToLog (string.Format("In UnsubscribeAll, result: {0}", result.Message), PNLoggingMethod.LevelInfo);
                }
                #endif            
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

        public void BounceRequest(){
            AbortPreviousRequest(null);
            ContinueToSubscribeRestOfChannels();
        }

        public void AbortPreviousRequest(List<ChannelEntity> existingChannels)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format("AbortPreviousRequest: Aborting previous subscribe/presence requests having channel(s)={0} and ChannelGroup(s) = {1}", Helpers.GetNamesFromChannelEntities(existingChannels, false), Helpers.GetNamesFromChannelEntities(existingChannels, true)), PNLoggingMethod.LevelInfo);
            #endif

            webRequest.AbortRequest(webRequestId, false);
        }

        public void ContinueToSubscribeRestOfChannels()
        {
            List<ChannelEntity> subscribedChannels = this.PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;

            if (subscribedChannels != null && subscribedChannels.Count > 0)
            {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("ContinueToSubscribeRestOfChannels: {0}", subscribedChannels.Count()), PNLoggingMethod.LevelError);
                #endif                
                
                hbWorker.ResetInternetCheckSettings();
                RunSubscribeRequest (0, false);
            }
            else
            {
                hbWorker.StopHeartbeat();
                phbWorker.StopPresenceHeartbeat();
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog(string.Format("ContinueToSubscribeRestOfChannels: All channels are Unsubscribed. Further subscription was stopped"), PNLoggingMethod.LevelInfo);
                #endif
            }
        }

        public void Add (long timetokenToUse, List<ChannelEntity> existingChannels){
            //Abort existing request
            try{
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog("after add", PNLoggingMethod.LevelInfo);
                #endif            
                
                internetStatus = true;
                if (internetStatus) {

                    if (!timetokenToUse.Equals (0)) {
                        lastSubscribeTimetokenForNewMultiplex = timetokenToUse;
                    } else if (existingChannels.Count > 0) {
                        lastSubscribeTimetokenForNewMultiplex = lastSubscribeTimetoken;
                    }
                    AbortPreviousRequest (existingChannels);
                    RunSubscribeRequest (0, false);
                }
                
            }catch (Exception ex){
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("ex.ToString(): {0}", ex.ToString()), PNLoggingMethod.LevelError);
                #endif                                
            }
        }

        private bool CheckAllChannelsAreUnsubscribed()
        {
            if (PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups.Count <=0)
            {
                hbWorker.StopHeartbeat();
                phbWorker.StopPresenceHeartbeat();

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
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog("in  RunSubscribeRequest", PNLoggingMethod.LevelInfo);
            #endif            
            
            if (CheckAllChannelsAreUnsubscribed())
            {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog("All channels unsubscribed", PNLoggingMethod.LevelInfo);
                #endif            
                
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
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("MultiChannelSubscribeRequest: Building request for {0} with timetoken={1}", Helpers.GetAllNamesFromChannelEntities(channelEntities, true), lastTimetoken), PNLoggingMethod.LevelInfo);
                #endif
                // Build URL
				string channelsJsonState = PubNubInstance.SubscriptionInstance.CompiledUserState;

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
                    this.PubNubInstance
                );

                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("RunSubscribeRequest: {0}", requestUrl.OriginalString), PNLoggingMethod.LevelError);
                #endif                

                RequestState requestState = new RequestState ();
                requestState.OperationType = PNOperationType.PNSubscribeOperation;
                requestState.URL = requestUrl.OriginalString; 
                requestState.Timeout = PubNubInstance.PNConfig.SubscribeTimeout;
                requestState.Pause = 0;
                requestState.Reconnect = reconnect;
                //http://ps.pndsn.com/v2/presence/sub-key/sub-c-5c4fdcc6-c040-11e5-a316-0619f8945a4f/uuid/UUID_WhereNow?pnsdk=PubNub-Go%2F3.14.0&uuid=UUID_WhereNow
                webRequestId = webRequest.Run(requestState);

            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("MultiChannelSubscribeRequest: method:_subscribe \n channel={0} \n timetoken={1} \n Exception Details={2}", Helpers.GetAllNamesFromChannelEntities(channelEntities, true), timetoken.ToString (), ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
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
					subscribeEnvelope.TimetokenMeta = Helpers.CreateTimetokenMetadata (message ["t"], "Subscribe TT: ", this.PubNubInstance.PNLog);
					subscribeEnvelope.Messages = Helpers.CreateListOfSubscribeMessage (message ["m"], this.PubNubInstance.PNLog);

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
        }

        private void  SubscribePresenceHanlder (CustomEventArgs cea)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog("WebRequestCompleteHandler FireEvent", PNLoggingMethod.LevelInfo);
            #endif            
            
            SubscribeEnvelope resultSubscribeEnvelope = null;
            string jsonString = cea.Message;
            if (!jsonString.Equals("[]")) {
                resultSubscribeEnvelope = ParseReceiedJSONV2 (jsonString);
            
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
                    #if (ENABLE_PUBNUB_LOGGING)
                    this.PubNubInstance.PNLog.WriteToLog("WebRequestCompleteHandler", PNLoggingMethod.LevelInfo);
                    #endif            
                    
                    break;
                default:
                    break;
                }
            }
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
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog("SendResponseToConnectCallback", PNLoggingMethod.LevelInfo);
            #endif            
            
            int count = PubNubInstance.SubscriptionInstance.ChannelsAndChannelGroupsAwaitingConnectCallback.Count;
            if(count > 0){
                bool updateIsAwaitingConnectCallback = false;
                for (int i=0; i<count; i++){
                    ChannelEntity ce = PubNubInstance.SubscriptionInstance.ChannelsAndChannelGroupsAwaitingConnectCallback[i];
                    updateIsAwaitingConnectCallback = true;
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
                }
                if (updateIsAwaitingConnectCallback) {
                    PubNubInstance.SubscriptionInstance.UpdateIsAwaitingConnectCallbacksOfEntity (PubNubInstance.SubscriptionInstance.ChannelsAndChannelGroupsAwaitingConnectCallback, false);
                }
            }
            
        }

        internal void FindChannelEntityAndCallback (SubscribeMessage subscribeMessage, ChannelIdentity ci){
            bool isPresenceChannel  = Utility.IsPresenceChannel(subscribeMessage.Channel);

            PNStatus pns = new PNStatus ();
            pns.Error = false;
            SusbcribeEventEventArgs mea = new SusbcribeEventEventArgs();
            mea.Status = pns;

            if (((subscribeMessage.SubscriptionMatch.Contains (".*")) && isPresenceChannel) || (isPresenceChannel)){
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog("Raising presence message event ", PNLoggingMethod.LevelInfo);
                #endif            
                
                PNPresenceEventResult subMessageResult; 
                CreatePNPresenceEventResult(subscribeMessage, out subMessageResult);
                mea.PresenceEventResult = subMessageResult;
                PubNubInstance.RaiseEvent (mea);
            } else {
                PNMessageResult subMessageResult; 
                CreatePNMessageResult(subscribeMessage, out subMessageResult);
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog("Raising message event ", PNLoggingMethod.LevelInfo);
                #endif            
                
                if(!string.IsNullOrEmpty(this.PubNubInstance.PNConfig.CipherKey) && (this.PubNubInstance.PNConfig.CipherKey.Length > 0)){
                    subMessageResult.Payload = Helpers.DecodeMessage(PubNubInstance.PNConfig.CipherKey, subMessageResult.Payload, PNOperationType.PNSubscribeOperation, this.PubNubInstance);
                } 
                mea.MessageResult = subMessageResult;
                PubNubInstance.RaiseEvent (mea);

            }
        }

        internal void ResponseToUserCallbackForSubscribe(List<SubscribeMessage> subscribeMessages)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog(string.Format("In ResponseToUserCallbackForSubscribeV2"), PNLoggingMethod.LevelInfo);
            #endif
            if (subscribeMessages.Count >= this.PubNubInstance.PNConfig.MessageQueueOverflowCount)
            {
                PNStatus pnStatus = Helpers.CreatePNStatus(
                            PNStatusCategory.PNRequestMessageCountExceededCategory,
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

        #region "Heartbeats"

        public void CreateEventArgsAndRaiseEvent(PNStatus pnStatus){
            SusbcribeEventEventArgs mea = new SusbcribeEventEventArgs();
            mea.Status = pnStatus;

            PubNubInstance.RaiseEvent (mea);
        }

        protected void ExceptionHandler (RequestState pnRequestState)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            List<ChannelEntity> channelEntities = PubNubInstance.SubscriptionInstance.AllSubscribedChannelsAndChannelGroups;
            
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

        private void WebRequestCompleteHandler (object sender, EventArgs ea)
        {
            CustomEventArgs cea = ea as CustomEventArgs;

            try {
                if ((cea != null) && (cea.CurrRequestType.Equals(PNCurrentRequestType.Subscribe))) {
                    PNStatus pnStatus;
                    if (Helpers.TryCheckErrorTypeAndCallback<U>(cea, this.PubNubInstance, out pnStatus)){
                        #if (ENABLE_PUBNUB_LOGGING)
                        this.PubNubInstance.PNLog.WriteToLog("Is Error true", PNLoggingMethod.LevelInfo);
                        #endif            
                        
                        ExceptionHandler(cea.PubNubRequestState);
                        
                        CreateEventArgsAndRaiseEvent(pnStatus);
                    } else {
                        SubscribePresenceHanlder (cea);    
                    }

                }
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog (string.Format ("WebRequestCompleteHandler: Exception={0}", ex.ToString ()), PNLoggingMethod.LevelError);
                #endif
                ExceptionHandler(cea.PubNubRequestState);
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
            if(Utility.TryCheckKeyAndParseInt(pnPresenceEventDict, "occupancy", "occupancy", out log, out occupancy)){
                #if (ENABLE_PUBNUB_LOGGING)
                this.PubNubInstance.PNLog.WriteToLog(string.Format("occupancy: {0}", occupancy), PNLoggingMethod.LevelInfo);
                #endif            
            }
            long timetoken;
            Utility.TryCheckKeyAndParseLong(pnPresenceEventDict, "timestamp", "timestamp", out log, out timetoken);
            bool hereNowRefresh = false;
            if(pnPresenceEventDict.ContainsKey("here_now_refresh")){
                hereNowRefresh = (bool)pnPresenceEventDict["here_now_refresh"];
            }

            PNPresenceEvent pnPresenceEvent = new PNPresenceEvent (
                (pnPresenceEventDict.ContainsKey("action"))?pnPresenceEventDict["action"].ToString():"",
                (pnPresenceEventDict.ContainsKey("uuid"))?pnPresenceEventDict["uuid"].ToString():"",
                occupancy,
                timetoken,
                (pnPresenceEventDict.ContainsKey("state"))?pnPresenceEventDict["state"]:null,
                Utility.CheckKeyAndConvertObjToStringArr((pnPresenceEventDict.ContainsKey("join"))?pnPresenceEventDict["join"]:null),
                Utility.CheckKeyAndConvertObjToStringArr((pnPresenceEventDict.ContainsKey("leave"))?pnPresenceEventDict["leave"]:null),
                Utility.CheckKeyAndConvertObjToStringArr((pnPresenceEventDict.ContainsKey("timeout"))?pnPresenceEventDict["timeout"]:null),
                hereNowRefresh
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

