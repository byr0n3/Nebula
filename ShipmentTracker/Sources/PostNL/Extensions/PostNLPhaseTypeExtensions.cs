using System.Runtime.CompilerServices;
using ShipmentTracker.Models.Common;
using ShipmentTracker.Sources.PostNL.Models;

namespace ShipmentTracker.Sources.PostNL.Extensions
{
	internal static class PostNLPhaseTypeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ShipmentState ToShipmentState(this PostNLPhaseType @this) =>
			Unsafe.BitCast<PostNLPhaseType, ShipmentState>(@this);
	}
}
