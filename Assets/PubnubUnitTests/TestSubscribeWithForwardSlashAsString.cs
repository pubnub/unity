using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
	[IntegrationTest.DynamicTestAttribute ("TestSubscribeWithForwardSlashAsString")]
	public class TestSubscribeWithForwardSlashAsString: MonoBehaviour
	{
		public IEnumerator Start ()
		{
			CommonIntergrationTests common = new CommonIntergrationTests ();
			string TestName = "TestSubscribeWithForwardSlashAsString";

			string message = "Test message with /";

			yield return StartCoroutine(common.DoSubscribeThenPublishAndParse(false, TestName, false, false, message));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

		}
	}
}
