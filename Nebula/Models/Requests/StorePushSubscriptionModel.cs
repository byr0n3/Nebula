using System.Text.Json.Serialization;

namespace Nebula.Models.Requests
{
	public sealed class StorePushSubscriptionModel
	{
		public required string Endpoint { get; init; }

		[JsonPropertyName("expirationTime")] public System.DateTime? Expiration { get; init; }

		public required PushSubscriptionKeys Keys { get; init; }

		public sealed class PushSubscriptionKeys
		{
			public required string P256dh { get; init; }

			public required string Auth { get; init; }
		}
	}
}
