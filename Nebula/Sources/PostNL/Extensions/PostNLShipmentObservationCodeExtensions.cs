using System.Runtime.CompilerServices;
using Nebula.Models.Common;
using Nebula.Sources.PostNL.Models;

namespace Nebula.Sources.PostNL.Extensions
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
