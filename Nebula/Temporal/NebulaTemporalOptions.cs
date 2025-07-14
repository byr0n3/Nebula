using Microsoft.Extensions.Options;

namespace Nebula.Temporal
{
	public sealed class NebulaTemporalOptions : IOptions<NebulaTemporalOptions>
	{
		public required string Host { get; init; }

		public required string Namespace { get; init; }

		NebulaTemporalOptions IOptions<NebulaTemporalOptions>.Value =>
			this;
	}
}
