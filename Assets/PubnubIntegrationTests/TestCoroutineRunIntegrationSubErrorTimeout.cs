using System;
using UnityEngine;
using System.Collections;
using PubNubMessaging.Core;
using UnityEngine.TestTools;

namespace PubNubMessaging.Tests
{
    public class TestCoroutineRunIntegrationSubErrorTimeout
    {
        [UnityTest]
        public IEnumerator Start ()
        {
            /*CommonIntergrationTests common = new CommonIntergrationTests ();

            string[] multiChannel = {"testChannel"};

            Pubnub pubnub = new Pubnub (
                CommonIntergrationTests.PublishKey,
                CommonIntergrationTests.SubscribeKey,
                "",
                "",
                true
            );

            CurrentRequestType crt = CurrentRequestType.Subscribe;
            string expectedMessage = "[[],";
            string expectedChannels = string.Join (",", multiChannel);
            long nanoSecondTime = Pubnub.TranslateDateTimeToPubnubUnixNanoSeconds (DateTime.UtcNow);

            string url = string.Format ("http://ps.pndsn.com/subscribe/{0}/{1}/0/{2}?uuid={3}&pnsdk={4}", CommonIntergrationTests.SubscribeKey, 
                expectedChannels, nanoSecondTime, pubnub.SessionUUID, pubnub.Version
            );
            ResponseType respType =  ResponseType.Subscribe;

            IEnumerator ienum = common.TestCoroutineRunError(url, 5, -1, multiChannel, false,
                false, this.name, expectedMessage, expectedChannels, true, true, false, 0, crt, respType);
            
            yield return StartCoroutine(ienum);

            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", this.name));*/
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
        }
    }
}


