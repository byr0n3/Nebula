using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Elegance.Enums;

namespace ShipmentTracker.WebPush
{
	[Enum]
	[JsonConverter(typeof(JsonPushNotificationUrgencyConverter2))]
	public enum PushNotificationUrgency
	{
		[EnumValue("normal")] Normal,
		[EnumValue("very-low")] VeryLow,
		[EnumValue("low")] Low,
		[EnumValue("high")] High,
	}

	// `System.Text.Json` doesn't play nice with source-generated converters.
	internal sealed class JsonPushNotificationUrgencyConverter2 : JsonConverter<PushNotificationUrgency>
	{
		private static readonly JsonPushNotificationUrgencyConverter converter = new();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override PushNotificationUrgency Read(ref Utf8JsonReader reader, System.Type type, JsonSerializerOptions options) =>
			JsonPushNotificationUrgencyConverter2.converter.Read(ref reader, type, options);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(Utf8JsonWriter writer, PushNotificationUrgency value, JsonSerializerOptions options) =>
			JsonPushNotificationUrgencyConverter2.converter.Write(writer, value, options);
	}
}
