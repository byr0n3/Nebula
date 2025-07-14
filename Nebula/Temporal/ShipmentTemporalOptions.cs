using Microsoft.Extensions.Options;

namespace Nebula.Temporal
{
	public sealed class ShipmentTemporalOptions : IOptions<ShipmentTemporalOptions>
	{
		public required string Host { get; init; }

		public required string Namespace { get; init; }

		ShipmentTemporalOptions IOptions<ShipmentTemporalOptions>.Value =>
			this;
	}
}
