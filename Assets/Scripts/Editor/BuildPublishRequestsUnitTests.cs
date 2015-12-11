using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Text;

namespace PubNubMessaging.Tests
{
	[TestFixture]
	public class BuildPublishRequestsUnitTests
	{
		#if DEBUG	
		[Test]
		public void TestBuildPublishRequest ()
		{
			TestBuildPublishRequestCommon (false, true, "", "", "");
		}

		[Test]
		public void TestBuildPublishRequestSSL ()
		{
			TestBuildPublishRequestCommon (true, true, "", "", "");
		}

		[Test]
		public void TestBuildPublishRequestNoStore ()
		{
			TestBuildPublishRequestCommon (false, false, "", "", "");
		}

		[Test]
		public void TestBuildPublishRequestSSLNoStore ()
		{
			TestBuildPublishRequestCommon (true, false, "", "", "");
		}

		[Test]
		public void TestBuildPublishRequestCipherNoStore ()
		{
			TestBuildPublishRequestCommon (false, false, "", "enigma", "");
		}

		[Test]
		public void TestBuildPublishRequestCipherSSLNoStore ()
		{
			TestBuildPublishRequestCommon (true, false, "", "enigma", "");
		}

		[Test]
		public void TestBuildPublishRequestCipherSSL ()
		{
			TestBuildPublishRequestCommon (true, true, "", "enigma", "");
		}

		[Test]
		public void TestBuildPublishRequestCipher ()
		{
			TestBuildPublishRequestCommon (false, true, "", "enigma", "");
		}

		[Test]
		public void TestBuildPublishRequestSecret ()
		{
			TestBuildPublishRequestCommon (false, true, "Secret", "", "");
		}

		[Test]
		public void TestBuildPublishRequestSSLSecret ()
		{
			TestBuildPublishRequestCommon (true, true, "Secret", "", "");
		}

		[Test]
		public void TestBuildPublishRequestNoStoreSecret ()
		{
			TestBuildPublishRequestCommon (false, false, "Secret", "", "");
		}

		[Test]
		public void TestBuildPublishRequestSSLNoStoreSecret ()
		{
			TestBuildPublishRequestCommon (true, false, "Secret", "", "");
		}

		[Test]
		public void TestBuildPublishRequestCipherNoStoreSecret ()
		{
			TestBuildPublishRequestCommon (false, false, "Secret", "enigma", "");
		}

		[Test]
		public void TestBuildPublishRequestCipherSSLNoStoreSecret ()
		{
			TestBuildPublishRequestCommon (true, false, "Secret", "enigma", "");
		}

		[Test]
		public void TestBuildPublishRequestCipherSSLSecret ()
		{
			TestBuildPublishRequestCommon (true, true, "Secret", "enigma", "");
		}

		[Test]
		public void TestBuildPublishRequestCipherSecret ()
		{
			TestBuildPublishRequestCommon (false, true, "Secret", "enigma", "");
		}

		[Test]
		public void TestBuildPublishRequestAuth ()
		{
			TestBuildPublishRequestCommon (false, true, "", "", "authKey");
		}

		[Test]
		public void TestBuildPublishRequestSSLAuth ()
		{
			TestBuildPublishRequestCommon (true, true, "", "", "authKey");
		}

		[Test]
		public void TestBuildPublishRequestNoStoreAuth ()
		{
			TestBuildPublishRequestCommon (false, false, "", "", "authKey");
		}

		[Test]
		public void TestBuildPublishRequestSSLNoStoreAuth ()
		{
			TestBuildPublishRequestCommon (true, false, "", "", "authKey");
		}

		[Test]
		public void TestBuildPublishRequestCipherNoStoreAuth ()
		{
			TestBuildPublishRequestCommon (false, false, "", "enigma", "authKey");
		}

		[Test]
		public void TestBuildPublishRequestCipherSSLNoStoreAuth ()
		{
			TestBuildPublishRequestCommon (true, false, "", "enigma", "authKey");
		}

