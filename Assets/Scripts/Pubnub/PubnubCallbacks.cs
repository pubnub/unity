using System;
using System.Collections.Generic;
using System.Net;

namespace PubNubMessaging.Core
{
	internal static class PubnubCallbacks
	{
		#region "Error Callbacks"

		internal static PubnubClientError CallErrorCallback (PubnubErrorSeverity errSeverity, PubnubMessageSource msgSource,
			string channel, Action<PubnubClientError> errorCallback, 
			string message, PubnubErrorCode errorType, PubnubWebRequest req, 
			PubnubWebResponse res)
		{
			int statusCode = (int)errorType;

			string errorDescription = PubnubErrorCodeDescription.GetStatusCodeDescription (errorType);

			PubnubClientError error = new PubnubClientError (statusCode, errSeverity, message, msgSource, req, res, errorDescription, channel);
			GoToCallback (error, errorCallback);
			return error;
		}

		internal static PubnubClientError CallErrorCallback (PubnubErrorSeverity errSeverity, PubnubMessageSource msgSource,
			string channel, Action<PubnubClientError> errorCallback, 
			string message, int currentHttpStatusCode, string statusMessage,
			PubnubWebRequest req, PubnubWebResponse res)
		{
			PubnubErrorCode pubnubErrorType = PubnubErrorCodeHelper.GetErrorType ((int)currentHttpStatusCode, statusMessage);

			int statusCode = (int)pubnubErrorType;

			string errorDescription = PubnubErrorCodeDescription.GetStatusCodeDescription (pubnubErrorType);

			PubnubClientError error = new PubnubClientError (statusCode, errSeverity, message, msgSource, req, res, errorDescription, channel);
			GoToCallback (error, errorCallback);
			return error;
		}

		internal static PubnubClientError CallErrorCallback (PubnubErrorSeverity errSeverity, PubnubMessageSource msgSource,
			string channel, Action<PubnubClientError> errorCallback, 
			Exception ex, PubnubWebRequest req, 
			PubnubWebResponse res)
		{
			PubnubErrorCode errorType = PubnubErrorCodeHelper.GetErrorType (ex);

			int statusCode = (int)errorType;
			string errorDescription = PubnubErrorCodeDescription.GetStatusCodeDescription (errorType);

			PubnubClientError error = new PubnubClientError (statusCode, errSeverity, true, ex.Message, ex, msgSource, req, res, errorDescription, channel);
			GoToCallback (error, errorCallback);
			return error;
		}

		internal static PubnubClientError CallErrorCallback (PubnubErrorSeverity errSeverity, PubnubMessageSource msgSource,
			string channel, Action<PubnubClientError> errorCallback, 
			WebException webex, PubnubWebRequest req, 
			PubnubWebResponse res)
		{
			PubnubErrorCode errorType = PubnubErrorCodeHelper.GetErrorType (webex.Status, webex.Message);
			int statusCode = (int)errorType;
			string errorDescription = PubnubErrorCodeDescription.GetStatusCodeDescription (errorType);

			PubnubClientError error = new PubnubClientError (statusCode, errSeverity, true, webex.Message, webex, msgSource, req, res, errorDescription, channel);
			GoToCallback (error, errorCallback);
			return error;
		}

		internal static void JsonResponseToCallback<T> (List<object> result, Action<T> callback, IJsonPluggableLibrary jsonPluggableLibrary)
		{
			string callbackJson = "";

			if (typeof(T) == typeof(string)) {
				callbackJson = jsonPluggableLibrary.SerializeToJsonString (result);

				Action<string> castCallback = callback as Action<string>;
				castCallback (callbackJson);
			}
		}

		internal static void JsonResponseToCallback<T> (object result, Action<T> callback, IJsonPluggableLibrary jsonPluggableLibrary)
		{
			string callbackJson = "";

			if (typeof(T) == typeof(string)) {
				try {
					//LoggingMethod.WriteToLog (string.Format ("DateTime {0}, before _jsonPluggableLibrary.SerializeToJsonString", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
					callbackJson = jsonPluggableLibrary.SerializeToJsonString (result);
					LoggingMethod.WriteToLog (string.Format ("DateTime {0}, after _jsonPluggableLibrary.SerializeToJsonString {1}", DateTime.Now.ToString (), callbackJson), LoggingMethod.LevelInfo);
					Action<string> castCallback = callback as Action<string>;
					castCallback (callbackJson);
				} catch (Exception ex) {
					LoggingMethod.WriteToLog (string.Format ("DateTime {0}, JsonResponseToCallback = {1} ", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);        
				}
			}
		}

		internal static void GoToCallback<T> (object result, Action<T> Callback)
		{
			if (Callback != null) {
				if (typeof(T) == typeof(string)) {
					JsonResponseToCallback (result, Callback);
				} else {
					Callback ((T)(object)result);
				}
			}
		}

		internal static void GoToCallback (object result, Action<string> Callback)
		{
			if (Callback != null) {
				JsonResponseToCallback (result, Callback);
			}
		}

		internal static void GoToCallback (object result, Action<object> Callback)
		{
			if (Callback != null) {
				Callback (result);
			}
		}

		internal static void GoToCallback (PubnubClientError error, Action<PubnubClientError> Callback, PubnubErrorFilter.Level errorLevel)
		{
			if (Callback != null && error != null) {
				if ((int)error.Severity <= (int)errorLevel) { //Checks whether the error serverity falls in the range of error filter level
					//Do not send 107 = PubnubObjectDisposedException
					//Do not send 105 = WebRequestCancelled
					//Do not send 130 = PubnubClientMachineSleep
					if (error.StatusCode != 107
						&& error.StatusCode != 105
						&& error.StatusCode != 130
						&& error.StatusCode != 4040) { //Error Code that should not go out
						Callback (error);
					}
				}
			}
		}


		#endregion
	}
}

