using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Nebula.Models.Common;
using Nebula.Services;
using ShipmentModel = Nebula.Models.Common.Shipment;

namespace Nebula.Web.Pages
{
	public sealed partial class Shipment : ComponentBase
	{
		internal const string SourceQueryKey = "source";

		[Inject] public required ShipmentsService Shipments { get; init; }

		[Parameter] public required string Code { get; init; }

		[Parameter] public required string ZipCode { get; init; }

		[SupplyParameterFromQuery(Name = Shipment.SourceQueryKey)]
		private string? Source { get; init; }

		private ShipmentModel shipment;
		private bool loading = true;

		private string Summary =>
			(this.shipment.State) switch
			{
				ShipmentState.Registered     => "The shipment has been registered, but hasn't arrived at the delivery service yet.",
				ShipmentState.Received       => "The shipment has been registered and received.",
				ShipmentState.Sorted         => "The shipment has been sorted.",
				ShipmentState.OutForDelivery => "The shipment is out for delivery!",
				ShipmentState.Delivered      => "The delivery service reported that the shipment has been delivered.",
				_                            => throw new System.Exception("Unknown state"),
			};

		protected override Task OnInitializedAsync() =>
			this.LoadAsync();

		private async Task LoadAsync()
		{
			this.loading = true;

			this.shipment = await this.Shipments.GetShipmentAsync(new ShipmentRequest
			{
				Code = this.Code,
				ZipCode = this.ZipCode,
				// @todo From client
				Country = Country.Netherlands,
				Language = Language.Dutch,
				Source = this.Source is not null ? ShipmentSourceEnumData.FromValue(this.Source) : default,
			});

			this.loading = false;
		}
	}
}
