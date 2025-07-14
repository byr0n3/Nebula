using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Elegance.Enums;

namespace Nebula.Sources.DHL.Models
{
	[Enum]
	[JsonConverter(typeof(JsonDHLShipmentPhaseConverter2))]
	public enum DHLShipmentPhase
	{
		[EnumValue("UNKNOWN")] Unknown,
		[EnumValue("DATA_RECEIVED")] DataReceived,
		[EnumValue("UNDERWAY")] Underway,
		[EnumValue("IN_DELIVERY")] InDelivery,
		[EnumValue("DELIVERED")] Delivered,

		// Phase categories

		/// <summary>
		/// <p>Indicates an update/change coming from the receiver.</p>
		/// <p>For example, the receiver changes the requested delivery location/time.</p>
		/// </summary>
		[EnumValue("INTERVENTION")]
		Intervention,
	}

	internal sealed class JsonDHLShipmentPhaseConverter2 : JsonConverter<DHLShipmentPhase>
	{
		private static readonly JsonDHLShipmentPhaseConverter converter = new();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override DHLShipmentPhase Read(ref Utf8JsonReader reader, System.Type type, JsonSerializerOptions options) =>
			JsonDHLShipmentPhaseConverter2.converter.Read(ref reader, type, options);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(Utf8JsonWriter writer, DHLShipmentPhase value, JsonSerializerOptions options) =>
			JsonDHLShipmentPhaseConverter2.converter.Write(writer, value, options);
	}
}
