using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;
using System.Collections.Generic;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class TimeBuildRequestsTests
    {
        #if DEBUG
        [Test]
        public void TestBuildTimeRequest ()
        {
            TestBuildTimeRequestCommon (false);
        }

        [Test]
        public void TestBuildTimeRequestSSL ()
        {
            TestBuildTimeRequestCommon (true);
        }

        [Test]
        public void TestBuildTimeRequestSSLQP ()
        {
            TestBuildTimeRequestCommon (true, true);
        }

        public void TestBuildTimeRequestCommon(bool ssl){
            TestBuildTimeRequestCommon(ssl, false);
        }

        public void TestBuildTimeRequestCommon(bool ssl, bool sendQueryParams){
            string uuid = "customuuid";
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
            pnConfiguration.Secure = ssl;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UserId = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            Uri uri = BuildRequests.BuildTimeRequest (pnUnity, queryParams);

            //https://ps.pndsn.com/time/0?uuid=customuuid&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            string expected = string.Format ("http{0}://{1}/time/0?uuid={2}&pnsdk={3}{4}",
                ssl?"s":"", pnConfiguration.Origin, uuid, 
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNTimeOperation, false, false),
                queryParamString
            );
            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        #endif
    }
}  