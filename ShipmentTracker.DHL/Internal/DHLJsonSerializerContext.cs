using System.Text.Json.Serialization;
using ShipmentTracker.DHL.Models;

namespace ShipmentTracker.DHL.Internal
{
	[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata,
								 PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
								 DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
								 Converters =
								 [
									 typeof(JsonDateTimeUtcConverter),
								 ])]
	[JsonSerializable(typeof(DHLShipment[]))]
	internal sealed partial class DHLJsonSerializerContext : JsonSerializerContext;
}
