using Microsoft.AspNetCore.Components;
using ParcelTracker.Models.Dto;

namespace ParcelTracker.Web.Components.Account
{
	public sealed partial class GridItem : ComponentBase
	{
		[Parameter] [EditorRequired] public required ShipmentDto Shipment { get; init; }
	}
}
