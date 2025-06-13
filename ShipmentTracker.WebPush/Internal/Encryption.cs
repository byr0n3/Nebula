using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using ShipmentTracker.WebPush.Internal.Models;

namespace ShipmentTracker.WebPush.Internal
{
	internal static class Encryption
	{
		public static EncryptionResult Encrypt(PushSubscription subscription, DeclarativePushNotification notification, VapidOptions vapid)
		{
			Debug.Assert(vapid.IsValid);

			var salt = new byte[16];

			System.Random.Shared.NextBytes(salt);

			// @todo stackalloc
			var payload = JsonSerializer.SerializeToUtf8Bytes(notification,
															  WebPushJsonSerializerContext.Default.DeclarativePushNotification!);

			var userPublicKey = UrlSafeBase64.Decode(subscription.P256dh);
			var userPrivateKey = UrlSafeBase64.Decode(subscription.Auth);

			byte[] serverPublicKey;
			byte[] key;

			// @todo Fix The shared secret cannot be derived from the keying material
			using (var server = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256))
			using (var user = ECDiffieHellman.Create(GetEncryptionParameters(userPublicKey)))
			{
				server.GenerateKey(ECCurve.NamedCurves.nistP256);

				// @todo rent buffer
				serverPublicKey = server.ExportSubjectPublicKeyInfo();
				key = server.DeriveKeyFromHmac(user.PublicKey, HashAlgorithmName.SHA256, userPrivateKey);
			}

			// This works:
			/*var serverKeyPair = GenerateKeys();

			var ecdhAgreement = AgreementUtilities.GetBasicAgreement("ECDH");
			ecdhAgreement.Init(serverKeyPair.Private);

			var userPublicKeyMalware = GetPublicKey(userPublicKey);

			key = ecdhAgreement.CalculateAgreement(userPublicKeyMalware).ToByteArrayUnsigned();
			serverPublicKey = ((ECPublicKeyParameters)serverKeyPair.Public).Q.GetEncoded(false);*/

			var prk = Encryption.Hkdf(userPrivateKey, key, "Content-Encoding: auth\0"u8, 32);
			var cek = Encryption.Hkdf(salt, prk, CreateInfoChunk("aesgcm", userPublicKey, serverPublicKey), 16);
			var nonce = Encryption.Hkdf(salt, prk, CreateInfoChunk("nonce", userPublicKey, serverPublicKey), 12);

			var paddedPayload = Encryption.PadInput(payload);
			var encrypted = Encryption.EncryptPayload(paddedPayload, cek, nonce);

			return new EncryptionResult
			{
				Salt = salt,
				Payload = encrypted,
				PublicKey = serverPublicKey,
			};
		}

		private static byte[] EncryptPayload(scoped System.ReadOnlySpan<byte> payload,
											 scoped System.ReadOnlySpan<byte> cek,
											 scoped System.ReadOnlySpan<byte> nonce)
		{
			System.Span<byte> cipher = stackalloc byte[payload.Length];
			System.Span<byte> tag = stackalloc byte[AesGcm.TagByteSizes.MaxSize];

			using (var aes = new AesGcm(cek, AesGcm.TagByteSizes.MaxSize))
			{
				aes.Encrypt(nonce, payload, cipher, tag);
			}

			System.Span<byte> encryptedPayload = stackalloc byte[cipher.Length + tag.Length];

			var copied = cipher.TryCopyTo(encryptedPayload);
			Debug.Assert(copied);

			copied = tag.TryCopyTo(encryptedPayload.Slice(cipher.Length));
			Debug.Assert(copied);

			return encryptedPayload.ToArray();
		}

		public static AsymmetricCipherKeyPair GenerateKeys()
		{
			var ecParameters = NistNamedCurves.GetByName("P-256");
			var ecSpec = new ECDomainParameters(ecParameters.Curve, ecParameters.G, ecParameters.N, ecParameters.H,
												ecParameters.GetSeed());
			var keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator("ECDH");
			keyPairGenerator.Init(new ECKeyGenerationParameters(ecSpec, new SecureRandom()));

			return keyPairGenerator.GenerateKeyPair();
		}

