using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Text;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class BuildChannelGroupPAMCRDRequestUnitTests
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

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl
            );
            pubnub.AuthenticationKey = authKey;
            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pubnub.AuthenticationKey);
            }

            Uri uri = BuildRequests.BuildAddChannelsToChannelGroupRequest (channels, "", channelGroup,
                uuid, ssl, pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            string ch = string.Join(",", channels);

            //http://pubsub.pubnub.com/v1/channel-registration/sub-key/demo-36/channel-group/channelGroup?add=addChannel1,%20addChannel2&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX%2F3.7
            string expected = string.Format ("http{0}://{1}/v1/channel-registration/sub-key/{2}/channel-group/{3}?add={4}&uuid={5}{6}&pnsdk={7}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, channelGroup, 
                Utility.EncodeUricomponent(ch, ResponseType.ChannelGroupAdd, true, true),
                uuid, authKeyString, 
                Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.ChannelGroupAdd, false, true)

            );
            string received = uri.OriginalString;
            Common.LogAndCompare (expected, received);
        }

        //remove channels
        //remove cg
        /*public void TestBuildRemoveChannelsFromChannelGroupRequestCommon(string[] channels, bool ssl, string authKey){

            string channelGroup = "channelGroup";
            string uuid = "customuuid";

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl
            );
            pubnub.AuthenticationKey = authKey;
            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pubnub.AuthenticationKey);
            }

            Uri uri = BuildRequests.BuildRemoveChannelsFromChannelGroupRequest (channels, "", channelGroup,
                uuid, ssl, pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            string ch = string.Join(",", channels);

            //http://pubsub.pubnub.com/v1/channel-registration/sub-key/demo-36/channel-group/channelGroup?add=addChannel1,%20addChannel2&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX%2F3.7
            string expected = string.Format ("http{0}://{1}/v1/channel-registration/sub-key/{2}/channel-group/{3}?add={4}&uuid={5}{6}&pnsdk={7}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, channelGroup, 
                Utility.EncodeUricomponent(ch, ResponseType.ChannelGroupAdd, true, true),
                uuid, authKeyString, 
                Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.ChannelGroupAdd, false, true)

            );
            string received = uri.OriginalString;
            Common.LogAndCompare (expected, received);
        }

        //GetChannels
        //Get All CG
        public void TestBuildGetChannelsForChannelGroupRequestCommon(string[] channels, bool ssl, string authKey){

            string channelGroup = "channelGroup";
            string uuid = "customuuid";

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl
            );
            pubnub.AuthenticationKey = authKey;
            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pubnub.AuthenticationKey);
            }

            Uri uri = BuildRequests.BuildGetChannelsForChannelGroupRequest (channels, "", channelGroup,
                uuid, ssl, pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            string ch = string.Join(",", channels);

            //http://pubsub.pubnub.com/v1/channel-registration/sub-key/demo-36/channel-group/channelGroup?add=addChannel1,%20addChannel2&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX%2F3.7
            string expected = string.Format ("http{0}://{1}/v1/channel-registration/sub-key/{2}/channel-group/{3}?add={4}&uuid={5}{6}&pnsdk={7}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, channelGroup, 
                Utility.EncodeUricomponent(ch, ResponseType.ChannelGroupAdd, true, true),
                uuid, authKeyString, 
                Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.ChannelGroupAdd, false, true)

            );
            string received = uri.OriginalString;
            Common.LogAndCompare (expected, received);
        }

        /*public void TestBuildAddChannelsToChannelGroupRequestCommon(string[] channels, bool ssl, string authKey){

            string channelGroup = "channelGroup";
            string uuid = "customuuid";

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl
            );
            pubnub.AuthenticationKey = authKey;
            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pubnub.AuthenticationKey);
            }

            Uri uri = BuildRequests.BuildChannelGroupAuditAccessRequest (channels, "", channelGroup,
                uuid, ssl, pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            string ch = string.Join(",", channels);

            //http://pubsub.pubnub.com/v1/channel-registration/sub-key/demo-36/channel-group/channelGroup?add=addChannel1,%20addChannel2&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX%2F3.7
            string expected = string.Format ("http{0}://{1}/v1/channel-registration/sub-key/{2}/channel-group/{3}?add={4}&uuid={5}{6}&pnsdk={7}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, channelGroup, 
                Utility.EncodeUricomponent(ch, ResponseType.ChannelGroupAdd, true, true),
                uuid, authKeyString, 
                Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.ChannelGroupAdd, false, true)

            );
            string received = uri.OriginalString;
            Common.LogAndCompare (expected, received);
        }

        public void TestBuildAddChannelsToChannelGroupRequestCommon(string[] channels, bool ssl, string authKey){

            string channelGroup = "channelGroup";
            string uuid = "customuuid";

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl
            );
            pubnub.AuthenticationKey = authKey;
            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pubnub.AuthenticationKey);
            }

            Uri uri = BuildRequests.BuildChannelGroupGrantAccessRequest (channels, "", channelGroup,
                uuid, ssl, pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            string ch = string.Join(",", channels);

            //http://pubsub.pubnub.com/v1/channel-registration/sub-key/demo-36/channel-group/channelGroup?add=addChannel1,%20addChannel2&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX%2F3.7
            string expected = string.Format ("http{0}://{1}/v1/channel-registration/sub-key/{2}/channel-group/{3}?add={4}&uuid={5}{6}&pnsdk={7}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, channelGroup, 
                Utility.EncodeUricomponent(ch, ResponseType.ChannelGroupAdd, true, true),
                uuid, authKeyString, 
                Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.ChannelGroupAdd, false, true)

            );
            string received = uri.OriginalString;
            Common.LogAndCompare (expected, received);
        }*/

        #endif
    }
}

