namespace ShipmentTracker.WebPush.Internal.Models
{
	internal readonly struct DeclarativePushNotification
	{
		public required int WebPush { get; init; }

		public required PushNotification Notification { get; init; }
	}
}
