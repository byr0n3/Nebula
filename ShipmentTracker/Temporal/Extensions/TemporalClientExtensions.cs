using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Elegance.Extensions;
using Temporalio.Client;

namespace ShipmentTracker.Temporal.Extensions
{
	internal static class TemporalClientExtensions
	{
		public const string ShipmentTaskQueue = "ShipmentTracker";

		/// <summary>
		/// (Re)start the shipment workflow for a given shipment.
		/// </summary>
		/// <param name="client">The Temporal Client.</param>
		/// <param name="arguments">The arguments to use for the workflow.</param>
		/// <returns>Task that is completed when the workflow has started.</returns>
		/// <remarks>The workflow <b>has not</b> finished executing once this task returns, it's only registered and started.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Task StartShipmentWorkflowAsync(this ITemporalClient client, TrackShipmentArguments arguments) =>
			client.StartWorkflowAsync<ShipmentWorkflow>(
				(wf) => wf.TrackAsync(arguments),
				new WorkflowOptions
				{
					Id = $"shipment-{arguments.ShipmentId.Str()}",
					TaskQueue = TemporalClientExtensions.ShipmentTaskQueue,
					StartDelay = arguments.Delay,
				}
			);
	}
}
