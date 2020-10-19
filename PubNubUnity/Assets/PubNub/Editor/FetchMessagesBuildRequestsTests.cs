using System;
using PubNubAPI;
using NUnit.Framework;
using System.Text;
using System.Collections.Generic;

namespace PubNubAPI.Tests
{
    [TestFixture]
    public class FetchMessagesBuildRequestsTests
    {
        #if DEBUG   
        [Test]
        public void TestBuildFetchMessagesRequest ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestFetchMessagesBuildRequestCommon (false, false, false, "authKey", startTime, endTime, 25, false);
        }

        [Test]
        public void TestBuildFetchMessagesRequestWithMeta ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestFetchMessagesBuildRequestCommon (false, false, false, "authKey", startTime, endTime, 25, false, true, false);
        }

        [Test]
        public void TestBuildFetchMessagesRequestWithMetaAndMessageActions ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestFetchMessagesBuildRequestCommon (false, false, false, "authKey", startTime, endTime, 25, false, true, true);
        }

        [Test]
        public void TestBuildFetchMessagesRequestWithoutMetaAndMessageActions ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestFetchMessagesBuildRequestCommon (false, false, false, "authKey", startTime, endTime, 25, false, false, true);
        }

        public void TestFetchMessagesBuildRequestCommon(bool ssl, bool reverse, bool includeTimetoken, 
            string authKey, long startTime, long endTime, int count, bool sendQueryParams){
                TestFetchMessagesBuildRequestCommon (false, false, false, "authKey", startTime, endTime, 25, false, false, false);
        }
        public void TestFetchMessagesBuildRequestCommon(bool ssl, bool reverse, bool includeTimetoken, 
            string authKey, long startTime, long endTime, int count, bool sendQueryParams, bool withMeta, bool withMessageActions){
                TestFetchMessagesBuildRequestCommon (false, false, false, "authKey", startTime, endTime, 25, false, false, false, false, false);
        }
        
        public void TestFetchMessagesBuildRequestCommon(bool ssl, bool reverse, bool includeTimetoken, 
            string authKey, long startTime, long endTime, int count, bool sendQueryParams, bool withMeta, bool withMessageActions, bool withMessageType, bool withUUID){
            string[] channels = new[] {"history_channel", "history_channel2"};
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

            string startTimeString = "";
            string endTimeString = "";
            if (startTime != -1) {
                startTimeString = string.Format ("&start={0}", startTime.ToString ());
            }
            if (endTime != -1) {
                endTimeString = string.Format ("&end={0}", endTime.ToString ());
            }

            if(count == -1){
                count = 100;
            }

            Uri uri = BuildRequests.BuildFetchRequest (channels, startTime, endTime, (uint)count, reverse, 
                includeTimetoken, pnUnity, queryParams, withMeta, withMessageActions, withMessageType, withUUID
            );

            if (count == -1) {
                count = 100;
            }
            string expected = "";
            if(withMessageActions){
                //http://ps.pndsn.com/v3/history-with-actions/sub-key/demo/channel/history_channel,history_channel2?max=90&start=14498416434364941&end=14498416799269095&auth=authKey&uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.1.1 
                expected = string.Format ("http{0}://{1}/v3/history-with-actions/sub-key/{2}/channel/{3}?max={4}{5}{6}{7}{8}{13}{9}&uuid={10}&pnsdk={11}{12}",
                    ssl?"s":"", pnConfiguration.Origin, EditorCommon.SubscribeKey, string.Join(",", channels), count,
                    includeTimetoken?"&include_token=true":"", reverse?"&reverse=true":"",
                    startTimeString, endTimeString, authKeyString, uuid, 
                    Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNHistoryOperation, false, true),
                    queryParamString, withMeta?"&include_meta=true":""
                );
            } else {
                //http://ps.pndsn.com/v3/history/sub-key/demo/channel/history_channel,history_channel2?max=90&start=14498416434364941&end=14498416799269095&auth=authKey&uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX%2F4.1.1 
                expected = string.Format ("http{0}://{1}/v3/history/sub-key/{2}/channel/{3}?max={4}{5}{6}{7}{8}{13}{9}&uuid={10}&pnsdk={11}{12}",
                    ssl?"s":"", pnConfiguration.Origin, EditorCommon.SubscribeKey, string.Join(",", channels), count,
                    includeTimetoken?"&include_token=true":"", reverse?"&reverse=true":"",
                    startTimeString, endTimeString, authKeyString, uuid, 
                    Utility.EncodeUricomponent(pnUnity.Version, PNOperationType.PNHistoryOperation, false, true),
                    queryParamString, withMeta?"&include_meta=true":""
                );
            }
            string received = uri.OriginalString;
            EditorCommon.LogAndCompare (expected, received);
        }
        #endif
    }
}
