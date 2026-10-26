using System;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using PubnubApi.Security.Crypto;
using PubnubApi.Security.Crypto.Cryptors;

namespace PubnubApi.Unity.Tests {
	/// <summary>
	/// Checks that the decrypt-only AES-GCM ("ACGM") cryptor from the bundled PubnubApiUnity.dll is usable
	/// inside Unity, and that the BouncyCastle library merged into that DLL stays internal.
	/// No network access is needed.
	/// </summary>
	public class AesGcmCryptoModule {
		private const string CipherKey = "enigma";
		private const string PlainText = "\"Hello Unity from AES-GCM!\"";

		// ACGM payload for PlainText, generated outside Unity with System.Security.Cryptography.AesGcm:
		// "PNED" | 0x01 | "ACGM" | 0x1C | nonce (12) | tag (16) | cipher text, key = SHA256("enigma").
		private const string AcgmPayload =
			"UE5FRAFBQ0dNHBEiM0RVZneImaq7zMgfX6w00dhbfdXplqx/4gQwsQv7VgD7QzMrUmRAcfdtHoCVL0ZK0ET03q8=";

		private const int CipherTextOffset = 10 + 12 + 16;
		private const string BouncyCastleNamespace = "Org.BouncyCastle";

		private static Assembly SdkAssembly => typeof(Pubnub).Assembly;

		#region Public API availability

		[Test]
		public void AesGcmCryptorIsPublicCryptor() {
			Type type = typeof(AesGcmCryptor);

			Assert.IsTrue(type.IsPublic, "AesGcmCryptor should be public");
			Assert.IsTrue(typeof(ICryptor).IsAssignableFrom(type), "AesGcmCryptor should implement ICryptor");
			Assert.IsNotNull(type.GetConstructor(new[] { typeof(string) }),
				"AesGcmCryptor(string cipherKey) constructor should be available");
			Assert.IsNotNull(type.GetConstructor(new[] { typeof(string), typeof(PubnubLogModule) }),
				"AesGcmCryptor(string cipherKey, PubnubLogModule logger) constructor should be available");
		}

		[Test]
		public void CryptoModuleExposesAesGcmFactories() {
			AesGcmCryptor fromKey = CryptoModule.CreateAesGcmCryptor(CipherKey);
			AesGcmCryptor fromKeyAndLogger = CryptoModule.CreateAesGcmCryptor(CipherKey, null);

			Assert.AreEqual("ACGM", fromKey.Identifier);
			Assert.AreEqual("ACGM", fromKeyAndLogger.Identifier);
			Assert.AreEqual(CipherKey, fromKey.CipherKey);
		}

		[Test]
		public void InternalCipherKeyHelperIsNotPublic() {
			MethodInfo helper = typeof(CryptoModule).GetMethod("CreateFromCipherKey",
				BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			Assert.IsNotNull(helper, "CreateFromCipherKey should exist in the bundled DLL");
			Assert.IsFalse(helper.IsPublic, "CreateFromCipherKey is an internal SDK helper and must not be public");
		}

		[Test]
		public void AesGcmCryptorPublicApiDoesNotExposeBouncyCastleTypes() {
			var publicMembers = typeof(AesGcmCryptor).GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			foreach (MemberInfo member in publicMembers) {
				foreach (Type type in SignatureTypes(member)) {
					Assert.IsFalse(IsBouncyCastleType(type),
						$"Public member {member.Name} exposes BouncyCastle type {type.FullName}");
				}
			}
		}

		#endregion

		#region Bundled BouncyCastle stays internal

		[Test]
		public void SdkDllDoesNotReferenceSeparateBouncyCastleAssembly() {
			var references = SdkAssembly.GetReferencedAssemblies().Select(a => a.Name).ToList();

			CollectionAssert.DoesNotContain(references, "BouncyCastle.Cryptography",
				"PubnubApiUnity.dll should have BouncyCastle merged in, not reference it as a separate DLL");
		}

		[Test]
		public void BouncyCastleIsMergedIntoSdkDll() {
			Type gcmCipher = SdkAssembly.GetType("Org.BouncyCastle.Crypto.Modes.GcmBlockCipher");
			Type aesUtilities = SdkAssembly.GetType("Org.BouncyCastle.Crypto.AesUtilities");

			Assert.IsNotNull(gcmCipher, "GcmBlockCipher should be merged into PubnubApiUnity.dll");
			Assert.IsNotNull(aesUtilities, "AesUtilities should be merged into PubnubApiUnity.dll");
		}

		[Test]
		public void BouncyCastleTypesAreNotPubliclyVisible() {
			var exportedBouncyCastleTypes = SdkAssembly.GetExportedTypes().Where(IsBouncyCastleType).ToList();
			Assert.IsEmpty(exportedBouncyCastleTypes,
				"No BouncyCastle type should be exported: " +
				string.Join(", ", exportedBouncyCastleTypes.Take(5).Select(t => t.FullName)));

			Type gcmCipher = SdkAssembly.GetType("Org.BouncyCastle.Crypto.Modes.GcmBlockCipher");
			Assert.IsNotNull(gcmCipher);
			Assert.IsFalse(gcmCipher.IsVisible, "Merged BouncyCastle types should be internal");
		}

		[Test]
		public void BouncyCastleIsNotResolvableAsSeparateAssembly() {
			Type standalone = Type.GetType("Org.BouncyCastle.Crypto.Modes.GcmBlockCipher, BouncyCastle.Cryptography");

			Assert.IsNull(standalone, "The SDK should not require (or bring along) a separate BouncyCastle.Cryptography assembly");
		}

		#endregion

		#region Decryption works inside Unity

		[Test]
		public void AesGcmCryptorDecryptsString() {
			Assert.AreEqual(PlainText, new AesGcmCryptor(CipherKey).Decrypt(AcgmPayload));
		}

		[Test]
		public void AesGcmCryptorDecryptsBytes() {
			byte[] decrypted = new AesGcmCryptor(CipherKey).Decrypt(Convert.FromBase64String(AcgmPayload));

			CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(PlainText), decrypted);
		}

