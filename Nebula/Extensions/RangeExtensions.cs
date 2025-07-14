using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using Nebula.Models;

namespace Nebula.Extensions
{
	internal static class RangeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Format(this Range @this, [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string? format = null) =>
			$"{@this.Lower.ToString(format, CultureInfo.CurrentCulture)} – {@this.Upper.ToString(format, CultureInfo.CurrentCulture)}";
	}
}
