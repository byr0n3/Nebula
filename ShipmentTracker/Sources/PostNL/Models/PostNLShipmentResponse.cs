using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ShipmentTracker.Sources.PostNL.Models
{
	public readonly struct PostNLShipmentResponse
	{
		[JsonPropertyName("colli")] public required Dictionary<string, PostNLShipment> Data { get; init; }
	}
}
