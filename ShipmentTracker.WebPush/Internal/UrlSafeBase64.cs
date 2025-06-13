namespace ShipmentTracker.WebPush.Internal
{
	// @todo
	[System.Obsolete("Optimize")]
	internal static class UrlSafeBase64
	{
		public static byte[] Decode(string base64)
		{
			base64 = base64.Replace('-', '+').Replace('_', '/');

			while (base64.Length % 4 != 0)
			{
				base64 += "=";
			}

			return System.Convert.FromBase64String(base64);
		}

		public static string Encode(byte[] data)
		{
			return System.Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_').TrimEnd('=');
		}
	}
}
