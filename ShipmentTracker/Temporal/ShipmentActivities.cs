using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elegance.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NpgsqlTypes;
using ShipmentTracker.Extensions;
using ShipmentTracker.Models.Common;
using ShipmentTracker.Services;
using ShipmentTracker.Sources;
using ShipmentTracker.Temporal.Extensions;
using Temporalio.Activities;
using Temporalio.Client;
using Temporalio.Exceptions;
using Vapid.NET.Models;

namespace ShipmentTracker.Temporal
{
	internal sealed class ShipmentActivities
	{
		private static CancellationToken CancelToken =>
			ActivityExecutionContext.Current.CancellationToken;

		private readonly ITemporalClient client;
		private readonly IShipmentSource[] sources;
		private readonly PushNotificationsService pushNotifications;
		private readonly IDbContextFactory<ShipmentDbContext> dbFactory;

		public ShipmentActivities(ITemporalClient client,
								  System.IServiceProvider services,
								  PushNotificationsService pushNotifications,
								  IDbContextFactory<ShipmentDbContext> dbFactory)
		{
			this.client = client;
			this.dbFactory = dbFactory;
			this.pushNotifications = pushNotifications;
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
		public Task SendPushNotificationsAsync(int shipmentId, Shipment shipment)
		{
			var notification = new PushNotification
			{
				Title = shipment.TrackingCode,
				Body = ShipmentActivities.GetNotificationBody(in shipment),
				Navigate = $"/shipments/{shipment.TrackingCode}/{shipment.Recipient.ZipCode}",
				Topic = $"shipment-{shipmentId.Str()}",
			};

			return this.pushNotifications.SendNotificationsAsync(shipmentId, notification, ShipmentActivities.CancelToken);
		}

		// @todo Localized
		private static string GetNotificationBody(in Shipment shipment) =>
			(shipment.State) switch
			{
				ShipmentState.Received       => "Your shipment has been received by the delivery company.",
				ShipmentState.Sorted         => "Your shipment has been sorted.",
				ShipmentState.OutForDelivery => $"Out for delivery: {shipment.Eta.FormatTime()}",
				ShipmentState.Delivered      => "Your shipment has been delivered!",
				_                            => "The shipment's information has been updated.",
			};
	}
}
