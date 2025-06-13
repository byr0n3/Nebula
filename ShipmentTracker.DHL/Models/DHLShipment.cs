using System.Text.Json.Serialization;
using ShipmentTracker.Common;
using ShipmentTracker.DHL.Internal;

namespace ShipmentTracker.DHL.Models
{
	public readonly struct DHLShipment : System.IEquatable<DHLShipment>
	{
		public required string Id { get; init; }

		[JsonPropertyName("barcodes")] public required string[] TrackingCodes { get; init; }

		// @todo Enum
		public required string Type { get; init; }

		public required System.DateTime Created { get; init; }

		[JsonPropertyName("lastUpdated")] public required System.DateTime Updated { get; init; }

		[JsonPropertyName("deliveredAt")] public System.DateTime DeliveryDate { get; init; }

		[JsonPropertyName("receiver")] public required DHLShipmentContact Recipient { get; init; }

		public DHLShipmentContact Origin { get; init; }

		[JsonPropertyName("shipper")] public required DHLShipmentShipper Sender { get; init; }

		[JsonPropertyName("events")] public required DHLShipmentEvent[] Events { get; init; }

		public float DeclaredWeight { get; init; }

		public float Height { get; init; }

		public float Width { get; init; }

		public float Length { get; init; }

		[JsonPropertyName("volumetricWeight")] public float Weight { get; init; }

		[JsonPropertyName("plannedDeliveryTimeframe")]
		[JsonConverter(typeof(JsonDateTimeRangeStringConverter))]
		public Range<System.DateTime> EstimatedDeliveryTime { get; init; }

		public bool Equals(DHLShipment other) =>
			string.Equals(this.Id, other.Id, System.StringComparison.Ordinal);

		public override bool Equals(object? @object) =>
			(@object is DHLShipment other) && this.Equals(other);

		public override int GetHashCode() =>
			System.StringComparer.Ordinal.GetHashCode(this.Id);

		public static bool operator ==(in DHLShipment left, in DHLShipment right) =>
			left.Equals(right);

		public static bool operator !=(in DHLShipment left, in DHLShipment right) =>
			!left.Equals(right);
	}

	public readonly struct DHLShipmentContact
	{
		public required string Name { get; init; }

		public required ContactAddress Address { get; init; }

		public readonly struct ContactAddress
		{
			public required string Street { get; init; }

			public required string HouseNumber { get; init; }

			[JsonPropertyName("addition")] public string? HouseNumberAddition { get; init; }

			[JsonPropertyName("postalCode")] public required string ZipCode { get; init; }

			public required string City { get; init; }

			public required string CountryCode { get; init; }
		}
	}

	public readonly struct DHLShipmentShipper
	{
		public required string Name { get; init; }
	}

	public readonly struct DHLShipmentEvent
	{
		public required DHLShipmentPhase Category { get; init; }

		public required DHLShipmentEventStatus Status { get; init; }

		public required System.DateTime Timestamp { get; init; }

		public string? Remarks { get; init; }
	}
}
