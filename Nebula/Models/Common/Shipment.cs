using System.Runtime.InteropServices;

namespace Nebula.Models.Common
{
	public readonly struct Shipment : System.IEquatable<Shipment>
	{
		/// <summary>
		/// Unique identifier for the shipment. This can be the shipment's bar/tracking code, but also a different unique value.
		/// </summary>
		public required string Id { get; init; }

		/// <summary>
		/// The shipment's tracking code used to find the shipment.
		/// </summary>
		public required string TrackingCode { get; init; }

		/// <summary>
		/// Indicates what delivery service is handling the shipment.
		/// </summary>
		public required ShipmentSource Source { get; init; }

		/// <summary>
		/// The last-known state of the shipment.
		/// </summary>
		public required ShipmentState State { get; init; }

		/// <summary>
		/// The type of shipment, for example, a package or letter.
		/// </summary>
		public required ShipmentType Type { get; init; }

		/// <summary>
		/// Contact information of the person who receives the package.
		/// </summary>
		public required ShipmentContact Recipient { get; init; }

		/// <summary>
		/// Contact information of the person who sent the package.
		/// </summary>
		public required ShipmentContact Sender { get; init; }

		/// <summary>
		/// The dimensions of the shipment, like size and weight.
		/// </summary>
		public required ShipmentDetails Details { get; init; }

		/// <summary>
		/// Status updates of the delivery.
		/// </summary>
		/// <remarks>This should <b>NOT</b> the current state, as this is a log of previous updates.</remarks>
		public required ShipmentEvent[]? Events { get; init; }

		/// <summary>
		/// The date and time the shipment was created/registered.
		/// </summary>
		public required System.DateTime Created { get; init; }

		/// <summary>
		/// The date and time the shipment was last updated.
		/// </summary>
		public required System.DateTime Updated { get; init; }

		/// <summary>
		/// The date and time the shipment arrived.
		/// </summary>
		/// <remarks>If <see cref="State"/> is not <see cref="ShipmentState.Delivered"/>, this value should be <see langword="default"/>.</remarks>
		public required System.DateTime Arrived { get; init; }

		/// <summary>
		/// The estimated date-time period that the shipment should arrive at.
		/// </summary>
		/// <remarks>If <see cref="State"/> is <see cref="ShipmentState.Delivered"/>, this value should be <see langword="default"/>.</remarks>
		public required Range Eta { get; init; }

		/// <summary>
		/// The amount of time the delivery is delayed.
		/// </summary>
		public required System.TimeSpan Delay { get; init; }

		/// <summary>
		/// Tracking codes that point to related shipments.
		/// </summary>
		/// <remarks>These tracking codes should be from the same delivery service, to the same address.</remarks>
		public string[]? RelatedTrackingCodes { get; init; }

		public bool Equals(Shipment other) =>
			string.Equals(this.Id, other.Id, System.StringComparison.Ordinal);

		public override bool Equals(object? @object) =>
			(@object is Shipment other) && this.Equals(other);

		public override int GetHashCode() =>
			System.StringComparer.Ordinal.GetHashCode(this.Id);

		public static bool operator ==(in Shipment left, in Shipment right) =>
			left.Equals(right);

		public static bool operator !=(in Shipment left, in Shipment right) =>
			!left.Equals(right);
	}

	/// <summary>
	/// Defines contact information about the shipment, like the recipient or the sender.
	/// </summary>
	public readonly struct ShipmentContact
	{
		/// <summary>
		/// The name of the contact.
		/// </summary>
		/// <remarks>This could be the name of a person, but also a company name.</remarks>
		public required string? Name { get; init; }

		/// <summary>
		/// The street name of the contact.
		/// </summary>
		/// <remarks>This does <b>NOT</b> contain the <see cref="HouseNumber"/>.</remarks>
		public required string Street { get; init; }

		/// <summary>
		/// The house number of the contact.
		/// </summary>
		/// <remarks>This does <b>NOT</b> contain the <see cref="HouseNumberSuffix"/>.</remarks>
		public required string HouseNumber { get; init; }

		/// <summary>
		/// The optional suffix of the contact's house number.
		/// </summary>
		public required string? HouseNumberSuffix { get; init; }

		/// <summary>
		/// The zip-code/postal-code of the contact.
		/// </summary>
		public required string ZipCode { get; init; }

		/// <summary>
		/// The living place of the contact.
		/// </summary>
		/// <remarks>This is either a town of a city.</remarks>
		public required string Place { get; init; }

		/// <summary>
		/// The country where the contact lives.
		/// </summary>
		public required Country Country { get; init; }

		public override string ToString() =>
			$"""
			 {this.Name}
			 {this.Street} {this.HouseNumber} {this.HouseNumberSuffix}
			 {this.Place} {this.ZipCode}
			 {CountryEnumData.GetValue(this.Country)}
			 """;
	}

	/// <summary>
	/// Defines the dimensions of the shipment, like it's size and weight.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct ShipmentDetails
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
		/// The length/depth of the package.
		/// </summary>
		/// <remarks>This value is in millimeters/<c>mm</c></remarks>
		public required float Length { get; init; }

		/// <summary>
		/// The weight of the package.
		/// </summary>
		/// <remarks>This value is in grams/<c>g</c></remarks>
		public required float Weight { get; init; }

		/// <summary>
		/// Formats the weight of the shipment to a human-readable string, in kilograms/<c>kg</c>.
		/// </summary>
		public string WeightKg =>
			string.Create(null, $"{float.Round(this.Weight / 1000f, 2)} kg");

		/// <summary>
		/// Formats the dimensions of the shipment to a human-readable string, in centimeters/<c>cm</c>.
		/// </summary>
		/// <remarks>This only formats the width, height and length of the shipment (in that exact order).</remarks>
		public string DimensionsCm =>
			string.Create(
				null,
				$"{float.Round(this.Width / 10f, 1)} x {float.Round(this.Height / 10f, 1)} x {float.Round(this.Length / 10f, 1)} cm"
			);

		public override string ToString() =>
			$"""
			 Weight: {this.WeightKg}
			 Dimensions: {this.DimensionsCm}
			 """;
	}

	/// <summary>
	/// Defines a previous state update of the shipment.
	/// </summary>
	public readonly struct ShipmentEvent : System.IEquatable<ShipmentEvent>
	{
		/// <summary>
		/// The new state of the shipment.
		/// </summary>
		public required ShipmentEventType State { get; init; }

		/// <summary>
		/// The date and time when this event/update happened.
		/// </summary>
		public required System.DateTime Timestamp { get; init; }

		/// <summary>
		/// A delivery service provided-message about the update.
		/// </summary>
		/// <remarks>This message is (normally) localized in the language given to the request.</remarks>
		public string? Description { get; init; }

		public long Key =>
			(long)this.State ^ this.Timestamp.Ticks;

		public bool Equals(ShipmentEvent other) =>
			(this.State == other.State) && this.Timestamp.Equals(other.Timestamp);

		public override bool Equals(object? @object) =>
			(@object is ShipmentEvent other) && this.Equals(other);

		public override int GetHashCode() =>
			System.HashCode.Combine(this.State, this.Timestamp);

		public static bool operator ==(ShipmentEvent left, ShipmentEvent right) =>
			left.Equals(right);

		public static bool operator !=(ShipmentEvent left, ShipmentEvent right) =>
			!left.Equals(right);
	}
}
