using System.Threading.Tasks;
using ShipmentTracker.Models.Common;
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
			Shipment shipment = default;

			while (shipment.State != ShipmentState.Delivered)
			{
				shipment = await Workflow.ExecuteActivityAsync<ShipmentActivities, Shipment>(
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
					await Workflow.ExecuteActivityAsync<ShipmentActivities>(
						(a) => a.SendPushNotificationsAsync(arguments.ShipmentId, shipment),
						ShipmentWorkflow.activityOptions
					);
				}

				await Workflow.DelayAsync(arguments.Delay);
			}
		}
	}
}
