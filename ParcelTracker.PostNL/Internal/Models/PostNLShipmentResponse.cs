using System.Collections.Generic;
using System.Text.Json.Serialization;
using ParcelTracker.PostNL.Models;

namespace ParcelTracker.PostNL.Internal.Models
{
	public readonly struct PostNLShipmentResponse
	{
		[JsonPropertyName("colli")] public required Dictionary<string, PostNLShipment> Data { get; init; }
	}
}
