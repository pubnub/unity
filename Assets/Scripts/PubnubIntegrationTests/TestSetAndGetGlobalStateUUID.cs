using System;
using System.Collections;
using UnityEngine;

namespace PubNubMessaging.Tests
{
    [IntegrationTest.DynamicTestAttribute ("TestSetAndGetGlobalStateUUID")]
    public class TestSetAndGetGlobalStateUUID: MonoBehaviour
    {
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();
            string testName = "TestSetAndGetGlobalStateUUID";

            yield return StartCoroutine(common.SetAndGetStateAndParseUUID(false, testName));
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", testName));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
        }
    }
}

