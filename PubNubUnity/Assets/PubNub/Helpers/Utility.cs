using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

namespace PubNubAPI
{
    public static class Utility
    {
        public static readonly string PresenceChannelSuffix = "-pnpres";
        public static readonly int iOSRequestTimeout = 59;

        #if(UNITY_IOS)
        public static int CheckTimeoutValue(int value){
            if (value > iOSRequestTimeout) {
                //#if (ENABLE_PUBNUB_LOGGING)
                //LoggingMethod.WriteToLog (string.Format("Forcing timeout value to {0} as iOS force closes the www request after {0} secs", iOSRequestTimeout), LoggingMethod.LevelInfo, PubNubInstance.PNConfig.LogVerbosity);
                //#endif*/

                return iOSRequestTimeout;
            } else {
                return value;
            }
        }
        #endif  
    
        public static bool CheckDictionaryForError(Dictionary<string, object> dictionary, string keyName){
            if(dictionary.ContainsKey(keyName) && dictionary[keyName].Equals(true)){
                return true;
            } else {
                return false;
            }
        }

        public static string ReadMessageFromResponseDictionary(Dictionary<string, object> dictionary, string keyName){
            object objMessage;
            dictionary.TryGetValue(keyName, out objMessage);
            
            if(objMessage != null){
                return objMessage.ToString();
            } else {
                return "";
            }
        }

        //TODO Handle exception
        public static bool TryCheckKeyAndParseLong(IDictionary dict, string what, string key, out string log, out long sequenceNumber){
            sequenceNumber = 0;
            log = ""; 
            if (dict.Contains (key)) {
                long seqNumber;
                if (!Int64.TryParse (dict [key].ToString(), out seqNumber)) {
                    log = string.Format ("{0}, {1} conversion failed: {2}.", what, key, dict [key].ToString ());
                }
                sequenceNumber = seqNumber;
                return true;
            }
            return false;
        }

        //TODO Handle exception
        public static bool TryCheckKeyAndParseInt(IDictionary dict, string what, string key, out string log, out int val){
            val = 0;
            log = "";
            if (dict.Contains (key)) {
                int seqNumber;
                if (!int.TryParse (dict [key].ToString(), out seqNumber)) {
                    log = string.Format ("{0}, {1} conversion failed: {2}.", what, key, dict [key].ToString ());
                    return false;
                }
                val = seqNumber;
                return true;
            }
            log = string.Format ("{0}, {1} key not found.", what, key);
            return false;
        }

        public static long ValidateTimetoken(string timetoken, bool raiseError){
            if(!string.IsNullOrEmpty(timetoken)){
                long r;
                if (long.TryParse (timetoken, out r)) {
                    return r;
                } else if (raiseError) {
                    throw new ArgumentException ("Invalid timetoken");
                } else {
                    return 0;
                }
            } else {
                return 0;
            }
        }

        public static string CheckChannelGroup(string channelGroup, bool convertToPresence){
            string[] multiChannelGroups = channelGroup.Split(',');
            if (multiChannelGroups.Length > 0) {
                for (int index = 0; index < multiChannelGroups.Length; index++) {
                    if (!string.IsNullOrEmpty (multiChannelGroups [index]) && multiChannelGroups [index].Trim ().Length > 0) {
                        if (convertToPresence) {
                            multiChannelGroups [index] = string.Format ("{0}{1}", multiChannelGroups [index], Utility.PresenceChannelSuffix);
                        } 
                    } else {
                        throw new MissingMemberException (string.Format("Invalid channel group '{0}'", multiChannelGroups [index]));
                    }
                }
            } else {
                throw new ArgumentException("Channel Group is null");
            }
            return string.Join(",", multiChannelGroups);
        }

        public static List<string> CheckAndAddNameSpace(string nameSpace){
            List<string> url = new List<string>();
            if (!string.IsNullOrEmpty(nameSpace) && nameSpace.Trim().Length > 0)
            {
                url.Add("namespace");
                url.Add(nameSpace);
                return url;
            }
            return null;
        }

        public static void CheckChannelOrChannelGroup(string channel, string channelGroup){
            if ((string.IsNullOrEmpty(channel) || string.IsNullOrEmpty(channel.Trim())) 
                && (string.IsNullOrEmpty(channelGroup) || string.IsNullOrEmpty(channelGroup.Trim())))
            {
                throw new ArgumentException("Both Channel and ChannelGroup are empty.");
            }
        }

        public static void CheckChannels(string[] channels)
        {
            if (channels == null || channels.Length == 0)
            {
                throw new ArgumentException("Missing channel(s)");
            }
        }

