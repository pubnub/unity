namespace PubnubApi.Unity {
	public static class PubnubUnityUtils {
		/// <summary>
		/// Creates a new Pubnub instance with Unity specific settings (like WebGL setup, logging, and JSON library)
		/// </summary>
		/// <param name="configuration">Pubnub configuration object</param>
		/// <param name="webGLBuildMode">Flag for enabling WebGL mode - sets httpTransportService to UnityWebGLHttpClientService</param>
		/// <param name="unityLogging">Flag to set Unity specific logger (UnityPubNubLogger)</param>
		/// <returns></returns>
		public static Pubnub NewUnityPubnub(PNConfiguration configuration, bool webGLBuildMode = false, bool unityLogging = false) {
			var pubnub = webGLBuildMode
				? new Pubnub(configuration, httpTransportService: new UnityWebGLHttpClientService(),
					ipnsdkSource: new UnityPNSDKSource())
				: new Pubnub(configuration, ipnsdkSource: new UnityPNSDKSource());
			if (unityLogging) {
				pubnub.SetLogger(new UnityPubNubLogger(pubnub.InstanceId));
			}
			pubnub.SetJsonPluggableLibrary(new NewtonsoftJsonUnity(configuration));
			return pubnub;
		}

		/// <summary>
		/// Creates a new Pubnub instance with Unity specific settings (like WebGL setup, logging, and JSON library)
		/// </summary>
		/// <param name="configurationAsset">Pubnub configuration Scriptable Object asset</param>
		/// <param name="userId">Client user ID</param>
		/// <returns></returns>
		public static Pubnub NewUnityPubnub(PNConfigAsset configurationAsset, string userId) {
			configurationAsset.UserId = userId;
			var pnConfig = ((PNConfiguration)configurationAsset);
			return NewUnityPubnub(pnConfig, configurationAsset.EnableWebGLBuildMode, configurationAsset.LogToUnityConsole);
		}
	}
}