namespace Nebula
{
	internal static class TimeZones
	{
		public static readonly System.TimeZoneInfo[] Supported =
		[
			System.TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam"),
		];

		public static System.TimeZoneInfo Default =>
			TimeZones.Supported[0];
	}
}
