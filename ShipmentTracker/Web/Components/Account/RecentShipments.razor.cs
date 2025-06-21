using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShipmentTracker.Extensions;
using ShipmentTracker.Models;
using ShipmentTracker.Models.Dto;
using ShipmentTracker.Services;

namespace ShipmentTracker.Web.Components.Account
{
	public sealed partial class RecentShipments : ComponentBase
	{
		[CascadingParameter] public required HttpContext HttpContext { get; init; }

		[Inject] public required IDbContextFactory<ShipmentDbContext> DbFactory { get; init; }

		private List<ShipmentDto>? shipments;

		protected override Task OnInitializedAsync() =>
			this.LoadAsync();

		private async Task LoadAsync()
		{
			var userId = this.HttpContext.User.GetClaimValue<int>(UserClaim.Id);

			var db = await this.DbFactory.CreateDbContextAsync();

			await using (db)
			{
				this.shipments = await db.GetUserShipments(userId)
										 .OrderByDescending(static (s) => s.Created)
										 .Select(static (s) => new ShipmentDto
										  {
											  Id = s.Id,
											  Code = s.Code,
											  ZipCode = s.ZipCode,
											  Source = s.Source,
											  State = s.State,
											  Eta = s.Eta,
											  Arrived = s.Arrived,
											  Sender = s.Sender,
										  })
										 .Take(3)
										 .ToListAsync();
			}
		}
	}
}
