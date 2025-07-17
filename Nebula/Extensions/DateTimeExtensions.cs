using System.Diagnostics;

namespace Nebula.Extensions
{
	internal static class DateTimeExtensions
	{
		public static System.DateTime ToTimeZone(this System.DateTime @this, System.TimeZoneInfo timeZone)
		{
			Debug.Assert(@this.Kind != System.DateTimeKind.Local);

			return System.DateTime.SpecifyKind(@this.Add(timeZone.GetUtcOffset(@this)), System.DateTimeKind.Local);
		}
	}
}
