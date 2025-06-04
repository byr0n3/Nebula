using NpgsqlTypes;
using ParcelTracker.Common.Models;

namespace ParcelTracker.Models.Dto
{
	public readonly struct ShipmentDto : System.IEquatable<ShipmentDto>
	{
		public required int Id { get; init; }

		public required string Code { get; init; }

		public required string ZipCode { get; init; }

		public required ShipmentSource Source { get; init; }

		public required ShipmentState State { get; init; }

		public required NpgsqlRange<System.DateTime>? Eta { get; init; }

		public required System.DateTime? Arrived { get; init; }

		public required string? Sender { get; init; }

		public bool Equals(ShipmentDto other) =>
			this.Id == other.Id;

		public override bool Equals(object? @object) =>
			(@object is ShipmentDto other) && this.Equals(other);

		public override int GetHashCode() =>
			this.Id;

		public static bool operator ==(ShipmentDto left, ShipmentDto right) =>
			left.Equals(right);

		public static bool operator !=(ShipmentDto left, ShipmentDto right) =>
			!left.Equals(right);
	}
}
