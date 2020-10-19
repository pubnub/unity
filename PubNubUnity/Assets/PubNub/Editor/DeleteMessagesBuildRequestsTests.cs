using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class DeleteMessagesBuildRequestsTests
    {
        #if DEBUG  

        [Test]
        public void TestDeleteMessagesBuildRequestsSecretKeyError(){
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            bool statusError = false;
            string msg = "";
            pnUnity.DeleteMessages().Channel("test").Async((result, status) => {
				statusError = status.Error;
                msg = status.ErrorData.Info;
             });
            Assert.AreEqual (true, statusError);
            Assert.AreEqual ("SecretKey is required", msg);
            
        }
        [Test]
        public void TestDeleteMessagesBuildRequestsChannelError(){
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            bool statusError = false;
            string msg = "";
            pnUnity.DeleteMessages().Channel("").Async((result, status) => {
				statusError = status.Error;
                msg = status.ErrorData.Info;
             });
            Assert.AreEqual (true, statusError);
            Assert.AreEqual ("DeleteHistory Channel is empty", msg);
            
        }

        [Test]
        public void TestDeleteMessagesBuildRequests(){
            long startTime = -1;
            long endTime = -1;
            string channel = "DeleteMessagesChannel";
            string paramsInOrder = "pnsdk={0}&timestamp={1}&uuid=DeleteMessagesTestUUID";
            string uriParamString = "uuid={0}{1}{2}&timestamp={3}{4}&pnsdk={5}";

            DeleteMessagesBuildRequestsCommon (true, channel, "", startTime, endTime, paramsInOrder, uriParamString);
        }

        [Test]
        public void TestDeleteMessagesBuildRequestsStartEnd(){
            long startTime = 15078932998876451;
            long endTime = 15078933628583256;
            string channel = "DeleteMessagesChannel2";
            string paramsInOrder = "{0}&pnsdk={1}{2}&timestamp={3}&uuid=DeleteMessagesTestUUID";
            //uuid=PubNubUnityExample&start=15078932998876451&end=15078933628583256&timestamp=1534756366&auth=authKey&pnsdk=PubNub-CSharp-UnityWin%2F4.0.1
            string uriParamString = "uuid={0}{1}{2}&timestamp={3}{4}&pnsdk={5}";

            DeleteMessagesBuildRequestsCommon (true, channel, "", startTime, endTime, paramsInOrder, uriParamString);
        }

        [Test]
        public void TestDeleteMessagesBuildRequestsEnd(){
            long startTime = 15078932998876451;
            long endTime = -1;
            string channel = "DeleteMessagesChannel2";
            string paramsInOrder = "pnsdk={0}{1}&timestamp={2}&uuid=DeleteMessagesTestUUID";
            //uuid=PubNubUnityExample&start=15078932998876451&end=15078933628583256&timestamp=1534756366&auth=authKey&pnsdk=PubNub-CSharp-UnityWin%2F4.0.1
            string uriParamString = "uuid={0}{1}{2}&timestamp={3}{4}&pnsdk={5}";

            DeleteMessagesBuildRequestsCommon (true, channel, "", startTime, endTime, paramsInOrder, uriParamString);
        }

        [Test]
        public void TestDeleteMessagesBuildRequestsStart(){
            long startTime = -1;
            long endTime = 15078933628583256;
            string channel = "DeleteMessagesChannel2";
            string paramsInOrder = "{0}&pnsdk={1}&timestamp={2}&uuid=DeleteMessagesTestUUID";
            //uuid=PubNubUnityExample&start=15078932998876451&end=15078933628583256&timestamp=1534756366&auth=authKey&pnsdk=PubNub-CSharp-UnityWin%2F4.0.1
            string uriParamString = "uuid={0}{1}{2}&timestamp={3}{4}&pnsdk={5}";

            DeleteMessagesBuildRequestsCommon (true, channel, "", startTime, endTime, paramsInOrder, uriParamString);
        }

        [Test]
        public void TestDeleteMessagesBuildRequestsStartQP(){
            long startTime = -1;
            long endTime = 15078933628583256;
            string channel = "DeleteMessagesChannel2";
            string paramsInOrder = "{0}&pnsdk={1}&timestamp={2}&uuid=DeleteMessagesTestUUID";
            //uuid=PubNubUnityExample&start=15078932998876451&end=15078933628583256&timestamp=1534756366&auth=authKey&pnsdk=PubNub-CSharp-UnityWin%2F4.0.1
            string uriParamString = "uuid={0}{1}{2}&timestamp={3}{4}&pnsdk={5}";

            DeleteMessagesBuildRequestsCommon (true, channel, "", startTime, endTime, paramsInOrder, uriParamString, true);
        }

        public void DeleteMessagesBuildRequestsCommon(bool ssl, string channel, string authKey, long startTime, long endTime, string paramsInOrderVal, string uriParamString){
            DeleteMessagesBuildRequestsCommon(ssl, channel, authKey, startTime, endTime, paramsInOrderVal, uriParamString, false);
        }

        public void DeleteMessagesBuildRequestsCommon(bool ssl, string channel, string authKey, long startTime, long endTime, string paramsInOrderVal, string uriParamString, bool sendQueryParams){
            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            if(sendQueryParams){
                queryParams.Add("a","f");
            } else {
                queryParams = null;
            }
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.SecretKey = EditorCommon.SecretKey;
            pnConfiguration.Secure = ssl;
            pnConfiguration.CipherKey = "";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = "DeleteMessagesTestUUID";
            pnConfiguration.AuthKey = authKey;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pnConfiguration.AuthKey);
            }

            string startTimeString = "";
            if(startTime!=-1){
                startTimeString = string.Format("&start={0}", startTime);
            }

            string endTimeString = "";
            if(endTime!=-1){
                endTimeString = string.Format("&end={0}", endTime);
            }

            Uri uri = BuildRequests.BuildDeleteMessagesRequest (channel, startTime, endTime, pnUnity, queryParams);

            var segments = uri.Segments;
            Assert.AreEqual("v3/", segments[1]);
            Assert.AreEqual("history/", segments[2]);
            Assert.AreEqual("sub-key/", segments[3]);
            Assert.AreEqual(EditorCommon.SubscribeKey+"/", segments[4]);
            Assert.AreEqual("channel/", segments[5]);
            Assert.AreEqual(channel, segments[6]);
            
            var q = uri.Query;
            int index = q.IndexOf('?');
            var query = q.Substring(index + 1)
                            .Split('&');

            string timestamp = "";
            string sig = "";
            string version = Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNDeleteMessagesOperation, false, true);
                            
            foreach(string qkv in query){
                var kv = qkv.Split('=');

                UnityEngine.Debug.Log(string.Format("key:{0}, value:{1}",kv[0], kv[1]));
                switch (kv[0]){
                case "uuid":
                Assert.AreEqual(pnConfiguration.UUID,kv[1]);
                break;
                case "timestamp":
                timestamp = kv[1];
                break;
                case "pnsdk":
                Assert.AreEqual(version,kv[1]);
                break;
                case "a":
                Assert.AreEqual("f",kv[1]);
                break;
                case "signature":
                sig = kv[1];
                break;
                }
            }
            string uriParamStringFormatted = string.Format(
                uriParamString, 
                pnConfiguration.UUID, 
                startTimeString, 
                endTimeString, 
                timestamp, 
                authKeyString,
                version
                );
            
            Uri uriFormatted = new Uri(string.Format("http{0}://{1}/v3/history/sub-key/{2}/channel/{3}?{4}", ssl?"s":"", pnConfiguration.Origin, EditorCommon.SubscribeKey, channel, uriParamStringFormatted
                ));    
            UnityEngine.Debug.Log("uriFormatted: " + uriFormatted);
            
            string paramsInOrder = BuildRequests.SetParametersInOrder(uriFormatted);
            if((startTime==-1) && (endTime==-1)){
                paramsInOrderVal = string.Format(paramsInOrderVal, version, timestamp);
            }else if(startTime==-1){
                paramsInOrderVal = string.Format(paramsInOrderVal, endTimeString.Substring(1), version, timestamp);
            }else if(endTime==-1){
                paramsInOrderVal = string.Format(paramsInOrderVal, version, startTimeString, timestamp);
            } else {
                paramsInOrderVal = string.Format(paramsInOrderVal, endTimeString.Substring(1), version, startTimeString, timestamp);
            }
            UnityEngine.Debug.Log("paramsInOrder:" + paramsInOrder);
            UnityEngine.Debug.Log("paramsInOrderVal:" + paramsInOrderVal);
            Assert.AreEqual(paramsInOrderVal, paramsInOrder);

            string sigGenerated = BuildRequests.GenerateSignatureAndAddToURL(
                pnUnity, 
                uriFormatted, 
                string.Format("/v3/history/sub-key/{0}/channel/{1}", EditorCommon.SubscribeKey, channel));
            UnityEngine.Debug.Log("sigGenerated:" + sigGenerated);
            Assert.IsTrue(sigGenerated.Contains(string.Format("&signature={0}",sig))); 
            
        }
        #endif
    }
}
