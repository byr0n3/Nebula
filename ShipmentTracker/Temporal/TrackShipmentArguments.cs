using ShipmentTracker.Models.Common;

namespace ShipmentTracker.Temporal
{
	public readonly struct TrackShipmentArguments
	{
		/// <summary>
		/// The source of the shipment.
		/// </summary>
		public required ShipmentSource Source { get; init; }

		/// <summary>
		/// The shipment's tracking code.
		/// </summary>
		public required string Code { get; init; }

		/// <summary>
		/// The shipment's recipient's zip code.
		/// </summary>
		public required string ZipCode { get; init; }

		/// <summary>
		/// The shipment's ID as stored in the database.
		/// </summary>
		public required int ShipmentId { get; init; }

		/// <summary>
		/// Amount of time to wait between shipment updates.
		/// </summary>
		public required System.TimeSpan Delay { get; init; }
	}
}
