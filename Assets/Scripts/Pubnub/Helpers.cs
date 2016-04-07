using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Collections;

namespace PubNubMessaging.Core
{
    internal static class Helpers
    {
        #region "Helpers"
        internal static bool CheckChannelsInMultiChannelSubscribeRequest(string multiChannel, 
            SafeDictionary<string, long> multiChannelSubscribe, SafeDictionary<string, PubnubWebRequest> channelRequest)
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
        }

        internal static List<string> RemoveDuplicateChannelsAndCheckForAlreadySubscribedChannels<T>(ResponseType type, 
            string channel, Action<PubnubClientError> errorCallback, string[] rawChannels, List<string> validChannels, 
            bool networkConnection, SafeDictionary<string, long> multiChannelSubscribe, PubnubErrorFilter.Level errorLevel)
        {
            if (rawChannels.Length > 0 && networkConnection)
            {
                if (rawChannels.Length != rawChannels.Distinct().Count())
                {
                    rawChannels = rawChannels.Distinct().ToArray();
                    string message = "Detected and removed duplicate channels";
                    PubnubCallbacks.CallErrorCallback<T>(message, null, channel, PubnubErrorCode.DuplicateChannel, PubnubErrorSeverity.Info, errorCallback, errorLevel);
                }
                for (int index = 0; index < rawChannels.Length; index++)
                {
                    if (rawChannels[index].Trim().Length > 0)
                    {
                        string channelName = rawChannels[index].Trim();
                        if (type == ResponseType.Presence)
                        {
                            channelName = string.Format("{0}{1}", channelName, Utility.PresenceChannelSuffix);
                        }
                        if (multiChannelSubscribe.ContainsKey(channelName))
                        {
                            string message = string.Format("{0}Already subscribed", (Utility.IsPresenceChannel(channelName)) ? "Presence " : "");
                            PubnubErrorCode errorType = (Utility.IsPresenceChannel(channelName)) ? PubnubErrorCode.AlreadyPresenceSubscribed : PubnubErrorCode.AlreadySubscribed;
                            PubnubCallbacks.CallErrorCallback<T>(message, null, channelName.Replace(Utility.PresenceChannelSuffix, ""), errorType, PubnubErrorSeverity.Info, errorCallback, errorLevel);
                        }
                        else
                        {
                            validChannels.Add(channelName);
                        }
                    }
                }
            }
            return validChannels;
        }

        internal static List<string> GetValidChannels<T>(ResponseType type, Action<PubnubClientError> errorCallback, 
            string[] rawChannels, SafeDictionary<string, long> multiChannelSubscribe, PubnubErrorFilter.Level errorLevel)
        {
            List<string> validChannels = new List<string>();
            if (rawChannels.Length > 0)
            {
                for (int index = 0; index < rawChannels.Length; index++)
                {
                    if (rawChannels[index].Trim().Length > 0)
                    {
                        string channelName = rawChannels[index].Trim();
                        if (type == ResponseType.PresenceUnsubscribe)
                        {
                            channelName = string.Format("{0}{1}", channelName, Utility.PresenceChannelSuffix);
                        }
                        if (!multiChannelSubscribe.ContainsKey(channelName))
                        {
                            string message = string.Format("{0}Channel Not Subscribed", (Utility.IsPresenceChannel(channelName)) ? "Presence " : "");
                            PubnubErrorCode errorType = (Utility.IsPresenceChannel(channelName)) ? PubnubErrorCode.NotPresenceSubscribed : PubnubErrorCode.NotSubscribed;
                            #if (ENABLE_PUBNUB_LOGGING)
                            LoggingMethod.WriteToLog(string.Format("DateTime {0}, channel={1} unsubscribe response={2}", DateTime.Now.ToString(), channelName, message), LoggingMethod.LevelInfo);
                            #endif
                            PubnubCallbacks.CallErrorCallback<T>(message, null, channelName, errorType, PubnubErrorSeverity.Info, errorCallback, errorLevel);
                        }
                        else
                        {
                            validChannels.Add(channelName);
                        }
                    }
                    else
                    {
                        string message = "Invalid Channel Name For Unsubscribe";
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog(string.Format("DateTime {0}, channel={1} unsubscribe response={2}", DateTime.Now.ToString(), rawChannels[index], message), LoggingMethod.LevelInfo);
                        #endif
                        PubnubCallbacks.CallErrorCallback<T>(message, null, rawChannels[index].ToString(), PubnubErrorCode.InvalidChannel, PubnubErrorSeverity.Info, errorCallback, errorLevel);
                    }
                }
            }
            return validChannels;
        }

