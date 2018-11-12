using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class PresenceHeartbeatBuildRequestsTests
    {
        #if DEBUG 
        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsWithStateAuthSSL ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"test\":{\"key1\":\"value1\",\"key2\":\"value2\"},\"test2\":{\"key1\":\"value1\",\"key2\":\"value2\"}}";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, true, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsAuthSSL ()
        {
            string[] channels = { "test", "test2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, true, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestWithStateAuthSSL ()
        {
            string[] channels = { "test" };
            string userState = "{\"k\":\"v\"}";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, true, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestAuthSSL ()
        {
            string[] channels = { "test" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, true, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsWithStateAuth ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"test\":{\"key1\":\"value1\",\"key2\":\"value2\"},\"test2\":{\"key1\":\"value1\",\"key2\":\"value2\"}}";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, false, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsAuth ()
        {
            string[] channels = { "test", "test2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, false, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestWithStateAuth ()
        {
            string[] channels = { "test" };
            string userState = "{\"k\":\"v\"}";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, false, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestAuth ()
        {
            string[] channels = { "test" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, false, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsWithStateSSL ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"test\":{\"key1\":\"value1\",\"key2\":\"value2\"},\"test2\":{\"key1\":\"value1\",\"key2\":\"value2\"}}";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, true, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsSSL ()
        {
            string[] channels = { "test", "test2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, true, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestWithStateSSL ()
        {
            string[] channels = { "test" };
            string userState = "{\"k\":\"v\"}";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, true, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestSSL ()
        {
            string[] channels = { "test" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, true, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsWithState ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"test\":{\"key1\":\"value1\",\"key2\":\"value2\"},\"test2\":{\"key1\":\"value1\",\"key2\":\"value2\"}}";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, false, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannels ()
        {
            string[] channels = { "test", "test2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, false, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestWithState ()
        {
            string[] channels = { "test" };
            string userState = "{\"k\":\"v\"}";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, false, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequest ()
        {
            string[] channels = { "test" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channels, userState, false, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsWithStateAuthSSLCG ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"test\":{\"key1\":\"value1\",\"key2\":\"value2\"},\"test2\":{\"key1\":\"value1\",\"key2\":\"value2\"}}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, true, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsAuthSSLCG ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, true, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestWithStateAuthSSLCG ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, true, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestAuthSSLCG ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, true, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsWithStateAuthCG ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"test\":{\"key1\":\"value1\",\"key2\":\"value2\"},\"test2\":{\"key1\":\"value1\",\"key2\":\"value2\"}}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, false, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsAuthCG ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, false, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestWithStateAuthCG ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, false, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestAuthCG ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, false, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsWithStateSSLCG ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"test\":{\"key1\":\"value1\",\"key2\":\"value2\"},\"test2\":{\"key1\":\"value1\",\"key2\":\"value2\"}}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, true, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsSSLCG ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, true, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestWithStateSSLCG ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, true, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestSSLCG ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, true, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsWithStateCG ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"test\":{\"key1\":\"value1\",\"key2\":\"value2\"},\"test2\":{\"key1\":\"value1\",\"key2\":\"value2\"}}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, false, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsCG ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, false, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestWithStateCG ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, false, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestCG ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups,  null, userState, false, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsWithStateAuthSSLCGnCH ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"test\":{\"key1\":\"value1\",\"key2\":\"value2\"},\"test2\":{\"key1\":\"value1\",\"key2\":\"value2\"}}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, true, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsAuthSSLCGnCH ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, true, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestWithStateAuthSSLCGnCH ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, true, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestAuthSSLCGnCH ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, true, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsWithStateAuthCGnCH ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"test\":{\"key1\":\"value1\",\"key2\":\"value2\"},\"test2\":{\"key1\":\"value1\",\"key2\":\"value2\"}}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, false, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsAuthCGnCH ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, false, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestWithStateAuthCGnCH ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, false, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestAuthCGnCH ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, false, "authKey");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsWithStateSSLCGnCH ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"test\":{\"key1\":\"value1\",\"key2\":\"value2\"},\"test2\":{\"key1\":\"value1\",\"key2\":\"value2\"}}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, true, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsSSLCGnCH ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, true, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestWithStateSSLCGnCH ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, true, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestSSLCGnCH ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, true, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsWithStateCGnCH ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"test\":{\"key1\":\"value1\",\"key2\":\"value2\"},\"test2\":{\"key1\":\"value1\",\"key2\":\"value2\"}}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, false, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestMultipleChannelsCGnCH ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, false, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestWithStateCGnCH ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, false, "");
        }

        [Test]
        public void TestBuildPresenceHeartbeatRequestCGnCH ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            string userState = "";
            TestBuildPresenceHeartbeatRequestCommon (channelGroups, channels, userState, false, "");
        }



        public void TestBuildPresenceHeartbeatRequestCommon(string[] channels, string userState, 
            bool ssl, string authKey){
            TestBuildPresenceHeartbeatRequestCommon(null, channels, userState, ssl, authKey);
        }

        public void TestBuildPresenceHeartbeatRequestCommon(string[] channelGroups, string[] channels, string userState, 
            bool ssl, string authKey){

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
            pnConfiguration.AuthKey = authKey;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pnConfiguration.AuthKey);
            }

            string cgStr = "";
            string cg = ""; 
            if (channelGroups != null)
            {
                cg = string.Join (",", channelGroups);
                cgStr = string.Format("&channel-group={0}", Utility.EncodeUricomponent (cg, PNOperationType.PNSubscribeOperation, true, false));
            }        

            string chStr = ",";
            string ch = "";
            if (channels != null){
                ch = string.Join (",", channels);
                chStr = ch;
            }

            Uri uri = BuildRequests.BuildPresenceHeartbeatRequest (ch, cg, userState, pnUnity);

            //https://ps.pndsn.com/v2/presence/sub_key/demo-36/channel/user_state_channel/heartbeat?uuid=customuuid&state={"k":"v"}&auth=authKey&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0

            string expected = string.Format ("http{0}://{1}/v2/presence/sub_key/{2}/channel/{3}/heartbeat?uuid={4}{5}{6}{9}{7}&pnsdk={8}",
                ssl?"s":"", EditorCommon.Origin, 
                EditorCommon.SubscribeKey, 
                chStr, 
                uuid, 
                (userState=="")?"":"&state=", 
                Utility.EncodeUricomponent(userState, PNOperationType.PNSubscribeOperation, false, false),
                authKeyString, 
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNSubscribeOperation, false, true),
                cgStr
            );
            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }
        #endif
    }
}        