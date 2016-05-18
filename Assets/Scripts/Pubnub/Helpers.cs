using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Collections;
using System.Text;

namespace PubNubMessaging.Core
{
    internal static class Helpers
    {
        #region "Helpers"

        internal static string BuildJsonUserState (Dictionary<string, object> userStateDictionary)
        {
            StringBuilder jsonStateBuilder = new StringBuilder ();

            if (userStateDictionary != null) {
                string[] userStateKeys = userStateDictionary.Keys.ToArray<string> ();

                for (int keyIndex = 0; keyIndex < userStateKeys.Length; keyIndex++) {
                    string useStateKey = userStateKeys [keyIndex];
                    object userStateValue = userStateDictionary [useStateKey];
                    if (userStateValue == null) {
                        jsonStateBuilder.AppendFormat ("\"{0}\":{1}", useStateKey, string.Format ("\"{0}\"", "null"));
                    } else {
                        jsonStateBuilder.AppendFormat ("\"{0}\":{1}", useStateKey, (userStateValue.GetType ().ToString () == "System.String") ? string.Format ("\"{0}\"", userStateValue) : userStateValue);
                    }
                    if (keyIndex < userStateKeys.Length - 1) {
                        jsonStateBuilder.Append (",");
                    }
                }
            }

            return jsonStateBuilder.ToString ();
        }

        internal static string BuildJsonUserState (List<ChannelEntity> ce)
        {
            string retJsonUserState = "";

            StringBuilder jsonStateBuilder = new StringBuilder ();

            if (ce != null) {
                foreach (ChannelEntity c in ce) {
                    string currentJsonState = BuildJsonUserState (c.ChannelParams.UserState);
                    if (!string.IsNullOrEmpty (currentJsonState)) {
                        currentJsonState = string.Format ("\"{0}\":{{{1}}}",c.ChannelID.ChannelOrChannelGroupName, currentJsonState);
                        if (jsonStateBuilder.Length > 0) {
                            jsonStateBuilder.Append (",");
                        }
                        jsonStateBuilder.Append (currentJsonState);
                    }
                }

                if (jsonStateBuilder.Length > 0) {
                    retJsonUserState = string.Format ("{{{0}}}", jsonStateBuilder.ToString ());
                }
            }

            return retJsonUserState;
        }

        internal static string GetNamesFromChannelEntities (List<ChannelEntity> channelEntities, bool isChannelGroup){
            
            StringBuilder sb = new StringBuilder ();
            if (channelEntities != null) {
                int count = 0;
                foreach (ChannelEntity c in channelEntities) {
                    if (count > 0) {
                        sb.Append (",");
                    }
                    if (isChannelGroup && c.ChannelID.IsChannelGroup) {
                        sb.Append (c.ChannelID.ChannelOrChannelGroupName);
                    } else if (!isChannelGroup && !c.ChannelID.IsChannelGroup) {
                        sb.Append (c.ChannelID.ChannelOrChannelGroupName);
                    }
                    count++;
                }
            }
            return sb.ToString();
        }

        internal static string GetNamesFromChannelEntities (List<ChannelEntity> channelEntities){
            StringBuilder sbCh = new StringBuilder ();
            StringBuilder sbChGrp = new StringBuilder ();
            if (channelEntities != null) {
                int countCh = 0;
                int countChGrp = 0;
                foreach (ChannelEntity c in channelEntities) {
                    if (c.ChannelID.IsChannelGroup) {
                        if (countChGrp > 0) {
                            sbChGrp.Append (",");
                        }

                        sbChGrp.Append (c.ChannelID.ChannelOrChannelGroupName);
                        countChGrp++;
                    } else {
                        if (countCh > 0) {
                            sbCh.Append (",");
                        }

                        sbCh.Append (c.ChannelID.ChannelOrChannelGroupName);
                        countCh++;
                    }

                }
            }
            return string.Format ("channel(s) = {0} and channelGroups(s) = {1}", sbCh.ToString(), sbChGrp.ToString());
        }

        internal static bool UpdateOrAddUserStateOfEntity<T>(string channel, bool isChannelGroup, Dictionary<string, object> userState,
            Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel, ref List<ChannelEntity> channelEntities)
        {
            ChannelEntity ce = CreateChannelEntity<T> (channel, false, isChannelGroup, userState, null, null, null, null, null);
            bool stateChanged = Subscription.Instance.UpdateOrAddUserStateOfEntity (ce, userState);
            if (!stateChanged) {
                string message = "No change in User State";

                PubnubCallbacks.CallErrorCallback<T> (ce, message,
                    PubnubErrorCode.UserStateUnchanged, PubnubErrorSeverity.Info, errorLevel);
            } else {
                channelEntities.Add (ce);
            }
            return stateChanged;
        }

        internal static bool CheckAndAddExistingUserState<T>(string channel, string channelGroup, Dictionary<string, object> userState,
            Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel, out string returnUserState, out List<ChannelEntity> channelEntities 
        )
        {
            string[] channels = channel.Split (',');
            string[] channelGroups = channelGroup.Split (',');
            bool stateChanged = false;
            channelEntities = new List<ChannelEntity> ();

            foreach (string ch in channels) {
                bool changeState = UpdateOrAddUserStateOfEntity<T> (ch, false, userState, errorCallback, errorLevel, ref channelEntities);
                if(changeState && !stateChanged){
                    stateChanged = true;
                }
            }

            foreach (string ch in channelGroups) {
                bool changeState = UpdateOrAddUserStateOfEntity<T> (ch, true, userState, errorCallback, errorLevel, ref channelEntities);
                if(changeState && !stateChanged){
                    stateChanged = true;
                }
            }

            returnUserState = BuildJsonUserState(channelEntities);

            return stateChanged;
        }

