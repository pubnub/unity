using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class StateBuildRequestsTests
    {
        #if DEBUG    
        [Test]
        public void TestBuildSetUserStateRequest ()
        {
            TestBuildSetUserStateRequestCommon (false, "");
        }

        [Test]
        public void TestBuildSetUserStateRequestAuth ()
        {
            TestBuildSetUserStateRequestCommon (false, "authKey");
        }

        [Test]
        public void TestBuildSetUserStateRequestSSL ()
        {
            TestBuildSetUserStateRequestCommon (true, "");
        }

        [Test]
        public void TestBuildSetUserStateRequestAuthSSL ()
        {
            TestBuildSetUserStateRequestCommon (true, "authKey");
        }

        [Test]
        public void TestBuildSetUserStateRequestCGnCH ()
        {
            TestBuildSetUserStateRequestCommon ( true, true, false, "");
        }

        [Test]
        public void TestBuildSetUserStateRequestAuthCGnCH ()
        {
            TestBuildSetUserStateRequestCommon ( true, true, false, "authKey");
        }

        [Test]
        public void TestBuildSetUserStateRequestSSLCGnCH ()
        {
            TestBuildSetUserStateRequestCommon ( true, true, true, "");
        }

        [Test]
        public void TestBuildSetUserStateRequestAuthSSLCGnCH ()
        {
            TestBuildSetUserStateRequestCommon ( true, true, true, "authKey");
        }

        [Test]
        public void TestBuildSetUserStateRequestCG ()
        {
            TestBuildSetUserStateRequestCommon ( true, false,  false, "");
        }

        [Test]
        public void TestBuildSetUserStateRequestAuthCG ()
        {
            TestBuildSetUserStateRequestCommon ( true, false,  false, "authKey");
        }

        [Test]
        public void TestBuildSetUserStateRequestSSLCG ()
        {
            TestBuildSetUserStateRequestCommon ( true, false,  true, "");
        }

        [Test]
        public void TestBuildSetUserStateRequestAuthSSLCG ()
        {
            TestBuildSetUserStateRequestCommon ( true, false,  true, "authKey");
        }

        public void TestBuildSetUserStateRequestCommon(bool ssl, string authKey){
            TestBuildSetUserStateRequestCommon(false, false, ssl, authKey);
        }

        public void TestBuildSetUserStateRequestCommon(bool testCg, bool testCh, bool ssl, string authKey){
            string channel = "user_state_channel";
            string userState = "{\"k\":\"v\"}";
            string uuid = "customuuid";
            string channelGroup = "user_state_channelGroup";
            string channelGroupStr = string.Format("&channel-group={0}",  Utility.EncodeUricomponent(channelGroup, PNOperationType.PNSetStateOperation, true, false));
            if(testCh && testCg){
                // test both
            } else if(testCg){
                channel = ",";
            } else {
                channelGroup = "";
                channelGroupStr = ""; 
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
            pnConfiguration.AuthKey = authKey;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pnConfiguration.AuthKey);
            }

            Uri uri = BuildRequests.BuildSetStateRequest (channel, channelGroup, userState, uuid, pnUnity);

            //https://ps.pndsn.com/v2/presence/sub_key/demo-36/channel/user_state_channel/uuid/customuuid/data?state={"k":"v"}&uuid=customuuid&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            //https://ps.pndsn.com/v2/presence/sub_key/demo-36/channel/user_state_channel/uuid/customuuid/data?state={"k":"v"}&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            string expected = string.Format ("http{0}://{1}/v2/presence/sub_key/{2}/channel/{3}/uuid/{4}/data?state={5}{9}&uuid={6}{7}&pnsdk={8}",
                ssl?"s":"", pnConfiguration.Origin, EditorCommon.SubscribeKey, channel, uuid, 
                Utility.EncodeUricomponent(userState, PNOperationType.PNSetStateOperation, false, false),
                uuid, authKeyString, 
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNSetStateOperation, false, false),
                channelGroupStr
            );
            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        [Test]
        public void TestBuildGetUserStateRequestAuthSSL ()
        {
            TestBuildGetUserStateRequestCommon (true, "authKey");
        }

        [Test]
        public void TestBuildGetUserStateRequestAuth ()
        {
            TestBuildGetUserStateRequestCommon (false, "authKey");
        }

        [Test]
        public void TestBuildGetUserStateRequest ()
        {
            TestBuildGetUserStateRequestCommon (false, "");
        }

        [Test]
        public void TestBuildGetUserStateRequestSSL ()
        {
            TestBuildGetUserStateRequestCommon (true, "");
        }

        [Test]
        public void TestBuildGetUserStateRequestAuthSSLCGnCH()
        {
            TestBuildGetUserStateRequestCommon (true, true, true, "authKey");
        }

        [Test]
        public void TestBuildGetUserStateRequestAuthCGnCH()
        {
            TestBuildGetUserStateRequestCommon (true, true, false, "authKey");
        }

        [Test]
        public void TestBuildGetUserStateRequestCGnCH()
        {
            TestBuildGetUserStateRequestCommon (true, true, false, "");
        }

        [Test]
        public void TestBuildGetUserStateRequestSSLCGnCH()
        {
            TestBuildGetUserStateRequestCommon (true, true, true, "");
        }

        [Test]
        public void TestBuildGetUserStateRequestAuthSSLCG()
        {
            TestBuildGetUserStateRequestCommon (true, false, true, "authKey");
        }

        [Test]
        public void TestBuildGetUserStateRequestAuthCG()
        {
            TestBuildGetUserStateRequestCommon (true, false, false, "authKey");
        }

        [Test]
        public void TestBuildGetUserStateRequestCG()
        {
            TestBuildGetUserStateRequestCommon (true, false, false, "");
        }

        [Test]
        public void TestBuildGetUserStateRequestSSLCG()
        {
            TestBuildGetUserStateRequestCommon (true, false, true, "");
        }

        public void TestBuildGetUserStateRequestCommon(bool ssl, string authKey){
            TestBuildGetUserStateRequestCommon(false, false, ssl, authKey);
        }

        public void TestBuildGetUserStateRequestCommon(bool testCg, bool testCh, bool ssl, string authKey){
            string channel = "user_state_channel";
            string userState = "{\"k\":\"v\"}";
            string uuid = "customuuid";
            string channelGroup = "user_state_channelGroup";
            string channelGroupStr = string.Format("&channel-group={0}",  Utility.EncodeUricomponent(channelGroup, PNOperationType.PNGetStateOperation, true, false));
            if(testCh && testCg){
                // test both
            } else if(testCg){
                channel = ",";
            } else {
                channelGroup = "";
                channelGroupStr = ""; 
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
            pnConfiguration.AuthKey = authKey;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            
            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pnConfiguration.AuthKey);
            }

            Uri uri = BuildRequests.BuildGetStateRequest (channel, channelGroup, uuid, pnUnity);

            //https://ps.pndsn.com/v2/presence/sub_key/demo-36/channel/user_state_channel/uuid/customuuid?uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            string expected = string.Format ("http{0}://{1}/v2/presence/sub_key/{2}/channel/{3}/uuid/{4}?uuid={6}{9}{7}&pnsdk={8}",
                ssl?"s":"", pnConfiguration.Origin, EditorCommon.SubscribeKey, channel, uuid, 
                Utility.EncodeUricomponent(userState, PNOperationType.PNGetStateOperation, false, false),
                uuid, authKeyString, 
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNGetStateOperation, false, false),
                channelGroupStr
            );
            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        #endif
    }
}

