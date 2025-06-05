using System.Text.Json.Serialization;
using ShipmentTracker.PostNL.Internal.Models;

namespace ShipmentTracker.PostNL.Internal
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
