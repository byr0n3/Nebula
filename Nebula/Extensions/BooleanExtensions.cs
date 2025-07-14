using System.Runtime.CompilerServices;

namespace Nebula.Extensions
{
	internal static class BooleanExtensions
	{
		/// <summary>
		/// Converts a boolean value to its lowercase string representation.
		/// </summary>
		/// <param name="this">The boolean value to convert.</param>
		/// <returns>A string representing the boolean value ("true" or "false").</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Str(this bool @this) =>
			@this ? "true" : "false";
	}
}
