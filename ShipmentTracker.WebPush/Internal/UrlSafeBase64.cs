using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using Elegance.Utilities;

namespace ShipmentTracker.WebPush.Internal
{
	internal static class UrlSafeBase64
	{
		// @todo
		[System.Obsolete("Optimize")]
		public static byte[] Decode(string base64)
		{
			base64 = base64.Replace('-', '+').Replace('_', '/');

			while (base64.Length % 4 != 0)
			{
				base64 += "=";
			}

			return System.Convert.FromBase64String(base64);
		}

		// @todo
		[System.Obsolete("Optimize")]
		public static string Encode(scoped System.ReadOnlySpan<byte> data)
		{
			return System.Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_').TrimEnd('=');
		}

		// @todo
		[System.Obsolete("Optimize")]
		public static string Encode(RentedArray<byte> data)
		{
			return System.Convert.ToBase64String(data.Slice(0, data.Length)).Replace('+', '-').Replace('/', '_').TrimEnd('=');
		}

		public static int Encode(scoped System.ReadOnlySpan<byte> src, scoped System.Span<byte> dst)
		{
			var status = Base64.EncodeToUtf8(src, dst, out _, out var written);

			Debug.Assert(status == OperationStatus.Done);

			dst.Slice(0, written);

			UrlSafeBase64.Replace(dst, "+"u8, "-"u8);
			UrlSafeBase64.Replace(dst, "/"u8, "_"u8);

			// @todo Replace('+', '-').Replace('/', '_')

			dst = System.MemoryExtensions.TrimEnd(dst, "="u8);

			return dst.Length;
		}

		private static void Replace(scoped System.Span<byte> dst,
									scoped System.ReadOnlySpan<byte> find,
									scoped System.ReadOnlySpan<byte> replace)
		{
			Debug.Assert(find.Length == replace.Length);

			int idx;

			while ((idx = System.MemoryExtensions.IndexOf(dst, find)) != -1)
			{
				replace.TryCopyTo(dst.Slice(idx));
			}
		}
	}
}
