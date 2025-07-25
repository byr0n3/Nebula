using System.Runtime.CompilerServices;
using Nebula.Models.Common;

namespace Nebula.Extensions
{
	internal static class ShipmentEventTypeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ShipmentState ToShipmentState(this ShipmentEventType @this) =>
			(@this) switch
			{
				ShipmentEventType.Registered     => ShipmentState.Registered,
				ShipmentEventType.Received       => ShipmentState.Received,
				ShipmentEventType.Sorted         => ShipmentState.Sorted,
				ShipmentEventType.OutForDelivery => ShipmentState.OutForDelivery,
				ShipmentEventType.Delivered      => ShipmentState.Delivered,
				_                                => ShipmentState.Registered,
			};
	}
}
