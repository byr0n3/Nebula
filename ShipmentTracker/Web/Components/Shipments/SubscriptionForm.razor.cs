using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using ShipmentTracker.Common.Models;
using ShipmentTracker.Extensions;
using ShipmentTracker.Models;
using ShipmentTracker.Models.Dto;
using ShipmentTracker.Services;
using ShipmentTracker.Temporal;
using ShipmentTracker.Temporal.Extensions;
using Temporalio.Client;

namespace ShipmentTracker.Web.Components.Shipments
{
	public sealed partial class SubscriptionForm : ComponentBase
	{
		private const string formName = "shipment-subscription";

		[CascadingParameter] public required HttpContext HttpContext { get; init; }

		[Inject] public required ShipmentsService Shipments { get; init; }

		[Inject] public required ITemporalClient Temporal { get; init; }

		[Parameter] [EditorRequired] public required Shipment Shipment { get; set; }

		private readonly EditContext context = new(0);
		private UserShipmentDto userShipment;

		protected override Task OnInitializedAsync() =>
			this.LoadAsync();

		private async Task LoadAsync()
		{
			var userId = this.HttpContext.User.GetClaimValue<int>(UserClaim.Id);

			this.userShipment =
				await this.Shipments.GetOrCreateUserShipmentDtoAsync(this.Shipment, userId, this.HttpContext.RequestAborted);
		}

		private async Task ToggleSubscriptionAsync()
		{
			if (this.HttpContext.User.Identity?.IsAuthenticated != true)
			{
				return;
			}

			var userId = this.HttpContext.User.GetClaimValue<int>(UserClaim.Id);

			if (this.userShipment.Subscribed)
			{
				await this.Shipments.UnsubscribeUserAsync(this.userShipment.ShipmentId, userId, this.HttpContext.RequestAborted);
			}
			else
			{
				await this.Shipments.SubscribeUserAsync(this.userShipment.ShipmentId, userId, this.HttpContext.RequestAborted);

				await this.Temporal.StartShipmentWorkflowAsync(new TrackShipmentArguments
				{
					Source = this.Shipment.Source,
					Code = this.Shipment.TrackingCode,
					ZipCode = this.Shipment.Recipient.ZipCode,
					ShipmentId = this.userShipment.ShipmentId,
					// @todo Based on account
					Delay = System.TimeSpan.FromMinutes(1),
				});
			}

			this.userShipment = this.userShipment with
			{
				Subscribed = !this.userShipment.Subscribed,
			};
		}
	}
}
