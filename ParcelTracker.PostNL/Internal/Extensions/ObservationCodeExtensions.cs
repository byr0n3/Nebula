using System.Runtime.CompilerServices;
using ParcelTracker.Common.Models;
using ParcelTracker.PostNL.Models;

namespace ParcelTracker.PostNL.Internal.Extensions
{
	internal static class ObservationCodeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ShipmentEventType ToShipmentEventType(this PostNLShipmentObservationCode @this) =>
			(@this) switch
			{
				PostNLShipmentObservationCode.Registered => ShipmentEventType.Registered,
				PostNLShipmentObservationCode.Received   => ShipmentEventType.Received,
				PostNLShipmentObservationCode.Sorted     => ShipmentEventType.Sorted,
				PostNLShipmentObservationCode.Delivery   => ShipmentEventType.OutForDelivery,
				_                                        => ShipmentEventType.Registered,
			};
	}
}
