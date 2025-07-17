using System.Globalization;

namespace Nebula.Utilities
{
	internal static class DateTimeParsing
	{
		public static System.DateTime GetDateTimeUtc(scoped System.ReadOnlySpan<char> value)
		{
			const string formatWithTz = "yyyy-MM-ddTHH:mm:sszzz";
			const string formatWithMsTz = "yyyy-MM-ddTHH:mm:ss.fffzzz";

			if (DateTimeParsing.TryGetAsUtc(value, formatWithTz, out var result) ||
				DateTimeParsing.TryGetAsUtc(value, formatWithMsTz, out result))
			{
				return result;
			}

			throw new System.ArgumentException($"Invalid DateTime string: {value}", nameof(value));
		}

		private static bool TryGetAsUtc(scoped System.ReadOnlySpan<char> value,
										scoped System.ReadOnlySpan<char> format,
										out System.DateTime result) =>
			System.DateTime.TryParseExact(value, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal, out result);
	}
}
