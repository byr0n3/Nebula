using System.Text.Json.Serialization;
using ParcelTracker.PostNL.Internal.Models;

namespace ParcelTracker.PostNL.Internal
{
	[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata,
								 PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
								 DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonSerializable(typeof(PostNLShipmentResponse))]
	internal sealed partial class PostNLJsonSerializerContext : JsonSerializerContext;
}
