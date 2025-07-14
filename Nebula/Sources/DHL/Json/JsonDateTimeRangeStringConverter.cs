using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nebula.Models;

namespace Nebula.Sources.DHL.Json
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

			var lower = JsonDateTimeUtcConverter.Parse(buffer.Slice(0, separatorIdx));
			var upper = JsonDateTimeUtcConverter.Parse(buffer.Slice(separatorIdx + 1));

			return new Range<System.DateTime>(lower, upper);
		}

		public override void Write(Utf8JsonWriter writer, Range<System.DateTime> value, JsonSerializerOptions options) =>
			throw new System.NotSupportedException();
	}
}
