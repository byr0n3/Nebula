using System.Runtime.CompilerServices;
using Nebula.Models.Common;

namespace Nebula.Extensions
{
	internal static class ShipmentStateExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Str(this ShipmentState @this) =>
			ShipmentStateEnumData.GetValue(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Str(this ShipmentEventType @this) =>
			ShipmentEventTypeEnumData.GetValue(@this);
	}
}
