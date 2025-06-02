using System.Runtime.CompilerServices;
using ParcelTracker.Common.Models;
using ParcelTracker.DHL.Models;

namespace ParcelTracker.DHL.Internal.Extensions
{
	internal static class DHLShipmentPhaseExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ShipmentState ToShipmentState(this DHLShipmentPhase @this) =>
			(@this) switch
			{
				DHLShipmentPhase.Underway   => ShipmentState.Received,
				DHLShipmentPhase.InDelivery => ShipmentState.OutForDelivery,
				DHLShipmentPhase.Delivered  => ShipmentState.Delivered,
				_                           => ShipmentState.Registered,
			};
	}
}
