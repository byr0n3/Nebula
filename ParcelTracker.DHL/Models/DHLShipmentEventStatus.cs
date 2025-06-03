using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Elegance.Enums;

namespace ParcelTracker.DHL.Models
{
	[Enum]
	[JsonConverter(typeof(JsonDHLShipmentEventStatusConverter2))]
	public enum DHLShipmentEventStatus
	{
		Unknown,

		[EnumValue("PRENOTIFICATION_RECEIVED")]
		Registered,

		[EnumValue("PARCEL_SORTED_AT_HUB")] Received,

		[EnumValue("PARCEL_ARRIVED_AT_LOCAL_DEPOT")]
		Sorted,

		[EnumValue("OUT_FOR_DELIVERY")] OutForDelivery,

		[EnumValue("DELIVERED")] Delivered,

		[EnumValue("INTERVENTION_RECEIVER_REQUESTS_DELIVERY_AT_SAFEPLACE")]
		DeliveryLocationUpdated,
	}

	internal sealed class JsonDHLShipmentEventStatusConverter2 : JsonConverter<DHLShipmentEventStatus>
	{
		private static readonly JsonDHLShipmentEventStatusConverter converter = new();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override DHLShipmentEventStatus Read(ref Utf8JsonReader reader, System.Type type, JsonSerializerOptions options) =>
			JsonDHLShipmentEventStatusConverter2.converter.Read(ref reader, type, options);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(Utf8JsonWriter writer, DHLShipmentEventStatus value, JsonSerializerOptions options) =>
			JsonDHLShipmentEventStatusConverter2.converter.Write(writer, value, options);
	}
}
