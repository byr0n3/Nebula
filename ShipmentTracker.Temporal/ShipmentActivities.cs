using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NpgsqlTypes;
using ShipmentTracker.Common;
using ShipmentTracker.Common.Models;
using ShipmentTracker.Database;
using ShipmentTracker.Database.Extensions;
using ShipmentTracker.Temporal.Extensions;
using Temporalio.Activities;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace ShipmentTracker.Temporal
{
	internal sealed class ShipmentActivities
	{
		private readonly ITemporalClient client;
		private readonly IShipmentSource[] sources;
		private readonly IDbContextFactory<ShipmentDbContext> dbFactory;

		public ShipmentActivities(ITemporalClient client, System.IServiceProvider services, IDbContextFactory<ShipmentDbContext> dbFactory)
		{
			this.client = client;
			this.dbFactory = dbFactory;
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

			var shipment = await source.GetShipmentAsync(request).ConfigureAwait(false);

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
			var db = await this.dbFactory.CreateDbContextAsync().ConfigureAwait(false);

			await using (db.ConfigureAwait(false))
			{
				NpgsqlRange<System.DateTime>? eta = shipment.Eta != default
					? new NpgsqlRange<System.DateTime>(shipment.Eta.Lower, true, shipment.Eta.Upper, true)
					: null;

				// We use EF Core's change tracker to detect if the shipment's data actually updated.
				var entity = await db.Shipments
									 .WhereId(shipmentId)
									 .AsTracking()
									 .FirstOrDefaultAsync()
									 .ConfigureAwait(false);

				if (entity is null)
				{
					return false;
				}

				entity.State = shipment.State;
				entity.Eta = eta;
				entity.Arrived = shipment.Arrived;

				var updated = await db.SaveChangesAsync().ConfigureAwait(false);

				return updated == 1;
			}
		}
	}
}
