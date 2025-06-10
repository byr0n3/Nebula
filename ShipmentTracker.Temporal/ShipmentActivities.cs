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

		[Activity]
		public Task StartWorkflowAsync(TrackShipmentArguments arguments) =>
			this.client.StartShipmentWorkflowAsync(arguments);

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

		[Activity]
		public async Task<bool> UpdateShipmentAsync(int shipmentId, Shipment shipment)
		{
			var db = await this.dbFactory.CreateDbContextAsync().ConfigureAwait(false);

			await using (db.ConfigureAwait(false))
			{
				NpgsqlRange<System.DateTime>? eta = shipment.Eta != default
					? new NpgsqlRange<System.DateTime>(shipment.Eta.Lower, true, shipment.Eta.Upper, true)
					: null;

				var updated = await db.Shipments
									  .WhereId(shipmentId)
									  .ExecuteUpdateAsync((calls) =>
															  calls.SetProperty(static (s) => s.State, shipment.State)
																   .SetProperty(static (s) => s.Eta, eta)
																   .SetProperty(static (s) => s.Arrived, shipment.Arrived))
									  .ConfigureAwait(false);

				return updated == 1;
			}
		}
	}
}
