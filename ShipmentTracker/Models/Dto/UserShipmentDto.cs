using System.Runtime.InteropServices;

namespace ShipmentTracker.Models.Dto
{
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct UserShipmentDto : System.IEquatable<UserShipmentDto>
	{
		public required int ShipmentId { get; init; }

		public required bool Subscribed { get; init; }

		public bool Equals(UserShipmentDto other) =>
			(this.ShipmentId == other.ShipmentId) && (this.Subscribed == other.Subscribed);

		public override bool Equals(object? @object) =>
			(@object is UserShipmentDto other) && this.Equals(other);

		public override int GetHashCode() =>
			System.HashCode.Combine(this.ShipmentId, this.Subscribed);

		public static bool operator ==(UserShipmentDto left, UserShipmentDto right) =>
			left.Equals(right);

		public static bool operator !=(UserShipmentDto left, UserShipmentDto right) =>
			!left.Equals(right);
	}
}
