using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class ChannelGroupBuildRequestsTests
    {
       
        #if DEBUG    
        [Test]
        public void BuildAddChannelsToChannelGroupRequest ()
        {
            string [] channels = {"addChannel1, addChannel2"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, false, "authKey");
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestSSL ()
        {
            string [] channels = {"addChannel1, addChannel2"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, true, "authKey");
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestNoAuth ()
        {
            string [] channels = {"addChannel1, addChannel2"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, false, "");
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestSSLNoAuth ()
        {
            string [] channels = {"addChannel1, addChannel2"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, true, "");
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestCH ()
        {
            string [] channels = {"addChannel1"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, false, "authKey");
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestSSLCH ()
        {
            string [] channels = {"addChannel1"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, true, "authKey");
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestNoAuthCH ()
        {
            string [] channels = {"addChannel1"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, false, "");
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestSSLNoAuthCh ()
        {
            string [] channels = {"addChannel1"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, true, "");
        }

        public void TestBuildAddChannelsToChannelGroupRequestCommon(string[] channels, bool ssl, string authKey){

            string channelGroup = "channelGroup";
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

            pnConfiguration.AuthKey = authKey;
            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pnConfiguration.AuthKey);
            }

            Uri uri = BuildRequests.BuildAddChannelsToChannelGroupRequest (channels, "", channelGroup,
                pnUnity
            );

            string ch = string.Join(",", channels);

            //http://ps.pndsn.com/v1/channel-registration/sub-key/demo-36/channel-group/channelGroup?add=addChannel1,%20addChannel2&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX%2F3.7
            string expected = string.Format ("http{0}://{1}/v1/channel-registration/sub-key/{2}/channel-group/{3}?add={4}&uuid={5}{6}&pnsdk={7}",
                ssl?"s":"", pnConfiguration.Origin, EditorCommon.SubscribeKey, channelGroup, 
                Utility.EncodeUricomponent(ch, PNOperationType.PNAddChannelsToGroupOperation, true, true),
                uuid, authKeyString, 
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNAddChannelsToGroupOperation, false, true)

            );
            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        [Test]
        public void BuildBuildRemoveAllChannelsFromChannelGroupRequestSSL ()
        {
            string [] channels = null;
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, true, "");
        }


        [Test]
        public void BuildBuildRemoveChannelsFromChannelGroupRequestSSL ()
        {
            string [] channels = {"addChannel1"};
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, true, "");
        }

        [Test]
        public void BuildBuildRemoveAllChannelsFromChannelGroupRequestSSLAuth ()
        {
            string [] channels = null;
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, true, "authKey");
        }

        [Test]
        public void BuildBuildRemoveChannelsFromChannelGroupRequestSSLAuthMulti ()
        {
            string [] channels = {"addChannel1", "addChannel2"};
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, true, "authKey");
        }

        [Test]
        public void BuildBuildRemoveChannelsFromChannelGroupRequestSSLAuth ()
        {
            string [] channels = {"addChannel1"};
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, true, "authKey");
        }

        [Test]
        public void BuildBuildRemoveAllChannelsFromChannelGroupRequest ()
        {
            string [] channels = null;
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, false, "");
        }


        [Test]
        public void BuildBuildRemoveChannelsFromChannelGroupRequest ()
        {
            string [] channels = {"addChannel1"};
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, false, "");
        }

        [Test]
        public void BuildBuildRemoveAllChannelsFromChannelGroupRequestAuth ()
        {
            string [] channels = null;
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, false, "authKey");
        }


        [Test]
        public void BuildBuildRemoveChannelsFromChannelGroupRequestAuth ()
        {
            string [] channels = {"addChannel1"};
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, false, "authKey");
        }

        //remove channels
        //remove cg
        public void TestBuildRemoveChannelsFromChannelGroupRequestCommon(string[] channels, bool ssl, string authKey){

            string channelGroup = "channelGroup";
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

            pnConfiguration.AuthKey = authKey;
            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pnConfiguration.AuthKey);
            }

            Uri uri = BuildRequests.BuildRemoveChannelsFromChannelGroupRequest (channels, "", channelGroup,
                pnUnity
            );

            string ch = "";
            string chStr = "";
            string chStr2 = "/remove";
            if (channels != null && channels.Length > 0) {
                ch = string.Join(",", channels);
                chStr = string.Format ("remove={0}&", Utility.EncodeUricomponent(ch, PNOperationType.PNRemoveChannelsFromGroupOperation, true, false));
                chStr2 = "";
            }

            //http://ps.pndsn.com/v1/channel-registration/sub-key/demo-36/channel-group/channelGroup?add=addChannel1,%20addChannel2&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX%2F3.7
            string expected = string.Format ("http{0}://{1}/v1/channel-registration/sub-key/{2}/channel-group/{3}{8}?{4}uuid={5}{6}&pnsdk={7}",
                ssl?"s":"", pnConfiguration.Origin, EditorCommon.SubscribeKey, channelGroup, 
                chStr, uuid, authKeyString, 
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNRemoveChannelsFromGroupOperation, false, true),
                chStr2

            );
            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequest ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (false, false, "");
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequestSSL ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (false, true, "");
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequestAuth ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (false, false, "authKey");
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequestSSLAuth ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (false, true, "authKey");
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequestAll ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (true, false, "");
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequestSSLAll ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (true, true, "");
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequestAuthAll ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (true, false, "authKey");
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequestSSLAuthAll ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (true, true, "authKey");
        }

        //GetChannels
        //Get All CG
        public void TestBuildGetChannelsForChannelGroupRequestCommon(bool allCg, 
            bool ssl, string authKey){

            string channelGroup = "channelGroup";
            string channelGroupStr ="channel-group/";
            if(allCg){
                channelGroup = "";
                channelGroupStr= "channel-group";
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
            pnConfiguration.AuthKey = authKey;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pnConfiguration.AuthKey);
            }

            Uri uri = BuildRequests.BuildGetChannelsForChannelGroupRequest ("", channelGroup, allCg,
                pnUnity
            );

            //http://ps.pndsn.com/v1/channel-registration/sub-key/demo-36/channel-group/channelGroup?add=addChannel1,%20addChannel2&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX%2F3.7
            string expected = string.Format ("http{0}://{1}/v1/channel-registration/sub-key/{2}/{8}{3}?uuid={5}{6}&pnsdk={7}",
                ssl?"s":"", pnConfiguration.Origin, EditorCommon.SubscribeKey, channelGroup, 
                "",
                uuid, authKeyString, 
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNChannelsForGroupOperation, false, true),
                channelGroupStr

            );
            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }

        #endif
    }
}

