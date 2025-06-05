using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using ShipmentTracker.Common;

namespace ShipmentTracker.PostNL.Extensions
{
	public static class ServiceCollectionExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IServiceCollection AddPostNLClient(this IServiceCollection services)
		{
			services.AddHttpClient<IShipmentSource, PostNLClient>("Post NL");

			return services;
		}
	}
}
