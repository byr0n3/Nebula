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

			if (updated)
			{
				// @todo Send push notification
			}

			if (shipment.State == ShipmentState.Delivered)
			{
				return;
			}

			await Workflow.ExecuteActivityAsync<ShipmentActivities>(
				(a) => a.StartWorkflowAsync(arguments),
				ShipmentWorkflow.activityOptions
			);
		}
	}
}
