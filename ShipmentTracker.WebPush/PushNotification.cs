using System.Text.Json.Serialization;

namespace ShipmentTracker.WebPush
{
	public readonly struct PushNotification
	{
		public required string Title { get; init; }

		public required string Body { get; init; }

		public required string Navigate { get; init; }

		[JsonPropertyName("lang")] public string? Language { get; init; }

		// @todo Enum?
		[JsonPropertyName("dir")] public string? Direction { get; init; }

		public bool Silent { get; init; }

		[JsonPropertyName("app_badge")] public int BadgeCount { get; init; }

		public string? Topic { get; init; }

		[JsonIgnore] public PushNotificationUrgency Urgency { get; init; }

		[JsonIgnore] public System.TimeSpan Ttl { get; init; }
	}
}
