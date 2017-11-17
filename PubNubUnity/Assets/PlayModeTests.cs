using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using PubNubAPI;

namespace PubNubAPI.Tests
{
	public class PlayModeTests {

		
		public bool SslOn = false;
		public bool CipherOn = false;
		public bool AsObject = false;
		public static string Origin = "ps.pndsn.com";
        public static string PublishKey = "demo-36";
        public static string SubscribeKey = "demo-36";
        public static string SecretKey = "demo-36";

		[UnityTest]
		public IEnumerator TestTime() {
			PNConfiguration pnConfiguration = new PNConfiguration ();
            pnConfiguration.Origin = Origin;
            pnConfiguration.SubscribeKey = SubscribeKey;
            pnConfiguration.PublishKey = PublishKey;
            pnConfiguration.CipherKey = "enigma";
            pnConfiguration.LogVerbosity = PNLogVerbosity.BODY; 
            pnConfiguration.PresenceTimeout = 60;
            pnConfiguration.PresenceInterval= 30;

			PubNub pubnub = new PubNub(pnConfiguration);
			bool testReturn = false;
			pubnub.Time ().Async ((result, status) => {
                Assert.True(!status.Error);
				Assert.True(!result.TimeToken.Equals(0));
				testReturn = true;
            });
			yield return new WaitForSeconds (2);
			Assert.True(testReturn, "test didnt return");
		}
	}
}
