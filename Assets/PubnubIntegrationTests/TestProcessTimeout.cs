using System;
using UnityEngine;
using System.Collections;
using PubNubMessaging.Core;
using UnityEngine.TestTools;

namespace PubNubMessaging.Tests
{
    public class TestProcessTimeout
    {
        [UnityTest]
        public IEnumerator Start ()
        {
            /*CommonIntergrationTests common = new CommonIntergrationTests ();
            string url = "https://ps.pndsn.com/time/0";
            string[] multiChannel = {"testChannel"};

            CurrentRequestType crt = CurrentRequestType.Subscribe;
            string expectedMessage = "[14";
            string expectedChannels = string.Join (",", multiChannel);
            ResponseType respType =  ResponseType.Subscribe;

            IEnumerator ienum = common.TestProcessTimeout<string>(url, 1, -1, multiChannel, false, respType, crt, 
                false, false, 0, expectedMessage);
            yield return StartCoroutine(ienum);

            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", this.name));*/
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
        }
    }
}


