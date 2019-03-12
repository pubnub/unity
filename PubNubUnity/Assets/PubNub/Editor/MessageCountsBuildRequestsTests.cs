using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class MessageCountsBuildRequestsTests
    {
        #if DEBUG
        [Test]
        public void TestBuildMessageCountsRequestMultiChannelAuthSSL ()
        {
            string[] channels = { "test", "test2" };
            long[] channelsTimetoken = { 15499825804610610, 15499925804610615 };
            string timetoken = "15499825804610609";
            TestMessageCountsBuildRequestCommon (channels, channelsTimetoken, timetoken, true, "authKey", false);
        }

        [Test]
        public void TestBuildMessageCountsRequestMultiChannelTTAuthSSL ()
        {
            string[] channels = { "test", "test2" };
            long[] channelsTimetoken = { };
            string timetoken = "15499825804610609";
            TestMessageCountsBuildRequestCommon (channels, channelsTimetoken, timetoken, true, "authKey", false);
        }

        [Test]
        public void TestBuildMessageCountsRequestMultiChannelCTAuthSSL ()
        {
            string[] channels = { "test", "test2" };
            long[] channelsTimetoken = { 15499825804610610, 15499925804610615 };
            string timetoken = "";
            TestMessageCountsBuildRequestCommon (channels, channelsTimetoken, timetoken, true, "authKey", false);
        }

        [Test]
        public void TestBuildMessageCountsRequestMultiChannelAuthSSLQP ()
        {
            string[] channels = { "test", "test2" };
            long[] channelsTimetoken = { 15499825804610610, 15499925804610615 };
            string timetoken = "15499825804610609";
            TestMessageCountsBuildRequestCommon (channels, channelsTimetoken, timetoken, true, "authKey", true);
        }

        [Test]
        public void TestBuildMessageCountsRequestMultiChannelTTAuthSSLQP ()
        {
            string[] channels = { "test", "test2" };
            long[] channelsTimetoken = { };
            string timetoken = "15499825804610609";
            TestMessageCountsBuildRequestCommon (channels, channelsTimetoken, timetoken, true, "authKey", true);
        }

        [Test]
        public void TestBuildMessageCountsRequestMultiChannelCTAuthSSLQP ()
        {
            string[] channels = { "test", "test2" };
            long[] channelsTimetoken = { 15499825804610610, 15499925804610615 };
            string timetoken = "";
            TestMessageCountsBuildRequestCommon (channels, channelsTimetoken, timetoken, true, "authKey", true);
        }

        public void TestMessageCountsBuildRequestCommon(string[] channels, long[] channelsTimetoken, string timetoken, bool ssl, string authKey, bool sendQueryParams)
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
                channelsTimetokenStr = String.Join(",", channelsTimetoken.Select(p=>p.ToString()).ToArray());
            }

            Uri uri = BuildRequests.BuildMessageCountsRequest (channels, channelsTimetokenStr, timetoken, pnUnity, queryParams);

            //https://ps.pndsn.com/v3/history/sub-key/demo/message-counts/test,test2?timetoken=15499825804610609&channelsTimetoken=15499825804610610,15499925804610615&auth=authKey&uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.1.1 
            string expected = string.Format ("http{0}://{1}/v3/history/sub-key/{2}/message-counts/{3}?timetoken={4}&channelsTimetoken={5}{6}&uuid={7}&pnsdk={8}{9}",
                ssl?"s":"", pnConfiguration.Origin, EditorCommon.SubscribeKey, chStr, timetoken,
                channelsTimetokenStr, authKeyString,
                uuid, Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNMessageCountsOperation, false, true),
                queryParamString
            );
            string received = uri.OriginalString;
            received.Trim().Equals(expected.Trim());
            EditorCommon.LogAndCompare (expected, received);   
        }
        #endif
    }
}
