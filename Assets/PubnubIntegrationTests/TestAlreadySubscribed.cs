using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using PubNubMessaging.Core;

namespace PubNubMessaging.Tests
{
    public class TestAlreadySubscribed
    {
        public bool SslOn = false;
        public bool AsObject = false;
        public bool IsPresence = false;
        string name = "TestAlreadySubscribed";
        [UnityTest]
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();
            yield return common.DoAlreadySubscribeTest(SslOn, this.name, AsObject, IsPresence);
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", this.name));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
        }
    }
}


