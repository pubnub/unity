// snippet.cleanup_basic_usage
using PubnubApi;
using UnityEngine;

public class PubnubCleanupExample : MonoBehaviour {
	private void OnApplicationQuit() {
		// Performing cleanup operations
		Debug.Log("Cleaning up PubNub resources...");
		Pubnub.CleanUp();
		Debug.Log("Cleanup complete.");
	}
}
// snippet.end