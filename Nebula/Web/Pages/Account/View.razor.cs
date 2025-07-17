using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Nebula.Extensions;
using Nebula.Models;
using Nebula.Models.Database;
using Nebula.Models.Requests;
using Nebula.Services;

namespace Nebula.Web.Pages.Account
{
	public sealed partial class View : ComponentBase
	{
		private const string formName = "edit-account";

		[CascadingParameter] public required HttpContext HttpContext { get; init; }

		[Inject] public required IStringLocalizer<AccountLocalization> Localizer { get; init; }

		[Inject] public required IDbContextFactory<ShipmentDbContext> DbContext { get; init; }

		[Inject] public required AuthenticationService<User> Authentication { get; init; }

		[SupplyParameterFromForm(FormName = View.formName)]
		private EditAccountModel Model { get; set; } = new();

		protected override void OnInitialized()
		{
			if (!this.Model.IsValid)
			{
				this.Model = new EditAccountModel
				{
					Email = this.HttpContext.User.GetClaimValue(UserClaim.Email),
					Culture = CultureInfo.CurrentCulture.Name,
					UiCulture = CultureInfo.CurrentUICulture.Name,
					TimeZone = this.HttpContext.User.GetClaimValue(UserClaim.TimeZone),
				};
			}
		}

		private async Task UpdateAsync()
		{
			Debug.Assert(this.Model.IsValid);

			var password = this.Model.HasPassword ? Hashing.Hash(this.Model.Password) : null;

			this.Model.Password = null;
			this.Model.PasswordConfirmation = null;

			var userId = this.HttpContext.User.GetClaimValue<int>(UserClaim.Id);

			var db = await this.DbContext.CreateDbContextAsync();

			await using (db)
			{
				var updated = await db.Users
									  .WhereId(userId)
									  .ExecuteUpdateAsync((calls) =>
									   {
										   calls.SetProperty(static (u) => u.Email, this.Model.Email)
												.SetProperty(static (u) => u.Culture, this.Model.Culture)
												.SetProperty(static (u) => u.UiCulture, this.Model.UiCulture)
												.SetProperty(static (u) => u.TimeZone, this.Model.TimeZone)
												.SetProperty(static (u) => u.Password, (u) => password ?? u.Password);
									   });

				Debug.Assert(updated == 1);
			}

			await this.Authentication.LoginAsync(this.HttpContext, new User
			{
				Id = userId,
				Email = this.Model.Email,
				Password = [],
				Flags = this.HttpContext.User.GetClaimEnum<UserFlags>(UserClaim.Flags),
				Culture = this.Model.Culture,
				UiCulture = this.Model.UiCulture,
				TimeZone = this.Model.TimeZone,
				Created = this.HttpContext.User.GetClaimValue<System.DateTime>(UserClaim.Created),
			}, true);

			Cultures.AppendCultureCookie(this.HttpContext, this.Model.Culture, this.Model.UiCulture);

			// Rerenders the page properly to apply the newly selected culture.
			this.HttpContext.Response.Redirect(this.HttpContext.Request.Path);
		}
	}
}
