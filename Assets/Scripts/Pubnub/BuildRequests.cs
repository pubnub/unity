using System;
using System.Text;
using System.Collections.Generic;

namespace PubNubMessaging.Core
{
    public class BuildRequests
    {

        internal static RequestState<T> BuildRequestState<T>(string[] channel, ResponseType responseType, 
            bool reconnect, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback,
            long id, bool timeout, long timetoken, Type typeParam
        ){
            RequestState<T> requestState = new RequestState<T> ();
            requestState.Channels = channel;
            requestState.RespType = responseType;
            requestState.Reconnect = reconnect;
            requestState.UserCallback = userCallback;
            requestState.ErrorCallback = errorCallback;
            requestState.ConnectCallback = connectCallback;
            requestState.ID = id;
            requestState.Timeout = timeout;
            requestState.Timetoken = timetoken;
            requestState.TypeParameterType = typeParam;

            return requestState;
        }

        #region "Build Requests"
        internal static Uri BuildRegisterDevicePushRequest(string channel, PushTypeService pushType, 
            string pushToken,  string uuid, 
            bool ssl, string origin, string authenticationKey,string subscribeKey)
        {
            StringBuilder parameterBuilder = new StringBuilder();

            parameterBuilder.AppendFormat("?add={0}", Utility.EncodeUricomponent(channel, ResponseType.PushRegister, true, false));
            parameterBuilder.AppendFormat("&type={0}", pushType.ToString().ToLower());

            // Build URL
            List<string> url = new List<string>();
            url.Add("v1");
            url.Add("push");
            url.Add("sub-key");
            url.Add(subscribeKey);
            url.Add("devices");
            url.Add(pushToken);

            return BuildRestApiRequest<Uri>(url, ResponseType.PushRegister, uuid, ssl, origin, 0, authenticationKey, parameterBuilder.ToString());
        }

        internal static Uri BuildRemoveChannelPushRequest(string channel, PushTypeService pushType, 
            string pushToken,  string uuid, 
            bool ssl, string origin, string authenticationKey,string subscribeKey)
        {
            StringBuilder parameterBuilder = new StringBuilder();

            parameterBuilder.AppendFormat("?remove={0}", Utility.EncodeUricomponent(channel, ResponseType.PushRemove, true, false));
            parameterBuilder.AppendFormat("&type={0}", pushType.ToString().ToLower());

            // Build URL
            List<string> url = new List<string>();
            url.Add("v1");
            url.Add("push");
            url.Add("sub-key");
            url.Add(subscribeKey);
            url.Add("devices");
            url.Add(pushToken);

            return BuildRestApiRequest<Uri>(url, ResponseType.PushRemove, uuid, ssl, origin, 0, authenticationKey, parameterBuilder.ToString());
        }

        internal static Uri BuildGetChannelsPushRequest(PushTypeService pushType, string pushToken, string uuid, 
            bool ssl, string origin, string authenticationKey, string subscribeKey)
        {
            StringBuilder parameterBuilder = new StringBuilder();

            parameterBuilder.AppendFormat("?type={0}", pushType.ToString().ToLower());

            // Build URL
            List<string> url = new List<string>();
            url.Add("v1");
            url.Add("push");
            url.Add("sub-key");
            url.Add(subscribeKey);
            url.Add("devices");
            url.Add(pushToken);

            return BuildRestApiRequest<Uri>(url, ResponseType.PushGet, uuid, ssl, origin, 0, authenticationKey, parameterBuilder.ToString());
        }

        internal static Uri BuildUnregisterDevicePushRequest(PushTypeService pushType, string pushToken, string uuid, 
            bool ssl, string origin, string authenticationKey, string subscribeKey)
        {
            StringBuilder parameterBuilder = new StringBuilder();

            parameterBuilder.AppendFormat("?type={0}", pushType.ToString().ToLower());

            // Build URL
            List<string> url = new List<string>();
            url.Add("v1");
            url.Add("push");
            url.Add("sub-key");
            url.Add(subscribeKey);
            url.Add("devices");
            url.Add(pushToken);
            url.Add("remove");

            return BuildRestApiRequest<Uri>(url, ResponseType.PushUnregister, uuid, ssl, origin, 0, authenticationKey, parameterBuilder.ToString());
        }

