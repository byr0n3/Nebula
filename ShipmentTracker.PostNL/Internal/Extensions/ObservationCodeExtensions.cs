using System.Runtime.CompilerServices;
using ShipmentTracker.Common.Models;
using ShipmentTracker.PostNL.Models;

namespace ShipmentTracker.PostNL.Internal.Extensions
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
