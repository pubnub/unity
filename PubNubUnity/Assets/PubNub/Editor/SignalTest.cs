using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;
using System.Collections.Generic;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class SignalsTests
    {
        #if DEBUG   
        [Test]
        public void TestSignalRequest ()
        {
            TestSignalCommon (true, false);
        }

        [Test]
        public void TestSignalRequestQueryParam ()
        {
            TestSignalCommon (true, true);
        }

        [Test]
        public void TestAddListenerSignalCallback ()
        {
            PNConfiguration pnConfiguration = new PNConfiguration ();
            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            bool receivedCallback = false;
            Action<PNSignalEventResult> callback = (pnser) => receivedCallback = true;
            pnUnity.AddListener(null, null, null, callback, null, null, null, null);

            pnUnity.RaiseEvent(new SubscribeEventEventArgs()
            {
                SignalEventResult = new PNSignalEventResult(null, null, null, 0, 0, null, null)
            });

            Assert.IsTrue(receivedCallback);
        }

        public void TestSignalCommon(bool ssl, bool sendQueryParams){
            string channel = EditorCommon.GetRandomChannelName();
            string message = "Test signal";
            string uuid = "customuuid";
            string signature = "0";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);


            string originalMessage = Helpers.JsonEncodePublishMsg (message, "", pnUnity.JsonLibrary, new PNLoggingMethod(pnConfiguration.LogVerbosity));

            Uri uri = BuildRequests.BuildSignalRequest (channel, originalMessage, pnUnity, queryParams);

            https://ps.pndsn.com/signal/demo/demo/0/UnityUnitTests_69/0/%22Test%20signal%22?uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.3.0 
            string expected = string.Format ("http{0}://{1}/signal/{2}/{3}/{4}/{5}/0/{6}?uuid={7}&pnsdk={8}{9}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.PublishKey, 
                EditorCommon.SubscribeKey, 
                signature, 
                channel, 
                Utility.EncodeUricomponent(originalMessage, PNOperationType.PNPublishOperation, false, false), 
                uuid, 
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }
        #endif
    }
}
