using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Elegance.Extensions;
using Temporalio.Client;

namespace ShipmentTracker.Temporal.Extensions
{
	public static class TemporalClientExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Task StartShipmentWorkflowAsync(this ITemporalClient client, TrackShipmentArguments arguments) =>
			client.StartWorkflowAsync<ShipmentWorkflow>(
				(wf) => wf.TrackAsync(arguments),
				new WorkflowOptions
				{
					Id = $"shipment-{arguments.ShipmentId.Str()}",
					TaskQueue = "ShipmentTracker",
					StartDelay = arguments.Delay,
				}
			);
	}
}
