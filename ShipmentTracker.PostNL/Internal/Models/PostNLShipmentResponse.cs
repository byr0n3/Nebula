using System.Collections.Generic;
using System.Text.Json.Serialization;
using ShipmentTracker.PostNL.Models;

namespace ShipmentTracker.PostNL.Internal.Models
{
	public readonly struct PostNLShipmentResponse
	{
		[JsonPropertyName("colli")] public required Dictionary<string, PostNLShipment> Data { get; init; }
	}
}
