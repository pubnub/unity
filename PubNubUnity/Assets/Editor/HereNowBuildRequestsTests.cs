using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class HereNowBuildRequestsTests
    {
        #if DEBUG    
        [Test]
        public void TestBuildHereNowRequest ()
        {
            TestBuildHereNowRequestCommon (false, false, false, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestSSL ()
        {
            TestBuildHereNowRequestCommon (true, false, false, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUID ()
        {
            TestBuildHereNowRequestCommon (false, true, false, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDSSL ()
        {
            TestBuildHereNowRequestCommon (true, true, false, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestInclState ()
        {
            TestBuildHereNowRequestCommon (false, false, true, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestInclStateSSL ()
        {
            TestBuildHereNowRequestCommon (true, false, true, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDInclState ()
        {
            TestBuildHereNowRequestCommon (false, true, true, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDInclStateSSL ()
        {
            TestBuildHereNowRequestCommon (true, true, true, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestNoAuth ()
        {
            TestBuildHereNowRequestCommon (false, false, false, "");
        }

        [Test]
        public void TestBuildHereNowRequestSSLNoAuth ()
        {
            TestBuildHereNowRequestCommon (true, false, false, "");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDNoAuth ()
        {
            TestBuildHereNowRequestCommon (false, true, false, "");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDSSLNoAuth ()
        {
            TestBuildHereNowRequestCommon (true, true, false, "");
        }

        [Test]
        public void TestBuildHereNowRequestInclStateNoAuth ()
        {
            TestBuildHereNowRequestCommon (false, false, true, "");
        }

        [Test]
        public void TestBuildHereNowRequestInclStateSSLNoAuth ()
        {
            TestBuildHereNowRequestCommon (true, false, true, "");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDInclStateNoAuth ()
        {
            TestBuildHereNowRequestCommon (false, true, true, "");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDInclStateSSLNoAuth ()
        {
            TestBuildHereNowRequestCommon (true, true, true, "");
        }

        //=======CG

        public void TestBuildHereNowRequestCG ()
        {
            TestBuildHereNowRequestCommon (true, false, false, false, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestSSLCG ()
        {
            TestBuildHereNowRequestCommon (true, true, false, false, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDCG ()
        {
            TestBuildHereNowRequestCommon (true, false, true, false, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDSSLCG ()
        {
            TestBuildHereNowRequestCommon (true, true, true, false, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestInclStateCG ()
        {
            TestBuildHereNowRequestCommon (true, false, false, true, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestInclStateSSLCG ()
        {
            TestBuildHereNowRequestCommon (true, true, false, true, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDInclStateCG ()
        {
            TestBuildHereNowRequestCommon (true, false, true, true, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDInclStateSSLCG ()
        {
            TestBuildHereNowRequestCommon (true, true, true, true, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestNoAuthCG ()
        {
            TestBuildHereNowRequestCommon (true, false, false, false, "");
        }

        [Test]
        public void TestBuildHereNowRequestSSLNoAuthCG ()
        {
            TestBuildHereNowRequestCommon (true, true, false, false, "");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDNoAuthCG ()
        {
            TestBuildHereNowRequestCommon (true, false, true, false, "");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDSSLNoAuthCG ()
        {
            TestBuildHereNowRequestCommon (true, true, true, false, "");
        }

        [Test]
        public void TestBuildHereNowRequestInclStateNoAuthCG ()
        {
            TestBuildHereNowRequestCommon (true, false, false, true, "");
        }

        [Test]
        public void TestBuildHereNowRequestInclStateSSLNoAuthCG ()
        {
            TestBuildHereNowRequestCommon (true, true, false, true, "");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDInclStateNoAuthCG ()
        {
            TestBuildHereNowRequestCommon (true, false, true, true, "");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDInclStateSSLNoAuthCG ()
        {
            TestBuildHereNowRequestCommon (true, true, true, true, "");
        }

        //========CGnCH

        public void TestBuildHereNowRequestCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, false, false, false, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestSSLCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, true, false, false, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, false, true, false, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDSSLCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, true, true, false, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestInclStateCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, false, false, true, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestInclStateSSLCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, true, false, true, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDInclStateCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, false, true, true, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDInclStateSSLCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, true, true, true, "authKey");
        }

        [Test]
        public void TestBuildHereNowRequestNoAuthCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, false, false, false, "");
        }

        [Test]
        public void TestBuildHereNowRequestSSLNoAuthCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, true, false, false, "");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDNoAuthCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, false, true, false, "");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDSSLNoAuthCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, true, true, false, "");
        }

        [Test]
        public void TestBuildHereNowRequestInclStateNoAuthCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, false, false, true, "");
        }

        [Test]
        public void TestBuildHereNowRequestInclStateSSLNoAuthCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, true, false, true, "");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDInclStateNoAuthCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, false, true, true, "");
        }

        [Test]
        public void TestBuildHereNowRequestShowUUIDInclStateSSLNoAuthCGnCH ()
        {
            TestBuildHereNowRequestCommon (true, true, true, true, true, "");
        }

        public void TestBuildHereNowRequestCommon(bool ssl, bool showUUIDList, bool includeUserState, string authKey){
            TestBuildHereNowRequestCommon(true, false, ssl, showUUIDList, includeUserState, authKey);
        }

        public void TestBuildHereNowRequestCommon(bool testCg, bool ssl, bool showUUIDList, bool includeUserState, string authKey){
            TestBuildHereNowRequestCommon(false, testCg, ssl, showUUIDList, includeUserState, authKey);
        }

        public void TestBuildHereNowRequestCommon(bool testCh, bool testCg, bool ssl, bool showUUIDList, bool includeUserState, string authKey){
            
            string channel = "here_now_channel";
            string channelGroup = "here_now_channelGroup";
            string channelGroupStr = string.Format("&channel-group={0}",  Utility.EncodeUricomponent(channelGroup, PNOperationType.PNHereNowOperation, true, false));
            if(testCh && testCg){
                // test both
            } else if(testCg){
                channel = ",";
            } else {
                channelGroup = "";
                channelGroupStr = ""; 
            }
            string uuid = "customuuid";

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.Secure = ssl;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;
            pnConfiguration.AuthKey = authKey;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pnConfiguration.AuthKey);
            }

            int disableUUID = (showUUIDList) ? 0 : 1;
            int userState = (includeUserState) ? 1 : 0;
            string parameters = string.Format ("disable_uuids={0}&state={1}", disableUUID, userState);

            Uri uri = BuildRequests.BuildHereNowRequest (channel, channelGroup, showUUIDList, includeUserState, pnUnity);

            //http://ps.pndsn.com/v2/presence/sub_key/demo-36/channel/here_now_channel?disable_uuids=1&state=0&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX/3.6.9.0
            string expected = string.Format ("http{0}://{1}/v2/presence/sub_key/{2}/channel/{3}?{4}{8}&uuid={5}{6}&pnsdk={7}",
                ssl?"s":"", pnConfiguration.Origin, EditorCommon.SubscribeKey, channel, parameters,
                uuid, authKeyString, 
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNHereNowOperation, false, true),
                channelGroupStr
            );
            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        [Test]
        public void TestBuildGlobalHereNowRequest ()
        {
            TestBuildGlobalHereNowRequestCommon (false, false, false, "authKey");
        }

        [Test]
        public void TestBuildGlobalHereNowRequestSSL ()
        {
            TestBuildGlobalHereNowRequestCommon (true, false, false, "authKey");
        }

        [Test]
        public void TestBuildGlobalHereNowRequestShowUUID ()
        {
            TestBuildGlobalHereNowRequestCommon (false, true, false, "authKey");
        }

        [Test]
        public void TestBuildGlobalHereNowRequestShowUUIDSSL ()
        {
            TestBuildGlobalHereNowRequestCommon (true, true, false, "authKey");
        }

        [Test]
        public void TestBuildGlobalHereNowRequestInclState ()
        {
            TestBuildGlobalHereNowRequestCommon (false, false, true, "authKey");
        }

        [Test]
        public void TestBuildGlobalHereNowRequestInclStateSSL ()
        {
            TestBuildGlobalHereNowRequestCommon (true, false, true, "authKey");
        }

        [Test]
        public void TestBuildGlobalHereNowRequestShowUUIDInclState ()
        {
            TestBuildGlobalHereNowRequestCommon (false, true, true, "authKey");
        }

        [Test]
        public void TestBuildGlobalHereNowRequestShowUUIDInclStateSSL ()
        {
            TestBuildGlobalHereNowRequestCommon (true, true, true, "authKey");
        }

        [Test]
        public void TestBuildGlobalHereNowRequestNoAuth ()
        {
            TestBuildGlobalHereNowRequestCommon (false, false, false, "");
        }

        [Test]
        public void TestBuildGlobalHereNowRequestSSLNoAuth ()
        {
            TestBuildGlobalHereNowRequestCommon (true, false, false, "");
        }

        [Test]
        public void TestBuildGlobalHereNowRequestShowUUIDNoAuth ()
        {
            TestBuildGlobalHereNowRequestCommon (false, true, false, "");
        }

        [Test]
        public void TestBuildGlobalHereNowRequestShowUUIDSSLNoAuth ()
        {
            TestBuildGlobalHereNowRequestCommon (true, true, false, "");
        }

        [Test]
        public void TestBuildGlobalHereNowRequestInclStateNoAuth ()
        {
            TestBuildGlobalHereNowRequestCommon (false, false, true, "");
        }

        [Test]
        public void TestBuildGlobalHereNowRequestInclStateSSLNoAuth ()
        {
            TestBuildGlobalHereNowRequestCommon (true, false, true, "");
        }

        [Test]
        public void TestBuildGlobalHereNowRequestShowUUIDInclStateNoAuth ()
        {
            TestBuildGlobalHereNowRequestCommon (false, true, true, "");
        }

        [Test]
        public void TestBuildGlobalHereNowRequestShowUUIDInclStateSSLNoAuth ()
        {
            TestBuildGlobalHereNowRequestCommon (true, true, true, "");
        }

        public void TestBuildGlobalHereNowRequestCommon(bool ssl, bool showUUIDList, bool includeUserState, string authKey){
            string uuid = "customuuid";

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.Secure = ssl;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;
            pnConfiguration.AuthKey = authKey;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pnConfiguration.AuthKey);
            }

            int disableUUID = (showUUIDList) ? 0 : 1;
            int userState = (includeUserState) ? 1 : 0;
            string parameters = string.Format ("disable_uuids={0}&state={1}", disableUUID, userState);

            Uri uri = BuildRequests.BuildHereNowRequest ("", "", showUUIDList, includeUserState, pnUnity);

            //http://ps.pndsn.com/v2/presence/sub_key/demo-36?disable_uuids=1&state=0&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX/3.6.9.0
            string expected = string.Format ("http{0}://{1}/v2/presence/sub_key/{2}?{3}&uuid={4}{5}&pnsdk={6}",
                ssl?"s":"", pnConfiguration.Origin, EditorCommon.SubscribeKey, parameters,
                uuid, authKeyString, 
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNHereNowOperation, false, true)
            );
            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        

        #endif
    }
}

