using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace PubNubAPI
{
    public sealed class Counter
    {
        private uint current = 0;
        private object syncRoot;

        public Counter(){
            syncRoot = new object();
            current = 0;
        }

        public uint NextValue()
        {
            lock (syncRoot) {
                this.current++;
            }
            return this.current;
        }

        public void Reset()
        {
            lock (syncRoot) {
                this.current = 0;
            }
        }
    }

    public static class Helpers
    {
        internal const string PresenceChannelSuffix = "-pnpres";

        #region "Helpers"

        internal static bool CheckRequestTimeoutMessageInError(CustomEventArgs cea){
            if (cea.IsError && cea.Message.ToString().Contains ("The request timed out.")) {
                return true;
            } else {
                return false;
            }
        }

        public static bool CheckErrorTypeAndCallback<T> (CustomEventArgs cea, PubNubUnity pnUnity, out PNStatus pnStatus){
            bool retBool = false;
            PNStatusCategory pnStatusCat = PNStatusCategory.PNUnknownCategory;
            if (cea.IsTimeout || CheckRequestTimeoutMessageInError(cea)){
                pnStatusCat = PNStatusCategory.PNTimeoutCategory;
                retBool = true;
            } else if(cea.IsError){
                if ((cea.Message.Contains ("NameResolutionFailure")
                    || cea.Message.Contains ("ConnectFailure")
                    || cea.Message.Contains ("ServerProtocolViolation")
                    || cea.Message.Contains ("ProtocolError")
                )) {
                    pnStatusCat = PNStatusCategory.PNNetworkIssuesCategory;
                } else if(cea.Message.Contains ("Aborted")){
                    pnStatusCat = PNStatusCategory.PNCancelledCategory;
                } else if ((cea.Message.Contains ("403")) 
                    || (cea.Message.Contains ("java.io.FileNotFoundException")) 
                    || ((pnUnity.Version.Contains("UnityWeb")) && (cea.Message.Contains ("Failed downloading")))) {
                        pnStatusCat = PNStatusCategory.PNAccessDeniedCategory;
                } else if (cea.PubNubRequestState != null){
                    if(cea.PubNubRequestState.ResponseCode.Equals(403)){
                        pnStatusCat = PNStatusCategory.PNAccessDeniedCategory;
                    } else if (cea.PubNubRequestState.ResponseCode.Equals ("500")){
                        pnStatusCat = PNStatusCategory.PNUnexpectedDisconnectCategory;
                    } else if (cea.PubNubRequestState.ResponseCode.Equals ("502")){
                        pnStatusCat = PNStatusCategory.PNUnexpectedDisconnectCategory;
                    } else if (cea.PubNubRequestState.ResponseCode.Equals ("503")){
                        pnStatusCat = PNStatusCategory.PNUnexpectedDisconnectCategory;  
                    } else if (cea.PubNubRequestState.ResponseCode.Equals ("504")){
                        pnStatusCat = PNStatusCategory.PNTimeoutCategory;  
                    } else if (cea.PubNubRequestState.ResponseCode.Equals ("414")){
                        pnStatusCat = PNStatusCategory.PNBadRequestCategory;  
                    } else if (cea.PubNubRequestState.ResponseCode.Equals ("481")){
                        pnStatusCat = PNStatusCategory.PNUnexpectedDisconnectCategory;
                    } else if (cea.PubNubRequestState.ResponseCode.Equals ("451")){
                        pnStatusCat = PNStatusCategory.PNUnexpectedDisconnectCategory;
                    } else if (cea.PubNubRequestState.ResponseCode.Equals ("400")){
                        pnStatusCat = PNStatusCategory.PNBadRequestCategory;  
                    } 
                } 
                retBool = true;
            } else {
                retBool = false;
            }

            if(retBool){
                pnStatus = CreatePNStatus(
                        pnStatusCat,
                        cea.Message,
                        null,
                        retBool,
                        PNOperationType.PNSubscribeOperation,
                        pnUnity.SubscriptionInstance.AllChannels,
                        pnUnity.SubscriptionInstance.AllChannelGroups,
                        cea.PubNubRequestState,
                        pnUnity
                    );
            } else {
                pnStatus = null;
            }
            return retBool;

        }

        #region "CreatePNStatus"
        internal static PNStatus CreatePNStatus(PNStatusCategory category, PNErrorData errorData, bool error, PNOperationType operation, List<string> channels, List<string> channelGroups, RequestState pnRequestState, PubNubUnity pnUnity){
            long statusCode = 0;
            string url = "";
            if(pnRequestState != null){
                statusCode = pnRequestState.ResponseCode;
                url = pnRequestState.URL;
            }

            PNStatus pnStatus = new PNStatus(
                category,
                errorData,
                error,
                statusCode,
                operation,
                pnUnity.PNConfig.Secure,
                pnUnity.PNConfig.UUID,
                pnUnity.PNConfig.AuthKey,
                pnUnity.PNConfig.Origin,
                channels,
                channelGroups,
                url
            );

            #if (ENABLE_PUBNUB_LOGGING)
            pnUnity.PNLog.WriteToLog (string.Format ("CreatePNStatus: \n" + 
                "category={0} \n" +
                "errorData={1} \n" +
                "error={2} \n" +
                "statusCode={3} \n" +
                "operation={4} \n" +
                "tlsEnabled={5} \n" +
                "uuid={6} \n" +
                "authKey={7} \n" +
                "origin={8} \n" +
                "channels={9} \n" +
                "channelGroups={10} \n" +
                "clientRequest={11} \n" , 
                category.ToString(), 
                ((errorData != null) && (errorData.Ex != null)) ? errorData.Ex.ToString() : "null", 
                error.ToString(),
                statusCode.ToString(),
                operation.ToString(),
                pnUnity.PNConfig.Secure,
                pnUnity.PNConfig.UUID,
                pnUnity.PNConfig.AuthKey,
                pnUnity.PNConfig.Origin,
                (channels != null) ? string.Join(",", channels.ToArray()) : "null",
                (channelGroups != null) ? string.Join(",", channelGroups.ToArray()) : "null",
                //(clientRequest != null) ? clientRequest.ToString() : "null"
                url
                ), PNLoggingMethod.LevelInfo);
            #endif

            return pnStatus;
        }

        internal static PNStatus CreatePNStatus(PNStatusCategory category, string errorString, Exception errorException, bool error, PNOperationType operation, List<ChannelEntity> affectedChannels, List<ChannelEntity> affectedChannelGroups, RequestState pnRequestState, PubNubUnity pnUnity){
            PNErrorData errorData = CreateErrorData(errorString, errorException);
            
            List<string> channels = CreateListOfStringFromListOfChannelEntity(affectedChannels);
            List<string> channelGroups = CreateListOfStringFromListOfChannelEntity(affectedChannelGroups);

            return CreatePNStatus(category, errorData, error, operation, channels, channelGroups, pnRequestState, pnUnity);
        }

        internal static PNStatus CreatePNStatus(PNStatusCategory category, string errorString, Exception errorException, bool error, PNOperationType operation, List<string> channels, List<string> channelGroups, RequestState pnRequestState, PubNubUnity pnUnity){
            PNErrorData errorData = CreateErrorData(errorString, errorException);

            return CreatePNStatus(category, errorData, error, operation, channels, channelGroups, pnRequestState, pnUnity);
        }

        internal static PNStatus CreatePNStatus(PNStatusCategory category, string errorString, Exception errorException, bool error, PNOperationType operation, ChannelEntity channelEntity, RequestState pnRequestState, PubNubUnity pnUnity){
            PNErrorData errorData = null;
            if((!string.IsNullOrEmpty(errorString)) || (errorException != null)){
                errorData = new PNErrorData();
                errorData.Info = errorString;
                errorData.Ex = errorException;
            }
            List<string> affectedChannels = null;
            List<string> affectedChannelGroups = null;

            if(channelEntity.ChannelID.IsChannelGroup){
                affectedChannelGroups = new List<string>();
                affectedChannelGroups.Add(channelEntity.ChannelID.ChannelOrChannelGroupName);
            } else {
                affectedChannels = new List<string>();
                affectedChannels.Add(channelEntity.ChannelID.ChannelOrChannelGroupName);
            }

            return CreatePNStatus(category, errorData, error, operation, affectedChannels, affectedChannelGroups, pnRequestState, pnUnity);

        }

        #endregion
        
        internal static List<string> CreateListOfStringFromListOfChannelEntity(List<ChannelEntity> channelEntities){
            List<string> channelList = null;
            if(channelEntities != null){
                channelList = new List<string>();
                foreach(ChannelEntity ce in channelEntities){
                    channelList.Add(ce.ChannelID.ChannelOrChannelGroupName);
                }
            }
            return channelList;
        }

        internal static PNErrorData CreateErrorData(string errorString, Exception errorException){
            PNErrorData errorData = null;
            if((!string.IsNullOrEmpty(errorString)) || (errorException != null)){
                errorData = new PNErrorData();
                errorData.Info = errorString;
                errorData.Ex = errorException;
            }
            return errorData;
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

        public static string JsonEncodePublishMsg (object originalMessage, string cipherKey, IJsonLibrary jsonPluggableLibrary, PNLoggingMethod pnLog)
        {
            string message = jsonPluggableLibrary.SerializeToJsonString (originalMessage);

            if (cipherKey.Length > 0) {
                PubnubCrypto aes = new PubnubCrypto (cipherKey, pnLog);
                string encryptMessage = aes.Encrypt (message);
                message = jsonPluggableLibrary.SerializeToJsonString (encryptMessage);
            }

            return message;
        }
        internal static string GetNamesFromChannelEntities (List<ChannelEntity> channelEntities, bool isChannelGroup){

            StringBuilder sb = new StringBuilder ();
            if (channelEntities != null) {
                int count = 0;
                foreach (ChannelEntity c in channelEntities) {
                    if (isChannelGroup && c.ChannelID.IsChannelGroup) {
                        if (count > 0) {
                            sb.Append (",");
                        }

                        sb.Append (c.ChannelID.ChannelOrChannelGroupName);
                        count++;
                    } else if (!isChannelGroup && !c.ChannelID.IsChannelGroup) {
                        if (count > 0) {
                            sb.Append (",");
                        }

                        sb.Append (c.ChannelID.ChannelOrChannelGroupName);
                        count++;
                    }
                }
            }
            return sb.ToString();
        }

        internal static TimetokenMetadata CreateTimetokenMetadata (object timeTokenDataObject, string whichTT, ref PNLoggingMethod pnLog)
        {
            Dictionary<string, object> timeTokenData = (Dictionary<string, object>)timeTokenDataObject;
            string log;
            long timetoken = Utility.CheckKeyAndParseLong(timeTokenData, whichTT, "t", out log);
            //TODO use trygetvalue
            TimetokenMetadata timetokenMetadata = new TimetokenMetadata (timetoken, (timeTokenData.ContainsKey ("r")) ? timeTokenData["r"].ToString(): "");

            #if (ENABLE_PUBNUB_LOGGING)
            pnLog.WriteToLog (string.Format ("TimetokenMetadata: {0} \nTimetoken: {1} \nRegion: {2}\nlog : {3}", whichTT, timetokenMetadata.Timetoken, timetokenMetadata.Region, log), PNLoggingMethod.LevelInfo);
            #endif

            return timetokenMetadata;
        }

        internal static List<object> DeserializeAndAddToResult (string jsonString, string multiChannel, IJsonLibrary jsonPluggableLibrary, bool addChannel)
        {
            Dictionary<string, object> dictionary = jsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonString);
            List<object> result = new List<object> ();
            result.Add (dictionary);
            if (addChannel) {
                result.Add (multiChannel);
            }
            return result;
        }

        internal static IEnumerable<string> GetDuplicates(List<string> rawChannels)
        {
            var results = from string a in rawChannels
                group a by a into g
                    where g.Count() > 1
                select g;
            foreach (var group in results)
                foreach (var item in group)
                    yield return item;
        }

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

        internal static bool IsPresenceChannel (string channel)
        {
            if (channel.LastIndexOf (PresenceChannelSuffix) > 0) {
                return true;
            } else {
                return false;
            }
        }

        internal static ChannelEntity CreateChannelEntity(string channelOrChannelGroupName2, bool isAwaitingConnectCallback, bool isChannelGroup, Dictionary<string, object> userState, ref PNLoggingMethod pnLog){
            string channelOrChannelGroupName = channelOrChannelGroupName2.Trim ();
            #if (ENABLE_PUBNUB_LOGGING)
            pnLog.WriteToLog(string.Format("CreateChannelEntity: channelOrChannelGroupName {0}, {1}", channelOrChannelGroupName.ToString(), channelOrChannelGroupName2.ToString()), PNLoggingMethod.LevelInfo);
            #endif

            if (!string.IsNullOrEmpty(channelOrChannelGroupName)) {
                ChannelIdentity ci = new ChannelIdentity ();
                ci.ChannelOrChannelGroupName = channelOrChannelGroupName;
                ci.IsPresenceChannel = IsPresenceChannel (channelOrChannelGroupName);
                ci.IsChannelGroup = isChannelGroup;

                ChannelParameters cp = new ChannelParameters ();
                cp.IsAwaitingConnectCallback = isAwaitingConnectCallback;
                cp.UserState = userState;
                //cp.TypeParameterType = typeof(T);

                //cp.Callbacks = PubnubCallbacks.GetPubnubChannelCallback<T> (userCallback, connectCallback, errorCallback, 
                  //  disconnectCallback, wildcardPresenceCallback);

                ChannelEntity ce = new ChannelEntity (ci, cp);

                return ce;
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                pnLog.WriteToLog(string.Format("CreateChannelEntity: channelOrChannelGroupName empty, is channel group {0}", isChannelGroup.ToString()), PNLoggingMethod.LevelInfo);
                #endif

                return null;
            }
        }

        internal static List<ChannelEntity> CreateChannelEntity(string[] channelOrChannelGroupNames, bool isAwaitingConnectCallback, bool isChannelGroup, Dictionary<string, object> userState, ref PNLoggingMethod pnLog){
            List<ChannelEntity> channelEntities = null;
            if (channelOrChannelGroupNames != null) {
                channelEntities = new List<ChannelEntity> ();
                foreach (string ch in channelOrChannelGroupNames) {
                    ChannelEntity chEntity= CreateChannelEntity (ch, isAwaitingConnectCallback, isChannelGroup, userState, ref pnLog);
                    if (chEntity != null) {
                        channelEntities.Add (chEntity);
                    }
                }
                #if (ENABLE_PUBNUB_LOGGING)
                pnLog.WriteToLog (string.Format ("CreateChannelEntity 2: channelEntities={0}",  channelEntities.Count), PNLoggingMethod.LevelInfo);
                #endif
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                pnLog.WriteToLog(string.Format("CreateChannelEntity 2: channelOrChannelGroupNames null, is channel group {0}", isChannelGroup.ToString()), PNLoggingMethod.LevelInfo);
                #endif
            }

            return channelEntities;
        }

        internal static string BuildJsonUserState (List<ChannelEntity> listChannelEntities)
        {
            string retJsonUserState = "";

            StringBuilder jsonStateBuilder = new StringBuilder ();

            if (listChannelEntities != null) {
                foreach (ChannelEntity c in listChannelEntities) {
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

        internal static object DecodeMessage (string cipherKey, object element, IJsonLibrary jsonPluggableLibrary, PNLoggingMethod pnLog)
        {
            PubnubCrypto aes = new PubnubCrypto (cipherKey, pnLog);
            string decryptMessage = "";
            try {
                decryptMessage = aes.Decrypt (element.ToString ());
            }
            catch (Exception ex) {
                decryptMessage = "**DECRYPT ERROR**";
                throw ex;
                //PubnubCallbacks.CallErrorCallback<object> (ex, channelEntities, PubnubErrorCode.None, PubnubErrorSeverity.Critical, errorLevel);
            }
            object decodeMessage = (decryptMessage == "**DECRYPT ERROR**") ? decryptMessage : jsonPluggableLibrary.DeserializeToObject (decryptMessage);
            return decodeMessage;
        }

        internal static void AddToSubscribeMessageList (object dictObject, ref List<SubscribeMessage> subscribeMessages, ref PNLoggingMethod pnLog)
        {
            var dict = dictObject as IDictionary;      
            if ((dict != null) && (dict.Count > 1)) {
                string shard = (dict.Contains ("a")) ? dict ["a"].ToString () : "";
                string subscriptionMatch = (dict.Contains ("b")) ? dict ["b"].ToString () : "";
                string channel = (dict.Contains ("c")) ? dict ["c"].ToString () : "";
                object payload = (dict.Contains ("d")) ? (object)dict ["d"] : null;
                string flags = (dict.Contains ("f")) ? dict ["f"].ToString () : "";
                string issuingClientId = (dict.Contains ("i")) ? dict ["i"].ToString () : "";
                string subscribeKey = (dict.Contains ("k")) ? dict ["k"].ToString () : "";
                string log; 
                long sequenceNumber = Utility.CheckKeyAndParseLong (dict, "sequenceNumber", "s", out log);
                
                TimetokenMetadata originatingTimetoken = (dict.Contains ("o")) ? Helpers.CreateTimetokenMetadata (dict ["o"], "Originating TT: ", ref pnLog) : null;
                TimetokenMetadata publishMetadata = (dict.Contains ("p")) ? Helpers.CreateTimetokenMetadata (dict ["p"], "Publish TT: ", ref pnLog) : null;
                object userMetadata = (dict.Contains ("u")) ? (object)dict ["u"] : null;

                SubscribeMessage subscribeMessage = new SubscribeMessage (
                    shard,
                    subscriptionMatch,
                    channel,
                    payload,
                    flags,
                    issuingClientId,
                    subscribeKey,
                    sequenceNumber,
                    originatingTimetoken,
                    publishMetadata,
                    userMetadata
                );
                #if (ENABLE_PUBNUB_LOGGING)
                pnLog.WriteToLog (string.Format ("AddToSubscribeMessageList: \n" +
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
                "userMetadata {12} \n" +
                "log {13} \n",
                 
                shard,
                subscriptionMatch,
                channel,
                payload.ToString (),
                flags,
                issuingClientId,
                subscribeKey,
                sequenceNumber,
                (originatingTimetoken != null) ? originatingTimetoken.Timetoken.ToString () : "",
                (originatingTimetoken != null) ? originatingTimetoken.Region : "",
                (publishMetadata != null) ? publishMetadata.Timetoken.ToString () : "",
                (publishMetadata != null) ? publishMetadata.Region : "",
                (userMetadata != null) ? userMetadata.ToString () : "null",
                log), 
                PNLoggingMethod.LevelInfo);
                #endif

                subscribeMessages.Add (subscribeMessage);
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                pnLog.WriteToLog (string.Format ("AddToSubscribeMessageList: CreateListOfSubscribeMessage create SubscribeMessage failed. dictObject type: {0}, dict type : {1}", dictObject.ToString (), dict.ToString ()), PNLoggingMethod.LevelInfo);
            }
            #endif
        }

        internal static List<SubscribeMessage> CreateListOfSubscribeMessage (object message, ref PNLoggingMethod pnLog)
        {
            List<SubscribeMessage> subscribeMessages = new List<SubscribeMessage> ();
            if (message != null) {
                //JSONFx
                object[] messages = message as object[];

                if (messages != null) {
                    var myObjectArray = (from item in messages
                        select item as object).ToArray ();
                    if ((myObjectArray!= null) && (myObjectArray.Length > 0)) {

                        foreach (object dictObject in myObjectArray) {
                            AddToSubscribeMessageList (dictObject, ref subscribeMessages, ref pnLog);
                        }
                    }
                } else {
                    //MiniJSON
                    List<object> messageList = message as List<object>;
                    if ((messageList != null) && messageList.Count > 0) {
                        foreach (object dictObject in messageList) {
                            AddToSubscribeMessageList (dictObject, ref subscribeMessages, ref pnLog);
                        }
                    }
                }
            }
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                pnLog.WriteToLog (string.Format ("CreateListOfSubscribeMessage: no messages "), PNLoggingMethod.LevelInfo);
            }
            #endif

            return subscribeMessages;
        }
        /*internal static List<object> DecryptCipheredMessage (List<object> message, List<ChannelEntity> channelEntities, string cipherKey, IJsonPluggableLibrary jsonPluggableLibrary, PubnubErrorFilter.Level errorLevel)
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

        internal static List<object> DecodeDecryptLoop (List<object> message, List<ChannelEntity> channelEntities, string cipherKey, IJsonPluggableLibrary jsonPluggableLibrary, PubnubErrorFilter.Level errorLevel)
        {
            if (cipherKey.Length > 0) {
                return DecryptCipheredMessage (message, channelEntities, cipherKey, jsonPluggableLibrary, errorLevel);
            } else {
                return DecryptNonCipheredMessage (message);
            }
        }  */  
        /*public static KeyValuePair<object, object > Cast<K, V>(this KeyValuePair<K, V> kvp)
        {
            return new KeyValuePair<object, object>(kvp.Key, kvp.Value);
        }

        public static KeyValuePair<T, V> CastFrom<T, V>(object obj)
        {
            return (KeyValuePair<T, V>) obj;
        }

        public static KeyValuePair<object , object > CastFrom(object obj)
        {
            var type = obj.GetType();
            if (type.IsGenericType)
            {
                if (type == typeof (KeyValuePair<,>))
                {
                    var key = type.GetProperty("Key");
                    var value = type.GetProperty("Value");
                    var keyObj = key.GetValue(obj, null);
                    var valueObj = value.GetValue(obj, null);
                    return new KeyValuePair<object, object>(keyObj, valueObj);
                }
            }
            throw new ArgumentException(" ### -> public static KeyValuePair<object , object > CastFrom(Object obj) : Error : obj argument must be KeyValuePair<,>");
        }*/

        /*public static T ConvertValue<T,W>(W value) where W : IConvertible
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static T ConvertValue<T>(string value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }*/
        #endregion
    }
}