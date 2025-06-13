using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ShipmentTracker.WebPush.Internal.Models;

namespace ShipmentTracker.WebPush.Internal
{
	internal static class JwtSigner
	{
		public static string Sign(string publicKey, string privateKey, JwtInfo info, JwtData data)
		{
			// @todo stackalloc
			var infoBytes = JsonSerializer.SerializeToUtf8Bytes(info, WebPushJsonSerializerContext.Default.JwtInfo!);
			var dataBytes = JsonSerializer.SerializeToUtf8Bytes(data, WebPushJsonSerializerContext.Default.JwtData!);

			var infoBase64 = UrlSafeBase64.Encode(infoBytes);
			var dataBase64 = UrlSafeBase64.Encode(dataBytes);

			var unsignedToken = $"{infoBase64}.{dataBase64}";
			// @todo stackalloc
			var message = Encoding.UTF8.GetBytes(unsignedToken);

			System.Span<byte> hash = stackalloc byte[SHA256.HashSizeInBytes];

			var hashed = SHA256.TryHashData(message, hash, out _);

			Debug.Assert(hashed);

			byte[] signed;

			using (var key = JwtSigner.CreateSigningKey(publicKey, privateKey))
			{
				signed = key.SignHash(hash);
			}

			// @todo Fix padding?

			var signature = UrlSafeBase64.Encode(signed);

			return $"{unsignedToken}.{signature}";
		}

		private static ECDsa CreateSigningKey(string publicKey, string privateKey) =>
			ECDsa.Create(Encryption.GetEncryptionParameters(publicKey, privateKey));
	}
}