        public static void CheckChannel(string channel)
        {
            if (string.IsNullOrEmpty(channel) || string.IsNullOrEmpty(channel.Trim()))
            {
                throw new ArgumentException("Missing Channel");
            }
        }

        public static void CheckMessage(object message)
        {
            if (message == null)
            {
                throw new ArgumentException("Message is null");
            }
        }

        public static void CheckString(string message, string what)
        {
            if (message == null)
            {
                throw new ArgumentException(string.Format("{0} is null", what));
            }
        }

        public static void CheckPublishKey(string publishKey)
        {
            if (string.IsNullOrEmpty(publishKey) || string.IsNullOrEmpty(publishKey.Trim()) || publishKey.Length <= 0)
            {
                throw new MissingMemberException("Invalid publish key");
            }
        }

        public static Guid GenerateGuid ()
        {
            return Guid.NewGuid ();
        }

        public static bool IsPresenceChannel (string channel)
        {
            if (channel.LastIndexOf (PresenceChannelSuffix, StringComparison.InvariantCulture) > 0) {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsUnsafe (char ch, bool ignoreComma)
        {
            if (ignoreComma) {
                return " ~`!@#$%^&*()+=[]\\{}|;':\"/<>?".IndexOf (ch) >= 0;
            } else {
                return " ~`!@#$%^&*()+=[]\\{}|;':\",/<>?".IndexOf (ch) >= 0;
            }
        }

        private static char ToHex (int ch)
        {
            return (char)(ch < 10 ? '0' + ch : 'A' + ch - 10);
        }

        public static string EncodeUricomponent (string s, PNOperationType type, bool ignoreComma, bool ignorePercent2fEncode)
        {
            string encodedUri = "";
            StringBuilder o = new StringBuilder ();
            foreach (char ch in s) {
                if (IsUnsafe (ch, ignoreComma)) {
                    o.Append ('%');
                    o.Append (ToHex (ch / 16));
                    o.Append (ToHex (ch % 16));
                } else {
                    if (ch == ',' && ignoreComma) {
                        o.Append (ch.ToString ());
                    } else if (Char.IsSurrogate (ch)) {
                        o.Append (ch);
                    } else {
                        string escapeChar = System.Uri.EscapeDataString (ch.ToString ());
                        o.Append (escapeChar);
                    }
                }
            }
            encodedUri = o.ToString ();
            if (type == PNOperationType.PNHereNowOperation || type == PNOperationType.PNHistoryOperation || type == PNOperationType.PNLeaveOperation || type == PNOperationType.PNPresenceHeartbeatOperation
                || type == PNOperationType.PNAddPushNotificationsOnChannelsOperation || type == PNOperationType.PNRemovePushNotificationsFromChannelsOperation || type == PNOperationType.PNPushNotificationEnabledChannelsOperation || type == PNOperationType.PNRemoveAllPushNotificationsOperation
            ) {
                if (!ignorePercent2fEncode) {
                    encodedUri = encodedUri.Replace ("%2F", "%252F");
                }
            }

            return encodedUri;
        }

        public static long TranslateDateTimeToSeconds (DateTime dotNetUTCDateTime)
        {
            TimeSpan timeSpan = dotNetUTCDateTime - new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long timeStamp = Convert.ToInt64 (timeSpan.TotalSeconds);
            return timeStamp;
        }

        /// <summary>
        /// Convert the UTC/GMT DateTime to Unix Nano Seconds format
        /// </summary>
        /// <param name="dotNetUTCDateTime"></param>
        /// <returns></returns>
        public static long TranslateDateTimeToPubnubUnixNanoSeconds (DateTime dotNetUTCDateTime)
        {
            TimeSpan timeSpan = dotNetUTCDateTime - new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long timeStamp = Convert.ToInt64 (timeSpan.TotalSeconds) * 10000000;
            return timeStamp;
        }

        /// <summary>
        /// Convert the Unix Nano Seconds format time to UTC/GMT DateTime
        /// </summary>
        /// <param name="unixNanoSecondTime"></param>
        /// <returns></returns>
        public static DateTime TranslatePubnubUnixNanoSecondsToDateTime (long unixNanoSecondTime)
        {
            double timeStamp = (double)unixNanoSecondTime / 10000000;
            DateTime dateTime = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds (timeStamp);
            return dateTime;
        }

        public static List<string> CheckKeyAndConvertObjToStringArr(object obj){
            if (obj == null) {
                return null;
            }
            List<string> lstArr = ((IEnumerable)obj).Cast<string> ().ToList ();
            #if (ENABLE_PUBNUB_LOGGING)
            foreach (string lst in lstArr){
                UnityEngine.Debug.Log ("clientlist:" + lst);
            }
            #endif
            return lstArr;
        }
    }
}