        internal static string[] GetCurrentSubscriberChannels (SafeDictionary<string, long> multiChannelSubscribe)
        {
            string[] channels = null;
            if (multiChannelSubscribe != null && multiChannelSubscribe.Keys.Count > 0) {
                channels = multiChannelSubscribe.Keys.ToArray<string> ();
            }

            return channels;
        }

        internal static void ProcessResponseCallbacks<T> (List<object> result, RequestState<T> asynchRequestState, 
            SafeDictionary<string, long> multiChannelSubscribe, string cipherKey, 
            SafeDictionary<PubnubChannelCallbackKey, object> channelCallbacks, IJsonPluggableLibrary jsonPluggableLibrary)
        {
            if (result != null && result.Count >= 1 && asynchRequestState.UserCallback != null) {
                Helpers.ResponseToConnectCallback<T> (result, asynchRequestState.RespType, asynchRequestState.Channels, 
                    asynchRequestState.ConnectCallback, multiChannelSubscribe, channelCallbacks, jsonPluggableLibrary);
                Helpers.ResponseToUserCallback<T> (result, asynchRequestState.RespType, asynchRequestState.Channels, 
                    asynchRequestState.UserCallback, cipherKey, channelCallbacks, jsonPluggableLibrary);
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

        internal static object DecodeMessage (PubnubCrypto aes, object element, string[] channels, 
            Action<PubnubClientError> errorCallback, IJsonPluggableLibrary jsonPluggableLibrary, 
            PubnubErrorFilter.Level errorLevel)
        {
            string decryptMessage = "";
            try {
                decryptMessage = aes.Decrypt (element.ToString ());
            }
            catch (Exception ex) {
                decryptMessage = "**DECRYPT ERROR**";
                string multiChannel = string.Join (",", channels);
                PubnubCallbacks.CallErrorCallback<object> (ex, null, multiChannel, PubnubErrorCode.None, 
                    PubnubErrorSeverity.Critical, errorCallback, errorLevel);
            }
            object decodeMessage = (decryptMessage == "**DECRYPT ERROR**") ? decryptMessage : jsonPluggableLibrary.DeserializeToObject (decryptMessage);
            return decodeMessage;
        }

        internal static List<object> DecryptCipheredMessage (List<object> message, string[] channels, 
            Action<PubnubClientError> errorCallback, string cipherKey, IJsonPluggableLibrary jsonPluggableLibrary, 
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
                    receivedMsg.Add (DecodeMessage (aes, element, channels, errorCallback, jsonPluggableLibrary, errorLevel));
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

        internal static List<object> DecodeDecryptLoop (List<object> message, string[] channels, 
            Action<PubnubClientError> errorCallback, string cipherKey, IJsonPluggableLibrary jsonPluggableLibrary, 
            PubnubErrorFilter.Level errorLevel)
        {
            if (cipherKey.Length > 0) {
                return DecryptCipheredMessage (message, channels, errorCallback, cipherKey, jsonPluggableLibrary, errorLevel);
            } else {
                return DecryptNonCipheredMessage (message);
            }
        }    

        #endregion

        #region "Other Methods"

        internal static void CheckSubscribedChannelsAndSendCallbacks<T> (string[] channels, bool isPresence, 
            ResponseType type, int pubnubNetworkCheckRetries, SafeDictionary<PubnubChannelCallbackKey, 
            object> channelCallbacks, PubnubErrorFilter.Level errorLevel){
            if (channels != null && channels.Length > 0) {
                string message = string.Format ("Unsubscribed after {0} failed retries", pubnubNetworkCheckRetries);;
                PubnubErrorCode pnErrorCode = PubnubErrorCode.UnsubscribedAfterMaxRetries;

                if (isPresence) {
                    message = string.Format ("Presence Unsubscribed after {0} failed retries", pubnubNetworkCheckRetries);
                    pnErrorCode = PubnubErrorCode.PresenceUnsubscribedAfterMaxRetries;
                }

                PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (message, null, channels,
                    PubnubErrorSeverity.Critical, channelCallbacks, 
                    false, pnErrorCode, type, errorLevel);

                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, {1} Subscribe JSON network error response={2}", 
                    DateTime.Now.ToString (), (isPresence)?"Presence":"", message), LoggingMethod.LevelInfo);
                #endif
            }
        }
            
        public static List<object> WrapResultBasedOnResponseType<T> (RequestState<T> pubnubRequestState, string jsonString, 
            SafeDictionary<PubnubChannelCallbackKey, object> channelCallbacks, 
            IJsonPluggableLibrary jsonPluggableLibrary, PubnubErrorFilter.Level errorLevel, string cipherKey)
        {
            return WrapResultBasedOnResponseType<T> (pubnubRequestState.RespType, jsonString, pubnubRequestState.Channels,
                pubnubRequestState.ErrorCallback, channelCallbacks, jsonPluggableLibrary, 
                errorLevel, cipherKey, pubnubRequestState.ChannelGroups
            );
        }

        public static List<object> WrapResultBasedOnResponseType<T> (ResponseType type, string jsonString, string[] channels, 
            Action<PubnubClientError> errorCallback, SafeDictionary<PubnubChannelCallbackKey, 
            object> channelCallbacks, IJsonPluggableLibrary jsonPluggableLibrary, PubnubErrorFilter.Level errorLevel, string cipherKey,
            string[] channelGroups
        )
        {
            List<object> result = new List<object> ();

            try {
                string multiChannel = (channels != null) ? string.Join (",", channels) : "";
                if (!string.IsNullOrEmpty (jsonString)) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, jsonString = {1}", DateTime.Now.ToString (), jsonString), LoggingMethod.LevelInfo);
                    #endif
                    object deSerializedResult = jsonPluggableLibrary.DeserializeToObject (jsonString);
                    List<object> result1 = ((IEnumerable)deSerializedResult).Cast<object> ().ToList ();

                    if (result1 != null && result1.Count > 0) {
                        result = result1;
                    }

                    switch (type) {
                    case ResponseType.DetailedHistory:
                        result = DecodeDecryptLoop (result, channels, errorCallback, cipherKey, jsonPluggableLibrary, errorLevel);
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
                ProcessWrapResultBasedOnResponseTypeException<T> (type, channels, errorCallback, channelCallbacks, errorLevel, ex);
            }
            return result;
        }

        internal static List<object> DeserializeAndAddToResult (string jsonString, string multiChannel, IJsonPluggableLibrary jsonPluggableLibrary, bool addChannel)
        {
            Dictionary<string, object> dictionary = jsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonString);
            List<object> result = new List<object> ();
            result.Add (dictionary);
            if (addChannel) {
                result.Add (multiChannel);
            }
            return result;
        }

        internal static void ProcessWrapResultBasedOnResponseTypeException<T> (ResponseType type, string[] channels, 
            Action<PubnubClientError> errorCallback, SafeDictionary<PubnubChannelCallbackKey, object> channelCallbacks, 
            PubnubErrorFilter.Level errorLevel, Exception ex)
        {
            if (channels != null) {
                if (type == ResponseType.Subscribe || type == ResponseType.Presence) {
                    PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (ex, channels, PubnubErrorSeverity.Critical, 
                        channelCallbacks, false, PubnubErrorCode.None, type, errorLevel);
                }
                else {
                    if (errorCallback != null) {
                        PubnubCallbacks.CallErrorCallback<T> (ex, null, string.Join (",", channels), PubnubErrorCode.None, 
                            PubnubErrorSeverity.Critical, errorCallback, errorLevel);
                    }
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
            return itemMessage;
        }

        internal static void ResponseToUserCallbackForSubscribeSendCallbacks<T> (List<object> result, string cipherKey, SafeDictionary<PubnubChannelCallbackKey, 
            object> channelCallbacks, IJsonPluggableLibrary jsonPluggableLibrary, object[] messages)
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

                    PubnubCallbacks.SendCallbacksBasedOnType<T>(channelCallbacks, jsonPluggableLibrary, currentChannel, itemMessage);

                }
            }
        }

