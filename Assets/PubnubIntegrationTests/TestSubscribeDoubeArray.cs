using System;
using UnityEngine.TestTools;
using UnityEngine;
using Pathfinding.Serialization.JsonFx;
using PubNubMessaging.Core;
using System.Collections;

namespace PubNubMessaging.Tests
{
    public class TestSubscribeDoubeArray
    {
        string name = "TestSubscribeDoubeArray";
        public double[] Message = {1.2};
        public bool SslOn = false;
        public bool CipherOn = false;
        public bool AsObject = false;
        [UnityTest]
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();
            yield return common.DoSubscribeThenPublishAndParse(SslOn, this.name, AsObject, CipherOn, Message, "[1.2]", true);
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", this.name));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
        }
    }
}

