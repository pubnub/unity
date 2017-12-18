using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace PubNubMessaging.Tests
{
    //[IntegrationTest.DynamicTestAttribute ("TestSetAndDeleteGlobalState")]
    public class TestSetAndDeleteGlobalState
    {
        [UnityTest]
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();
            string testName = "TestSetAndDeleteGlobalState";

            yield return common.SetAndDeleteStateAndParse(false, testName);
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", testName));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

        }

    }
}

