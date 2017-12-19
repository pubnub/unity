using System;
using UnityEngine;
using System.Collections;
using PubNubMessaging.Core;
using UnityEngine.TestTools;

namespace PubNubMessaging.Tests
{
    public class TestCoroutineRunIntegrationSubError
    {
        string name = "TestCoroutineRunIntegrationSubError";
        [UnityTest]
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();
            string url = "http://ps.pndsn.com";
            string[] multiChannel = {"testChannel"};

            //Inducing Error by setting wrong request type
            CurrentRequestType crt = CurrentRequestType.Subscribe;
            string expectedMessage = "404 Nothing";
            string expectedChannels = string.Join (",", multiChannel);
            ResponseType respType =  ResponseType.SubscribeV2;

            IEnumerator ienum = common.TestCoroutineRunError(url, 20, -1, multiChannel, false,
                false, this.name, expectedMessage, expectedChannels, true, false, false, 0, crt, respType);
            yield return ienum;

            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", this.name));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
        }
    }
}

