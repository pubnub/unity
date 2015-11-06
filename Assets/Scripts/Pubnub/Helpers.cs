using System;
using System.Collections.Generic;
using System.Text;

namespace PubNubMessaging.Core
{
	internal static class Helpers
	{
		#region "Other Methods"
		internal static RequestState<T> BuildRequestState<T>(string[] channel, ResponseType responseType, 
			bool reconnect, Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback,
			long id, bool timeout, long timetoken, Type typeParam
		){
			RequestState<T> requestState = new RequestState<T> ();
			requestState.Channels = channel;
			requestState.Type = responseType;
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

		private static bool IsUnsafe (char ch, bool ignoreComma)
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

		private static string EncodeUricomponent (string s, ResponseType type, bool ignoreComma, bool ignorePercent2fEncode)
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
			if (type == ResponseType.HereNow || type == ResponseType.DetailedHistory || type == ResponseType.Leave || type == ResponseType.PresenceHeartbeat) {
				if (!ignorePercent2fEncode) {
					encodedUri = encodedUri.Replace ("%2F", "%252F");
				}
			}

			return encodedUri;
		}

		private static string Md5 (string text)
		{
			MD5 md5 = new MD5CryptoServiceProvider ();
			byte[] data = Encoding.Unicode.GetBytes (text);
			byte[] hash = md5.ComputeHash (data);
			string hexaHash = "";
			foreach (byte b in hash)
				hexaHash += String.Format ("{0:x2}", b);
			return hexaHash;
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
			double timeStamp = unixNanoSecondTime / 10000000;
			DateTime dateTime = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds (timeStamp);
			return dateTime;
		}

		internal static PubnubChannelCallbackKey GetPubnubChannelCallbackKey(string activeChannel, ResponseType responseType){
			PubnubChannelCallbackKey callbackKey = new PubnubChannelCallbackKey ();
			callbackKey.Channel = activeChannel;
			callbackKey.Type = responseType;
		}

		#endregion

		#region "Build Requests"
		internal static Uri BuildPublishRequest (string channel, string message, bool storeInHistory, string uuid, 
			bool ssl, string origin, string authenticationKey, string pnsdkVersion, 
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
				signature = Md5 (stringToSign.ToString ());
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

			return BuildRestApiRequest<Uri> (url, ResponseType.Publish, uuid, ssl, origin, 0, authenticationKey, pnsdkVersion, parameters);
		}

		internal static Uri BuildDetailedHistoryRequest (string channel, long start, long end, 
			int count, bool reverse, bool includeToken, string uuid, bool ssl, string origin, 
			string authenticationKey, string pnsdkVersion, string subscribeKey)
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
				parameterBuilder.AppendFormat ("&auth={0}", EncodeUricomponent (authenticationKey, ResponseType.DetailedHistory, false, false));
			}

			parameterBuilder.AppendFormat ("&uuid={0}", EncodeUricomponent (uuid, ResponseType.DetailedHistory, false, false));
			parameterBuilder.AppendFormat ("&pnsdk={0}", EncodeUricomponent (pnsdkVersion, ResponseType.DetailedHistory, false, true));

			parameters = parameterBuilder.ToString ();

			List<string> url = new List<string> ();

			url.Add ("v2");
			url.Add ("history");
			url.Add ("sub-key");
			url.Add (subscribeKey);
			url.Add ("channel");
			url.Add (channel);

