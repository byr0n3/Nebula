using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Nebula.Extensions;
using Nebula.Models;
using Nebula.Models.Dto;
using Nebula.Resources;
using Nebula.Services;

namespace Nebula.Web.Pages.Account
{
	public sealed partial class Shipments : ComponentBase
	{
		[CascadingParameter] public required HttpContext HttpContext { get; init; }

		[Inject] public required IStringLocalizer<ShipmentsLocalization> Localizer { get; init; }

		[Inject] public required IStringLocalizer<ShipmentStateLocalization> ShipmentStateLocalizer { get; init; }

		private IQueryable<ShipmentDto> GetQuery(ShipmentDbContext db)
		{
			var userId = this.HttpContext.User.GetClaimValue<int>(UserClaim.Id);

			return db.GetUserShipments(userId)
					 .OrderBy(static (s) => s.State)
					 .ThenByDescending(static (s) => s.Created)
					 .Select(static (s) => new ShipmentDto
					  {
						  Id = s.Id,
						  Code = s.Code,
						  ZipCode = s.ZipCode,
						  Source = s.Source,
						  State = s.State,
						  Eta = s.Eta,
						  Arrived = s.Arrived ?? s.Updated,
						  Sender = s.Sender,
						  Created = s.Created,
					  });
		}
	}
}
