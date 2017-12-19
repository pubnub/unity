using System;
using UnityEngine.TestTools;
using UnityEngine;
using Pathfinding.Serialization.JsonFx;
using PubNubMessaging.Core;
using System.Collections;
using UnityEngine.TestTools;

namespace PubNubMessaging.Tests
{
    public class TestSubscribeLongArray
    {
        string name = "TestSubscribeLongArray";
        public long[] Message = {14255515120803306};
        public bool SslOn = false;
        public bool CipherOn = false;
        public bool AsObject = false;
        [UnityTest]
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();
            yield return common.DoSubscribeThenPublishAndParse(SslOn, this.name, AsObject, CipherOn, Message, "[\"14255515120803306\"]", true);
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", this.name));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
        }
    }
}

