using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PubNubAPI.Tests
{
    [TestFixture]
    public class MessageActionsTests
    {
        #if DEBUG   
        [Test]
        public void TestAddMessageActionsUserRequest ()
        {
            TestAddMessageActionsCommon (true, false);
        }

        [Test]
        public void TestAddMessageActionsUserRequestQP ()
        {
            TestAddMessageActionsCommon (true, true);
        }

        public void TestAddMessageActionsCommon(bool ssl, bool sendQueryParams){
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            string channelName = "message_actions_channel";
            string tt = "15742286266685611";

            Uri uri = BuildRequests.BuildAddMessageActionsRequest (channelName, tt, pnUnity, queryParams);

            //https://ps.pndsn.com/v1/message-actions/demo/channel/message_actions_channel/message/15742286266685611?uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.5.0
            string expected = string.Format ("http{0}://{1}/v1/message-actions/{2}/channel/message_actions_channel/message/{6}?uuid={3}&pnsdk={4}{5}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString,
                tt
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);

        }

        [Test]
        public void TestRemoveMessageActionsUserRequest ()
        {
            TestRemoveMessageActionsCommon (true, false);
        }

        [Test]
        public void TestRemoveMessageActionsUserRequestQP ()
        {
            TestRemoveMessageActionsCommon (true, true);
        }

        public void TestRemoveMessageActionsCommon(bool ssl, bool sendQueryParams){
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            string channelName = "message_actions_channel";
            string tt = "15742286266685611";
            string att = "15742286266685611";

            Uri uri = BuildRequests.BuildRemoveMessageActionsRequest (channelName, tt, att, pnUnity, queryParams);

            //https://ps.pndsn.com/v1/message-actions/demo/channel/message_actions_channel/message/15742286266685611/action/15742286266685611?uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.5.0 
            string expected = string.Format ("http{0}://{1}/v1/message-actions/{2}/channel/message_actions_channel/message/{6}/action/{7}?uuid={3}&pnsdk={4}{5}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString,
                tt,
                att
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);

        }

        [Test]
        public void TestGetMessageActionsUserRequest ()
        {
            TestGetMessageActionsCommon (true, false, 0, 0, 0);
        }

        [Test]
        public void TestGetMessageActionsUserRequestQP ()
        {
            TestGetMessageActionsCommon (true, true, 0, 0, 0);
        }

        [Test]
        public void TestGetMessageActionsUserRequestWithLimit ()
        {
            TestGetMessageActionsCommon (true, false, 0, 0, 10);
        }

        [Test]
        public void TestGetMessageActionsUserRequestQPWithLimit ()
        {
            TestGetMessageActionsCommon (true, true, 0, 0, 10);
        }

        [Test]
        public void TestGetMessageActionsUserRequestWithStart ()
        {
            TestGetMessageActionsCommon (true, false, 15742286266685611, 0, 0);
        }

        [Test]
        public void TestGetMessageActionsUserRequestQPWithStart ()
        {
            TestGetMessageActionsCommon (true, true, 15742286266685611, 0, 0);
        }

        [Test]
        public void TestGetMessageActionsUserRequestWithLimitStart ()
        {
            TestGetMessageActionsCommon (true, false, 15742286266685611, 0, 10);
        }

        [Test]
        public void TestGetMessageActionsUserRequestQPWithLimitStart ()
        {
            TestGetMessageActionsCommon (true, true, 15742286266685611, 0, 10);
        }

        [Test]
        public void TestGetMessageActionsUserRequestWithEnd ()
        {
            TestGetMessageActionsCommon (true, false, 0, 15742286266685611, 0);
        }

        [Test]
        public void TestGetMessageActionsUserRequestQPWithEnd ()
        {
            TestGetMessageActionsCommon (true, true, 0, 15742286266685611, 0);
        }

        [Test]
        public void TestGetMessageActionsUserRequestWithLimitEnd ()
        {
            TestGetMessageActionsCommon (true, false, 0, 15742286266685611, 10);
        }

        [Test]
        public void TestGetMessageActionsUserRequestQPWithLimitEnd ()
        {
            TestGetMessageActionsCommon (true, true, 0, 15742286266685611, 10);
        }

        [Test]
        public void TestGetMessageActionsUserRequestWithStartEnd ()
        {
            TestGetMessageActionsCommon (true, false, 15742286266685611, 15742286266685611, 0);
        }

        [Test]
        public void TestGetMessageActionsUserRequestQPWithStartEnd ()
        {
            TestGetMessageActionsCommon (true, true, 15742286266685611, 15742286266685611, 0);
        }

        [Test]
        public void TestGetMessageActionsUserRequestWithLimitStartEnd ()
        {
            TestGetMessageActionsCommon (true, false, 15742286266685611, 15742286266685611, 10);
        }

        [Test]
        public void TestGetMessageActionsUserRequestQPWithLimitStartEnd ()
        {
            TestGetMessageActionsCommon (true, true, 15742286266685611, 15742286266685611, 10);
        }

        public void TestGetMessageActionsCommon(bool ssl, bool sendQueryParams, long start, long end, int limit){
            string uuid = "customuuid";

            Dictionary<string,string> queryParams = new Dictionary<string, string>();
            string queryParamString = "";
            if(sendQueryParams){
                queryParams.Add("d","f");
                queryParamString="&d=f";
            } else {
                queryParams = null;
            }

            PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;
            pnConfiguration.UUID = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);
            string channelName = "message_actions_channel";
            string tt = "15742286266685611";
            string att = "15742286266685611";

            Uri uri = BuildRequests.BuildGetMessageActionsRequest (channelName, start, end, limit, pnUnity, queryParams);

            //https://ps.pndsn.com/v1/message-actions/demo/channel/message_actions_channel?uuid=customuuid&limit=10&start=15742286266685611&end=15742286266685611&pnsdk=PubNub-CSharp-UnityOSX%2F4.5.0 
            string expected = string.Format ("http{0}://{1}/v1/message-actions/{2}/channel/message_actions_channel?uuid={3}{6}{7}{8}&pnsdk={4}{5}",
                ssl?"s":"", 
                pnConfiguration.Origin, 
                EditorCommon.SubscribeKey, 
                uuid, 
                Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNPublishOperation, false, false),
                queryParamString,
                limit>0?"&limit="+limit.ToString():"",
                start>0?"&start="+start.ToString():"",
                end>0?"&end="+end.ToString():""
            );

            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);

        }        
        #endif
    }
}