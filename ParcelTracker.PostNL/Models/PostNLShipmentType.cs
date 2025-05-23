using System.Text.Json;
using System.Text.Json.Serialization;
using Elegance.Enums;

namespace ParcelTracker.PostNL.Models
{
	[Enum]
	[JsonConverter(typeof(PostNLShipmentTypeJsonConverter))]
	public enum PostNLShipmentType
	{
		Unknown,
		[EnumValue("Parcel")] Parcel,
		[EnumValue("LetterboxParcel")] LetterboxParcel,
	}

	internal sealed class PostNLShipmentTypeJsonConverter : JsonConverter<PostNLShipmentType>
	{
		public override PostNLShipmentType Read(ref Utf8JsonReader reader, System.Type _, JsonSerializerOptions __)
		{
			System.Span<char> buffer = stackalloc char[32];

			var copied = reader.CopyString(buffer);

			// @todo Update `Elegance.Enums` for Span support (byte and char)

			return PostNLShipmentTypeEnumData.FromValue(new string(buffer.Slice(0, copied)));
		}

		public override void Write(Utf8JsonWriter writer, PostNLShipmentType value, JsonSerializerOptions _) =>
			throw new System.NotSupportedException();
	}
}
