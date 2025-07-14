using System.Runtime.CompilerServices;
using Nebula.Models.Common;
using Nebula.Sources.PostNL.Models;

namespace Nebula.Sources.PostNL.Extensions
{
	internal static class PostNLPhaseTypeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ShipmentState ToShipmentState(this PostNLPhaseType @this) =>
			Unsafe.BitCast<PostNLPhaseType, ShipmentState>(@this);
	}
}
