using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;
using UnityEngine.TestTools;

namespace PubNubMessaging.Tests
{
    //[IntegrationTest.DynamicTestAttribute ("TestPresenceHeartbeat")]
    public class TestPresenceHeartbeat
    {
        [UnityTest]
        public IEnumerator Start ()
        {
            //CommonIntergrationTests common = new CommonIntergrationTests ();
            string TestName = "TestPresenceHeartbeat";

            //yield return StartCoroutine(common.DoSubscribeThenHereNowAsObjectAndParse(false, TestName));
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

        }
    }
}

