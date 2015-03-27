using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
	[IntegrationTest.DynamicTestAttribute ("TestSubscribeWithUnicodeChars")]
	public class TestSubscribeWithUnicodeChars: MonoBehaviour
	{
		public IEnumerator Start ()
		{
			CommonIntergrationTests common = new CommonIntergrationTests ();
			string TestName = "TestSubscribeWithUnicodeChars";

			string message = "Text with ÜÖ漢語";

			yield return StartCoroutine(common.DoSubscribeThenPublishAndParse(false, TestName, false, false, message));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

		}
	}
}
