using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;
using UnityEngine.TestTools;

namespace PubNubMessaging.Tests
{
    public class TestHereNow
    {
        string name = "TestHereNow";
        public bool SslOn = false;
        public bool CipherOn = false;
        public bool AsObject = false;
        public bool WithState = false;
        public string CustomUUID = "";
        [UnityTest]
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();
            yield return common.DoSubscribeThenHereNowAndParse (SslOn, this.name, !AsObject, WithState, CustomUUID);
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", this.name));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
        }
    }
}