        internal static Uri BuildPublishRequest (string channel, string message, bool storeInHistory, string uuid, 
            bool ssl, string origin, string authenticationKey, 
            string publishKey, string subscribeKey, string cipherKey, string secretKey)
        {
            string parameters = (storeInHistory) ? "" : "store=0";

            // Generate String to Sign
            string signature = "0";
            if (secretKey.Length > 0) {
                StringBuilder stringToSign = new StringBuilder ();
                stringToSign
                    .Append (publishKey)
                    .Append ('/')
                    .Append (subscribeKey)
                    .Append ('/')
                    .Append (secretKey)
                    .Append ('/')
                    .Append (channel)
                    .Append ('/')
                    .Append (message); // 1

                // Sign Message
                signature = Utility.Md5 (stringToSign.ToString ());
            }

            // Build URL
            List<string> url = new List<string> ();
            url.Add ("publish");
            url.Add (publishKey);
            url.Add (subscribeKey);
            url.Add (signature);
            url.Add (channel);
            url.Add ("0");
            url.Add (message);

            return BuildRestApiRequest<Uri> (url, ResponseType.Publish, uuid, ssl, origin, 0, authenticationKey, parameters);
        }

        internal static Uri BuildDetailedHistoryRequest (string channel, long start, long end, 
            int count, bool reverse, bool includeToken, string uuid, bool ssl, string origin, 
            string authenticationKey, string subscribeKey)
        {
            StringBuilder parameterBuilder = new StringBuilder ();
            string parameters = "";
            if (count <= -1)
                count = 100;

            parameterBuilder.AppendFormat ("?count={0}", count);
            if (includeToken) {
                parameterBuilder.AppendFormat ("&include_token={0}", includeToken.ToString ().ToLower ());
            }
            if (reverse) {
                parameterBuilder.AppendFormat ("&reverse={0}", reverse.ToString ().ToLower ());
            }
            if (start != -1) {
                parameterBuilder.AppendFormat ("&start={0}", start.ToString ().ToLower ());
            }
            if (end != -1) {
                parameterBuilder.AppendFormat ("&end={0}", end.ToString ().ToLower ());
            }
            if (!string.IsNullOrEmpty (authenticationKey)) {
                parameterBuilder.AppendFormat ("&auth={0}", Utility.EncodeUricomponent (authenticationKey, ResponseType.DetailedHistory, false, false));
            }

            parameterBuilder.AppendFormat ("&uuid={0}", Utility.EncodeUricomponent (uuid, ResponseType.DetailedHistory, false, false));
            parameterBuilder.AppendFormat ("&pnsdk={0}", Utility.EncodeUricomponent (PubnubUnity.Version, ResponseType.DetailedHistory, false, true));

            parameters = parameterBuilder.ToString ();

            List<string> url = new List<string> ();

            url.Add ("v2");
            url.Add ("history");
            url.Add ("sub-key");
            url.Add (subscribeKey);
            url.Add ("channel");
            url.Add (channel);

            return BuildRestApiRequest<Uri> (url, ResponseType.DetailedHistory, uuid, ssl, origin, 0, authenticationKey, parameters);
        }

        internal static Uri BuildHereNowRequest (string channel, bool showUUIDList, bool includeUserState, string uuid, 
            bool ssl, string origin, string authenticationKey, string subscribeKey)
        {
            int disableUUID = (showUUIDList) ? 0 : 1;
            int userState = (includeUserState) ? 1 : 0;
            string parameters = string.Format ("?disable_uuids={0}&state={1}", disableUUID, userState);

            List<string> url = new List<string> ();

            url.Add ("v2");
            url.Add ("presence");
            url.Add ("sub_key");
            url.Add (subscribeKey);
            url.Add ("channel");
            url.Add (channel);

            return BuildRestApiRequest<Uri> (url, ResponseType.HereNow, uuid, ssl, origin, 0, authenticationKey, parameters);
        }

