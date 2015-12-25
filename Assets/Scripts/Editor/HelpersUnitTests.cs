using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace PubNubMessaging.Tests
{
	[TestFixture]
	public class HelpersUnitTests
	{
		#if DEBUG
		int ExceptionCode =0;
		bool result1 = false;
		bool readCallback = false;

		List<object> resultList = new List<object>();
		string ExpectedConnectResponse = "";
		string ExpectedRegularResponse = "";

		[Test]
		public void TestCheckChannelsInMultiChannelSubscribeRequestFalse2 (){

			string[] multiChannel = {"testChannel", "testChannel2"};

			TestCheckChannelsInMultiChannelSubscribeRequestCommon (multiChannel, "testChannel", false);
		}

		[Test]
		public void TestCheckChannelsInMultiChannelSubscribeRequestTrue (){

			string[] multiChannel = {};

			TestCheckChannelsInMultiChannelSubscribeRequestCommon (multiChannel, "testChannel", true);
		}

		[Test]
		public void TestCheckChannelsInMultiChannelSubscribeRequestFalse (){ 

			string[] multiChannel = {"testChannel", "testChannel2"}; 
			TestCheckChannelsInMultiChannelSubscribeRequestCommon (multiChannel, "testChannel3", false);
		}

		void TestCheckChannelsInMultiChannelSubscribeRequestCommon(string[] multiChannel, string channel, bool isTrue){
			SafeDictionary<string, long> multiChannelSubscribe = new SafeDictionary<string, long> ();
			foreach (string currentChannel in multiChannel) {
				multiChannelSubscribe.AddOrUpdate (currentChannel, 14498416434364941, (key, oldValue) => Convert.ToInt64 (14498416434364941));
			}

			SafeDictionary<string, PubnubWebRequest> channelRequest = new SafeDictionary<string, PubnubWebRequest> ();
			PubnubWebRequest pnwr = new PubnubWebRequest(new UnityEngine.WWW("pubsub.pubnub.com"));
			foreach (string currentChannel in multiChannel) {
				channelRequest.AddOrUpdate (currentChannel, pnwr, (key, oldState) => pnwr);
			}

			if (isTrue) {
				Assert.IsTrue (Helpers.CheckChannelsInMultiChannelSubscribeRequest (channel, 
					multiChannelSubscribe, channelRequest));
			} else {
				Assert.IsFalse(Helpers.CheckChannelsInMultiChannelSubscribeRequest(channel, 
					multiChannelSubscribe, channelRequest));
			}
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresRemove (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, true, ResponseType.Presence);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresAlreadySubscribed (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, true, ResponseType.Presence);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNew (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel3";
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, false, true, ResponseType.Presence);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresRemoveObj (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, true, ResponseType.Presence);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresAlreadySubscribedObj (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, true, ResponseType.Presence);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNewObj (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel3";
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, false, true, ResponseType.Presence);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetRemove (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, false, ResponseType.Presence);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetAlreadySubscribed (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, false, ResponseType.Presence);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetNew (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel3";
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, false, false, ResponseType.Presence);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetRemoveObj (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, false, ResponseType.Presence);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetAlreadySubscribedObj (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, false, ResponseType.Presence);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetNewObj (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel3";
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, false, false, ResponseType.Presence);
		}
			
		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsRemove (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, true, ResponseType.Subscribe);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsAlreadySubscribed (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, true, ResponseType.Subscribe);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNew (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel3";
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, false, true, ResponseType.Subscribe);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsRemoveObj (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, true, ResponseType.Subscribe);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsAlreadySubscribedObj (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, true, ResponseType.Subscribe);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNewObj (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel3";
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, false, true, ResponseType.Subscribe);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetRemove (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, false, ResponseType.Subscribe);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetAlreadySubscribed (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, false, ResponseType.Subscribe);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetNew (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel3";
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, false, false, ResponseType.Subscribe);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetRemoveObj (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, false, ResponseType.Subscribe);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetAlreadySubscribedObj (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel";
			ExceptionCode = 112;
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, false, ResponseType.Subscribe);
		}

		[Test]
		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetNewObj (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string channel = "testChannel3";
			TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, false, false, ResponseType.Subscribe);
		}

		public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<T> (string channel,
			string[] multiChannel, bool fireCallback, bool networkConnection, ResponseType responseType
		){ 

			//Remove duplicate channels
			//Already subscribed
			//New channel
			readCallback = fireCallback;
			List<string> validChannels = new List<string> ();
			validChannels.Add (channel);

			PubnubErrorFilter.Level errorLevel = PubnubErrorFilter.Level.Info;

			SafeDictionary<string, long> multiChannelSubscribe = new SafeDictionary<string, long> ();
			foreach (string currentChannel in multiChannel) {
				multiChannelSubscribe.AddOrUpdate (currentChannel, 14498416434364941, (key, oldValue) => Convert.ToInt64 (14498416434364941));
			}
			bool result1 = false;
			Action<PubnubClientError> errorcb = ErrorCallbackCommonExceptionHandler;
			if (!readCallback) {
				errorcb = null;
			}
			List<string> validChannels2 = Helpers.RemoveDuplicateChannelsAndCheckForAlreadySubscribedChannels<T> (responseType,
				channel, errorcb, multiChannel, validChannels, networkConnection, 
				multiChannelSubscribe, errorLevel
			);
			if (!readCallback) {
				UnityEngine.Debug.Log ("not fireCallback");
				Assert.IsTrue (validChannels2.Contains (channel));
			}
		}

		void ErrorCallbackCommonExceptionHandler (PubnubClientError result)
		{
			UnityEngine.Debug.Log ("in fireCallback");
			UnityEngine.Debug.Log (result.StatusCode);
			UnityEngine.Debug.Log (ExceptionCode);

			if (readCallback) {
				Assert.IsTrue (result.StatusCode.Equals (ExceptionCode));
			}
		}

		[Test]
		public void TestGetValidChannels (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string[] rawChannels = {"testChannel", "testChannel2"}; 

			TestGetValidChannelsCommon<string> (rawChannels, multiChannel, false, ResponseType.Subscribe);
		}

		[Test]
		public void TestGetValidChannelsEmpty (){ 
			string[] multiChannel = {"testChannel3", "testChannel"}; 
			string[] rawChannels = {" "}; 
			ExceptionCode = 117;
			TestGetValidChannelsCommon<string> (rawChannels, multiChannel, true, ResponseType.Subscribe);
		}

		[Test]
		public void TestGetValidChannelsNotSub (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string[] rawChannels = {"testChannel3", "testChannel", "testChannel2"}; 
			ExceptionCode = 118;
			TestGetValidChannelsCommon<string> (rawChannels, multiChannel, true, ResponseType.Subscribe);
		}

		[Test]
		public void TestGetValidChannelsNotSubUnsub (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string[] rawChannels = {"testChannel3", "testChannel", "testChannel2"}; 
			ExceptionCode = 118;
			TestGetValidChannelsCommon<string> (rawChannels, multiChannel, true, ResponseType.Unsubscribe);
		}

		[Test]
		public void TestGetValidChannelsUnsub (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string[] rawChannels = {"testChannel", "testChannel2"}; 

			TestGetValidChannelsCommon<string> (rawChannels, multiChannel, false, ResponseType.Unsubscribe);
		}

		[Test]
		public void TestGetValidChannelsEmptyUnsub (){ 
			string[] multiChannel = {"testChannel3", "testChannel"}; 
			string[] rawChannels = {" "}; 
			ExceptionCode = 117;
			TestGetValidChannelsCommon<string> (rawChannels, multiChannel, true, ResponseType.Unsubscribe);
		}

		[Test]
		public void TestGetValidChannelsNotSubPUnsub (){ 
			string[] multiChannel = {"testChannel", "testChannel2"}; 
			string[] rawChannels = {"testChannel3", "testChannel", "testChannel2"}; 
			ExceptionCode = 119;
			TestGetValidChannelsCommon<string> (rawChannels, multiChannel, true, ResponseType.PresenceUnsubscribe);
		}

		[Test]
		public void TestGetValidChannelsPUnsub (){ 
			string[] multiChannel = {"testChannel-pnpres", "testChannel2-pnpres"}; 
			string[] rawChannels = {"testChannel", "testChannel2"}; 

			TestGetValidChannelsCommon<string> (rawChannels, multiChannel, false, ResponseType.PresenceUnsubscribe);
		}

		[Test]
		public void TestGetValidChannelsEmptyPUnsub (){ 
			string[] multiChannel = {"testChannel3", "testChannel"}; 
			string[] rawChannels = {" "}; 
			ExceptionCode = 117;
			TestGetValidChannelsCommon<string> (rawChannels, multiChannel, true, ResponseType.PresenceUnsubscribe);
		}

		public void TestGetValidChannelsCommon<T> (string[] rawChannels,
			string[] multiChannel, bool fireCallback, ResponseType responseType
		){ 
			readCallback = fireCallback;

			PubnubErrorFilter.Level errorLevel = PubnubErrorFilter.Level.Info;
			SafeDictionary<string, long> multiChannelSubscribe = new SafeDictionary<string, long> ();
			foreach (string currentChannel in multiChannel) {
				multiChannelSubscribe.AddOrUpdate (currentChannel, 14498416434364941, (key, oldValue) => Convert.ToInt64 (14498416434364941));
			}
			bool result1 = false;

			Action<PubnubClientError> errorcb = ErrorCallbackCommonExceptionHandler;
			if (!readCallback) {
				errorcb = null;
			}

			List<string> validChannels2 = Helpers.GetValidChannels<T> (responseType,
				errorcb, rawChannels, multiChannelSubscribe, errorLevel
			);
			if (!readCallback) {
				UnityEngine.Debug.Log ("not fireCallback" + validChannels2.Count);
				foreach (string channel in validChannels2) {
					UnityEngine.Debug.Log ("ch:" + channel);
				}
				if (responseType.Equals (ResponseType.PresenceUnsubscribe)) {
					Assert.IsTrue (validChannels2.Contains (rawChannels [0] + "-pnpres"));
				} else {
					Assert.IsTrue (validChannels2.Contains (rawChannels [0]));
				}
			}
		}

		[Test]
		public void TestGetCurrentSubscriberChannels (){ 
			string[] multiChannel = {"testChannel3", "testChannel"}; 

			SafeDictionary<string, long> multiChannelSubscribe = new SafeDictionary<string, long> ();
			foreach (string currentChannel in multiChannel) {
				multiChannelSubscribe.AddOrUpdate (currentChannel, 14498416434364941, (key, oldValue) => Convert.ToInt64 (14498416434364941));
			}

			string[] channels = Helpers.GetCurrentSubscriberChannels (multiChannelSubscribe);
			bool bFound = false;
			foreach (string ch in channels) {
				bool bFound1 = false;
				foreach (string ch2 in multiChannel) {
					if (ch.Equals (ch2)) {
						bFound1 = true;
						break;
					}
				}
				if (bFound1) {
					bFound = true;
				} else {
					bFound = false;
				}
			}
			Assert.IsTrue (bFound);
		}

		void UserCallbackCommonExceptionHandler (string result)
		{
			UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
			bool bRes = result.Equals (ExpectedRegularResponse);
			UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK: {0}", bRes));
			Assert.IsTrue (bRes);

		}

		void UserCallbackCommonExceptionHandler (object result)
		{
			UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result.ToString()));
			bool bRes = result.Equals (ExpectedRegularResponse);
			UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK: {0}", bRes));
			Assert.IsTrue (bRes);
		}

		void DisconnectCallbackCommonExceptionHandler (string result)
		{
			UnityEngine.Debug.Log (string.Format ("Disconnect CALLBACK LOG: {0}", result));
		}

		void ConnectCallbackCommonExceptionHandler (string result)
		{
			UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
			bool bRes = result.Equals (ExpectedConnectResponse);
			UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK: {0}", bRes));
			Assert.IsTrue (bRes);
		}

		void ConnectCallbackCommonExceptionHandler (object result)
		{
			UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result.ToString()));
			Pubnub pubnub = new Pubnub (
				Common.PublishKey,
				Common.SubscribeKey,
				"",
				"",
				true
			);
			bool bRes = pubnub.JsonPluggableLibrary.SerializeToJsonString(result).Equals (ExpectedConnectResponse);
			UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK: {0}", bRes));
			Assert.IsTrue (bRes);
		}

		[Test]
		public void TestProcessResponseCallbacksConnectedSub (){ 
			string[] multiChannel = {"testChannel"};
			List<object> result = new List<object> ();
			List<object> result2 = new List<object> ();
			//result2.Add ("[]");
			result.Add (result2);
			result.Add (14498416434364941);
			result.Add (string.Join(",", multiChannel));

			ExpectedConnectResponse = "[1,\"Connected\",\"testChannel\"]";
			TestProcessResponseCallbacksCommon<string> (multiChannel, result, "", 14498416434364941, false, false, ResponseType.Subscribe,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		[Test]
		public void TestProcessResponseCallbacksConnectedPres (){ 
			string[] multiChannel = {"testChannel"};
			List<object> result = new List<object> ();
			List<object> result2 = new List<object> ();
			//result2.Add ("[]");
			result.Add (result2);
			result.Add (14498416434364941);
			result.Add (string.Join(",", multiChannel));

			ExpectedConnectResponse = "[1,\"Presence Connected\",\"testChannel\"]";
			TestProcessResponseCallbacksCommon<string> (multiChannel, result, "", 14498416434364941, false, false, ResponseType.Presence,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		[Test]
		public void TestProcessResponseCallbacksConnectedSubObj (){ 
			string[] multiChannel = {"testChannel"};
			List<object> result = new List<object> ();
			List<object> result2 = new List<object> ();
			//result2.Add ("[]");
			result.Add (result2);
			result.Add (14498416434364941);
			result.Add (string.Join(",", multiChannel));

			ExpectedConnectResponse = "[1,\"Connected\",\"testChannel\"]";
			TestProcessResponseCallbacksCommon<object> (multiChannel, result, "", 14498416434364941, false, false, ResponseType.Subscribe,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		[Test]
		public void TestProcessResponseCallbacksConnectedPresObj (){ 
			string[] multiChannel = {"testChannel"};
			List<object> result = new List<object> ();
			List<object> result2 = new List<object> ();
			//result2.Add ("[]");
			result.Add (result2);
			result.Add (14498416434364941);
			result.Add (string.Join(",", multiChannel));

			ExpectedConnectResponse = "[1,\"Presence Connected\",\"testChannel\"]";
			TestProcessResponseCallbacksCommon<object> (multiChannel, result, "", 14498416434364941, false, false, ResponseType.Presence,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		[Test]
		public void TestProcessResponseCallbacksSub (){ 
			string[] multiChannel = {"testChannel"};
			List<object> result = new List<object> ();
			List<object> result2 = new List<object> ();
			result2.Add ("[\"test message\"]");
			result.Add (result2);
			result.Add (14498416434364941);
			result.Add (string.Join(",", multiChannel));

			ExpectedRegularResponse = "[\"[\"test message\"]\",\"14498416434364941\",\"testChannel\"]";

			TestProcessResponseCallbacksCommon<string> (multiChannel, result, "", 14498416434364941, false, false, ResponseType.Subscribe,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		public void TestProcessResponseCallbacksCommon<T>(string [] multiChannel, List<object> result, string cipherKey, long timetoken, bool isTimeout,
			bool resumeOnReconnect, ResponseType responseType,
			Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback 
		){
			Pubnub pubnub = new Pubnub (
				Common.PublishKey,
				Common.SubscribeKey,
				"",
				cipherKey,
				true
			);

			SafeDictionary<string, long> multiChannelSubscribe = new SafeDictionary<string, long> ();
			foreach (string currentChannel in multiChannel) {
				//multiChannelSubscribe.AddOrUpdate (currentChannel, 14498416434364941, (key, oldValue) => Convert.ToInt64 (14498416434364941));
				multiChannelSubscribe.AddOrUpdate (currentChannel, 0, (key, oldValue) => Convert.ToInt64 (0));
			}

			SafeDictionary<PubnubChannelCallbackKey, object> channelCallbacks = Common.CreateChannelCallbacks (multiChannel, responseType,
				userCallback, connectCallback, errorCallback);
			

			RequestState<T> requestState = BuildRequests.BuildRequestState<T> (multiChannel, responseType, 
				resumeOnReconnect, userCallback, connectCallback, errorCallback, 0, isTimeout, timetoken, typeof(T));


			Helpers.ProcessResponseCallbacks <T> (result, requestState, multiChannelSubscribe, 
				cipherKey, channelCallbacks, pubnub.JsonPluggableLibrary);
		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesObjSubscribeConnectedCipher (){ 
			string[] multiChannel = {"testChannel3", "testChannel"};
			resultList = new List<object> ();
			resultList.Add ("[]");
			resultList.Add ("14498416434364941");
			resultList.Add (string.Join(",", multiChannel));

			TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "enigma", "[[], 14498416434364941]", ResponseType.Subscribe,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);
		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesObjSubscribeMessageCipher (){ 
			string[] multiChannel = {"testChannel3", "testChannel"};
			resultList = new List<object> ();
			resultList.Add ("[\"test\"]");
			resultList.Add ("14498416434364941");
			resultList.Add (string.Join(",", multiChannel));

			TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "enigma", "[[\"test\"], 14498416434364941]", ResponseType.Subscribe,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);
		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesObjSubscribeMessageCipherYay (){ 
			string[] multiChannel = {"testChannel3", "testChannel"};
			resultList = new List<object> ();
			resultList.Add ("[\"q/xJqqN6qbiZMXYmiQC1Fw==\"]");
			resultList.Add ("14498416434364941");
			resultList.Add (string.Join(",", multiChannel));

			TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "enigma", "[[\"q/xJqqN6qbiZMXYmiQC1Fw==\"], 14498416434364941]", ResponseType.Subscribe,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);
		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesObjSubscribe2MessageCipher (){ 
			string[] multiChannel = {"testChannel3"};
			resultList = new List<object> ();
			resultList.Add ("[\"test\",\"test2\"]");
			resultList.Add ("14498416434364941");
			resultList.Add (string.Join(",", multiChannel));

			TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "enigma", "[[\"test\",\"test2\"], 14498416434364941, \"testChannel3\"]", ResponseType.Subscribe,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);
		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesObjPublishCipher (){ 
			string[] multiChannel = {"testChannel3"};
			resultList = new List<object> ();
			resultList.Add (1);
			resultList.Add ("Sent");
			resultList.Add ("14498416434364941");
			resultList.Add (string.Join(",", multiChannel));

			TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "enigma", "[1, \"Sent\", \"14498416434364941\"]", ResponseType.Publish,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesObjPublish (){ 
			string[] multiChannel = {"testChannel3"};
			resultList = new List<object> ();
			resultList.Add (1);
			resultList.Add ("Sent");
			resultList.Add ("14498416434364941");
			resultList.Add (string.Join(",", multiChannel));

			TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "", "[1, \"Sent\", \"14498416434364941\"]", ResponseType.Publish,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesObjDH (){ 
			string[] multiChannel = {"testChannel3"};
			resultList = new List<object> ();
			resultList.Add ("[\"test1\",\"test2\"]");
			resultList.Add (string.Join(",", multiChannel));

			TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "", "[[\"test1\",\"test2\"], \"testChannel3\"]", ResponseType.DetailedHistory,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesObjHereNow (){ 
			string[] multiChannel = {"testChannel3"};
			resultList = new List<object> ();
			resultList.Add ("{\"status\":200,\"message\":\"OK\",\"service\":\"Presence\",\"occupancy\":0}");
			resultList.Add (string.Join(",", multiChannel));

			TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "", 
				"{\"status\": 200, \"message\": \"OK\", \"service\": \"Presence\", \"occupancy\": 0}", ResponseType.HereNow,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesObjGlobalHereNow (){ 
			string[] multiChannel = {"testChannel3"};
			resultList = new List<object> ();
			resultList.Add ("{\"status\":200,\"message\":\"OK\",\"payload\":{\"channels\":{\"e\":{\"occupancy\":1},\"sentiment\":{\"occupancy\":1},\"testChannel\":{\"occupancy\":3}},\"total_channels\":3,\"total_occupancy\":5},\"service\":\"Presence\"}");

			TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "", 
				"{\"status\": 200, \"message\": \"OK\", \"payload\": {\"channels\": {\"e\": {\"occupancy\": 1}, \"sentiment\": {\"occupancy\": 1}, \"testChannel\": {\"occupancy\": 3}}, \"total_channels\": 3, \"total_occupancy\": 5}, \"service\": \"Presence\"}",
				ResponseType.GlobalHereNow,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesObjTime(){ 
			string[] multiChannel = {};
			resultList = new List<object> ();
			resultList.Add ("14510493331774269");

			TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "", "[14510493331774269]", ResponseType.Time,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesSubscribeConnectedCipher (){ 
			string[] multiChannel = {"testChannel3", "testChannel"};
			resultList = new List<object> ();
			resultList.Add ("[]");
			resultList.Add ("14498416434364941");
			resultList.Add (string.Join(",", multiChannel));

			TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "enigma", "[[], 14498416434364941]", ResponseType.Subscribe,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);
		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesSubscribeMessageCipher (){ 
			string[] multiChannel = {"testChannel3", "testChannel"};
			resultList = new List<object> ();
			resultList.Add ("[\"test\"]");
			resultList.Add ("14498416434364941");
			resultList.Add (string.Join(",", multiChannel));

			TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "enigma", "[[\"test\"], 14498416434364941]", ResponseType.Subscribe,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);
		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesSubscribeMessageCipherYay (){ 
			string[] multiChannel = {"testChannel3", "testChannel"};
			resultList = new List<object> ();
			resultList.Add ("[\"q/xJqqN6qbiZMXYmiQC1Fw==\"]");
			resultList.Add ("14498416434364941");
			resultList.Add (string.Join(",", multiChannel));

			TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "enigma", "[[\"q/xJqqN6qbiZMXYmiQC1Fw==\"], 14498416434364941]", ResponseType.Subscribe,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);
		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesSubscribe2MessageCipher (){ 
			string[] multiChannel = {"testChannel3"};
			resultList = new List<object> ();
			resultList.Add ("[\"test\",\"test2\"]");
			resultList.Add ("14498416434364941");
			resultList.Add (string.Join(",", multiChannel));

			TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "enigma", "[[\"test\",\"test2\"], 14498416434364941, \"testChannel3\"]", ResponseType.Subscribe,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);
		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesPublishCipher (){ 
			string[] multiChannel = {"testChannel3"};
			resultList = new List<object> ();
			resultList.Add (1);
			resultList.Add ("Sent");
			resultList.Add ("14498416434364941");
			resultList.Add (string.Join(",", multiChannel));
							
			TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "enigma", "[1, \"Sent\", \"14498416434364941\"]", ResponseType.Publish,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesPublish (){ 
			string[] multiChannel = {"testChannel3"};
			resultList = new List<object> ();
			resultList.Add (1);
			resultList.Add ("Sent");
			resultList.Add ("14498416434364941");
			resultList.Add (string.Join(",", multiChannel));

			TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "", "[1, \"Sent\", \"14498416434364941\"]", ResponseType.Publish,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesDH (){ 
			string[] multiChannel = {"testChannel3"};
			resultList = new List<object> ();
			resultList.Add ("[\"test1\",\"test2\"]");
			resultList.Add (string.Join(",", multiChannel));

			TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "", "[[\"test1\",\"test2\"], \"testChannel3\"]", ResponseType.DetailedHistory,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesHereNow (){ 
			string[] multiChannel = {"testChannel3"};
			resultList = new List<object> ();
			resultList.Add ("{\"status\":200,\"message\":\"OK\",\"service\":\"Presence\",\"occupancy\":0}");
			resultList.Add (string.Join(",", multiChannel));

			TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "", 
				"{\"status\": 200, \"message\": \"OK\", \"service\": \"Presence\", \"occupancy\": 0}", ResponseType.HereNow,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesGlobalHereNow (){ 
			string[] multiChannel = {"testChannel3"};
			resultList = new List<object> ();
			resultList.Add ("{\"status\":200,\"message\":\"OK\",\"payload\":{\"channels\":{\"e\":{\"occupancy\":1},\"sentiment\":{\"occupancy\":1},\"testChannel\":{\"occupancy\":3}},\"total_channels\":3,\"total_occupancy\":5},\"service\":\"Presence\"}");

			TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "", 
				"{\"status\": 200, \"message\": \"OK\", \"payload\": {\"channels\": {\"e\": {\"occupancy\": 1}, \"sentiment\": {\"occupancy\": 1}, \"testChannel\": {\"occupancy\": 3}}, \"total_channels\": 3, \"total_occupancy\": 5}, \"service\": \"Presence\"}",
				ResponseType.GlobalHereNow,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		[Test]
		public void TestWrapResultBasedOnResponseTypesTime(){ 
			string[] multiChannel = {};
			resultList = new List<object> ();
			resultList.Add ("14510493331774269");

			TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "", "[14510493331774269]", ResponseType.Time,
				UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
			);

		}

		public void TestWrapResultBasedOnResponseTypeCommon<T>(string [] channels, string cipherKey, string jsonString,
			ResponseType responseType, Action<T> userCallback, 
			Action<T> connectCallback, Action<PubnubClientError> errorCallback ){

			//here now {"status": 200, "message": "OK", "service": "Presence", "occupancy": 0}
			//global herenow {"status": 200, "message": "OK", "payload": {"channels": {"e": {"occupancy": 1}, "sentiment": {"occupancy": 1}, "testChannel": {"occupancy": 3}}, "total_channels": 3, "total_occupancy": 5}, "service": "Presence"}
			//time [14510493331774269]
			Pubnub pubnub = new Pubnub (
				Common.PublishKey,
				Common.SubscribeKey,
				"",
				cipherKey,
				true
			);

			SafeDictionary<PubnubChannelCallbackKey, object> channelCallbacks = Common.CreateChannelCallbacks (channels, responseType,
				                                                                    userCallback, connectCallback, errorCallback);

			List<object> list = Helpers.WrapResultBasedOnResponseType<T> (responseType, jsonString, channels, errorCallback, channelCallbacks, 
				                    pubnub.JsonPluggableLibrary, PubnubErrorFilter.Level.Info, cipherKey);

			bool bResult = false;
			bResult = MatchList (bResult, responseType, list, resultList, pubnub.JsonPluggableLibrary);

			Assert.IsTrue (bResult);
		}

		bool MatchList(bool bResult, ResponseType responseType, List<object> list, List<object> resList, IJsonPluggableLibrary jsonPluggableLibrary){
			foreach (object obj in list) {
				UnityEngine.Debug.Log ("obj:" + obj.ToString ());	
				bool bResult1 = false;
				Type valueType = obj.GetType();
				var expectedType = typeof(String);
				var expectedType2 = typeof(Object);
				var expectedType3 = typeof(String[]);
				var expectedType4 = typeof(Object[]);
				foreach (object obj2 in resList) {
					UnityEngine.Debug.Log ("obj2:" + obj2.ToString ());

					if (valueType.IsArray && (expectedType.IsAssignableFrom (valueType.GetElementType ())
					    || expectedType2.IsAssignableFrom (valueType.GetElementType ())
					    || expectedType3.IsAssignableFrom (valueType.GetElementType ())
					    || expectedType4.IsAssignableFrom (valueType.GetElementType ())
					    )) {
						UnityEngine.Debug.Log ("obj2:" + obj2.ToString () + valueType.IsArray
						+ expectedType.IsAssignableFrom (valueType.GetElementType ())
						+ expectedType2.IsAssignableFrom (valueType.GetElementType ())
						+ expectedType3.IsAssignableFrom (valueType.GetElementType ())
						+ expectedType4.IsAssignableFrom (valueType.GetElementType ())
						+ valueType.GetElementType ().ToString ());	

						if (expectedType.IsAssignableFrom (valueType.GetElementType ())) {
							string[] arrObj = ((System.Collections.IEnumerable)obj).Cast<object> ().Select (x => x.ToString ()).ToArray ();
							foreach (string str in arrObj) {
								UnityEngine.Debug.Log ("array:" + str);	
							}
							bResult1 = jsonPluggableLibrary.SerializeToJsonString (arrObj).Equals (obj2.ToString ());
							UnityEngine.Debug.Log ("array" + bResult1 + jsonPluggableLibrary.SerializeToJsonString (arrObj));
						} else {
							object[] arrObj = ((System.Collections.IEnumerable)obj).Cast<object> ().Select (x => x.ToString ()).ToArray ();
							foreach (string str in arrObj) {
								UnityEngine.Debug.Log ("obj array:" + str);	
							}
							bResult1 = jsonPluggableLibrary.SerializeToJsonString (arrObj).Equals (obj2.ToString ());
							UnityEngine.Debug.Log ("obj array" + bResult1 + jsonPluggableLibrary.SerializeToJsonString (arrObj));
						}
						if (bResult1) {
							break;
						}
					} else if (obj.GetType().IsGenericType){
						UnityEngine.Debug.Log ("generic:" + obj2.ToString());
						bResult1 = jsonPluggableLibrary.SerializeToJsonString (obj).Equals (obj2.ToString ());
						UnityEngine.Debug.Log ("obj generic" + bResult1 + jsonPluggableLibrary.SerializeToJsonString (obj));
						if (bResult1) {
							break;
						}
					} else {
						UnityEngine.Debug.Log ("non array/generic:" + obj2.ToString());
						if (obj.ToString().Equals (obj2.ToString())) {
							UnityEngine.Debug.Log ("obj-obj2:" + obj.ToString () + "-" + obj2.ToString ());
							bResult1 = true;
							break;
						} 
					}
				}
				if (bResult1) {
					bResult = true;
				} else {
					bResult = false;
					UnityEngine.Debug.Log ("obj not found:" + obj.ToString ());
					break;
				}
			}
			return bResult;
		}

		#endif
	}
}

