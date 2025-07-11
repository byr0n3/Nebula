using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipmentTracker.Extensions;
using ShipmentTracker.Models;
using ShipmentTracker.Models.Database;
using ShipmentTracker.Models.Requests;
using ShipmentTracker.Services;

namespace ShipmentTracker
{
	internal static class ApiEndpoints
	{
		public static async Task PushSubscribeAsync(HttpContext context,
													IDbContextFactory<ShipmentDbContext> dbFactory,
													[FromBody] StorePushSubscriptionModel model)
		{
			if (context.User.Identity?.IsAuthenticated != true)
			{
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				return;
			}

			int saved;

			var db = await dbFactory.CreateDbContextAsync(context.RequestAborted).ConfigureAwait(false);

			await using (db.ConfigureAwait(false))
			{
				var exists = await db.UsersPushSubscriptions
									 .AnyAsync((ups) => ups.Endpoint == model.Endpoint, context.RequestAborted)
									 .ConfigureAwait(false);

				if (!exists)
				{
					db.UsersPushSubscriptions.Add(new UserPushSubscription
					{
						UserId = context.User.GetClaimValue<int>(UserClaim.Id),
						Endpoint = model.Endpoint,
						P256dh = model.Keys.P256dh,
						Auth = model.Keys.Auth,
						Expires = model.Expiration,
					});

					saved = await db.SaveChangesAsync(context.RequestAborted).ConfigureAwait(false);
				}
				else
				{
					saved = 1;
				}
			}

			context.Response.StatusCode = saved == 1 ? StatusCodes.Status201Created : StatusCodes.Status500InternalServerError;
		}
	}
}