        internal static Uri BuildGlobalHereNowRequest (bool showUUIDList, bool includeUserState, string uuid, 
            bool ssl, string origin, string authenticationKey, string subscribeKey)
        {
            int disableUUID = (showUUIDList) ? 0 : 1;
            int userState = (includeUserState) ? 1 : 0;
            string parameters = string.Format ("?disable_uuids={0}&state={1}", disableUUID, userState);

            List<string> url = new List<string> ();

            url.Add ("v2");
            url.Add ("presence");
            url.Add ("sub_key");
            url.Add (subscribeKey);

            return BuildRestApiRequest<Uri> (url, ResponseType.GlobalHereNow, uuid, ssl, origin, 0, authenticationKey, parameters);
        }

        internal static Uri BuildWhereNowRequest (string uuid, string sessionUUID,
            bool ssl, string origin, string authenticationKey, string subscribeKey)
        {
            List<string> url = new List<string> ();

            url.Add ("v2");
            url.Add ("presence");
            url.Add ("sub_key");
            url.Add (subscribeKey);
            url.Add ("uuid");
            url.Add (uuid);

            return BuildRestApiRequest<Uri> (url, ResponseType.WhereNow, sessionUUID, ssl, origin, 0, authenticationKey, "");
        }

        internal static Uri BuildTimeRequest (string uuid, bool ssl, string origin)
        {
            List<string> url = new List<string> ();

            url.Add ("time");
            url.Add ("0");

            return BuildRestApiRequest<Uri> (url, ResponseType.Time, uuid, ssl, origin, 0, "", "");
        }

        internal static Uri BuildGrantAccessRequest (string channel, bool read, bool write, int ttl, string uuid, 
            bool ssl, string origin, string authenticationKey, 
            string publishKey, string subscribeKey, string cipherKey, string secretKey)
        {
            string signature = "0";
            long timeStamp = Utility.TranslateDateTimeToSeconds (DateTime.UtcNow);
            string queryString = "";

            StringBuilder queryStringBuilder = new StringBuilder ();
            if (!string.IsNullOrEmpty (authenticationKey)) {
                queryStringBuilder.AppendFormat ("auth={0}", Utility.EncodeUricomponent (authenticationKey, ResponseType.GrantAccess, false, false));
            }

            if (!string.IsNullOrEmpty (channel)) {
                queryStringBuilder.AppendFormat ("{0}channel={1}", (queryStringBuilder.Length > 0) ? "&" : "", Utility.EncodeUricomponent (channel, ResponseType.GrantAccess, false, false));
            }

            queryStringBuilder.AppendFormat ("{0}", (queryStringBuilder.Length > 0) ? "&" : "");
            queryStringBuilder.AppendFormat ("pnsdk={0}", Utility.EncodeUricomponent (PubnubUnity.Version, ResponseType.GrantAccess, false, true));
            queryStringBuilder.AppendFormat ("&r={0}", Convert.ToInt32 (read));
            queryStringBuilder.AppendFormat ("&timestamp={0}", timeStamp.ToString ());
            if (ttl > -1) {
                queryStringBuilder.AppendFormat ("&ttl={0}", ttl.ToString ());
            }
            queryStringBuilder.AppendFormat ("&uuid={0}", Utility.EncodeUricomponent (uuid, ResponseType.GrantAccess, false, false));
            queryStringBuilder.AppendFormat ("&w={0}", Convert.ToInt32 (write));

            if (secretKey.Length > 0) {
                StringBuilder string_to_sign = new StringBuilder ();
                string_to_sign.Append (subscribeKey)
                    .Append ("\n")
                    .Append (publishKey)
                    .Append ("\n")
                    .Append ("grant")
                    .Append ("\n")
                    .Append (queryStringBuilder.ToString ());

                PubnubCrypto pubnubCrypto = new PubnubCrypto (cipherKey);
                signature = pubnubCrypto.PubnubAccessManagerSign (secretKey, string_to_sign.ToString ());
                queryString = string.Format ("signature={0}&{1}", signature, queryStringBuilder.ToString ());
            }

            string parameters = "";
            parameters += "?" + queryString;

            List<string> url = new List<string> ();
            url.Add ("v1");
            url.Add ("auth");
            url.Add ("grant");
            url.Add ("sub-key");
            url.Add (subscribeKey);

            return BuildRestApiRequest<Uri> (url, ResponseType.GrantAccess, uuid, ssl, origin, 0, authenticationKey, parameters);
        }

