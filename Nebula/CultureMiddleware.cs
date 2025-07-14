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
			if (context.User.TryGetClaimValue(UserClaim.Culture, out var culture))
			{
				CultureInfo.CurrentCulture = new CultureInfo(culture);
				CultureInfo.CurrentUICulture = new CultureInfo(culture);
			}

			Cultures.AppendCultureCookie(context, CultureInfo.CurrentCulture);

			return this.next.Invoke(context);
		}
	}
}
