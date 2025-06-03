using System.Diagnostics;
using System.Threading.Tasks;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ParcelTracker.Database;
using ParcelTracker.Database.Models;
using ParcelTracker.Models.Requests;

namespace ParcelTracker.Web.Pages.Auth
{
	public sealed partial class Login : ComponentBase
	{
		private const string formName = "login";

		[CascadingParameter] public required HttpContext HttpContext { get; init; }

		[Inject] public required IDbContextFactory<ShipmentDbContext> DbFactory { get; init; }

		[Inject] public required AuthenticationService<User> Authentication { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		[SupplyParameterFromQuery] public string? ReturnUrl { get; init; }

		[SupplyParameterFromForm(FormName = Login.formName)]
		private LoginModel Model { get; set; } = new();

		private async Task LoginAsync()
		{
			Debug.Assert(this.Model.IsValid);

			User? user;

			var db = await this.DbFactory.CreateDbContextAsync(this.HttpContext.RequestAborted);

			await using (db)
			{
				user = await this.Model
								 .GetQuery(db)
								 .FirstOrDefaultAsync(this.HttpContext.RequestAborted);
			}

			// User- and password validation happens in `LoginModel`.
			Debug.Assert(user is not null);

			await this.Authentication.LoginAsync(this.HttpContext, user, this.Model.Persistent);

			var path = new PathString(this.ReturnUrl);

			this.Navigation.NavigateTo(path.HasValue ? path.ToUriComponent() : "/account", true);
		}
	}
}
