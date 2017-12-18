using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace PubNubMessaging.Tests
{
    //[IntegrationTest.DynamicTestAttribute ("TestSetAndGetGlobalStateUUID")]
    public class TestSetAndGetGlobalStateUUID
    {
        [UnityTest]
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();
            string testName = "TestSetAndGetGlobalStateUUID";

            yield return common.SetAndGetStateAndParseUUID(false, testName);
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", testName));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
        }
    }
}

