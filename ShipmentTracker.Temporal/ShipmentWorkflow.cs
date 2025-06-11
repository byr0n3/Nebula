using System.Threading.Tasks;
using ShipmentTracker.Common.Models;
using Temporalio.Common;
using Temporalio.Workflows;

namespace ShipmentTracker.Temporal
{
	[Workflow]
	internal sealed class ShipmentWorkflow
	{
		private static readonly ActivityOptions activityOptions =
			new()
			{
				StartToCloseTimeout = System.TimeSpan.FromMinutes(5),
				RetryPolicy = new RetryPolicy
				{
					InitialInterval = System.TimeSpan.FromSeconds(1),
					MaximumInterval = System.TimeSpan.FromSeconds(100),
					MaximumAttempts = 3,
					NonRetryableErrorTypes = [],
				},
			};

		/// <summary>
		/// Find a shipment from the provided source, update its data in the database and inform subscribed users.
		/// </summary>
		/// <param name="arguments">The arguments to use when fetching the shipment.</param>
		[WorkflowRun]
		public async Task TrackAsync(TrackShipmentArguments arguments)
		{
			var shipment = await Workflow.ExecuteActivityAsync<ShipmentActivities, Shipment>(
				(a) => a.GetShipmentAsync(arguments),
				ShipmentWorkflow.activityOptions
			);

			var updated = await Workflow.ExecuteActivityAsync<ShipmentActivities, bool>(
				(a) => a.UpdateShipmentAsync(arguments.ShipmentId, shipment),
				ShipmentWorkflow.activityOptions
			);

			// The shipment was updated, we should inform subscribers about this.
			if (updated)
			{
				// @todo Send push notification
			}

			// If the shipment has been successfully delivered, there's no need to keep updating the shipment.
			if (shipment.State == ShipmentState.Delivered)
			{
				return;
			}

			// Start another (delayed) workflow to update the shipment once more.
			await Workflow.ExecuteActivityAsync<ShipmentActivities>(
				(a) => a.StartWorkflowAsync(arguments),
				ShipmentWorkflow.activityOptions
			);
		}
	}
}
