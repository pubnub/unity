using System;
using PubnubApi.PNSDK;

namespace PubnubApi.Unity
{
    public class UnityPNSDKSource : IPNSDKSource
    {
	    public string GetPNSDK() {
			#if(UNITY_IOS)
			        Version = string.Format("PubNub-CSharp-UnityIOS/{0}", build);
			#elif(UNITY_STANDALONE_WIN)
			        Version = string.Format("PubNub-CSharp-UnityWin/{0}", build);
			#elif(UNITY_STANDALONE_OSX)
			        Version = string.Format("PubNub-CSharp-UnityOSX/{0}", build);
			#elif(UNITY_ANDROID)
			        Version = string.Format("PubNub-CSharp-UnityAndroid/{0}", build);
			#elif(UNITY_STANDALONE_LINUX)
			        Version = string.Format("PubNub-CSharp-UnityLinux/{0}", build);
			#elif(UNITY_WEBPLAYER)
			        Version = string.Format("PubNub-CSharp-UnityWeb/{0}", build);
			#elif(UNITY_WEBGL)
					Version = string.Format("PubNub-CSharp-UnityWebGL/{0}", build);
			#else
			        Version = string.Format("PubNub-CSharp-Unity/{0}", build);
			#endif
	    }
    }
}
