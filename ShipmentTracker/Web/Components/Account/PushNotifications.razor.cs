using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Vapid.NET;

namespace ShipmentTracker.Web.Components.Account
{
	public sealed partial class PushNotifications : ComponentBase
	{
		[Inject] public required IOptions<VapidOptions> Vapid { get; init; }
	}
}