        internal static void ResponseToUserCallbackForSubscribe<T> (List<object> result, ResponseType type, string[] channels, 
            Action<T> userCallback, string cipherKey, SafeDictionary<PubnubChannelCallbackKey, 
            object> channelCallbacks, IJsonPluggableLibrary jsonPluggableLibrary)
        {
            var messages = (from item in result
                select item as object).ToArray ();

            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, result: {1}", DateTime.Now.ToString (), result.ToString ()), LoggingMethod.LevelInfo);
            #endif

            if (messages != null && messages.Length > 0) {
                ResponseToUserCallbackForSubscribeSendCallbacks <T>(result, cipherKey, channelCallbacks, jsonPluggableLibrary, messages);
            }            
        }
            
        internal static void CheckResultListAndCallCallback<T>(List<object> result, Action<T> userCallback, 
            IJsonPluggableLibrary jsonPluggableLibrary){
            if (result != null && result.Count > 0) {
                PubnubCallbacks.GoToCallback<T> (result, userCallback, jsonPluggableLibrary);
            }
        }

        internal static void ResponseToUserCallback<T> (List<object> result, ResponseType type, string[] channels, 
            Action<T> userCallback, string cipherKey, SafeDictionary<PubnubChannelCallbackKey, 
            object> channelCallbacks, IJsonPluggableLibrary jsonPluggableLibrary)
        {
            switch (type) {
                case ResponseType.Subscribe:
                case ResponseType.Presence:
                    ResponseToUserCallbackForSubscribe<T>(result, type, channels, userCallback, cipherKey, channelCallbacks, jsonPluggableLibrary);
                break;
                case ResponseType.Leave:
                //No response to callback
                    break;
                case ResponseType.Publish:
                case ResponseType.DetailedHistory:
                case ResponseType.HereNow:
                case ResponseType.GlobalHereNow:
                case ResponseType.WhereNow:                
                case ResponseType.Time:
                case ResponseType.GrantAccess:
                case ResponseType.AuditAccess:
                case ResponseType.RevokeAccess:
                case ResponseType.GetUserState:
                case ResponseType.SetUserState:
                case ResponseType.PushRegister:
                case ResponseType.PushRemove:
                case ResponseType.PushGet:
                case ResponseType.PushUnregister:
                    CheckResultListAndCallCallback(result, userCallback, jsonPluggableLibrary);
                    break;
                default:
                    break;
            }
        }

