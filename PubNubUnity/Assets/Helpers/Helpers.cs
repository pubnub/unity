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
        private uint current;
        private readonly object syncRoot;

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
        #region "Helpers"

        public static bool CheckRequestTimeoutMessageInError(CustomEventArgs cea){
            if (cea.IsError && cea.Message.ToString().Contains ("The request timed out.")) {
                return true;
            } else {
                return false;
            }
        }

        public static bool TryCheckErrorTypeAndCallback<T> (CustomEventArgs cea, PubNubUnity pnUnity, out PNStatus pnStatus){
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
                    switch(cea.PubNubRequestState.ResponseCode){
                        case 403:
                            pnStatusCat = PNStatusCategory.PNAccessDeniedCategory;
                        break;
                        case 500:
                        case 502:
                        case 503:
                        case 481:
                        case 451:
                            pnStatusCat = PNStatusCategory.PNUnexpectedDisconnectCategory;
                        break;
                        case 504:
                            pnStatusCat = PNStatusCategory.PNTimeoutCategory;
                        break;
                        case 414:
                        case 400:
                            pnStatusCat = PNStatusCategory.PNBadRequestCategory;
                        break;
                        default:
                            pnStatusCat = PNStatusCategory.PNUnknownCategory;
                        break;
                        
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
                url
                ), PNLoggingMethod.LevelInfo);
            #endif

            return pnStatus;
        }

        public static PNStatus CreatePNStatus(PNStatusCategory category, string errorString, Exception errorException, bool error, PNOperationType operation, List<ChannelEntity> affectedChannels, List<ChannelEntity> affectedChannelGroups, RequestState pnRequestState, PubNubUnity pnUnity){
            PNErrorData errorData = CreateErrorData(errorString, errorException);
            
            List<string> channels = CreateListOfStringFromListOfChannelEntity(affectedChannels);
            List<string> channelGroups = CreateListOfStringFromListOfChannelEntity(affectedChannelGroups);

            return CreatePNStatus(category, errorData, error, operation, channels, channelGroups, pnRequestState, pnUnity);
        }

        public static PNStatus CreatePNStatus(PNStatusCategory category, string errorString, Exception errorException, bool error, PNOperationType operation, List<string> channels, List<string> channelGroups, RequestState pnRequestState, PubNubUnity pnUnity){
            PNErrorData errorData = CreateErrorData(errorString, errorException);

            return CreatePNStatus(category, errorData, error, operation, channels, channelGroups, pnRequestState, pnUnity);
        }

        public static PNStatus CreatePNStatus(PNStatusCategory category, string errorString, Exception errorException, bool error, PNOperationType operation, ChannelEntity channelEntity, RequestState pnRequestState, PubNubUnity pnUnity){
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

        public static string GetAllNamesFromChannelEntities (List<ChannelEntity> channelEntities, bool descriptive){
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
            if(descriptive){
                return string.Format ("channel(s) = {0} and channelGroups(s) = {1}", sbCh.ToString(), sbChGrp.ToString());
            } else {
                return string.Format ("{0},{1}", sbCh.ToString(), sbChGrp.ToString());
            }
            
        }

        public static string JsonEncodePublishMsg (object originalMessage, string cipherKey, IJsonLibrary jsonPluggableLibrary, PNLoggingMethod pnLog)
        {
            string message = jsonPluggableLibrary.SerializeToJsonString (originalMessage);

            if (!string.IsNullOrEmpty(cipherKey) && (cipherKey.Length > 0)) {
                PubnubCrypto aes = new PubnubCrypto (cipherKey, pnLog);
                string encryptMessage = aes.Encrypt (message);
                message = jsonPluggableLibrary.SerializeToJsonString (encryptMessage);
            }

            return message;
        }
        public static string GetNamesFromChannelEntities (List<ChannelEntity> channelEntities, bool isChannelGroup){

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

        public static TimetokenMetadata CreateTimetokenMetadata (object timeTokenDataObject, string whichTT, PNLoggingMethod pnLog)
        {
            Dictionary<string, object> timeTokenData = (Dictionary<string, object>)timeTokenDataObject;
            string log;
            long timetoken;
            Utility.TryCheckKeyAndParseLong(timeTokenData, whichTT, "t", out log, out timetoken);
            TimetokenMetadata timetokenMetadata = new TimetokenMetadata (timetoken, (timeTokenData.ContainsKey ("r")) ? timeTokenData["r"].ToString(): "");

            #if (ENABLE_PUBNUB_LOGGING)
            pnLog.WriteToLog (string.Format ("TimetokenMetadata: {0} \nTimetoken: {1} \nRegion: {2}\nlog : {3}", whichTT, timetokenMetadata.Timetoken, timetokenMetadata.Region, log), PNLoggingMethod.LevelInfo);
            #endif

            return timetokenMetadata;
        }

        public static List<object> DeserializeAndAddToResult (string jsonString, string multiChannel, IJsonLibrary jsonPluggableLibrary, bool addChannel)
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
            foreach (var group in results){
                foreach (var item in group){
                    yield return item;
                }
            }
        }

        public static string BuildJsonUserState (Dictionary<string, object> userStateDictionary)
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

        public static ChannelEntity CreateChannelEntity(string channelOrChannelGroupName2, bool isAwaitingConnectCallback, bool isChannelGroup, Dictionary<string, object> userState, PNLoggingMethod pnLog){
            string channelOrChannelGroupName = channelOrChannelGroupName2.Trim ();
            #if (ENABLE_PUBNUB_LOGGING)
            pnLog.WriteToLog(string.Format("CreateChannelEntity: channelOrChannelGroupName {0}, {1}", channelOrChannelGroupName.ToString(), channelOrChannelGroupName2.ToString()), PNLoggingMethod.LevelInfo);
            #endif

            if (!string.IsNullOrEmpty(channelOrChannelGroupName)) {
                ChannelIdentity ci = new ChannelIdentity ();
                ci.ChannelOrChannelGroupName = channelOrChannelGroupName;
                ci.IsPresenceChannel = Utility.IsPresenceChannel (channelOrChannelGroupName);
                ci.IsChannelGroup = isChannelGroup;

                ChannelParameters cp = new ChannelParameters ();
                cp.IsAwaitingConnectCallback = isAwaitingConnectCallback;
                cp.UserState = userState;
                ChannelEntity ce = new ChannelEntity (ci, cp);

                return ce;
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                pnLog.WriteToLog(string.Format("CreateChannelEntity: channelOrChannelGroupName empty, is channel group {0}", isChannelGroup.ToString()), PNLoggingMethod.LevelInfo);
                #endif

                return null;
            }
        }

        public static List<ChannelEntity> CreateChannelEntity(string[] channelOrChannelGroupNames, bool isAwaitingConnectCallback, bool isChannelGroup, Dictionary<string, object> userState, PNLoggingMethod pnLog){
            List<ChannelEntity> channelEntities = null;
            if (channelOrChannelGroupNames != null) {
                channelEntities = new List<ChannelEntity> ();
                foreach (string ch in channelOrChannelGroupNames) {
                    ChannelEntity chEntity= CreateChannelEntity (ch, isAwaitingConnectCallback, isChannelGroup, userState, pnLog);
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

        public static string BuildJsonUserState (List<ChannelEntity> listChannelEntities)
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

        public static object DecodeMessage (string cipherKey, object element, PNOperationType pnOperationType, PubNubUnity pnUnity)
        {
            PubnubCrypto aes = new PubnubCrypto (cipherKey, pnUnity.PNLog);
            string decryptMessage = "";
            try {
                decryptMessage = aes.Decrypt (element.ToString ());
            }
            catch (Exception ex) {
                decryptMessage = "**DECRYPT ERROR**";
                PNStatus pnStatus = CreatePNStatus(
                        PNStatusCategory.PNDecryptionErrorCategory,
                        string.Format("{0}, {1}", decryptMessage, element.ToString()), 
                        ex,
                        true,
                        PNOperationType.PNSubscribeOperation,
                        pnUnity.SubscriptionInstance.AllChannels,
                        pnUnity.SubscriptionInstance.AllChannelGroups,
                        null,
                        pnUnity
                    );
                pnUnity.SubWorker.CreateEventArgsAndRaiseEvent(pnStatus);    
            }
            object decodeMessage = (decryptMessage == "**DECRYPT ERROR**") ? decryptMessage : pnUnity.JsonLibrary.DeserializeToObject (decryptMessage);
            return decodeMessage;
        }

        public static bool TryAddToSubscribeMessageList (object dictObject, ref List<SubscribeMessage> subscribeMessages, PNLoggingMethod pnLog)
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
                long sequenceNumber;
                Utility.TryCheckKeyAndParseLong (dict, "sequenceNumber", "s", out log, out sequenceNumber);
                
                TimetokenMetadata originatingTimetoken = (dict.Contains ("o")) ? Helpers.CreateTimetokenMetadata (dict ["o"], "Originating TT: ", pnLog) : null;
                TimetokenMetadata publishMetadata = (dict.Contains ("p")) ? Helpers.CreateTimetokenMetadata (dict ["p"], "Publish TT: ", pnLog) : null;
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
                return true;
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                pnLog.WriteToLog (string.Format ("AddToSubscribeMessageList: CreateListOfSubscribeMessage create SubscribeMessage failed. dictObject type: {0}, dict type : {1}", dictObject.ToString (), dict.ToString ()), PNLoggingMethod.LevelInfo);
            }
            #endif
            return false;
        }

        public static List<SubscribeMessage> CreateListOfSubscribeMessage (object message, PNLoggingMethod pnLog)
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
                             TryAddToSubscribeMessageList (dictObject, ref subscribeMessages, pnLog);
                        }
                    }
                } else {
                    //MiniJSON
                    List<object> messageList = message as List<object>;
                    if ((messageList != null) && messageList.Count > 0) {
                        foreach (object dictObject in messageList) {
                            TryAddToSubscribeMessageList (dictObject, ref subscribeMessages, pnLog);
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

        #endregion
    }
}