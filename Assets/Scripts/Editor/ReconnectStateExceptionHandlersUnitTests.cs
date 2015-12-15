using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Text;

namespace PubNubMessaging.Tests
{
	[TestFixture]
	public class ReconnectStateExceptionHandlersUnitTests
	{
		#if DEBUG
		void UserCallback (string result)
		{
			UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
		}

		void ErrorCallback (PubnubClientError result)
		{
			UnityEngine.Debug.Log (string.Format ("DisplayErrorMessage LOG: {0}", result));
		}

		void DisconnectCallback (string result)
		{
			UnityEngine.Debug.Log (string.Format ("Disconnect CALLBACK LOG: {0}", result));
		}

		void ConnectCallback (string result)
		{
			UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
		}


		[Test]
		public void TestBuildRequestStateHistory ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<string> (channels, CurrentRequestType.NonSubscribe , ResponseType.DetailedHistory, 
				false, UserCallback, ConnectCallback, ErrorCallback, 0, false, 0, null
			);
		}

		[Test]
		public void TestBuildRequestStateHereNow ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<string> (channels, CurrentRequestType.NonSubscribe , ResponseType.HereNow, 
				false, UserCallback, ConnectCallback, ErrorCallback, 0, false, 0, null
			);
		}

		[Test]
		public void TestBuildRequestStateSubscribe ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<string> (channels, CurrentRequestType.NonSubscribe , ResponseType.HereNow, 
				false, UserCallback, ConnectCallback, ErrorCallback, 0, false, 0, null
			);
		}


		public void TestBuildRequestStateCommon<T>(string[] channels, CurrentRequestType requestType, ResponseType responseType, 
			bool reconnect, Action<T> userCallback,
			Action<T> connectCallback, Action<PubnubClientError> errorCallback,
			long id, bool timeout, long timetoken, Type typeParam
		){
			RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channels, responseType, 
				reconnect, userCallback, connectCallback, errorCallback, id, timeout, timetoken, typeParam);

			StoredRequestState.Instance.SetRequestState (requestType, requestState);
			RequestState<T> reqState = StoredRequestState.Instance.GetStoredRequestState(requestType) as RequestState<T>;

			Assert.IsTrue (reqState.Equals (requestState));
		}	
		#endif
	}
}