		private static ECPublicKeyParameters GetPublicKey(byte[] publicKey)
		{
			Asn1Object keyTypeParameters = new DerSequence(new DerObjectIdentifier("1.2.840.10045.2.1"),
														   new DerObjectIdentifier("1.2.840.10045.3.1.7"));
			Asn1Object derEncodedKey = new DerBitString(publicKey);

			Asn1Object derSequence = new DerSequence(keyTypeParameters, derEncodedKey);

			var base64EncodedDerSequence = System.Convert.ToBase64String(derSequence.GetDerEncoded());
			var pemKey = "-----BEGIN PUBLIC KEY-----\n";
			pemKey += base64EncodedDerSequence;
			pemKey += "\n-----END PUBLIC KEY-----";

			var reader = new StringReader(pemKey);
			var pemReader = new PemReader(reader);
			var keyPair = pemReader.ReadObject();
			return (ECPublicKeyParameters)keyPair;
		}

		private static byte[] FormatServerPublicKey(byte[] key)
		{
			System.Span<byte> buffer = stackalloc byte[1 + (key.Length - 8)];

			buffer[0] = 0x04;
			var copied = System.MemoryExtensions.AsSpan(key, 8).TryCopyTo(buffer);
			Debug.Assert(copied);

			return buffer.ToArray();
		}

		public static ECParameters GetEncryptionParameters(string publicKey, string privateKey)
		{
			var decodedPublicKey = UrlSafeBase64.Decode(publicKey);
			var decodedPrivateKey = UrlSafeBase64.Decode(privateKey);

			return GetEncryptionParameters(decodedPublicKey, decodedPrivateKey);
		}

		public static ECParameters GetEncryptionParameters(byte[] decodedPublicKey, byte[] decodedPrivateKey)
		{
			return new ECParameters
			{
				Curve = ECCurve.NamedCurves.nistP256,
				D = decodedPrivateKey,
				Q = new ECPoint
				{
					X = System.MemoryExtensions.AsSpan(decodedPublicKey, 1, 32).ToArray(),
					Y = System.MemoryExtensions.AsSpan(decodedPublicKey, 33, 32).ToArray(),
				},
			};
		}

		public static ECParameters GetEncryptionParameters(string publicKey)
		{
			var decodedPublicKey = UrlSafeBase64.Decode(publicKey);

			return GetEncryptionParameters(decodedPublicKey);
		}

		public static ECParameters GetEncryptionParameters(scoped System.ReadOnlySpan<byte> decodedPublicKey)
		{
			return new ECParameters
			{
				Curve = ECCurve.NamedCurves.nistP256,
				Q = new ECPoint
				{
					X = decodedPublicKey.Slice(1, 32).ToArray(),
					Y = decodedPublicKey.Slice(33, 32).ToArray(),
				},
			};
		}

		private static byte[] PadInput(byte[] data)
		{
			var result = new byte[2 + data.Length];

			var copied = System.MemoryExtensions.AsSpan(data).TryCopyTo(System.MemoryExtensions.AsSpan(result, 2));
			Debug.Assert(copied);

			return result;
		}

		private static byte[] Hkdf(scoped System.ReadOnlySpan<byte> salt,
								   scoped System.ReadOnlySpan<byte> prk,
								   scoped System.ReadOnlySpan<byte> info,
								   int length)
		{
			System.Span<byte> dst = stackalloc byte[length];

			HKDF.DeriveKey(HashAlgorithmName.SHA256, prk, dst, salt, info);

			// @todo Unneeded copy
			return dst.ToArray();
		}

		// @todo
		[System.Obsolete("Optimize")]
		private static byte[] CreateInfoChunk(string type, byte[] recipientPublicKey, byte[] senderPublicKey)
		{
			var output = new List<byte>();

			output.AddRange(Encoding.UTF8.GetBytes($"Content-Encoding: {type}\0P-256\0"));
			output.AddRange(ConvertInt(recipientPublicKey.Length));
			output.AddRange(recipientPublicKey);
			output.AddRange(ConvertInt(senderPublicKey.Length));
			output.AddRange(senderPublicKey);

			return output.ToArray();
		}

		// @todo
		[System.Obsolete("Optimize")]
		private static byte[] ConvertInt(int number)
		{
			var output = System.BitConverter.GetBytes(System.Convert.ToUInt16(number));
			if (System.BitConverter.IsLittleEndian)
			{
				System.Array.Reverse(output);
			}

			return output;
		}
	}
}