		[Test]
		public void CryptoModuleRoutesAcgmAlongsideOtherCryptors() {
			var module = new CryptoModule(new AesCbcCryptor(CipherKey),
				new ICryptor[] { new LegacyCryptor(CipherKey), new AesGcmCryptor(CipherKey) });

			Assert.AreEqual(PlainText, module.Decrypt(AcgmPayload));
			Assert.AreEqual("yay!", module.Decrypt(module.Encrypt("yay!")));
		}

		[Test]
		public void CipherKeyOnlyConfigurationDecryptsAcgm() {
			var config = new PNConfiguration(new UserId(Guid.NewGuid().ToString())) {
				PublishKey = "demo-36",
				SubscribeKey = "demo-36"
			};
#pragma warning disable CS0618
			config.CipherKey = CipherKey;
#pragma warning restore CS0618
			var pubnub = new Pubnub(config);
			try {
				Assert.AreEqual(PlainText, pubnub.Decrypt(AcgmPayload));
			} finally {
				pubnub.Destroy();
			}
		}

		[Test]
		public void TamperedAcgmPayloadFailsWithGenericError() {
			byte[] tampered = Convert.FromBase64String(AcgmPayload);
			tampered[CipherTextOffset] ^= 0x01;

			var ex = Assert.Throws<PNException>(() => new AesGcmCryptor(CipherKey).Decrypt(tampered));
			Assert.AreEqual("Decrypt Error", ex.Message);

			var wrongKey = Assert.Throws<PNException>(() => new AesGcmCryptor("wrong-key").Decrypt(AcgmPayload));
			Assert.AreEqual("Decrypt Error", wrongKey.Message);
		}

		#endregion

		#region Decrypt-only restrictions

		[Test]
		public void AesGcmEncryptionIsNotSupported() {
			var cryptor = new AesGcmCryptor(CipherKey);

			Assert.Throws<NotSupportedException>(() => cryptor.Encrypt("yay!"));
			Assert.Throws<NotSupportedException>(() => cryptor.Encrypt(new byte[] { 1, 2, 3 }));
			Assert.Throws<NotSupportedException>(() => cryptor.EncryptFile("source", "destination"));
		}

		[Test]
		public void AesGcmCryptorCannotBeDefaultCryptor() {
			Assert.Throws<ArgumentException>(() => new CryptoModule(new AesGcmCryptor(CipherKey), null));
		}

		#endregion

		private static bool IsBouncyCastleType(Type type) {
			return type?.Namespace != null && type.Namespace.StartsWith(BouncyCastleNamespace, StringComparison.Ordinal);
		}

		private static Type[] SignatureTypes(MemberInfo member) {
			switch (member) {
				case MethodBase method:
					var types = method.GetParameters().Select(p => p.ParameterType).ToList();
					if (method is MethodInfo methodInfo) {
						types.Add(methodInfo.ReturnType);
					}
					return types.ToArray();
				case PropertyInfo property:
					return new[] { property.PropertyType };
				case FieldInfo field:
					return new[] { field.FieldType };
				case EventInfo eventInfo:
					return new[] { eventInfo.EventHandlerType };
				default:
					return Array.Empty<Type>();
			}
		}
	}
}
