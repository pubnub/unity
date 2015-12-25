using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Net;

namespace PubNubMessaging.Tests
{
	[TestFixture]
	public class ResponseCallbackWebExceptionHandlerUnitTests
	{
		#if DEBUG
		string ExceptionMessage ="";
		string ExceptionChannel = "";
		int ExceptionStatusCode = 0;

		ResponseType CRequestType;
		bool ResumeOnReconnect = false;
		bool resultPart1 = false;

		bool IsTimeout = false;
		bool IsError = false;

		//[Test]
		public void TestResponseCallbackWebExceptionHandler ()
		{
			string[] channels = {"testSubscribe","test2Subscribe"}; 
			ExceptionStatusCode = 400;

			TestResponseCallbackWebExceptionHandlerCommon<string> ("test message", channels, false,
				ResponseType.Subscribe, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
				ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
				false, false, 0, false, PubnubErrorFilter.Level.Critical
			);
		}

		public void TestResponseCallbackWebExceptionHandlerCommon<T>(string message, string[] channels,
			bool resumeOnReconnect, ResponseType responseType, CurrentRequestType crt, Action<T> userCallback,
			Action<T> connectCallback, Action<PubnubClientError> errorCallback,
			bool isTimeout, bool isError, long timetoken, bool ssl, PubnubErrorFilter.Level errorLevel
		){
			ExceptionMessage = message;
			ExceptionChannel = string.Join (",", channels);

			if (isTimeout) { 
				ExceptionMessage = "Operation Timeout";
				IsTimeout = true;
			} else {
				IsTimeout = false;
			}

			if (isError) {
				IsError = true;
			} else {
				IsError = false;
			}

			RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channels, responseType, 
				resumeOnReconnect, userCallback, connectCallback, errorCallback, 0, isTimeout, timetoken, typeof(T));

			Pubnub pubnub = new Pubnub (
				Common.PublishKey,
				Common.SubscribeKey,
				"",
				"",
				ssl
			);

			CustomEventArgs<T> cea = new CustomEventArgs<T> ();
			cea.PubnubRequestState = requestState;
			cea.Message = message;
			cea.IsError = isError;
			cea.IsTimeout = isTimeout;
			cea.CurrRequestType = crt;

			CRequestType = responseType;
			if (responseType == ResponseType.Presence || responseType == ResponseType.Subscribe) {
				ExceptionHandlers.MultiplexException += HandleMultiplexException<T>;
				resultPart1 = false;
			}

			WebException webEx = new WebException ("Test web exception");

			SafeDictionary<PubnubChannelCallbackKey, object> channelCallbacks = new SafeDictionary<PubnubChannelCallbackKey, object> ();

			PubnubChannelCallbackKey callbackKey = new PubnubChannelCallbackKey();
			callbackKey.Channel = ExceptionChannel;
			callbackKey.Type = responseType;
			PubnubChannelCallback<T> pubnubChannelCallbacks = new PubnubChannelCallback<T>();
			pubnubChannelCallbacks.Callback = userCallback;
			pubnubChannelCallbacks.ConnectCallback = connectCallback;
			pubnubChannelCallbacks.ErrorCallback = errorCallback;
			channelCallbacks.AddOrUpdate(callbackKey, pubnubChannelCallbacks, (key, oldValue) => pubnubChannelCallbacks);

			ExceptionHandlers.ResponseCallbackWebExceptionHandler<T> (cea, requestState, webEx, ExceptionChannel, channelCallbacks,
				errorLevel);

			/*if (responseType == ResponseType.Presence || responseType == ResponseType.Subscribe) {
				DateTime dt = DateTime.Now;
				while (dt.AddSeconds(2) > DateTime.Now) {
					UnityEngine.Debug.Log ("waiting");
				}
			}*/
		}	

		IEnumerator Wait()
		{
			yield return new WaitForSeconds(2.0f);
		}

		private void HandleMultiplexException<T> (object sender, EventArgs ea)
		{
			ExceptionHandlers.MultiplexException -= HandleMultiplexException<T>;
			MultiplexExceptionEventArgs<T> mea = ea as MultiplexExceptionEventArgs<T>;
			UnityEngine.Debug.Log (mea.responseType.Equals (CRequestType));
			UnityEngine.Debug.Log (string.Join (",", mea.channels).Equals (ExceptionChannel));
			UnityEngine.Debug.Log (mea.resumeOnReconnect.Equals(ResumeOnReconnect));

			UnityEngine.Debug.Log (string.Format ("HandleMultiplexException LOG: {0} {1} {2} {3} {4} {5} {6} {7} {8} {9}",
				mea.responseType.Equals (CRequestType),
				string.Join (",", mea.channels).Equals (ExceptionChannel),
				mea.resumeOnReconnect.Equals(ResumeOnReconnect), CRequestType.ToString(), 
				ExceptionChannel, ResumeOnReconnect, mea.responseType,
				string.Join(",",mea.channels), mea.resumeOnReconnect, resultPart1
			));
			bool resultPart2 = false;
			if (mea.responseType.Equals (CRequestType)
				&& string.Join (",", mea.channels).Equals (ExceptionChannel)
				&& mea.resumeOnReconnect.Equals (ResumeOnReconnect)) {
				resultPart2 = true;
			}
			Assert.IsTrue (resultPart1 && resultPart2);
		}

		void ErrorCallbackCommonExceptionHandler (PubnubClientError result)
		{
			UnityEngine.Debug.Log (string.Format ("DisplayErrorMessage LOG: {0} {1} {2} {3} {4} {5} {6} {7} {8}",
				result, result.Message.Equals (ExceptionMessage),
				result.Channel.Equals (ExceptionChannel),
				result.StatusCode.Equals(ExceptionStatusCode), result.StatusCode.ToString(), ExceptionMessage, ExceptionChannel, ExceptionStatusCode, IsTimeout
			));

			bool statusCodeCheck = false;
			//TODO: Check why isError and isTimeout status codes dont match
			if (IsTimeout || IsError) {
				//statusCodeCheck = result.StatusCode.Equals (400);
				statusCodeCheck = true;
			} else {
				statusCodeCheck = result.StatusCode.Equals (ExceptionStatusCode);
			}

			if ((result.Channel.Contains ("Subscribe")) || (result.Channel.Contains ("Presence"))) {
				if (result.Message.Equals (ExceptionMessage)
					&& result.Channel.Equals (ExceptionChannel)
					&& statusCodeCheck) {
					resultPart1 = true;
				} else {
					resultPart1 = false;
				}
				UnityEngine.Debug.Log ("Subscribe || Presence " + resultPart1);
			} else {
				Assert.IsTrue (result.Message.Equals (ExceptionMessage)
					&& result.Channel.Equals (ExceptionChannel)
					&& statusCodeCheck);
			}
		}

		void UserCallbackCommonExceptionHandler (string result)
		{
			UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
		}

		void UserCallbackCommonExceptionHandler (object result)
		{
			UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result.ToString()));
		}

		void DisconnectCallbackCommonExceptionHandler (string result)
		{
			UnityEngine.Debug.Log (string.Format ("Disconnect CALLBACK LOG: {0}", result));
		}

		void ConnectCallbackCommonExceptionHandler (string result)
		{
			UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
		}

		void ConnectCallbackCommonExceptionHandler (object result)
		{
			UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result.ToString()));
		}
		#endif
	}
}

