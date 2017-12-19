using System;
using System.Collections;
using UnityEngine;
using PubNubMessaging.Core;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace PubNubMessaging.Tests
{
    //[IntegrationTest.DynamicTestAttribute ("TestPublishKeyPresent")]
    public class TestPublishKeyPresent
    {
        [UnityTest]
        public IEnumerator Start ()
        {
            CommonIntergrationTests common = new CommonIntergrationTests ();
            string TestName = "TestPublishKeyPresent";
            Pubnub pubnub = new Pubnub (
                "",
                "demo",
                "",
                "",
                false
            );
            //UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Exception, "MissingMemberException: Invalid publish key");
            var ex = Assert.Throws<MissingMemberException>(() =>  pubnub.Publish<object> ("testchannel", "testmessage", common.DisplayReturnMessage, common.DisplayReturnMessage)); 
            
            Assert.That(ex.Message.Contains("Invalid publish key"), ex.Message, null);
            UnityEngine.Debug.Log (string.Format("{0}: After StartCoroutine", TestName));
            yield return new WaitForSeconds (CommonIntergrationTests.WaitTimeBetweenCalls);
            

        }
    }
}



