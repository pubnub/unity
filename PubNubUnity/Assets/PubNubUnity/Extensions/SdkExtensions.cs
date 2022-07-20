using PubnubApi;
using PubnubApi.EndPoint;
using PubNubUnity.Internal;

namespace PubNubUnity {
	public static class SdkExtensions {
		/// <summary>
		/// Execute the publish operation and run the callback upon completion. The callback is dispatched to Unity main thread
		/// </summary>
		/// <param name="operation">Publish operation</param>
		/// <param name="callback">Callback to run upon operation completion</param>
		public static void Execute(this PublishOperation operation, System.Action<PNPublishResult, PNStatus> callback) {
			operation.Execute(new PNPublishResultExt((a, b) => PNDispatcher.Dispatch(() => callback?.Invoke(a, b))));
		}
	}
}
