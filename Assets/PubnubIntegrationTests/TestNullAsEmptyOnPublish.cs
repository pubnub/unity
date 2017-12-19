using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace PubNubMessaging.Tests
{
    //[IntegrationTest.DynamicTestAttribute ("TestNullAsEmptyOnPublish")]
    public class TestNullAsEmptyOnPublish
    {
        [UnityTest]
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();
            string TestName = "TestNullAsEmptyOnPublish";
            Pubnub pubnub = new Pubnub (
                null,
                "demo",
                null,
                null,
                false
            );

            common.SetPubnub = pubnub;
            var ex = Assert.Throws<MissingMemberException>(() =>  pubnub.Publish<object> ("testchannel", "testmessage", common.DisplayReturnMessage, common.DisplayReturnMessage)); 
            
            Assert.That(ex.Message.Contains("Invalid publish key"), ex.Message, null);
            
            //pubnub.Publish<object> ("testchannel", "testmessage", common.DisplayReturnMessage, common.DisplayReturnMessage);
            
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
            
        }
    }
}




