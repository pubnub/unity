// snippet.using
using PubnubApi;
using PubnubApi.Unity;
using UnityEngine;

// snippet.end

public class GettingStartedSample {

	private static Pubnub pubnub;

	static void Init()
	{
		// snippet.pubnub_init
		// Configuration
		PNConfiguration pnConfiguration = new PNConfiguration(new UserId("myUniqueUserId"))
		{
			SubscribeKey = "demo",
			PublishKey = "demo",
			Secure = true
		};

		// Initialize PubNub
		Pubnub pubnub = PubnubUnityUtils.NewUnityPubnub(pnConfiguration);

		// If you're using Unity Editor setup you can get the Pubnub instance from PNManagerBehaviour
		// For more details, see https://www.pubnub.com/docs/sdks/unity#configure-pubnub
		/*
		[SerializeField] private PNManagerBehaviour pubnubManager;
		Pubnub pubnub = pubnubManager.pubnub;
		*/

		// snippet.end
	}

	static void InitProgramatically()
	{
		// snippet.init_code
		// Initialize PubNub using the configuration
		PNConfiguration pnConfiguration = new PNConfiguration(new UserId("myUniqueUserId"))
		{
			SubscribeKey = "demo",
			PublishKey = "demo",
			Secure = true
		};

		// Create the PubNub instance with the configuration
		Pubnub pubnub = PubnubUnityUtils.NewUnityPubnub(pnConfiguration);

		// You can also initializa a "raw" pubnub instance but would have to then setup all Unity-specific functionality by yourself
		Pubnub rawPubnub = new Pubnub(pnConfiguration);
		// snippet.end
	}

	static void AddListeners() {
		PNManagerBehaviour pnManagerBehaviour = null;

		// snippet.add_listeners
		// Get the listener from a PNManagerBehaviour:
		var listener = pnManagerBehaviour.listener;
		// Alternatively, create your own and add it to the Pubnub instance:
		var createdListener = new SubscribeCallbackListener();
		pubnub.AddListener(createdListener);

		// Add callbacks
		listener.onStatus += OnPnStatus;
		listener.onMessage += OnPnMessage;

		// Status event handler
		void OnPnStatus(Pubnub pn, PNStatus status) {
			Debug.Log(status.Category == PNStatusCategory.PNConnectedCategory ? "Connected" : "Not connected");
		}

		// Message event handler
		void OnPnMessage(Pubnub pn, PNMessageResult<object> result) {
			Debug.Log($"Message received: {result.Message}");
		}
		// snippet.end
	}

	static void CreateSubscription() {
		// snippet.create_subscription
		// Subscribe to a channel using modern API
		Channel channel = pubnub.Channel("TestChannel");
		Subscription subscription = channel.Subscription();
		subscription.Subscribe<object>();

		// Or using the legacy API
		pubnub.Subscribe<string>().Channels(new[] { "TestChannel" }).Execute();
		// snippet.end
	}

	static async void PublishMessages() {
		Transform transform = null;
		// snippet.publish_messages
		// Publish a simple message
		await pubnub.Publish().Channel("TestChannel").Message("Hello World from Unity!").ExecuteAsync();

		// Publish a Unity object using the GetJsonSafe extension method for handling circular references
		await pubnub.Publish().Channel("TestChannel").Message(transform.position.GetJsonSafe()).ExecuteAsync();

		// Using a callback instead of async/await
		pubnub.Publish()
			.Channel("TestChannel")
			.Message("Hello World from Unity!")
			.Execute((result, status) => {
				if (!status.Error) {
					Debug.Log("Message sent successfully!");
				} else {
					Debug.LogError("Failed to send message: " + status.ErrorData.Information);
				}
			});
		// snippet.end
	}
}