        internal static Uri BuildAuditAccessRequest (string channel, string uuid, 
            bool ssl, string origin, string authenticationKey, 
            string publishKey, string subscribeKey, string cipherKey, string secretKey)
        {
            string signature = "0";

            long timeStamp = Utility.TranslateDateTimeToSeconds (DateTime.UtcNow);
            string queryString = "";
            StringBuilder queryStringBuilder = new StringBuilder ();
            if (!string.IsNullOrEmpty (authenticationKey)) {
                queryStringBuilder.AppendFormat ("auth={0}", Utility.EncodeUricomponent (authenticationKey, ResponseType.AuditAccess, false, false));
            }
            if (!string.IsNullOrEmpty (channel)) {
                queryStringBuilder.AppendFormat ("{0}channel={1}", (queryStringBuilder.Length > 0) ? "&" : "", Utility.EncodeUricomponent (channel, ResponseType.AuditAccess, false, false));
            }
            queryStringBuilder.AppendFormat ("{0}pnsdk={1}", (queryStringBuilder.Length > 0) ? "&" : "", Utility.EncodeUricomponent (PubnubUnity.Version, ResponseType.AuditAccess, false, true));
            queryStringBuilder.AppendFormat ("{0}timestamp={1}", (queryStringBuilder.Length > 0) ? "&" : "", timeStamp.ToString ());
            queryStringBuilder.AppendFormat ("{0}uuid={1}", (queryStringBuilder.Length > 0) ? "&" : "", Utility.EncodeUricomponent (uuid, ResponseType.AuditAccess, false, false));

            if (secretKey.Length > 0) {
                StringBuilder string_to_sign = new StringBuilder ();
                string_to_sign.Append (subscribeKey)
                    .Append ("\n")
                    .Append (publishKey)
                    .Append ("\n")
                    .Append ("audit")
                    .Append ("\n")
                    .Append (queryStringBuilder.ToString ());

                PubnubCrypto pubnubCrypto = new PubnubCrypto (cipherKey);
                signature = pubnubCrypto.PubnubAccessManagerSign (secretKey, string_to_sign.ToString ());
                queryString = string.Format ("signature={0}&{1}", signature, queryStringBuilder.ToString ());
            }

            string parameters = "";
            parameters += "?" + queryString;

            List<string> url = new List<string> ();
            url.Add ("v1");
            url.Add ("auth");
            url.Add ("audit");
            url.Add ("sub-key");
            url.Add (subscribeKey);

            return BuildRestApiRequest<Uri> (url, ResponseType.AuditAccess, uuid, ssl, origin, 0, authenticationKey, parameters);
        }

        internal static Uri BuildSetUserStateRequest (string channel, string jsonUserState, string uuid, 
            bool ssl, string origin, string authenticationKey, string subscribeKey)
        {
            string parameters = string.Format ("?state={0}", Utility.EncodeUricomponent (jsonUserState, ResponseType.SetUserState, false, false));

            List<string> url = new List<string> ();

            url.Add ("v2");
            url.Add ("presence");
            url.Add ("sub_key");
            url.Add (subscribeKey);
            url.Add ("channel");
            url.Add (channel);
            url.Add ("uuid");
            url.Add (uuid);
            url.Add ("data");

            return BuildRestApiRequest<Uri> (url, ResponseType.SetUserState, uuid, ssl, origin, 0, authenticationKey, parameters);
        }

        internal static Uri BuildGetUserStateRequest (string channel, string uuid,
            bool ssl, string origin, string authenticationKey, string subscribeKey)
        {
            List<string> url = new List<string> ();

            url.Add ("v2");
            url.Add ("presence");
            url.Add ("sub_key");
            url.Add (subscribeKey);
            url.Add ("channel");
            url.Add (channel);
            url.Add ("uuid");
            url.Add (uuid);

            return BuildRestApiRequest<Uri> (url, ResponseType.GetUserState, uuid, ssl, origin, 0, authenticationKey, "");
        }