		[Test]
		public void TestBuildPublishRequestCipherSSLAuth ()
		{
			TestBuildPublishRequestCommon (true, true, "", "enigma", "authKey");
		}

		[Test]
		public void TestBuildPublishRequestCipherAuth ()
		{
			TestBuildPublishRequestCommon (false, true, "", "enigma", "authKey");
		}

		[Test]
		public void TestBuildPublishRequestSecretAuth ()
		{
			TestBuildPublishRequestCommon (false, true, "Secret", "", "authKey");
		}

		[Test]
		public void TestBuildPublishRequestSSLSecretAuth ()
		{
			TestBuildPublishRequestCommon (true, true, "Secret", "", "authKey");
		}

		[Test]
		public void TestBuildPublishRequestNoStoreSecretAuth ()
		{
			TestBuildPublishRequestCommon (false, false, "Secret", "", "authKey");
		}

		[Test]
		public void TestBuildPublishRequestSSLNoStoreSecretAuth ()
		{
			TestBuildPublishRequestCommon (true, false, "Secret", "", "authKey");
		}

		[Test]
		public void TestBuildPublishRequestCipherNoStoreSecretAuth ()
		{
			TestBuildPublishRequestCommon (false, false, "Secret", "enigma", "authKey");
		}

		[Test]
		public void TestBuildPublishRequestCipherSSLNoStoreSecretAuth ()
		{
			TestBuildPublishRequestCommon (true, false, "Secret", "enigma", "authKey");
		}

		[Test]
		public void TestBuildPublishRequestCipherSSLSecretAuth ()
		{
			TestBuildPublishRequestCommon (true, true, "Secret", "enigma", "authKey");
		}

		[Test]
		public void TestBuildPublishRequestCipherSecretAuth ()
		{
			TestBuildPublishRequestCommon (false, true, "Secret", "enigma", "authKey");
		}

		public void TestBuildPublishRequestCommon(bool ssl, bool storeInHistory, string secretKey, 
			string cipherKey, string authKey){

			string channel = "publish_channel";
			string message = "Test message";
			string uuid = "customuuid";
			string signature = "0";
			Pubnub pubnub = new Pubnub (
				Common.PublishKey,
				Common.SubscribeKey,
				secretKey,
				cipherKey,
				ssl
			);
			pubnub.AuthenticationKey = authKey;
			string authKeyString = "";
			if (!string.IsNullOrEmpty(authKey)) {
				authKeyString = string.Format ("&auth={0}", pubnub.AuthenticationKey);
			}
			string originalMessage = Helpers.JsonEncodePublishMsg (message, cipherKey, pubnub.JsonPluggableLibrary);
			if (secretKey.Length > 0) {
				StringBuilder stringToSign = new StringBuilder ();
				stringToSign
					.Append (Common.PublishKey)
					.Append ('/')
					.Append (Common.SubscribeKey)
					.Append ('/')
					.Append (secretKey)
					.Append ('/')
					.Append (channel)
					.Append ('/')
					.Append (originalMessage); // 1

				// Sign Message
				signature = Utility.Md5 (stringToSign.ToString ());
			}

			Uri uri = BuildRequests.BuildPublishRequest (channel, originalMessage, storeInHistory, uuid, ssl, 
				pubnub.Origin, pubnub.AuthenticationKey, Common.PublishKey, Common.SubscribeKey,
				cipherKey, secretKey
			);

			//http://pubsub.pubnub.com/publish/demo-36/demo-36/0/publish_channel/0?uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX/3.6.9.0
			string expected = string.Format ("http{0}://{1}/publish/{2}/{3}/{4}/{5}/0/{6}?uuid={7}{8}{9}&pnsdk={10}",
				ssl?"s":"", pubnub.Origin, Common.PublishKey, Common.SubscribeKey, signature, channel, originalMessage, 
				uuid, storeInHistory?"":"&store=0", authKeyString, PubnubUnity.Version
			);
			string received = uri.ToString ();
			Common.LogAndCompare (expected, received);
		}

		#endif
	}
}

