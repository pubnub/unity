using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Text;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class BuildSubscribeHearbeatLeaveUserStateRequestUnitTests
    {
        #if DEBUG
        [Test]
        public void TestBuildSubscribeRequest ()
        {
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, "", "", false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSL ()
        {
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, "", "", true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannel ()
        {
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, "", "", false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannel ()
        {
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, "", "", true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestTT ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLTT ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTT ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTT ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestState ()
        {
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, "", userState, false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLState ()
        {
            string[] channels = { "test" };
            string userState = "{\"k\":\"v\"}";
            TestBuildSubscribeRequestCommon (channels, "", userState, true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelState ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildSubscribeRequestCommon (channels, "", userState, false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelState ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildSubscribeRequestCommon (channels, "", userState, true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestTTState ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLTTState ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTTState ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, false, "");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTState ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, true, "");
        }

        [Test]
        public void TestBuildSubscribeRequestAuth ()
        {
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, "", "", false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLAuth ()
        {
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, "", "", true, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelAuth ()
        {
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, "", "", false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelAuth ()
        {
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, "", "", true, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestTTAuth ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLTTAuth ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", true, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTTAuth ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTAuth ()
        {
            long timetoken = 14498416434364941;
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, "", true, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestStateAuth ()
        {
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, "", userState, false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLStateAuth ()
        {
            string[] channels = { "test" };
            string userState = "{\"k\":\"v\"}";
            TestBuildSubscribeRequestCommon (channels, "", userState, true, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelStateAuth ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildSubscribeRequestCommon (channels, "", userState, false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelStateAuth ()
        {
            string[] channels = { "test", "test2" };
            string userState = "{\"k\":\"v\"}";
            TestBuildSubscribeRequestCommon (channels, "", userState, true, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestTTStateAuth ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLTTStateAuth ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, true, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestMultiChannelTTStateAuth ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, false, "authKey");
        }

        [Test]
        public void TestBuildSubscribeRequestSSLMultiChannelTTStateAuth ()
        {
            long timetoken = 14498416434364941;
            string userState = "{\"k\":\"v\"}";
            string[] channels = { "test", "test2" };
            TestBuildSubscribeRequestCommon (channels, timetoken, userState, true, "authKey");
        }


        public void TestBuildSubscribeRequestCommon(string[] channels, object timetoken, string userState,
            bool ssl, string authKey){
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

            Uri uri = BuildRequests.BuildMultiChannelSubscribeRequest (channels, timetoken, userState, uuid, ssl, 
                pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            //http://pubsub.pubnub.com/subscribe/demo-36/test/0/21221?uuid=customuuid&state={"k":"v"}&auth=authKey&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            string expected = string.Format ("http{0}://{1}/subscribe/{2}/{3}/0/{4}?uuid={5}{6}{7}{8}&pnsdk={9}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, string.Join (",", channels), timetoken.ToString(),
                uuid, (userState=="")?"":"&state=", userState, authKeyString, PubnubUnity.Version
            );
            string received = uri.ToString ();
            Common.LogAndCompare (expected, received);
        }

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

        public void TestBuildLeaveRequestCommon(string[] channels, bool ssl, string authKey){
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

            Uri uri = BuildRequests.BuildMultiChannelLeaveRequest (channels, uuid, ssl, 
                pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            //https://pubsub.pubnub.com/v2/presence/sub_key/demo-36/channel/test/leave?uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            string expected = string.Format ("http{0}://{1}/v2/presence/sub_key/{2}/channel/{3}/leave?uuid={4}{5}&pnsdk={6}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, string.Join (",", channels),
                uuid, authKeyString, PubnubUnity.Version
            );
            string received = uri.ToString ();
            Common.LogAndCompare (expected, received);
        }

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

        public void TestBuildPresenceHeartbeatRequestCommon(string[] channels, string userState, 
            bool ssl, string authKey){

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

            Uri uri = BuildRequests.BuildPresenceHeartbeatRequest (channels, userState, uuid, ssl, 
                pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            //https://pubsub.pubnub.com/v2/presence/sub_key/demo-36/channel/user_state_channel/heartbeat?uuid=customuuid&state={"k":"v"}&auth=authKey&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0

            string expected = string.Format ("http{0}://{1}/v2/presence/sub_key/{2}/channel/{3}/heartbeat?uuid={4}{5}{6}{7}&pnsdk={8}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, string.Join (",", channels), uuid, (userState=="")?"":"&state=", userState,
                authKeyString, PubnubUnity.Version
            );
            string received = uri.ToString ();
            Common.LogAndCompare (expected, received);
        }

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

        public void TestBuildSetUserStateRequestCommon(bool ssl, string authKey){
            string channel = "user_state_channel";
            string userState = "{\"k\":\"v\"}";
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

            Uri uri = BuildRequests.BuildSetUserStateRequest (channel, userState, uuid, ssl, 
                pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            //https://pubsub.pubnub.com/v2/presence/sub_key/demo-36/channel/user_state_channel/uuid/customuuid/data?state={"k":"v"}&uuid=customuuid&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            //https://pubsub.pubnub.com/v2/presence/sub_key/demo-36/channel/user_state_channel/uuid/customuuid/data?state={"k":"v"}&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            string expected = string.Format ("http{0}://{1}/v2/presence/sub_key/{2}/channel/{3}/uuid/{4}/data?state={5}&uuid={6}{7}&pnsdk={8}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, channel, uuid, userState,
                uuid, authKeyString, PubnubUnity.Version
            );
            string received = uri.ToString ();
            Common.LogAndCompare (expected, received);
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

        public void TestBuildGetUserStateRequestCommon(bool ssl, string authKey){
            string channel = "user_state_channel";
            string userState = "{\"k\":\"v\"}";
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

            Uri uri = BuildRequests.BuildGetUserStateRequest (channel, uuid, ssl, 
                pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            //https://pubsub.pubnub.com/v2/presence/sub_key/demo-36/channel/user_state_channel/uuid/customuuid?uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            string expected = string.Format ("http{0}://{1}/v2/presence/sub_key/{2}/channel/{3}/uuid/{4}?uuid={6}{7}&pnsdk={8}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, channel, uuid, userState,
                uuid, authKeyString, PubnubUnity.Version
            );
            string received = uri.ToString ();
            Common.LogAndCompare (expected, received);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestAPNS ()
        {
            TestBuildRegisterDevicePushRequestCommon (true, "", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestMPNS ()
        {
            TestBuildRegisterDevicePushRequestCommon (true, "", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestGCM ()
        {
            TestBuildRegisterDevicePushRequestCommon (true, "", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestWNS ()
        {
            TestBuildRegisterDevicePushRequestCommon (true, "", "pushToken", PushTypeService.WNS);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestAPNSAuth ()
        {
            TestBuildRegisterDevicePushRequestCommon (true, "authKey", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestMPNSAuth ()
        {
            TestBuildRegisterDevicePushRequestCommon (true, "authKey", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestGCMAuth ()
        {
            TestBuildRegisterDevicePushRequestCommon (true, "authKey", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestWNSAuth ()
        {
            TestBuildRegisterDevicePushRequestCommon (true, "authKey", "pushToken", PushTypeService.WNS);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestSSLAPNS ()
        {
            TestBuildRegisterDevicePushRequestCommon (false, "", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestSSLMPNS ()
        {
            TestBuildRegisterDevicePushRequestCommon (false, "", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestSSLGCM ()
        {
            TestBuildRegisterDevicePushRequestCommon (false, "", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestSSLWNS ()
        {
            TestBuildRegisterDevicePushRequestCommon (false, "", "pushToken", PushTypeService.WNS);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestSSLAPNSAuth ()
        {
            TestBuildRegisterDevicePushRequestCommon (false, "authKey", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestSSLMPNSAuth ()
        {
            TestBuildRegisterDevicePushRequestCommon (false, "authKey", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestSSLGCMAuth ()
        {
            TestBuildRegisterDevicePushRequestCommon (false, "authKey", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void TestBuildRegisterDevicePushRequestSSLWNSAuth ()
        {
            TestBuildRegisterDevicePushRequestCommon (false, "authKey", "pushToken", PushTypeService.WNS);
        }

        public void TestBuildRegisterDevicePushRequestCommon(bool ssl, string authKey, string pushToken, PushTypeService pushType){
            string channel = "push_channel";
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

            Uri uri = BuildRequests.BuildRegisterDevicePushRequest (channel, pushType, pushToken, uuid, ssl, 
                pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            //[1, "Modified Channels"]
            //https://pubsub.pubnub.com/v1/push/sub-key/demo-36/devices/pushToken?add=push_channel&type=apns&uuid=customuuid&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            string expected = string.Format ("http{0}://{1}/v1/push/sub-key/{2}/devices/{3}?add={4}&type={5}&uuid={6}{7}&pnsdk={8}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, pushToken, 
                Utility.EncodeUricomponent(channel, ResponseType.PushRegister, true, false), pushType.ToString().ToLower(),
                uuid, authKeyString, PubnubUnity.Version
            );
            string received = uri.ToString ();
            UnityEngine.Debug.Log("exp:"+expected);
            UnityEngine.Debug.Log(received);
            Common.LogAndCompare (expected, received);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestAPNS ()
        {
            TestBuildRemoveChannelPushRequestCommon (true, "", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestMPNS ()
        {
            TestBuildRemoveChannelPushRequestCommon (true, "", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestGCM ()
        {
            TestBuildRemoveChannelPushRequestCommon (true, "", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestWNS ()
        {
            TestBuildRemoveChannelPushRequestCommon (true, "", "pushToken", PushTypeService.WNS);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestAPNSAuth ()
        {
            TestBuildRemoveChannelPushRequestCommon (true, "authKey", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestMPNSAuth ()
        {
            TestBuildRemoveChannelPushRequestCommon (true, "authKey", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestGCMAuth ()
        {
            TestBuildRemoveChannelPushRequestCommon (true, "authKey", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestWNSAuth ()
        {
            TestBuildRemoveChannelPushRequestCommon (true, "authKey", "pushToken", PushTypeService.WNS);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestSSLAPNS ()
        {
            TestBuildRemoveChannelPushRequestCommon (false, "", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestSSLMPNS ()
        {
            TestBuildRemoveChannelPushRequestCommon (false, "", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestSSLGCM ()
        {
            TestBuildRemoveChannelPushRequestCommon (false, "", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestSSLWNS ()
        {
            TestBuildRemoveChannelPushRequestCommon (false, "", "pushToken", PushTypeService.WNS);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestSSLAPNSAuth ()
        {
            TestBuildRemoveChannelPushRequestCommon (false, "authKey", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestSSLMPNSAuth ()
        {
            TestBuildRemoveChannelPushRequestCommon (false, "authKey", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestSSLGCMAuth ()
        {
            TestBuildRemoveChannelPushRequestCommon (false, "authKey", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void TestBuildRemoveChannelPushRequestSSLWNSAuth ()
        {
            TestBuildRemoveChannelPushRequestCommon (false, "authKey", "pushToken", PushTypeService.WNS);
        }

        public void TestBuildRemoveChannelPushRequestCommon(bool ssl, string authKey, string pushToken, PushTypeService pushType){
            string channel = "push_channel";
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

            Uri uri = BuildRequests.BuildRemoveChannelPushRequest (channel, pushType, pushToken, uuid, ssl, 
                pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );
            //[1, "Modified Channels"]
            //http://pubsub.pubnub.com/v1/push/sub-key/demo-36/devices/pushToken?remove=push_channel&type=mpns&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            string expected = string.Format ("http{0}://{1}/v1/push/sub-key/{2}/devices/{3}?remove={4}&type={5}&uuid={6}{7}&pnsdk={8}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, pushToken, 
                Utility.EncodeUricomponent(channel, ResponseType.PushRemove, true, false), pushType.ToString().ToLower(),
                uuid, authKeyString, PubnubUnity.Version
            );
            string received = uri.ToString ();
            UnityEngine.Debug.Log("exp:"+expected);
            UnityEngine.Debug.Log(received);
            Common.LogAndCompare (expected, received);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestAPNS ()
        {
            TestBuildGetChannelsPushRequestCommon (true, "", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestMPNS ()
        {
            TestBuildGetChannelsPushRequestCommon (true, "", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestGCM ()
        {
            TestBuildGetChannelsPushRequestCommon (true, "", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestWNS ()
        {
            TestBuildGetChannelsPushRequestCommon (true, "", "pushToken", PushTypeService.WNS);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestAPNSAuth ()
        {
            TestBuildGetChannelsPushRequestCommon (true, "authKey", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestMPNSAuth ()
        {
            TestBuildGetChannelsPushRequestCommon (true, "authKey", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestGCMAuth ()
        {
            TestBuildGetChannelsPushRequestCommon (true, "authKey", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestWNSAuth ()
        {
            TestBuildGetChannelsPushRequestCommon (true, "authKey", "pushToken", PushTypeService.WNS);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestSSLAPNS ()
        {
            TestBuildGetChannelsPushRequestCommon (false, "", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestSSLMPNS ()
        {
            TestBuildGetChannelsPushRequestCommon (false, "", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestSSLGCM ()
        {
            TestBuildGetChannelsPushRequestCommon (false, "", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestSSLWNS ()
        {
            TestBuildGetChannelsPushRequestCommon (false, "", "pushToken", PushTypeService.WNS);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestSSLAPNSAuth ()
        {
            TestBuildGetChannelsPushRequestCommon (false, "authKey", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestSSLMPNSAuth ()
        {
            TestBuildGetChannelsPushRequestCommon (false, "authKey", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestSSLGCMAuth ()
        {
            TestBuildGetChannelsPushRequestCommon (false, "authKey", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void TestBuildGetChannelsPushRequestSSLWNSAuth ()
        {
            TestBuildGetChannelsPushRequestCommon (false, "authKey", "pushToken", PushTypeService.WNS);
        }

        public void TestBuildGetChannelsPushRequestCommon(bool ssl, string authKey, string pushToken, PushTypeService pushType){
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

            Uri uri = BuildRequests.BuildGetChannelsPushRequest (pushType, pushToken, uuid, ssl, 
                pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            //[1, "Modified Channels"]
            //["push_channel"]
            //https://pubsub.pubnub.com/v1/push/sub-key/demo-36/devices/pushToken?type=wns&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            //https://pubsub.pubnub.com/v1/push/sub-key/demo-36/devices/pushToken?type=mpns&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            string expected = string.Format ("http{0}://{1}/v1/push/sub-key/{2}/devices/{3}?type={4}&uuid={5}{6}&pnsdk={7}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, pushToken, 
                pushType.ToString().ToLower(),
                uuid, authKeyString, PubnubUnity.Version
            );
            string received = uri.ToString ();
            UnityEngine.Debug.Log("exp:"+expected);
            UnityEngine.Debug.Log(received);
            Common.LogAndCompare (expected, received);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestAPNS ()
        {
            BuildUnregisterDevicePushRequestCommon (true, "", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestMPNS ()
        {
            BuildUnregisterDevicePushRequestCommon (true, "", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestGCM ()
        {
            BuildUnregisterDevicePushRequestCommon (true, "", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestWNS ()
        {
            BuildUnregisterDevicePushRequestCommon (true, "", "pushToken", PushTypeService.WNS);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestAPNSAuth ()
        {
            BuildUnregisterDevicePushRequestCommon (true, "authKey", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestMPNSAuth ()
        {
            BuildUnregisterDevicePushRequestCommon (true, "authKey", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestGCMAuth ()
        {
            BuildUnregisterDevicePushRequestCommon (true, "authKey", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestWNSAuth ()
        {
            BuildUnregisterDevicePushRequestCommon (true, "authKey", "pushToken", PushTypeService.WNS);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestSSLAPNS ()
        {
            BuildUnregisterDevicePushRequestCommon (false, "", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestSSLMPNS ()
        {
            BuildUnregisterDevicePushRequestCommon (false, "", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestSSLGCM ()
        {
            BuildUnregisterDevicePushRequestCommon (false, "", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestSSLWNS ()
        {
            BuildUnregisterDevicePushRequestCommon (false, "", "pushToken", PushTypeService.WNS);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestSSLAPNSAuth ()
        {
            BuildUnregisterDevicePushRequestCommon (false, "authKey", "pushToken", PushTypeService.APNS);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestSSLMPNSAuth ()
        {
            BuildUnregisterDevicePushRequestCommon (false, "authKey", "pushToken", PushTypeService.MPNS);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestSSLGCMAuth ()
        {
            BuildUnregisterDevicePushRequestCommon (false, "authKey", "pushToken", PushTypeService.GCM);
        }

        [Test]
        public void BuildUnregisterDevicePushRequestSSLWNSAuth ()
        {
            BuildUnregisterDevicePushRequestCommon (false, "authKey", "pushToken", PushTypeService.WNS);
        }

        public void BuildUnregisterDevicePushRequestCommon(bool ssl, string authKey, string pushToken, PushTypeService pushType){
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

            Uri uri = BuildRequests.BuildUnregisterDevicePushRequest (pushType, pushToken, uuid, ssl, 
                pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );
            //[1, "Removed Device"]
            //https://pubsub.pubnub.com/v1/push/sub-key/demo-36/devices/pushToken/remove?type=wns&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0
            string expected = string.Format ("http{0}://{1}/v1/push/sub-key/{2}/devices/{3}/remove?type={4}&uuid={5}{6}&pnsdk={7}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, pushToken, 
                pushType.ToString().ToLower(),
                uuid, authKeyString, PubnubUnity.Version
            );
            string received = uri.ToString ();
            UnityEngine.Debug.Log("exp:"+expected);
            UnityEngine.Debug.Log(received);
            Common.LogAndCompare (expected, received);
        }
        #endif
    }
}
