using System;
using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

namespace PubnubApi.Unity.Tests {

	public static class TestUtils {
		public static IEnumerator AsCoroutine(this Task task)
		{
			while (!task.IsCompleted) yield return null;
			// if task is faulted, throws the exception
			task.GetAwaiter().GetResult();
		}
	}

	public class PNTestBase {
		protected static Pubnub pn;
		protected static SubscribeCallbackListener listener = new();
		protected static PNConfiguration configuration;

		[OneTimeSetUp]
		public void OneTimeSetUp() {
			var envPub = System.Environment.GetEnvironmentVariable("PUB_KEY");
			var envSub = System.Environment.GetEnvironmentVariable("SUB_KEY");
			var envSec = System.Environment.GetEnvironmentVariable("PAM_SECRET_KEY");
			configuration = new PNConfiguration(new UserId(System.Guid.NewGuid().ToString())) {
				PublishKey = string.IsNullOrEmpty(envPub) ? "demo-36" : envPub,
				SubscribeKey = string.IsNullOrEmpty(envSub) ? "demo-36" : envSub,
				SecretKey = envSec ?? "demo-36"
			};
			pn = new Pubnub(configuration);

			pn.AddListener(listener);

			pn.Subscribe<string>().Channels(new[] { "test" }).WithPresence().Execute();
			pn.Subscribe<string>().ChannelGroups(new[] { "testgroup" }).WithPresence().Execute();
		}

		[OneTimeTearDown]
		public async void OneTimeTearDown() {
			pn.UnsubscribeAll<string>();

			// wat
			await Task.Delay(1000);

			pn.Destroy();
		}

		protected Action<object, PNStatus>
			Callback(out IEnumerator awaiter, out Func<CallbackResult<object>> assigner) {
			CallbackResult<object> wrappedResult = new();
			assigner = () => wrappedResult;

			float startTime = Time.time;

			awaiter = new WaitUntil(() => wrappedResult.status != null || Time.time > startTime + 10f);
			return (res, status) => {
				wrappedResult.result = res;
				wrappedResult.status = status;

				if (status.Error) Debug.Log(status.ErrorData.Information);
			};
		}
	}
}