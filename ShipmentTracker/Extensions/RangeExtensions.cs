using System.Globalization;
using System.Runtime.CompilerServices;
using ShipmentTracker.Models;

namespace ShipmentTracker.Extensions
{
	internal static class RangeExtensions
	{
		// @todo Use user's culture
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string FormatTime(this Range<System.DateTime> @this) =>
			$"{@this.Lower.ToString("t", CultureInfo.InvariantCulture)} – {@this.Upper.ToString("t", CultureInfo.InvariantCulture)}";
	}
}
