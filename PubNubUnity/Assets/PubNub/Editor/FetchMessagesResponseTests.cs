using System;
using NUnit.Framework;
using System.Linq;
using UnityEngine;

namespace PubNubAPI.Tests
{
    public class FetchMessagesReq : FetchMessagesRequestBuilder
    {

        public FetchMessagesReq(PubNubUnity pn, Action<PNFetchMessagesResult, PNStatus> callback) : base(pn)
        {
            base.Callback = callback;
        }

        public void CreatePubNubResp(object deSerializedResult, RequestState requestState)
        {
            base.CreatePubNubResponse(deSerializedResult, requestState);
        }

    }

    [TestFixture]
    public class FetchMessagesResponseTests
    {
#if DEBUG
        [Test]
        public void FetchMessagesResponseWithMoreField ()
		{
            FetchMessageResponseTestCommon (true, true);

        }

        [Test]
        public void FetchMessagesResponseWithoutMoreField ()
        {
            FetchMessageResponseTestCommon(true, false);
        }

        public void FetchMessageResponseTestCommon(bool ssl, bool withMoreField)
		{
            string uuid = "customuuid";
            PNConfiguration pnConfiguration = new PNConfiguration();
            pnConfiguration.Origin = EditorCommon.Origin;
            pnConfiguration.SubscribeKey = EditorCommon.SubscribeKey;
            pnConfiguration.PublishKey = EditorCommon.PublishKey;
            pnConfiguration.Secure = ssl;
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY;
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval = 30;
            pnConfiguration.UserId = uuid;

            PubNubUnity pnUnity = new PubNubUnity(pnConfiguration, null, null);

            bool testResult = false;
            PNFetchMessagesResult fetchMessagesResult = new PNFetchMessagesResult();
            PNStatus pnStatus = new PNStatus();

            FetchMessagesReq fetchMessagesReq = new FetchMessagesReq(pnUnity, (result, status) => {
                if (status != null) {
                    pnStatus = status;
                    Debug.Log(pnStatus.Error);
                    Assert.True(pnStatus.Error.Equals(false));
                    Assert.True(pnStatus.StatusCode.Equals(0), pnStatus.StatusCode.ToString());
                }

                if (result != null) {
                    fetchMessagesResult = result;
                    Assert.AreEqual(3, fetchMessagesResult.Channels["c"].Count);
                    Assert.AreEqual("hello", fetchMessagesResult.Channels["c"].FirstOrDefault().Payload);
					if (withMoreField) {
                        Assert.AreEqual("/v3/history-with-actions/s/channel/c?start=15610547826970000&limit=98", fetchMessagesResult.More["url"]);
                        Assert.AreEqual(15610547826970000, fetchMessagesResult.More["start"]);
                        Assert.AreEqual(98, fetchMessagesResult.More["max"]);
                    }
                    testResult = true;
                }

            });
            string jsonString;
            if (withMoreField) {
                jsonString = "{\"status\":200,\"channels\":{\"c\":[{\"message\":\"hello\",\"timetoken\":\"16063629095708958\",\"actions\":{\"emoji\":{\"smily\":[{\"uuid\":\"test\",\"actionTimetoken\":\"16063644690020070\"}]}}},{\"message\":\"world\",\"timetoken\":\"16063629100769480\",\"actions\":{\"emoji\":{\"smily\":[{\"uuid\":\"test\",\"actionTimetoken\":\"16063644895175100\"}]}}},{\"message\":\"mymessage\",\"timetoken\":\"16063629102027064\",\"actions\":{\"emoji\":{\"smily\":[{\"uuid\":\"test\",\"actionTimetoken\":\"16063645043453400\"}]}}}]},\"error_message\":\"\",\"error\":false,\"more\":{\"url\":\"/v3/history-with-actions/s/channel/c?start=15610547826970000&limit=98\",\"start\":\"15610547826970000\",\"max\":98}}";
            }
            else {
                jsonString = "{\"status\":200,\"channels\":{\"c\":[{\"message\":\"hello\",\"timetoken\":\"16063629095708958\"},{\"message\":\"world\",\"timetoken\":\"16063629100769480\"},{\"message\":\"unity\",\"timetoken\":\"16063629102027064\"}]},\"error_message\":\"\",\"error\":false}";
			}
            object deSerializedResult = pnUnity.JsonLibrary.DeserializeToObject(jsonString);
            fetchMessagesReq.CreatePubNubResp(deSerializedResult, null);
            Assert.That(() => testResult, Is.True.After(100), "FetchMessages didn't return");

        }
#endif
    }

}
