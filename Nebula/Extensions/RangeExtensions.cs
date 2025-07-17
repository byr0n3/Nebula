using System.Runtime.CompilerServices;
using Nebula.Models;

namespace Nebula.Extensions
{
	internal static class RangeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Range ToTimeZone(this Range @this, System.TimeZoneInfo timeZone) =>
			new(@this.Lower.ToTimeZone(timeZone), @this.Upper.ToTimeZone(timeZone));
	}
}
