using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
	[IntegrationTest.DynamicTestAttribute ("TestDetailedHistorySSLAsString")]
	public class TestDetailedHistorySSLAsString: MonoBehaviour
	{
		public IEnumerator Start ()
		{
			CommonIntergrationTests common = new CommonIntergrationTests ();
			string TestName = "TestDetailedHistorySSLAsString";

			object[] message = {"Test Detailed History 1","Test Detailed History 2","Test Detailed History 3","Test Detailed History 4",
				"Test Detailed History 5","Test Detailed History 6","Test Detailed History 7","Test Detailed History 8",
				"Test Detailed History 9","Test Detailed History 10"};

			yield return StartCoroutine(common.DoPublishThenDetailedHistoryAndParse(true, TestName, message, false, false, false, message.Length, false));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

		}
	}
}

