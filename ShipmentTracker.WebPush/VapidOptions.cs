using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace ShipmentTracker.WebPush
{
	public sealed class VapidOptions : IOptions<VapidOptions>
	{
		public string? Subject { get; init; }

		public string? PublicKey { get; init; }

		public string? PrivateKey { get; init; }

		public bool IsValid
		{
			[MemberNotNullWhen(true, nameof(this.Subject), nameof(this.PublicKey), nameof(this.PrivateKey))]
			get => (this.Subject is not null) && (this.PublicKey is not null) && (this.PrivateKey is not null);
		}

		VapidOptions IOptions<VapidOptions>.Value =>
			this;
	}
}
