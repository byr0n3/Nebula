using Microsoft.EntityFrameworkCore;
using ParcelTracker.Database.Models;

namespace ParcelTracker.Database
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
	}
}
