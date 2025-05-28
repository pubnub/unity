using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

namespace PubnubApi.Unity.Tests {

	public class DummyCustomClass {
		public string someText;
		public int someInt;
		public List<int> someCollection;
	}

	public class Publish : PNTestBase {
		string lastMessage = null;

		private void OnMessage(Pubnub arg1, PNMessageResult<object> arg2) {
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

		[UnityTest]
		public IEnumerator PublishAndReceiveCustomMessageWithUnityJson() {
			yield return TestTask().AsCoroutine();
			async Task TestTask() {
				pn.SetJsonPluggableLibrary(new NewtonsoftJsonUnity(configuration));

				pn.Subscribe<DummyCustomClass>().Channels(new []{"test_channel"}).Execute();
				await Task.Delay(3000);

				var correctMessage = false;
				var receivedReset = new ManualResetEvent(false);

				var messageDelegate = new Action<Pubnub,PNMessageResult<object>>(delegate(Pubnub p, PNMessageResult<object> message) {
					if (message.Message is DummyCustomClass dummyClassObject
					    && dummyClassObject.someCollection.SequenceEqual(new List<int>() { 2, 1, 3, 7 })
					    && dummyClassObject.someText == "hello there"
					    && dummyClassObject.someInt == 97) {
						correctMessage = true;
					}
					receivedReset.Set();
				});
				listener.onMessage += messageDelegate;

				var publishResult = await pn.Publish().Channel("test_channel").Message(new DummyCustomClass() {
					someCollection = new List<int>() { 2, 1, 3, 7 },
					someInt = 97,
					someText = "hello there"
				}).ExecuteAsync();

				Assert.IsNotNull(publishResult.Result, "publishResult.Result should not be null");
				Assert.IsNotNull(publishResult.Status, "publishResult.Status should not be null");
				Assert.IsFalse(publishResult.Status.Error, $"publishResult.Status.Error is true, error: {publishResult.Status.ErrorData?.Information}");

				var received = receivedReset.WaitOne(15000);
				Assert.IsTrue(received, "didn't receive message callback");
				Assert.IsTrue(correctMessage, "deserialized message had incorrect data");

				listener.onMessage -= messageDelegate;
			}
		}

		[TearDown]
		public void TearDown() {
			listener.onMessage -= OnMessage;
		}
	}
}