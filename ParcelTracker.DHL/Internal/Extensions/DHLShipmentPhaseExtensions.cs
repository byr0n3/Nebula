using System.Runtime.CompilerServices;
using ParcelTracker.Common.Models;
using ParcelTracker.DHL.Models;

namespace ParcelTracker.DHL.Internal.Extensions
{
	internal static class DHLShipmentPhaseExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ShipmentEventType ToShipmentEventType(this DHLShipmentPhase @this) =>
			(@this) switch
			{
				DHLShipmentPhase.DataReceived => ShipmentEventType.Registered,
				DHLShipmentPhase.Underway     => ShipmentEventType.Received,
				DHLShipmentPhase.InDelivery   => ShipmentEventType.OutForDelivery,
				DHLShipmentPhase.Delivered    => ShipmentEventType.Delivered,
				DHLShipmentPhase.Intervention => ShipmentEventType.InformationUpdate,
				_                             => ShipmentEventType.Registered,
			};
	}
}
