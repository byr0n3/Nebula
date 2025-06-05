using System.Runtime.CompilerServices;
using ShipmentTracker.Common.Models;
using ShipmentTracker.PostNL.Models;

namespace ShipmentTracker.PostNL.Internal.Extensions
{
	internal static class PhaseTypeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ShipmentState ToShipmentState(this PostNLPhaseType @this) =>
			Unsafe.BitCast<PostNLPhaseType, ShipmentState>(@this);
	}
}
