using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Elegance.Enums;

namespace ParcelTracker.PostNL.Models
{
	[Enum]
	[JsonConverter(typeof(JsonPostNLShipmentTypeConverter2))]
	public enum PostNLShipmentType
	{
		Unknown,
		[EnumValue("Parcel")] Parcel,
		[EnumValue("LetterboxParcel")] LetterboxParcel,
	}

	internal sealed class JsonPostNLShipmentTypeConverter2 : JsonConverter<PostNLShipmentType>
	{
		private static readonly JsonPostNLShipmentTypeConverter converter = new();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override PostNLShipmentType Read(ref Utf8JsonReader reader, System.Type type, JsonSerializerOptions options) =>
			JsonPostNLShipmentTypeConverter2.converter.Read(ref reader, type, options);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(Utf8JsonWriter writer, PostNLShipmentType value, JsonSerializerOptions options) =>
			JsonPostNLShipmentTypeConverter2.converter.Write(writer, value, options);
	}
}
