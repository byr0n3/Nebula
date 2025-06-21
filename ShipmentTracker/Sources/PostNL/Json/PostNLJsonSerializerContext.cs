using System.Text.Json.Serialization;
using ShipmentTracker.Sources.PostNL.Models;

namespace ShipmentTracker.Sources.PostNL.Json
{
	[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata,
								 PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
								 DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
								 Converters =
								 [
									 typeof(JsonDateTimeUtcConverter),
								 ])]
	[JsonSerializable(typeof(PostNLShipmentResponse))]
	internal sealed partial class PostNLJsonSerializerContext : JsonSerializerContext;
}
