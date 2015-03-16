using System;
using UnityTest;
using UnityEngine;
using Pathfinding.Serialization.JsonFx;
using PubNubMessaging.Core;
using System.Collections;

namespace PubNubMessaging.Tests
{
	[IntegrationTest.DynamicTestAttribute ("TestSubscribeJoin")]
	public class TestSubscribeJoin: MonoBehaviour
	{
		CommonIntergrationTests common = new CommonIntergrationTests ();
		string TestName = "TestSubscribeJoin";

		public IEnumerator Start ()
		{
			yield return StartCoroutine(common.DoPresenceSubscribeAndParse(false, TestName));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
		}

	}
}

