using Microsoft.Extensions.DependencyInjection;
using Temporalio.Extensions.Hosting;

namespace ShipmentTracker.Temporal.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddWorkflow(this IServiceCollection services, ShipmentTemporalOptions options)
		{
			services.AddHostedTemporalWorker(options.Host, options.Namespace, "ShipmentTracker")
					.AddSingletonActivities<ShipmentActivities>()
					.AddWorkflow<ShipmentWorkflow>();

			services.AddTemporalClient(options.Host, options.Namespace);
		}
	}
}
