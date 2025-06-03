using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ParcelTracker.Database.Models
{
	[Table("users_shipments")]
	[PrimaryKey(nameof(UserShipment.UserId), nameof(UserShipment.ShipmentId))]
	public sealed class UserShipment
	{
		[Column("user_id")] public required int UserId { get; init; }

		[Column("shipment_id")] public required int ShipmentId { get; init; }

		public User User { get; init; } = null!;

		public Shipment Shipment { get; init; } = null!;
	}
}
