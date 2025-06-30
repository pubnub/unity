// snippet.using
using PubnubApi;

// snippet.end
using System;
using System.Globalization;

public class LoggingSample
{
    // snippet.custom_logger
    // A custom logger that logs information on Unity console.
    // Use can implement logger that can log information using log4Net or file etc.
    // You can find a default Unity-specific logger in UnityPubNubLogger.cs
    public class PubnubUnityConsoleLoggerExample : IPubnubLogger {
	    public void Trace(string traceLog) =>
		    UnityEngine.Debug.Log($"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}] [TRACE] {traceLog}");

	    public void Debug(string debugLog) =>
	        UnityEngine.Debug.Log($"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}] [DEBUG] {debugLog}");

	    public void Info(string infoLog) =>
		    UnityEngine.Debug.Log($"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}] [INFO] {infoLog}");

	    public void Warn(string warningLog) =>
		    UnityEngine.Debug.LogWarning($"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}] [WARN] {warningLog}");

	    public void Error(string errorLog) =>
		    UnityEngine.Debug.LogError($"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}] [ERROR] {errorLog}");
    }
    // snippet.end

    public static void EnableLogging()
    {
        // snippet.enable_logging
        var pubnubConfiguration = new PNConfiguration(new UserId("uniqueUserId"))
        {
            SubscribeKey = "[yourSubscribeKey]",
            PublishKey = "[yourPublishKey]",
            LogLevel = PubnubLogLevel.Debug,
        };
        var pubnub = new Pubnub(pubnubConfiguration);

        // If you're using Unity Editor setup you can get the Pubnub instance from PNManagerBehaviour
        // For more details, see https://www.pubnub.com/docs/sdks/unity#configure-pubnub
        /*
        [SerializeField] private PNManagerBehaviour pubnubManager;
        Pubnub pubnub = pubnubManager.pubnub;
        */

        var customLogger = new PubnubUnityConsoleLoggerExample();
        pubnub.SetLogger(customLogger);

        // To remove the custom logger. Use RemoveLogger().
        pubnub.RemoveLogger(customLogger);
        // snippet.end
    }
}