using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ParcelTracker.Database;

namespace ParcelTracker.Web.Pages.Account
{
	public sealed partial class View : ComponentBase
	{
		[CascadingParameter] public required HttpContext HttpContext { get; init; }

		[Inject] public required IDbContextFactory<ShipmentDbContext> DbFactory { get; init; }
	}
}
