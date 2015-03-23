using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
	[IntegrationTest.DynamicTestAttribute ("TestDetailedHistoryCipher")]
	public class TestDetailedHistoryCipher: MonoBehaviour
	{
		public IEnumerator Start ()
		{
			CommonIntergrationTests common = new CommonIntergrationTests ();
			string TestName = "TestDetailedHistoryCipher";

			yield return StartCoroutine(common.DoPublishThenDetailedHistoryAndParse(false, TestName, "Test Detailed History", true, true, false, 1));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

		}
	}
}
