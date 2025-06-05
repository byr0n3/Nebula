using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace ShipmentTracker.PostNL.Models
{
	public readonly struct PostNLShipment : System.IEquatable<PostNLShipment>
	{
		[JsonPropertyName("identification")] public required string Id { get; init; }

		[JsonPropertyName("barcode")] public required string TrackingCode { get; init; }

		public required PostNLShipmentContext Context { get; init; }

		public PostNLShipmentContact Recipient { get; init; }

		// @todo Is the same as `Recipient`? If so, we can remove this duplicate.
		public PostNLShipmentContact DeliveryAddress { get; init; }

		public PostNLShipmentContact Sender { get; init; }

		[JsonPropertyName("effectiveDate")] public System.DateTime Created { get; init; }

		[JsonPropertyName("lastObservation")] public System.DateTime Updated { get; init; }

		public System.DateTime? DeliveryDate { get; init; }

		[JsonPropertyName("eta")] public PostNLShipmentEstimatedTimeOfArrival EstimatedTimeOfArrival { get; init; }

		public bool IsDelivered { get; init; }

		/// <summary>
		/// Whether the shipment has a signature of the receiver.
		/// </summary>
		/// <remarks>
		/// This property is <see langword="true"/> if the delivery has a signature from the receiver.
		/// </remarks>
		public bool HasProofOfDelivery { get; init; }

		public required PostNLShipmentDetails Details { get; init; }

		[JsonPropertyName("statusPhase")] public required PostNLShipmentStatusPhase Status { get; init; }

		public required PostNLShipmentInternationalStatus InternationalStatus { get; init; }

		public required PostNLShipmentObservation[] Observations { get; init; }

		[JsonPropertyName("relatedCollos")] public PostNLRelatedShipment[]? Related { get; init; }

		public bool Equals(PostNLShipment other) =>
			string.Equals(this.Id, other.Id, System.StringComparison.Ordinal);

		public override bool Equals(object? @object) =>
			(@object is PostNLShipment other) && this.Equals(other);

		public override int GetHashCode() =>
			this.Id.GetHashCode(System.StringComparison.Ordinal);

		public static bool operator ==(in PostNLShipment left, in PostNLShipment right) =>
			left.Equals(right);

		public static bool operator !=(in PostNLShipment left, in PostNLShipment right) =>
			!left.Equals(right);
	}

	public readonly struct PostNLShipmentContext
	{
		[JsonPropertyName("shipmentType")] public required PostNLShipmentType Type { get; init; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public readonly struct PostNLShipmentContact
	{
		[JsonPropertyName("names")] public required ContactName Name { get; init; }

		public required ContactAddress Address { get; init; }

		public override string ToString() =>
			$"""
			 {this.Name.PersonName}
			 {this.Address}
			 """;

		public readonly struct ContactName
		{
			public string? PersonName { get; init; }

			public string? CompanyName { get; init; }

			public string Name =>
				this.CompanyName ?? this.PersonName ?? "–";
		}

		public readonly struct ContactAddress
		{
			public required string Street { get; init; }

			public required string HouseNumber { get; init; }

			public required string? HouseNumberSuffix { get; init; }

			[JsonPropertyName("postalCode")] public required string ZipCode { get; init; }

			public required string Town { get; init; }

			[JsonPropertyName("country")] public required string CountryCode { get; init; }

			public override string ToString() =>
				$"""
				 {this.Street} {this.HouseNumber} {this.HouseNumberSuffix}
				 {this.Town} {this.ZipCode}
				 {this.CountryCode}
				 """;
		}
	}

	public readonly struct PostNLShipmentEstimatedTimeOfArrival
	{
		// @todo Enum
		public required string Type { get; init; }

		public required System.DateTime Start { get; init; }

		public required System.DateTime End { get; init; }

		public required int? MinutesOverdue { get; init; }
	}

	public readonly struct PostNLShipmentDetails
	{
		public required ShipmentDimensions? Dimensions { get; init; }

		[StructLayout(LayoutKind.Sequential)]
		public readonly struct ShipmentDimensions
		{
			/// <summary>
			/// The height of the package.
			/// </summary>
			/// <remarks>This value is in millimeters/<c>mm</c></remarks>
			public required float Height { get; init; }

			/// <summary>
			/// The width of the package.
			/// </summary>
			/// <remarks>This value is in millimeters/<c>mm</c></remarks>
			public required float Width { get; init; }

			/// <summary>
			/// The depth of the package.
			/// </summary>
			/// <remarks>This value is in millimeters/<c>mm</c></remarks>
			public required float Depth { get; init; }

			/// <summary>
			/// The weight of the package.
			/// </summary>
			/// <remarks>This value is in grams/<c>g</c></remarks>
			public required float Weight { get; init; }

			public override string ToString() =>
				$"""
				 {float.Round(this.Weight / 1000f, 1)} kg
				 {float.Round(this.Width / 10f, 1)} x {float.Round(this.Height / 10f, 1)} x {float.Round(this.Depth / 10f, 1)} cm
				 """;
		}
	}

	public readonly struct PostNLShipmentStatusPhase
	{
		[JsonPropertyName("index")] public required PostNLPhaseType Type { get; init; }

		/// <summary>
		/// The status message of the shipment.
		/// </summary>
		/// <remarks>This message will be localized in the language given to the URL using the search parameters.</remarks>
		public required string Message { get; init; }

		// @todo `route`?
	}

	public readonly struct PostNLShipmentInternationalStatus
	{
		public required bool IsForeignShipment { get; init; }

		// @todo Enum
		public required string Phase { get; init; }

		// @todo `delay`?
	}

	public readonly struct PostNLShipmentObservation
	{
		[JsonPropertyName("observationDate")] public required System.DateTime Date { get; init; }

		[JsonPropertyName("observationCode")] public required PostNLShipmentObservationCode Code { get; init; }

		/// <summary>
		/// The description detailing the observation, describing what happened during this observation.
		/// </summary>
		/// <remarks>This message will be localized in the language given to the URL using the search parameters.</remarks>
		public required string Description { get; init; }
	}

	public readonly struct PostNLRelatedShipment
	{
		public required ShipmentIdentification Identification { get; init; }

		public readonly struct ShipmentIdentification
		{
			[JsonPropertyName("barcode")] public required string TrackingCode { get; init; }
		}
	}
}
