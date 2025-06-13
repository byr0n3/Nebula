using System.Diagnostics;
using ShipmentTracker.WebPush.Internal.Models;

namespace ShipmentTracker.WebPush.Internal
{
	internal static class Jwt
	{
		public static string GetSignedToken(string endpoint, VapidOptions vapid)
		{
			Debug.Assert(vapid.IsValid);

			var info = new JwtInfo
			{
				Type = "JWT",
				Algorithm = "ES256",
			};

			var uri = new System.Uri(endpoint, System.UriKind.Absolute);
			var audience = string.Create(null, stackalloc char[uri.Scheme.Length + 3 + uri.Host.Length], $"{uri.Scheme}://{uri.Host}");

			var data = new JwtData
			{
				Audience = audience,
				Expiration = System.DateTime.UtcNow.AddHours(12),
				Subject = vapid.Subject,
			};

			return JwtSigner.Sign(vapid.PublicKey, vapid.PrivateKey, info, data);
		}
	}
}
