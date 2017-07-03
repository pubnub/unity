using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Collections;
using System.Text;
using UnityEngine;

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

    internal static class Helpers
    {
        #region "Helpers"
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

        internal static string JsonEncodePublishMsg (object originalMessage, string cipherKey, IJsonLibrary jsonPluggableLibrary)
        {
            string message = jsonPluggableLibrary.SerializeToJsonString (originalMessage);

            if (cipherKey.Length > 0) {
                PubnubCrypto aes = new PubnubCrypto (cipherKey);
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

        internal static void LogChannelEntitiesDictionary(PubNubUnity pn){
            StringBuilder sbLogs = new StringBuilder();
            foreach (var ci in pn.SubscriptionInstance.ChannelEntitiesDictionary) {
                sbLogs.AppendFormat("\nChannelEntitiesDictionary \nChannelOrChannelGroupName:{0} \nIsChannelGroup:{1} \nIsPresenceChannel:{2}\n", ci.Key.ChannelOrChannelGroupName, ci.Key.IsChannelGroup.ToString(), ci.Key.IsPresenceChannel.ToString());
            }
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("LogChannelEntitiesDictionary: Count: {2} \n{1}", sbLogs.ToString(), pn.SubscriptionInstance.ChannelEntitiesDictionary.Count), LoggingMethod.LevelInfo, pn.PNConfig.LogVerbosity);
            #endif
        }

        internal static TimetokenMetadata CreateTimetokenMetadata (object timeTokenDataObject, string whichTT, PNLogVerbosity pnLogVerbosity)
        {
            Dictionary<string, object> timeTokenData = (Dictionary<string, object>)timeTokenDataObject;
            string log;
            long timetoken = Utility.CheckKeyAndParseLong(timeTokenData, whichTT, "t", out log);
            TimetokenMetadata timetokenMetadata = new TimetokenMetadata (timetoken, (timeTokenData.ContainsKey ("r")) ? timeTokenData["r"].ToString(): "");

            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("TimetokenMetadata: {1} \nTimetoken: {2} \nRegion: {3}\nlog : {4}", whichTT, timetokenMetadata.Timetoken, timetokenMetadata.Region, log), LoggingMethod.LevelInfo, pnLogVerbosity);
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

        internal static void AddToSubscribeMessageList (object dictObject, ref List<SubscribeMessage> subscribeMessages, PNLogVerbosity pnLogVerbosity)
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
                
                TimetokenMetadata originatingTimetoken = (dict.Contains ("o")) ? CreateTimetokenMetadata (dict ["o"], "Originating TT: ", pnLogVerbosity) : null;
                TimetokenMetadata publishMetadata = (dict.Contains ("p")) ? CreateTimetokenMetadata (dict ["p"], "Publish TT: ", pnLogVerbosity) : null;
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
                LoggingMethod.WriteToLog (string.Format ("AddToSubscribeMessageList: \n" +
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
                "userMetadata {13} \n" +
                "log {14} \n",
                 
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
                LoggingMethod.LevelInfo, pnLogVerbosity);
                #endif

                subscribeMessages.Add (subscribeMessage);
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                LoggingMethod.WriteToLog (string.Format ("AddToSubscribeMessageList: CreateListOfSubscribeMessage create SubscribeMessage failed. dictObject type: {1}, dict type : {2}", dictObject.ToString (), dict.ToString ()), LoggingMethod.LevelInfo, pnLogVerbosity);
            }
            #endif
        }

        internal static List<SubscribeMessage> CreateListOfSubscribeMessage (object message, PNLogVerbosity pnLogVerbosity)
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
                            AddToSubscribeMessageList (dictObject, ref subscribeMessages, pnLogVerbosity);
                        }
                    }
                } else {
                    //MiniJSON
                    List<object> messageList = message as List<object>;
                    if ((messageList != null) && messageList.Count > 0) {
                        foreach (object dictObject in messageList) {
                            AddToSubscribeMessageList (dictObject, ref subscribeMessages, pnLogVerbosity);
                        }
                    }
                }
            }
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                LoggingMethod.WriteToLog (string.Format ("CreateListOfSubscribeMessage: no messages ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo, pnLogVerbosity);
            }
            #endif

            return subscribeMessages;
        }

        internal static void CreateHistoryResult(string cipherKey, object deSerializedResult, ref PNResult result, ref PNStatus pnStatus){
            //[[{"message":{"text":"hey"},"timetoken":14985452911089049}],14985452911089049,14985452911089049] 
            //[[{"text":"hey"}],14985452911089049,14985452911089049]
            /*try{
                PNHistoryResult pnHistoryResult = new PNHistoryResult();
                var myObjectArray = deSerializedResult as object[];
                if(myObjectArray != null){
                    foreach (var it in myObjectArray){
                        Debug.Log(it);
                    }
                    Dictionary<string, object>[] historyMessages = deSerializedResult as Dictionary<string, object>[];
                    foreach(var historyMessage in historyMessages){
                        if(cipherKey.Length > 0){
                            PubnubCrypto aes = new PubnubCrypto (cipherKey);
                            //Decrypt();
                            aes.Decrypt();
                        } 
                        //non encrypted, 
                        if(Utility.IsDictionary(historyMessage)){
                            //user: dictionary
                            //pn: message -> user, timetoken
                            
                        } else {
                                //user: string, long, array
                            
                        }

                    }

                    if(myObjectArray.Length>=1){
                        pnHistoryResult.StartTimetoken = Utility.ValidateTimetoken(myObjectArray[1].ToString(), true);
                    }

                    if(myObjectArray.Length>=2){
                        pnHistoryResult.EndTimetoken = Utility.ValidateTimetoken(myObjectArray[2].ToString(), true);
                    }
                }
            } catch (Exception ex) {
                throw ex;
            }*/
            /*result = DecodeDecryptLoop (result, pubnubRequestState.ChannelEntities, cipherKey, jsonPluggableLibrary, errorLevel);
            result.Add (multiChannel);*/
        }

        /*internal static void CreateWhereNowResult(object deSerializedResult, ref PNResult result, ref PNStatus pnStatus){
            //TODO: handle {"message": "Not found: '/v2/presence/sub_key/demo/uuid'", "error": 1, "service": "Presence"}
            //{"status": 200, "message": "OK", "payload": {"channels": ["channel1", "channel2"]}, "service": "Presence"}
            PNWhereNowResult pnWhereNowResult = new PNWhereNowResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            if (dictionary!=null && dictionary.ContainsKey("error") && dictionary["error"].Equals(true)){
                result = null;
                pnStatus.Error = true;
                //TODO create error data
            } else if(dictionary!=null && dictionary.ContainsKey("payload")){
                Dictionary<string, object> payload = dictionary["payload"] as Dictionary<string, object>;
                string[] ch = payload["channels"] as string[];
                List<string> channels = ch.ToList<string>();//new List<string> ();
                /*foreach(KeyValuePair<string, object> key in dictionary["payload"] as Dictionary<string, object>){
                    Debug.Log(key.Key + key.Value);
                    result1.Add (key.Value as string);
                }
                foreach(string key in channels){
                    Debug.Log(key);
                }*/

                //result1.Add (multiChannel);
                //List<string> result1 = ((IEnumerable)deSerializedResult).Cast<string> ().ToList ();
                /*  pnWhereNowResult.Channels = channels;
                result = pnWhereNowResult;
                pnStatus.Error = false;
            } else{
                result = null;
                pnStatus.Error = true;
            }
        }*/
        /*internal static void CreateTimeResult(object deSerializedResult, ref PNResult result, ref PNStatus pnStatus){
            Int64[] c = deSerializedResult as Int64[];
            if ((c != null) && (c.Length > 0)) {
                PNTimeResult pnTimeResult = new PNTimeResult();
                pnTimeResult.TimeToken = c [0];
                result = pnTimeResult;
                pnStatus.Error = false;
            } else {
                result = null;
                pnStatus.Error = true;
            }
        }*/

        internal static object DecodeMessage (string cipherKey, object element, IJsonLibrary jsonPluggableLibrary)
        {
            PubnubCrypto aes = new PubnubCrypto (cipherKey);
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