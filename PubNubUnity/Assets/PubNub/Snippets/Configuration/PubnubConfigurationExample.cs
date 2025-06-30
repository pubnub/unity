// snippet.configuration_basic_usage
using System.Collections.Generic;
using PubnubApi;
using PubnubApi.Security.Crypto;
using PubnubApi.Security.Crypto.Cryptors;
using PubnubApi.Unity;
using UnityEngine;

public class PubnubConfigurationExample : MonoBehaviour {
    // Serialized fields to allow configuration within Unity Editor
    [SerializeField] private string userId = "myUniqueUserId";
    [SerializeField] private string subscribeKey = "demo"; // Replace with your actual SubscribeKey
    [SerializeField] private string publishKey = "demo"; // Replace with your actual PublishKey if publishing is needed
    [SerializeField] private string secretKey = "yourSecretKey"; // Used if Access Manager operations are needed
    [SerializeField] private string authKey = "authKey"; // Used if Access Manager is enabled
    [SerializeField] private string filterExpression = "such=wow";
    [SerializeField] private bool useSSL = true;
    [SerializeField] private bool logToUnityConsole = true;

    //Note that you can always use the PnConfigAsset Scriptable Object for setting these values in editor
    [SerializeField] private PNConfigAsset configAsset;

    private void Start() {
        // Initialize a PNConfiguration object with the provided values
        PNConfiguration pnConfiguration = new PNConfiguration(new UserId(userId)) {
            SubscribeKey = subscribeKey,
            PublishKey = publishKey,
            SecretKey = secretKey,
            AuthKey = authKey,
            Secure = useSSL,
            CryptoModule = new CryptoModule(new AesCbcCryptor("enigma"), new List<ICryptor> { new LegacyCryptor("enigma") }),
            LogLevel = PubnubLogLevel.All,
            SubscribeTimeout = 310,
            NonSubscribeRequestTimeout = 300,
            FilterExpression = filterExpression,
            HeartbeatNotificationOption = PNHeartbeatNotificationOption.All
        };

        pnConfiguration.SetPresenceTimeoutWithCustomInterval(120, 59);
        pnConfiguration.PresenceTimeout = 120;

        // Create a PubNub instance and perform Unity-specific setup
        Pubnub pubnub = PubnubUnityUtils.NewUnityPubnub(pnConfiguration, unityLogging: logToUnityConsole);

        // Analogous setup with the Scriptable Object config:
        Pubnub anotherPubnub = PubnubUnityUtils.NewUnityPubnub(configAsset);

        Debug.Log("PubNub configured and ready to use.");
    }
}
// snippet.end