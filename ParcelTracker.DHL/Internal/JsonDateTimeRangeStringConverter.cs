using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using ParcelTracker.Common;

namespace ParcelTracker.DHL.Internal
{
	internal sealed class JsonDateTimeRangeStringConverter : JsonConverter<Range<System.DateTime>>
	{
		public override Range<System.DateTime> Read(ref Utf8JsonReader reader, System.Type _, JsonSerializerOptions __)
		{
			const char separator = '/';

			System.Span<char> buffer = stackalloc char[64];

			var read = reader.CopyString(buffer);

			buffer = buffer.Slice(0, read);

			var separatorIdx = System.MemoryExtensions.IndexOf(buffer, separator);

			Debug.Assert(separatorIdx != -1, "Invalid range value");

			var lower = JsonDateTimeRangeStringConverter.ParseDateTime(buffer.Slice(0, separatorIdx));
			var upper = JsonDateTimeRangeStringConverter.ParseDateTime(buffer.Slice(separatorIdx + 1));

			return new Range<System.DateTime>(lower, upper);
		}

		public override void Write(Utf8JsonWriter writer, Range<System.DateTime> value, JsonSerializerOptions options) =>
			throw new System.NotSupportedException();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static System.DateTime ParseDateTime(scoped System.ReadOnlySpan<char> value) =>
			System.DateTime.TryParseExact(value,
										  "yyyy-MM-ddTHH:mm:sszzz",
										  DateTimeFormatInfo.InvariantInfo,
										  DateTimeStyles.RoundtripKind,
										  out var result)
				? result
				: throw new System.ArgumentException($"Value '{value}' is not in the correct format", nameof(value));
	}
}
