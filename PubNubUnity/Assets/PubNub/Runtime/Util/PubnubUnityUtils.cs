using PubnubApi.PNSDK;
using UnityEngine;

namespace PubnubApi.Unity {
	public static class PubnubUnityUtils {
		/// <summary>
		/// Creates a new Pubnub instance with Unity specific settings (like WebGL setup, logging, and JSON library)
		/// </summary>
		/// <param name="configuration">Pubnub configuration object</param>
		/// <param name="webGLBuildMode">Flag for enabling WebGL mode - sets httpTransportService to UnityWebGLHttpClientService</param>
		/// <param name="unityLogging">Flag to set Unity specific logger (UnityPubNubLogger)</param>
		/// <param name="customIpnsdkSource">Custom PNSDK source, used for analytics and debugging.</param>
		/// <param name="customIHttpClientService">Custom IHttpClientService internal transport layer to be used by the Pubnub instance.</param>
		/// <returns>A new initialized Pubnub instance optimized for Unity usage.</returns>
		public static Pubnub NewUnityPubnub(PNConfiguration configuration, bool webGLBuildMode = false,
			bool unityLogging = false, IPNSDKSource customIpnsdkSource = null,
			IHttpClientService customIHttpClientService = null)
		{
			Pubnub pubnub;
			var ipnsdkSource = customIpnsdkSource ?? new UnityPNSDKSource();
			//With a custom transport layer
			if (customIHttpClientService != null) {
				pubnub = new Pubnub(configuration, httpTransportService: customIHttpClientService,
					ipnsdkSource: ipnsdkSource);
			}
			//With the WebGL transport layer
			else if (webGLBuildMode) {
				pubnub = new Pubnub(configuration, httpTransportService: new UnityWebGLHttpClientService(),
					ipnsdkSource: ipnsdkSource);
			}
			//With the Unity Http/2 supported transport layer
			else if (configuration.EnableHttp2) {
#if UNITY_6000_5_OR_NEWER
				var handler = new UnityEngine.Networking.UnityHttpMessageHandler()
				{
					HttpForcedVersion = UnityEngine.Networking.HttpForcedVersion.NotForced
				};
				var transport = new HttpClientService(handler, enableHttp2: true);
				pubnub = new Pubnub(configuration, httpTransportService: transport, ipnsdkSource: new UnityPNSDKSource());
				pubnub.SetJsonPluggableLibrary(new NewtonsoftJsonUnity(configuration));
#else
				Debug.LogWarning(
					"Project version is below Unity 6.5 so HTTP/2 support can only be enabled via a third-party HTTP handler, " +
					"see documentation for more information. This created Pubnub instance will have the default non-HTTP/2 supporting transport. " +
					"If you don't require HTTP/2 and don't want to see this warning set \"EnableHttp2\" to false in the Pubnub config.");
				pubnub = new Pubnub(configuration, ipnsdkSource: ipnsdkSource);
#endif
			}
			//Default
			else {
				pubnub = new Pubnub(configuration, ipnsdkSource: ipnsdkSource);
			}

			pubnub.SetJsonPluggableLibrary(new NewtonsoftJsonUnity(configuration));
			if (unityLogging) {
				pubnub.SetLogger(new UnityPubNubLogger(pubnub.InstanceId));
			}

			return pubnub;
		}

		/// <summary>
		/// Creates a new Pubnub instance with Unity specific settings (like WebGL setup, logging, and JSON library)
		/// </summary>
		/// <param name="configurationAsset">Pubnub configuration Scriptable Object asset</param>
		/// <param name="userId">Client user ID for this instance</param>
		/// <param name="ipnsdkSource">Optional: PNSDK source, used for analytics and debugging.</param>
		/// <returns>A new initialized Pubnub instance.</returns>
		public static Pubnub NewUnityPubnub(PNConfigAsset configurationAsset, string userId,
			IPNSDKSource ipnsdkSource = null) {
			configurationAsset.UserId = userId;
			var pnConfig = ((PNConfiguration)configurationAsset);
			return NewUnityPubnub(pnConfig, configurationAsset.EnableWebGLBuildMode,
				configurationAsset.LogToUnityConsole, customIpnsdkSource: ipnsdkSource);
		}
	}
}