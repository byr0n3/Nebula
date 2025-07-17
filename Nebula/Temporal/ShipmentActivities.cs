using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elegance.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using NpgsqlTypes;
using Nebula.Extensions;
using Nebula.Models.Common;
using Nebula.Resources;
using Nebula.Services;
using Nebula.Sources;
using Nebula.Temporal.Extensions;
using Temporalio.Activities;
using Temporalio.Client;
using Temporalio.Exceptions;
using Vapid.NET;
using Vapid.NET.Models;

namespace Nebula.Temporal
{
	internal sealed class ShipmentActivities
	{
		private static CancellationToken CancelToken =>
			ActivityExecutionContext.Current.CancellationToken;

		private readonly VapidClient vapid;
		private readonly ITemporalClient client;
		private readonly IShipmentSource[] sources;
		private readonly IDbContextFactory<ShipmentDbContext> dbFactory;
		private readonly IStringLocalizer<PushNotifications> notificationLocalizer;

		public ShipmentActivities(VapidClient vapid,
								  ITemporalClient client,
								  System.IServiceProvider services,
								  IDbContextFactory<ShipmentDbContext> dbFactory,
								  IStringLocalizer<PushNotifications> notificationLocalizer)
		{
			this.vapid = vapid;
			this.client = client;
			this.dbFactory = dbFactory;
			this.notificationLocalizer = notificationLocalizer;
			this.sources = services.GetServices<IShipmentSource>().ToArray();
		}

		/// <inheritdoc cref="TemporalClientExtensions.StartShipmentWorkflowAsync"/>
		[Activity]
		public Task StartWorkflowAsync(TrackShipmentArguments arguments) =>
			this.client.StartShipmentWorkflowAsync(arguments);

		/// <summary>
		/// Get a shipment from the delivery service, based on the given <paramref name="arguments"/>.
		/// </summary>
		/// <param name="arguments">The arguments to use to find the shipment.</param>
		/// <returns>The found shipment/</returns>
		/// <exception cref="ApplicationFailureException">If there was no suitable source found, or the shipment wasn't found at the source.</exception>
		[Activity]
		public async Task<Shipment> GetShipmentAsync(TrackShipmentArguments arguments)
		{
			var source = this.sources.FirstOrDefault((s) => s.Source == arguments.Source);

			if (source is null)
			{
				throw new ApplicationFailureException("No shipment source found", details: [arguments.Source, arguments.Code]);
			}

			var request = new ShipmentRequest
			{
				Code = arguments.Code,
				ZipCode = arguments.ZipCode,
			};

			var shipment = await source.GetShipmentAsync(request, ShipmentActivities.CancelToken).ConfigureAwait(false);

			if (shipment == default)
			{
				throw new ApplicationFailureException("No shipment found", details: [arguments.Source, arguments.Code]);
			}

			return shipment;
		}

		/// <summary>
		/// Update a given shipment in the database.
		/// </summary>
		/// <param name="shipmentId">The ID of the database shipment to update.</param>
		/// <param name="shipment">The actual shipment from the delivery service.</param>
		/// <returns><see langword="true"/> if the shipment had its data updated, <see langword="false"/> otherwise.</returns>
		[Activity]
		public async Task<bool> UpdateShipmentAsync(int shipmentId, Shipment shipment)
		{
			var db = await this.dbFactory.CreateDbContextAsync(ShipmentActivities.CancelToken).ConfigureAwait(false);

			await using (db.ConfigureAwait(false))
			{
				NpgsqlRange<System.DateTime>? eta = shipment.Eta != default
					? new NpgsqlRange<System.DateTime>(shipment.Eta.Lower, true, shipment.Eta.Upper, true)
					: null;

				// We use EF Core's change tracker to detect if the shipment's data actually updated.
				var entity = await db.Shipments
									 .WhereId(shipmentId)
									 .AsTracking()
									 .FirstOrDefaultAsync(ShipmentActivities.CancelToken)
									 .ConfigureAwait(false);

				if (entity is null)
				{
					return false;
				}

				entity.State = shipment.State;
				entity.Eta = eta;
				entity.Arrived = shipment.Arrived;

				var updated = await db.SaveChangesAsync(ShipmentActivities.CancelToken).ConfigureAwait(false);

				return updated == 1;
			}
		}

		[Activity]
		public async Task SendPushNotificationsAsync(int shipmentId, Shipment shipment)
		{
			var token = ShipmentActivities.CancelToken;

			var db = await this.dbFactory.CreateDbContextAsync(token).ConfigureAwait(false);

			await using (db.ConfigureAwait(false))
			{
				var subscriptions = db.UsersPushSubscriptions
									  .Where((ups) => db.UsersShipments
														.Any((us) => (us.ShipmentId == shipmentId) && (us.UserId == ups.UserId)))
									  .Join(
										   db.Users,
										   static (ups) => ups.UserId,
										   static (u) => u.Id,
										   static (ups, u) => new
										   {
											   Subscription = ups,
											   Culture = new UserCulture
											   {
												   Culture = u.Culture,
												   UiCulture = u.UiCulture,
												   TimeZone = u.TimeZone,
											   },
										   }
									   )
									  .AsAsyncEnumerable();

				var tasks = new List<Task>();

				await foreach (var subscription in subscriptions.WithCancellation(token).ConfigureAwait(false))
				{
					if (subscription.Subscription.Expires <= System.DateTime.UtcNow)
					{
						db.UsersPushSubscriptions.Remove(subscription.Subscription);
						continue;
					}

					var notification = new PushNotification
					{
						Title = shipment.TrackingCode,
						Body = this.GetNotificationBody(in shipment, subscription.Culture),
						Navigate = $"/shipments/{shipment.TrackingCode}/{shipment.Recipient.ZipCode}",
						Topic = $"shipment-{shipmentId.Str()}",
					};

					tasks.Add(this.vapid.SendAsync(subscription.Subscription, notification, token));
				}

				await db.SaveChangesAsync(token).ConfigureAwait(false);

				await Task.WhenAll(tasks).ConfigureAwait(false);
			}
		}

		private string GetNotificationBody(in Shipment shipment, UserCulture culture)
		{
			CultureInfo.CurrentCulture = new CultureInfo(culture.Culture);
			CultureInfo.CurrentUICulture = new CultureInfo(culture.UiCulture);

			var msg = this.notificationLocalizer[shipment.State.Str()].Value;

			var timeZone = System.TimeZoneInfo.FindSystemTimeZoneById(culture.TimeZone);

			if ((shipment.Eta != default) && (shipment.State != ShipmentState.Delivered))
			{
				var eta = shipment.Eta.ToTimeZone(timeZone);

				msg = shipment.State == ShipmentState.OutForDelivery
					? string.Create(null, $"{msg} {eta:t}")
					: string.Create(null, $"{msg} {eta:g}");
			}

			return msg;
		}

		private readonly struct UserCulture
		{
			public required string Culture { get; init; }

			public required string UiCulture { get; init; }

			public required string TimeZone { get; init; }
		}
	}
}
