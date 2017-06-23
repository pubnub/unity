using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Collections;
using System.Text;

namespace PubNubAPI
{
    public sealed class Counter
    {
        private uint current = 0;
        private object syncRoot;

        public Counter(){
            syncRoot = new Object();
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
        #endregion
    }
}