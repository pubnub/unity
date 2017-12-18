using System;
using UnityEngine.TestTools;
using UnityEngine;
using Pathfinding.Serialization.JsonFx;
using PubNubMessaging.Core;
using System.Collections;

namespace PubNubMessaging.Tests
{
    public class TestSubscribeJoin
    {
        string name = "TestSubscribeJoin";
        CommonIntergrationTests common = new CommonIntergrationTests ();

        public bool SslOn = false;
        public bool AsObject = false;
        [UnityTest]
        public IEnumerator Start ()
        {
            yield return common.DoPresenceThenSubscribeAndParse(SslOn, this.name, AsObject);
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", this.name));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
        }

    }
}

