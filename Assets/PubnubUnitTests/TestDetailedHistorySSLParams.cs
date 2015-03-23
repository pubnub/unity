using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
	[IntegrationTest.DynamicTestAttribute ("TestDetailedHistorySSLParams")]
	public class TestDetailedHistorySSLParams: MonoBehaviour
	{
		public IEnumerator Start ()
		{
			CommonIntergrationTests common = new CommonIntergrationTests ();
			string TestName = "TestDetailedHistorySSLParams";

			yield return StartCoroutine(common.DoPublishThenDetailedHistoryAndParse(true, TestName, "Test Detailed History", true, false, false, 10));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

		}
	}
}
