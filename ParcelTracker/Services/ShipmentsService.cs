using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ParcelTracker.Common;
using ParcelTracker.Common.Models;

namespace ParcelTracker.Services
{
	public sealed class ShipmentsService
	{
		private readonly IShipmentSource[] sources;
		private readonly ILogger<ShipmentsService> logger;

		public ShipmentsService(System.IServiceProvider services, ILogger<ShipmentsService> logger)
		{
			this.logger = logger;
			this.sources = services.GetServices<IShipmentSource>().ToArray();

			this.logger.LogDebug("ShipmentsService initialized with {SourcesCount} sources", this.sources.Length);
		}

		public async ValueTask<Shipment> GetShipmentAsync(ShipmentRequest request, CancellationToken token = default)
		{
			foreach (var source in this.sources)
			{
				if ((request.Source != default) && (request.Source != source.Source))
				{
					continue;
				}

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
	}
}
