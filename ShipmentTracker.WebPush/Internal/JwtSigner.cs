using System.Buffers.Text;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Elegance.Utilities;
using JetBrains.Annotations;
using ShipmentTracker.WebPush.Internal.Models;

namespace ShipmentTracker.WebPush.Internal
{
	internal static class JwtSigner
	{
		private static System.ReadOnlySpan<byte> Separator =>
			"."u8;

		public static string Sign(string publicKey, string privateKey, JwtInfo info, JwtData data)
		{
			var message = JwtSigner.GetUnsignedToken(info, data);
			var signature = JwtSigner.HashAndSign(message, publicKey, privateKey);

			using (message)
			using (signature)
			{
				System.Span<byte> buffer = stackalloc byte[message.Length + JwtSigner.Separator.Length + signature.Length];

				var builder = new ByteBuilder(buffer);
				{
					builder.Append(message);
					builder.Append(JwtSigner.Separator);
					builder.Append(signature);
				}

				return Encoding.UTF8.GetString(builder.Result);
			}
		}

		[MustDisposeResource]
		private static RentedArray<byte> GetUnsignedToken(JwtInfo info, JwtData data)
		{
			// @todo stackalloc
			var infoBytes = JsonSerializer.SerializeToUtf8Bytes(info, WebPushJsonSerializerContext.Default.JwtInfo!);
			var dataBytes = JsonSerializer.SerializeToUtf8Bytes(data, WebPushJsonSerializerContext.Default.JwtData!);

			System.Span<byte> infoBase64 = stackalloc byte[Base64.GetMaxEncodedToUtf8Length(infoBytes.Length)];
			System.Span<byte> dataBase64 = stackalloc byte[Base64.GetMaxEncodedToUtf8Length(dataBytes.Length)];

			var infoLength = UrlSafeBase64.Encode(infoBytes, infoBase64);
			infoBase64 = infoBase64.Slice(0, infoLength);

			var dataLength = UrlSafeBase64.Encode(dataBytes, dataBase64);
			dataBase64 = dataBase64.Slice(0, dataLength);

			var result = new RentedArray<byte>(infoLength + JwtSigner.Separator.Length + dataLength);

			var builder = new ByteBuilder(result);
			{
				builder.Append(infoBase64);
				builder.Append(JwtSigner.Separator);
				builder.Append(dataBase64);
			}

			return result;
		}

		[MustDisposeResource]
		private static RentedArray<byte> HashAndSign(scoped System.ReadOnlySpan<byte> message, string publicKey, string privateKey)
		{
			System.Span<byte> hash = stackalloc byte[SHA256.HashSizeInBytes];

			var hashed = SHA256.TryHashData(message, hash, out _);

			Debug.Assert(hashed);

			return JwtSigner.Sign(hash, publicKey, privateKey);
		}

		[MustDisposeResource]
		private static RentedArray<byte> Sign(scoped System.ReadOnlySpan<byte> hash, string publicKey, string privateKey)
		{
			using (var ecdsa = ECDsa.Create(Encryption.GetEncryptionParameters(publicKey, privateKey)))
			{
				System.Span<byte> signature = stackalloc byte[ecdsa.GetMaxSignatureSize(default)];

				var signed = ecdsa.TrySignHash(hash, signature, default, out var written);

				Debug.Assert(signed);

				signature.Slice(0, written);

				System.Span<byte> base64 = stackalloc byte[Base64.GetMaxEncodedToUtf8Length(written)];

				written = UrlSafeBase64.Encode(signature, base64);

				base64 = base64.Slice(0, written);

				var result = new RentedArray<byte>(base64.Length);

				var copied = base64.TryCopyTo(result);

				Debug.Assert(copied);

				return result;
			}
		}
	}
}
