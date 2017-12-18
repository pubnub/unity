using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;
using UnityEngine.TestTools;

namespace PubNubMessaging.Tests
{
    //[IntegrationTest.DynamicTestAttribute ("TestPublishKeyOverride")]
    public class TestPublishKeyOverride
    {
        [UnityTest]
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();
            string TestName = "TestPublishKeyOverride";
            Pubnub pubnub = new Pubnub (
                "",
                "demo",
                "",
                "",
                false
            );

            common.SetPubnub = pubnub;

            yield return common.DoPublishAndParse(true, TestName, "Simple message test", "Sent", false, true);
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

        }
    }
}



