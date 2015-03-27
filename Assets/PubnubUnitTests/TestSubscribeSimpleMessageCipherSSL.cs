using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
	[IntegrationTest.DynamicTestAttribute ("TestSubscribeSimpleMessageCipherSSL")]
	public class TestSubscribeSimpleMessageCipherSSL: MonoBehaviour
	{
		public IEnumerator Start ()
		{
			CommonIntergrationTests common = new CommonIntergrationTests ();
			string TestName = "TestSubscribeSimpleMessageCipherSSL";

			string message = "Test message";

			yield return StartCoroutine(common.DoSubscribeThenPublishAndParse(true, TestName, true, true, message));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

		}
	}
}
