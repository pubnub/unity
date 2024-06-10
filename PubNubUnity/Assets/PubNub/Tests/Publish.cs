using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PubnubApi.Unity.Tests {

	public class Publish : PNTestBase {
		string lastMessage = null;

		private void OnMessage(Pubnub arg1, PNMessageResult<object> arg2) {
			Debug.Log($"Message Received. Channel: {arg2.Channel} / Message: {arg2.Message}");
			Debug.Log($"Received Message type {arg2.Message?.GetType()?.ToString()}");
			lastMessage = arg2.Message as string;
		}

		[SetUp]
		public void SetUp() {
			listener.onMessage += OnMessage;
		}

		[UnityTest]
		public IEnumerator PublishTestMessage() {
			pn.Publish().Channel("test").Message("test")
				.Execute(Callback(out var awaiter, out var assigner));
			yield return awaiter;
			var s = assigner().status;

			Assert.IsFalse(s.Error);
		}

		[UnityTest]
		public IEnumerator ReceiveMessage() {
			yield return new WaitUntil(() => lastMessage != null);

			Assert.AreEqual("test", lastMessage);
			if (lastMessage != "test") {
				Debug.Log($"Message type {lastMessage?.GetType()?.ToString()}");
			}

			lastMessage = null;
		}

		[TearDown]
		public void TearDown() {
			listener.onMessage -= OnMessage;
		}
	}
}