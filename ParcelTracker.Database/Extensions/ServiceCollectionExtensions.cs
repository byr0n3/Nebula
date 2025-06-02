using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace ParcelTracker.Database.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddDatabase(this IServiceCollection services, string connectionString, bool development, int poolSize = 8)
		{
			services.AddPooledDbContextFactory<ParcelDbContext>((builder) =>
			{
				var dataSource = new NpgsqlDataSourceBuilder(connectionString)
				   .Build();

				builder.EnableDetailedErrors(development)
					   .EnableSensitiveDataLogging(development)
					   .EnableThreadSafetyChecks(development)
					   .UseNpgsql(dataSource);
			}, poolSize);
		}
	}
}
