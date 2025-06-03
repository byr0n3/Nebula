using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ParcelTracker.Common
{
	/// <summary>
	/// A set of <typeparamref name="TValue"/> with a lower- and upper bound.
	/// </summary>
	/// <typeparam name="TValue">The type of the lower- and upper bound.</typeparam>
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct Range<TValue> : System.IEquatable<Range<TValue>>
		where TValue : struct, System.IEquatable<TValue>
	{
		public required TValue Lower { get; init; }

		public required TValue Upper { get; init; }

		[SetsRequiredMembers]
		public Range(TValue lower, TValue upper)
		{
			this.Lower = lower;
			this.Upper = upper;
		}

		public bool Equals(Range<TValue> other) =>
			this.Lower.Equals(other.Lower) && this.Upper.Equals(other.Upper);

		public override bool Equals(object? @object) =>
			(@object is Range<TValue> other) && this.Equals(other);

		public override int GetHashCode() =>
			System.HashCode.Combine(this.Lower, this.Upper);

		public static bool operator ==(Range<TValue> left, Range<TValue> right) =>
			left.Equals(right);

		public static bool operator !=(Range<TValue> left, Range<TValue> right) =>
			!left.Equals(right);
	}
}
