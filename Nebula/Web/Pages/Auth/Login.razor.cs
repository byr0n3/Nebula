using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Nebula.Models.Database;
using Nebula.Models.Requests;
using Nebula.Services;

namespace Nebula.Web.Pages.Auth
{
	public sealed partial class Login : ComponentBase
	{
		private const string formName = "login";

		[Inject] public required IStringLocalizer<LoginLocalization> Localizer { get; init; }

		[Inject] public required IDbContextFactory<ShipmentDbContext> DbFactory { get; init; }

		[Inject] public required AuthenticationService Authentication { get; init; }

		[Inject] public required NavigationManager Navigation { get; init; }

		[SupplyParameterFromQuery] public string? ReturnUrl { get; init; }

		[SupplyParameterFromForm(FormName = Login.formName)]
		private LoginModel Model { get; set; } = new();

		private async Task LoginAsync()
		{
			Debug.Assert(this.Model.IsValid);

			User? user;

			var db = await this.DbFactory.CreateDbContextAsync();

			await using (db)
			{
				user = await this.Model.GetQuery(db).FirstOrDefaultAsync();
			}

			// User- and password validation happens in `LoginModel`.
			Debug.Assert(user is not null);

			await this.Authentication.AuthenticateAsync(user, this.Model.Persistent);

			var path = new PathString(this.ReturnUrl);

			this.Navigation.NavigateTo(path.HasValue ? path.ToUriComponent() : "/shipments", true);
		}
	}
}
