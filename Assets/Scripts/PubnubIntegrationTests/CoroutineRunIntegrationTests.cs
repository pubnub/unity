using System;
using UnityEngine;
using System.Collections;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
	public class CoroutineRunIntegrationTests: MonoBehaviour
	{
		public IEnumerator Start ()
		{
			CommonIntergrationTests common = new CommonIntergrationTests ();
            string url = "pubsub.pubnub.com";
            string[] multiChannel = {"testChannel"};

            CurrentRequestType crt = CurrentRequestType.Subscribe;
            string expectedMessage = "";
            string expectedChannels = string.Join (",", multiChannel);
            ResponseType respType =  ResponseType.Subscribe;

            yield return StartCoroutine(common.TestCoroutineRun(url, 20, -1, multiChannel, false,
                false, this.name, false, expectedMessage, expectedChannels, false, false, 0, crt, respType));
			UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", this.name));
			yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
		}
	}
}

