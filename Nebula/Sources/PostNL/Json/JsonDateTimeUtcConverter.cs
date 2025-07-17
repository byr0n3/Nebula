using System.Text.Json;
using System.Text.Json.Serialization;
using Nebula.Utilities;

namespace Nebula.Sources.PostNL.Json
{
	internal sealed class JsonDateTimeUtcConverter : JsonConverter<System.DateTime>
	{
		public override System.DateTime Read(ref Utf8JsonReader reader, System.Type _, JsonSerializerOptions __)
		{
			System.Span<char> buffer = stackalloc char[32];

			var read = reader.CopyString(buffer);

			buffer = buffer.Slice(0, read);

			return DateTimeParsing.GetDateTimeUtc(buffer);
		}

		public override void Write(Utf8JsonWriter writer, System.DateTime value, JsonSerializerOptions options) =>
			throw new System.NotSupportedException();
	}
}
