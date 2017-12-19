using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;
using UnityEngine.TestTools;

namespace PubNubMessaging.Tests
{
    public class TestDetailedHistory
    {
        string name = "TestDelUserStateCG";
        public bool SslOn = false;
        public bool CipherOn = false;
        public bool AsObject = false;
        public bool NoStore = false;
        
        [UnityTest]
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();
            object[] message = {"Test Detailed History"};

            yield return common.DoPublishThenDetailedHistoryAndParse(SslOn, this.name, message, AsObject, CipherOn, NoStore, message.Length, false);
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", this.name));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);

        }
    }
}