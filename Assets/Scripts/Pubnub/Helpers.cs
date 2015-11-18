using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Linq;
using System.Collections;

namespace PubNubMessaging.Core
{
	internal static class Helpers
	{
		#region "Other Methods"

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

		private static object DecodeMessage (PubnubCrypto aes, object element, string[] channels, 
			Action<PubnubClientError> errorCallback, IJsonPluggableLibrary jsonPluggableLibrary, PubnubErrorFilter.Level errorLevel)
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

		private static List<object> DecryptCipheredMessage (List<object> message, string[] channels, 
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

		private static List<object> DecryptNonCipheredMessage (List<object> message)
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

		private static List<object> DecodeDecryptLoop (List<object> message, string[] channels, 
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

				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, {1} Subscribe JSON network error response={2}", 
					DateTime.Now.ToString (), (isPresence)?"Presence":"", message), LoggingMethod.LevelInfo);

			}
		}

		//TODO: Refactor 6
		/// <summary>
		/// Gets the result by wrapping the json response based on the request
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="jsonString"></param>
		/// <param name="channels"></param>
		/// <param name="reconnect"></param>
		/// <param name="lastTimetoken"></param>
		/// <param name="errorCallback"></param>
		/// <returns></returns>
		public static List<object> WrapResultBasedOnResponseType<T> (ResponseType type, string jsonString, string[] channels, 
			Action<PubnubClientError> errorCallback, SafeDictionary<PubnubChannelCallbackKey, 
			object> channelCallbacks, IJsonPluggableLibrary jsonPluggableLibrary, PubnubErrorFilter.Level errorLevel, string cipherKey)
		{
			List<object> result = new List<object> ();

			try {
				string multiChannel = (channels != null) ? string.Join (",", channels) : "";
				if (!string.IsNullOrEmpty (jsonString)) {
					LoggingMethod.WriteToLog (string.Format ("DateTime {0}, jsonString = {1}", DateTime.Now.ToString (), jsonString), LoggingMethod.LevelInfo);
					object deSerializedResult = jsonPluggableLibrary.DeserializeToObject (jsonString);
					List<object> result1 = ((IEnumerable)deSerializedResult).Cast<object> ().ToList ();

					if (result1 != null && result1.Count > 0) {
						result = result1;
					}

					switch (type) {
					case ResponseType.Publish:
						result.Add (multiChannel);
						break;
					case ResponseType.DetailedHistory:
						result = DecodeDecryptLoop (result, channels, errorCallback, cipherKey, jsonPluggableLibrary, errorLevel);
						result.Add (multiChannel);
						break;
					case ResponseType.HereNow:
						Dictionary<string, object> dictionary = jsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonString);
						result = new List<object> ();
						result.Add (dictionary);
						result.Add (multiChannel);
						break;
					case ResponseType.GlobalHereNow:
						Dictionary<string, object> globalHereNowDictionary = jsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonString);
						result = new List<object> ();
						result.Add (globalHereNowDictionary);
						break;
					case ResponseType.WhereNow:
						Dictionary<string, object> whereNowDictionary = jsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonString);
						result = new List<object> ();
						result.Add (whereNowDictionary);
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
						result.Add (multiChannel);
						break;
					case ResponseType.Leave:
						result.Add (multiChannel);
						break;
					case ResponseType.GrantAccess:
					case ResponseType.AuditAccess:
					case ResponseType.RevokeAccess:
						Dictionary<string, object> grantDictionary = jsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonString);
						result = new List<object> ();
						result.Add (grantDictionary);
						result.Add (multiChannel);
						break;
					case ResponseType.GetUserState:
					case ResponseType.SetUserState:
						Dictionary<string, object> userStateDictionary = jsonPluggableLibrary.DeserializeToDictionaryOfObject (jsonString);
						result = new List<object> ();
						result.Add (userStateDictionary);
						result.Add (multiChannel);
						break;
					default:
						break;
					}
					;//switch stmt end
					//}
				} else {
					LoggingMethod.WriteToLog (string.Format ("DateTime {0}, json string null ", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
				}
			} catch (Exception ex) {
				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WrapResultBasedOnResponseType exception: {1} ", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
				if (channels != null) {
					if (type == ResponseType.Subscribe
						|| type == ResponseType.Presence) {

						PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (ex, channels,
							PubnubErrorSeverity.Critical, channelCallbacks, 
							false, PubnubErrorCode.None, type, errorLevel);


					} else {
						if (errorCallback != null) {
							PubnubCallbacks.CallErrorCallback<T> (ex, null, string.Join (",", channels), 
								PubnubErrorCode.None, PubnubErrorSeverity.Critical, errorCallback, errorLevel);

						}
					}
				}
			}
			return result;
		}

		//TODO: Refactor 5
		internal static  void ResponseToUserCallback<T> (List<object> result, ResponseType type, string[] channels, 
			Action<T> userCallback, string cipherKey, SafeDictionary<PubnubChannelCallbackKey, 
			object> channelCallbacks, IJsonPluggableLibrary jsonPluggableLibrary)
		{
			string[] messageChannels;
			switch (type) {
			case ResponseType.Subscribe:
			case ResponseType.Presence:
				//try{
				var messages = (from item in result
					select item as object).ToArray ();
				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, result: {1}", DateTime.Now.ToString (), result.ToString ()), LoggingMethod.LevelInfo);
				if (messages != null && messages.Length > 0) {
					LoggingMethod.WriteToLog (string.Format ("DateTime {0}, messageList typeOF: {1}", DateTime.Now.ToString (), messages [0].GetType ().ToString ()), LoggingMethod.LevelInfo);
					var messageList = messages [0] as object[];
					int i = 0;
					foreach (object o in result) {
						if (i == 0) {
							IList collection = (IList)o;
							messageList = new object[collection.Count];
							bool added = false;
							int j = 0;
							foreach (object c in collection) {
								//LoggingMethod.WriteToLog (string.Format ("DateTime {0}, collection: {1} type: {2}", DateTime.Now.ToString (), c.ToString (), c.GetType ().ToString ()), LoggingMethod.LevelInfo);
								if ((c.GetType () == typeof(System.Int32)) || (c.GetType () == typeof(System.Double)) || (c.GetType () == typeof(System.Int64)) || (c.GetType () == typeof(System.Boolean))) {
									added = true;
									LoggingMethod.WriteToLog (string.Format ("DateTime {0}, collection: {1} in type: {2}", DateTime.Now.ToString (), c.ToString (), c.GetType ().ToString ()), LoggingMethod.LevelInfo);
									messageList [j] = c;

								} else if (c.GetType () == typeof(System.String)) {

									added = true;
									LoggingMethod.WriteToLog (string.Format ("DateTime {0}, collection: {1} in type: {2}", DateTime.Now.ToString (), c.ToString (), c.GetType ().ToString ()), LoggingMethod.LevelInfo);
									messageList [j] = c.ToString ();
								} else {
									try{
										messageList [j] = c;
										added = true;
										LoggingMethod.WriteToLog (string.Format ("DateTime {0}, collection other types: {1} in type: {2}", DateTime.Now.ToString (), c.ToString (), c.GetType ().ToString ()), LoggingMethod.LevelInfo);
									}catch (Exception ex){
										added = false;
										LoggingMethod.WriteToLog (string.Format ("DateTime {0}, collection other types: {1} in type: {2}, exception {3} ", DateTime.Now.ToString (), c.ToString (), c.GetType ().ToString (), ex.ToString()), LoggingMethod.LevelInfo);
									}
								}
								j++;
							}

							if (!added) {
								collection.CopyTo (messageList, 0);
							}
						}
						i++;
					}

					messageChannels = messages [2].ToString ().Split (',');
					LoggingMethod.WriteToLog (string.Format ("DateTime {0}, (messageChannels: {1}", DateTime.Now.ToString (), messageChannels.ToString ()), LoggingMethod.LevelInfo);
					if (messageList != null && messageList.Length > 0) {
						for (int messageIndex = 0; messageIndex < messageList.Length; messageIndex++) {
							string currentChannel = (messageChannels.Length == 1) ? (string)messageChannels [0] : (string)messageChannels [messageIndex];
							List<object> itemMessage = new List<object> ();
							if (currentChannel.Contains ("-pnpres")) {
								itemMessage.Add (messageList [messageIndex]);
							} else {
								//decrypt the subscriber message if cipherkey is available
								if (cipherKey.Length > 0) {
									object decodeMessage;
									try {
										PubnubCrypto aes = new PubnubCrypto (cipherKey);
										string decryptMessage = aes.Decrypt (messageList [messageIndex].ToString ());
										decodeMessage = (decryptMessage == "**DECRYPT ERROR**") ? decryptMessage : jsonPluggableLibrary.DeserializeToObject (decryptMessage);
									} catch (Exception decryptEx) {
										decodeMessage = messageList [messageIndex].ToString ();
										LoggingMethod.WriteToLog (string.Format ("DateTime {0}, decodeMessage Exception: {1}", DateTime.Now.ToString (), decryptEx.ToString ()), LoggingMethod.LevelError);
									}
									itemMessage.Add (decodeMessage);
								} else {
									itemMessage.Add (messageList [messageIndex]);
								}
							}
							itemMessage.Add (messages [1].ToString ());
							itemMessage.Add (currentChannel.Replace ("-pnpres", ""));


							PubnubChannelCallbackKey callbackKey = new PubnubChannelCallbackKey ();
							callbackKey.Channel = currentChannel;
							callbackKey.Type = (currentChannel.LastIndexOf ("-pnpres") == -1) ? ResponseType.Subscribe : ResponseType.Presence;
							LoggingMethod.WriteToLog (string.Format ("DateTime {0}, currentChannel: {1}", DateTime.Now.ToString (), currentChannel.ToString ()), LoggingMethod.LevelInfo);
							if (channelCallbacks.Count > 0 && channelCallbacks.ContainsKey (callbackKey)) {
								LoggingMethod.WriteToLog (string.Format ("DateTime {0}, typeof(T): {1}", DateTime.Now.ToString (), typeof(T).ToString ()), LoggingMethod.LevelInfo);
								if ((typeof(T) == typeof(string) && channelCallbacks [callbackKey].GetType ().Name.Contains ("[System.String]")) ||
									(typeof(T) == typeof(object) && channelCallbacks [callbackKey].GetType ().Name.Contains ("[System.Object]"))) {
									PubnubChannelCallback<T> currentPubnubCallback = channelCallbacks [callbackKey] as PubnubChannelCallback<T>;
									if (currentPubnubCallback != null && currentPubnubCallback.Callback != null) {
										PubnubCallbacks.GoToCallback<T> (itemMessage, currentPubnubCallback.Callback, jsonPluggableLibrary);
									}
								} else if (channelCallbacks [callbackKey].GetType ().FullName.Contains ("[System.String")) {
									PubnubChannelCallback<string> retryPubnubCallback = channelCallbacks [callbackKey] as PubnubChannelCallback<string>;
									if (retryPubnubCallback != null && retryPubnubCallback.Callback != null) {
										PubnubCallbacks.GoToCallback (itemMessage, retryPubnubCallback.Callback, jsonPluggableLibrary);
									}
								} else if (channelCallbacks [callbackKey].GetType ().FullName.Contains ("[System.Object")) {
									PubnubChannelCallback<object> retryPubnubCallback = channelCallbacks [callbackKey] as PubnubChannelCallback<object>;
									if (retryPubnubCallback != null && retryPubnubCallback.Callback != null) {
										PubnubCallbacks.GoToCallback (itemMessage, retryPubnubCallback.Callback, jsonPluggableLibrary);
									}
								}

							}
						}
					}
				}
				break;
			case ResponseType.Publish:
				if (result != null && result.Count > 0) {
					PubnubCallbacks.GoToCallback<T> (result, userCallback, jsonPluggableLibrary);
				}
				break;
			case ResponseType.DetailedHistory:
				if (result != null && result.Count > 0) {
					PubnubCallbacks.GoToCallback<T> (result, userCallback, jsonPluggableLibrary);
				}
				break;
			case ResponseType.HereNow:
				if (result != null && result.Count > 0) {
					PubnubCallbacks.GoToCallback<T> (result, userCallback, jsonPluggableLibrary);
				}
				break;
			case ResponseType.GlobalHereNow:
				if (result != null && result.Count > 0) {
					PubnubCallbacks.GoToCallback<T> (result, userCallback, jsonPluggableLibrary);
				}
				break;
			case ResponseType.WhereNow:
				if (result != null && result.Count > 0) {
					PubnubCallbacks.GoToCallback<T> (result, userCallback, jsonPluggableLibrary);
				}
				break;
			case ResponseType.Time:
				if (result != null && result.Count > 0) {
					PubnubCallbacks.GoToCallback<T> (result, userCallback, jsonPluggableLibrary);
				}
				break;
			case ResponseType.Leave:
				//No response to callback
				break;
			case ResponseType.GrantAccess:
			case ResponseType.AuditAccess:
			case ResponseType.RevokeAccess:
			case ResponseType.GetUserState:
			case ResponseType.SetUserState:
				if (result != null && result.Count > 0) {
					PubnubCallbacks.GoToCallback<T> (result, userCallback, jsonPluggableLibrary);
				}
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
							channel.Replace ("-pnpres", ""), jsonPluggableLibrary);
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

		internal static PubnubClientError CreatePubnubClientError<T>(Exception ex, 
			RequestState<T> requestState, string channel, PubnubErrorCode errorCode, PubnubErrorSeverity severity){

			if (errorCode.Equals (PubnubErrorCode.None)) {
				errorCode = PubnubErrorCodeHelper.GetErrorType (ex);
			}

			int statusCode = (int)errorCode;
			string errorDescription = PubnubErrorCodeDescription.GetStatusCodeDescription (errorCode);

			PubnubClientError error = new PubnubClientError (statusCode, severity, true, ex.Message, ex, 
				PubnubMessageSource.Client, (requestState==null)?null:requestState.Request, 
				(requestState==null)?null:requestState.Response, errorDescription, channel);

			LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PubnubClientError = {1}", DateTime.Now.ToString (), error.ToString ()), LoggingMethod.LevelInfo);
			return error;
		}

		internal static PubnubClientError CreatePubnubClientError<T>(string message, 
			RequestState<T> requestState, string channel, PubnubErrorCode errorCode, PubnubErrorSeverity severity){

			int statusCode = (int)errorCode;
			string errorDescription = PubnubErrorCodeDescription.GetStatusCodeDescription (errorCode);

			PubnubClientError error = new PubnubClientError (statusCode, severity, message, PubnubMessageSource.Client, 
				(requestState==null)?null:requestState.Request, (requestState==null)?null:requestState.Response, errorDescription, channel);

			LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PubnubClientError = {1}", DateTime.Now.ToString (), error.ToString ()), LoggingMethod.LevelInfo);
			return error;
		}

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

		public static string EncodeUricomponent (string s, ResponseType type, bool ignoreComma, bool ignorePercent2fEncode)
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

		public static string Md5 (string text)
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

		#endregion

	}
}

