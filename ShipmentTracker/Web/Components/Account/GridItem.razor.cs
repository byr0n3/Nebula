using Microsoft.AspNetCore.Components;
using ShipmentTracker.Models.Dto;

namespace ShipmentTracker.Web.Components.Account
{
	public sealed partial class GridItem : ComponentBase
	{
		[Parameter] [EditorRequired] public required ShipmentDto Shipment { get; init; }
	}
}
