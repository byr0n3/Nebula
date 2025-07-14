using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Logging;

namespace Nebula.Services
{
	public sealed class AuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
	{
		private static readonly ClaimsPrincipal defaultUser = new();

		protected override System.TimeSpan RevalidationInterval =>
			System.TimeSpan.FromMinutes(15);

		private readonly AuthenticationService authenticationService;

		public AuthenticationStateProvider(ILoggerFactory loggerFactory, AuthenticationService authenticationService) : base(loggerFactory)
		{
			this.authenticationService = authenticationService;

			this.OnAuthenticationChanged(this.authenticationService.User);

			this.authenticationService.UserChanged += this.OnAuthenticationChanged;
		}

		private void OnAuthenticationChanged(ClaimsPrincipal? user) =>
			this.SetAuthenticationState(Task.FromResult(new AuthenticationState(user ?? AuthenticationStateProvider.defaultUser)));

		protected override Task<bool> ValidateAuthenticationStateAsync(AuthenticationState state, CancellationToken token) =>
			// @todo Validate somehow
			Task.FromResult(false);

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.authenticationService.UserChanged -= this.OnAuthenticationChanged;
			}

			base.Dispose(disposing);
		}
	}
}
