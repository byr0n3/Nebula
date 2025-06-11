using System.Diagnostics;
using System.Threading.Tasks;
using Elegance.Extensions;
using Microsoft.AspNetCore.Components;
using ShipmentTracker.Common.Models;
using ShipmentTracker.Extensions;
using ShipmentTracker.Models;
using ShipmentTracker.Models.Dto;
using ShipmentTracker.Services;
using ShipmentTracker.Temporal;
using ShipmentTracker.Temporal.Extensions;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace ShipmentTracker.Web.Components.Shipments
{
	public sealed partial class Automation : ComponentBase
	{
		[Inject] public required AuthenticationService Authentication { get; init; }

		[Inject] public required ShipmentsService Shipments { get; init; }

		[Inject] public required ITemporalClient Temporal { get; init; }

		[Parameter] [EditorRequired] public required Shipment Shipment { get; set; }

		private UserShipmentDto userShipment;
		private System.DateTime nextUpdate;
		private bool workflowRunning;

		protected override Task OnInitializedAsync() =>
			this.LoadAsync();

		private async Task LoadAsync()
		{
			Debug.Assert(this.Authentication.User is not null);

			var userId = this.Authentication.User.GetClaimValue<int>(UserClaim.Id);

			this.userShipment = await this.Shipments.GetOrCreateUserShipmentDtoAsync(this.Shipment, userId);

			if (this.userShipment.ShipmentId == default)
			{
				return;
			}

			try
			{
				var handle = this.Temporal.GetWorkflowHandle($"shipment-{this.userShipment.ShipmentId.Str()}");

				var description = await handle.DescribeAsync();

				this.workflowRunning = description.Status == WorkflowExecutionStatus.Running;

				if (this.workflowRunning && description.ExecutionTime is not null)
				{
					this.nextUpdate = description.ExecutionTime.GetValueOrDefault();
				}
			}
			catch (RpcException ex) when (ex.Code == RpcException.StatusCode.NotFound)
			{
				this.workflowRunning = false;
			}
		}

		private async Task ToggleSubscriptionAsync()
		{
			Debug.Assert(this.Authentication.User is not null);

			var userId = this.Authentication.User.GetClaimValue<int>(UserClaim.Id);

			if (this.userShipment.Subscribed)
			{
				await this.Shipments.UnsubscribeUserAsync(this.userShipment.ShipmentId, userId);
			}
			else
			{
				await this.Shipments.SubscribeUserAsync(this.userShipment.ShipmentId, userId);

				if (this.Shipment.State != ShipmentState.Delivered)
				{
					await this.StartWorkflowAsync();
				}
			}

			this.userShipment = this.userShipment with
			{
				Subscribed = !this.userShipment.Subscribed,
			};
		}

		// @todo Show toast/notification/feedback that workflow started
		private Task StartWorkflowAsync() =>
			this.Temporal.StartShipmentWorkflowAsync(new TrackShipmentArguments
			{
				Source = this.Shipment.Source,
				Code = this.Shipment.TrackingCode,
				ZipCode = this.Shipment.Recipient.ZipCode,
				ShipmentId = this.userShipment.ShipmentId,
				// @todo Based on account
				Delay = System.TimeSpan.FromMinutes(1),
			});
	}
}
