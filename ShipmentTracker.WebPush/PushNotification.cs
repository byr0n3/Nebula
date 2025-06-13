namespace ShipmentTracker.WebPush
{
	public readonly struct PushNotification
	{
		public required string Title { get; init; }

		public required string Content { get; init; }

		public string? Topic { get; init; }

		public System.TimeSpan Ttl { get; init; }
	}
}