			return BuildRestApiRequest<Uri> (url, ResponseType.DetailedHistory, uuid, ssl, origin, 0, authenticationKey, pnsdkVersion, parameters);
		}

		internal static Uri BuildHereNowRequest (string channel, bool showUUIDList, bool includeUserState, string uuid, 
			bool ssl, string origin, string authenticationKey, string pnsdkVersion, string subscribeKey)
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

			return BuildRestApiRequest<Uri> (url, ResponseType.HereNow, uuid, ssl, origin, 0, authenticationKey, pnsdkVersion, parameters);
		}

		internal static Uri BuildGlobalHereNowRequest (bool showUUIDList, bool includeUserState, string uuid, 
			bool ssl, string origin, string authenticationKey, string pnsdkVersion, string subscribeKey)
		{
			int disableUUID = (showUUIDList) ? 0 : 1;
			int userState = (includeUserState) ? 1 : 0;
			string parameters = string.Format ("?disable_uuids={0}&state={1}", disableUUID, userState);

			List<string> url = new List<string> ();

			url.Add ("v2");
			url.Add ("presence");
			url.Add ("sub_key");
			url.Add (subscribeKey);

			return BuildRestApiRequest<Uri> (url, ResponseType.GlobalHereNow, uuid, ssl, origin, 0, authenticationKey, pnsdkVersion, parameters);
		}

		internal static Uri BuildWhereNowRequest (string uuid, string sessionUUID,
			bool ssl, string origin, string authenticationKey, string pnsdkVersion, string subscribeKey)
		{
			List<string> url = new List<string> ();

			url.Add ("v2");
			url.Add ("presence");
			url.Add ("sub_key");
			url.Add (subscribeKey);
			url.Add ("uuid");
			url.Add (uuid);

			return BuildRestApiRequest<Uri> (url, ResponseType.WhereNow, sessionUUID, ssl, origin, 0, authenticationKey, pnsdkVersion, "");
		}

		internal static Uri BuildTimeRequest (string uuid, bool ssl, string origin, string pnsdkVersion)
		{
			List<string> url = new List<string> ();

			url.Add ("time");
			url.Add ("0");

			return BuildRestApiRequest<Uri> (url, ResponseType.Time, uuid, ssl, origin, 0, "", pnsdkVersion, "");
		}

		internal static Uri BuildGrantAccessRequest (string channel, bool read, bool write, int ttl, string uuid, 
			bool ssl, string origin, string authenticationKey, string pnsdkVersion, 
			string publishKey, string subscribeKey, string cipherKey, string secretKey)
		{
			string signature = "0";
			long timeStamp = TranslateDateTimeToSeconds (DateTime.UtcNow);
			string queryString = "";

			StringBuilder queryStringBuilder = new StringBuilder ();
			if (!string.IsNullOrEmpty (authenticationKey)) {
				queryStringBuilder.AppendFormat ("auth={0}", EncodeUricomponent (authenticationKey, ResponseType.GrantAccess, false, false));
			}

			if (!string.IsNullOrEmpty (channel)) {
				queryStringBuilder.AppendFormat ("{0}channel={1}", (queryStringBuilder.Length > 0) ? "&" : "", EncodeUricomponent (channel, ResponseType.GrantAccess, false, false));
			}

			queryStringBuilder.AppendFormat ("{0}", (queryStringBuilder.Length > 0) ? "&" : "");
			queryStringBuilder.AppendFormat ("pnsdk={0}", EncodeUricomponent (pnsdkVersion, ResponseType.GrantAccess, false, true));
			queryStringBuilder.AppendFormat ("&r={0}", Convert.ToInt32 (read));
			queryStringBuilder.AppendFormat ("&timestamp={0}", timeStamp.ToString ());
			if (ttl > -1) {
				queryStringBuilder.AppendFormat ("&ttl={0}", ttl.ToString ());
			}
			queryStringBuilder.AppendFormat ("&uuid={0}", EncodeUricomponent (uuid, ResponseType.GrantAccess, false, false));
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

			return BuildRestApiRequest<Uri> (url, ResponseType.GrantAccess, uuid, ssl, origin, 0, authenticationKey, pnsdkVersion, parameters);
		}

		internal static Uri BuildAuditAccessRequest (string channel, string uuid, 
			bool ssl, string origin, string authenticationKey, string pnsdkVersion,
			string publishKey, string subscribeKey, string cipherKey, string secretKey)
		{
			string signature = "0";

			long timeStamp = TranslateDateTimeToSeconds (DateTime.UtcNow);
			string queryString = "";
			StringBuilder queryStringBuilder = new StringBuilder ();
			if (!string.IsNullOrEmpty (authenticationKey)) {
				queryStringBuilder.AppendFormat ("auth={0}", EncodeUricomponent (authenticationKey, ResponseType.AuditAccess, false, false));
			}
			if (!string.IsNullOrEmpty (channel)) {
				queryStringBuilder.AppendFormat ("{0}channel={1}", (queryStringBuilder.Length > 0) ? "&" : "", EncodeUricomponent (channel, ResponseType.AuditAccess, false, false));
			}
			queryStringBuilder.AppendFormat ("{0}pnsdk={1}", (queryStringBuilder.Length > 0) ? "&" : "", EncodeUricomponent (pnsdkVersion, ResponseType.AuditAccess, false, true));
			queryStringBuilder.AppendFormat ("{0}timestamp={1}", (queryStringBuilder.Length > 0) ? "&" : "", timeStamp.ToString ());
			queryStringBuilder.AppendFormat ("{0}uuid={1}", (queryStringBuilder.Length > 0) ? "&" : "", EncodeUricomponent (uuid, ResponseType.AuditAccess, false, false));

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

			return BuildRestApiRequest<Uri> (url, ResponseType.AuditAccess, uuid, ssl, origin, 0, authenticationKey, pnsdkVersion, parameters);
		}

		internal static Uri BuildSetUserStateRequest (string channel, string jsonUserState, string uuid, 
			bool ssl, string origin, string authenticationKey, string pnsdkVersion, string subscribeKey)
		{
			string parameters = string.Format ("?state={0}", EncodeUricomponent (jsonUserState, ResponseType.SetUserState, false, false));

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

			return BuildRestApiRequest<Uri> (url, ResponseType.SetUserState, uuid, ssl, origin, 0, authenticationKey, pnsdkVersion, parameters);
		}

		internal static Uri BuildGetUserStateRequest (string channel, string uuid,
			bool ssl, string origin, string authenticationKey, string pnsdkVersion, string subscribeKey)
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

			return BuildRestApiRequest<Uri> (url, ResponseType.GetUserState, uuid, ssl, origin, 0, authenticationKey, pnsdkVersion, "");
		}

		internal static Uri BuildPresenceHeartbeatRequest (string[] channels, string channelsJsonState, string uuid,
			bool ssl, string origin, string authenticationKey, string pnsdkVersion, string subscribeKey)
		{
			string parameters = "";
			if (channelsJsonState != "{}" && channelsJsonState != "") {
				parameters = string.Format ("&state={0}", EncodeUricomponent (channelsJsonState, ResponseType.PresenceHeartbeat, false, false));
			}
			LoggingMethod.WriteToLog (string.Format ("DateTime {0}, presenceHeartbeatParameters {1}", DateTime.Now.ToString (), parameters), LoggingMethod.LevelInfo);

			List<string> url = new List<string> ();

			url.Add ("v2");
			url.Add ("presence");
			url.Add ("sub_key");
			url.Add (subscribeKey);
			url.Add ("channel");
			url.Add (string.Join (",", channels));
			url.Add ("heartbeat");

			return BuildRestApiRequest<Uri> (url, ResponseType.PresenceHeartbeat, uuid, ssl, origin, 0, authenticationKey, pnsdkVersion, parameters);
		}

		internal static Uri BuildMultiChannelLeaveRequest (string[] channels, string uuid, 
			bool ssl, string origin, string authenticationKey, string pnsdkVersion, string subscribeKey)
		{
			List<string> url = new List<string> ();

			url.Add ("v2");
			url.Add ("presence");
			url.Add ("sub_key");
			url.Add (subscribeKey);
			url.Add ("channel");
			url.Add (string.Join (",", channels));
			url.Add ("leave");

			return BuildRestApiRequest<Uri> (url, ResponseType.Leave, uuid, ssl, origin, 0, authenticationKey, pnsdkVersion, "");
		}

		internal static Uri BuildMultiChannelSubscribeRequest (string[] channels, object timetoken, string channelsJsonState, string uuid,
			bool ssl, string origin, string authenticationKey, string pnsdkVersion, string subscribeKey)
		{
			string parameters = "";

			if (channelsJsonState != "{}" && channelsJsonState != "") {
				parameters = string.Format ("&state={0}", EncodeUricomponent (channelsJsonState, ResponseType.Subscribe, false, false));
			}

			List<string> url = new List<string> ();
			url.Add ("subscribe");
			url.Add (subscribeKey);
			url.Add (string.Join (",", channels));
			url.Add ("0");
			url.Add (timetoken.ToString ());

			return BuildRestApiRequest<Uri> (url, ResponseType.Subscribe, uuid, ssl, origin, 0, authenticationKey, pnsdkVersion, parameters);
		}

		//sessionid
		//ssl
		//origin
		//pubnubPresenceHeartbeatInSeconds
		//authenticationKey
		//pnsdkVersion	
		//parameters

		private static Uri BuildRestApiRequest<T> (List<string> urlComponents, ResponseType type, string uuid, bool ssl, string origin, 
			int pubnubPresenceHeartbeatInSeconds, string authenticationKey, string pnsdkVersion, string parameters)
		{
			StringBuilder url = new StringBuilder ();

			uuid = EncodeUricomponent (uuid, type, false, false);

			// Add http or https based on SSL flag
			if (ssl) {
				url.Append ("https://");
			} else {
				url.Append ("http://");
			}

			// Add Origin To The Request
			url.Append (origin);

			// Generate URL with UTF-8 Encoding
			for (int componentIndex = 0; componentIndex < urlComponents.Count; componentIndex++) {
				url.Append ("/");

				if (type == ResponseType.Publish && componentIndex == urlComponents.Count - 1) {
					url.Append (EncodeUricomponent (urlComponents [componentIndex].ToString (), type, false, false));
				} else {
					url.Append (EncodeUricomponent (urlComponents [componentIndex].ToString (), type, true, false));
				}
			}
			switch (type) {
			case ResponseType.Presence:
			case ResponseType.Subscribe:
			case ResponseType.Leave:

				url.AppendFormat ("?uuid={0}", uuid);
				url.Append (parameters);
				if (!string.IsNullOrEmpty (authenticationKey)) {
					url.AppendFormat ("&auth={0}", EncodeUricomponent (authenticationKey, type, false, false));
				}
				if (pubnubPresenceHeartbeatInSeconds != 0) {
					url.AppendFormat ("&heartbeat={0}", pubnubPresenceHeartbeatInSeconds);
				}
				url.AppendFormat ("&pnsdk={0}", EncodeUricomponent (pnsdkVersion, type, false, true));
				break;

			case ResponseType.PresenceHeartbeat:

				url.AppendFormat ("?uuid={0}", uuid);
				url.Append (parameters);
				if (pubnubPresenceHeartbeatInSeconds != 0) {
					url.AppendFormat ("&heartbeat={0}", pubnubPresenceHeartbeatInSeconds);
				}
				if (!string.IsNullOrEmpty (authenticationKey)) {
					url.AppendFormat ("&auth={0}", EncodeUricomponent (authenticationKey, type, false, false));
				}
				url.AppendFormat ("&pnsdk={0}", EncodeUricomponent (pnsdkVersion, type, false, true));

				break;

			case ResponseType.SetUserState:

				url.Append (parameters);
				url.AppendFormat ("&uuid={0}", uuid);
				if (!string.IsNullOrEmpty (authenticationKey)) {
					url.AppendFormat ("&auth={0}", EncodeUricomponent (authenticationKey, type, false, false));
				}
				url.AppendFormat ("&pnsdk={0}", EncodeUricomponent (pnsdkVersion, type, false, true));
				break;

			case ResponseType.GetUserState:

				url.AppendFormat ("?uuid={0}", uuid);
				if (!string.IsNullOrEmpty (authenticationKey)) {
					url.AppendFormat ("&auth={0}", EncodeUricomponent (authenticationKey, type, false, false));
				}
				url.AppendFormat ("&pnsdk={0}", EncodeUricomponent (pnsdkVersion, type, false, true));
				break;
			case ResponseType.HereNow:

				url.Append (parameters);
				url.AppendFormat ("&uuid={0}", uuid);
				if (!string.IsNullOrEmpty (authenticationKey)) {
					url.AppendFormat ("&auth={0}", EncodeUricomponent (authenticationKey, type, false, false));
				}
				url.AppendFormat ("&pnsdk={0}", EncodeUricomponent (pnsdkVersion, type, false, true));
				break;

			case ResponseType.GlobalHereNow:

				url.Append (parameters);
				url.AppendFormat ("&uuid={0}", uuid);
				if (!string.IsNullOrEmpty (authenticationKey)) {
					url.AppendFormat ("&auth={0}", EncodeUricomponent (authenticationKey, type, false, false));
				}
				url.AppendFormat ("&pnsdk={0}", EncodeUricomponent (pnsdkVersion, type, false, true));
				break;

			case ResponseType.WhereNow:

				url.AppendFormat ("?uuid={0}", uuid);
				if (!string.IsNullOrEmpty (authenticationKey)) {
					url.AppendFormat ("&auth={0}", EncodeUricomponent (authenticationKey, type, false, false));
				}
				url.AppendFormat ("&pnsdk={0}", EncodeUricomponent (pnsdkVersion, type, false, true));
				break;

			case ResponseType.Publish:

				url.AppendFormat ("?uuid={0}", uuid);
				if (parameters != "") {
					url.AppendFormat ("&{0}", parameters);
				}
				if (!string.IsNullOrEmpty (authenticationKey)) {
					url.AppendFormat ("&auth={0}", EncodeUricomponent (authenticationKey, type, false, false));
				}
				url.AppendFormat ("&pnsdk={0}", EncodeUricomponent (pnsdkVersion, type, false, true));
				break;

			case ResponseType.DetailedHistory:
			case ResponseType.GrantAccess:
			case ResponseType.AuditAccess:
			case ResponseType.RevokeAccess:
				url.Append (parameters);
				break;
			default:
				url.AppendFormat ("?uuid={0}", uuid);
				url.AppendFormat ("&pnsdk={0}", EncodeUricomponent (pnsdkVersion, type, false, true));
				break;
			}

			Uri requestUri = new Uri (url.ToString ());

			return requestUri;

		}
		#endregion
	}
}

