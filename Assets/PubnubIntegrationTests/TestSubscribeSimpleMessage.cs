using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;
using UnityEngine.TestTools;

namespace PubNubMessaging.Tests
{
    public class TestSubscribeSimpleMessage
    {
        string name = "TestSubscribeSimpleMessage";
        public string Message = "Test message";
        public bool SslOn = false;
        public bool CipherOn = false;
        public bool AsObject = false;
        [UnityTest]
        public IEnumerator Start ()
        {
			#if !PUBNUB_PS_V2_RESPONSE
            CommonIntergrationTests common = new CommonIntergrationTests ();
            yield return common.DoSubscribeThenPublishAndParse(SslOn, this.name, AsObject, CipherOn, Message, Message, false);
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", this.name));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
			#else
			yield return null;
			UnityEngine.Debug.Log (string.Format("{0}: Ignoring test", this.name));
			//Assert.Pass();
			#endif

        }
    }
}
