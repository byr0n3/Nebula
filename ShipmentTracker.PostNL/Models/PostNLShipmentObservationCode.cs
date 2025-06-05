using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Elegance.Enums;

namespace ShipmentTracker.PostNL.Models
{
	[Enum]
	[JsonConverter(typeof(JsonPostNLShipmentObservationCodeConverter2))]
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

	internal sealed class JsonPostNLShipmentObservationCodeConverter2 : JsonConverter<PostNLShipmentObservationCode>
	{
		private static readonly JsonPostNLShipmentObservationCodeConverter converter = new();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override PostNLShipmentObservationCode Read(ref Utf8JsonReader reader, System.Type type, JsonSerializerOptions options) =>
			JsonPostNLShipmentObservationCodeConverter2.converter.Read(ref reader, type, options);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(Utf8JsonWriter writer, PostNLShipmentObservationCode value, JsonSerializerOptions options) =>
			JsonPostNLShipmentObservationCodeConverter2.converter.Write(writer, value, options);
	}
}
