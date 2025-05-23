using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using ParcelTracker.Common;

namespace ParcelTracker.DHL.Extensions
{
	public static class ServiceCollectionExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IServiceCollection AddDHLClient(this IServiceCollection services)
		{
			services.AddHttpClient<IShipmentSource, DHLClient>("DHL");

			return services;
		}
	}
}
