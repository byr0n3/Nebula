using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using ShipmentTracker.Database.Models;

namespace ShipmentTracker.Database
{
	public sealed class ShipmentDbContext : DbContext
	{
		public required DbSet<User> Users { get; init; }

		public required DbSet<Shipment> Shipments { get; init; }

		public required DbSet<UserShipment> UsersShipments { get; init; }

		public ShipmentDbContext(DbContextOptions<ShipmentDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<User>(static (builder) =>
			{
				builder.HasMany(static (u) => u.Shipments)
					   .WithMany(static (s) => s.Users)
					   .UsingEntity<UserShipment>();
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IQueryable<Shipment> GetUserShipments(int userId) =>
			this.UsersShipments
				.Where((us) => us.UserId == userId)
				.Join(
					 this.Shipments,
					 static (us) => us.ShipmentId,
					 static (s) => s.Id,
					 static (_, s) => s
				 );
	}
}
