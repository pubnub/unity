using System;
using UnityEngine;
using System.Collections;
using PubNubMessaging.Core;
using UnityEngine.TestTools;

namespace PubNubMessaging.Tests
{
    public class TestCoroutineRunIntegrationTests
    {
        string name = "TestCoroutineRunIntegrationTests";
        [UnityTest]
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();
            string url = "https://ps.pndsn.com/time/0";
            string[] multiChannel = {"testChannel"};
            //
            CurrentRequestType crt = CurrentRequestType.NonSubscribe;
            string expectedMessage = "[14";
            string expectedChannels = string.Join (",", multiChannel);
            ResponseType respType =  ResponseType.Time;

            IEnumerator ienum = common.TestCoroutineRunProcessResponse(url, 20, -1, multiChannel, false,
                false, this.name, expectedMessage, expectedChannels, false, false, false, 0, crt, respType);
            yield return ienum;
            
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", this.name));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
        }
    }

}

