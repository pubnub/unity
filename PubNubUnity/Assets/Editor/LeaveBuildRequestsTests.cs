using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class LeaveBuildRequestsTests
    {
        //TODO: Add heartbeat
        #if DEBUG
        [Test]
        public void TestBuildLeaveRequestMultiChannelAuthSSL ()
        {
            string[] channels = { "test", "test2" };
            TestBuildLeaveRequestCommon (channels, true, "authKey");
        }

        [Test]
        public void TestBuildLeaveRequestAuthSSL ()
        {
            string[] channels = { "test" };
            TestBuildLeaveRequestCommon (channels, true, "authKey");
        }

        [Test]
        public void TestBuildLeaveRequestMultiChannelAuth ()
        {
            string[] channels = { "test", "test2" };
            TestBuildLeaveRequestCommon (channels, false, "authKey");
        }

        [Test]
        public void TestBuildLeaveRequestAuth ()
        {
            string[] channels = { "test" };
            TestBuildLeaveRequestCommon (channels, false, "authKey");
        }

        [Test]
        public void TestBuildLeaveRequestMultiChannelSSL ()
        {
            string[] channels = { "test", "test2" };
            TestBuildLeaveRequestCommon (channels, true, "");
        }

        [Test]
        public void TestBuildLeaveRequestSSL ()
        {
            string[] channels = { "test" };
            TestBuildLeaveRequestCommon (channels, true, "");
        }

        [Test]
        public void TestBuildLeaveRequestMultiChannel ()
        {
            string[] channels = { "test", "test2" };
            TestBuildLeaveRequestCommon (channels, false, "");
        }

        [Test]
        public void TestBuildLeaveRequest ()
        {
            string[] channels = { "test" };
            TestBuildLeaveRequestCommon (channels, false, "");
        }

        [Test]
        public void TestBuildLeaveRequestMultiChannelAuthSSLCG ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            TestBuildLeaveRequestCommon (channels, channelGroups, true, "authKey");
        }

        [Test]
        public void TestBuildLeaveRequestAuthSSLCG ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            TestBuildLeaveRequestCommon (channels, channelGroups, true, "authKey");
        }

        [Test]
        public void TestBuildLeaveRequestMultiChannelAuthCG ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            TestBuildLeaveRequestCommon (channels, channelGroups, false, "authKey");
        }

        [Test]
        public void TestBuildLeaveRequestAuthCG ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            TestBuildLeaveRequestCommon (channels, channelGroups, false, "authKey");
        }

        [Test]
        public void TestBuildLeaveRequestMultiChannelSSLCG ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg" };
            TestBuildLeaveRequestCommon (channels, channelGroups, true, "");
        }

        [Test]
        public void TestBuildLeaveRequestSSLCG ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            TestBuildLeaveRequestCommon (channels, channelGroups, true, "");
        }

        [Test]
        public void TestBuildLeaveRequestMultiChannelCG ()
        {
            string[] channels = { "test", "test2" };
            string[] channelGroups = { "cg", "cg2" };
            TestBuildLeaveRequestCommon (channels, channelGroups, false, "");
        }

        [Test]
        public void TestBuildLeaveRequestCG ()
        {
            string[] channels = { "test" };
            string[] channelGroups = { "cg", "cg2" };
            TestBuildLeaveRequestCommon (channels, channelGroups, false, "");
        }


        [Test]
        public void TestBuildLeaveRequestMultiChannelAuthSSLCGOnly ()
        {
            string[] channelGroups = { "test", "test2" };
            TestBuildLeaveRequestCommon (null,  channelGroups, true, "authKey");
        }

        [Test]
        public void TestBuildLeaveRequestAuthSSLCGOnly ()
        {
            string[] channelGroups = { "test" };
            TestBuildLeaveRequestCommon (null,  channelGroups, true, "authKey");
        }

        [Test]
        public void TestBuildLeaveRequestMultiChannelAuthCGOnly ()
        {
            string[] channelGroups = { "test", "test2" };
            TestBuildLeaveRequestCommon (null,  channelGroups, false, "authKey");
        }

        [Test]
        public void TestBuildLeaveRequestAuthCGOnly ()
        {
            string[] channelGroups = { "test" };
            TestBuildLeaveRequestCommon (null,  channelGroups, false, "authKey");
        }

        [Test]
        public void TestBuildLeaveRequestMultiChannelSSLCGOnly ()
        {
            string[] channelGroups = { "test", "test2" };
            TestBuildLeaveRequestCommon (null,  channelGroups, true, "");
        }

        [Test]
        public void TestBuildLeaveRequestSSLCGOnly ()
        {
            string[] channelGroups = { "test" };
            TestBuildLeaveRequestCommon (null,  channelGroups, true, "");
        }

        [Test]
        public void TestBuildLeaveRequestMultiChannelCGOnly ()
        {
            string[] channelGroups = { "test", "test2" };
            TestBuildLeaveRequestCommon (null,  channelGroups, false, "");
        }

        [Test]
        public void TestBuildLeaveRequestCGOnly ()
        {
            string[] channelGroups = { "test" };
            TestBuildLeaveRequestCommon (null,  channelGroups, false, "");
        }

        public void TestBuildLeaveRequestCommon(string[] channels, bool ssl, string authKey){
            TestBuildLeaveRequestCommon(channels, null, ssl, authKey);
        }

        public void TestBuildLeaveRequestCommon(string[] channels, string[] channelGroups, bool ssl, string authKey)
        {

            string uuid = "customuuid";
            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.Secure = ssl;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 

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
                cgStr = string.Format("&channel-group={0}", Utility.EncodeUricomponent (cg, PNOperationType.PNLeaveOperation, true, false));
            }        

            string chStr = ",";
            string ch = "";
            if (channels != null){
                ch = string.Join (",", channels);
                chStr = ch;
            }

            Uri uri = BuildRequests.BuildLeaveRequest (ch, cg, pnUnity);

            //https://ps.pndsn.com/v2/presence/sub_key/demo-36/channel/test/leave?uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            string expected = string.Format ("http{0}://{1}/v2/presence/sub_key/{2}/channel/{3}/leave?uuid={4}{7}{5}&pnsdk={6}",
                ssl?"s":"", pnConfiguration.Origin, EditorCommon.SubscribeKey, chStr,
                uuid, authKeyString, Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNLeaveOperation, false, true),
                cgStr
            );
            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }
        #endif
    }
}        