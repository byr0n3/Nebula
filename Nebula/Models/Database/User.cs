using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Elegance.AspNet.Authentication;

namespace Nebula.Models.Database
{
	[Table("users")]
	public sealed class User : IEntity, IAuthenticatable
	{
		[Column("id")] public int Id { get; init; }

		[StringLength(128)] [Column("email")] public required string Email { get; init; }

		[Column("password", TypeName = "bytea")]
		public required byte[] Password { get; init; }

		[Column("flags")] public required UserFlags Flags { get; init; }

		/// <summary>
		/// The locale/culture to use when formatting values.
		/// </summary>
		[StringLength(8)]
		[Column("culture")]
		public required string Culture { get; init; }

		/// <summary>
		/// The display language to show UI elements with.
		/// </summary>
		[StringLength(8)]
		[Column("ui_culture")]
		public required string UiCulture { get; init; }

		[StringLength(32)]
		[Column("timezone")]
		public required string TimeZone { get; init; }

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
		Superuser = 1 << 1,
	}
}
