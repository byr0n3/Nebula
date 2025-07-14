using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nebula.Models.Common;
using Nebula.Npgqsl.Translators;
using Nebula.Services;
using Nebula.Sources;
using Nebula.Sources.DHL;
using Nebula.Sources.PostNL;

namespace Nebula.Extensions
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
