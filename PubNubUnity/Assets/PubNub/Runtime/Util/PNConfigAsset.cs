using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PubnubApi.Unity {
	[CreateAssetMenu(fileName = "PNConfigAsset", menuName = "PubNub/PubNub Config Asset")]
	public class PNConfigAsset : ScriptableObject {
		public string PublishKey = "demo";
		public string SubscribeKey = "demo";
		public string SecretKey = "";
		public string AuthKey;
		public string CipherKey;
		public bool Secure;
		public bool EnableEventEngine = true;
		public bool EnableWebGLBuildMode;
		public bool LogToUnityConsole;
		public PNLogVerbosity LogVerbosity;

		[SerializeField] private bool externalJsonEnabled = false;
		[SerializeField] private UnityEngine.TextAsset externalJsonFile;

		[System.NonSerialized] public string UserId;

		public static implicit operator PNConfiguration(PNConfigAsset asset) {
			if (string.IsNullOrEmpty(asset.UserId)) {
				throw new NullReferenceException("You need to set the UserId before passing configuration");
			}
			var config = new PNConfiguration(new UserId(new UserId(asset.UserId)));
			config.SubscribeKey = asset.SubscribeKey;
			config.PublishKey = asset.PublishKey;
			config.SecretKey = asset.SecretKey;
			config.AuthKey = asset.AuthKey;
			config.CipherKey = asset.CipherKey;
			config.EnableEventEngine = asset.EnableEventEngine;
			config.Secure = asset.Secure;
			config.LogVerbosity = asset.LogVerbosity;
			return config;
		}
	}
}