using System;
using System.Collections;
using UnityEngine;

namespace PubNubMessaging.Tests
{
	[IntegrationTest.DynamicTestAttribute ("TestSubscribeJoinSSLAsString")]
	public class TestSubscribeJoinSSLAsString: MonoBehaviour
	{
		CommonIntergrationTests common = new CommonIntergrationTests ();
		string TestName = "TestSubscribeJoinSSLAsString";

		public IEnumerator Start ()
		{
			yield return StartCoroutine(common.DoPresenceSubscribeAndParse(true, TestName));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

		}

	}
}

