using ShipmentTracker.Common.Models;

namespace ShipmentTracker.Temporal
{
	public readonly struct TrackShipmentArguments
	{
		public required ShipmentSource Source { get; init; }

		public required string Code { get; init; }

		public required string ZipCode { get; init; }

		public required int ShipmentId { get; init; }

		public required System.TimeSpan Delay { get; init; }
	}
}
