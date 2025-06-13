using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShipmentTracker.Database;
using ShipmentTracker.Database.Extensions;
using ShipmentTracker.Database.Models;
using ShipmentTracker.Models.Requests;
using ShipmentTracker.WebPush;

namespace ShipmentTracker.Services
{
	public sealed class PushNotificationService
	{
		private readonly VapidOptions vapid;
		private readonly IDbContextFactory<ShipmentDbContext> dbFactory;

		public PushNotificationService(VapidOptions vapid, IDbContextFactory<ShipmentDbContext> dbFactory)
		{
			this.vapid = vapid;
			this.dbFactory = dbFactory;
		}

		public async ValueTask<UserPushSubscription> StoreSubscriptionAsync(StorePushSubscriptionModel model,
																			int userId,
																			CancellationToken token = default)
		{
			var db = await this.dbFactory.CreateDbContextAsync(token).ConfigureAwait(false);

			await using (db.ConfigureAwait(false))
			{
				var entity = db.UsersPushSubscriptions.Add(new UserPushSubscription
				{
					UserId = userId,
					Endpoint = model.Endpoint,
					Expires = model.Expiration,
					P256dh = model.Keys.P256dh,
					Auth = model.Keys.Auth,
				});

				var saved = await db.SaveChangesAsync(token).ConfigureAwait(false);

				Debug.Assert(saved == 1);

				return entity.Entity;
			}
		}

		public async IAsyncEnumerable<UserPushSubscription> GetSubscriptionsAsync(int userId,
																				  [EnumeratorCancellation] CancellationToken token =
																					  default)
		{
			var db = await this.dbFactory.CreateDbContextAsync(token).ConfigureAwait(false);

			await using (db.ConfigureAwait(false))
			{
				var enumerable = db.UsersPushSubscriptions.WhereUserId(userId).AsAsyncEnumerable();

				await foreach (var subscription in enumerable.WithCancellation(token).ConfigureAwait(false))
				{
					yield return subscription;
				}
			}
		}
	}
}
