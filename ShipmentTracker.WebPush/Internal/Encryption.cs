using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;
using Elegance.Utilities;
using JetBrains.Annotations;
using ShipmentTracker.WebPush.Internal.Models;

namespace ShipmentTracker.WebPush.Internal
{
	internal static partial class Encryption
	{
		[MustDisposeResource]
		public static EncryptionResult Encrypt(PushSubscription subscription, DeclarativePushNotification notification, VapidOptions vapid)
		{
			Debug.Assert(vapid.IsValid);

			var salt = new RentedArray<byte>(16);

			System.Random.Shared.NextBytes(salt);

			// @todo stackalloc
			var payload = JsonSerializer.SerializeToUtf8Bytes(notification,
															  WebPushJsonSerializerContext.Default.DeclarativePushNotification!);

			var userPublicKey = UrlSafeBase64.Decode(subscription.P256dh);
			var userPrivateKey = UrlSafeBase64.Decode(subscription.Auth);

			RentedArray<byte> serverPublicKey;
			byte[] key;

			using (var server = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256))
			using (var user = ECDiffieHellman.Create(Encryption.GetEncryptionParameters(userPublicKey)))
			{
				server.GenerateKey(ECCurve.NamedCurves.nistP256);

				serverPublicKey = Encryption.ExportRawPublicKey(server.PublicKey);
				key = server.DeriveRawSecretAgreement(user.PublicKey);
			}

			System.Span<byte> prk = stackalloc byte[32];
			System.Span<byte> cek = stackalloc byte[16];
			System.Span<byte> nonce = stackalloc byte[12];

			Encryption.HkdfPrk(prk, userPrivateKey, key);
			Encryption.HkdfCek(cek, salt, prk, userPublicKey, serverPublicKey);
			Encryption.HkdfNonce(nonce, salt, prk, userPublicKey, serverPublicKey);

			System.Span<byte> paddedPayload = stackalloc byte[2 + payload.Length];

			Encryption.PadInput(payload, paddedPayload);
			var encrypted = Encryption.EncryptPayload(paddedPayload, cek, nonce);

			return new EncryptionResult
			{
				Salt = salt,
				Payload = encrypted,
				PublicKey = serverPublicKey,
			};
		}

		private static RentedArray<byte> ExportRawPublicKey(ECDiffieHellmanPublicKey key)
		{
			var parameters = key.ExportParameters();

			var x = parameters.Q.X;
			var y = parameters.Q.Y;

			Debug.Assert(x is not null);
			Debug.Assert(y is not null);

			var buffer = new RentedArray<byte>(1 + x.Length + y.Length);

			buffer[0] = 0x04;
			System.MemoryExtensions.AsSpan(x).TryCopyTo(buffer.Slice(1));
			System.MemoryExtensions.AsSpan(y).TryCopyTo(buffer.Slice(1 + x.Length));

			return buffer;
		}

		private static RentedArray<byte> EncryptPayload(scoped System.ReadOnlySpan<byte> payload,
														scoped System.ReadOnlySpan<byte> cek,
														scoped System.ReadOnlySpan<byte> nonce)
		{
			System.Span<byte> cipher = stackalloc byte[payload.Length];
			System.Span<byte> tag = stackalloc byte[AesGcm.TagByteSizes.MaxSize];

			using (var aes = new AesGcm(cek, AesGcm.TagByteSizes.MaxSize))
			{
				aes.Encrypt(nonce, payload, cipher, tag);
			}

			var encryptedPayload = new RentedArray<byte>(cipher.Length + tag.Length);

			var copied = cipher.TryCopyTo(encryptedPayload);
			Debug.Assert(copied);

			copied = tag.TryCopyTo(encryptedPayload.Slice(cipher.Length));
			Debug.Assert(copied);

			return encryptedPayload;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void PadInput(scoped System.ReadOnlySpan<byte> src, scoped System.Span<byte> dst)
		{
			var copied = src.TryCopyTo(dst.Slice(2));
			Debug.Assert(copied);
		}

		public static ECParameters GetEncryptionParameters(string publicKey, string privateKey)
		{
			var decodedPublicKey = UrlSafeBase64.Decode(publicKey);
			var decodedPrivateKey = UrlSafeBase64.Decode(privateKey);

			return Encryption.GetEncryptionParameters(decodedPublicKey, decodedPrivateKey);
		}

		private static ECParameters GetEncryptionParameters(scoped System.ReadOnlySpan<byte> decodedPublicKey, byte[] decodedPrivateKey)
		{
			return new ECParameters
			{
				Curve = ECCurve.NamedCurves.nistP256,
				D = decodedPrivateKey,
				Q = new ECPoint
				{
					X = decodedPublicKey.Slice(1, 32).ToArray(),
					Y = decodedPublicKey.Slice(33, 32).ToArray(),
				},
			};
		}

		private static ECParameters GetEncryptionParameters(scoped System.ReadOnlySpan<byte> decodedPublicKey)
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
	}
}