        internal static void ResponseToConnectCallback<T> (List<object> result, ResponseType type, string[] channels, 
            Action<T> connectCallback, SafeDictionary<string, long> multiChannelSubscribe, SafeDictionary<PubnubChannelCallbackKey, 
            object> channelCallbacks, IJsonPluggableLibrary jsonPluggableLibrary)
        {
            //Check callback exists and make sure previous timetoken = 0
            if (channels != null && connectCallback != null
                && channels.Length > 0) {
                IEnumerable<string> newChannels = from channel in multiChannelSubscribe
                        where channel.Value == 0
                    select channel.Key;
                foreach (string channel in newChannels) {
                    switch (type) {
                    case ResponseType.Subscribe:
                        var connectResult = Helpers.CreateJsonResponse ("Connected", channel, jsonPluggableLibrary);
                        PubnubCallbacks.SendConnectCallback<T> (jsonPluggableLibrary, connectResult, channel, type, channelCallbacks);

                        break;
                    case ResponseType.Presence:
                        var connectResult2 = Helpers.CreateJsonResponse ("Presence Connected", 
                        channel.Replace (Utility.PresenceChannelSuffix, ""), jsonPluggableLibrary);
                        PubnubCallbacks.SendConnectCallback<T> (jsonPluggableLibrary, connectResult2, channel, type, channelCallbacks);

                        break;
                    default:
                        break;
                    }
                }
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

        internal static PubnubClientError CreatePubnubClientError<T>(WebException ex, 
            RequestState<T> requestState, string channel, PubnubErrorSeverity severity){

            PubnubErrorCode errorCode = PubnubErrorCodeHelper.GetErrorType (ex.Status, ex.Message);
            return CreatePubnubClientError<T> (ex, requestState, channel, errorCode, severity);
        }

        internal static PubnubErrorCode GetTimeOutErrorCode (ResponseType responseType)
        {
            switch(responseType){
            case ResponseType.AuditAccess:
            case ResponseType.GrantAccess:
            case ResponseType.RevokeAccess:
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
                 */
                return PubnubErrorCode.OperationTimeout;
            }
        }

        internal static PubnubClientError CreatePubnubClientError<T>(Exception ex, 
            RequestState<T> requestState, string channel, PubnubErrorCode errorCode, PubnubErrorSeverity severity){

            if (errorCode.Equals (PubnubErrorCode.None)) {
                errorCode = PubnubErrorCodeHelper.GetErrorType (ex);
            }

            int statusCode = (int)errorCode;
            string errorDescription = PubnubErrorCodeDescription.GetStatusCodeDescription (errorCode);
            PubnubWebRequest request = null;
            PubnubWebResponse response = null;
            string channelGroupsString = null;

            if (requestState == null) {
                request = requestState.Request;
                response = requestState.Response;
                channelGroupsString = (requestState.ChannelGroups != null) ? string.Join(",", requestState.ChannelGroups) : "";
            }

            PubnubClientError error = new PubnubClientError (statusCode, severity, true, ex.Message, ex, 
                PubnubMessageSource.Client, request, response, errorDescription, channel, channelGroupsString);

            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PubnubClientError = {1}", DateTime.Now.ToString (), error.ToString ()), LoggingMethod.LevelInfo);
            #endif
            return error;
        }

        internal static PubnubClientError CreatePubnubClientError<T>(string message, 
            RequestState<T> requestState, string channel, PubnubErrorCode errorCode, PubnubErrorSeverity severity){

            int statusCode = (int)errorCode;
            string errorDescription = PubnubErrorCodeDescription.GetStatusCodeDescription (errorCode);

            PubnubWebRequest request = null;
            PubnubWebResponse response = null;
            string channelGroupsString = null;

            if (requestState == null) {
                request = requestState.Request;
                response = requestState.Response;
                channelGroupsString = (requestState.ChannelGroups != null) ? string.Join(",", requestState.ChannelGroups) : "";
            }

            PubnubClientError error = new PubnubClientError (statusCode, severity, message, PubnubMessageSource.Client, 
                request, response, errorDescription, channel, channelGroupsString);

            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PubnubClientError = {1}", DateTime.Now.ToString (), error.ToString ()), LoggingMethod.LevelInfo);
            #endif
            return error;
        }

        #endregion

    }
}
