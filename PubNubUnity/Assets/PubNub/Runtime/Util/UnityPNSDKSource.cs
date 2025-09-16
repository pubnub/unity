using System;
using PubnubApi.PNSDK;

namespace PubnubApi.Unity
{
    public class UnityPNSDKSource : IPNSDKSource {

	    private const string build = "9.1.0";
	    public string Build => build;

	    public string GetPNSDK() {
			#if(UNITY_IOS)
			        return string.Format("PubNub-CSharp-UnityIOS/{0}", build);
			#elif(UNITY_STANDALONE_WIN)
					return string.Format("PubNub-CSharp-UnityWin/{0}", build);
			#elif(UNITY_STANDALONE_OSX)
			        return string.Format("PubNub-CSharp-UnityOSX/{0}", build);
			#elif(UNITY_ANDROID)
			        return string.Format("PubNub-CSharp-UnityAndroid/{0}", build);
			#elif(UNITY_STANDALONE_LINUX)
			        return string.Format("PubNub-CSharp-UnityLinux/{0}", build);
			#elif(UNITY_WEBPLAYER)
			        return string.Format("PubNub-CSharp-UnityWeb/{0}", build);
			#elif(UNITY_WEBGL)
					return string.Format("PubNub-CSharp-UnityWebGL/{0}", build);
			#else
			        return string.Format("PubNub-CSharp-Unity/{0}", build);
			#endif
	    }
    }
}
