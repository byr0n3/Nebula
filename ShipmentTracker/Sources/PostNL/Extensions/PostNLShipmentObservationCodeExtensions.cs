using System.Runtime.CompilerServices;
using ShipmentTracker.Models.Common;
using ShipmentTracker.Sources.PostNL.Models;

namespace ShipmentTracker.Sources.PostNL.Extensions
{
	internal static class PostNLShipmentObservationCodeExtensions
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
