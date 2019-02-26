using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;
using System.Collections.Generic;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class MessageCountsBuildRequestsTests
    {
        #if DEBUG
        [Test]
        public void TestBuildLeaveRequestMultiChannelAuthSSL ()
        {
            string[] channels = { "test", "test2" };
            string[] channelsTimetoken = { "15499825804610610", "15499925804610615" };
            string timetoken = "15499825804610609";
            TestMessageCountsBuildRequestCommon (channels, channelsTimetoken, timetoken, true, "authKey", false);
        }

        [Test]
        public void TestBuildLeaveRequestMultiChannelTTAuthSSL ()
        {
            string[] channels = { "test", "test2" };
            string[] channelsTimetoken = { };
            string timetoken = "15499825804610609";
            TestMessageCountsBuildRequestCommon (channels, channelsTimetoken, timetoken, true, "authKey", false);
        }

        [Test]
        public void TestBuildLeaveRequestMultiChannelCTAuthSSL ()
        {
            string[] channels = { "test", "test2" };
            string[] channelsTimetoken = { "15499825804610610", "15499925804610615" };
            string timetoken = "";
            TestMessageCountsBuildRequestCommon (channels, channelsTimetoken, timetoken, true, "authKey", false);
        }

        [Test]
        public void TestBuildLeaveRequestMultiChannelAuthSSLQP ()
        {
            string[] channels = { "test", "test2" };
            string[] channelsTimetoken = { "15499825804610610", "15499925804610615" };
            string timetoken = "15499825804610609";
            TestMessageCountsBuildRequestCommon (channels, channelsTimetoken, timetoken, true, "authKey", true);
        }

        [Test]
        public void TestBuildLeaveRequestMultiChannelTTAuthSSLQP ()
        {
            string[] channels = { "test", "test2" };
            string[] channelsTimetoken = { };
            string timetoken = "15499825804610609";
            TestMessageCountsBuildRequestCommon (channels, channelsTimetoken, timetoken, true, "authKey", true);
        }

        [Test]
        public void TestBuildLeaveRequestMultiChannelCTAuthSSLQP ()
        {
            string[] channels = { "test", "test2" };
            string[] channelsTimetoken = { "15499825804610610", "15499925804610615" };
            string timetoken = "";
            TestMessageCountsBuildRequestCommon (channels, channelsTimetoken, timetoken, true, "authKey", true);
        }

        public void TestMessageCountsBuildRequestCommon(string[] channels, string[] channelsTimetoken, string timetoken, bool ssl, string authKey, bool sendQueryParams)
        {
            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }
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

            string channelsTimetokenStr = "" , chStr = "";
            if(channels != null){
                chStr = string.Join(",", channels);
            }

            if(channelsTimetoken != null){
                channelsTimetokenStr = string.Join(",", channelsTimetoken);
            }

            Uri uri = BuildRequests.BuildMessageCountsRequest (channels, channelsTimetoken, timetoken, pnUnity, queryParams);

            //https://ps.pndsn.com/v3/history/sub-key/demo/message-counts/test,test2?timetoken=15499825804610609&channelsTimetoken=15499825804610610,15499925804610615&auth=authKey&uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.1.1 
            string expected = string.Format ("http{0}://{1}/v3/history/sub-key/{2}/message-counts/{3}?timetoken={4}&channelsTimetoken={5}{6}&uuid={7}&pnsdk={8}{9}",
                ssl?"s":"", pnConfiguration.Origin, EditorCommon.SubscribeKey, chStr, timetoken,
                channelsTimetokenStr, authKeyString,
                uuid, Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNLeaveOperation, false, true),
                queryParamString
            );
            string received = uri.OriginalString;
            received.Trim().Equals(expected.Trim());
            EditorCommon.LogAndCompare (expected, received);   
        }
        #endif
    }
}
