namespace Nebula.Models.Common
{
	public readonly struct ShipmentRequest
	{
		public required string Code { get; init; }

		public required string ZipCode { get; init; }

		public Country Country { get; init; }

		public Language Language { get; init; }

		public ShipmentSource Source { get; init; }
	}
}
