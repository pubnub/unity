using System;
using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

namespace PubnubApi.Unity.Tests {
	public class PNTestBase {
		protected static Pubnub pn;
		protected static SubscribeCallbackListener listener = new();

		[OneTimeSetUp]
		public void OneTimeSetUp() {
			PNConfiguration pnConfiguration = new PNConfiguration(new UserId(System.Guid.NewGuid().ToString())) {
				PublishKey = System.Environment.GetEnvironmentVariable("PUB_KEY"),
				SubscribeKey = System.Environment.GetEnvironmentVariable("SUB_KEY"),
				SecretKey = System.Environment.GetEnvironmentVariable("PAM_SECRET_KEY")
			};
			pn = new Pubnub(pnConfiguration);

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