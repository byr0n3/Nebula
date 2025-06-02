using Microsoft.EntityFrameworkCore;
using ParcelTracker.Database.Models;

namespace ParcelTracker.Database
{
	public sealed class ParcelDbContext : DbContext
	{
		public required DbSet<User> Users { get; init; }

		public ParcelDbContext(DbContextOptions<ParcelDbContext> options) : base(options)
		{
		}
	}
}
