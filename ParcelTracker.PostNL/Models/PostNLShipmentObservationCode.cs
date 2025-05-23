using System.Text.Json;
using System.Text.Json.Serialization;
using Elegance.Enums;

namespace ParcelTracker.PostNL.Models
{
	[Enum]
	[JsonConverter(typeof(PostNLShipmentObservationCodeJsonConverter))]
	public enum PostNLShipmentObservationCode
	{
		Unknown,

		/// <summary>
		/// Shipment expected, but not yet arrived or processed at PostNL.
		/// </summary>
		[EnumValue("A01")]
		Registered,

		/// <summary>
		/// Initial calculation of Estimated Time of Arrival.
		/// </summary>
		[EnumValue("A18")]
		InitialEtaCalculated,

		/// <summary>
		/// Delivery cannot be changed.
		/// </summary>
		[EnumValue("A95")]
		DeliveryFixed,

		/// <summary>
		/// Pre-alerted shipment enriched by PostNL management.
		/// </summary>
		[EnumValue("A98")]
		PreAlert,

		/// <summary>
		/// Shipment received by PostNL.
		/// </summary>
		[EnumValue("B01")]
		Received,

		/// <summary>
		/// Shipment has been sorted.
		/// </summary>
		[EnumValue("J01")]
		Sorted,

		/// <summary>
		/// Driver is en route.
		/// </summary>
		[EnumValue("J05")]
		Delivery,

		/// <summary>
		/// The shipment has arrived in the destination country.
		/// </summary>
		[EnumValue("X02")]
		ArrivedInDestinationCountry,

		/// <summary>
		/// Customs clearance in progress.
		/// </summary>
		[EnumValue("X03")]
		InClearance,
	}

	internal sealed class PostNLShipmentObservationCodeJsonConverter : JsonConverter<PostNLShipmentObservationCode>
	{
		public override PostNLShipmentObservationCode Read(ref Utf8JsonReader reader, System.Type _, JsonSerializerOptions __)
		{
			System.Span<char> buffer = stackalloc char[32];

			var copied = reader.CopyString(buffer);

			// @todo Update `Elegance.Enums` for Span support (byte and char)

			return PostNLShipmentObservationCodeEnumData.FromValue(new string(buffer.Slice(0, copied)));
		}

		public override void Write(Utf8JsonWriter writer, PostNLShipmentObservationCode value, JsonSerializerOptions _) =>
			throw new System.NotSupportedException();
	}
}
