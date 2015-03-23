using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
	[IntegrationTest.DynamicTestAttribute ("TestDetailedHistorySSLCipher")]
	public class TestDetailedHistorySSLCipher: MonoBehaviour
	{
		public IEnumerator Start ()
		{
			CommonIntergrationTests common = new CommonIntergrationTests ();
			string TestName = "TestDetailedHistorySSLCipher";

			yield return StartCoroutine(common.DoPublishThenDetailedHistoryAndParse(true, TestName, "Test Detailed History", true, true, false, 1));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

		}
	}
}
