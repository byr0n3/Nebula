using System.Diagnostics;
using System.Threading.Tasks;
using Elegance.AspNet.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
					Username = this.HttpContext.User.GetClaimValue(UserClaim.Username),
					Email = this.HttpContext.User.GetClaimValue(UserClaim.Email),
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
										   calls.SetProperty(static (u) => u.Username, this.Model.Username)
												.SetProperty(static (u) => u.Email, this.Model.Email)
												.SetProperty(static (u) => u.Password, (u) => password ?? u.Password);
									   });

				Debug.Assert(updated == 1);
			}

			await this.Authentication.LoginAsync(this.HttpContext, new User
			{
				Id = userId,
				Username = this.Model.Username,
				Email = this.Model.Email,
				Password = [],
				Flags = this.HttpContext.User.GetClaimEnum<UserFlags>(UserClaim.Flags),
				Created = this.HttpContext.User.GetClaimValue<System.DateTime>(UserClaim.Created),
			}, true);
		}
	}
}
