using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShipmentTracker.Extensions;
using ShipmentTracker.Models.Database;
using ShipmentTracker.Models.Requests;
using Vapid.NET;
using Vapid.NET.Models;

namespace ShipmentTracker.Services
{
	public sealed class PushNotificationsService
	{
		private readonly VapidClient client;
		private readonly IDbContextFactory<ShipmentDbContext> dbFactory;

		public PushNotificationsService(VapidClient client, IDbContextFactory<ShipmentDbContext> dbFactory)
		{
			this.client = client;
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

		public async Task SendNotificationsAsync(int shipmentId, PushNotification notification, CancellationToken token = default)
		{
			var db = await this.dbFactory.CreateDbContextAsync(token).ConfigureAwait(false);

			await using (db.ConfigureAwait(false))
			{
				var subscriptions = db.UsersPushSubscriptions
									  .Where((ups) => db.UsersShipments
														.Any((us) => (us.ShipmentId == shipmentId) && (us.UserId == ups.UserId)))
									  .AsAsyncEnumerable();

				var tasks = new List<Task>();

				await foreach (var subscription in subscriptions.WithCancellation(token).ConfigureAwait(false))
				{
					if (subscription.Expires <= System.DateTime.UtcNow)
					{
						db.UsersPushSubscriptions.Remove(subscription);
						continue;
					}

					tasks.Add(this.client.SendAsync(subscription, notification, token));
				}

				await db.SaveChangesAsync(token).ConfigureAwait(false);

				await Task.WhenAll(tasks).ConfigureAwait(false);
			}
		}
	}
}
