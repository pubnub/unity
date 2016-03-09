#define REDUCE_PUBNUB_COROUTINES
using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using UnityEngine;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class CoroutineClassUnitTests
    {
        /*.   CheckElapsedTime
            4.  CheckIfRequestIsRunning
            5.  CheckPauseTime
            6.  StopTimeouts*/

        #if DEBUG && REDUCE_PUBNUB_COROUTINES
        [Test]
        public void TestSetGetCoroutineParamsSub(){
            string[] multiChannel = {"testChannel"};

            CurrentRequestType crt = CurrentRequestType.Subscribe;
            string expectedMessage = "[[]";
            string expectedChannels = string.Join (",", multiChannel);
            ResponseType respType =  ResponseType.Subscribe;

            Pubnub pubnub = new Pubnub (
                CommonIntergrationTests.PublishKey,
                CommonIntergrationTests.SubscribeKey,
                "",
                "",
                true
            );

            //http://pubsub.pubnub.com/subscribe/{0}/{1}/0/{2}?uuid={3}&pnsdk={4}
            string url = string.Format ("http://pubsub.pubnub.com/subscribe/{0}/{1}/0/{2}?uuid={3}&pnsdk={4}", CommonIntergrationTests.SubscribeKey, 
                expectedChannels, 0, pubnub.SessionUUID, pubnub.Version
            );

            SetGetCoroutineParams<string> (multiChannel, url, crt, respType, pubnub.SubscribeTimeout, false, false);

        }

        [Test]
        public void TestSetGetCoroutineParamsNonSub(){
            string[] multiChannel = {"testChannel"};

            CurrentRequestType crt = CurrentRequestType.NonSubscribe;
            string expectedMessage = "[[]";
            string expectedChannels = string.Join (",", multiChannel);
            ResponseType respType =  ResponseType.Time;

            Pubnub pubnub = new Pubnub (
                CommonIntergrationTests.PublishKey,
                CommonIntergrationTests.SubscribeKey,
                "",
                "",
                true
            );

            string url = "https://pubsub.pubnub.com/time/0";

            SetGetCoroutineParams<string> (multiChannel, url, crt, respType, pubnub.NonSubscribeTimeout, false, false);

        }

        [Test]
        public void TestSetGetCoroutineParamsHB(){
            string[] multiChannel = {"testChannel"};

            CurrentRequestType crt = CurrentRequestType.Heartbeat;
            string expectedMessage = "[[]";
            string expectedChannels = string.Join (",", multiChannel);
            ResponseType respType =  ResponseType.Heartbeat;

            Pubnub pubnub = new Pubnub (
                CommonIntergrationTests.PublishKey,
                CommonIntergrationTests.SubscribeKey,
                "",
                "",
                true
            );

            string url = "https://pubsub.pubnub.com/time/0";

            SetGetCoroutineParams<string> (multiChannel, url, crt, respType, pubnub.NonSubscribeTimeout, false, false);

        }

        [Test]
        public void TestSetGetCoroutineParamsPHB(){
            string[] multiChannel = {"testChannel"};

            CurrentRequestType crt = CurrentRequestType.PresenceHeartbeat;
            string expectedMessage = "[[]";
            string expectedChannels = string.Join (",", multiChannel);
            ResponseType respType =  ResponseType.PresenceHeartbeat;

            Pubnub pubnub = new Pubnub (
                CommonIntergrationTests.PublishKey,
                CommonIntergrationTests.SubscribeKey,
                "",
                "",
                true
            );

            string url = string.Format ("http://pubsub.pubnub.com/v2/presence/sub_key/{0}/channel/{1}/heartbeat?uuid={2}&heartbeat=62&pnsdk={3}", CommonIntergrationTests.SubscribeKey, 
                expectedChannels, pubnub.SessionUUID, pubnub.Version
            );

            SetGetCoroutineParams<string> (multiChannel, url, crt, respType, pubnub.NonSubscribeTimeout, false, false);

        }

        public void SetGetCoroutineParams<T>(string[] multiChannel, string url, CurrentRequestType crt, ResponseType respType,
            int timeout, bool resumeOnReconnect, bool isTimeout
        ){
            
            RequestState<T> pubnubRequestState = BuildRequests.BuildRequestState<T> (multiChannel, respType, 
                resumeOnReconnect, null, 
                null, null, 0, isTimeout, 0, typeof(T));

            CoroutineParams<T> cp = new CoroutineParams<T> (url, timeout, 0, crt, typeof(T), pubnubRequestState);

            GameObject go = new GameObject ();
            CoroutineClass cc = go.AddComponent<CoroutineClass> ();

            cc.SetCoroutineParams<T>(crt, cp);

            CoroutineParams<T> cp2 = cc.GetCoroutineParams<T>(crt) as CoroutineParams<T>;
            Assert.True (cp.crt.Equals (cp2.crt));
            Assert.True (cp.pause.Equals (cp2.pause));
            Assert.True (cp.timeout.Equals (cp2.timeout));
            Assert.True (cp.url.Equals (cp2.url));
        }

        [Test]
        public void TestCheckElapsedTimeSub(){
            
        }

        void CheckElapsedTime<T>(CurrentRequestType crt){
            GameObject go = new GameObject ();
            CoroutineClass cc = go.AddComponent<CoroutineClass> ();

            if(crt.Equals(CurrentRequestType.Subscribe)){
                cc.SubCoroutineComplete += CcCoroutineComplete<T>;
            } else if (crt.Equals(CurrentRequestType.NonSubscribe)){
                cc.NonSubCoroutineComplete += CcCoroutineComplete<T>;
            } else if (crt.Equals(CurrentRequestType.PresenceHeartbeat)){
                cc.PresenceHeartbeatCoroutineComplete += CcCoroutineComplete<T>;
            } else if (crt.Equals(CurrentRequestType.Heartbeat)){
                cc.HeartbeatCoroutineComplete += CcCoroutineComplete<T>;
            }

        }

        void CcCoroutineComplete<T> (object sender, EventArgs ea)
        {
            CustomEventArgs<T> cea = ea as CustomEventArgs<T>;
            if (cea != null && cea.PubnubRequestState != null) {
                UnityEngine.Debug.Log ("cea.PubnubRequestState.Channels:" + string.Join (",", cea.PubnubRequestState.Channels));
                UnityEngine.Debug.Log ("cea.IsError:" + cea.IsError);
                UnityEngine.Debug.Log ("cea.IsTimeout:" + cea.IsTimeout);
                UnityEngine.Debug.Log ("cea.CurrRequestType:" + cea.CurrRequestType);
                UnityEngine.Debug.Log ("cea.PubnubRequestState.RespType:" + cea.PubnubRequestState.RespType);
                UnityEngine.Debug.Log ("cea.Message:" + cea.Message);
            }
        }
        //#endif
        #endif
    }
}

