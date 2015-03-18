using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
	[IntegrationTest.DynamicTestAttribute ("TestCustomUUID")]
	public class TestCustomUUID: MonoBehaviour
	{
		public IEnumerator Start ()
		{
			CommonIntergrationTests common = new CommonIntergrationTests ();
			string testName = "TestCustomUUID";

			yield return StartCoroutine(common.DoSubscribeThenHereNowAndParse(false, testName, true, false, testName));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", testName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

		}
	}
}

