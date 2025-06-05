using System.Diagnostics.CodeAnalysis;

namespace ShipmentTracker.Common.Models
{
	public readonly struct ShipmentRequest
	{
		public required string Code { get; init; }

		public required string ZipCode { get; init; }

		public Country Country { get; init; }

		public Language Language { get; init; }

		public ShipmentSource Source { get; init; }

		[SetsRequiredMembers]
		public ShipmentRequest(string code, string zipCode, Country country = default, Language language = default)
		{
			this.Code = code;
			this.ZipCode = zipCode;
			this.Country = country;
			this.Language = language;
		}
	}
}
