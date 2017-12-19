using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace PubNubMessaging.Tests
{
    //[IntegrationTest.DynamicTestAttribute ("TestSetAndGetGlobalState")]
    public class TestSetAndGetGlobalState
    {
        [UnityTest]
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();
            string testName = "TestSetAndGetGlobalState";

            yield return common.SetAndGetStateAndParse(false, testName);
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", testName));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
        }
    }
}

