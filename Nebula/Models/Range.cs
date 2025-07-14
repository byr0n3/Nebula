using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Nebula.Models
{
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct Range : System.IEquatable<Range>, System.ISpanFormattable, System.IUtf8SpanFormattable
	{
		public required System.DateTime Lower { get; init; }

		public required System.DateTime Upper { get; init; }

		[SetsRequiredMembers]
		public Range(System.DateTime lower, System.DateTime upper)
		{
			this.Lower = lower;
			this.Upper = upper;
		}

		public bool Equals(Range other) =>
			this.Lower.Equals(other.Lower) && this.Upper.Equals(other.Upper);

		public override bool Equals(object? @object) =>
			(@object is Range other) && this.Equals(other);

		public override int GetHashCode() =>
			System.HashCode.Combine(this.Lower, this.Upper);

		public static bool operator ==(Range left, Range right) =>
			left.Equals(right);

		public static bool operator !=(Range left, Range right) =>
			!left.Equals(right);

		public string ToString([StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string? format, System.IFormatProvider? provider) =>
			string.Format(provider, $"{this.Lower.ToString(format, provider)} – {this.Upper.ToString(format, provider)}");

		public bool TryFormat(System.Span<char> destination,
							  out int written,
							  [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
							  System.ReadOnlySpan<char> format,
							  System.IFormatProvider? provider)
		{
			written = 0;

			{
				var done = this.Lower.TryFormat(destination, out var writtenLower, format, provider);

				if (!done)
				{
					return false;
				}

				written += writtenLower;
			}

			{
				var done = System.MemoryExtensions.AsSpan(" – ").TryCopyTo(destination.Slice(written));

				if (!done)
				{
					return false;
				}

				written += 3;
			}

			{
				var done = this.Upper.TryFormat(destination.Slice(written), out var writtenUpper, format, provider);

				if (!done)
				{
					return false;
				}

				written += writtenUpper;
			}

			return true;
		}

		public bool TryFormat(System.Span<byte> destination,
							  out int written,
							  [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
							  System.ReadOnlySpan<char> format,
							  System.IFormatProvider? provider)
		{
			written = 0;

			{
				var done = this.Lower.TryFormat(destination, out var writtenLower, format, provider);

				if (!done)
				{
					return false;
				}

				written += writtenLower;
			}

			{
				var done = " – "u8.TryCopyTo(destination.Slice(written));

				if (!done)
				{
					return false;
				}

				written += 3;
			}

			{
				var done = this.Upper.TryFormat(destination.Slice(written), out var writtenUpper, format, provider);

				if (!done)
				{
					return false;
				}

				written += writtenUpper;
			}

			return true;
		}

		public override string ToString() =>
			string.Format(CultureInfo.CurrentCulture, $"{this.Lower:g} – {this.Upper:g}");
	}
}
