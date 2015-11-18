using System;

namespace PubNubMessaging.Core
{
	public class ExceptionHandlers
	{
		internal static void PublishExceptionHandler<T> (string channelName, bool requestTimeout, 
			Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
		{
			if (requestTimeout) {
				string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, JSON publish response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback<T> (message, null, channelName, 
					PubnubErrorCode.PublishOperationTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
			}
		}

		internal static void PAMAccessExceptionHandler<T> (string channelName, bool requestTimeout, 
			Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
		{
			if (requestTimeout) {
				string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, PAMAccessExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback<T> (message, null, channelName, 
					PubnubErrorCode.PAMAccessOperationTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
			}
		}

		internal static void WhereNowExceptionHandler<T> (string uuid, bool requestTimeout, 
			Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
		{
			if (requestTimeout) {
				string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, WhereNowExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback<T> (message, null, uuid, 
					PubnubErrorCode.WhereNowOperationTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
			}
		}

		internal static void HereNowExceptionHandler<T> (string channelName, bool requestTimeout, 
			Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
		{
			if (requestTimeout) {
				string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, HereNowExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback<T> (message, null, channelName, 
					PubnubErrorCode.HereNowOperationTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
			}
		}

		internal static void GlobalHereNowExceptionHandler<T> (bool requestTimeout, 
			Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
		{
			if (requestTimeout) {
				string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, GlobalHereNowExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback<T> (message, null, "", 
					PubnubErrorCode.GlobalHereNowOperationTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
			}
		}

		internal static void DetailedHistoryExceptionHandler<T> (string channelName, bool requestTimeout, 
			Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
		{
			if (requestTimeout) {
				string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, DetailedHistoryExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback<T> (message, null, channelName, 
					PubnubErrorCode.DetailedHistoryOperationTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
			}
		}

		internal static void TimeExceptionHandler<T> (bool requestTimeout, 
			Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
		{
			if (requestTimeout) {
				string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, TimeExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback<T> (message, null, "", 
					PubnubErrorCode.TimeOperationTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
			}
		}

		internal static void SetUserStateExceptionHandler<T> (string channelName, bool requestTimeout, 
			Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
		{
			if (requestTimeout) {
				string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, SetUserStateExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback<T> (message, null, channelName, 
					PubnubErrorCode.SetUserStateTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
			}
		}

		internal static void GetUserStateExceptionHandler<T> (string channelName, bool requestTimeout, 
			Action<PubnubClientError> errorCallback, PubnubErrorFilter.Level errorLevel)
		{
			if (requestTimeout) {
				string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

				LoggingMethod.WriteToLog (string.Format ("DateTime {0}, GetUserStateExceptionHandler response={1}", DateTime.Now.ToString (), message), LoggingMethod.LevelInfo);

				PubnubCallbacks.CallErrorCallback<T> (message, null, channelName, 
					PubnubErrorCode.GetUserStateTimeout, PubnubErrorSeverity.Critical, errorCallback, errorLevel);
			}
		}
	}
}

