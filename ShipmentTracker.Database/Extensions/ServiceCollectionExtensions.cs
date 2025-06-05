using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShipmentTracker.Common.Models;
using ShipmentTracker.Database.Internal.Translators;

namespace ShipmentTracker.Database.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddDatabase(this IServiceCollection services, string connectionString, bool development, int poolSize = 8)
		{
			services.AddPooledDbContextFactory<ShipmentDbContext>((builder) =>
			{
				builder.EnableDetailedErrors(development)
					   .EnableSensitiveDataLogging(development)
					   .EnableThreadSafetyChecks(development)
					   .UseNpgsql(
							connectionString,
							static (options) =>
							{
								options.MapEnum<ShipmentSource>(nameTranslator: ShipmentSourceNameTranslator.Instance);
								options.MapEnum<ShipmentState>(nameTranslator: ShipmentStateNameTranslator.Instance);
							}
						);
			}, poolSize);
		}
	}
}