        internal static Uri BuildPresenceHeartbeatRequest (string[] channels, string channelsJsonState, string uuid,
            bool ssl, string origin, string authenticationKey, string subscribeKey)
        {
            string parameters = "";
            if (channelsJsonState != "{}" && channelsJsonState != "") {
                parameters = string.Format ("&state={0}", Utility.EncodeUricomponent (channelsJsonState, ResponseType.PresenceHeartbeat, false, false));
            }
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, presenceHeartbeatParameters {1}", DateTime.Now.ToString (), parameters), LoggingMethod.LevelInfo);
            #endif

            List<string> url = new List<string> ();

            url.Add ("v2");
            url.Add ("presence");
            url.Add ("sub_key");
            url.Add (subscribeKey);
            url.Add ("channel");
            url.Add (string.Join (",", channels));
            url.Add ("heartbeat");

            return BuildRestApiRequest<Uri> (url, ResponseType.PresenceHeartbeat, uuid, ssl, origin, 0, authenticationKey, parameters);
        }

        internal static Uri BuildMultiChannelLeaveRequest (string[] channels, string uuid, 
            bool ssl, string origin, string authenticationKey, string subscribeKey)
        {
            List<string> url = new List<string> ();

            url.Add ("v2");
            url.Add ("presence");
            url.Add ("sub_key");
            url.Add (subscribeKey);
            url.Add ("channel");
            url.Add (string.Join (",", channels));
            url.Add ("leave");

            return BuildRestApiRequest<Uri> (url, ResponseType.Leave, uuid, ssl, origin, 0, authenticationKey, "");
        }

        internal static Uri BuildMultiChannelSubscribeRequest (string[] channels, object timetoken, string channelsJsonState, string uuid,
            bool ssl, string origin, string authenticationKey, string subscribeKey)
        {
            string parameters = "";

            if (channelsJsonState != "{}" && channelsJsonState != "") {
                parameters = string.Format ("&state={0}", Utility.EncodeUricomponent (channelsJsonState, ResponseType.Subscribe, false, false));
            }

            List<string> url = new List<string> ();
            url.Add ("subscribe");
            url.Add (subscribeKey);
            url.Add (string.Join (",", channels));
            url.Add ("0");
            url.Add (timetoken.ToString ());

            return BuildRestApiRequest<Uri> (url, ResponseType.Subscribe, uuid, ssl, origin, 0, authenticationKey, parameters);
        }

        static StringBuilder AddSSLAndEncodeURL<T>(List<string> urlComponents, ResponseType type, bool ssl, string origin, StringBuilder url)
        {
            // Add http or https based on SSL flag
            if (ssl)
            {
                url.Append("https://");
            }
            else
            {
                url.Append("http://");
            }
            // Add Origin To The Request
            url.Append(origin);
            // Generate URL with UTF-8 Encoding
            for (int componentIndex = 0; componentIndex < urlComponents.Count; componentIndex++)
            {
                url.Append("/");
                if (type == ResponseType.Publish && componentIndex == urlComponents.Count - 1)
                {
                    url.Append(Utility.EncodeUricomponent(urlComponents[componentIndex].ToString(), type, false, false));
                }
                else
                {
                    url.Append(Utility.EncodeUricomponent(urlComponents[componentIndex].ToString(), type, true, false));
                }
            }
            return url;
        }

        private static StringBuilder AppendAuthKeyToURL(StringBuilder url, string authenticationKey, ResponseType type){
            if (!string.IsNullOrEmpty (authenticationKey)) {
                url.AppendFormat ("&auth={0}", Utility.EncodeUricomponent (authenticationKey, type, false, false));
            }
            return url;
        }

        private static StringBuilder AppendUUIDToURL(StringBuilder url, string uuid, bool firstInQS){
            if (firstInQS)
            {
                url.AppendFormat("?uuid={0}", uuid);
            }
            else
            {
                url.AppendFormat("&uuid={0}", uuid);
            }
            return url;
        }

