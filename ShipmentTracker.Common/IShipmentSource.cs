using System.Threading;
using System.Threading.Tasks;
using ShipmentTracker.Common.Models;

namespace ShipmentTracker.Common
{
	public interface IShipmentSource
	{
		public ShipmentSource Source { get; }

		/// <summary>
		/// Validate if the given <see cref="ShipmentRequest"/> can be handled by this source.
		/// </summary>
		/// <param name="request">The shipment request to validate.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns>
		/// <see langword="true"/> if the <see cref="ShipmentRequest"/> can be handled by this source, <see langword="false"/> otherwise.
		/// </returns>
		public ValueTask<bool> ValidateAsync(ShipmentRequest request, CancellationToken token = default);

		/// <summary>
		/// Fetch a shipment from this source.
		/// </summary>
		/// <param name="request">THe shipment request to fetch.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns>The retrieved shipment from the source (if it exists).</returns>
		public ValueTask<Shipment> GetShipmentAsync(ShipmentRequest request, CancellationToken token = default);
	}
}
