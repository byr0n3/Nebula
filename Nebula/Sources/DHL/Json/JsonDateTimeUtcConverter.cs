using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nebula.Sources.DHL.Json
{
	internal sealed class JsonDateTimeUtcConverter : JsonConverter<System.DateTime>
	{
		public override System.DateTime Read(ref Utf8JsonReader reader, System.Type _, JsonSerializerOptions __)
		{
			System.Span<char> buffer = stackalloc char[32];

			var read = reader.CopyString(buffer);

			buffer = buffer.Slice(0, read);

			return JsonDateTimeUtcConverter.Parse(buffer);
		}

		public override void Write(Utf8JsonWriter writer, System.DateTime value, JsonSerializerOptions options) =>
			throw new System.NotSupportedException();

		// @todo Use `TryParseExact` (theres at least 3 different formats)
		// - ending with Z
		// - ending with zzz
		// - ending with .msZ
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static System.DateTime Parse(scoped System.ReadOnlySpan<char> value) =>
			System.DateTime.SpecifyKind(
				System.DateTime.Parse(value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal),
				System.DateTimeKind.Utc
			);
	}
}
