using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using ShipmentTracker.Database;
using ShipmentTracker.Extensions;
using ShipmentTracker.Models;
using ShipmentTracker.Models.Dto;

namespace ShipmentTracker.Web.Pages.Account
{
	public sealed partial class Shipments : ComponentBase
	{
		[CascadingParameter] public required HttpContext HttpContext { get; init; }

		private IQueryable<ShipmentDto> GetQuery(ShipmentDbContext db)
		{
			var userId = this.HttpContext.User.GetClaimValue<int>(UserClaim.Id);

			return db.GetUserShipments(userId)
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
					  });
		}
	}
}
