using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ParcelTracker.Common
{
	/// <summary>
	/// A set of <typeparamref name="TValue"/> with a lower- and upper bound.
	/// </summary>
	/// <typeparam name="TValue">The type of the lower- and upper bound.</typeparam>
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct Range<TValue> where TValue : struct
	{
		public required TValue Lower { get; init; }

		public required TValue Upper { get; init; }

		[SetsRequiredMembers]
		public Range(TValue lower, TValue upper)
		{
			this.Lower = lower;
			this.Upper = upper;
		}
	}
}
