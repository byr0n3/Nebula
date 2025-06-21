using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Elegance.AspNet.Authentication;

namespace ShipmentTracker.Models.Database
{
	[Table("users")]
	public sealed class User : IEntity, IAuthenticatable
	{
		[Column("id")] public int Id { get; init; }

		[StringLength(128)]
		[Column("username")]
		public required string Username { get; init; }

		[StringLength(128)] [Column("email")] public required string Email { get; init; }

		[Column("password", TypeName = "bytea")]
		public required byte[] Password { get; init; }

		[Column("flags")] public UserFlags Flags { get; init; }

		[Column("created")]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public System.DateTime Created { get; init; }

		public ICollection<Shipment> Shipments { get; init; } = null!;
		public ICollection<UserShipment> UserShipments { get; init; } = null!;
		public ICollection<UserPushSubscription> PushSubscriptions { get; init; } = null!;
	}

	[System.Flags]
	public enum UserFlags
	{
		None = 0,
		Active = 1 << 0,
	}
}
