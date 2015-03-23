using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
	[IntegrationTest.DynamicTestAttribute ("TestDetailedHistorySSLCipherParams")]
	public class TestDetailedHistorySSLCipherParams: MonoBehaviour
	{
		public IEnumerator Start ()
		{
			CommonIntergrationTests common = new CommonIntergrationTests ();
			string TestName = "TestDetailedHistorySSLCipherParams";

			yield return StartCoroutine(common.DoPublishThenDetailedHistoryAndParse(true, TestName, "Test Detailed History", true, true, false, 10));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

		}
	}
}
