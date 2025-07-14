using System.Text.Json.Serialization;
using Nebula.Sources.PostNL.Models;

namespace Nebula.Sources.PostNL.Json
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