        private static StringBuilder AppendPresenceHeartbeatToURL(StringBuilder url, int pubnubPresenceHeartbeatInSeconds){
            if (pubnubPresenceHeartbeatInSeconds != 0) {
                url.AppendFormat ("&heartbeat={0}", pubnubPresenceHeartbeatInSeconds);
            }
            return url;
        }

        private static StringBuilder AppendPNSDKVersionToURL(StringBuilder url, string pnsdkVersion, ResponseType type){
            url.AppendFormat ("&pnsdk={0}", Utility.EncodeUricomponent (pnsdkVersion, type, false, true));
            return url;
        }

        //sessionid
        //ssl
        //origin
        //pubnubPresenceHeartbeatInSeconds
        //authenticationKey
        //pnsdkVersion    
        //parameters
        private static Uri BuildRestApiRequest<T> (List<string> urlComponents, ResponseType type, string uuid, bool ssl, string origin, 
            int pubnubPresenceHeartbeatInSeconds, string authenticationKey, string parameters)
        {
            StringBuilder url = new StringBuilder ();
            string pnsdkVersion = PubnubUnity.Version;
            uuid = Utility.EncodeUricomponent (uuid, type, false, false);

            url = AddSSLAndEncodeURL<T>(urlComponents, type, ssl, origin, url);

            switch (type) {
                case ResponseType.Presence:
                case ResponseType.Subscribe:
                case ResponseType.Leave:

                    url = AppendUUIDToURL(url, uuid, true);
                    url.Append(parameters);
                    url = AppendAuthKeyToURL(url, authenticationKey, type);

                    url = AppendPresenceHeartbeatToURL(url, pubnubPresenceHeartbeatInSeconds);
                    url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                    break;

                case ResponseType.PresenceHeartbeat:

                    url = AppendUUIDToURL(url, uuid, true);
                    url.Append (parameters);
                    url = AppendPresenceHeartbeatToURL(url, pubnubPresenceHeartbeatInSeconds);
                    url = AppendAuthKeyToURL(url, authenticationKey, type);
                    url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                    break;

                case ResponseType.SetUserState:

                    url.Append (parameters);
                    url = AppendUUIDToURL(url, uuid, false);
                    url = AppendAuthKeyToURL(url, authenticationKey, type);
                    url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                    break;

                case ResponseType.GetUserState:

                    url = AppendUUIDToURL(url, uuid, true);
                    url = AppendAuthKeyToURL(url, authenticationKey, type);
                    url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                    break;
                case ResponseType.HereNow:

                    url.Append (parameters);
                    url = AppendUUIDToURL(url, uuid, false);
                    url = AppendAuthKeyToURL(url, authenticationKey, type);
                    url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                    break;

                case ResponseType.GlobalHereNow:

                    url.Append (parameters);
                    url = AppendUUIDToURL(url, uuid, false);
                    url = AppendAuthKeyToURL(url, authenticationKey, type);
                    url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                    break;

                case ResponseType.WhereNow:

                    url = AppendUUIDToURL(url, uuid, true);
                    url = AppendAuthKeyToURL(url, authenticationKey, type);
                    url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                    break;

                case ResponseType.Publish:

                    url = AppendUUIDToURL(url, uuid, true);
                    if (parameters != "") {
                        url.AppendFormat ("&{0}", parameters);
                    }
                    url = AppendAuthKeyToURL(url, authenticationKey, type);
                    url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);

                    break;
                case ResponseType.PushGet:
                case ResponseType.PushRegister:
                case ResponseType.PushRemove:
                case ResponseType.PushUnregister:
                    url.Append (parameters);
                    url = AppendUUIDToURL(url, uuid, false);
                    url = AppendAuthKeyToURL(url, authenticationKey, type);
                    url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                    break;
                case ResponseType.DetailedHistory:
                case ResponseType.GrantAccess:
                case ResponseType.AuditAccess:
                case ResponseType.RevokeAccess:
                    url.Append (parameters);
                    break;
                default:
                    url = AppendUUIDToURL(url, uuid, true);
                    url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
                    break;
            }

            Uri requestUri = new Uri (url.ToString ());

            return requestUri;

        }
        #endregion
    }
}

