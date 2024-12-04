using PubnubApi;
using UnityEngine;

public class UnityPNLog : IPubnubLog
{
	public void WriteToLog(string logText) {
		Debug.Log(logText);
	}
}
