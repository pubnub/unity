using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
	[IntegrationTest.DynamicTestAttribute ("TestPublishComplexEncrypted")]
	public class TestPublishComplexEncrypted: MonoBehaviour
	{
		public IEnumerator Start ()
		{
			CommonIntergrationTests common = new CommonIntergrationTests ();
			string TestName = "TestPublishComplexEncrypted";

			object message = new PubnubDemoObject ();

			yield return StartCoroutine(common.DoPublishAndParse(false, TestName, message, "Sent", false, true));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

		}
	}
}




