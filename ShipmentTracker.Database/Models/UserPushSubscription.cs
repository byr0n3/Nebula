using System.ComponentModel.DataAnnotations.Schema;
using ShipmentTracker.Database.Interfaces;

namespace ShipmentTracker.Database.Models
{
	[Table("users_push_subscriptions")]
	public sealed class UserPushSubscription : IEntity, IEntityWithUser
	{
		[Column("id")] public int Id { get; init; }

		[Column("user_id")] public required int UserId { get; init; }

		[Column("endpoint")] public required string Endpoint { get; init; }

		[Column("expires")] public required System.DateTime? Expires { get; init; }

		[Column("p256dh")] public required string P256dh { get; init; }

		[Column("auth")] public required string Auth { get; init; }

		[Column("created")]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public System.DateTime Created { get; init; }

		public User User { get; init; } = null!;
	}
}
