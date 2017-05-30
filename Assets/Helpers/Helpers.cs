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
        internal static TimetokenMetadata CreateTimetokenMetadata (object timeTokenDataObject, string whichTT)
        {
            Dictionary<string, object> timeTokenData = (Dictionary<string, object>)timeTokenDataObject;
            TimetokenMetadata timetokenMetadata = new TimetokenMetadata (Utility.CheckKeyAndParseLong(timeTokenData, whichTT, "t"), 
                (timeTokenData.ContainsKey ("r")) ? timeTokenData["r"].ToString(): "");

            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, TimetokenMetadata: {1} \nTimetoken: {2} \nRegion: {3}", 
            DateTime.Now.ToString (), whichTT, timetokenMetadata.Timetoken, timetokenMetadata.Region), 
            LoggingMethod.LevelInfo);
            #endif

            return timetokenMetadata;
        }

        internal static void AddToSubscribeMessageList (object dictObject, ref List<SubscribeMessage> subscribeMessages)
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
                long sequenceNumber = Utility.CheckKeyAndParseLong (dict, "sequenceNumber", "s"); 

                TimetokenMetadata originatingTimetoken = (dict.Contains ("o")) ? CreateTimetokenMetadata (dict ["o"], "Originating TT: ") : null;
                TimetokenMetadata publishMetadata = (dict.Contains ("p")) ? CreateTimetokenMetadata (dict ["p"], "Publish TT: ") : null;
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
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, AddToSubscribeMessageList: \n" +
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
                (userMetadata != null) ? userMetadata.ToString () : "null"), 
                LoggingMethod.LevelInfo);
                #endif

                subscribeMessages.Add (subscribeMessage);
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, AddToSubscribeMessageList: CreateListOfSubscribeMessage create " +
            "SubscribeMessage failed. dictObject type: {1}, dict type : {2}", 
            DateTime.Now.ToString (), dictObject.ToString (), dict.ToString ()), LoggingMethod.LevelInfo);
            }
            #endif
        }


        internal static List<SubscribeMessage> CreateListOfSubscribeMessage (object message)
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
                            AddToSubscribeMessageList (dictObject, ref subscribeMessages);
                        }
                    }
                } else {
                    //MiniJSON
                    List<object> messageList = message as List<object>;
                    if ((messageList != null) && messageList.Count > 0) {
                        foreach (object dictObject in messageList) {
                            AddToSubscribeMessageList (dictObject, ref subscribeMessages);
                        }
                    }
                }
            }
            #if (ENABLE_PUBNUB_LOGGING)
            else {
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CreateListOfSubscribeMessage: no messages ",
            DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            }
            #endif

            return subscribeMessages;
        }
        #endregion
    }
}