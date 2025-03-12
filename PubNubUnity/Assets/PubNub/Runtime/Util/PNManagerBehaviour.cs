using UnityEngine;

namespace PubnubApi.Unity {
	[AddComponentMenu("PubNub/PubNub Manager")]
	[HelpURL("https://www.pubnub.com/docs/sdks/unity")]
	public class PNManagerBehaviour : MonoBehaviour {
		public PNConfigAsset pnConfiguration;

		public Pubnub pubnub { get; protected set; }

		public SubscribeCallbackListener listener { get; }
			= new SubscribeCallbackListener();

		/// <summary>
		/// Initializes a PubNub instance, and the associated event listener.
		/// </summary>
		/// <param name="userId">You can use one User ID to represent a user on all their devices, or use one User ID per client. If you allow a user to connect from multiple devices simultaneously, use the same User ID for each device, as PubNub features such as Presence, which determine's a user's online status, rely on User IDs.<br/><a href="https://www.pubnub.com/docs/general/setup/application-setup#user-ids">See documentation</a></param>
		/// <returns></returns>
		public Pubnub Initialize(string userId) {
			if (Application.isPlaying) {
				DontDestroyOnLoad(gameObject);
			}

			if (pnConfiguration is null) {
				Debug.LogError("PNConfigAsset is missing", this);
				return null;
			}

			if (pubnub is not null) {
				Debug.LogError("PubNub has already been initialized");
				return pubnub;
			}

			pnConfiguration.UserId = userId;
			var pnConfig = ((PNConfiguration)pnConfiguration);
			pubnub = pnConfiguration.EnableWebGLBuildMode
				? new Pubnub(pnConfig, httpTransportService: new UnityWebGLHttpClientService(),
					ipnsdkSource: new UnityPNSDKSource())
				: new Pubnub(pnConfig, ipnsdkSource: new UnityPNSDKSource());
			if (pnConfiguration.LogToUnityConsole) {
				pubnub.SetLogger(new UnityPubNubLogger(pubnub.InstanceId));
			}
			pubnub.SetJsonPluggableLibrary(new NewtonsoftJsonUnity(pnConfig));
			pubnub.AddListener(listener);
			return pubnub;
		}

		protected virtual void OnDestroy() {
			pubnub.UnsubscribeAll<string>();
			pubnub.UnsubscribeAll<object>();
		}

		protected virtual void OnDisable() {
			pubnub.UnsubscribeAll<string>();
			pubnub.UnsubscribeAll<object>();
		}

		public static implicit operator Pubnub(PNManagerBehaviour pn) {
			return pn.pubnub;
		}
	}
}