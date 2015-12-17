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
		string[] Channels;
		bool ResumeOnReconnect;
		bool waitForCompletion = true;


		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutHeartbeat ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.Heartbeat, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		public void TestUrlRequestCommonExceptionHandlerCommon<T>(string message, string[] channels,
			bool resumeOnReconnect, ResponseType responseType, Action<T> userCallback,
			Action<T> connectCallback, Action<PubnubClientError> errorCallback,
			bool timeout, PubnubErrorFilter.Level errorLevel
		){
			ExceptionMessage = message;

			if (timeout) { 
				ExceptionMessage = "Operation Timeout";
			} else {
				ExceptionStatusCode = (int)PubnubErrorCode.None;
			}
			CRequestType = responseType;
			ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (message, responseType, channels, 
				timeout, userCallback, connectCallback, errorCallback, resumeOnReconnect, errorLevel );

			/*if (responseType == ResponseType.Presence || responseType == ResponseType.Subscribe) {
				//waitForCompletion = true;
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
			UnityEngine.Debug.Log (string.Format ("HandleMultiplexException LOG: {0} {1} {2} {3} {4} {5} {6} {7} {8}",
				mea.requestType.Equals (CRequestType) ,
				mea.channels.Equals (Channels),
				mea.resumeOnReconnect.Equals(ResumeOnReconnect), CRequestType.ToString(), 
				string.Join(",",Channels), ResumeOnReconnect, mea.requestType,
				string.Join(",",mea.channels), mea.resumeOnReconnect
			));
			//waitForCompletion = false;
			Assert.True (mea.requestType.Equals (CRequestType) 
				&& string.Join(",",mea.channels).Equals (string.Join(",",Channels))
				&& mea.resumeOnReconnect.Equals(ResumeOnReconnect)
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutHeartbeat ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.Heartbeat, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		public void TestCommonExceptionHandlerCommon<T>(string message, string channel,
			ResponseType responseType, Action<PubnubClientError> errorCallback,
			bool timeout, PubnubErrorFilter.Level errorLevel
		){
			ExceptionMessage = message;

			if (timeout) { 
				ExceptionMessage = "Operation Timeout";
			} else {
				ExceptionStatusCode = (int)PubnubErrorCode.None;
			}

			ExceptionChannel = channel;
			ExceptionHandlers.CommonExceptionHandler<T> (message, channel, timeout, errorCallback, errorLevel, responseType);
		}	

		void ErrorCallbackCommonExceptionHandler (PubnubClientError result)
		{
			UnityEngine.Debug.Log (string.Format ("DisplayErrorMessage LOG: {0} {1} {2} {3} {4} {5} {6}",
				result, result.Message.Equals (ExceptionMessage),
				result.Channel.Equals (ExceptionChannel),
				result.StatusCode.Equals(ExceptionStatusCode), ExceptionMessage, ExceptionChannel, ExceptionStatusCode
			));
			Assert.True (result.Message.Equals (ExceptionMessage) 
				&& result.Channel.Equals (ExceptionChannel)
				&& result.StatusCode.Equals(ExceptionStatusCode)
			);
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

		[Test]
		public void TestUrlRequestCommonExceptionHandlerTimeoutSubscribeReconnect ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = true;
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;

			ExceptionHandlers.MultiplexException += HandleMultiplexException<object>;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, true,
				ResponseType.Subscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);

		}
		#endif
	}
}

