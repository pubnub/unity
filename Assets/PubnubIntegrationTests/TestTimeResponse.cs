using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;
using UnityEngine.TestTools;

namespace PubNubMessaging.Tests
{
    public class TestTimeResponse
    {
        string name = "TestTimeResponse";
        public bool SslOn = false;
        public bool CipherOn = false;
        public bool AsObject = false;
        [UnityTest]
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();
            yield return common.DoTimeAndParse(SslOn, this.name, AsObject);
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", this.name));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
        }
    }
}