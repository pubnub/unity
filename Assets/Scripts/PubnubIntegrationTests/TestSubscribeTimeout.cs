using System;
using UnityEngine;
using System.Collections;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
    public class TestSubscribeTimeout: MonoBehaviour
    {
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();

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
            string url = string.Format ("http://pubsub.pubnub.com/subscribe/{0}/{1}/0/0?uuid={2}&pnsdk={3}", CommonIntergrationTests.SubscribeKey, 
                             expectedChannels, pubnub.SessionUUID, pubnub.Version
                         );
            ResponseType respType =  ResponseType.Subscribe;

            IEnumerator ienum = common.TestCoroutineRunProcessResponse(url, 20, -1, multiChannel, false,
                false, this.name, expectedMessage, expectedChannels, false, false, false, 0, crt, respType);
            yield return StartCoroutine(ienum);

            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", this.name));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
        }
    }
}

