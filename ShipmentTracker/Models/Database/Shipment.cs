using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using NpgsqlTypes;
using ShipmentTracker.Models.Common;

namespace ShipmentTracker.Models.Database
{
	[Table("shipments")]
	public sealed class Shipment : IEntity
	{
		[Column("id")] public int Id { get; init; }

		[Column("code")] [StringLength(128)] public required string Code { get; init; }

		[Column("source")] public required ShipmentSource Source { get; init; }

		[StringLength(8)] [Column("zip_code")] public required string ZipCode { get; init; }

		[Column("state")] public required ShipmentState State { get; set; }

		[Column("eta")] public NpgsqlRange<System.DateTime>? Eta { get; set; }

		[Column("arrived")] public System.DateTime? Arrived { get; set; }

		[StringLength(128)]
		[Column("recipient")]
		public string? Recipient { get; init; }

		[Column("sender")] [StringLength(128)] public string? Sender { get; init; }

		[Column("created")] public required System.DateTime Created { get; init; }

		[Column("updated")] public required System.DateTime Updated { get; init; }

		public ICollection<User> Users { get; init; } = null!;
		public ICollection<UserShipment> UserShipments { get; init; } = null!;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Shipment(Common.Shipment shipment) =>
			new()
			{
				Code = shipment.TrackingCode,
				Source = shipment.Source,
				ZipCode = shipment.Recipient.ZipCode.Replace(' ', '\0'),
				State = shipment.State,
				Eta = shipment.Eta != default ? new NpgsqlRange<System.DateTime>(shipment.Eta.Lower, true, shipment.Eta.Upper, true) : null,
				Arrived = shipment.Arrived != default ? shipment.Arrived : null,
				Recipient = shipment.Recipient.Name,
				Sender = shipment.Sender.Name,
				Created = shipment.Created,
				Updated = shipment.Updated,
			};
	}
}
