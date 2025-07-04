// snippet.grant_token_example
using System.Collections.Generic;
using PubnubApi;
using PubnubApi.Unity;
using UnityEngine;

public class GrantTokenExample : MonoBehaviour {
	//Reference to a pubnub manager previously setup in Unity Editor
	//For more details see https://www.pubnub.com/docs/sdks/unity#configure-pubnub
	//NOTE: For Access Management to work the keyset must have PAM enabled
	[SerializeField] private PNManagerBehaviour pubnubManager;

	//An editor-serialized string with the test channel ID
	[SerializeField] private string testChannelId = "test_channel_id";

	private async void Start() {
		//Getting a reference to the Pubnub instance
		var pubnub = pubnubManager.pubnub;

		// Note that you can also initialize Pubnub instance for Unity directly from code:
		/*
		PNConfiguration pnConfiguration = new PNConfiguration(new UserId("myUniqueUserId"))
		{
			SubscribeKey = "demo",
			PublishKey = "demo",
		};
		Pubnub pubnub = PubnubUnityUtils.NewUnityPubnub(pnConfiguration);
		*/

		//Creating a full access set for the sake of demonstration
		var fullAccess = new PNTokenAuthValues() {
			Read = true,
			Write = true,
			Create = true,
			Get = true,
			Delete = true,
			Join = true,
			Update = true,
			Manage = true
		};

		//Asynchronously executing a full-access grant operation for the test channel for user ID specified in the config
		//The $"{testChannelId}{Constants.Pnpres}" addition ensured full access to the channel with PubNub Presence operations
		var grantResult = await pubnub.GrantToken().TTL(30).AuthorizedUuid(pubnub.PNConfig.UserId).Resources(
				new PNTokenResources() {
					Channels = new Dictionary<string, PNTokenAuthValues>() {
						{
							testChannelId, fullAccess
						},
						{
							$"{testChannelId}{Constants.Pnpres}", fullAccess
						}
					}
				})
			.ExecuteAsync();

		//Doing simple error handling in case something went wrong during the grant operation
		if (grantResult.Status.Error) {
			Debug.LogError($"Error in grant operation: {grantResult.Status.ErrorData.Information}");
		}
	}
}
// snippet.end