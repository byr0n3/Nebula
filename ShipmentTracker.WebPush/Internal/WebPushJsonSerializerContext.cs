using System.Text.Json.Serialization;
using ShipmentTracker.WebPush.Internal.Models;

namespace ShipmentTracker.WebPush.Internal
{
	[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata,
								 PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
	[JsonSerializable(typeof(PushNotification))]
	[JsonSerializable(typeof(JwtInfo))]
	[JsonSerializable(typeof(JwtData))]
	[JsonSerializable(typeof(DeclarativePushNotification))]
	internal sealed partial class WebPushJsonSerializerContext : JsonSerializerContext;
}
