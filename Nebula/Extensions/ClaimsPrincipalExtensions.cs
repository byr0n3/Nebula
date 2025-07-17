using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Elegance.AspNet.Authentication.Extensions;
using Nebula.Models;
using Nebula.Models.Database;

namespace Nebula.Extensions
{
	internal static class ClaimsPrincipalExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasUserFlag(this ClaimsPrincipal @this, UserFlags flags) =>
			((UserFlags)@this.GetClaimValue<int>(UserClaim.Flags) & flags) != UserFlags.None;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetClaimValue(this ClaimsPrincipal @this, UserClaim claimType, [NotNullWhen(true)] out string? result) =>
			@this.TryGetClaimValue(UserClaimEnumData.GetValue(claimType)!, out result);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetClaimValue<T>(this ClaimsPrincipal @this, UserClaim claimType, [NotNullWhen(true)] out T? result)
			where T : System.ISpanParsable<T> =>
			@this.TryGetClaimValue(UserClaimEnumData.GetValue(claimType)!, out result);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string? GetClaimValue(this ClaimsPrincipal @this, UserClaim claimType) =>
			@this.GetClaimValue(UserClaimEnumData.GetValue(claimType)!);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? GetClaimValue<T>(this ClaimsPrincipal @this, UserClaim claimType)
			where T : System.ISpanParsable<T> =>
			@this.GetClaimValue<T>(UserClaimEnumData.GetValue(claimType)!);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T GetClaimEnum<T>(this ClaimsPrincipal @this, UserClaim claimType)
			where T : System.Enum =>
			Unsafe.BitCast<int, T>(@this.GetClaimValue<int>(UserClaimEnumData.GetValue(claimType)!));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static System.TimeZoneInfo GetTimeZoneInfo(this ClaimsPrincipal @this) =>
			@this.TryGetClaimValue(UserClaim.TimeZone, out var value)
				? System.TimeZoneInfo.FindSystemTimeZoneById(value)
				: TimeZones.Default;
	}
}
