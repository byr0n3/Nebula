using System.Text.Json.Serialization;
using Nebula.Sources.DHL.Models;

namespace Nebula.Sources.DHL.Json
{
	[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata,
								 PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
								 DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonSerializable(typeof(DHLShipment[]))]
	internal sealed partial class DHLJsonSerializerContext : JsonSerializerContext;
}
