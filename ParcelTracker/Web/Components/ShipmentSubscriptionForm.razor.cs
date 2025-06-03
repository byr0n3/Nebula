using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using ParcelTracker.Common.Models;
using ParcelTracker.Extensions;
using ParcelTracker.Models;
using ParcelTracker.Services;

namespace ParcelTracker.Web.Components
{
	public sealed partial class ShipmentSubscriptionForm : ComponentBase
	{
		private const string formName = "shipment-subscription";

		[CascadingParameter] public required HttpContext HttpContext { get; init; }

		[Inject] public required ShipmentsService Shipments { get; init; }

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
			}

			this.userShipment = this.userShipment with
			{
				Subscribed = !this.userShipment.Subscribed,
			};
		}
	}
}
