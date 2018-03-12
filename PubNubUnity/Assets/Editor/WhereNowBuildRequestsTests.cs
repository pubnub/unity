using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class WhereNowBuildRequestsTests
    {
        #if DEBUG 
        [Test]
        public void TestBuildWhereNowRequest ()
        {
            TestBuildWhereNowRequestCommon (false, "authKey", "");
        }

        [Test]
        public void TestBuildWhereNowRequestSSL ()
        {
            TestBuildWhereNowRequestCommon (true, "authKey", "");
        }

        [Test]
        public void TestBuildWhereNowRequestNoAuth ()
        {
            TestBuildWhereNowRequestCommon (false, "", "");
        }

        [Test]
        public void TestBuildWhereNowRequestSSLNoAuth ()
        {
            TestBuildWhereNowRequestCommon (true, "", "");
        }

        [Test]
        public void TestBuildWhereNowRequestSessionUUID ()
        {
            TestBuildWhereNowRequestCommon (false, "authKey", "sessionUUID");
        }

        [Test]
        public void TestBuildWhereNowRequestSSLSessionUUID ()
        {
            TestBuildWhereNowRequestCommon (true, "authKey", "sessionUUID");
        }

        [Test]
        public void TestBuildWhereNowRequestNoAuthSessionUUID ()
        {
            TestBuildWhereNowRequestCommon (false, "", "sessionUUID");
        }

        [Test]
        public void TestBuildWhereNowRequestSSLNoAuthSessionUUID ()
        {
            TestBuildWhereNowRequestCommon (true, "", "sessionUUID");
        }

        public void TestBuildWhereNowRequestCommon(bool ssl, string authKey, string uuid){
            string sessionUUID = "customuuid";

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.Secure = ssl;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = sessionUUID;
            pnConfiguration.AuthKey = authKey;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pnConfiguration.AuthKey);
            }

            Uri uri = BuildRequests.BuildWhereNowRequest (uuid, pnUnity);

            //http://ps.pndsn.com/v2/presence/sub_key/demo-36/uuid/customuuid?uuid=&auth=authKey&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            string expected = string.Format ("http{0}://{1}/v2/presence/sub_key/{2}/uuid/{3}?uuid={4}{5}&pnsdk={6}",
                ssl?"s":"", pnConfiguration.Origin, EditorCommon.SubscribeKey, uuid, sessionUUID,
                authKeyString,
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNWhereNowOperation, false, false)
            );
            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        #endif
    }
}        