using System;
using System.Globalization;
using PubnubApi;

public class UnityPubNubLogger : IPubnubLogger {

	private string id;

	public UnityPubNubLogger(string id) {
		this.id = id;
	}

	public void Trace(string logMessage) {
		UnityEngine.Debug.Log($"{DateTime.Now.ToString(CultureInfo.InvariantCulture)} PubNub-{id} Trace {logMessage}");
	}

	public void Debug(string logMessage) {
		UnityEngine.Debug.Log($"{DateTime.Now.ToString(CultureInfo.InvariantCulture)} PubNub-{id} Debug {logMessage}");
	}

	public void Info(string logMessage) {
		UnityEngine.Debug.Log($"{DateTime.Now.ToString(CultureInfo.InvariantCulture)} PubNub-{id} Info {logMessage}");
	}

	public void Warn(string logMessage) {
		UnityEngine.Debug.LogWarning($"{DateTime.Now.ToString(CultureInfo.InvariantCulture)} PubNub-{id} Warn {logMessage}");
	}

	public void Error(string logMessage) {
		UnityEngine.Debug.LogError($"{DateTime.Now.ToString(CultureInfo.InvariantCulture)} PubNub-{id} Error {logMessage}");
	}
}