        internal static ChannelEntity CreateChannelEntity<T>(string channelOrChannelGroupName2, bool isAwaitingConnectCallback,
            bool isChannelGroup, Dictionary<string, object> userState,
            Action<T> userCallback, Action<T> connectCallback, 
            Action<PubnubClientError> errorCallback, Action<T> disconnectCallback, Action<T> wildcardPresenceCallback
        ){
            string channelOrChannelGroupName = channelOrChannelGroupName2.Trim ();
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, channelOrChannelGroupName {1}, {2}", DateTime.Now.ToString(), 
                channelOrChannelGroupName.ToString(), channelOrChannelGroupName2.ToString()), LoggingMethod.LevelInfo);
            #endif

            if (!string.IsNullOrEmpty(channelOrChannelGroupName)) {
                ChannelIdentity ci = new ChannelIdentity ();
                ci.ChannelOrChannelGroupName = channelOrChannelGroupName;
                ci.IsPresenceChannel = Utility.IsPresenceChannel (channelOrChannelGroupName);
                ci.IsChannelGroup = isChannelGroup;

                ChannelParameters cp = new ChannelParameters ();
                cp.IsAwaitingConnectCallback = isAwaitingConnectCallback;
                cp.UserState = userState;
                cp.TypeParameterType = typeof(T);

                cp.Callbacks = PubnubCallbacks.GetPubnubChannelCallback<T> (userCallback, connectCallback, errorCallback, 
                    disconnectCallback, wildcardPresenceCallback);

                ChannelEntity ce = new ChannelEntity (ci, cp);

                return ce;
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, channelOrChannelGroupName empty, is channel group {1}", DateTime.Now.ToString(), isChannelGroup.ToString()), LoggingMethod.LevelInfo);
                #endif

                return null;
            }
        }

        internal static List<ChannelEntity> CreateChannelEntity<T>(string[] channelOrChannelGroupNames, bool isAwaitingConnectCallback,
            bool isChannelGroup, Dictionary<string, object> userState,
            Action<T> userCallback, Action<T> connectCallback, 
            Action<PubnubClientError> errorCallback, Action<T> disconnectCallback, Action<T> wildcardPresenceCallback
        ){
            List<ChannelEntity> channelEntities = null;
            if (channelOrChannelGroupNames != null) {
                channelEntities = new List<ChannelEntity> ();
                foreach (string ch in channelOrChannelGroupNames) {
                    ChannelEntity chEntity= CreateChannelEntity<T> (ch, isAwaitingConnectCallback, isChannelGroup, userState,
                        userCallback, connectCallback, errorCallback, disconnectCallback, wildcardPresenceCallback
                    );
                    if (chEntity != null) {
                        channelEntities.Add (chEntity);
                    }
                }
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, channelEntities={1}", DateTime.Now.ToString (), channelEntities.Count), LoggingMethod.LevelInfo);
                #endif
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, channelOrChannelGroupNames null, is channel group {1}", DateTime.Now.ToString(), isChannelGroup.ToString()), LoggingMethod.LevelInfo);
                #endif
            }

            return channelEntities;
        }

        /*internal static bool CheckChannelsInMultiChannelSubscribeRequest(string multiChannel)
        {
            if (!channelRequest.ContainsKey(multiChannel))
            {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, MultiChannelSubscribeRequest _channelRequest doesnt contain {1}", DateTime.Now.ToString(), multiChannel), LoggingMethod.LevelInfo);
                #endif
                string[] currentChannels = multiChannelSubscribe.Keys.ToArray<string>();
                if (currentChannels != null && currentChannels.Length > 0)
                {
                    #if (ENABLE_PUBNUB_LOGGING)    
                    string currentSubChannels = string.Join(",", currentChannels);
                    LoggingMethod.WriteToLog(string.Format("DateTime {0}, using existing channels: {1}", DateTime.Now.ToString(), currentSubChannels), LoggingMethod.LevelInfo);
                    #endif
                }
                else
                {
                    return true;
                }
            }
            return false;
        }*/

        internal static IEnumerable<string> GetDuplicates(string[] 
            rawChannels)
        {
            var results = from string a in rawChannels
                group a by a into g
                    where g.Count() > 1
                select g;
            foreach (var group in results)
                foreach (var item in group)
                    yield return item;
        }

        internal static void LogChannelEntitiesDictionary(){
            StringBuilder sbLogs = new StringBuilder();
            foreach (var ci in Subscription.Instance.ChannelEntitiesDictionary) {
                sbLogs.AppendFormat("\nChannelEntitiesDictionary \nChannelOrChannelGroupName:{0} \nIsChannelGroup:{1} \nIsPresenceChannel:{2}", 
                    ci.Key.ChannelOrChannelGroupName, ci.Key.IsChannelGroup.ToString(), ci.Key.IsPresenceChannel.ToString()
                );
            }
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, {1}", DateTime.Now.ToString (), sbLogs.ToString()), LoggingMethod.LevelInfo);
        }

        internal static bool CreateChannelEntityAndAddToSubscribe <T>(ResponseType type, string[] rawChannels, bool isChannelGroup,
            Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback, 
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback,
            PubnubErrorFilter.Level errorLevel, bool unsubscribeCheck, ref List<ChannelEntity> channelEntities
            )
        {
            bool bReturn = false;    
            for (int index = 0; index < rawChannels.Length; index++)
            {
                string channelName = rawChannels[index].Trim();
                #if (ENABLE_PUBNUB_LOGGING)
                #endif

                if (channelName.Length > 0) {
                    if (type == ResponseType.Presence) {
                        channelName = string.Format ("{0}{1}", channelName, Utility.PresenceChannelSuffix);
                    }
                    #if (ENABLE_PUBNUB_LOGGING)
                    Helpers.LogChannelEntitiesDictionary();
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, channel={1}", DateTime.Now.ToString (), channelName), LoggingMethod.LevelInfo);
                    #endif
                    //create channelEntity
                    ChannelEntity ce = Helpers.CreateChannelEntity (channelName, true, isChannelGroup, null, 
                        userCallback, connectCallback, errorCallback, disconnectCallback, wildcardPresenceCallback);

                    if (Subscription.Instance.ChannelEntitiesDictionary.ContainsKey (ce.ChannelID)) {
                        if (!unsubscribeCheck) {
                            string message = string.Format ("{0}Already subscribed", (ce.ChannelID.IsPresenceChannel)? "Presence " : "");
                            PubnubErrorCode errorType = (ce.ChannelID.IsPresenceChannel) ? PubnubErrorCode.AlreadyPresenceSubscribed : PubnubErrorCode.AlreadySubscribed;
                            PubnubCallbacks.CallErrorCallback<T> (ce, message,
                                errorType, PubnubErrorSeverity.Info, errorLevel);
                            #if (ENABLE_PUBNUB_LOGGING)
                            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, channel={1} response={2}", DateTime.Now.ToString (), channelName, message), LoggingMethod.LevelInfo);
                            #endif
                        } else {
                            string message = string.Format ("{0}Channel Not Subscribed", (ce.ChannelID.IsPresenceChannel) ? "Presence " : "");
                            PubnubErrorCode errorType = (ce.ChannelID.IsPresenceChannel) ? PubnubErrorCode.NotPresenceSubscribed : PubnubErrorCode.NotSubscribed;
                            #if (ENABLE_PUBNUB_LOGGING)
                            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, channel={1} response={2}", DateTime.Now.ToString (), channelName, message), LoggingMethod.LevelInfo);
                            #endif
                            PubnubCallbacks.CallErrorCallback<T> (ce, message,
                                errorType, PubnubErrorSeverity.Info, errorLevel);
                        }
                    } else {
                        channelEntities.Add (ce);
                        bReturn = true;
                    }
                } else {
                    string message = "Invalid Channel Name";
                    if (isChannelGroup) {
                        message = "Invalid Channel Group Name";
                    }
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog(string.Format("DateTime {0}, channel={1} response={2}", DateTime.Now.ToString(), channelName, message), 
                        LoggingMethod.LevelInfo);
                    #endif
                    //PubnubCallbacks.CallErrorCallback<T>(message, errorCallback, PubnubErrorCode.InvalidChannel, 
                        //PubnubErrorSeverity.Info, errorLevel);
                }
            }
            return bReturn;
        }

        internal static bool RemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<T>(ResponseType type, 
            Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback, 
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback,
            string[] channelsOrChannelGroups, bool isChannelGroup,
            PubnubErrorFilter.Level errorLevel, bool unsubscribeCheck, ref List<ChannelEntity> channelEntities)
        {
            bool bReturn = false;
            if (channelsOrChannelGroups.Length > 0) {
                if (channelsOrChannelGroups.Length != channelsOrChannelGroups.Distinct ().Count ()) {
                    channelsOrChannelGroups = channelsOrChannelGroups.Distinct ().ToArray ();
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, distinct channelsOrChannelGroups len={1}, channelsOrChannelGroups = {2}", 
                        DateTime.Now.ToString (), channelsOrChannelGroups.Length, string.Join(",", channelsOrChannelGroups)), 
                        LoggingMethod.LevelInfo);
                    #endif

                    string channel = string.Join (",", GetDuplicates (channelsOrChannelGroups).Distinct<string> ().ToArray<string> ());
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, duplicates channelsOrChannelGroups {1}", 
                        DateTime.Now.ToString (), channel), 
                        LoggingMethod.LevelInfo);
                    #endif

                    string message = string.Format ("Detected and removed duplicate channels {0}", channel); 

                    PubnubCallbacks.CallErrorCallback<T> (message, errorCallback, PubnubErrorCode.DuplicateChannel, 
                        PubnubErrorSeverity.Info, errorLevel);
                }
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, channelsOrChannelGroups len={1}, channelsOrChannelGroups = {2}", 
                    DateTime.Now.ToString (), channelsOrChannelGroups.Length, string.Join(",", channelsOrChannelGroups)), 
                    LoggingMethod.LevelInfo);
                #endif

                bReturn = CreateChannelEntityAndAddToSubscribe<T> (type, channelsOrChannelGroups, isChannelGroup, 
                    userCallback, connectCallback, errorCallback, wildcardPresenceCallback, disconnectCallback, errorLevel, false, ref channelEntities);
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, channelsOrChannelGroups len <=0", 
                    DateTime.Now.ToString ()), 
                    LoggingMethod.LevelInfo);
                #endif
            }
            return bReturn;
        }

        internal static bool RemoveDuplicatesCheckAlreadySubscribedAndGetChannels<T>(ResponseType type, 
            Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback, 
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback,
            string[] rawChannels, string[] rawChannelGroups, 
            PubnubErrorFilter.Level errorLevel, bool unsubscribeCheck, out List<ChannelEntity> channelEntities)
        {
            bool bReturn = false;
            bool channelAdded = false;
            bool channelGroupAdded = false;
            channelEntities = new List<ChannelEntity> ();
            if (rawChannels != null && rawChannels.Length > 0) {
                channelAdded = RemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<T> (type, userCallback, connectCallback, errorCallback,
                    wildcardPresenceCallback, disconnectCallback, rawChannels, false, errorLevel, unsubscribeCheck, ref channelEntities
                );
            }

            if (rawChannelGroups != null && rawChannelGroups.Length > 0) {
                channelGroupAdded = RemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<T> (type, userCallback, connectCallback, errorCallback,
                    wildcardPresenceCallback, disconnectCallback, rawChannelGroups, true, errorLevel, unsubscribeCheck, ref channelEntities
                );
            }

            bReturn = channelAdded | channelGroupAdded;

            return bReturn;
        }

        /*internal static string[] GetCurrentSubscriberChannels (SafeDictionary<string, bool> multiChannelSubscribe, bool isChannelGroup)
        {
            string[] channels = null;
            if (multiChannelSubscribe != null && multiChannelSubscribe.Keys.Count > 0) {
                channels = multiChannelSubscribe.Keys.ToArray<string> ();
            }

            return channels;
        }*/

        internal static void ProcessResponseCallbacks<T> (ref List<object> result, RequestState<T> asynchRequestState, 
            string cipherKey, IJsonPluggableLibrary jsonPluggableLibrary)
        {
            if (result != null && result.Count >= 1) {
                Helpers.ResponseToConnectCallback<T> (result, asynchRequestState, jsonPluggableLibrary);
                Helpers.ResponseToUserCallback<T> (result, asynchRequestState, cipherKey, jsonPluggableLibrary);
            } 
        }
        #endregion

        #region "Encoding and Crypto"

        internal static string JsonEncodePublishMsg (object originalMessage, string cipherKey, IJsonPluggableLibrary jsonPluggableLibrary)
        {
            string message = jsonPluggableLibrary.SerializeToJsonString (originalMessage);

            if (cipherKey.Length > 0) {
                PubnubCrypto aes = new PubnubCrypto (cipherKey);
                string encryptMessage = aes.Encrypt (message);
                message = jsonPluggableLibrary.SerializeToJsonString (encryptMessage);
            }

            return message;
        }

        internal static object DecodeMessage (PubnubCrypto aes, object element, List<ChannelEntity> channelEntities,
            IJsonPluggableLibrary jsonPluggableLibrary, 
            PubnubErrorFilter.Level errorLevel)
        {
            string decryptMessage = "";
            try {
                decryptMessage = aes.Decrypt (element.ToString ());
            }
            catch (Exception ex) {
                decryptMessage = "**DECRYPT ERROR**";
                PubnubCallbacks.CallErrorCallback<object> (ex, channelEntities, PubnubErrorCode.None, 
                    PubnubErrorSeverity.Critical, errorLevel);
            }
            object decodeMessage = (decryptMessage == "**DECRYPT ERROR**") ? decryptMessage : jsonPluggableLibrary.DeserializeToObject (decryptMessage);
            return decodeMessage;
        }

        internal static List<object> DecryptCipheredMessage (List<object> message, List<ChannelEntity> channelEntities,
            string cipherKey, IJsonPluggableLibrary jsonPluggableLibrary, 
            PubnubErrorFilter.Level errorLevel)
        {
            List<object> returnMessage = new List<object> ();

            PubnubCrypto aes = new PubnubCrypto (cipherKey);
            var myObjectArray = (from item in message
                select item as object).ToArray ();
            IEnumerable enumerable = myObjectArray [0] as IEnumerable;

            if (enumerable != null) {
                List<object> receivedMsg = new List<object> ();
                foreach (object element in enumerable) {
                    receivedMsg.Add (DecodeMessage (aes, element, channelEntities, jsonPluggableLibrary, errorLevel));
                }
                returnMessage.Add (receivedMsg);
            }
            for (int index = 1; index < myObjectArray.Length; index++) {
                returnMessage.Add (myObjectArray [index]);
            }
            return returnMessage;
        }

        internal static List<object> DecryptNonCipheredMessage (List<object> message)
        {
            List<object> returnMessage = new List<object> ();
            var myObjectArray = (from item in message
                select item as object).ToArray ();
            IEnumerable enumerable = myObjectArray [0] as IEnumerable;
            if (enumerable != null) {
                List<object> receivedMessage = new List<object> ();
                foreach (object element in enumerable) {
                    receivedMessage.Add (element);
                }
                returnMessage.Add (receivedMessage);
            }
            for (int index = 1; index < myObjectArray.Length; index++) {
                returnMessage.Add (myObjectArray [index]);
            }
            return returnMessage;
        }

        internal static List<object> DecodeDecryptLoop (List<object> message, List<ChannelEntity> channelEntities, 
            string cipherKey, IJsonPluggableLibrary jsonPluggableLibrary, 
            PubnubErrorFilter.Level errorLevel)
        {
            if (cipherKey.Length > 0) {
                return DecryptCipheredMessage (message, channelEntities, cipherKey, jsonPluggableLibrary, errorLevel);
            } else {
                return DecryptNonCipheredMessage (message);
            }
        }    

        #endregion

        #region "Other Methods"

        internal static void CheckSubscribedChannelsAndSendCallbacks<T> (List<ChannelEntity> channelEntities,
            ResponseType type, int pubnubNetworkCheckRetries, PubnubErrorFilter.Level errorLevel){
            if (channelEntities != null && channelEntities.Count > 0) {
                foreach (ChannelEntity channelEntity in channelEntities) {
                    string message = string.Format ("Unsubscribed after {0} failed retries", pubnubNetworkCheckRetries);;
                    PubnubErrorCode pnErrorCode = PubnubErrorCode.UnsubscribedAfterMaxRetries;

                    if (channelEntity.ChannelID.IsPresenceChannel) {
                        message = string.Format ("Presence Unsubscribed after {0} failed retries", pubnubNetworkCheckRetries);
                        pnErrorCode = PubnubErrorCode.PresenceUnsubscribedAfterMaxRetries;
                    }

                    //TODO: Recheck
                    PubnubClientError error = Helpers.CreatePubnubClientError<T> (message, null, channelEntities, pnErrorCode,
                        PubnubErrorSeverity.Critical);  
                    
                    PubnubCallbacks.FireErrorCallback<T> (channelEntity,
                        errorLevel, error);

                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, {1} Subscribe JSON network error response={2}", 
                        DateTime.Now.ToString (), (channelEntity.ChannelID.IsPresenceChannel)?"Presence":"", message), LoggingMethod.LevelInfo);
                    #endif

                }
            }
        }
            
        public static void WrapResultBasedOnResponseType<T> (RequestState<T> pubnubRequestState, string jsonString, 
            IJsonPluggableLibrary jsonPluggableLibrary, PubnubErrorFilter.Level errorLevel, string cipherKey, ref List<object> result)
        {
            /*return WrapResultBasedOnResponseType<T> (jsonString, pubnubRequestState,
                 jsonPluggableLibrary, 
                errorLevel, cipherKey
            );
        }

        public static List<object> WrapResultBasedOnResponseType<T> (ResponseType type, string jsonString, string[] channels, 
            IJsonPluggableLibrary jsonPluggableLibrary, PubnubErrorFilter.Level errorLevel, string cipherKey
        )
        {*/
            try {
                string multiChannel = Helpers.GetNamesFromChannelEntities(pubnubRequestState.ChannelEntities, false); 
                string multiChannelGroup = Helpers.GetNamesFromChannelEntities(pubnubRequestState.ChannelEntities, true);
                if (!string.IsNullOrEmpty (jsonString)) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, jsonString = {1}", DateTime.Now.ToString (), jsonString), LoggingMethod.LevelInfo);
                    #endif
                    object deSerializedResult = jsonPluggableLibrary.DeserializeToObject (jsonString);
                    List<object> result1 = ((IEnumerable)deSerializedResult).Cast<object> ().ToList ();

                    if (result1 != null && result1.Count > 0) {
                        result = result1;
                    }

                    switch (pubnubRequestState.RespType) {
                    case ResponseType.DetailedHistory:
                        result = DecodeDecryptLoop (result, pubnubRequestState.ChannelEntities, cipherKey, jsonPluggableLibrary, errorLevel);
                        result.Add (multiChannel);
                        break;
                    case ResponseType.Time:
                        Int64[] c = deSerializedResult as Int64[];
                        if ((c != null) && (c.Length > 0)) {
                            result = new List<object> ();
                            result.Add (c [0]);
                        }
                        break;
                    case ResponseType.Subscribe:
                    case ResponseType.Presence:
                    case ResponseType.Leave:
                        if (!string.IsNullOrEmpty(multiChannelGroup))
                        {
                            result.Add(multiChannelGroup);
                        }
                        if (!string.IsNullOrEmpty(multiChannel))
                        {
                            result.Add(multiChannel);
                        }
                        break;
                    case ResponseType.Publish:
                    case ResponseType.PushRegister:
                    case ResponseType.PushRemove:
                    case ResponseType.PushGet:
                    case ResponseType.PushUnregister:
                        result.Add (multiChannel);
                        break;
                    case ResponseType.GrantAccess:
                    case ResponseType.AuditAccess:
                    case ResponseType.RevokeAccess:
                    case ResponseType.GetUserState:
                    case ResponseType.SetUserState:
                    case ResponseType.WhereNow:
                    case ResponseType.HereNow:
                        result = DeserializeAndAddToResult (jsonString, multiChannel, jsonPluggableLibrary, true);
                        break;
                    case ResponseType.GlobalHereNow:
                        result = DeserializeAndAddToResult (jsonString, multiChannel, jsonPluggableLibrary, false);
                        break;
                    case ResponseType.ChannelGroupAdd:
                    case ResponseType.ChannelGroupRemove:
                    case ResponseType.ChannelGroupGet:
                        result = DeserializeAndAddToResult (jsonString, multiChannel, jsonPluggableLibrary, false);

                        break;
                    case ResponseType.ChannelGroupGrantAccess:
                    case ResponseType.ChannelGroupAuditAccess:
                    case ResponseType.ChannelGroupRevokeAccess:
                        result = DeserializeAndAddToResult (jsonString, "", jsonPluggableLibrary, false);
                        result.Add(multiChannelGroup);
                        break;

                    default:
                        break;
                    }
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, json string null ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                }
                #endif
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WrapResultBasedOnResponseType exception: {1} ", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
                ProcessWrapResultBasedOnResponseTypeException<T> (pubnubRequestState, errorLevel, ex);
            }
            //return result;
        }

        internal static List<object> DeserializeAndAddToResult (string jsonString, string multiChannel, 
            IJsonPluggableLibrary jsonPluggableLibrary, bool addChannel)
        {
            Dictionary<string, object> dictionary = jsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonString);
            List<object> result = new List<object> ();
            result.Add (dictionary);
            if (addChannel) {
                result.Add (multiChannel);
            }
            return result;
        }

        internal static void ProcessWrapResultBasedOnResponseTypeException<T> (RequestState<T> pubnubRequestState, 
            PubnubErrorFilter.Level errorLevel, Exception ex)
        {
            if (pubnubRequestState.ChannelEntities != null) {
                if (pubnubRequestState.RespType == ResponseType.Subscribe || pubnubRequestState.RespType == ResponseType.Presence) {
                    PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (ex, pubnubRequestState, PubnubErrorSeverity.Critical, 
                        PubnubErrorCode.None, errorLevel);
                }
                else {
                    PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (ex, pubnubRequestState, PubnubErrorSeverity.Critical, 
                        PubnubErrorCode.None, errorLevel);
                }
            }
        }

        internal static object[] CreateMessageList(List<object> result, object[] messageList)
        {
            int i = 0;
            foreach (object o in result)
            {
                if (i == 0)
                {
                    IList collection = (IList)o;
                    messageList = new object[collection.Count];
                    bool added = false;
                    int j = 0;
                    foreach (object c in collection)
                    {
                        if ((c.GetType() == typeof(System.Int32)) || (c.GetType() == typeof(System.Double)) || (c.GetType() == typeof(System.Int64)) || (c.GetType() == typeof(System.Boolean)))
                        {
                            added = true;
                            #if (ENABLE_PUBNUB_LOGGING)
                            LoggingMethod.WriteToLog(string.Format("DateTime {0}, collection: {1} in type: {2}", DateTime.Now.ToString(), c.ToString(), c.GetType().ToString()), LoggingMethod.LevelInfo);
                            #endif
                            messageList[j] = c;
                        }
                        else if (c.GetType() == typeof(System.String))
                        {
                            added = true;
                            #if (ENABLE_PUBNUB_LOGGING)
                            LoggingMethod.WriteToLog(string.Format("DateTime {0}, collection: {1} in type: {2}", DateTime.Now.ToString(), c.ToString(), c.GetType().ToString()), LoggingMethod.LevelInfo);
                            #endif
                            messageList[j] = c.ToString();
                        }
                        else
                        {
                            try
                            {
                                messageList[j] = c;
                                added = true;
                                #if (ENABLE_PUBNUB_LOGGING)
                                LoggingMethod.WriteToLog(string.Format("DateTime {0}, collection other types: {1} in type: {2}", DateTime.Now.ToString(), c.ToString(), c.GetType().ToString()), LoggingMethod.LevelInfo);
                                #endif
                            }
                            catch (Exception ex)
                            {
                                added = false;
                                #if (ENABLE_PUBNUB_LOGGING)
                                LoggingMethod.WriteToLog(string.Format("DateTime {0}, collection other types: {1} in type: {2}, exception {3} ", DateTime.Now.ToString(), c.ToString(), c.GetType().ToString(), ex.ToString()), LoggingMethod.LevelInfo);
                                #endif
                            }
                        }
                        j++;
                    }
                    if (!added)
                    {
                        collection.CopyTo(messageList, 0);
                    }
                }
                i++;
            }
            return messageList;
        }

        internal static List<object> AddMessageToList(string cipherKey, IJsonPluggableLibrary jsonPluggableLibrary, 
            object[] messages, int messageIndex, string currentChannel, object[] messageList)
        {
            List<object> itemMessage = new List<object>();
            if (currentChannel.Contains(Utility.PresenceChannelSuffix))
            {
                itemMessage.Add(messageList[messageIndex]);
            }
            else
            {
                //decrypt the subscriber message if cipherkey is available
                if (cipherKey.Length > 0)
                {
                    object decodeMessage;
                    try
                    {
                        PubnubCrypto aes = new PubnubCrypto(cipherKey);
                        string decryptMessage = aes.Decrypt(messageList[messageIndex].ToString());
                        decodeMessage = (decryptMessage == "**DECRYPT ERROR**") ? decryptMessage : jsonPluggableLibrary.DeserializeToObject(decryptMessage);
                    }
                    catch (Exception decryptEx)
                    {
                        decodeMessage = messageList[messageIndex].ToString();
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog(string.Format("DateTime {0}, decodeMessage Exception: {1}", DateTime.Now.ToString(), decryptEx.ToString()), LoggingMethod.LevelError);
                        #endif
                    }
                    itemMessage.Add(decodeMessage);
                }
                else
                {
                    itemMessage.Add(messageList[messageIndex]);
                }
            }
            itemMessage.Add(messages[1].ToString());
            itemMessage.Add(currentChannel.Replace(Utility.PresenceChannelSuffix, ""));
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, messageList: {1} index {2}, timestamp {3}, channel {4}", DateTime.Now.ToString(),
                messageList[messageIndex], messageIndex.ToString(), messages[1].ToString(), currentChannel), LoggingMethod.LevelInfo);
            #endif

            return itemMessage;
        }

        internal static void ResponseToUserCallbackForSubscribeSendCallbacks<T> (List<object> result, string cipherKey, 
            List<ChannelEntity> channelEntities,
            IJsonPluggableLibrary jsonPluggableLibrary, object[] messages)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, messageList typeOF: {1}", DateTime.Now.ToString (), 
                messages [0].GetType ().ToString ()), LoggingMethod.LevelInfo);
            #endif

            var messageList = messages [0] as object[];
            messageList = CreateMessageList(result, messageList);

            string[] messageChannels = messages [2].ToString ().Split (',');

            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, (messageChannels: {1}", DateTime.Now.ToString(), messageChannels.ToString()), LoggingMethod.LevelInfo);
            #endif

            if (messageList != null && messageList.Length > 0)
            {
                for (int messageIndex = 0; messageIndex < messageList.Length; messageIndex++)
                {
                    string currentChannel = (messageChannels.Length == 1) ? (string)messageChannels[0] : (string)messageChannels[messageIndex];
                    var itemMessage = AddMessageToList(cipherKey, jsonPluggableLibrary, messages, messageIndex, currentChannel, messageList);

                    ChannelEntity ce = channelEntities [messageIndex];
                    if (ce != null) {
                        PubnubCallbacks.SendCallbacks<T> (jsonPluggableLibrary, ce, itemMessage, CallbackType.Success, true);
                    }
                    #if (ENABLE_PUBNUB_LOGGING)
                    else {
                        LoggingMethod.WriteToLog(string.Format("DateTime {0}, ChannelEntity null: {1} index {2}", DateTime.Now.ToString(),
                            messageChannels.ToString(), messageIndex.ToString()), LoggingMethod.LevelInfo);
                    }
                    #endif
                }
            }
        }

        internal static void ResponseToUserCallbackForSubscribe<T> (List<object> result, List<ChannelEntity> channelEntities,
            string cipherKey, IJsonPluggableLibrary jsonPluggableLibrary)
        {
            var messages = (from item in result
                select item as object).ToArray ();

            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, result: {1}", DateTime.Now.ToString (), result.ToString ()), LoggingMethod.LevelInfo);
            #endif

            if (messages != null && messages.Length > 0) {
                ResponseToUserCallbackForSubscribeSendCallbacks <T>(result, cipherKey, channelEntities, jsonPluggableLibrary, messages);
            }            
        }
            
        /*internal static void CheckResultListAndCallCallback<T>(List<object> result, List<ChannelEntity> channelEntities, 
            IJsonPluggableLibrary jsonPluggableLibrary){
            if (result != null && result.Count > 0) {
                PubnubCallbacks.GoToCallback<T> (result, userCallback, jsonPluggableLibrary);
            }
        }*/

        internal static void ResponseToUserCallback<T> (List<object> result, RequestState<T> asynchRequestState, string cipherKey, 
            IJsonPluggableLibrary jsonPluggableLibrary)
        {
            switch (asynchRequestState.RespType) {
                case ResponseType.Subscribe:
                case ResponseType.Presence:
                    ResponseToUserCallbackForSubscribe<T>(result, asynchRequestState.ChannelEntities, cipherKey, jsonPluggableLibrary);
                break;
                case ResponseType.Leave:
                //No response to callback
                    break;
                case ResponseType.Publish:
                case ResponseType.DetailedHistory:
                case ResponseType.HereNow:
                case ResponseType.GlobalHereNow:
                case ResponseType.WhereNow:
                case ResponseType.GrantAccess:
                case ResponseType.AuditAccess:
                case ResponseType.RevokeAccess:
                case ResponseType.GetUserState:
                case ResponseType.SetUserState:
                case ResponseType.Time:
                case ResponseType.PushRegister:
                case ResponseType.PushRemove:
                case ResponseType.PushGet:
                case ResponseType.PushUnregister:
                case ResponseType.ChannelGroupAdd:
                case ResponseType.ChannelGroupRemove:
                case ResponseType.ChannelGroupGet:
                case ResponseType.ChannelGroupGrantAccess:
                case ResponseType.ChannelGroupAuditAccess:
                case ResponseType.ChannelGroupRevokeAccess:
                
                    PubnubCallbacks.SendCallbacks<T>(jsonPluggableLibrary, asynchRequestState, result, CallbackType.Success, false);
                    break;
                default:
                    break;
            }
        }

        internal static void ResponseToConnectCallback<T> (List<object> result, RequestState<T> asynchRequestState,
             IJsonPluggableLibrary jsonPluggableLibrary)
        {
            //Check callback exists and make sure previous timetoken = 0
            if (asynchRequestState.ChannelEntities != null) {
                foreach (ChannelEntity channelEntity in asynchRequestState.ChannelEntities) {
                    if (channelEntity.ChannelParams.IsAwaitingConnectCallback) {
                        switch (asynchRequestState.RespType) {
                        case ResponseType.Subscribe:
                            var connectResult = Helpers.CreateJsonResponse ("Connected", channelEntity.ChannelID.ChannelOrChannelGroupName, jsonPluggableLibrary);
                            PubnubCallbacks.SendCallbacks<T> (jsonPluggableLibrary, channelEntity, connectResult, 
                                CallbackType.Connect, false);

                            break;
                        case ResponseType.Presence:
                            var connectResult2 = Helpers.CreateJsonResponse ("Presence Connected", 
                                                 channelEntity.ChannelID.ChannelOrChannelGroupName.Replace (Utility.PresenceChannelSuffix, ""), jsonPluggableLibrary);
                            PubnubCallbacks.SendCallbacks<T> (jsonPluggableLibrary, channelEntity, connectResult2, 
                                CallbackType.Connect, false);
                            break;
                        default:
                            break;
                        }
                    }
                }
                Subscription.Instance.UpdateIsAwaitingConnectCallbacksOfEntity(asynchRequestState.ChannelEntities, false);
            }

        }

        internal static List<object> CreateJsonResponse(string message, string channel, IJsonPluggableLibrary jsonPluggableLibrary){
            string jsonString = "";
            List<object> connectResult = new List<object> ();
            jsonString = string.Format ("[1, \"{0}\"]", message);
            connectResult = jsonPluggableLibrary.DeserializeToListOfObject (jsonString);
            connectResult.Add (channel);

            return connectResult;
        }

        internal static PubnubErrorCode GetTimeOutErrorCode (ResponseType responseType)
        {
            switch(responseType){
            case ResponseType.AuditAccess:
            case ResponseType.GrantAccess:
            case ResponseType.RevokeAccess:
            case ResponseType.ChannelGroupGrantAccess:
            case ResponseType.ChannelGroupAuditAccess:
            case ResponseType.ChannelGroupRevokeAccess:
                return PubnubErrorCode.PAMAccessOperationTimeout;
            case ResponseType.DetailedHistory:
            case ResponseType.History:
                return PubnubErrorCode.DetailedHistoryOperationTimeout;
            case ResponseType.GetUserState:
                return PubnubErrorCode.GetUserStateTimeout;
            case ResponseType.GlobalHereNow:
                return PubnubErrorCode.GlobalHereNowOperationTimeout;
            case ResponseType.SetUserState:
                return PubnubErrorCode.SetUserStateTimeout;
            case ResponseType.HereNow:
                return PubnubErrorCode.HereNowOperationTimeout;
            case ResponseType.Publish:
                return PubnubErrorCode.PublishOperationTimeout;
            case ResponseType.Time:
                return PubnubErrorCode.TimeOperationTimeout;
            case ResponseType.WhereNow:
                return PubnubErrorCode.WhereNowOperationTimeout;
            default:
                /* 
                 * ResponseType.Presence:
                 * ResponseType.PresenceUnsubscribe:
                 * ResponseType.Leave:
                 * ResponseType.Subscribe:
                 * ResponseType.Unsubscribe:
                 * ResponseType.Heartbeat:
                 * ResponseType.PresenceHeartbeat:
                 * ResponseType.ChannelGroupAdd
                 * ResponseType.ChannelGroupRemove
                 * ResponseType.ChannelGroupGet
                 */
                return PubnubErrorCode.OperationTimeout;
            }
        }

        internal static PubnubClientError CreatePubnubClientError<T>(Exception ex, 
            RequestState<T> requestState, List<ChannelEntity> channelEntities, PubnubErrorCode errorCode, PubnubErrorSeverity severity){

            if (errorCode.Equals (PubnubErrorCode.None)) {
                errorCode = PubnubErrorCodeHelper.GetErrorType (ex);
            }

            int statusCode = (int)errorCode;
            string errorDescription = PubnubErrorCodeDescription.GetStatusCodeDescription (errorCode);
            PubnubWebRequest request = null;
            PubnubWebResponse response = null;

            if (requestState != null) {
                request = requestState.Request;
                response = requestState.Response;
                if (channelEntities == null && requestState.ChannelEntities != null) {
                    channelEntities = requestState.ChannelEntities;
                }
            }

            PubnubClientError error = new PubnubClientError (statusCode, severity, true, ex.Message, ex, 
                PubnubMessageSource.Client, request, response, errorDescription, 
                channelEntities);

            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PubnubClientError = {1}", DateTime.Now.ToString (), error.ToString ()), LoggingMethod.LevelInfo);
            #endif
            return error;
        }

        internal static PubnubClientError CreatePubnubClientError<T>(string message, 
            RequestState<T> requestState, PubnubErrorCode errorCode, PubnubErrorSeverity severity, string channels, string channelGroups){

            int statusCode = (int)errorCode;
            string errorDescription = PubnubErrorCodeDescription.GetStatusCodeDescription (errorCode);

            PubnubWebRequest request = null;
            PubnubWebResponse response = null;

            if (requestState != null) {
                request = requestState.Request;
                response = requestState.Response;
            }

            PubnubClientError error = new PubnubClientError (statusCode, severity, message, PubnubMessageSource.Client, 
                request, response, errorDescription, 
                channels, channelGroups
            );

            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PubnubClientError = {1}", DateTime.Now.ToString (), error.ToString ()), LoggingMethod.LevelInfo);
            #endif
            return error;
        }

        internal static PubnubClientError CreatePubnubClientError<T>(Exception ex, 
            RequestState<T> requestState, PubnubErrorCode errorCode, PubnubErrorSeverity severity, string channels, string channelGroups){

            int statusCode = (int)errorCode;
            string errorDescription = PubnubErrorCodeDescription.GetStatusCodeDescription (errorCode);

            PubnubWebRequest request = null;
            PubnubWebResponse response = null;

            if (requestState != null) {
                request = requestState.Request;
                response = requestState.Response;
            }

            PubnubClientError error = new PubnubClientError (statusCode, severity, ex.Message, PubnubMessageSource.Client, 
                request, response, errorDescription, 
                channels, channelGroups
            );

            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PubnubClientError = {1}", DateTime.Now.ToString (), error.ToString ()), LoggingMethod.LevelInfo);
            #endif
            return error;
        }

        internal static PubnubClientError CreatePubnubClientError<T>(string message, 
            RequestState<T> requestState, List<ChannelEntity> channelEntities, PubnubErrorCode errorCode, PubnubErrorSeverity severity){

            int statusCode = (int)errorCode;
            string errorDescription = PubnubErrorCodeDescription.GetStatusCodeDescription (errorCode);

            PubnubWebRequest request = null;
            PubnubWebResponse response = null;

            if (requestState != null) {
                request = requestState.Request;
                response = requestState.Response;
            }

            PubnubClientError error = new PubnubClientError (statusCode, severity, message, PubnubMessageSource.Client, 
                request, response, errorDescription, 
                channelEntities
            );

            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PubnubClientError = {1}", DateTime.Now.ToString (), error.ToString ()), LoggingMethod.LevelInfo);
            #endif
            return error;
        }

        internal static PubnubClientError CreatePubnubClientError<T>(WebException ex, 
            RequestState<T> requestState, string channel, PubnubErrorSeverity severity){

            PubnubErrorCode errorCode = PubnubErrorCodeHelper.GetErrorType (ex.Status, ex.Message);
            return CreatePubnubClientError<T> (ex, requestState, null,  errorCode, severity);
        }

        #endregion

    }
}
