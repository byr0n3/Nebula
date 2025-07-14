using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Nebula
{
	internal static class Cultures
	{
		public const string CookieName = "Nebula.Culture";

		public static readonly CultureInfo[] Supported =
		[
			new("en-US"),
			new("nl-NL"),
		];

		public static CultureInfo Default =>
			Cultures.Supported[0];

		private static readonly CookieOptions cookieOptions = new()
		{
			SameSite = SameSiteMode.Lax,
			MaxAge = System.TimeSpan.MaxValue,
			Expires = System.DateTimeOffset.MaxValue,
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AppendCultureCookie(HttpContext? context, string culture) =>
			Cultures.AppendCultureCookie(context, new CultureInfo(culture));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AppendCultureCookie(HttpContext? context, CultureInfo info) =>
			context?.Response.Cookies.Append(
				Cultures.CookieName,
				CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(info, info)),
				Cultures.cookieOptions
			);
	}
}
