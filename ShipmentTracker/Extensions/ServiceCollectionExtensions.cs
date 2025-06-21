using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShipmentTracker.Models.Common;
using ShipmentTracker.Npgqsl.Translators;
using ShipmentTracker.Services;
using ShipmentTracker.Sources;
using ShipmentTracker.Sources.DHL;
using ShipmentTracker.Sources.PostNL;

namespace ShipmentTracker.Extensions
{
	internal static class ServiceCollectionExtensions
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

		public static IServiceCollection AddPostNLClient(this IServiceCollection services)
		{
			services.AddHttpClient<IShipmentSource, PostNLClient>("PostNL");

			return services;
		}

		public static IServiceCollection AddDHLClient(this IServiceCollection services)
		{
			services.AddHttpClient<IShipmentSource, DHLClient>("DHL");

			return services;
		}
	}
}
