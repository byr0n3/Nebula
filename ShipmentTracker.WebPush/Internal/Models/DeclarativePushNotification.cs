using System.Runtime.InteropServices;

namespace ShipmentTracker.WebPush.Internal.Models
{
	[StructLayout(LayoutKind.Sequential)]
	internal readonly struct DeclarativePushNotification
	{
		public required int WebPush { get; init; }

		public required PushNotification Notification { get; init; }
	}
}
