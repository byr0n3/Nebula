using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nebula.Extensions;
using Nebula.Models;

namespace Nebula
{
	public sealed class CultureMiddleware
	{
		private readonly RequestDelegate next;

		public CultureMiddleware(RequestDelegate next) =>
			this.next = next;

		public Task InvokeAsync(HttpContext context)
		{
			if (context.User.TryGetClaimValue(UserClaim.Culture, out var language))
			{
				CultureInfo.CurrentCulture = new CultureInfo(language);
				CultureInfo.CurrentUICulture = new CultureInfo(language);
			}

			if (context.User.TryGetClaimValue(UserClaim.UiCulture, out var locale))
			{
				CultureInfo.CurrentUICulture = new CultureInfo(locale);
			}

			Cultures.AppendCultureCookie(context, CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture);

			return this.next.Invoke(context);
		}
	}
}
