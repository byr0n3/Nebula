using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Http;
using ShipmentTracker.Models.Database;

namespace ShipmentTracker.Services
{
	public sealed class AuthenticationService
	{
		public delegate void OnUserChanged(ClaimsPrincipal? user);

		public event OnUserChanged? UserChanged;

		public ClaimsPrincipal? User
		{
			get;
			set
			{
				field = value;
				this.UserChanged?.Invoke(value);
			}
		}

		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly AuthenticationService<User> authentication;

		public AuthenticationService(IHttpContextAccessor httpContextAccessor, AuthenticationService<User> authentication)
		{
			this.authentication = authentication;
			this.httpContextAccessor = httpContextAccessor;

			this.User = this.httpContextAccessor.HttpContext?.User;
		}

		public async Task AuthenticateAsync(User user, bool persistent)
		{
			var httpContext = this.httpContextAccessor.HttpContext;

			Debug.Assert(httpContext is not null);

			await this.authentication.LoginAsync(httpContext, user, persistent).ConfigureAwait(false);

			this.User = httpContext.User;
		}

		public Task DeauthenticateAsync()
		{
			var httpContext = this.httpContextAccessor.HttpContext;

			Debug.Assert(httpContext is not null);

			return this.authentication.LogoutAsync(httpContext);
		}
	}
}
