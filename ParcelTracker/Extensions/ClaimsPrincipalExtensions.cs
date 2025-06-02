using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Elegance.AspNet.Authentication.Extensions;
using ParcelTracker.Models;

namespace ParcelTracker.Extensions
{
	internal static class ClaimsPrincipalExtensions
	{
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
	}
}
