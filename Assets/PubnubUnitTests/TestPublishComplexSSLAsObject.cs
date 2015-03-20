using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
	[IntegrationTest.DynamicTestAttribute ("TestPublishComplexSSLAsObject")]
	public class TestPublishComplexSSLAsObject: MonoBehaviour
	{
		public IEnumerator Start ()
		{
			CommonIntergrationTests common = new CommonIntergrationTests ();
			string TestName = "TestPublishComplexSSLAsObject";

			object message = new PubnubDemoObject ();

			yield return StartCoroutine(common.DoPublishAndParse(true, TestName, message, "Sent", true));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

		}
	}
}



