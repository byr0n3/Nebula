using Microsoft.Extensions.DependencyInjection;
using Temporalio.Extensions.Hosting;

namespace Nebula.Temporal.Extensions
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Register a Temporal Worker to execute workflows and Temporal Client to start/track workflows.
		/// </summary>
		/// <param name="services">Service collection to add the services to.</param>
		/// <param name="options">Configuration options for the Temporal systems.</param>
		public static void AddWorkflow(this IServiceCollection services, ShipmentTemporalOptions options)
		{
			services.AddHostedTemporalWorker(options.Host, options.Namespace, TemporalClientExtensions.ShipmentTaskQueue)
					.AddSingletonActivities<ShipmentActivities>()
					.AddWorkflow<ShipmentWorkflow>();

			services.AddTemporalClient(options.Host, options.Namespace);
		}
	}
}
