using System;

namespace PubNubMessaging.Core
{
	#region "Channel callback"
	internal struct PubnubChannelCallbackKey
	{
		public string Channel;
		public ResponseType Type;
	}

	internal class PubnubChannelCallback<T>
	{
		public Action<T> Callback;
		public Action<PubnubClientError> ErrorCallback;
		public Action<T> ConnectCallback;
		public Action<T> DisconnectCallback;
		//public ResponseType Type;
		public PubnubChannelCallback ()
		{
			Callback = null;
			ConnectCallback = null;
			DisconnectCallback = null;
			ErrorCallback = null;
		}
	}
	#endregion
}

