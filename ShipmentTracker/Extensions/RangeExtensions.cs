using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using ShipmentTracker.Models;

namespace ShipmentTracker.Extensions
{
	internal static class RangeExtensions
	{
		// @todo Use user's culture
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format(this Range<System.DateTime> @this,
									[StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
									string? format = null) =>
			$"{@this.Lower.ToString(format, CultureInfo.InvariantCulture)} – {@this.Upper.ToString(format, CultureInfo.InvariantCulture)}";
	}
}
