using System.Runtime.CompilerServices;
using ParcelTracker.Common.Models;
using ParcelTracker.PostNL.Models;

namespace ParcelTracker.PostNL.Internal.Extensions
{
	internal static class ObservationCodeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ShipmentState ToShipmentState(this PostNLShipmentObservationCode @this) =>
			(@this) switch
			{
				PostNLShipmentObservationCode.Registered => ShipmentState.Registered,
				PostNLShipmentObservationCode.Received   => ShipmentState.Received,
				PostNLShipmentObservationCode.Sorted     => ShipmentState.Sorted,
				PostNLShipmentObservationCode.Delivery   => ShipmentState.OutForDelivery,
				_                                        => ShipmentState.Registered,
			};
	}
}
