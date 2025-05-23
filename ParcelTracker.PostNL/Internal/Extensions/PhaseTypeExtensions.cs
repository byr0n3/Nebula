using System.Runtime.CompilerServices;
using ParcelTracker.Common.Models;
using ParcelTracker.PostNL.Models;

namespace ParcelTracker.PostNL.Internal.Extensions
{
	internal static class PhaseTypeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ShipmentState ToShipmentState(this PostNLPhaseType @this) =>
			(@this) switch
			{
				PostNLPhaseType.Registered => ShipmentState.Registered,
				PostNLPhaseType.Sorted     => ShipmentState.Sorted,
				PostNLPhaseType.Delivered  => ShipmentState.Delivered,
				_                          => ShipmentState.Registered,
			};
	}
}
