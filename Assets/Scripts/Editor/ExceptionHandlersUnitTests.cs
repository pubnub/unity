using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Text;
using UnityEngine;
using System.Collections;

namespace PubNubMessaging.Tests
{
	[TestFixture]
	public class ExceptionHandlersUnitTests
	{
		#if DEBUG
		string ExceptionMessage ="";
		string ExceptionChannel = "";
		int ExceptionStatusCode = 0;

		ResponseType CRequestType;
		bool ResumeOnReconnect = false;
		bool resultPart1 = false;

		[Test]
		public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe400 ()
		{
			string[] channels = {"testSubscribe","test2Subscribe"}; 
			ExceptionStatusCode = 400;

			TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, false,
				ResponseType.Subscribe, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
				ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
				false, false, 0, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe404 ()
		{
			string[] channels = {"testSubscribe","test2Subscribe"}; 
			ExceptionStatusCode = 400;

			TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, false,
				ResponseType.Subscribe, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
				ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
				false, false, 0, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe414 ()
		{
			string[] channels = {"testSubscribe","test2Subscribe"}; 
			ExceptionStatusCode = 414;
			
			TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, false,
				ResponseType.Subscribe, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
				ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
				false, false, 0, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe504 ()
		{
			string[] channels = {"testSubscribe","test2Subscribe"}; 
			ExceptionStatusCode = 504;
			
			TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, false,
				ResponseType.Subscribe, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
				ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
				false, false, 0, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe503 ()
		{
			string[] channels = {"testSubscribe","test2Subscribe"}; 
			ExceptionStatusCode = 503;
			
			TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, false,
				ResponseType.Subscribe, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
				ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
				false, false, 0, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe500 ()
		{
			string[] channels = {"testSubscribe","test2Subscribe"}; 
			ExceptionStatusCode = 500;
			
			TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, false,
				ResponseType.Subscribe, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
				ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
				false, false, 0, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe403 ()
		{
			string[] channels = {"testSubscribe","test2Subscribe"}; 
			ExceptionStatusCode = 403;
			
			TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, false,
				ResponseType.Subscribe, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
				ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
				false, false, 0, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeNameResolutionFailure ()
		{
			string[] channels = {"testSubscribe","test2Subscribe"}; 
			ExceptionStatusCode = 122;
			
			TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, false,
				ResponseType.Subscribe, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
				ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
				false, false, 0, false, PubnubErrorFilter.Level.Warning
			);
		}

		[Test]
		public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeConnectFailure ()
		{
			string[] channels = {"testSubscribe","test2Subscribe"}; 
			ExceptionStatusCode = 122;
			
			TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, false,
				ResponseType.Subscribe, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
				ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
				false, false, 0, false, PubnubErrorFilter.Level.Warning
			);
		}

		[Test]
		public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeServerProtocolViolation ()
		{
			string[] channels = {"testSubscribe","test2Subscribe"}; 
			ExceptionStatusCode = 122;
			
			TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, false,
				ResponseType.Subscribe, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
				ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
				false, false, 0, false, PubnubErrorFilter.Level.Warning
			);
		}

		[Test]
		public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeProtocolError ()
		{
			string[] channels = {"testSubscribe","test2Subscribe"}; 
			ExceptionStatusCode = 122;
			
			TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, false,
				ResponseType.Subscribe, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
				ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
				false, false, 0, false, PubnubErrorFilter.Level.Warning
			);
		}

		[Test]
		public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeFNF ()
		{
			string[] channels = {"testSubscribe","test2Subscribe"}; 
			ExceptionStatusCode = 403;
			
			TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, false,
				ResponseType.Subscribe, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
				ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
				false, false, 0, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeFailedDL ()
		{
			string[] channels = {"testSubscribe","test2Subscribe"}; 
			ExceptionStatusCode = 400;
			
			TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, false,
				ResponseType.Subscribe, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
				ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
				false, false, 0, false, PubnubErrorFilter.Level.Warning
			);
		}

		public void TestResponseCallbackErrorOrTimeoutHandler<T>(string message, string[] channels,
			bool resumeOnReconnect, ResponseType responseType, CurrentRequestType crt, Action<T> userCallback,
			Action<T> connectCallback, Action<PubnubClientError> errorCallback,
			bool isTimeout, bool isError, long timetoken, bool ssl, PubnubErrorFilter.Level errorLevel
		){
			ExceptionMessage = message;
			ExceptionChannel = string.Join (",", channels);

			if (isTimeout) { 
				ExceptionMessage = "Operation Timeout";
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
			ExceptionHandlers.ResponseCallbackErrorOrTimeoutHandler<T> (cea, requestState, ExceptionChannel, 
				errorLevel, pubnub.JsonPluggableLibrary);

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
			UnityEngine.Debug.Log (string.Format ("DisplayErrorMessage LOG: {0} {1} {2} {3} {4} {5} {6}",
				result, result.Message.Equals (ExceptionMessage),
				result.Channel.Equals (ExceptionChannel),
				result.StatusCode.Equals(ExceptionStatusCode), ExceptionMessage, ExceptionChannel, ExceptionStatusCode
			));

			if ((result.Channel.Contains ("Subscribe")) || (result.Channel.Contains ("Presence"))) {
				if (result.Message.Equals (ExceptionMessage)
				    && result.Channel.Equals (ExceptionChannel)
				    && result.StatusCode.Equals (ExceptionStatusCode)) {
					resultPart1 = true;
				} else {
					resultPart1 = false;
				}
				UnityEngine.Debug.Log ("Subscribe || Presence " + resultPart1);
			} else {
				Assert.IsTrue (result.Message.Equals (ExceptionMessage)
				&& result.Channel.Equals (ExceptionChannel)
				&& result.StatusCode.Equals (ExceptionStatusCode));
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

