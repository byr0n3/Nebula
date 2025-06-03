using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using ParcelTracker.Common.Models;
using ParcelTracker.Services;
using ShipmentModel = ParcelTracker.Common.Models.Shipment;

namespace ParcelTracker.Web.Pages
{
	public sealed partial class Shipment : ComponentBase
	{
		[CascadingParameter] public required HttpContext HttpContext { get; init; }

		[Inject] public required ShipmentsService Shipments { get; init; }

		[Parameter] public required string Code { get; init; }

		[Parameter] public required string ZipCode { get; init; }

		private ShipmentModel shipment;

		protected override Task OnInitializedAsync() =>
			this.LoadAsync();

		private async Task LoadAsync() =>
			this.shipment = await this.Shipments.GetShipmentAsync(new ShipmentRequest
			{
				Code = this.Code,
				ZipCode = this.ZipCode,
				// @todo From client
				Country = Country.Netherlands,
				Language = Language.Dutch,
			}, this.HttpContext.RequestAborted);
	}
}
