using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Nebula.Extensions;
using Nebula.Models.Common;
using Nebula.Resources;
using Nebula.Services;
using ShipmentModel = Nebula.Models.Common.Shipment;

namespace Nebula.Web.Pages
{
	public sealed partial class Shipment : ComponentBase
	{
		internal const string SourceQueryKey = "source";

		[Inject] public required IStringLocalizer<ShipmentLocalization> Localizer { get; init; }

		[Inject] public required IStringLocalizer<ShipmentStateLocalization> ShipmentStateLocalizer { get; init; }

		[Inject] public required ShipmentsService Shipments { get; init; }

		[Parameter] public required string Code { get; init; }

		[Parameter] public required string ZipCode { get; init; }

		[SupplyParameterFromQuery(Name = Shipment.SourceQueryKey)]
		private string? Source { get; init; }

		private ShipmentModel shipment;
		private bool loading = true;

		private string LoadingBlockClass =>
			this.loading ? "skeleton-block" : string.Empty;

		private string Summary =>
			this.shipment == default ? string.Empty : this.Localizer[$"summary.{this.shipment.State.Str()}"];

		private bool Letter =>
			this.shipment.Type is ShipmentType.Letter or ShipmentType.LetterboxPackage;

		protected override Task OnInitializedAsync() =>
			this.LoadAsync();

		private async Task LoadAsync()
		{
			this.loading = true;

			this.shipment = await this.Shipments.GetShipmentAsync(new ShipmentRequest
			{
				Code = this.Code,
				ZipCode = this.ZipCode,
				// @todo From request
				Country = Country.Netherlands,
				Language = LanguageEnumData.FromValue(CultureInfo.CurrentCulture.TwoLetterISOLanguageName),
				Source = this.Source is not null ? ShipmentSourceEnumData.FromValue(this.Source) : default,
			});

			this.loading = false;
		}
	}
}
