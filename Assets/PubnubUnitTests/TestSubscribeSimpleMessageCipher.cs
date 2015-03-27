using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
	[IntegrationTest.DynamicTestAttribute ("TestSubscribeSimpleMessageCipher")]
	public class TestSubscribeSimpleMessageCipher: MonoBehaviour
	{
		public IEnumerator Start ()
		{
			CommonIntergrationTests common = new CommonIntergrationTests ();
			string TestName = "TestSubscribeSimpleMessageCipher";

			string message = "Test message";

			yield return StartCoroutine(common.DoSubscribeThenPublishAndParse(false, TestName, true, true, message));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

		}
	}
}
