using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
	[IntegrationTest.DynamicTestAttribute ("TestDetailedHistoryNoStore")]
	public class TestDetailedHistoryNoStore: MonoBehaviour
	{
		public IEnumerator Start ()
		{
			CommonIntergrationTests common = new CommonIntergrationTests ();
			string TestName = "TestDetailedHistoryNoStore";

			yield return StartCoroutine(common.DoPublishThenDetailedHistoryAndParse(true, TestName, "Simple message test No Store", false, false, true, 1));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

		}
	}
}