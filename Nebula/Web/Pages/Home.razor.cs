using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Nebula.Web.Pages
{
	public sealed partial class Home : ComponentBase
	{
		private const string formName = "find-shipment";

		[Inject] public required IStringLocalizer<HomeLocalization> Localizer { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		[SupplyParameterFromForm(FormName = Home.formName)]
		private FindShipmentModel Model { get; set; } = new();

		private void FindShipment()
		{
			Debug.Assert(this.Model.IsValid);

			// @todo Sanitize ZIP (remove spaces)
			this.Navigation.NavigateTo($"/shipments/{this.Model.Code}/{this.Model.ZipCode}", true);
		}

		private sealed class FindShipmentModel
		{
			[Required] public string? Code { get; set; }

			// @todo Validate format
			[Required] public string? ZipCode { get; set; }

			public bool IsValid
			{
				[MemberNotNullWhen(true, nameof(this.Code), nameof(this.ZipCode))]
				get => (this.Code is not null) && (this.ZipCode is not null);
			}
		}
	}
}
