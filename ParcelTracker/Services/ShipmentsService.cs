using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ParcelTracker.Common;
using ParcelTracker.Common.Models;
using ParcelTracker.Database;
using ParcelTracker.Database.Models;
using ParcelTracker.Models.Dto;
using Shipment = ParcelTracker.Common.Models.Shipment;

namespace ParcelTracker.Services
{
	public sealed class ShipmentsService
	{
		private readonly IShipmentSource[] sources;
		private readonly ILogger<ShipmentsService> logger;
		private readonly IDbContextFactory<ShipmentDbContext> dbFactory;

		public ShipmentsService(System.IServiceProvider services,
								ILogger<ShipmentsService> logger,
								IDbContextFactory<ShipmentDbContext> dbFactory)
		{
			this.logger = logger;
			this.dbFactory = dbFactory;
			this.sources = services.GetServices<IShipmentSource>().ToArray();

			this.logger.LogDebug("ShipmentsService initialized with {SourcesCount} sources", this.sources.Length);
		}

		public async ValueTask<Shipment> GetShipmentAsync(ShipmentRequest request, CancellationToken token = default)
		{
			if (request.Source != default)
			{
				var source = this.sources.First((source) => source.Source == request.Source);

				return await source.GetShipmentAsync(request, token).ConfigureAwait(false);
			}

			foreach (var source in this.sources)
			{
				var valid = await source.ValidateAsync(request, token).ConfigureAwait(false);

				if (!valid)
				{
					continue;
				}

				this.logger.LogDebug("Shipment found at source: {Source}", source.Source);

				return await source.GetShipmentAsync(request, token).ConfigureAwait(false);
			}

			this.logger.LogDebug("Shipment not found at any source: {Code}–{ZipCode}", request.Code, request.ZipCode);
			return default;
		}

		public async ValueTask<UserShipmentDto> GetOrCreateUserShipmentDtoAsync(Shipment shipment,
																				int userId,
																				CancellationToken token = default)
		{
			var db = await this.dbFactory.CreateDbContextAsync(token).ConfigureAwait(false);

			await using (db.ConfigureAwait(false))
			{
				var shipmentId = await ShipmentsService.GetOrCreateShipmentIdAsync(db, shipment, token).ConfigureAwait(false);

				var subscribed = await db.UsersShipments
										 .AnyAsync(
											  (us) => (us.UserId == userId) && (us.ShipmentId == shipmentId),
											  token
										  )
										 .ConfigureAwait(false);

				return new UserShipmentDto
				{
					ShipmentId = shipmentId,
					Subscribed = subscribed,
				};
			}
		}

		public async Task SubscribeUserAsync(int shipmentId, int userId, CancellationToken token = default)
		{
			var db = await this.dbFactory.CreateDbContextAsync(token).ConfigureAwait(false);

			await using (db.ConfigureAwait(false))
			{
				db.UsersShipments.Add(new UserShipment
				{
					UserId = userId,
					ShipmentId = shipmentId,
				});

				await db.SaveChangesAsync(token).ConfigureAwait(false);
			}
		}

		public async Task UnsubscribeUserAsync(int shipmentId, int userId, CancellationToken token = default)
		{
			var db = await this.dbFactory.CreateDbContextAsync(token).ConfigureAwait(false);

			await using (db.ConfigureAwait(false))
			{
				await db.UsersShipments
						.Where((us) => (us.UserId == userId) && (us.ShipmentId == shipmentId))
						.ExecuteDeleteAsync(token)
						.ConfigureAwait(false);
			}
		}

		private static async ValueTask<int> GetOrCreateShipmentIdAsync(ShipmentDbContext db,
																	   Shipment shipment,
																	   CancellationToken token = default)
		{
			var shipmentId = await db.Shipments
									 .Where((s) => (s.Code == shipment.TrackingCode) && (s.Source == shipment.Source))
									 .Select(static (s) => s.Id)
									 .FirstOrDefaultAsync(token)
									 .ConfigureAwait(false);

			if (shipmentId == default)
			{
				var entity = db.Shipments.Add(shipment);

				await db.SaveChangesAsync(token).ConfigureAwait(false);

				shipmentId = entity.Entity.Id;
			}

			return shipmentId;
		}
	}
}
